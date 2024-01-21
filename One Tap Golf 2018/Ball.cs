using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    GameController _gameController;
	// Use this for initialization
	void Start () {
        GameObject gameController = GameObject.Find("GameController");
        if (gameController != null)
        {
            _gameController = gameController.GetComponent<GameController>() as GameController;
        }
	}

    int i = 1;
	// Update is called once per frame
	void Update () {
        if (fire && path.Count > 0)
        {
            targetWayPoint = path[i];
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), targetWayPoint.transform.position, ballSpeed * Time.deltaTime);
            if (transform.position == targetWayPoint.transform.position && i < path.Count - 1)
            {
                i++;
            }
        }
	}

    bool gameOverCalled = false;
    public float ballSpeed = 2f;
    GameObject targetWayPoint;
    bool fire = false;
    List<GameObject> path = new List<GameObject>();
    public void FireTheBall(List<GameObject> wayPoints, float speedMultiplier)
    {
        for (int i = 0; i < wayPoints.Count; i++)
        {
            path.Add(wayPoints[i]);
        }
        fire = true;
        ballSpeed *= speedMultiplier;
    }

    bool ballInHole = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "FlagHole" && !gameOverCalled)
        {
            ballInHole = true;
            _gameController.NextLevel();
        }
        if (collision.gameObject.tag == "Ground" && !ballInHole && fire)
        {
            _gameController.GameOver();
            gameOverCalled = true;
        }
    }
}
