using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Parser : MonoBehaviour {

	[Space(20)]
	[Header("Parser Settings")]

	[Tooltip("Health points")] public float health;
	[Tooltip("Movement Speed")]public float movementSpeed;
	[Tooltip("Hit Damage")]public float hitDamage;
	[Tooltip("How long Parser is demobilized because of it attacking")]public float hitTime;
	[Tooltip("Minimum time before Parser attacks")]public float minTimeBeforeAttack;
	[Tooltip("Maximum time before Parser attacks")]public float maxTimeBefoerAttack;
	[Tooltip("Distance that attack hits")]public Vector2 attackRaycastOffset;
	[Tooltip("height of a vertical punch raycast")]public float attackRaycastLength;
	[Tooltip("Distance that is close enough to hit player")]public float distanceToHitPlayer;
	[Tooltip("Player Layer Mask")]public int playerLayer;


	private GameObject player;
	private Vector3 currentPosition;
	private Vector3 playerPosition;

	private Vector3 parserCenter;
	private bool demobilized;
	private Rigidbody2D parserRigidBody2d;
	private Animator parserAnimator;
	private SpriteRenderer parserSpriteRenderer;

	private int factorOfSight;
	private bool invoked;
	private bool nearPlayer;
	private bool dead;

	private bool running;
	private bool horizontal;
	private bool attacking;


	// Use this for initialization
	void Start () {
		initializeParser ();
	}


	/*
	 * awake function will be called whenever a game object with this script is created
	 * This function will initialize all variables that are needed at the begining
	 * Use this when making GameObjects from Scripts
	 * 
	 */
	void awake()
	{
		

	}

	// Update is called once per frame
	void Update () {
	
	}


	void FixedUpdate()
	{
		angleSelf ();
		Debug.DrawLine(this.transform.position, new Vector3(this.transform.position.x + 1.5f, this.transform.position.y, this.transform.position.z), Color.magenta);
		if (demobilized == false && dead == false) {
			//checkForEnemy ();

			playerPosition = player.transform.position;
			if (Vector3.Distance (playerPosition, this.transform.position) < 2f) {
				Debug.DrawLine (this.transform.position, player.transform.position, Color.yellow);
			}

			//if(nearPlayer == false)
				goToEnemy ();
			//if (nearPlayer == true) {

			//}
		}
	}


	/*
	 * initializeParser will set the variables that are needed to their begining value
	 */
	private void initializeParser()
	{
		parserRigidBody2d = GetComponent<Rigidbody2D> ();
		parserAnimator = GetComponent<Animator> ();
		parserSpriteRenderer = GetComponent<SpriteRenderer> ();
		running = false;
		horizontal = true;
		attacking = false;
		demobilized = false;
		dead = false;
		nearPlayer = false;
		factorOfSight = 1;
		currentPosition = this.transform.position;
		player = GameObject.FindGameObjectWithTag ("Player");
		// I dont know about this ~parserSpriteRenderer.sortingOrder = -Mathf.FloorToInt (this.transform.position.y);
	}


	private void angleSelf()
	{
		
		this.transform.LookAt (playerPosition);
		this.transform.Rotate (new Vector3 (0, -90, 0), Space.Self);
	}

	private void checkForEnemy()
	{
		playerPosition = player.transform.position;
		if (Vector3.Distance (playerPosition, this.transform.position) < 2f) {
			Debug.DrawLine (this.transform.position, player.transform.position, Color.yellow);
			nearPlayer = true;
		} else {
			nearPlayer = false;
		}
	}


	private void goToEnemy()
	{
		Vector3 directionToMove = playerPosition - this.transform.position;
		if (Vector3.Distance (playerPosition, this.transform.position) < 2.0f) {
			nearPlayer = true;
			stopMovement ();
			parserIdle ();
			if (playerPosition.x > this.transform.position.x) {
				factorOfSight = 1;
			} else {
				factorOfSight = 1;
			}
			Invoke ("attackEnemy", Random.Range (minTimeBeforeAttack, maxTimeBefoerAttack));
		} else {
			moveSelf (directionToMove.normalized.x, directionToMove.normalized.y);
			CancelInvoke ("attackEnemy");
		}
	}

	private void stopMovement()
	{
		parserRigidBody2d.velocity = new Vector2(0, 0);
	}

	private void parserIdle()
	{
		parserAnimator.SetBool ("Run", false);
		//parserAnimator.SetBool ("Horizontal", false);
		parserAnimator.SetBool ("Attack", false);
		invoked = false;
		if (dead == true) {
			//parserAnimator.SetBool ("dead", true);
			demobilized = true;
		}
	}

	private void run()
	{
		running = true;
		parserAnimator.SetBool ("Run", running);
	}


	private void moveSelf (float x, float y)
	{
		if (demobilized == false && dead == false) {
			parserRigidBody2d.velocity = new Vector2 (x * movementSpeed, y * movementSpeed);
			if (parserRigidBody2d.velocity == Vector2.zero) {
				running = false;
				parserAnimator.SetBool ("Run", running);
				CancelInvoke ("run");
				invoked = false;
			} else {
				if (invoked == false) {
					run ();
					//Invoke ("run", .05);
					invoked = true;
				}
			}
		}
	}


	private void attackEnemy()
	{
		PlayerController playerScript = player.GetComponent<PlayerController> ();
		if (playerScript.dead == false)
			attackPlayer ();
		Invoke("attackEnemy", Random.Range(minTimeBeforeAttack, maxTimeBefoerAttack));
	}


	private void attackPlayer()
	{
		if (demobilized == false && dead == false) {
			parserRigidBody2d.velocity = Vector2.zero;
			CancelInvoke ("run");
			hit ();
		}
	}


	private void hit()
	{
		parserAnimator.SetBool ("Attack", true);
		Debug.Log (Time.realtimeSinceStartup + " Attack Hit");

		RaycastHit2D[] player = Physics2D.RaycastAll (this.transform.position + new Vector3 (attackRaycastOffset.x * factorOfSight, attackRaycastOffset.y, 0), Vector2.up, attackRaycastLength, playerLayer);
		if (player.Length > 0) {
			player [0].transform.gameObject.GetComponent<PlayerController> ().getAttacked ();
		}

		StartCoroutine ("getDemobilized", hitTime);
	}



	private IEnumerator getDemobilized (float time)
	{
		demobilized = true;
		yield return new WaitForSeconds(time);
		parserIdle();
		demobilized = false;
	}
}
/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */
