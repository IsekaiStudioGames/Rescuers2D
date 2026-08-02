//----- DeverQuestEditorAudioBridge.cs START -----

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestEditorAudioBridge
    {
        private const BindingFlags Flags =
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        private static readonly Type AudioUtilType =
            typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");

        public static bool IsAvailable => AudioUtilType != null;

        public static bool VolumeSupported =>
            FindMethod(
                "SetPreviewClipVolume",
                "SetClipVolume") != null;

        public static bool PlaybackStatusSupported =>
            FindMethod(
                "IsPreviewClipPlaying",
                "IsClipPlaying") != null;

        public static bool Play(
            AudioClip clip,
            bool loop,
            float volume)
        {
            if (clip == null || AudioUtilType == null)
            {
                return false;
            }

            MethodInfo method = FindMethod(
                "PlayPreviewClip",
                "PlayClip");

            if (method == null)
            {
                return false;
            }

            try
            {
                Stop();
                method.Invoke(
                    null,
                    BuildArguments(method, clip, loop, volume));

                SetVolume(clip, volume);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[DeverQuest] Editor preview playback failed: " +
                    exception.GetBaseException().Message);

                return false;
            }
        }

        public static void Pause()
        {
            InvokeNoArgument(
                "PausePreviewClip",
                "PauseAllClips");
        }

        public static void Resume()
        {
            InvokeNoArgument(
                "ResumePreviewClip",
                "ResumeAllClips");
        }

        public static void Stop()
        {
            InvokeNoArgument(
                "StopAllPreviewClips",
                "StopAllClips");
        }

        public static bool IsPlaying(AudioClip clip)
        {
            MethodInfo method = FindMethod(
                "IsPreviewClipPlaying",
                "IsClipPlaying");

            if (method == null)
            {
                return false;
            }

            ParameterInfo[] parameters = method.GetParameters();
            object[] arguments = parameters.Length == 0
                ? Array.Empty<object>()
                : new object[] { clip };

            try
            {
                object result = method.Invoke(null, arguments);
                return result is bool isPlaying && isPlaying;
            }
            catch
            {
                return false;
            }
        }

        public static void SetVolume(
            AudioClip clip,
            float volume)
        {
            MethodInfo method = FindMethod(
                "SetPreviewClipVolume",
                "SetClipVolume");

            if (method == null)
            {
                return;
            }

            try
            {
                method.Invoke(
                    null,
                    BuildArguments(
                        method,
                        clip,
                        false,
                        Mathf.Clamp01(volume)));
            }
            catch
            {
                // Preview volume is optional across Unity editor versions.
            }
        }

        private static MethodInfo FindMethod(params string[] names)
        {
            if (AudioUtilType == null)
            {
                return null;
            }

            return AudioUtilType
                .GetMethods(Flags)
                .FirstOrDefault(
                    method => names.Contains(method.Name));
        }

        private static object[] BuildArguments(
            MethodInfo method,
            AudioClip clip,
            bool loop,
            float volume)
        {
            ParameterInfo[] parameters = method.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                Type parameterType =
                    parameters[index].ParameterType;

                if (parameterType == typeof(AudioClip))
                {
                    arguments[index] = clip;
                }
                else if (parameterType == typeof(bool))
                {
                    arguments[index] = loop;
                }
                else if (parameterType == typeof(int))
                {
                    arguments[index] = 0;
                }
                else if (parameterType == typeof(float))
                {
                    arguments[index] = volume;
                }
                else
                {
                    arguments[index] =
                        parameters[index].HasDefaultValue
                            ? parameters[index].DefaultValue
                            : Activator.CreateInstance(parameterType);
                }
            }

            return arguments;
        }

        private static void InvokeNoArgument(params string[] names)
        {
            MethodInfo method = FindMethod(names);

            if (method == null)
            {
                return;
            }

            try
            {
                method.Invoke(null, Array.Empty<object>());
            }
            catch
            {
                // Internal preview method signatures vary by Unity version.
            }
        }
    }
}

//----- DeverQuestEditorAudioBridge.cs END -----
