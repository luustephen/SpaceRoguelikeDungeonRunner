using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;

    Animator anim;
    PlayerMovement playerMovement;
    bool isDead;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        currentHealth = startingHealth;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            this.TakeDamage(5);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthSlider.value = currentHealth;
        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;
        playerMovement.enabled = false;
    }
}