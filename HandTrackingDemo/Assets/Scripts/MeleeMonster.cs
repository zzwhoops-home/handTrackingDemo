using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeMonster : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    public float minSpeed = 1.5f;
    public float maxSpeed = 2.0f;
    [SerializeField]
    private float speed;
    public float damage = 10.0f;
    public float offset = 0.75f;
    public float maxHealth = 20.0f;
    private float health;
    public Slider healthBar;
    private Vector3 destination;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.value = 1f;

        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        speed = Random.Range(minSpeed, maxSpeed);
        destination = PlayerLocation();
        transform.LookAt(destination);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private Vector3 PlayerLocation() {
        // add some randomness to movement, 
        Vector3 randomOffset = new Vector3(Random.Range(-offset, offset), 0f, Random.Range(-offset, offset));
        Vector3 target = new Vector3(player.transform.position.x + randomOffset.x, gameObject.transform.position.y, player.transform.position.z + randomOffset.z);

        return target;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCollider")) {
            playerController.Damage(damage);
            Destroy(gameObject);
        }
    }
    public void Damage(float amount) {
        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
        }
        healthBar.value = health / maxHealth;
    }
}
