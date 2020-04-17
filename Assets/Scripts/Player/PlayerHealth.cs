using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    /*ELEMENTAL TLDR
* Fire Water Lightning
* FWL
* 000 = 0 = None
* 100 = 4 = Fire
* 010 = 2 = Water
* 001 = 1 = Lightning
* 110 = 6 = Steam
* 101 = 5 = Fiery Lightning
* 011 = 3 = Water Lightning
* 111 = 7 = Storm?
* 
* 
* */

    public const int ELEMENTLESS = 0;
    public const int FIRE = 4;
    public const int WATER = 2;
    public const int LIGHTNING = 1;
    public const int FIREWATER = 6;
    public const int WATERLIGHTNING = 3;
    public const int FIRELIGHTNING = 5;
    public const int FIREWATERLIGHTNING = 7;

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
    private PlayerMovement2 movementScript;
    private bool isBurning = false;
    private ElementalEffects elementScript;

    Animator anim;
    bool isDead;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = startingHealth;
        sprite = GetComponent<SpriteRenderer>();
        originalMaterial = sprite.material;
        rigidbody = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovement2>();
        elementScript = GetComponent<ElementalEffects>(); //Pick up the element of the player
    }

    void Update()
    {
        if ((elementScript.element & FIRE) == FIRE && !isBurning)
        {
            isBurning = true;
            StartCoroutine(Burn(3, 5));
        }
        if (Input.GetKey(KeyCode.K))
        {
            this.TakeDamage(5);
        }
        healthSlider.value = currentHealth;
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
        movementScript.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            knockbackDirection = new Vector2(collision.transform.position.x - transform.position.x, collision.transform.position.y - transform.position.y); //Force direction to apply that pushes player away from enemy
            StartCoroutine(Knockback(knockbackTime));
            if (!invincible)
            { 
                TakeDamage(5);
                invincible = true;
                StartCoroutine(IFrames(invincibilityTime));
            }
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
        print("force " + knockbackForce);
        print("direction " + knockbackDirection);
        rigidbody.AddForce(-knockbackForce * knockbackDirection, ForceMode2D.Impulse);
        movementScript.LockMovement();
        yield return new WaitForSeconds(knockbackTime);
        movementScript.UnlockMovement();
        rigidbody.AddForce(knockbackForce*knockbackDirection, ForceMode2D.Impulse);
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0;
    }

    IEnumerator Burn(float time, int tickDmg)                //Slowly take health from player until burn subsides
    {
        if (time > 0)
        {
            while (time-- > 0)
            {
                currentHealth = currentHealth - tickDmg;
                print(currentHealth);
                yield return new WaitForSeconds(1);
            }
        }
        isBurning = false;
        elementScript.element = elementScript.element & 3; //Remove fire
    }
}