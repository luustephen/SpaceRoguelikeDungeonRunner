using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float timeToDestroy = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroy(timeToDestroy));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator destroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
