using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballShoot : MonoBehaviour
{
    public float lifetime = 5f;
    public float minSpeed = 4f;
    public float maxSpeed = 5f;
    private float speed;

    public float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    void OnCollisionEnter(Collision collisionInfo)
    {
        GameObject colliderObject = collisionInfo.gameObject;
        if (colliderObject.CompareTag("Monster")) {
            colliderObject.GetComponent<MeleeMonster>().Damage(damage);
            Destroy(gameObject);
        }
    }
}
