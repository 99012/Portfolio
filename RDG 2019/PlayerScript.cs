using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    float step = 1f;
    private int xDirection = -1;
    private int zDirection = 0;
    Rigidbody playerRigidbody;
    public float jumpForce = 5;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        playerAnimator = playerMesh.GetComponent<Animator>();
        _weapon = weapon.GetComponent<Weapon>() as Weapon;
    }

    private int jumpCount = 0;
    public int allowedJumps = 1;
    public GameObject playerMesh;
    Animator playerAnimator;

    Vector3 direction = new Vector3();
    Weapon _weapon;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Horizontal"))
        {
            this.transform.localPosition += new Vector3(xDirection, this.transform.localPosition.y, zDirection) * step *Time.deltaTime;
            playerAnimator.SetBool("Walk", true);
        }
        else
        {
            playerAnimator.SetBool("Walk", false);
        }
        if (Input.GetButtonDown("Jump") && jumpCount < allowedJumps)
        {
            jumpCount++;
            playerRigidbody.velocity += jumpForce * Vector3.up;
        }
        if (Input.GetButtonDown("Vertical"))
        {
            transform.Rotate(transform.localRotation.x, transform.localRotation.y - 90f, transform.localRotation.z, Space.Self);

            RotatePlayer();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            direction = new Vector3(this.transform.localPosition.x + xDirection * step * 5, this.transform.localPosition.y, this.transform.localPosition.z + zDirection * step * 5);
            playerAnimator.SetBool("Attack", true);
            weapon.GetComponent<Collider>().enabled = true;
            _weapon.hitDirection = direction; 
            StartCoroutine(Attack());
        }
    }

    public void SetDirections(int[] dirs) 
    {
        xDirection = dirs[0];
        zDirection = dirs[1];
    }
    private void RotatePlayer() 
    {
        if (xDirection == 0 && zDirection == 1)
        {
            xDirection = -1;
            zDirection = 0;
            return;
        }
        if (xDirection == -1 && zDirection == 0)
        {
            xDirection = 0;
            zDirection = -1;
            return;
        }
        if (xDirection == 0 && zDirection == -1)
        {
            xDirection = 1; 
            zDirection = 0;
            return;
        }
        if (xDirection == 1 && zDirection == 0)
        {
            xDirection = 0; 
            zDirection = 1;
            return;
        }
    }
    private void ResetJumps() 
    {
        jumpCount = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            ResetJumps();
        }
    }

    private IEnumerator Attack() 
    {
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("Attack", false);
        weapon.GetComponent<Collider>().enabled = false;
    }

    public GameObject weapon;
    public void WeaponAvailable() 
    {
        weapon.SetActive (true);
    }
}
