using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{

    private CharacterActions inputActions;

    [SerializeField] private FirefighterController firefighter;
    [SerializeField] private RescueSpecialistController rescueSpecialist;
    [SerializeField] private RiotOfficerController riotOfficer;
    [SerializeField]
    private CharacterCameraFollow2D characterCamera;

    private bool wasPressingUp;

    private enum ActiveCharacter { Firefighter, RiotOfficer, Specialist }
    private ActiveCharacter currentCharacter = ActiveCharacter.Firefighter;

    private Vector2 moveDirection;

    private void OnEnable()
    {
        inputActions.Controls.Enable();
    }
    private void OnDisable()
    {
        inputActions.Controls.Disable();
    }
    private void Start()
    {
        UpdateCameraTarget();
    }
    private void Awake()
    {

        inputActions = new();


        //universal moves
        inputActions.Controls.SwitchCharacters.performed += SwitchCharacters();












        //firefighter moves

        inputActions.Controls.firefighter_ladder.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Firefighter) firefighter.UseLadder();
        };

        inputActions.Controls.firefighter_axe.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Firefighter) firefighter.UseAxe();
        };
        inputActions.Controls.firefighter_extend.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Firefighter)
            {
                firefighter.UseLadderExtension();
            }
        };
        inputActions.Controls.Interact.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Firefighter)
            {
                firefighter.Interact();
            }
        };


        //riot moves
        inputActions.Controls.riot_shield.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficer.SetShield(true);
            }
        };

        inputActions.Controls.riot_shield.canceled += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficer.SetShield(false);
            }
        };

        inputActions.Controls.riot_bash.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficer.Bash();
            }
        };

        inputActions.Controls.riot_brace.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficer.SetBrace(true);
            }
        };

        inputActions.Controls.riot_brace.canceled += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficer.SetBrace(false);
            }
        };


        //specialist moves
        inputActions.Controls.specialist_crawl.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Specialist)
            {
                rescueSpecialist.Crawl();
            }
        };
        inputActions.Controls.specialist_jump.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Specialist)
            {
                rescueSpecialist.Jump();
            }
        };



        if (characterCamera == null)
        {
            characterCamera =
                FindFirstObjectByType<CharacterCameraFollow2D>();
        }
    }
    private void Update()
    {
        HandleMoveInput();

        switch (currentCharacter)
        {
            case ActiveCharacter.Firefighter:
                firefighter.Move(moveDirection);

                bool isPessingUp = moveDirection.y > 0.5f;
                if(isPessingUp && !wasPressingUp)
                {
                    firefighter.StartClimbing();
                }
                wasPressingUp = isPessingUp;
                break;
            case ActiveCharacter.RiotOfficer:
                riotOfficer.Move(moveDirection);
                break;
            case ActiveCharacter.Specialist:
                rescueSpecialist.Move(moveDirection);
                break;
        }
    }
    private void HandleMoveInput()
    {
        moveDirection =
            inputActions.Controls.Move.ReadValue<Vector2>();

    }
    private Action<InputAction.CallbackContext> SwitchCharacters()
    {
        return ctx =>
        {
            float value = ctx.ReadValue<float>();



            StopCurrentCharacterMovement();

            int totalCharacters = Enum.GetValues(typeof(ActiveCharacter)).Length;
            int currentIndex = (int)currentCharacter;

            if (value > 0)
            {
                currentIndex = (currentIndex + 1) % totalCharacters;
            }
            else if (value < 0)
            {
                currentIndex = (currentIndex - 1 + totalCharacters) % totalCharacters;
            }
            currentCharacter = (ActiveCharacter)currentIndex;
            UpdateCameraTarget();
        };
    }
    private void StopCurrentCharacterMovement()
    {
        switch (currentCharacter)
        {
            case ActiveCharacter.Firefighter:
                firefighter.Move(Vector2.zero);
                break;

            case ActiveCharacter.RiotOfficer:
                riotOfficer.Move(Vector2.zero);
                riotOfficer.SetShield(false);
                riotOfficer.SetBrace(false);
                break;

            case ActiveCharacter.Specialist:
                rescueSpecialist.Move(Vector2.zero);
                break;
        }
    }
    private void UpdateCameraTarget()
    {
        if (characterCamera == null)
        {
            characterCamera =
                FindFirstObjectByType<CharacterCameraFollow2D>();
        }

        if (characterCamera == null)
        {
            Debug.LogWarning(
                $"{nameof(PlayerInputReader)} could not find a " +
                $"{nameof(CharacterCameraFollow2D)}.",
                this);

            return;
        }

        Transform target = currentCharacter switch
        {
            ActiveCharacter.Firefighter =>
                firefighter != null
                    ? firefighter.transform
                    : null,

            ActiveCharacter.RiotOfficer =>
                riotOfficer != null
                    ? riotOfficer.transform
                    : null,

            ActiveCharacter.Specialist =>
                rescueSpecialist != null
                    ? rescueSpecialist.transform
                    : null,

            _ => null
        };

        if (target != null)
        {
            characterCamera.SetTarget(target);
        }
    }

}
