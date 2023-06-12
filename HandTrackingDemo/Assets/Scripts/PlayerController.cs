using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public float rotateCooldown = 0.75f;
    private float rotateCurrentCooldown;
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
    public float energyGainAmount = 3f;
    private float energyCooldown;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI energyText;
    public Slider healthBar;
    public Slider energyBar;
    public Image fireballCooldownImage;
    public Image electricArcCooldownImage;
    public Image rotateCooldownImage_1;
    public Image rotateCooldownImage_2;
    private RectTransform fBallRT;
    private RectTransform eArcRT;
    private float fBallImageHeight;
    private float eArcImageHeight;
    private RectTransform rotateRT;
    private float rotateImageHeight;
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
        StartCoroutine(RotateCooldown());

        // get width and height of player's screen
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // recttransforms for spell cooldown displays
        fBallRT = fireballCooldownImage.rectTransform;
        eArcRT = electricArcCooldownImage.rectTransform;
        rotateRT = rotateCooldownImage_1.rectTransform;

        // heights of cooldown display images
        fBallImageHeight = fBallRT.rect.height;
        eArcImageHeight = eArcRT.rect.height;
        rotateImageHeight = rotateRT.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        // update UI information for player
        healthText.text = string.Format("{0}/{1}", health, maxHealth);
        energyText.text = string.Format("{0}/{1}", energy, maxEnergy);

        healthBar.value = health / maxHealth;
        energyBar.value = energy / maxEnergy;

        float fBallHeight = (fireballCurrentCooldown / fireballCooldown) * fBallImageHeight;
        float eArcHeight = (electricArcCurrentCooldown / electricArcCooldown) * eArcImageHeight;
        float rotateHeight = (rotateCurrentCooldown / rotateCooldown) * rotateImageHeight;
        
        // lazy code lol
        fireballCooldownImage.rectTransform.sizeDelta = new Vector2(fBallImageHeight, fBallHeight);
        electricArcCooldownImage.rectTransform.sizeDelta = new Vector2(eArcImageHeight, eArcHeight);
        rotateCooldownImage_1.rectTransform.sizeDelta = new Vector2(rotateImageHeight, rotateHeight);
        rotateCooldownImage_2.rectTransform.sizeDelta = new Vector2(rotateImageHeight, rotateHeight);

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
        } else if (prediction == "pointer_left") {
            if (rotateCurrentCooldown <= 0f) {
                // rotate 90 deg left
                cam.transform.Rotate(Vector3.up, -90f, Space.World);
                rotateCurrentCooldown = rotateCooldown;

                // set back to neutral so things don't go haywire
                prediction = "neutral";
            }
        } else if (prediction == "pointer_right") {
            if (rotateCurrentCooldown <= 0f) {
                // rotate 90 deg right
                cam.transform.Rotate(Vector3.up, 90f, Space.World);
                rotateCurrentCooldown = rotateCooldown;

                // set back to neutral so things don't go haywire
                prediction = "neutral";
            }
        }
        else {
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
    private IEnumerator RotateCooldown() {
        while (true) {
            yield return new WaitUntil(() => rotateCurrentCooldown > 0f);
            while (true) {
                yield return new WaitForSeconds(0.1f);
                if (rotateCurrentCooldown - 0.1f == 0) {
                    rotateCurrentCooldown = 0f;
                } else {
                    rotateCurrentCooldown -= 0.1f;
                }
            }
        }
    }

    private void Recharge()
    {
        // need to add effects
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
                yield return new WaitForSeconds(0.1f);
                healingCooldown -= 0.1f;
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
                yield return new WaitForSeconds(0.1f);
                energyCooldown -= 0.1f;
            }
        }
    }
    private IEnumerator FireballCooldown()
    {
        while (true) {
            if (fireballCurrentCooldown > 0) {
                yield return new WaitForSeconds(0.05f);
                if (fireballCurrentCooldown - 0.05f < 0) {
                    fireballCurrentCooldown = 0f;
                } else {
                    fireballCurrentCooldown -= 0.05f;
                }
            } else {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
    private IEnumerator ElectricArcCooldown()
    {
        while (true) {
            if (electricArcCurrentCooldown > 0) {
                yield return new WaitForSeconds(0.05f);
                if (electricArcCurrentCooldown - 0.05f < 0) {
                    electricArcCurrentCooldown = 0f;
                } else {
                    electricArcCurrentCooldown -= 0.05f;
                }
            } else {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
