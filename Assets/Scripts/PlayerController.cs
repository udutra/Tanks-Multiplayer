﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    public PlayerMotor pMotor;
    public PlayerShoot pShoot;
    public PlayerHealth pHealth;
    public GameObject spawnFX;
    public float wait = 3f;
    public int score;

	void Start () {
        pMotor = GetComponent<PlayerMotor>();
        pShoot = GetComponent<PlayerShoot>();
        pHealth = GetComponent<PlayerHealth>();
	}
	
	void Update () {

        if (!isLocalPlayer || pHealth.isDead)
        {
            return;
        }
        
        Vector3 inputDirection = GetInput();

        if (inputDirection.sqrMagnitude > 0.25f)
        {
            pMotor.RotateChassis(inputDirection);
        }

        Vector3 turretDir = Utility.GetWorldPointFromScreenPoint(Input.mousePosition, pMotor.turret.position.y) - pMotor.turret.position;
        pMotor.RotateTurret(turretDir);

        if (Input.GetMouseButton(0))
        {
            pShoot.CmdShoot();
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer || pHealth.isDead)
        {
            return;
        }
        pMotor.MovePlayer(GetInput());
    }

    public Vector3 GetInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v);
    }
    
    public void Disable()
    {
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        transform.position = Vector3.zero;
        pMotor.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(wait);
        pHealth.Reset();
        GameObject newSpawnFX = Instantiate(spawnFX, transform.position, Quaternion.identity);
        Destroy(newSpawnFX, 3f);
    }
}
