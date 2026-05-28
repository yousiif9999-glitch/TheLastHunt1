using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Footsteps")]
    public AudioSource footstepAudio;

    public AudioClip walkSound;
    public AudioClip runSound;

    private CharacterController controller;
    private Vector3 velocity;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        yRotation = transform.eulerAngles.y;

        transform.rotation =
            Quaternion.Euler(0f, yRotation, 0f);

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                Quaternion.Euler(0f, 0f, 0f);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        if (footstepAudio != null)
        {
            footstepAudio.playOnAwake = false;
            footstepAudio.loop = true;
            footstepAudio.spatialBlend = 0f;
            footstepAudio.volume = 0.15f;
        }
    }

    void Update()
    {

        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity *
            Time.deltaTime;

        yRotation += mouseX;

        transform.rotation =
            Quaternion.Euler(0f, yRotation, 0f);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                Quaternion.Euler(xRotation, 0f, 0f);
        }


        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

  
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        bool isMoving = x != 0 || z != 0;

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift);

        float speed =
            isRunning ? runSpeed : walkSpeed;

        Vector3 move =
            transform.right * x +
            transform.forward * z;

        controller.Move(
            move.normalized *
            speed *
            Time.deltaTime
        );


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );


        HandleFootsteps(isRunning, isMoving);
    }

    void HandleFootsteps(bool isRunning, bool isMoving)
    {
        if (
            !isMoving ||
            !isGrounded ||
            footstepAudio == null
        )
        {
            if (footstepAudio != null &&
                footstepAudio.isPlaying)
            {
                footstepAudio.Stop();
            }

            return;
        }

        AudioClip targetClip =
            isRunning ? runSound : walkSound;

        if (targetClip == null)
            return;

        if (footstepAudio.clip != targetClip)
        {
            footstepAudio.clip = targetClip;

            footstepAudio.pitch =
                Random.Range(0.95f, 1.05f);

            footstepAudio.Play();
        }

        if (!footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
    }
}