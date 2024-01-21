using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
	public bool win = false;

	//public GameController _gameController;
	GameController _gameController;
	private Animator animator;
	public float speed = 2f;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (!win) {
			float dir = 0;
			if (Input.GetAxisRaw ("Horizontal") != 0) {//.GetAxisRaw("Horizontal")) {
				this.transform.Translate (Input.GetAxisRaw ("Horizontal") * speed * Time.deltaTime, 0, 0);
				dir = Input.GetAxisRaw ("Horizontal");
				if (dir < 0) {
					animator.SetTrigger ("Walk_L");
				} else {
					animator.SetTrigger ("Walk_R");
				}
			}
			if (Input.GetKeyDown (KeyCode.E) || Input.GetKeyDown (KeyCode.Space)) {
				dir = Input.GetAxisRaw ("Horizontal");
				//GameController _gameController = GetComponent(typeof(GameController)) as GameController;
				GameObject gameController = GameObject.Find("GameController");
				_gameController = gameController.GetComponent<GameController> ();
				if (dir < 0) {
					animator.SetTrigger ("Punch_L");
					_gameController.Punch (dir);
				} else {
					animator.SetTrigger ("Punch_R");
					_gameController.Punch (dir);
				}
			}
		} else {
			animator.SetTrigger ("Win");
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene ("Main");
		}
	}
}
