using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Damageable : MonoBehaviour
{
    public GameObject dmgIndicator;
    public GameObject player;
    private PlayerController playerController;
    private float dmgIndicatorOffset_X = 1f;
    private float dmgIndicatorOffset_Y = 0.3f;
    private Collider objCollider;
    public float maxHealth = 20.0f;
    public float force = 3.0f;
    public float distanceMultiplier = 5.0f;
    private float health;
    public Slider healthBar;
    // get current round in start function
    private int round;
    public GameObject mMonsterDestroy;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        health = maxHealth;
        healthBar.value = 1f;

        objCollider = gameObject.GetComponent<Collider>();
        round = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SpawnMonsters>().GetRound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damage(float amount) {
        CreateDMGIndicator(amount);

        health -= amount;
        if (health <= 0) {
            SelfDestroyed();
        }
        healthBar.value = health / maxHealth;
    }
    public Vector3 RandomCoords() {
        
        float x = Random.Range(-dmgIndicatorOffset_X, dmgIndicatorOffset_X);
        float y = Random.Range(0f, dmgIndicatorOffset_Y) + (objCollider.bounds.size.y);
        float z = Random.Range(-dmgIndicatorOffset_X, dmgIndicatorOffset_X);
        Vector3 offset = new Vector3(x, y, z);

        return gameObject.transform.position + offset;
    }
    public void CreateDMGIndicator(float amount) {
        GameObject ind = Instantiate(dmgIndicator, RandomCoords(), (gameObject.transform.rotation * Quaternion.Euler(0f, 180f, 0f)));
        ind.GetComponentInChildren<TextMeshProUGUI>().text = (Mathf.Round(amount * 10) / 10).ToString();
        ind.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-force, force), 0f, Random.Range(-force, force)));
    }
    public void SelfDestroyed() {
        if (gameObject.CompareTag("Monster")) {
            Instantiate(mMonsterDestroy, transform.position, Quaternion.identity);
        }
        float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
        int value = (50 + (int) (distance * distanceMultiplier)) + (round * 25);
        playerController.AddScore(value);
        Destroy(gameObject);
    }
    public void SelfDestroyedNoKill() {
        if (gameObject.CompareTag("Monster")) {
            Instantiate(mMonsterDestroy, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
