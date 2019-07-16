using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAI : MonoBehaviour {

    private GameObject player;
    private Room room; //Room that the enemy resides in
    private bool firstpass = true;
    public float speed = .1f;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (firstpass)
        {
            if (!transform.parent)
            {
                Destroy(gameObject);
            }
            else
            {
                room = transform.parent.GetComponent<Room>();
            }
            firstpass = false;
        }
        if (player && room && room.InsideRoom(player))
        {
            Vector3 normalizedDirection = (player.transform.position - transform.position).normalized;
            print(normalizedDirection);
            transform.Translate(normalizedDirection * speed);
            
        }
    }

    float GetDistanceToPlayer()
    {
        float distance = Mathf.Pow(player.transform.position.y - transform.position.y, 2) + Mathf.Pow(player.transform.position.x - transform.position.x, 2);
        return distance;
    }
}
