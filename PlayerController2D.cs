using UnityEngine;
using System.Collections;

public class PlayerController2D : MonoBehaviour {

	public int kickback;
	public int playerHealth = 100;
	public float moveSpeed, jumpSpeed;
	public Transform groundCheck;
	public Material flashMaterial;

	private bool isHurt;
	private bool grounded;
	private bool doubleJumped;
	private float groudCheckRadius = .1f;
	private float moveInput;
	private Rigidbody2D rb;
	private AudioSource audio;
	private Material savedMaterial;
	public AudioClip hurtSoundFX;
	private LayerMask myLayerMask = 1 << 8;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		audio = GetComponent<AudioSource>();
	}

	void FixedUpdate(){
		grounded = Physics2D.OverlapCircle (groundCheck.position, groudCheckRadius, myLayerMask);
		// Checks if player is on ground, and returns value.
	}

	void Update () {
		//=============== Moving ===============
		moveInput = Input.GetAxisRaw("Horizontal");
		// Checks the button press, and assigns 1 or -1 based off of input manager.

		if((moveInput != 0) && (grounded)){
			rb.AddForce (transform.right * moveSpeed);
		}

		if((Input.GetKeyDown(KeyCode.RightArrow)) || (Input.GetKeyDown(KeyCode.LeftArrow))){
			// Rotates player to face direction they are moving.
			float turnDirection = (moveInput * 90) - 90;
			transform.localRotation = Quaternion.Euler(new Vector3(0,turnDirection,0));
		}

		//=============== Jumping ===============
		if((Input.GetKeyDown(KeyCode.Space)) && (grounded)){
			rb.AddRelativeForce(Vector2.up * jumpSpeed * 100);
		}
	}
		
	void OnTriggerEnter2D(Collider2D Other){
		if(Other.gameObject.tag == "Enemy" && !isHurt){
			StartCoroutine(Hurt());
		}
	}

	IEnumerator Hurt() {
		isHurt = true;
		// Stops Hurt coroutine from being called again while running.
		playerHealth--;
		if (playerHealth > 0) {
			audio.PlayOneShot (hurtSoundFX);
			rb.velocity = new Vector2(0,0);
			// Sets player velocity to zero, this gives the kickback a consitant motion.
			rb.AddForce (transform.right * kickback * -.5f);
			rb.AddForce (transform.up * kickback);
			// Knocks player up away from enemy.
			savedMaterial = GetComponent<Renderer> ().material;
			GetComponent<Renderer> ().material = flashMaterial;
			// Flashes player sprite, to give visual feedback.
			yield return new WaitForSeconds (.25f);
			GetComponent<Renderer> ().material = savedMaterial;
			// Returns players orginal material.
		} else {
			playerIsDead ();
		}
		isHurt = false;
		// Allows Hurt coroutine to be called again.
	}

	void playerIsDead(){
		// Calls gameover method.
	}
}