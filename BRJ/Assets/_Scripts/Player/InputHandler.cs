using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool b_Input;
    public bool rb_Input;
    public bool rt_Input;
    public bool e_Input;
    public bool lockOnInput;

    public bool d_Pad_Up;
    public bool d_Pad_Down;
    public bool d_Pad_Left;
    public bool d_Pad_Right;


    public bool rollFlag;
    public bool sprintFlag;
    public bool comboFlag;
    public float rollInputTimer;
    public bool lockOnFlag;

    PlayerControls inputActions;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;
    PlayerManager playerManager;
    CameraHandler cameraHandler;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
        cameraHandler = FindObjectOfType<CameraHandler>();
    }


    public void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            inputActions.PlayerActions.RB.started += i => rb_Input = true;
            inputActions.PlayerActions.RT.started += i => rt_Input = true;
            inputActions.PlayerActions.Interact.started += i => playerManager.UseItem();

            inputActions.PlayerActions.Roll.started += i => b_Input = true;
            inputActions.PlayerActions.Roll.canceled += i => b_Input = false;

            inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        HandleMoveInput(delta);
        //HandleRollInput(delta);
        HandleAttackInput(delta);
        //HandleQuickSlotInput();
        HandleLockOnInput();
    }

    private void HandleMoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;

    }

    public void HandleRollInput(float delta)
    {
        if (b_Input)
        {
            rollInputTimer += delta;
            sprintFlag = true;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.2f)
            {
                sprintFlag = false;
                rollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    private void HandleAttackInput(float delta)
    {
        // RB input handlesw the right hands weapons light attack
        if (rb_Input)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon, false);
                comboFlag = false;
            }
            else
            {
                if (playerManager.canDoCombo)
                    return;
                if (playerManager.isInteracting)
                    return;
                playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
            }

        }

        if (rt_Input)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon, true);
                comboFlag = false;
            }
            else
            {
                if (playerManager.canDoCombo)
                    return;
                if (playerManager.isInteracting)
                    return;
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
            }
        }
    }

    private void HandleQuickSlotInput()
    {
        inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
        inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;
        if (d_Pad_Right)
        {
            playerInventory.ChangeRightWeapon();
        }
        if (d_Pad_Left)
        {
            playerInventory.ChangeLeftWeapon();
        }
    }

    private void HandleLockOnInput()
    {
        if (lockOnInput && lockOnFlag == false)
        {
            cameraHandler.ClearLockOnTargets();
            lockOnInput = false;


            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentlockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
            }

        }
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            //clear lock on targets
            cameraHandler.ClearLockOnTargets();


        }
        cameraHandler.SetCameraHeight();
    }
}
