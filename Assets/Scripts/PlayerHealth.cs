﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public float invincibilityTime = 2;
    private bool invincible = false;
    public float blinkTime = .3f;
    public Material transparent;
    private Material originalMaterial;
    private SpriteRenderer sprite;
    private Rigidbody2D rigidbody;
    private Vector2 knockbackDirection;
    public float knockbackTime = .3f;
    public float knockbackForce = 5;
    private PlayerMovement movementScript;

    Animator anim;
    PlayerMovement playerMovement;
    bool isDead;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        currentHealth = startingHealth;
        sprite = gameObject.GetComponent<SpriteRenderer>();
        originalMaterial = sprite.material;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        movementScript = gameObject.GetComponent<PlayerMovement>();
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
        print(currentHealth);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy" && !invincible)
        {
            this.TakeDamage(5);
            invincible = true;
            knockbackDirection = new Vector2(collision.transform.position.x - transform.position.x,collision.transform.position.y - transform.position.y); //Force direction to apply that pushes player away from enemy
            StartCoroutine(Knockback(knockbackTime));
            StartCoroutine(IFrames(invincibilityTime));
            print(knockbackTime);
        }
    }

    IEnumerator IFrames(float iTime)
    {
        for (float t = iTime; t > 0; t = t - blinkTime)
        {
            if(sprite.material == originalMaterial)
                sprite.material = transparent;
            else
                sprite.material = originalMaterial;

            yield return new WaitForSeconds(blinkTime);
        }

        invincible = false;
    }

    IEnumerator Knockback(float knockbackTime)
    {
        rigidbody.AddForce(-knockbackForce * knockbackDirection, ForceMode2D.Impulse);
        movementScript.LockMovement();
        yield return new WaitForSeconds(knockbackTime);
        movementScript.UnlockMovement();
        rigidbody.AddForce(knockbackForce*knockbackDirection, ForceMode2D.Impulse);
    }
}