using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private int camWidth = 1280;
    private int camHeight = 720;
    [Range(-720, 720)]
    public int crosshairX_adjust;
    [Range(-1280, 1280)]
    public int crosshairY_adjust;
    private int screenWidth;
    private int screenHeight;

    public GameObject firePoint;
    public GameObject fireball;
    public GameObject crosshair;
    public HandTracking handTracking;
    public ShootFromHand shootFromHand;

    public float maxHealth = 100.0f;
    private float health;
    public float timeBetweenHealing = 0.5f;
    public float timeHealAfterDamage = 3.0f;
    public float healAmount = 1.0f;
    private float healingCooldown;
    public float maxEnergy = 200.0f;
    private float energy;
    public float timeBetweenEnergy = 0.5f;
    public float energyCooldownAfterSpell = 3.0f;
    public float energyGainAmount = 1.0f;
    private float energyCooldown;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI energyText;

    // Start is called before the first frame update
    void Start()
    {
        // at the start, player starts at max HP
        health = maxHealth;
        energy = maxEnergy;

        // regenerate health if not recently damaged
        StartCoroutine(HealPlayer());

        // get width and height of player's screen
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = string.Format("Health: {0}", health);
        energyText.text = string.Format("Energy: {0}", energy);

        UpdateCrosshair();

        String prediction = handTracking.GetPosePrediction();
        ActionManager(prediction);
    }

    private void UpdateCrosshair()
    {
        // get center xy values
        float[] centerFloats = handTracking.GetCenterPoints();
        float x = centerFloats[0] + crosshairX_adjust;
        float y = centerFloats[1] + crosshairY_adjust;

        float xPercent = x / camWidth;
        float yPercent = y / camHeight;

        float crosshairX = xPercent * screenWidth;
        float crosshairY = screenHeight - (yPercent * screenHeight);
        print(String.Format("{0} {1} {2} {3} {4} {5}", x, y, xPercent, yPercent, crosshairX, crosshairY));

        crosshair.transform.position = new Vector3(crosshairX, crosshairY, 0f);
    } 

    public void ActionManager(String prediction) {
        if (prediction == "neutral") {
            
        } else {
            energyCooldown = energyCooldownAfterSpell;
            if (prediction == "at_screen") {
                Fireball();
            } else if (prediction == "peace") {
                ElectricArc();
            }
        }
    }
    public void Fireball()
    {
        GameObject spell;

        spell = Instantiate(fireball, firePoint.transform.position, Quaternion.identity);
        spell.transform.localRotation = shootFromHand.GetRotation();
    }

    public void ElectricArc()
    {
    }
    public void Damage(float amount) {
        health -= amount;
        healingCooldown = timeHealAfterDamage;
    }
    private IEnumerator HealPlayer()
    {
        while (true) {
            if (healingCooldown == 0 || healingCooldown - Time.deltaTime <= 0) {
                if (health + healAmount < maxHealth) {
                    health += healAmount;
                } else {
                    health = maxHealth;
                }
                yield return new WaitForSeconds(timeBetweenHealing);
            } else {
                healingCooldown -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    public void DrainEnergy(float amount) {
        energy -= amount;
    }
    private IEnumerator RegenEnergy()
    {
        while (true) {
            if (energyCooldown == 0 || energyCooldown - Time.deltaTime <= 0) {
                if (energy + energyGainAmount < maxEnergy) {
                    energy += energyGainAmount;
                } else {
                    energy = maxEnergy;
                }
                yield return new WaitForSeconds(timeBetweenEnergy);
            } else {
                energyCooldown -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
