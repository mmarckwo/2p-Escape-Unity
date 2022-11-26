using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class HPHandler : NetworkBehaviour
{

    [Header("Health Properties")]
    public float maxHealth = 100.0f;
    [HideInInspector]
    [Networked(OnChanged = nameof(HealthUpdate))]
    public float health { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    public Image healthBarFill;
    public Color goodHealth = new Color(69, 255, 137);
    public Color lowHealth = new Color(255, 0, 85);
    public float healthLerpSpeed = 5;       // higher lerp speed goes faster.

    bool isInit = false;

    void Start()
    {
        isDead = false;
        health = maxHealth;

        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        HPLerp();
    }

    void HealthUp(float healAmt)
    {
        //if (gameManager.ShouldntUpdate(this)) return;
        //hpRestore.Play();

        // restore HP by amount from source of heal.
        health += healAmt;

    }

    public void HealthDown(float hurtAmt)
    {
        //if (gameManager.ShouldntUpdate(this)) return;
        //hpDrain.Play();

        if (isDead == true) return;

        // decrease health by amount from source of damage.
        health -= hurtAmt;
    }

    void HPLerp()
    {
        // goes in Update() to animate lerp.
        // update health bar fill amount.
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, (health / maxHealth), Time.deltaTime * healthLerpSpeed);
    }

    // update health bar UI.
    static void HealthUpdate(Changed<HPHandler> changed)
    {
        float newHP = changed.Behaviour.health;

        changed.LoadOld();

        float oldHP = changed.Behaviour.health;

        if (newHP != oldHP)
            changed.Behaviour.localHealthUpdate(newHP);
    }

    private void localHealthUpdate(float newHealth)
    {
        if (!isInit) return;
        if (isDead == true) return;

        // clamp health to not go above max HP.
        if (newHealth > maxHealth)
        {
            health = maxHealth;
        }
        // make health bar green again if player recovers enough HP.
        if (newHealth / maxHealth > .30)
        {
            if (Object.HasInputAuthority)
                healthBarFill.color = goodHealth;
        }

        // clamp health to not go below 0.
        if (newHealth < 0)
        {
            health = 0;
        }

        // make the health bar red when the player is at low HP.
        if ((newHealth / maxHealth <= .30) || (newHealth == 1))
        {
            if (Object.HasInputAuthority) 
                healthBarFill.color = lowHealth;
        }

        // hp 0 = dead.
        if (health == 0)
        {
            isDead = true;
        }
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        if (isDeadOld)
        {
            changed.Behaviour.OnDeath();
        }
            
    }

    private void OnDeath()
    {
        Debug.Log("Death");
    }
}
