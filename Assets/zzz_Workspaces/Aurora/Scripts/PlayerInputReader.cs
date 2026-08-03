using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{


    private CharacterActions inputActions;

    [SerializeField] private FirefighterController fireFighterController;
    [SerializeField] private RescueSpecialistController rescueSpecialistController;
    [SerializeField] private RiotOfficerController riotOfficerController;
    [SerializeField]
    private CharacterCameraFollow2D characterCamera;

    [Header("Universal Interaction")]
    [SerializeField]
    private RescuerInteractor2D firefighterInteractor;

    [SerializeField]
    private RescuerInteractor2D riotOfficerInteractor;

    [SerializeField]
    private RescuerInteractor2D specialistInteractor;

    private bool wasPressingUp;

    public enum ActiveCharacter { Firefighter, RiotOfficer, Specialist }
    private ActiveCharacter currentCharacter = ActiveCharacter.Specialist;

    private Vector2 moveDirection;


    public ActiveCharacter CurrentCharacter =>
    currentCharacter;

    public Transform ActiveCharacterTransform
    {
        get
        {
            return currentCharacter switch
            {
                ActiveCharacter.Firefighter =>
                    fireFighterController != null
                        ? fireFighterController.transform
                        : null,

                ActiveCharacter.RiotOfficer =>
                    riotOfficerController != null
                        ? riotOfficerController.transform
                        : null,

                ActiveCharacter.Specialist =>
                    rescueSpecialistController != null
                        ? rescueSpecialistController.transform
                        : null,

                _ => null
            };
        }
    }

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
            if (currentCharacter == ActiveCharacter.Firefighter) fireFighterController.UseLadder();
        };

        inputActions.Controls.firefighter_axe.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Firefighter) fireFighterController.UseAxe();
        };
        inputActions.Controls.firefighter_extend.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Firefighter)
            {
                fireFighterController.UseLadderExtension();
            }
        };
        inputActions.Controls.Interact.performed +=
            HandleInteract;

        //riot moves
        inputActions.Controls.riot_shield.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficerController.SetShield(true);
            }
        };

        inputActions.Controls.riot_shield.canceled += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficerController.SetShield(false);
            }
        };

        inputActions.Controls.riot_bash.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficerController.Bash();
            }
        };

        inputActions.Controls.riot_brace.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficerController.SetBrace(true);
            }
        };

        inputActions.Controls.riot_brace.canceled += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficerController.SetBrace(false);
            }
        };

        inputActions.Controls.riot_c4.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.RiotOfficer)
            {
                riotOfficerController.PlaceC4();
            }
        };


        //specialist moves
        inputActions.Controls.specialist_crawl.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Specialist)
            {
                rescueSpecialistController.Crawl();
            }
        };
        inputActions.Controls.specialist_jump.performed += ctx =>
        {
            if (currentCharacter == ActiveCharacter.Specialist)
            {
                rescueSpecialistController.Jump();
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
                fireFighterController.Move(moveDirection);

                bool isPessingUp = moveDirection.y > 0.5f;
                if(isPessingUp && !fireFighterController.IsClimbing)
                {
                    fireFighterController.StartClimbing();
                }
                wasPressingUp = isPessingUp;
                break;
            case ActiveCharacter.RiotOfficer:
                riotOfficerController.Move(moveDirection);
                break;
            case ActiveCharacter.Specialist:
                rescueSpecialistController.Move(moveDirection);
                break;
        }
    }
    private void HandleMoveInput()
    {
        moveDirection =
            inputActions.Controls.Move.ReadValue<Vector2>();



    }

    private void HandleInteract(
    InputAction.CallbackContext context)
    {
        RescuerInteractor2D activeInteractor =
            currentCharacter switch
            {
                ActiveCharacter.Firefighter =>
                    firefighterInteractor,

                ActiveCharacter.RiotOfficer =>
                    riotOfficerInteractor,

                ActiveCharacter.Specialist =>
                    specialistInteractor,

                _ => null
            };

        if (activeInteractor == null)
        {
            Debug.LogWarning(
                $"{nameof(PlayerInputReader)} has no interactor " +
                $"assigned for {currentCharacter}.",
                this);

            return;
        }

        activeInteractor.Interact();
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
                fireFighterController.Move(Vector2.zero);
                break;

            case ActiveCharacter.RiotOfficer:
                riotOfficerController.Move(Vector2.zero);
                riotOfficerController.SetShield(false);
                riotOfficerController.SetBrace(false);
                break;

            case ActiveCharacter.Specialist:
                rescueSpecialistController.Move(Vector2.zero);
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



        Transform target = ActiveCharacterTransform;
        

        if (target != null)
        {
            characterCamera.SetTarget(target);
        }
    }

    public bool IsAnyCharacterWithinDistance(
    Vector2 position,
    float distance)
    {
        float squaredDistance = distance * distance;

        return IsWithinDistance(
                   fireFighterController != null
                       ? fireFighterController.transform
                       : null,
                   position,
                   squaredDistance) ||
               IsWithinDistance(
                   riotOfficerController != null
                       ? riotOfficerController.transform
                       : null,
                   position,
                   squaredDistance) ||
               IsWithinDistance(
                   rescueSpecialistController != null
                       ? rescueSpecialistController.transform
                       : null,
                   position,
                   squaredDistance);
    }

    private bool IsWithinDistance(
        Transform character,
        Vector2 position,
        float squaredDistance)
    {
        if (character == null)
        {
            return false;
        }

        Vector2 difference =
            (Vector2)character.position - position;

        return difference.sqrMagnitude <= squaredDistance;
    }
    public void SetGameplayInputEnabled(
    bool enabled)
    {
        if (inputActions == null)
            return;

        StopCurrentCharacterMovement();

        if (enabled)
        {
            inputActions.Controls.Enable();
        }
        else
        {
            inputActions.Controls.Disable();
        }
    }

    private void OnDestroy()
    {
        if (inputActions == null)
        {
            return;
        }

        inputActions.Controls.Interact.performed -=
            HandleInteract;

        inputActions.Dispose();
    }
}
