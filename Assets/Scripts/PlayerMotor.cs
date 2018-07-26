﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMotor : NetworkBehaviour
{

    private Rigidbody rb;
    public Transform chassis;
    public Transform turret;
    public float moveSpeed = 150f;
    public float chassisRotateSpeed = 3f;
    public float turretRotateSpeed = 6f;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	public void MovePlayer(Vector3 dir)
    {
        Vector3 moveDirection = dir * moveSpeed * Time.deltaTime;
        rb.velocity = moveDirection;
    }

    public void FaceDirection(Transform xForm, Vector3 dir, float rotSpeed)
    {
        if (dir != Vector3.zero && xForm != null)
        {
            Quaternion desiredRot = Quaternion.LookRotation(dir);
            xForm.rotation = Quaternion.Slerp(xForm.rotation, desiredRot, rotSpeed * Time.deltaTime);
        }
    }

    public void RotateChassis(Vector3 dir)
    {
        FaceDirection(chassis, dir, chassisRotateSpeed);
    }

    public void RotateTurret(Vector3 dir)
    {
        FaceDirection(turret, dir, turretRotateSpeed);
    }
}