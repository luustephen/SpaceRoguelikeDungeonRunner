using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;

    Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        currentHealth = startingHealth;
	}
	
	// Update is called once per frame
	void Update () {
        this.TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthSlider.value = currentHealth;
    }
}