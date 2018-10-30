using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public KeyCode primaryAttack, secondaryAttack;
    private bool primaryOnCooldown, secondaryOnCooldown;
    public float primaryAttackCooldown, secondaryAttackCooldown;
    private Transform attackHitbox;
    private SpriteRenderer attackSprite;
    private BoxCollider attackCollider;
    private SphereCollider secondaryattackspriteCollider;
    private int numSwingHitboxes;               //Odd number of swing hitboxes to create during swing
    private float swingIncrementWidth;
    public float swingWidth;
    public Rigidbody2D projectilePrefab;
    public Rigidbody2D projectileInstance;


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
        attackHitbox = gameObject.transform.GetChild(1);
        attackSprite = attackHitbox.gameObject.GetComponent<SpriteRenderer>();
        attackCollider  = attackHitbox.gameObject.GetComponent<BoxCollider>();
        numSwingHitboxes = 5;
        swingIncrementWidth = swingWidth/numSwingHitboxes;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(primaryAttack))            //Move hitbox into swinging position and enable it
        {
            if (!primaryOnCooldown && !secondaryOnCooldown)
            {
                primaryOnCooldown = true;
                Vector3 mousePosition = Input.mousePosition;
                float angle = Mathf.Atan2(mousePosition.y - Screen.height / 2, mousePosition.x - Screen.width / 2);
                float y = Mathf.Sin(angle);
                float x = Mathf.Cos(angle);
                attackHitbox.Translate(new Vector3(x, y, 0));
                attackHitbox.Rotate(new Vector3(0 ,0, Mathf.Rad2Deg * angle + 90));
                attackSprite.enabled = true;
                attackCollider.enabled = true;
                StartCoroutine("PrimaryCooldown", angle);
            }
        }

        if (Input.GetKeyDown(secondaryAttack))          //Start cooldown on secondary attack
        {
            if (!secondaryOnCooldown && !primaryOnCooldown)
            {
                Vector3 mousePosition = Input.mousePosition;
                projectileInstance = Instantiate(projectilePrefab, attackHitbox.position, Quaternion.identity) as Rigidbody2D;
                float angle = Mathf.Atan2(mousePosition.y - Screen.height / 2, mousePosition.x - Screen.width / 2);
                float y = Mathf.Sin(angle);
                float x = Mathf.Cos(angle);
                projectileInstance.AddForce(new Vector3(x, y, 0) * 1000);

                secondaryOnCooldown = true; 
                //secondaryattackSprite.enabled = true;
                //secondaryattackspriteCollider.enabled = true;
                //projectile.GetComponent<Rigidbody>().transform
                //projectile.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, Mathf.Rad2Deg * angle + 90));

                //rigidBody.useGravity = true;
                //rigidBody.MovePosition(mousePosition);
                StartCoroutine("SecondaryCooldown");
            }
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

    IEnumerator SecondaryCooldown()
    {
        yield return new WaitForSeconds(secondaryAttackCooldown);
        secondaryOnCooldown = false;
    }
}
