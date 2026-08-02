//----- DeverQuestGitService.cs START -----

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestGitStatus
    {
        public bool GitAvailable;
        public bool IsRepository;
        public string UnityProjectRoot = string.Empty;
        public string RepositoryRoot = string.Empty;
        public string Branch = string.Empty;
        public string HeadHash = string.Empty;
        public string ShortHash = string.Empty;
        public string HeadSubject = string.Empty;
        public long HeadCommitUnixSeconds;
        public string UpstreamBranch = string.Empty;
        public int AheadCount;
        public int BehindCount;
        public bool HasOriginRemote;
        public int StagedCount;
        public int UnstagedCount;
        public int UntrackedCount;
        public string Error = string.Empty;

        public bool HasStagedChanges => StagedCount > 0;
        public bool IsClean =>
            StagedCount == 0 &&
            UnstagedCount == 0 &&
            UntrackedCount == 0;
    }

    internal sealed class DeverQuestGitResult
    {
        public bool Succeeded;
        public string Output = string.Empty;
        public string Error = string.Empty;
    }

    internal static class DeverQuestGitService
    {
        private const int CommandTimeoutMilliseconds = 30000;

        public static DeverQuestGitStatus Refresh()
        {
            ResolveRoots(
                out string projectRoot,
                out string searchRoot);
            return RefreshResolved(projectRoot, searchRoot);
        }

        internal static void ResolveRoots(
            out string projectRoot,
            out string searchRoot)
        {
            projectRoot = Path.GetFullPath(
                Path.Combine(Application.dataPath, ".."));
            string configuredRoot =
                DeverQuestSettingsStore.Profile
                    .gitRepositoryOverridePath;
            searchRoot =
                !string.IsNullOrWhiteSpace(configuredRoot) &&
                Directory.Exists(configuredRoot)
                    ? configuredRoot
                    : projectRoot;
        }

        internal static DeverQuestGitStatus RefreshResolved(
            string projectRoot,
            string searchRoot)
        {
            DeverQuestGitResult rootResult = Run(
                searchRoot,
                "rev-parse",
                "--show-toplevel");

            if (!rootResult.Succeeded)
            {
                return new DeverQuestGitStatus
                {
                    GitAvailable =
                        rootResult.Error.IndexOf(
                            "could not start",
                            StringComparison.OrdinalIgnoreCase) < 0,
                    IsRepository = false,
                    UnityProjectRoot = projectRoot,
                    Error = rootResult.Error
                };
            }

            string repositoryRoot = rootResult.Output.Trim();
            DeverQuestGitStatus status = new DeverQuestGitStatus
            {
                GitAvailable = true,
                IsRepository = true,
                UnityProjectRoot = projectRoot,
                RepositoryRoot = repositoryRoot
            };

            DeverQuestGitResult branch = Run(
                repositoryRoot,
                "branch",
                "--show-current");
            DeverQuestGitResult hash = Run(
                repositoryRoot,
                "rev-parse",
                "HEAD");
            DeverQuestGitResult shortHash = Run(
                repositoryRoot,
                "rev-parse",
                "--short",
                "HEAD");
            DeverQuestGitResult subject = Run(
                repositoryRoot,
                "log",
                "-1",
                "--pretty=%s");
            DeverQuestGitResult timestamp = Run(
                repositoryRoot,
                "log",
                "-1",
                "--pretty=%ct");
            DeverQuestGitResult changes = Run(
                repositoryRoot,
                "status",
                "--porcelain");
            DeverQuestGitResult upstream = Run(
                repositoryRoot,
                "rev-parse",
                "--abbrev-ref",
                "--symbolic-full-name",
                "@{u}");
            DeverQuestGitResult origin = Run(
                repositoryRoot,
                "remote",
                "get-url",
                "origin");

            status.Branch = branch.Succeeded
                ? branch.Output.Trim()
                : string.Empty;
            status.HeadHash = hash.Succeeded
                ? hash.Output.Trim()
                : string.Empty;
            status.ShortHash = shortHash.Succeeded
                ? shortHash.Output.Trim()
                : string.Empty;
            status.HeadSubject = subject.Succeeded
                ? subject.Output.Trim()
                : string.Empty;
            if (timestamp.Succeeded)
            {
                long.TryParse(
                    timestamp.Output.Trim(),
                    out status.HeadCommitUnixSeconds);
            }
            status.UpstreamBranch = upstream.Succeeded
                ? upstream.Output.Trim()
                : string.Empty;
            status.HasOriginRemote = origin.Succeeded;

            if (!string.IsNullOrWhiteSpace(status.UpstreamBranch))
            {
                DeverQuestGitResult divergence = Run(
                    repositoryRoot,
                    "rev-list",
                    "--left-right",
                    "--count",
                    $"HEAD...{status.UpstreamBranch}");
                if (divergence.Succeeded)
                {
                    string[] counts = divergence.Output.Split(
                        new[] { ' ', '\t' },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (counts.Length >= 2)
                    {
                        int.TryParse(counts[0], out status.AheadCount);
                        int.TryParse(counts[1], out status.BehindCount);
                    }
                }
            }

            if (!changes.Succeeded)
            {
                status.Error = changes.Error;
                return status;
            }

            ParseStatus(changes.Output, status);
            return status;
        }

        internal static bool TryGetHeadSnapshot(
            out string repositoryRoot,
            out string headHash)
        {
            repositoryRoot = string.Empty;
            headHash = string.Empty;
            string projectRoot = Path.GetFullPath(
                Path.Combine(Application.dataPath, ".."));
            string configuredRoot =
                DeverQuestSettingsStore.Profile
                    .gitRepositoryOverridePath;
            string searchRoot =
                !string.IsNullOrWhiteSpace(configuredRoot) &&
                Directory.Exists(configuredRoot)
                    ? configuredRoot
                    : projectRoot;
            DeverQuestGitResult root = Run(
                searchRoot,
                "rev-parse",
                "--show-toplevel");
            if (!root.Succeeded)
            {
                return false;
            }
            repositoryRoot = root.Output.Trim();
            DeverQuestGitResult head = Run(
                repositoryRoot,
                "rev-parse",
                "HEAD");
            if (!head.Succeeded)
            {
                return false;
            }
            headHash = head.Output.Trim();
            return !string.IsNullOrWhiteSpace(headHash);
        }

        public static DeverQuestGitResult CommitStaged(
            string repositoryRoot,
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return new DeverQuestGitResult
                {
                    Error = "Enter a commit message first."
                };
            }

            return Run(
                repositoryRoot,
                "commit",
                "-m",
                message.Trim());
        }

        public static DeverQuestGitResult StageAll(
            string repositoryRoot)
        {
            return Run(repositoryRoot, "add", "-A");
        }

        public static DeverQuestGitResult Push(
            string repositoryRoot)
        {
            return Run(repositoryRoot, "push");
        }

        public static DeverQuestGitResult PublishBranch(
            string repositoryRoot,
            string branch)
        {
            if (string.IsNullOrWhiteSpace(branch))
            {
                return new DeverQuestGitResult
                {
                    Error =
                        "A detached HEAD cannot be published as a branch."
                };
            }

            return Run(
                repositoryRoot,
                "push",
                "--set-upstream",
                "origin",
                branch);
        }

        private static void ParseStatus(
            string output,
            DeverQuestGitStatus status)
        {
            using (StringReader reader = new StringReader(output))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length < 2)
                    {
                        continue;
                    }

                    char indexState = line[0];
                    char workTreeState = line[1];

                    if (indexState == '?' && workTreeState == '?')
                    {
                        status.UntrackedCount++;
                        continue;
                    }

                    if (indexState != ' ')
                    {
                        status.StagedCount++;
                    }

                    if (workTreeState != ' ')
                    {
                        status.UnstagedCount++;
                    }
                }
            }
        }

        private static DeverQuestGitResult Run(
            string workingDirectory,
            params string[] arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = BuildArguments(arguments),
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };
                startInfo.EnvironmentVariables["GIT_TERMINAL_PROMPT"] = "0";
                startInfo.EnvironmentVariables["GCM_INTERACTIVE"] = "Never";

                using (Process process = new Process
                       {
                           StartInfo = startInfo
                       })
                {
                    StringBuilder outputBuilder = new StringBuilder();
                    StringBuilder errorBuilder = new StringBuilder();
                    process.OutputDataReceived += (_, eventArgs) =>
                    {
                        if (eventArgs.Data != null)
                        {
                            outputBuilder.AppendLine(eventArgs.Data);
                        }
                    };
                    process.ErrorDataReceived += (_, eventArgs) =>
                    {
                        if (eventArgs.Data != null)
                        {
                            errorBuilder.AppendLine(eventArgs.Data);
                        }
                    };

                    if (!process.Start())
                    {
                        return new DeverQuestGitResult
                        {
                            Error = "Git could not start."
                        };
                    }

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (!process.WaitForExit(
                            CommandTimeoutMilliseconds))
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                            // The process may have exited between checks.
                        }

                        return new DeverQuestGitResult
                        {
                            Error =
                                "Git timed out after 30 seconds. The command " +
                                "was stopped; try the operation in GitHub " +
                                "Desktop and then refresh DeverQuest."
                        };
                    }

                    // Flush the final asynchronous output events.
                    process.WaitForExit();
                    string output = outputBuilder.ToString().Trim();
                    string error = errorBuilder.ToString().Trim();

                    return new DeverQuestGitResult
                    {
                        Succeeded = process.ExitCode == 0,
                        Output = output.Trim(),
                        Error = process.ExitCode == 0
                            ? string.Empty
                            : string.IsNullOrWhiteSpace(error)
                                ? output.Trim()
                                : error.Trim()
                    };
                }
            }
            catch (Exception exception)
            {
                return new DeverQuestGitResult
                {
                    Error =
                        $"Git could not start: {exception.Message}"
                };
            }
        }

        private static string BuildArguments(
            IReadOnlyList<string> arguments)
        {
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < arguments.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(QuoteArgument(arguments[index]));
            }

            return builder.ToString();
        }

        private static string QuoteArgument(string value)
        {
            value = value ?? string.Empty;
            return "\"" +
                   value.Replace("\"", "\\\"") +
                   "\"";
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestGitMonitor
    {
        private const long UnixEpochTicks = 621355968000000000L;
        private const string RepositoryKey =
            "EchoDevGames.DeverQuest.Git.ObservedRepository";
        private const string HeadKey =
            "EchoDevGames.DeverQuest.Git.ObservedHead";

        private static double nextCheckTime;
        private static Task<DeverQuestGitStatus> pendingRefresh;
        public static DeverQuestGitStatus LatestStatus
        {
            get;
            private set;
        }

        static DeverQuestGitMonitor()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public static void MarkObserved(DeverQuestGitStatus status)
        {
            if (status == null || !status.IsRepository)
            {
                return;
            }

            EditorPrefs.SetString(
                RepositoryKey,
                status.RepositoryRoot);
            EditorPrefs.SetString(HeadKey, status.HeadHash);
        }

        public static void SetLatestStatus(
            DeverQuestGitStatus status)
        {
            LatestStatus = status;
        }

        private static void Update()
        {
            if (pendingRefresh != null)
            {
                if (!pendingRefresh.IsCompleted)
                {
                    return;
                }

                Task<DeverQuestGitStatus> completed = pendingRefresh;
                pendingRefresh = null;

                if (completed.IsFaulted || completed.IsCanceled)
                {
                    return;
                }

                ProcessBackgroundStatus(completed.Result);
                return;
            }

            if (EditorApplication.timeSinceStartup < nextCheckTime)
            {
                return;
            }

            nextCheckTime =
                EditorApplication.timeSinceStartup + 15d;

            if (!DeverQuestSessionStore.HasActiveSession)
            {
                return;
            }

            DeverQuestGitService.ResolveRoots(
                out string projectRoot,
                out string searchRoot);

            // Git commands can wait on credential helpers, antivirus, large
            // repositories, or another Git process. Running the automatic
            // monitor off Unity's main thread prevents EditorApplication
            // update from freezing the entire Editor.
            pendingRefresh = Task.Run(() =>
                DeverQuestGitService.RefreshResolved(
                    projectRoot,
                    searchRoot));
        }

        private static void ProcessBackgroundStatus(
            DeverQuestGitStatus status)
        {
            if (status == null || !status.IsRepository)
            {
                LatestStatus = status;
                return;
            }

            LatestStatus = status;
            string observedRepository =
                EditorPrefs.GetString(RepositoryKey, string.Empty);
            string observedHead =
                EditorPrefs.GetString(HeadKey, string.Empty);

            if (observedRepository != status.RepositoryRoot ||
                string.IsNullOrWhiteSpace(observedHead))
            {
                MarkObserved(status);
                TryRecordFirstCommitDuringQuest(status);
                return;
            }

            if (observedHead == status.HeadHash)
            {
                return;
            }

            MarkObserved(status);
            if (DeverQuestSessionStore.HasActiveSession)
            {
                string subject = string.IsNullOrWhiteSpace(
                    status.HeadSubject)
                    ? "Git commit detected"
                    : status.HeadSubject;

                DeverQuestSessionStore.AddCommitEntry(
                    subject,
                    status.Branch,
                    status.ShortHash,
                    "Git Commit");
            }
        }

        private static void TryRecordFirstCommitDuringQuest(
            DeverQuestGitStatus status)
        {
            if (!DeverQuestSessionStore.HasActiveSession ||
                status.HeadCommitUnixSeconds <= 0)
            {
                return;
            }

            long sessionUnixSeconds =
                (DeverQuestSessionStore.ActiveSession.startedUtcTicks -
                 UnixEpochTicks) /
                TimeSpan.TicksPerSecond;

            if (status.HeadCommitUnixSeconds <
                sessionUnixSeconds - 2)
            {
                return;
            }

            DeverQuestSessionStore.AddCommitEntry(
                string.IsNullOrWhiteSpace(status.HeadSubject)
                    ? "Initial Git commit detected"
                    : status.HeadSubject,
                status.Branch,
                status.ShortHash,
                "Git Commit");
        }
    }
}

//----- DeverQuestGitService.cs END -----
