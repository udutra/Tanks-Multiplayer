using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public ParticleSystem explosionFX;
    public Rigidbody rb;
    public Collider col;
    public PlayerController owner;
    public int bounces = 2;
    public int valueBulletDamage = 1;

	void Start () {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
	}
	
	void Update () {
		
	}

    public void Explode()
    {
        rb.velocity = Vector3.zero;
        rb.Sleep();
        col.enabled = false;

        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = false;
        }

        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        explosionFX.transform.parent = null;
        explosionFX.Play();

        Destroy(explosionFX.gameObject, 1);

        Destroy(gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        if(rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if(player != null)
        {
            Explode();
            player.TakeDamage(valueBulletDamage, owner);
        }

        if(bounces <= 0)
        {
            Explode();
        }
        bounces--;
    }
}