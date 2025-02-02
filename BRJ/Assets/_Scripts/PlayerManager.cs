using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;
    AnimatorHandler animatorHandler;

    InteractableUI interactableUI;
    GameObject interactableUIGameObject;
    public ItemPickup pickup;

    public bool isInteracting;

    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;
    public bool canDoCombo;


    // Start is called before the first frame update
    void Start()
    {
        cameraHandler = CameraHandler.singleton;
        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        cameraHandler = FindObjectOfType<CameraHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        interactableUI = FindObjectOfType<InteractableUI>();
        interactableUIGameObject = interactableUI.transform.GetChild(0).gameObject;
        interactableUIGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");


        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);

        //CheckForInteractableObject();
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
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;
        inputHandler.d_Pad_Right = false;
        inputHandler.d_Pad_Left = false;
        inputHandler.d_Pad_Up = false;
        inputHandler.d_Pad_Down = false;
        inputHandler.e_Input = false;

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }

    public void CheckForInteractableObject()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                if (interactableObject != null)
                {
                    string interactableText = interactableObject.interactableText;
                    // set the UI text to the interactable objects text
                    // set the text pop up to true
                    interactableUI.interactableText.text = interactableText;
                    interactableUIGameObject.SetActive(true);

                    if (inputHandler.e_Input)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
        else
        {
            if (interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Debug.Log("Entered Item");
            pickup = other.GetComponent<ItemPickup>();
            if (interactableUIGameObject != null)
            {
                Interactable interactableObject = other.GetComponent<Interactable>();
                interactableUI.interactableText.text = interactableObject.interactableText;
                interactableUI.SetTextActive(true);
            }
        }
    }

    public void UseItem()
    {
        if (pickup != null)
        {
            inputHandler.e_Input = false;
            pickup.Interact(this);
            Destroy(pickup.gameObject);
            string statText = pickup.statIncrease.ToString() + " Increased!";
            interactableUI.SetTextInactiveOnTimer(1, statText);
            animatorHandler.PlayTargetAnimation("Pick Up Item", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            pickup = null;
            Debug.Log("Exited Item");
            if (interactableUIGameObject != null)
            {
                interactableUI.SetTextActive(false);
            }

        }
    }
}
