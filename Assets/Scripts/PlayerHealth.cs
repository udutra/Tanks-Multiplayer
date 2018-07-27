using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = "UpdadeHealfBar")]
    public float currentHealth;

    [SyncVar]
    public bool isDead = false;

    public GameObject deathPrefab;
    public RectTransform healthBar;
    public float maxHealth;
    public float healthBarXSize;

    private void Awake()
    {
        healthBarXSize = healthBar.sizeDelta.x;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= damage;

        if(currentHealth <= 0 && !isDead)
        {
            isDead = true;
            RpcDie();
        }
    }

    public void UpdadeHealfBar(float value)
    {
        healthBar.sizeDelta = new Vector2(value / maxHealth * healthBarXSize, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    public void RpcDie()
    {
        GameObject deathFX = Instantiate(deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        Destroy(deathFX, 3f);

        SetActiveState(false);

        gameObject.SendMessage("Disable");
    }

    public void SetActiveState(bool state)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = state;
        }

        foreach (Canvas c in GetComponentsInChildren<Canvas>())
        {
            c.enabled = state;
        }

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = state;
        }
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        SetActiveState(true);
        isDead = false;
    }
}
