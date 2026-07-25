//----- IPauseGameAuthority.cs START -----

using System;

public interface IPauseGameAuthority
{
    event Action<bool> PauseStateChanged;

    bool IsPaused
    {
        get;
    }

    void SetPaused(bool paused);
}

//----- IPauseGameAuthority.cs END -----