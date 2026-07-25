using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterType
{
    Firefighter,
    RiotOfficer,
    RescueSpecialist
}

public class PlayerController : MonoBehaviour
{
    [Header("Current Character")]
    public CharacterType activeCharacter;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float climbSpeed = 3f;
    public float crawlSpeed = 2f;

    [Header("State")]
    public bool isPaused;
    public bool onLadder;
    public bool inWater;
    public bool inConfinedSpace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            PauseControls();
            return;
        }

        GameplayControls();
    }

    private void GameplayControls()
    {
        HandleMovement();

        if (Keyboard.current.fKey.isPressed)
            UseSelectedItem();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Activate();

        if (Keyboard.current.jKey.isPressed)
            Ability1();

        if (Keyboard.current.kKey.isPressed)
            Ability2();

        if (Keyboard.current.qKey.isPressed)
            PreviousCharacter();

        if (Keyboard.current.eKey.isPressed)
            NextCharacter();

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            PauseGame();
    }

    private void HandleMovement()
    {
        float horizontal = 0;

        if (Keyboard.current.aKey.isPressed)
            horizontal = -1;

        if (Keyboard.current.dKey.isPressed)
            horizontal = 1;

        transform.Translate(Vector3.right * horizontal * moveSpeed * Time.deltaTime);

        if (Keyboard.current.wKey.isPressed && onLadder)
        {
            transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
        }

        if (Keyboard.current.sKey.isPressed)
        {
            if (onLadder)
            {
                transform.Translate(Vector3.down * climbSpeed * Time.deltaTime);
            }
            else if (activeCharacter == CharacterType.RescueSpecialist &&
                     inConfinedSpace)
            {
                Crawl();
            }
        }
    }

    private void Ability1()
    {
        switch (activeCharacter)
        {
            case CharacterType.Firefighter:
                ToggleLadder();
                break;

            case CharacterType.RiotOfficer:
                ToggleShield();
                break;

            case CharacterType.RescueSpecialist:
                Jump();
                break;
        }
    }

    private void Ability2()
    {
        switch (activeCharacter)
        {
            case CharacterType.Firefighter:
                AxeAttack();
                break;

            case CharacterType.RiotOfficer:
                StunBaton();
                break;

            case CharacterType.RescueSpecialist:
                JetSwim();
                break;
        }
    }

    private void PauseControls()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
            ResumeGame();

        if (Keyboard.current.enterKey.wasPressedThisFrame)
            OpenQuitConfirmation();
    }

    #region Gameplay Actions

    void UseSelectedItem()
    {
        Debug.Log("Use Item");
    }

    void Activate()
    {
        Debug.Log("Activate");
    }

    void PreviousCharacter()
    {
        Debug.Log("Previous Character");
    }

    void NextCharacter()
    {
        Debug.Log("Next Character");
    }

    void PauseGame()
    {
        isPaused = true;
        Debug.Log("Paused");
    }

    void ResumeGame()
    {
        isPaused = false;
        Debug.Log("Resumed");
    }

    void OpenQuitConfirmation()
    {
        Debug.Log("Quit Confirmation");
    }

    #endregion

    #region Character Abilities

    void ToggleLadder()
    {
        Debug.Log("Place / Retrieve Ladder");
    }

    void AxeAttack()
    {
        Debug.Log("Axe Attack");
    }

    void ToggleShield()
    {
        Debug.Log("Shield Toggle");
    }

    void StunBaton()
    {
        Debug.Log("Stun Baton");
    }

    void Jump()
    {
        if (!onLadder)
            Debug.Log("Jump");

        if (inWater)
            Debug.Log("Swim Up");
    }

    void JetSwim()
    {
        if (inWater)
            Debug.Log("Jet Swim");
    }

    void Crawl()
    {
        transform.Translate(Vector3.right * crawlSpeed * Time.deltaTime);
    }

    #endregion
}
