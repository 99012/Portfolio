using System.Collections.Generic;
using UnityEngine;

public class Parable : MonoBehaviour {

    public float startXPosition = 1;        // defines starting position of the parable on X axis
    public float endXPosition = 3;          // defines ending position of the parable on X axis - this value is changing while playing
    public float heightFactor = 0.125f;     // defines parable height factor
    public GameObject dotSprite;        // prefab variable
    Ball _ballScript;
    float gameLevel = 1;
    GameController _gameController;
	// Use this for initialization
	void Start () {
        tmpXDestination = endXPosition;
        GameObject ball = GameObject.Find("Ball");
        if (ball != null)
        {
            _ballScript = ball.GetComponent<Ball>() as Ball;
        }
        GameObject gameController = GameObject.Find("GameController");
        if (gameController != null)
        {
            _gameController = gameController.GetComponent<GameController>() as GameController;
        }
    }

    GameObject spawnedSprite;

    private float tmpXDestination;      // variable for checking if parable has changed

    public float parableDensityFactor = 0.3f;     // defines density of parable dots
    public float parableStep = 0.1f;        // defines difference beteween previous and next parable, lower value will cause in more iften parable rendering
    public float parableAcceleratingSpeed = 1f;
    public bool fire = false;
    private bool ballHasBeenFired = false;
    private bool endOfTime = false;     // if player will hold button for too long, this variable will swich to true and ball will fire automaticly
    public float maxParableRange = 17.5f;
    float timeButtonHeld = 0;
    // Update is called once per frame
    void Update () {
        if (endXPosition >= maxParableRange)
        {
            endOfTime = true;
        }
        if (Input.GetButton("Jump") && !fire && !endOfTime)
        {
            timeButtonHeld += Time.deltaTime;
            gameLevel = _gameController.gameLevelSpeed;
            endXPosition += Time.deltaTime * parableAcceleratingSpeed * gameLevel;
            if (endXPosition - tmpXDestination >= parableStep)
            {
                tmpXDestination = endXPosition;
                DrawParable();
            }
        }
        if (Input.GetButtonUp("Jump") && !ballHasBeenFired && !endOfTime)
        {
            if (timeButtonHeld < parableStep)
            {
                _gameController.GameOver();
            }
            fire = true;
            ballHasBeenFired = true;
            if (_ballScript != null)
            {
                _ballScript.FireTheBall(parableDots, endXPosition);
            }
        }
        if (endOfTime && !ballHasBeenFired)
        {
            fire = true;
            ballHasBeenFired = true;
            if (_ballScript != null)
            {
                _ballScript.FireTheBall(parableDots, endXPosition);
            }
        }
	}

    private float pointZero = 0;        // point where the end of parable reaches 0 on Y axis
    public List<GameObject> parableDots = new List<GameObject>();   // list where current parable dots are stored
    void DrawParable()
    {
        for (int i = 0; i < parableDots.Count; i++)
        {
            Destroy(parableDots[i]);
        }
        parableDots.Clear();
        for (float i = startXPosition; i < endXPosition + 1; i += parableDensityFactor)
        {
            //float yPos = -0.25f * ((i - 1) * (i - endXPosition));
            float yPos = (-1)* heightFactor * ((i - 1) * (i - endXPosition));
            if (i>startXPosition && yPos <= 0)
            {
                //pointZero = ((-1)*(0.25f * endXPosition + 0.25f) - Mathf.Sqrt((0.25f*endXPosition + 0.25f) * (0.25f * endXPosition + 0.25f) - 0.25f * endXPosition)) / (-0.5f);       // calculates a point where parable hits 0 on the X axis for heightfactor = -0.25f, basically is the same as endXPosition variable which is known
                pointZero = endXPosition;
                spawnedSprite = Instantiate(dotSprite, new Vector2(pointZero, 0), Quaternion.identity);
                parableDots.Add(spawnedSprite);
                spawnedSprite = Instantiate(dotSprite, new Vector2(i, yPos), Quaternion.identity);      // last parable point which indicates the hole
                spawnedSprite.gameObject.SetActive(false);
                parableDots.Add(spawnedSprite);
                break;
            }
            spawnedSprite = Instantiate(dotSprite, new Vector2(i, yPos), Quaternion.identity); 
            parableDots.Add(spawnedSprite);
        }
    }
}
