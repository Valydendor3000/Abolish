using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    Teleport teleportScript;
	[Header("Movement")]
	private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

	public float groundDrag;
 
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode switchKey = KeyCode.Q;

	[Header("Ground Check")]
	public float playerHeight;
	public LayerMask whatIsGround;
	public bool grounded;

	public Transform orientation;

    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToShoot;

    [Header("Settings")]
    public float shotCooldown;

    [Header("Shooting")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public float shootForce;
    public float shootUpwardForce;

    bool readyToShoot;

	float horizontalInput;
	float verticalInput;

	Vector3 moveDirection;

	Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        air
    }

	//public TMP_Text winText;
	/*public TMP_Text liveText;
    public TMP_Text ammoText;
    public TMP_Text grenadeText;
    public TMP_Text gunText;
    public TMP_Text rpgText;*/
    //public TMP_Text counterText;
	static int lives = 3;
    static int totalShots = 6;
    
    int kills;
	[SerializeField]
	private Transform respawnPoint;
	
	
	public int keyCount;
	/*public AudioSource gunSound;
	public AudioSource rpgSound;
    public AudioSource damageSound;
    public AudioSource explosionSound;*/
	
	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true; 
        readyToJump = true;
        readyToShoot = false;
		keyCount = 0;
		//SetLiveText();
	}
	private void Update ()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if (Input.GetKeyDown(shootKey) && readyToShoot && keyCount == 3 && totalShots > 0)
        {
            ShootGun();
        }
		MyInput();
        SpeedControl();
        StateHandler();
        //ShowKills();
        if(keyCount == 3)
        {
            readyToShoot = true;
        }
		if(grounded)
			rb.drag = groundDrag;
		else
			rb.drag = 0;
    }
	void FixedUpdate ()
	{
		MovePlayer();
	}
    /*private void ShowKills()
    {
        counterText.text = kills.ToString();
    }*/
    public void AddKill()
    {
        kills++;
    }
    private void ShootGun()
    {
        readyToShoot = false;

        GameObject projectile = Instantiate(objectToShoot, attackPoint.position, cam.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;
        
        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point  - attackPoint.position).normalized;
        }
        
        Vector3 forceToAdd = forceDirection * shootForce + transform.up * shootUpwardForce;
        
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        //gunSound.Play();

        totalShots--;

        //SetAmmoText();

        Destroy(projectile, 5f);

        Invoke(nameof(ResetShots), shotCooldown);
    }
    private void ResetShots()
    {
        readyToShoot = true;
    }
	private void MyInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
	}
    private void StateHandler()
    {
        if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if(grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air; 
        }
    }
	private void MovePlayer()
	{
		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
		

        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
	}
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag("Key"))
		{
			Destroy(other.gameObject);
            keyCount++;
            teleportScript.AddKey();
            /*gunText.text = "Gun Aquired";
            SetAmmoText();*/
		}
		if (other.gameObject.CompareTag("Ammo") && readyToShoot)
		{
			other.gameObject.SetActive (false);
			totalShots += 6 - totalShots;
            //SetAmmoText();
		}
		if (other.gameObject.CompareTag("EnemyBullet"))
		{
			Destroy(other.gameObject);
			//damageSound.Play();
			transform.position = respawnPoint.position;
			rb.velocity = Vector3.zero;
			loseLife();
			other.gameObject.SetActive (false);
		}
        if (other.gameObject.CompareTag("DeathZone"))
		{
			Destroy(gameObject);
			Debug.Log("Game Over!");
			Application.Quit();
		}
        if (other.gameObject.CompareTag("Door") && kills == 10)
        {
            Destroy(other.gameObject);
        }
	}
	private void loseLife()
	{
		lives--;
		//SetLiveText();
		if(lives < 1)
        {
            Destroy(gameObject);
			Debug.Log("Game Over!");
			Application.Quit();
        }
		Debug.Log("Player Destroyed. " + lives + " lives left.");
	}
	/*void SetLiveText()
	{
		liveText.text = "Lives: " + lives.ToString ();
	}
    void SetAmmoText()
    {
       ammoText.text = "Ammo: " + totalShots.ToString ();
    }
    void SetGrenadeText()
    {
       grenadeText.text = "Grenades: " + rpgShots.ToString ();
    }*/
}
