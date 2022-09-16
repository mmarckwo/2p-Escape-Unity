using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public CharacterController controller;
    public float originalStepOffset;

    [Header("Health Properties")]
    public float maxHealth = 10.0f;
    [HideInInspector]
    public float health;

    private GameObject healthBar;
    private Image healthBarFill;
    public Color goodHealth = new Color(69, 255, 137);
    public Color lowHealth = new Color(255, 0, 85);
    public float healthLerpSpeed = 5;       // higher lerp speed goes faster.

    [Header("Physics")]
    public float jumpForce = 22.5f;
    public float speed = 10.0f;
    [HideInInspector]
    Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f); // How fast we are going.
    // player will use its own gravity for jump physics.
    public float gravityScale = 2.5f;
    public static float globalGravity = -9.81f;
    [HideInInspector]
    public int umbrellaFloat = 1;

    [Header("Jump Sound")]
    // can use these to see ground type. maybe use it?
    //public Transform groundCheck;
    //public float groundDistance = .5f;
    //public LayerMask groundMask;
    //private bool isGrounded;
    public AudioSource jumpSound;

    void Start()
    {
        health = maxHealth;
        originalStepOffset = controller.stepOffset;

        // find health bar from UI in scene hierarchy.
        healthBar = GameObject.Find("Canvas/HealthBar/HealthBarInner");
        // get fill image of health bar.
        healthBarFill = healthBar.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        HPLerp();

        if(controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            controller.stepOffset = originalStepOffset;
        } 
        else
        {
            controller.stepOffset = 0;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // movement.
        Vector3 move = transform.right * x + transform.forward * z;
        move = Vector3.ClampMagnitude(move, 1f);
        controller.Move(move * speed * Time.deltaTime); 

        // gravity. divide by umbrella float strength (1 if not holding; no effect).
        velocity.y += (globalGravity * gravityScale / umbrellaFloat) * Time.deltaTime;
        //if (velocity.y > 12f) velocity.y = 0f;
        controller.Move(velocity * Time.deltaTime);

        // jump button. only jump when the player is touching the ground.
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            Jump();
        }

    }

    public void UmbrellaFall()
    {
        // partially umbrella mechanic, also prevents the player from moonjumping by opening after regular jump.
        if (velocity.y < 0f) return;

        velocity.y = Mathf.Sqrt(jumpForce / umbrellaFloat);
    }

    void Jump()
    {

        // if the umbrella is open, greatly reduce jump height. 
        if (umbrellaFloat != 1)
        {
            velocity.y = Mathf.Sqrt((jumpForce / 4f) * -2f * globalGravity / umbrellaFloat);
        }
        else
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * globalGravity);
        }
        
    }

    // update health bar UI.
    public void HealthUpdate()
    {
        // clamp health to not go above max HP.
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        // make health bar green again if player recovers enough HP.
        if (health / maxHealth > .30)
        {
            healthBarFill.color = goodHealth;
        }

        // clamp health to not go below 0.
        if (health < 0)
        {
            health = 0;
        }

        // make the health bar red when the player is at low HP.
        if ((health / maxHealth <= .30) || (health == 1))
        {
            healthBarFill.color = lowHealth;
        }

        // respawn when dead.
        if (health == 0)
        {
            Debug.Log("death");
            //UpdateScore(this.gameObject.tag);
            //this.photonView.RPC("UpdateScore", RpcTarget.All, this.gameObject.name);
        }
    }

    void HealthUp(float healAmt)
    {
		//if (gameManager.ShouldntUpdate(this)) return;
        //hpRestore.Play();

        // restore HP by amount from source of heal.
        health += healAmt;
        HealthUpdate();

    }

    void HealthDown(float hurtAmt)
    {
		//if (gameManager.ShouldntUpdate(this)) return;
        //hpDrain.Play();

        // decrease health by amount from source of damage.
        health -= hurtAmt;
        HealthUpdate();
    }

    void HPLerp()
    {
        // goes in Update() to animate lerp.
        // update health bar fill amount.
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, (health / maxHealth), Time.deltaTime * healthLerpSpeed);
    }
}
