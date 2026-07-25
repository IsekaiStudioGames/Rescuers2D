using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour {

    private CharacterActions inputActions;

    [SerializeField] private FirefighterController firefighter;
    [SerializeField] private RescueSpecialistController rescueSpecialist;
    [SerializeField] private RiotOfficerController riotOfficer;

    private enum ActiveCharacter { Firefighter, RiotOfficer, Specialist }
    private ActiveCharacter currentCharacter = ActiveCharacter.Firefighter;

    private Vector2 moveDirection;

    private void OnEnable() {
        inputActions.Controls.Enable(); 
    }
    private void OnDisable() {
        inputActions.Controls.Disable();
    }

    private void Awake() {

        inputActions = new();

        inputActions.Controls.SwitchCharacters.performed += SwitchCharacters();
        inputActions.Controls.firefighter_ladder.performed += ctx => {
            if (currentCharacter == ActiveCharacter.Firefighter) firefighter.UseLadder();
        };
        inputActions.Controls.firefighter_axe.performed += ctx => {
            if (currentCharacter == ActiveCharacter.Firefighter) firefighter.UseAxe();
        };
        inputActions.Controls.riot_shield.performed += ctx => {
            if (currentCharacter == ActiveCharacter.RiotOfficer) riotOfficer.ToggleShield();
        };
        inputActions.Controls.riot_shield.canceled += ctx => {
            if (currentCharacter == ActiveCharacter.RiotOfficer) riotOfficer.ToggleShield();
        };
        inputActions.Controls.specialist_jump.performed += ctx => {
            if (currentCharacter == ActiveCharacter.Specialist) rescueSpecialist.Crawl();
        };
        inputActions.Controls.specialist_jump.performed += ctx => {
            if (currentCharacter == ActiveCharacter.Specialist) rescueSpecialist.Jump();
        };
        inputActions.Controls.specialist_jump.performed += ctx => {
            if (currentCharacter == ActiveCharacter.Specialist) rescueSpecialist.Swim();
        };
    }
    private void Update() {
        HandleMoveInput();

        switch(currentCharacter) {
            case ActiveCharacter.Firefighter:
                firefighter.Move(moveDirection);
                break;
            case ActiveCharacter.RiotOfficer:
                riotOfficer.Move(moveDirection);
                break;
            case ActiveCharacter.Specialist:
                rescueSpecialist.Move(moveDirection);
                break;
        }
    }
    private void HandleMoveInput() {
        moveDirection = inputActions.Controls.Move.ReadValue<Vector2>();
    }
    private Action<InputAction.CallbackContext> SwitchCharacters() {
        return ctx => {
            float value = ctx.ReadValue<float>();

            Debug.Log($"[Input Router] Switch Characters, axis read {value}");

            if (value == 0) {
                Debug.LogWarning($"axis is at: {value}");
            }

            StopCurrentCharacterMovement();

            int totalCharacters = Enum.GetValues(typeof(ActiveCharacter)).Length;
            int currentIndex = (int)currentCharacter;

            if (value > 0) {
                currentIndex = (currentIndex + 1) % totalCharacters;
            } else if (value < 0) { 
                currentIndex = (currentIndex - 1 + totalCharacters) % totalCharacters;
            }
            currentCharacter = (ActiveCharacter)currentIndex;
        };
    }
    private void StopCurrentCharacterMovement() {
        // Important: Prevents a character from sliding forever if you swap mid-run
        switch (currentCharacter) {
            case ActiveCharacter.Firefighter: firefighter.Move(Vector2.zero); break;
            case ActiveCharacter.RiotOfficer: riotOfficer.Move(Vector2.zero); break;
            case ActiveCharacter.Specialist: rescueSpecialist.Move(Vector2.zero); break;
        }
    }
}