using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour {

	GameController _gameController;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Enemy") {
			GameObject gameController = GameObject.Find ("GameController");
			_gameController = gameController.GetComponent<GameController> ();
			_gameController.EnemyDead ();
			//Debug.Log ("Enemy Died");
		}
	}
}
