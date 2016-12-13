using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

	public int direction = 0; //0-right, 1-left
	public int speed = 10;
	public int jumpHeight = 200;
	static private int deaths = 0;
	static private int keys = 0;

	public Vector3 initialposition;




	bool door1 = false;

	public Animator animator; // movement animation object
	private SpriteRenderer sr;

	Vector3 dir = new Vector3(1, 0, 0);
	Vector3 teleDist = new Vector3 (5, 0, 0);
	Rigidbody r;


	public AudioSource source;
	public AudioSource[] list;

	public AudioClip walkingSound;
	public AudioClip jumpingSound;
	public AudioClip teleportSound;
	public AudioClip deathSound;

	Vector3 originalPos;

	public GameObject pause;
	public Text deathCount;
	public Text keyCount;

	// Use this for initialization
	void Start () 
	{

		originalPos = transform.position;

		source = GetComponent<AudioSource>();
		r = GetComponent<Rigidbody>();
		sr = GetComponent<SpriteRenderer>();
		initialposition = transform.position;

		//		list = new AudioClip[]{(AudioClip)Resources.Load ("Sounds/footstep.wav"),
		//			(AudioClip)Resources.Load ("Sounds/chimes.wav"),
		//			(AudioClip)Resources.Load ("Sounds/boing.wav")
		//		};

		updateDeaths ();
		updateKeys ();
	}

	bool isGrounded()
	{
		return (r.velocity.y >= -0.11f && r.velocity.y <= 0.21f);
	}

	// Update is called once per frame
	void Update () 
	{
		animator.SetBool ("walking", false);

		//Left-Right Movement
		if(Input.GetKey(KeyCode.LeftArrow)) {
			dir += transform.right * Time.deltaTime * -speed;
			direction = 1;
			animator.SetBool ("walking", true);
			sr.flipX = true;



		}
		else if(Input.GetKey(KeyCode.RightArrow)) {
			dir += transform.right * Time.deltaTime * speed;
			direction = 0;
			animator.SetBool ("walking", true);
			sr.flipX = false;

		}
		r.MovePosition (transform.position + dir);
		dir = new Vector3 (0, 0, 0);

		//Jumping
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			if (isGrounded())
			{
				r.AddForce(new Vector3(0, jumpHeight, 0));
				source.clip = jumpingSound;
				source.PlayOneShot(jumpingSound, 1F);
			}
		}

		//Teleporting
		if (Input.GetKeyDown(KeyCode.Space))
		{
			source.time = 1.2f;
			source.clip = teleportSound;
			source.Play();

			RaycastHit hit;
			//teleport right
			if (direction == 0)
			{
				//Check if there's anything in front of you that you might get stuck in
				if (Physics.Raycast(transform.position, Vector3.right, out hit, 5.0f))
				{

					//if there is, stop in front of it
					r.MovePosition (transform.position + new Vector3(hit.distance, 0, 0));

			
				}
				else
				{
					//otherwise teleport normally

					r.MovePosition (transform.position + teleDist);



				}
			}
			//teleport left
			else if (direction == 1)
			{
				//Check if there's anything in front of you that you might get stuck in
				if (Physics.Raycast(transform.position, Vector3.left, out hit, 5.0f))
				{
					//if there is, stop in front of it
					r.MovePosition (transform.position - new Vector3(hit.distance, 0, 0));

			
				}
				else
				{
					//otherwise teleport normally
					r.MovePosition (transform.position - teleDist);

				
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Pause ();
		}
	}

	void OnCollisionEnter (Collision col)
	{
		//Used tags so we can easily add in new doors and switches
		if (col.gameObject.CompareTag ("Switch")) {
			Switch s;
			s = col.gameObject.GetComponent<Switch> ();
			s.buttonPress (); //switch is pressed
				


		} else if (col.gameObject.CompareTag ("Door")) {
			Door d;
			d = col.gameObject.GetComponent<Door> ();
			d.checkSwitches (); //check if all switches have been pressed and update door lock state

			if (d.isUnlocked ()) {
				//move to next level
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

			}
		} else if (col.gameObject.CompareTag ("DeathPit")) {
			transform.position = initialposition;
			source.clip = deathSound;
			source.Play();
			deaths++;
			updateDeaths ();
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Trap")
		{
			transform.position = initialposition;
			source.clip = deathSound;
			source.Play();
			deaths++;
			updateDeaths ();
		}

		if (other.tag == "Key") {
			keys++;
			updateKeys ();
		}
	}

	public void Pause()
	{
		if (pause.gameObject.activeInHierarchy == false) {
			pause.gameObject.SetActive (true);
			Time.timeScale = 0;
		} else {
			pause.gameObject.SetActive (false);
			Time.timeScale = 1;
		}
	}

	public void Quit()
	{
		Application.Quit ();
	}

	public void updateDeaths() {
		deathCount.text = "Death Count: " + deaths;
	}

	public void updateKeys() {
		keyCount.text = "Key Count: " + keys;
	}
}
