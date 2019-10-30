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
    private int numSwingHitboxes;               //Odd number of swing hitboxes to create during swing
    private float swingIncrementWidth;
    public float swingWidth;
    public Rigidbody2D projectilePrefab;
    public Rigidbody2D projectileInstance;
    private Camera mainCamera;
    public float projectileSpeed = 1000;
    public int projectileElement = ELEMENTLESS;
    public GameObject projectileElementChangedBy;
    private int numShots = 1; //Number of shots per click + 1 (cause im an idiot)
    private bool followUpShot = false; //Whether the shot to be fired is automatically fired or player fired


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

        if (Input.GetKeyDown(secondaryAttack))          //Start cooldown on secondary attack
        {
            SecondaryAttack(numShots-1); //Compensation for bad programming, see numShots description
        }
    }

    public void PrimaryAttack()
    {
        if (!primaryOnCooldown && !secondaryOnCooldown)
        {
            primaryOnCooldown = true;
            Vector3 mousePosition = Input.mousePosition;
            float angle = Mathf.Atan2(mousePosition.y - mainCamera.WorldToScreenPoint(transform.position).y, mousePosition.x - mainCamera.WorldToScreenPoint(transform.position).x);
            float y = Mathf.Sin(angle);
            float x = Mathf.Cos(angle);
            attackHitbox.Translate(new Vector3(x, y, 0));
            attackHitbox.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * angle + 90));
            attackSprite.enabled = true;
            attackCollider.enabled = true;
            StartCoroutine(PrimaryCooldown(angle));
        }
    }

    public void SecondaryAttack(int shotsLeft)
    {
        if (!secondaryOnCooldown && !primaryOnCooldown || followUpShot)
        {
            Vector3 mousePosition = Input.mousePosition;
            projectileInstance = Instantiate(projectilePrefab, attackHitbox.position, Quaternion.identity) as Rigidbody2D;

            if (projectileInstance.GetComponent<ElementalEffects>() != null) //make sure projectile can have an element
                projectileInstance.GetComponent<ElementalEffects>().element = projectileInstance.GetComponent<ElementalEffects>().element | projectileElement;

            float angle = Mathf.Atan2(mousePosition.y - mainCamera.WorldToScreenPoint(transform.position).y, mousePosition.x - mainCamera.WorldToScreenPoint(transform.position).x);
            float y = Mathf.Sin(angle);
            float x = Mathf.Cos(angle);
            projectileInstance.AddForce(new Vector3(x, y, 0) * projectileSpeed);
            projectileInstance.transform.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * angle + 90));

            secondaryOnCooldown = true;
            //secondaryattackSprite.enabled = true;
            //secondaryattackspriteCollider.enabled = true;
            //projectile.GetComponent<Rigidbody>().transform
            //projectile.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, Mathf.Rad2Deg * angle + 90));

            //rigidBody.useGravity = true;
            //rigidBody.MovePosition(mousePosition);
            StartCoroutine(SecondaryCooldown(shotsLeft));
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
            SecondaryAttack(shotsLeft - 1);
        }
    }

    public void IncrementShots()
    {
        numShots++;
    }
}
