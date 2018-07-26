using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    private PlayerMotor pMotor;
    private PlayerShoot playerShoot;


	void Start () {
        pMotor = GetComponent<PlayerMotor>();
        playerShoot = GetComponent<PlayerShoot>();
	}
	
	void Update () {

        if (!isLocalPlayer)
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
            playerShoot.Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
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

    
}
