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
    public GameObject electricArc;
    public float fireballEnergy = 10.0f;
    public float electricArcEnergy = 20.0f;
    public float fireballCooldown = 0.5f;
    public float electricArcCooldown = 2.0f;
    private float fireballCurrentCooldown;
    private float electricArcCurrentCooldown;
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
    public float timeBetweenEnergy = 0.1f;
    public float timeBeforeGainEnergy = 1.0f;
    public float energyGainAmount = 2.5f;
    private float energyCooldown;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI debugText;

    // Start is called before the first frame update
    void Start()
    {
        // at the start, player starts at max HP
        health = maxHealth;
        energy = maxEnergy;

        // regenerate health if not recently damaged
        StartCoroutine(HealPlayer());
        StartCoroutine(RegenEnergy());

        // set cooldowns to 0
        fireballCurrentCooldown = 0.0f;
        electricArcCurrentCooldown = 0.0f;

        // manage cooldowns in coroutine
        StartCoroutine(FireballCooldown());
        StartCoroutine(ElectricArcCooldown());

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

        debugText.text = string.Format("{0}\n{1}\n{2}", fireballCurrentCooldown, energyCooldown, prediction);
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
        // print(String.Format("{0} {1} {2} {3} {4} {5}", x, y, xPercent, yPercent, crosshairX, crosshairY));

        crosshair.transform.position = new Vector3(crosshairX, crosshairY, 0f);
    } 

    public void ActionManager(String prediction) {
        if (prediction == "neutral") {
            Recharge();
        } else {
            if (prediction == "at_screen") {
                if (energy > fireballEnergy && fireballCurrentCooldown <= 0f) {
                    Fireball();
                    energyCooldown = timeBeforeGainEnergy;
                    fireballCurrentCooldown = fireballCooldown;
                } else {
                    Recharge();
                }
            } else if (prediction == "peace") {
                if (energy > electricArcEnergy && electricArcCurrentCooldown <= 0f) {
                    ElectricArc();
                    energyCooldown = timeBeforeGainEnergy;
                    electricArcCurrentCooldown = electricArcCooldown;
                } else {
                    Recharge();
                }
            }
        }
    }

    private void Recharge()
    {
        // need to add effects
        print("recharging");
    }

    public void Fireball()
    {
        GameObject spell;

        spell = Instantiate(fireball, firePoint.transform.position, Quaternion.identity);
        spell.transform.localRotation = shootFromHand.GetRotation();
        DrainEnergy(fireballEnergy);
    }

    public void ElectricArc()
    {
        GameObject spell;

        spell = Instantiate(electricArc, firePoint.transform.position, Quaternion.identity);
        spell.transform.localRotation = shootFromHand.GetRotation();
        DrainEnergy(electricArcEnergy);
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
    private IEnumerator FireballCooldown()
    {
        while (true) {
            if (fireballCurrentCooldown > 0) {
                yield return new WaitForSeconds(0.1f);
                if (fireballCurrentCooldown - 0.1f < 0) {
                    fireballCurrentCooldown = 0f;
                } else {
                    fireballCurrentCooldown -= 0.1f;
                }
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    private IEnumerator ElectricArcCooldown()
    {
        while (true) {
            if (electricArcCurrentCooldown > 0) {
                yield return new WaitForSeconds(0.1f);
                if (electricArcCurrentCooldown - 0.1f < 0) {
                    electricArcCurrentCooldown = 0f;
                } else {
                    electricArcCurrentCooldown -= 0.1f;
                }
            } else {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
