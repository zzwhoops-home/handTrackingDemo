using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricArcShoot : MonoBehaviour
{    
    public float lifetime = 1f;
    private float speed;
    private float distance;
    public float arcHeight;
    public float lerpTime;
    public float maxLength;
    public GameObject pos1;
    public GameObject pos2;
    public GameObject pos3;
    public GameObject pos4;
    private Vector3 pos4Original;
    private ShootFromHand shootFromHand;

    public float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        shootFromHand = GameObject.FindGameObjectWithTag("Player").GetComponent<ShootFromHand>();
        StartCoroutine(TryRaycastTarget());
        StartCoroutine(DestroyObject());

        pos4Original = pos4.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private IEnumerator TryRaycastTarget() {
        Ray ray = shootFromHand.handRay;
        RaycastHit hitData;

        while (true) {
            float cooldown = 0.05f;
            if (Physics.Raycast(ray, out hitData, maxLength)) {
                var collider = hitData.collider;
                if (collider.CompareTag("Monster")) {
                    pos4.transform.position = hitData.point;
                    collider.gameObject.GetComponent<Damageable>().Damage(damage * cooldown);
                    distance = hitData.distance;
                    LightningCurve();
                }
            }
            yield return new WaitForSeconds(cooldown);
        }
    }
    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(lifetime);
        pos4.transform.position = pos4Original;
        Destroy(gameObject);
    }
    private void LightningCurve() {
        Vector3 min = pos1.transform.position;
        Vector3 max = pos4.transform.position;

        float distanceOffset = distance * arcHeight;

        pos2.transform.position = ((max - min) * 0.33f) + new Vector3(0f, distanceOffset, 0f);
        pos3.transform.position = ((max - min) * 0.66f) + new Vector3(0f, distanceOffset, 0f);
    }
}
