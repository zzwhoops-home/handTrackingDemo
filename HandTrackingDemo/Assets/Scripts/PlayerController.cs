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
    public float energyGainAmount = 2f;
    private float energyCooldown;
    public TextMeshProUGUI healthText;
    public Image hurtImage;
    public GameObject hurtDirectionImage;
    public Transform hurtDirectionImageParent;
    [Range(0.0f, 0.9f)]
    public float hurtThreshold = 0.25f;
    public TextMeshProUGUI energyText;
    public Slider healthBar;
    public Animation healthBarShake;
    public Slider energyBar;
    public Image fireballCooldownImage;
    public Image electricArcCooldownImage;
    public Image rotateCooldownImage_1;
    public Image rotateCooldownImage_2;
    public Image facingImage;
    private RectTransform fBallRT;
    private RectTransform eArcRT;
    private float fBallImageHeight;
    private float eArcImageHeight;
    private RectTransform rotateRT;
    private float rotateImageHeight;
    public TextMeshProUGUI debugText;

    public TextMeshProUGUI gameOverScoreText;
    public GameObject gameOverPanel;
    public GameObject winScreen;
    private int scoreTarget;
    private int scoreDisplay;
    public TextMeshProUGUI scoreText;
    public int growthAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        // at the start, player starts at max HP, 0 score, win screen and game over are not enabled
        gameOverPanel.SetActive(false);
        gameOverScoreText.gameObject.SetActive(false);
        winScreen.SetActive(false);
        scoreTarget = 0;
        health = maxHealth;
        energy = maxEnergy;

        // set cooldowns to 0
        fireballCurrentCooldown = 0.0f;
        electricArcCurrentCooldown = 0.0f;

        // manage cooldowns in coroutine
        StartCoroutine(FireballCooldown());
        StartCoroutine(ElectricArcCooldown());
        StartCoroutine(RotateCooldown());

        // regenerate health if not recently damaged. Also add health warning effect and make sure it is transparent at the start of the game
        StartCoroutine(HealPlayer());
        StartCoroutine(RegenEnergy());
        StartCoroutine(HurtImage());

        Color hurtImageColor = hurtImage.color;
        hurtImageColor.a = 0f;
        hurtImage.color = hurtImageColor;

        // recttransforms for spell cooldown displays
        fBallRT = fireballCooldownImage.rectTransform;
        eArcRT = electricArcCooldownImage.rectTransform;
        rotateRT = rotateCooldownImage_1.rectTransform;

        // heights of cooldown display images
        fBallImageHeight = fBallRT.rect.height;
        eArcImageHeight = eArcRT.rect.height;
        rotateImageHeight = rotateRT.rect.height;

        // start coroutine to get screen dimensions after first frame renders
        StartCoroutine(GetScreenDimensions());

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

        UpdateScore();
        scoreText.text = FormatScore(scoreDisplay);

        debugText.text = string.Format("{0}\n{1}\n{2}", fireballCurrentCooldown, energyCooldown, prediction);
    }
    private IEnumerator GetScreenDimensions()
    {
        yield return null;

        screenWidth = Screen.width;
        screenHeight = Screen.height;
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
                facingImage.rectTransform.Rotate(Vector3.forward, 90f, Space.World);
                rotateCurrentCooldown = rotateCooldown;
            }
        } else if (prediction == "pointer_right") {
            if (rotateCurrentCooldown <= 0f) {
                // rotate 90 deg right
                cam.transform.Rotate(Vector3.up, 90f, Space.World);
                facingImage.rectTransform.Rotate(Vector3.forward, -90f, Space.World);
                rotateCurrentCooldown = rotateCooldown;
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
    // managing player damage
    public void Damage(float amount, Vector3 enemyPos) {
        if (health - amount <= 0) {
            Lose();
        } else {
            health -= amount;
            healingCooldown = timeHealAfterDamage;
            
            Vector3 playerForward = cam.transform.forward;
            Vector3 playerToDamager = enemyPos - transform.position;
            playerToDamager.y = 0f;
            
            float angle = Vector3.SignedAngle(playerForward, playerToDamager, Vector3.up);
            GameObject img = Instantiate(hurtDirectionImage, hurtDirectionImageParent);
            img.GetComponent<RectTransform>().Rotate(Vector3.forward, -angle, Space.World);
            if (!healthBarShake.isPlaying) {
                healthBarShake.Play();
            }
        }
    }
    public void Lose() {
        health = 0f;
        gameOverPanel.SetActive(true);
        GameOver();
    }
    public void Win() {
        winScreen.SetActive(true);
        GameOver();
    }
    public void GameOver() {
        gameOverScoreText.gameObject.SetActive(true);
        gameOverScoreText.text = String.Format("Score: {0}", FormatScore(scoreTarget));
        Time.timeScale = 0f;
    }
    // managing player score
    public void AddScore(int amount) => scoreTarget += amount;    
    private void UpdateScore()
    {
        if (scoreTarget > scoreDisplay) {
            int growth = Mathf.Max((scoreTarget - scoreDisplay) / 30, growthAmount);
            if (scoreDisplay + growth < scoreTarget) {
                scoreDisplay += growth;
            } else {
                scoreDisplay = scoreTarget;
            }
        }
    }
    private string FormatScore(int score) {
        string formattedScore = score.ToString("D6");
        return(String.Format("{0:n0}", int.Parse(formattedScore)));
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
    private IEnumerator HurtImage() {
        while (true) {
            yield return new WaitForSeconds(0.1f);
            float alpha = 0f;
            if (health < (hurtThreshold * maxHealth)) {
                if (health < ((hurtThreshold / 2) * maxHealth)) {
                    alpha = 1.0f;
                } else {
                    alpha = 0.5f;
                }
            }
            Color hurtImageColor = hurtImage.color;
            hurtImageColor.a = alpha;
            hurtImage.color = hurtImageColor;
        }
    }
}
