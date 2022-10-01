using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    private float health;
    private float lerpTimer;
    [Space(20)]
    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    [Space(20)]
    [Header("Shield Bar")]
    public Image shieldBar;
    public float curMaxShield;
    public bool shieldActive;

    [Space(20)]
    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;

    private float durationTimer;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        health = maxHealth;
        shieldActive = false;
        curMaxShield = 0f;
        shieldBar.fillAmount = 0f;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (overlay.color.a > 0)
        {
            if (health < 30)
                return;
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                // fade the image
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }
    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            // percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }
    public void AddShield(float newAddAmount)
    {
        shieldActive = true;

        if (curMaxShield < newAddAmount)
        {
            // new shield is larger
            curMaxShield = newAddAmount;
            shieldBar.fillAmount = newAddAmount;
        }
        else
        {
            // current shield is larger
            float addFraction = newAddAmount / curMaxShield;
            shieldBar.fillAmount += Mathf.Round(addFraction * 100.0f) * 0.01f;
        }
    }
    public void TakeDamage(float damage)
    {
        if (shieldActive)
        {
            // Shield is on
            float minusFraction = damage / curMaxShield;
            shieldBar.fillAmount -= Mathf.Round(minusFraction * 100.0f) * 0.01f;

            if (shieldBar.fillAmount <= 0.02f)
            {
                shieldBar.fillAmount = 0f;
                shieldActive = false;
            }
        }
        else
        {
            // Shield if off
            health -= damage;
            lerpTimer = 0f;
            durationTimer = 0;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.3f);
        }
    }
    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
}
