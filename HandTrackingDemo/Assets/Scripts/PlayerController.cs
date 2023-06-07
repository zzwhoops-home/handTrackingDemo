using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float maxHealth = 100.0f;
    private float health;
    public float maxEnergy = 200.0f;
    private float energy;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI energyText;

    // Start is called before the first frame update
    void Start()
    {
        // at the start, player starts at max HP
        health = maxHealth;
        energy = maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = string.Format("Health: {0}", health);
        energyText.text = string.Format("Energy: {0}", energy);
    }
}
