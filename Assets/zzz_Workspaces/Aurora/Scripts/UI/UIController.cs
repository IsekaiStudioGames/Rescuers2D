using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : Singleton<UIController>, IPauseGameAuthority {
    public bool IsPaused { get; set; }

    public event Action<bool> PauseStateChanged;

    [SerializeField] private GameObject pauseMenu;

    public void SetPaused(bool paused) {
        Debug.Log("Pause start");
        pauseMenu.SetActive(paused);
    }
}