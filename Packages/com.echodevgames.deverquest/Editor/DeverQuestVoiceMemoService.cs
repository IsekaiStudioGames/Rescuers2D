using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestVoiceMemoService
    {
        private const int SampleRate = 44100;
        private const int MaximumRecordingSeconds = 300;

        private static AudioClip recordingClip;
        private static string recordingDevice = string.Empty;
        private static DateTime recordingStartedUtc;

        static DeverQuestVoiceMemoService()
        {
            AssemblyReloadEvents.beforeAssemblyReload -=
                CancelRecording;
            AssemblyReloadEvents.beforeAssemblyReload +=
                CancelRecording;
        }

        public static bool IsRecording =>
            recordingClip != null;

        public static string[] Devices =>
            Microphone.devices ?? Array.Empty<string>();

        public static double RecordingSeconds =>
            IsRecording
                ? Math.Max(
                    0d,
                    (DateTime.UtcNow - recordingStartedUtc)
                    .TotalSeconds)
                : 0d;

        public static bool Start(
            string device,
            out string message)
        {
            message = string.Empty;
            if (IsRecording)
            {
                message = "A voice memo is already recording.";
                return false;
            }
            if (!DeverQuestSessionStore.HasActiveSession)
            {
                message = "Begin a Quest before recording a voice memo.";
                return false;
            }

            string[] devices = Devices;
            if (devices.Length == 0)
            {
                message =
                    "Unity did not detect a microphone. Check operating " +
                    "system microphone permissions.";
                return false;
            }

            recordingDevice =
                string.IsNullOrWhiteSpace(device)
                    ? devices[0]
                    : device;
            try
            {
                recordingClip = Microphone.Start(
                    recordingDevice,
                    false,
                    MaximumRecordingSeconds,
                    SampleRate);
                if (recordingClip == null)
                {
                    message = "Unity could not start the microphone.";
                    recordingDevice = string.Empty;
                    return false;
                }
                recordingStartedUtc = DateTime.UtcNow;
                message = "Voice memo recording started.";
                return true;
            }
            catch (Exception exception)
            {
                recordingClip = null;
                recordingDevice = string.Empty;
                message =
                    "Microphone recording failed: " +
                    exception.Message;
                return false;
            }
        }

        public static bool StopAndAttach(
            string memoName,
            out string message)
        {
            message = string.Empty;
            if (!IsRecording)
            {
                message = "No voice memo is recording.";
                return false;
            }

            AudioClip source = recordingClip;
            string device = recordingDevice;
            int samplePosition = Math.Max(
                0,
                Microphone.GetPosition(device));
            Microphone.End(device);
            recordingClip = null;
            recordingDevice = string.Empty;

            if (samplePosition <= 0)
            {
                UnityEngine.Object.DestroyImmediate(source);
                message = "The microphone produced no audio samples.";
                return false;
            }

            int channels = Math.Max(1, source.channels);
            float[] samples =
                new float[samplePosition * channels];
            if (!source.GetData(samples, 0))
            {
                UnityEngine.Object.DestroyImmediate(source);
                message = "Unity could not read the recorded samples.";
                return false;
            }

            try
            {
                DeverQuestProfile profile =
                    DeverQuestSettingsStore.Profile;
                string mediaFolder =
                    DeverQuestPathUtility.GetMediaFolder(
                        profile.timecardRootPath,
                        profile.developerName,
                        DateTime.Now);
                Directory.CreateDirectory(mediaFolder);
                string safeName =
                    DeverQuestPathUtility.MakeSafeFolderName(
                        string.IsNullOrWhiteSpace(memoName)
                            ? "Voice_Memo"
                            : memoName);
                string fileName =
                    $"{DateTime.Now:HHmmss}_{safeName}.wav";
                string path =
                    GetUniquePath(mediaFolder, fileName);
                WriteWave(
                    path,
                    samples,
                    channels,
                    source.frequency);
                double duration =
                    samplePosition /
                    (double)Math.Max(1, source.frequency);
                DeverQuestSessionStore.AddMediaAttachment(
                    new DeverQuestMediaAttachment
                    {
                        attachmentId =
                            Guid.NewGuid().ToString("N"),
                        attachmentType = "Voice Memo",
                        displayName =
                            Path.GetFileNameWithoutExtension(path),
                        filePath = path,
                        createdUtcTicks = DateTime.UtcNow.Ticks,
                        durationSeconds = duration
                    });
                message =
                    $"Voice memo attached: {Path.GetFileName(path)}";
                return true;
            }
            catch (Exception exception)
            {
                message =
                    "Voice memo save failed: " +
                    exception.Message;
                return false;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(source);
            }
        }

        public static bool AttachExistingFile(
            string sourcePath,
            out string message)
        {
            message = string.Empty;
            if (!DeverQuestSessionStore.HasActiveSession)
            {
                message = "Begin a Quest before attaching media.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(sourcePath) ||
                !File.Exists(sourcePath))
            {
                message = "Choose an existing media file.";
                return false;
            }

            try
            {
                DeverQuestProfile profile =
                    DeverQuestSettingsStore.Profile;
                string mediaFolder =
                    DeverQuestPathUtility.GetMediaFolder(
                        profile.timecardRootPath,
                        profile.developerName,
                        DateTime.Now);
                Directory.CreateDirectory(mediaFolder);
                string destination =
                    GetUniquePath(
                        mediaFolder,
                        Path.GetFileName(sourcePath));
                File.Copy(sourcePath, destination, false);
                DeverQuestSessionStore.AddMediaAttachment(
                    new DeverQuestMediaAttachment
                    {
                        attachmentId =
                            Guid.NewGuid().ToString("N"),
                        attachmentType = "Media File",
                        displayName =
                            Path.GetFileName(destination),
                        filePath = destination,
                        createdUtcTicks = DateTime.UtcNow.Ticks
                    });
                message =
                    $"Media attached: {Path.GetFileName(destination)}";
                return true;
            }
            catch (Exception exception)
            {
                message =
                    "Media attachment failed: " +
                    exception.Message;
                return false;
            }
        }

        public static void CancelRecording()
        {
            if (!IsRecording)
            {
                return;
            }

            try
            {
                Microphone.End(recordingDevice);
            }
            catch
            {
                // The device may already have stopped during shutdown.
            }
            UnityEngine.Object.DestroyImmediate(recordingClip);
            recordingClip = null;
            recordingDevice = string.Empty;
        }

        private static string GetUniquePath(
            string folder,
            string fileName)
        {
            string path = Path.Combine(folder, fileName);
            string name =
                Path.GetFileNameWithoutExtension(fileName);
            string extension =
                Path.GetExtension(fileName);
            int suffix = 2;
            while (File.Exists(path))
            {
                path = Path.Combine(
                    folder,
                    $"{name}_{suffix}{extension}");
                suffix++;
            }
            return path;
        }

        private static void WriteWave(
            string path,
            float[] samples,
            int channels,
            int frequency)
        {
            const short bitsPerSample = 16;
            int dataLength =
                samples.Length * sizeof(short);
            using (FileStream stream =
                   new FileStream(path, FileMode.CreateNew))
            using (BinaryWriter writer =
                   new BinaryWriter(stream))
            {
                writer.Write(
                    Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataLength);
                writer.Write(
                    Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(
                    Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(frequency);
                writer.Write(
                    frequency * channels *
                    bitsPerSample / 8);
                writer.Write(
                    (short)(channels * bitsPerSample / 8));
                writer.Write(bitsPerSample);
                writer.Write(
                    Encoding.ASCII.GetBytes("data"));
                writer.Write(dataLength);
                foreach (float sample in samples)
                {
                    short value = (short)Mathf.RoundToInt(
                        Mathf.Clamp(sample, -1f, 1f) *
                        short.MaxValue);
                    writer.Write(value);
                }
            }
        }
    }
}
