using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;

    public bool isInteracting;

    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {
        cameraHandler = CameraHandler.singleton;
        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");



        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        isSprinting = inputHandler.b_Input;

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }
}