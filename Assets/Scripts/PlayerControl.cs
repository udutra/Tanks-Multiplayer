using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{

    public PlayerMotor pMotor;
    public PlayerShoot pShoot;
    public PlayerHealth pHealth;
    public PlayerSetup pSetup;
    public GameObject spawnFX;
    public float wait = 3f;

    [SyncVar]
    public int score;

    public NetworkStartPosition[] spawnPoints;
    public Vector3 originalPosition;

	void Start () {
        pMotor = GetComponent<PlayerMotor>();
        pShoot = GetComponent<PlayerShoot>();
        pHealth = GetComponent<PlayerHealth>();
        pSetup = GetComponent<PlayerSetup>();
	}

   public override void OnStartLocalPlayer()
    {
        Debug.Log("Entrou");
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        originalPosition = transform.position;
    }

    /*public override void OnStartClient()
    {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        originalPosition = transform.position;
    }*/

    /*public override void OnStartAuthority()
    {
        Debug.Log("Entrou3");
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        originalPosition = transform.position;
    }*/

    /*public override void OnStartServer()
    {
        Debug.Log("Entrou4");
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        originalPosition = transform.position;
    }*/

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

    private void OnDestroy()
    {
        GameManager.allPlayers.Remove(this);
    }


    public IEnumerator Respawn()
    {
        SpawnPoint oldSpawn = GetNearestSpawnPoint();
		if(oldSpawn != null)
		{
			oldSpawn.isOcupied = false;
		}

		transform.position = GetRandomSpawnPosition();
		pMotor.rb.velocity = Vector3.zero;
		yield return new WaitForSeconds(3f);
		pHealth.Reset();
		GameObject newSpawnFX = Instantiate(spawnFX, transform.position, Quaternion.identity);
		Destroy(newSpawnFX, 3f);
        EnableControls();

    }

    public SpawnPoint GetNearestSpawnPoint()
    {
        Collider[] triggerColliders = Physics.OverlapSphere(transform.position, 3f, Physics.AllLayers, QueryTriggerInteraction.Collide);
        foreach (Collider c in triggerColliders)
        {
            SpawnPoint spawnpoint = c.GetComponent<SpawnPoint>();
            if (spawnpoint != null)
            {
                return spawnpoint;
            }
        }

        return null;
    }

    public Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints != null)
        {
            bool foundSpawner = false;
            Vector3 newStartPosition = new Vector3();
            float timeOut = Time.time + 2f;

            while (!foundSpawner)

            {
                NetworkStartPosition startPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                SpawnPoint spawnPoint = startPoint.GetComponent<SpawnPoint>();
                if (spawnPoint.isOcupied == false)
                {
                    newStartPosition = startPoint.transform.position;
                    foundSpawner = true;
                }

                if (Time.time > timeOut)
                {
                    foundSpawner = true;
                    newStartPosition = originalPosition;
                }
            }

            return newStartPosition;
        }

        return originalPosition;
    }

    public void EnableControls()
    {
        pShoot.Enable();
        pMotor.Enable();
    }

    public void DisableControls()
    {
        pShoot.Disable();
        pMotor.Disable();
    }

}
