using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float maxHealth = 10.0f;
    public float health;

    [Header("Physics")]
    public float jumpForce = 22.5f;
    public float baseSpeed = 10.0f;
    [HideInInspector]
    public float speed = 10.0f;
    [HideInInspector]
    public Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f); //How fast we are going.
    // player will use its own gravity for jump physics.
    public float gravityScale = 2.5f;
    public static float globalGravity = -9.81f;

    [Header("Jump Checking & Sound")]
    public Transform groundCheck;
    public float groundDistance = .5f;
    public LayerMask groundMask;
    private bool isGrounded;
    public AudioSource jumpSound;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // get rigidbody component, use custom gravity.
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        // jump button. only jump when the player is touching the ground.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // gravity.
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);

        // movement.
        float translation = Input.GetAxis("Vertical");
        float straffe = Input.GetAxis("Horizontal");
        this.velocity = new Vector3(straffe, 0, translation);

        float movementMagnitudeSquared = this.velocity.sqrMagnitude;

        if (movementMagnitudeSquared > 1.0f)
        {

            this.velocity /= Mathf.Sqrt(movementMagnitudeSquared);

        }

        this.velocity *= speed;

        //transform.Translate(movement);
        transform.position += this.velocity * Time.deltaTime;

        // check for jump availability.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    }

    void Jump()
    {
        rb.AddForce(new Vector3(0, 1, 0) * jumpForce, ForceMode.Impulse);
    }

}
