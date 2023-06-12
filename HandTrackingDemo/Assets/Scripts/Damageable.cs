using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Damageable : MonoBehaviour
{
    public GameObject dmgIndicator;
    private float dmgIndicatorOffset_X = 1f;
    private float dmgIndicatorOffset_Y = 0.3f;
    private Collider objCollider;
    public float maxHealth = 20.0f;
    public float force = 3.0f;
    private float health;
    public Slider healthBar;
    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.value = 1f;

        objCollider = gameObject.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damage(float amount) {
        CreateDMGIndicator(amount);

        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
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
}
