using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Player Controls
	[Range(0.0f, 10.0f)]

	public float moveSpeed = 3f;

	public float dodgeForce = 600f;

	public int playerHealth = 10;

	public bool dead;

	public bool playerCanMove = true;

	private Transform playerTransform;

	private Rigidbody2D playerRigidbody;

	private Animator playerAnimator;

	private float velocityX;

	private float velocityY;

	private bool running = false;

	private bool dodging = false;

	private bool facingLeft = false;

	private bool facingRight = false;

	private bool facingBack = false;

	private bool facingForward = true;

	private int playerLayer;

	private int enemyLayer;

	// Use this for initialization
	void Awake () {
		playerTransform = GetComponent<Transform> ();

		playerRigidbody = GetComponent<Rigidbody2D> ();

		playerAnimator = GetComponent<Animator> ();

		dead = false;

		if (playerRigidbody == null)
			Debug.LogError (Time.realtimeSinceStartup + " Rigidbody2d component mission from this gameobject"+ this.gameObject.ToString());

		if (playerAnimator == null)
			Debug.LogError (Time.realtimeSinceStartup + " Animator component missing from this gameObject" + this.gameObject.ToString());

		playerLayer = this.gameObject.layer;

		enemyLayer = LayerMask.NameToLayer ("Enemy");
	
	}
	
	// Update is called once per frame
	void Update () {

		if (playerCanMove == false || (Time.timeScale == 0f))
			return;

		velocityX = Input.GetAxisRaw ("Horizontal");

		velocityY = Input.GetAxisRaw ("Vertical");

		if (velocityX == 0 && velocityY == 0)
			running = false;
		else
			running = true;

		playerAnimator.SetBool ("Running", running);

		if (Input.GetButtonDown ("Jump") == true) {

			dodging = true;
			playerAnimator.SetBool ("Jumping", dodging);

			if (facingLeft == true) {
				velocityX = 0f;
				playerRigidbody.AddForce (new Vector2 (dodgeForce * -1, 0));
			}
			if (facingRight == true) {
				velocityX = 0f;
				playerRigidbody.AddForce (new Vector2 (dodgeForce, 0));
			}

			if (facingBack == true) {
				velocityY = 0f;
				playerRigidbody.AddForce (new Vector2 (0, dodgeForce));
			}

			if (facingForward == true) {
				velocityY = 0f;
				playerRigidbody.AddForce (new Vector2 (0, dodgeForce * -1));
			}
		}

		playerRigidbody.velocity = new Vector2 (velocityX * moveSpeed, velocityY * moveSpeed);
	
	}

	void LateUpdate()
	{


		facingRight = false;
		facingBack = false;
		facingForward = false;
		facingLeft = false;

		if (velocityX > 0 && velocityY == 0) {
			facingRight = true;
		}
		if (velocityX < 0 && velocityY == 0) {
			facingLeft = true;
		}
		if (velocityY < 0) {
			facingForward = true;
		}
		if (velocityY > 0) {
			facingBack = true;
		}
			
		playerAnimator.SetBool ("Right", facingRight);
		playerAnimator.SetBool ("Left", facingLeft);
		playerAnimator.SetBool ("Front", facingForward);
		playerAnimator.SetBool ("Back", facingBack);
	}


	public void getAttacked()
	{
		Debug.Log (Time.realtimeSinceStartup + " Player got attacked");
	}
}
