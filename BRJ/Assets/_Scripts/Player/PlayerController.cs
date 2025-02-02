/*using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private PlayerInput input;
    private Animator animator;

    [SerializeField] private CameraManager cameraManager;

    private float velocityZ = 0f;
    private float velocityX = 0f;

    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float deceleration = 2.0f;
    [SerializeField] private float maxRunVelocity = 2f;
    [SerializeField] private float maxWalkVelocity = 0.5f;

    [SerializeField] private float speed = 5;
    [SerializeField] private float rollSpeed = 5;

    private Vector2 currentMovement = Vector2.zero;
    private Vector2 lastMove = Vector2.zero;

    // increase performance
    int VelocityZHash;
    int VelocityXHash;
    int isRollingHash;

    private bool rollPressed = false;
    private bool movementPressed = false;
    private bool runPressed = false;

    [SerializeField] private Vector3 characterPos;
    private Transform CharacterTransform;

    private void Awake()
    {
        // set the singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Debug.Log("Too many character movements");
            Destroy(gameObject);
        }
        CharacterTransform = transform.GetChild(0);
        characterPos = CharacterTransform.localPosition;


        input = new PlayerInput();

        // get the animator of the 
        animator = transform.GetChild(0).GetComponent<Animator>();

        VelocityZHash = Animator.StringToHash("VelocityZ");
        VelocityXHash = Animator.StringToHash("VelocityX");
        isRollingHash = Animator.StringToHash("isRolling");

        input.Player.Move.ReadValue<Vector2>();
        input.Player.Move.performed += ctx =>
        {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
            if (movementPressed)
            {
                lastMove = currentMovement;
            }
        };

        input.Player.Move.canceled += ctx =>
        {
            movementPressed = false;
            currentMovement = Vector2.zero;
        };

        input.Player.Run.performed += ctx => runPressed = true;
        input.Player.Run.canceled += ctx => runPressed = false;

        // when the roll button is pressed, set rollPressed to true and start the rolling animation
        input.Player.Roll.performed += ctx =>
        {
            rollPressed = true;
            animator.SetBool(isRollingHash, true);
            // rotate the player to the correct direction
        };
    }

    void FixedUpdate()
    {
        // get key input from the player
        bool forwardPressed = currentMovement.y > 0;
        bool backwardsPressed = currentMovement.y < 0;
        bool leftPressed = currentMovement.x < 0;
        bool rightPressed = currentMovement.x > 0;
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        // set current maxVelocity
        float currentMaxVelocity = runPressed ? maxRunVelocity : maxWalkVelocity;

        LockedMovement();

        LockOrResetVelocity(forwardPressed, backwardsPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
        ChangeVelocity(forwardPressed, backwardsPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);

        Move();
    }

    private void LockedMovement()
    {
        transform.rotation = Quaternion.Euler(0, cameraManager.CurrentCameraRotation.y, 0);
    }

    private void Move()
    {
        // normalize movement
        Vector2 normalizedMove = currentMovement.normalized;
        transform.Translate(speed * velocityX * Time.deltaTime * Mathf.Abs(normalizedMove.x), 0, speed * velocityZ * Time.deltaTime * Mathf.Abs(normalizedMove.y));
    }

    // handles acceleration and deceleration
    private void ChangeVelocity(bool forwardPressed, bool backwardsPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // moving forward
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        // moving backwards
        if (backwardsPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        // moving Left
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        // moving right
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        // decelerate the player's forward velocity
        if (!forwardPressed && velocityZ > 0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        //decelerate the player's backwards velocity
        if (!backwardsPressed && velocityZ < 0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        // decelerate the player's left velocity
        if (!leftPressed && velocityX < 0)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        // decelerate the player's right velocity
        if (!rightPressed && velocityX > 0)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    // locks or resets velocity
    private void LockOrResetVelocity(bool forwardPressed, bool backwardsPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        if (!forwardPressed && !backwardsPressed && velocityZ != 0f && (velocityZ > -0.05f && velocityZ < 0.05f))
        {
            velocityZ = 0f;
        }

        // reset velocityX
        if (!leftPressed && !rightPressed && velocityX != 0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0f;
        }


        // lock forward velocity
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ > (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }


        // lock backwards velocity
        if (backwardsPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
            Debug.Log("1");
        }
        // decelerate to the maximum walk velocity
        else if (backwardsPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            Debug.Log(-currentMaxVelocity);
            Debug.Log("velocity: " + (velocityZ < -currentMaxVelocity));
            if (velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05f))
            {
                Debug.Log("2");
                velocityZ = -currentMaxVelocity;
            }
        }
        else if (backwardsPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
            Debug.Log("3");
        }


        // lock left velocity
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }


        // lock right velocity
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

}
*/