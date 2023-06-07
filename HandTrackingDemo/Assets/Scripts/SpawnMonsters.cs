using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonsters : MonoBehaviour
{
    public GameObject[] spawnPoints;

    public GameObject[] monsters;
    private int round = 0;
    private bool roundEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
        StartCoroutine(RoundDetector());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Spawn() 
    {
        round++;
        for (int i = 0 ; i < round * 5; i++) {
            // spawn from random spawnpoint in world
            int spawn = Random.Range(0, 4);
            Transform spawnPoint = spawnPoints[spawn].transform;
            // spawn monster
            Instantiate(monsters[0], spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitUntil(() => roundEnd);
        yield return new WaitForSeconds(10f);
    }
    private IEnumerator RoundDetector()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Monster");
        int count = enemies.Length;
        if (count > 0) {
             roundEnd = false;
        } else {
            roundEnd = true;
        }
    }
}
