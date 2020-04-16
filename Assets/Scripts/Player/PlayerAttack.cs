using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

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

    public KeyCode primaryAttack, secondaryAttack;
    private bool primaryOnCooldown, secondaryOnCooldown;
    public float primaryAttackCooldown, secondaryAttackCooldown;
    private Transform attackHitbox;
    private SpriteRenderer attackSprite;
    private BoxCollider2D attackCollider;
    private SphereCollider secondaryattackspriteCollider;
    private int numSwingHitboxes;   //Odd number of swing hitboxes to create during swing
    private float swingIncrementWidth;
    public float swingWidth;
    public Rigidbody2D projectilePrefab;
    public Rigidbody2D projectileInstance;
    public Rigidbody2D chargeSprite; //Sprite that appears when charging a shot
    private Rigidbody2D chargeSpriteInstance;
    private Camera mainCamera;
    public float projectileSpeed = 1000;
    public int projectileElement = ELEMENTLESS;
    public GameObject projectileElementChangedBy;
    private int numShots = 1; //Number of shots per click 
    private int numProjectilesPerShot = 1; //Number of projectiles fired per click
    private bool followUpShot = false; //Whether the shot to be fired is automatically fired or player fired
    private float spread = Mathf.PI / 6; //Degrees of spread on split shot in radians (60 degrees)
    private bool isHoming = false; //do shots home onto targets
    private float projectileWidthModifier = 1.0f; //How fat the projectile is
    public int maxBounces = 0; //Number of bounces the projectile bounces
    private bool isExplosive = false;
    private bool chargeShot = false;
    public float chargeTimeFull = .5f; //Minimum time needed to be able to fire a charged shot
    private float chargeTime = 0; //Time the player has held down charge, must be greater than chargetimefull to fire


    // Use this for initialization
    void Start ()
    {
        /* 
         * primaryAttack = KeyCode.Mouse0
         * secondaryAttack = KeyCode.Mouse1;
         */
        primaryOnCooldown = false;
        secondaryOnCooldown = false;
        //primaryAttackCooldown = .2f;
        //secondaryAttackCooldown = .3f;
        attackHitbox = transform.GetChild(1); //Find another way of doing this
        attackSprite = attackHitbox.gameObject.GetComponent<SpriteRenderer>();
        attackCollider  = attackHitbox.gameObject.GetComponent<BoxCollider2D>();
        numSwingHitboxes = 5;
        swingIncrementWidth = swingWidth/numSwingHitboxes;
        mainCamera = transform.GetChild(0).GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(primaryAttack))            //Move hitbox into swinging position and enable it
        {
            PrimaryAttack();
        }

        if ((Input.GetKeyDown(secondaryAttack) && !chargeShot) || (Time.time-chargeTime > chargeTimeFull && Input.GetKeyUp(secondaryAttack)))          //Fire projectile and start cooldown on secondary attack
        {
            SecondaryAttack(numShots); 
        }
        else if (chargeShot && Input.GetKey(secondaryAttack)) //If charge shot, create effect to signify it and count time before it can fire
        {
            float angle = GetMouseAngleToPlayer();
            float y = Mathf.Sin(angle);
            float x = Mathf.Cos(angle);
            if (Input.GetKeyDown(secondaryAttack))
            {
                chargeTime = Time.time;
                if (chargeSprite) //Create effect
                {
                    chargeSpriteInstance = Instantiate(chargeSprite, attackHitbox.position, Quaternion.identity, transform);
                    chargeSpriteInstance.transform.Translate(x, y, 0);
                }
            }
            else if(chargeSpriteInstance) //Make the charge effect follow the player cursor
            {
                chargeSpriteInstance.transform.SetPositionAndRotation(new Vector3(transform.position.x+x, transform.position.y+y, transform.position.z),chargeSpriteInstance.transform.rotation);
            }
        }
        else //Reset time secondary atk button is held down
        {
            chargeTime = Time.time;
        }
    }

    public void PrimaryAttack()
    {
        if (!primaryOnCooldown && !secondaryOnCooldown)
        {
            primaryOnCooldown = true;
            float angle = GetMouseAngleToPlayer();
            float y = Mathf.Sin(angle);
            float x = Mathf.Cos(angle);
            attackHitbox.Translate(new Vector3(x, y, 0));
            attackHitbox.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * angle + 90)); //+ 90
            attackSprite.enabled = true;
            attackCollider.enabled = true;
            StartCoroutine(PrimaryCooldown(angle));
        }
    }

    public void SecondaryAttack(int shotsLeft)
    {
        if (!secondaryOnCooldown && !primaryOnCooldown || (followUpShot && numShots > 0)) //Fire if primary/secondary attacks are not on cooldown or if followup shot is the one being fired
        {
            Vector3 mousePosition = Input.mousePosition;
            float radIncrement = spread / numProjectilesPerShot; //Degrees apart each shot should be from another if firing multiple projectiles

            for (int i = 0; i < numProjectilesPerShot; i++)
            {
                projectileInstance = Instantiate(projectilePrefab, attackHitbox.position, Quaternion.identity) as Rigidbody2D;

                if (projectileInstance.GetComponent<ElementalEffects>() != null) //make sure projectile can have an element
                    projectileInstance.GetComponent<ElementalEffects>().element = projectileInstance.GetComponent<ElementalEffects>().element | projectileElement;

                float angle = GetMouseAngleToPlayer();

                if(numProjectilesPerShot > 1 && numProjectilesPerShot%2 == 1) //If we shoot more than one projectile, offset their angle and direction fired by the number of shots
                    angle = angle - spread + (radIncrement * (i+1)) + (radIncrement*(numProjectilesPerShot/2)); //There is a different formula for even/odd number of projectiles
                else if(numProjectilesPerShot > 1 && numProjectilesPerShot % 2 == 0)
                    angle = angle - spread + (radIncrement * (i+1)) + ((radIncrement/2)*(numProjectilesPerShot/2)); //TODO maybe fix this formula up later, seems way to complex for what it does

                float y = Mathf.Sin(angle);
                float x = Mathf.Cos(angle);
                //print("i: " + i + " Angle: " + Mathf.Rad2Deg*angle + " RadIncrement: " + Mathf.Rad2Deg * radIncrement);
                projectileInstance.AddForce(new Vector3(x , y, 0) * projectileSpeed);
                projectileInstance.transform.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * angle + 90));

                PlayerProjectile projectileScript = projectileInstance.GetComponent<PlayerProjectile>();
                if (isHoming && projectileScript != null) //Pass on homing projectile
                {
                    projectileScript.SetHomingShots(true);
                }

                if(maxBounces > 0) //Pass on bounces 
                {
                    projectileScript.SetBounces(maxBounces);
                }

                if (isExplosive) //Pass on explosive
                {
                    projectileScript.ShouldExplode(isExplosive);
                }

                //Set width of projectile to the modifier
                projectileInstance.transform.localScale = new Vector3(projectileInstance.transform.localScale.x * projectileWidthModifier, projectileInstance.transform.localScale.y, projectileInstance.transform.localScale.z);
                //print(projectileInstance.transform.localScale);
            }

            secondaryOnCooldown = true;
            StartCoroutine(SecondaryCooldown(shotsLeft-1)); //Start routine to put a cooldown on secondary atk
        }
    }


    IEnumerator PrimaryCooldown(float angle)            //Wait for attack to end then move hitbox back and disable it
    {
        float y = Mathf.Sin(angle);
        float x = Mathf.Cos(angle);
        float returnX = attackHitbox.position.x;
        attackHitbox.Translate(new Vector3( -numSwingHitboxes/2 * swingIncrementWidth, 0, 0));
        yield return new WaitForSeconds(primaryAttackCooldown / numSwingHitboxes);
        for (int i = 0; i < numSwingHitboxes; i++) {
            attackHitbox.Translate(new Vector3(swingIncrementWidth, 0, 0));
            yield return new WaitForSeconds(primaryAttackCooldown / numSwingHitboxes);
        }
        attackHitbox.Translate(new Vector3(-swingIncrementWidth * ((numSwingHitboxes / 2) + 1), 0, 0));
        attackSprite.enabled = false;
        attackCollider.enabled = false;
        attackHitbox.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * -angle - 90));
        attackHitbox.Translate(new Vector3(-x, -y, 0));
        primaryOnCooldown = false;
    }

    IEnumerator SecondaryCooldown(int shotsLeft)
    {
        yield return new WaitForSeconds(secondaryAttackCooldown);

        if (shotsLeft == 0)
        {
            followUpShot = false;
            secondaryOnCooldown = false;
        }
        else
        {
            followUpShot = true;
            SecondaryAttack(shotsLeft);
        }
    }

    public void IncrementShots()
    {
        numShots++;
    }

    public void DecrementShots()
    {
        numShots--;
    }

    public void IncrementProjectilesPerShot()
    {
        numProjectilesPerShot++;
    }

    public void DecrementProjectilesPerShot()
    {
        numProjectilesPerShot--;
    }

    public void SetHomingShots(bool value)
    {
        isHoming = value;
    }

    public bool hasHomingShots()
    {
        return isHoming;
    }

    public void SetProjectileWidth(float width)
    {
        projectileWidthModifier = width;
    }

    public void IncreaseBounces()
    {
        maxBounces++;
    }

    public void SetExplosiveShot(bool shouldExplode)
    {
        isExplosive = shouldExplode;
    }

    public void SetChargeShot(bool shouldCharge)
    {
        chargeShot = shouldCharge;
    }

    public bool GetChargeShot()
    {
        return chargeShot;
    }

    public float GetChargeTime()
    {
        return chargeTime;
    }

    private float GetMouseAngleToPlayer()
    {
        return Mathf.Atan2(Input.mousePosition.y - mainCamera.WorldToScreenPoint(transform.position).y, Input.mousePosition.x - mainCamera.WorldToScreenPoint(transform.position).x);
    }
}
