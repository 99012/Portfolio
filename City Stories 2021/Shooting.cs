using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;             // defines the point of spawning bullet prefab
    public GameObject bulletPrefab;         // prefab of the bullet for regular gun
    public GameObject RPGbulletPrefab;      // prefab of the RPG

    public float bulletForce = 20f;         // speed of bullet
    public float damage = 10f;              // bullet damage
    float damageMultiplier = 1;             // changed on testosteron use, multiplies the damage value
    public ParticleSystem muzzleFlash;      // shoot effect
    public int weaponIndex = 0;             // id of weapon     >> 0 - pistol <<>> 1 - machine gun <<>> 2 - RPG <<

    public float fireRate = 15;             // defines how fast player can shoot, higher value allows to shoot more bullets in less time
    private float nextTimeToFire = 0f;      // used to calculate fire rate time

    public int myAmmo = 0;                  // represents the actual number of available bullets
    public int maxAmmo = 10;                // represents the max number of bullets in magazine
    private int currentAmmo;                // represents the number of bullets in magazine
    public float reloadTime = 1f;           // time needed to reload gun
    private bool isReloading = false;       // true if gun is beeing currently reloaded, false if not

    GameObject globalAmmo;                  // reference to GlobalAmmo object
    GlobalAmmo _globalAmmo;                 // reference to GlobalAmmo script

    GameObject player;                      // reference to Player object
    PlayerAnimations _playerAnimations;     // reference to PlayerAnimations script

    private bool playerInCar = false;       // variable used to define if player is in the car, true if yes, false if not
    private GameObject shootSound;          // gun shoot sound object
    //public Animator animator;
    private GameObject RPGMissile;

    private bool playerStatus;
    PlayerTakeDamage _playerTakeDamage;

    private void OnEnable()
    {
        _playerTakeDamage = GameObject.Find("Player").GetComponent<PlayerTakeDamage>() as PlayerTakeDamage;
        if (globalAmmo == null)
        {
            globalAmmo = GameObject.Find("GlobalAmmo");
            _globalAmmo = globalAmmo.GetComponent<GlobalAmmo>() as GlobalAmmo;
        }
        if (player == null)
        {
            player = GameObject.Find("Player");
            _playerAnimations = player.GetComponent<PlayerAnimations>() as PlayerAnimations;
        }
        try
        {
            shootSound = this.transform.Find("GunFireSound").gameObject;
        }
        catch
        {
            Debug.Log("GunFireSound not found");
        }
        isReloading = false;
        //animator.SetBool("Reloading", false);
        myAmmo = _globalAmmo.GetAmmoValue(weaponIndex);
        if (weaponIndex == 2)
        {
            RPGMissile = GameObject.Find("RPG_Missile");
        }
    }

    void Start()
    {
        _playerTakeDamage = GameObject.Find("Player").GetComponent<PlayerTakeDamage>() as PlayerTakeDamage;
        globalAmmo = GameObject.Find("GlobalAmmo");
        _globalAmmo = globalAmmo.GetComponent<GlobalAmmo>() as GlobalAmmo;
        //currentAmmo = maxAmmo;
        currentAmmo = _globalAmmo.GetLoadedAmmo(weaponIndex);
        myAmmo = _globalAmmo.GetAmmoValue(weaponIndex);
        player = GameObject.Find("Player");
        _playerAnimations = player.GetComponent<PlayerAnimations>() as PlayerAnimations;
        if (weaponIndex == 2)
        {
            RPGMissile = GameObject.Find("RPG_Missile");
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerStatus = _playerTakeDamage.GetPlayerStatus();
        if (!playerStatus)
        {
            if (isReloading)
            {
                return;
            }
            myAmmo = _globalAmmo.GetAmmoValue(weaponIndex);
            if (currentAmmo <= 0 && myAmmo > 0)
            {
                StartCoroutine(Reload());
                return;
            }
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0 && !playerInCar)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
            if (!Input.GetButton("Fire1") && !playerInCar)
            {
                _playerAnimations.ActivaateIdle();
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        
        _playerAnimations.ReloadGunTrigger(weaponIndex);
        Debug.Log("Setting reload trigger, weapon index " + weaponIndex.ToString());
        //animator.SetBool("Reloading", true);
        _globalAmmo.SetLoadedAmmo(maxAmmo, weaponIndex);
        currentAmmo = _globalAmmo.GetLoadedAmmo(weaponIndex);
        yield return new WaitForSeconds(reloadTime);    //  reloadTime - .25f
                                                        //  yield return new WaitForSeconds(.25f); 
        if (weaponIndex == 2)
        {
            //RPGMissile.SetActive(true);
            RPGMissile.GetComponent<MeshRenderer>().enabled = true;
        }
        //animator.SetBool("Reloading", false);

        isReloading = false;

        //currentAmmo = maxAmmo;
        //currentAmmo = _globalAmmo.GetAmmoValue(weaponIndex);
        
    }

    /// <summary>
    /// Shoots the bullet, set the bullets damage, updates GUI ammo, plays muzzle flash animation
    /// </summary>
    void Shoot()
    {
        
        currentAmmo--;
        //_globalAmmo.UpdateAmmo(weaponIndex, currentAmmo);
        _globalAmmo.SetLoadedAmmoValue(currentAmmo, weaponIndex);
        _playerAnimations.ShootGunValue(weaponIndex, true);

        muzzleFlash.Play();
        if (weaponIndex == 2)
        {
            //RPGMissile.SetActive(false);
            RPGMissile.GetComponent<MeshRenderer>().enabled = false;

            GameObject bullet = Instantiate(RPGbulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletSettings = bullet.GetComponent<Bullet>() as Bullet;
            bulletSettings.SetDamage(damage, true);     // true for special bullet
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletSettings = bullet.GetComponent<Bullet>() as Bullet;
            bulletSettings.SetDamage(damage);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
        if (shootSound != null)
        {
            shootSound.GetComponent<AudioSource>().Play();
        }
    }

    /// <summary>
    /// Sets damage multiplier for testosteron use
    /// </summary>
    /// <param name="multiplier">Multiplier of damage</param>
    public void Testosteron (int multiplier)
    {
        damageMultiplier = multiplier;
        damage *= damageMultiplier;
    }
}
