using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    public Rigidbody bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;
    public float nextFire;
    public Transform bulletSpawn;

	void Start () {
		
	}
	
	void Update () {
		
	}

    [Command]
    public void CmdShoot()
    {
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Rigidbody tempBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            tempBullet.velocity = bulletSpeed * bulletSpawn.transform.forward;
            tempBullet.GetComponent<Bullet>().owner = GetComponent<PlayerController>();
            NetworkServer.Spawn(tempBullet.gameObject);
        }
    }
}
