using System.Collections;
using System.Collections.Generic;
using IGameControlsActions;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour, IGameControlsActions.GameControls.IShootActions
{
    public Camera cam;
    private int camWidth = 1280;
    private int camHeight = 720;
    private int screenWidth;
    private int screenHeight;

    GameControls controls;
    public GameObject firePoint;
    public GameObject fireball;
    public GameObject crosshair;
    public HandTracking handTracking;
    public ShootFromHand shootFromHand;

    public float maxHealth = 100.0f;
    private float health;
    public float timeBetweenHealing = 0.5f;
    public float timeHealAfterDamage = 3.0f;
    private float healingCooldown;
    public float healAmount = 1.0f;
    public float maxEnergy = 200.0f;
    private float energy;
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
    }

    private void UpdateCrosshair()
    {
        // get center xy values
        float[] centerFloats = handTracking.GetCenterPoints();
        float x = centerFloats[0];
        float y = centerFloats[1];

        float xPercent = x / camWidth;
        float yPercent = y / camHeight;

        float crosshairX = xPercent * screenWidth;
        float crosshairY = screenHeight - (yPercent * screenHeight);
        print(String.Format("{0} {1} {2} {3} {4} {5}", x, y, xPercent, yPercent, crosshairX, crosshairY));

        crosshair.transform.position = new Vector3(crosshairX, crosshairY, 0f);
    }

    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new GameControls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            controls.Shoot.SetCallbacks(this);
        }
        controls.Shoot.Enable();
    }

    public void OnDisable()
    {
        controls.Shoot.Disable();
    }    

    public void OnFireball(InputAction.CallbackContext context)
    {
        GameObject spell;

        if (context.started) {
            spell = Instantiate(fireball, firePoint.transform.position, Quaternion.identity);
            spell.transform.localRotation = shootFromHand.GetRotation();
        }
    }

    public void OnLightningBolt(InputAction.CallbackContext context)
    {
    }

    public void OnLightRay(InputAction.CallbackContext context)
    {
    }

    public void OnIcicle(InputAction.CallbackContext context)
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
    // private IEnumerator RegenEnergy()
    // {
    //     while (true) {
    //         if (!recentlyDamaged) {
    //             if (health + healAmount < maxHealth) {
    //                 health += healAmount;
    //             } else {
    //                 health = maxHealth;
    //             }
    //             yield return new WaitForSeconds(timeBetweenHealing);
    //         } else {
    //             yield return new WaitForSeconds(timeHealAfterDamage);
    //             recentlyDamaged = false;
    //         }
    //     }
    // }
}
