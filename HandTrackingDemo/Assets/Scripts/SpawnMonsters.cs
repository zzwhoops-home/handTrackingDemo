using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonsters : MonoBehaviour
{
    public GameObject[] spawnPoints;

    public GameObject[] monsters;
    private int round = 0;
    public int numRounds = 10;
    public float spawnRadius = 0.5f;
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
        for (int i = 0; i < numRounds; i++) {
            round++;
            for (int m = 0 ; m < round * 5; m++) {
                // spawn from random spawnpoint in world
                int spawn = Random.Range(0, 4);
                Transform spawnPoint = spawnPoints[spawn].transform;
                
                // add randomness in spawn
                Vector3 randomSpawnPoint = spawnPoint.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
                
                // spawn monster
                Instantiate(monsters[0], randomSpawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitUntil(() => roundEnd);
            yield return new WaitForSeconds(10f);
        }
    }
    private IEnumerator RoundDetector()
    {
        // if any monster remains, do not continue rounds
        while (true) {
            yield return new WaitForSeconds(0.5f);
            GameObject enemy = GameObject.FindGameObjectWithTag("Monster");
            if (enemy) {
                roundEnd = false;
            } else {
                roundEnd = true;
            }
        }
    }
}
