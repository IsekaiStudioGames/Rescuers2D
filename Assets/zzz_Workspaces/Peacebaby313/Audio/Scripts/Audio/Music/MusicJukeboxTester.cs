//----- MusicJukeboxTester.cs START -----

using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class MusicJukeboxTester : MonoBehaviour
{
    [Header("Test Tracks")]
    [SerializeField]
    private MusicTrackData firstTrack;

    [SerializeField]
    private MusicTrackData secondTrack;

    [Header("Test Settings")]
    [Range(0.01f, 3f)]
    [SerializeField]
    private float testPlaybackSpeed = 0.75f;

    private MusicJukebox Jukebox
    {
        get
        {
            if (ApplicationBootstrap.Instance == null)
                return null;

            return ApplicationBootstrap.Instance.MusicJukebox;
        }
    }

    private void Update()
    {
        Keyboard keyboard =
            Keyboard.current;

        if (keyboard == null)
            return;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            PlayFirstTrack();
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            PlaySecondTrack();
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            RestartCurrentTrack();
        }

        if (keyboard.pKey.wasPressedThisFrame)
        {
            PauseMusic();
        }

        if (keyboard.oKey.wasPressedThisFrame)
        {
            ResumeMusic();
        }

        if (keyboard.sKey.wasPressedThisFrame)
        {
            StopMusic();
        }

        if (keyboard.iKey.wasPressedThisFrame)
        {
            StopMusicImmediately();
        }

        if (keyboard.vKey.wasPressedThisFrame)
        {
            EnableReverse();
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            EnableForward();
        }

        if (keyboard.lKey.wasPressedThisFrame)
        {
            ToggleLoop();
        }

        if (keyboard.tKey.wasPressedThisFrame)
        {
            ApplyTestSpeed();
        }

        if (keyboard.nKey.wasPressedThisFrame)
        {
            RestoreNormalSpeed();
        }
    }

    public void PlayFirstTrack()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.Play(
            firstTrack);
    }

    public void PlaySecondTrack()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.Play(
            secondTrack);
    }

    public void RestartCurrentTrack()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.Restart();
    }

    public void PauseMusic()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.Pause();
    }

    public void ResumeMusic()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.Resume();
    }

    public void StopMusic()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.Stop();
    }

    public void StopMusicImmediately()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.StopImmediate();
    }

    public void EnableReverse()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.SetReverse(
            true);
    }

    public void EnableForward()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.SetReverse(
            false);
    }

    public void ToggleLoop()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        bool shouldLoop =
            !jukebox.ActiveSource.loop;

        jukebox.SetLoop(
            shouldLoop);

        Debug.Log(
            $"[MUSIC TESTER] Loop: {shouldLoop}",
            this);
    }

    public void ApplyTestSpeed()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.SetPlaybackSpeed(
            testPlaybackSpeed);
    }

    public void RestoreNormalSpeed()
    {
        if (!TryGetJukebox(
                out MusicJukebox jukebox))
        {
            return;
        }

        jukebox.SetPlaybackSpeed(
            1f);
    }

    private bool TryGetJukebox(
        out MusicJukebox jukebox)
    {
        jukebox =
            Jukebox;

        if (jukebox != null)
            return true;

        Debug.LogWarning(
            "[MUSIC TESTER] MusicJukebox is unavailable.",
            this);

        return false;
    }
}

//----- MusicJukeboxTester.cs END -----