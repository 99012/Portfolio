using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    public Transform ufoPath1;
    public Transform ufoPath2;
    private List<GameObject> path1 = new List<GameObject>();
    private List<GameObject> path2 = new List<GameObject>();
    public float speed = 5;
    public GameObject ufoObject;
    Animator ufoAnimator;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in ufoPath1)
        {
            path1.Add(child.gameObject);
        }
        foreach (Transform child in ufoPath2)
        {
            path2.Add(child.gameObject);
        }
        ufoAnimator = ufoObject.GetComponent<Animator>();
    }
    GameObject targetWayPoint;
    private int i = 0;
    bool route1Started = false;
    bool route2Started = false;
    // Update is called once per frame
    void Update()
    {
        if (route1Started)
        {
            targetWayPoint = path1[i];
            this.transform.localPosition = Vector3.MoveTowards(new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z), targetWayPoint.transform.localPosition, speed * Time.deltaTime);
            
            if (this.transform.localPosition == targetWayPoint.transform.localPosition && i < path1.Count - 1)
            {
                i++;
            }
            if (this.transform.localPosition == path1[path1.Count-1].transform.localPosition)
            {
                ufoAnimator.SetBool("rotate", true);
                StartCoroutine(DelayTime());
            }
        }
        if (route2Started)
        {
            targetWayPoint = path2[i];
            this.transform.localPosition = Vector3.MoveTowards(new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z), targetWayPoint.transform.localPosition, speed * Time.deltaTime);

            if (this.transform.localPosition == targetWayPoint.transform.localPosition && i < path2.Count - 1)
            {
                i++;
            }
            else if (this.transform.localPosition == path2[path2.Count - 1].transform.localPosition)
            {
                i = 0;
                route2Started = false;
            }
        }
    }

    public void StartRoute() 
    {
        route1Started = true;
    }

    private IEnumerator DelayTime() 
    {
        route1Started = false;
        i = 0;
        yield return new WaitForSeconds(3);
        route2Started = true;
        ufoAnimator.SetBool("rotate", false);
    }
}
