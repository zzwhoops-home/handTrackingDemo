using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnMonsters : MonoBehaviour
{
    public GameObject[] spawnPoints;

    public GameObject[] monsters;
    private int round = 0;
    public int numRounds = 10;
    public float spawnRadius = 3f;
    private bool roundEnd = false;
    private PlayerController playerController;
    public TextMeshProUGUI roundText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
        StartCoroutine(RoundDetector());

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private IEnumerator Spawn() 
    {
        for (int i = 0; i < numRounds; i++) {
            round++;
            roundText.text = string.Format("Round: {0}/{1}", round, numRounds);
            for (int m = 0 ; m < round * 3; m++) {
                // spawn from random spawnpoint in world
                int spawn = Random.Range(0, 4);
                Transform spawnPoint = spawnPoints[spawn].transform;
                
                // add randomness in spawn
                Vector3 randomSpawnPoint = spawnPoint.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
                
                // spawn monster
                Instantiate(monsters[0], randomSpawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
            }
            if (round == numRounds) {
                yield return new WaitUntil(() => roundEnd);
                playerController.Win();
            } else {
                yield return new WaitUntil(() => roundEnd);
                yield return new WaitForSeconds(10f);
            }
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
    public int GetRound() => round;
}
