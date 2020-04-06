using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyChargeShot : MonoBehaviour
{
    private GameObject player;
    private PlayerAttack playerAttackScript;
    private float startTime;
    private float timeElapsed;
    public float sizeIncreaseNum = 0.05f;

    // Use this for initialization
    void Start() //Get player attack script
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttackScript = player.GetComponent<PlayerAttack>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime < playerAttackScript.chargeTimeFull)
        {
            transform.localScale = new Vector3(transform.localScale.x + sizeIncreaseNum, transform.localScale.y + sizeIncreaseNum , transform.localScale.z);
        }
        if(playerAttackScript.GetChargeShot() && playerAttackScript.GetChargeTime() == Time.time)
        {
            Destroy(gameObject);
        }
    }
}
