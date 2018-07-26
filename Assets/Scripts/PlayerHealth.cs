using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    public GameObject deathPrefab;
    public float currentHealth;
    public float maxHealth;
    public bool isDead = false;
    public RectTransform healthBar;
    public float healthBarXSize;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBarXSize = healthBar.sizeDelta.x;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdadeHealfBar(currentHealth);
        if(currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }

    public void UpdadeHealfBar(float value)
    {
        healthBar.sizeDelta = new Vector2(value / maxHealth * healthBarXSize, healthBar.sizeDelta.y);
    }

    public void Die()
    {
        GameObject deathFX = Instantiate(deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        Destroy(deathFX, 3f);

        SetActiveState(false);
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
}
