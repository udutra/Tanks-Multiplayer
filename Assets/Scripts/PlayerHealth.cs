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
    public PlayerController lastAttacker;
    public float maxHealth;
    public float healthBarXSize;

    private void Awake()
    {
        healthBarXSize = healthBar.sizeDelta.x;
    }

    private void Start()
    {
        Reset();
    }

    public void TakeDamage(float damage, PlayerController pc = null)
    {
        if (!isServer)
        {
            return;
        }

        if(pc != null && pc != this.GetComponent<PlayerController>() )
        {
            lastAttacker = pc;
        }

        currentHealth -= damage;

        if(currentHealth <= 0 && !isDead)
        {
            if(lastAttacker != null)
            {
                lastAttacker.score++;
                lastAttacker = null;
            }

            GameManager.instance.UpdateScore();

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
