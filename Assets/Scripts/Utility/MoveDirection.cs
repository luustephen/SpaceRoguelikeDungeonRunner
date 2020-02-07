using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : MonoBehaviour
{
    public float xDistance = 1;
    public float yDistance = 1;
    public float xSpeed = 1;
    public float ySpeed = 1;
    public float waitTime = 0; //Time to wait after loading before moving
    private Vector2 directionVectorX;
    private Vector3 directionVectorY;
    private float changeInX; //Change in the x movement per frame updated
    private float changeInY; //Change in the y movement per frame updated
    private float originalSignX;
    private float originalSignY;

    // Start is called before the first frame update
    void Start()
    {
        originalSignX = Mathf.Sign(xDistance);
        originalSignY = Mathf.Sign(yDistance);
        changeInX = xDistance / (1/xSpeed);
        changeInY = yDistance / (1/ySpeed);
        directionVectorX = new Vector2(changeInX, 0);
        directionVectorY = new Vector2(0, changeInY);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime == 0)
        {
            if (Mathf.Abs(xDistance) > 0)
            {
                transform.Translate(directionVectorX);
                xDistance -= changeInX;
                if (originalSignX != Mathf.Sign(xDistance))
                    xDistance = 0;
            }

            if (Mathf.Abs(yDistance) > 0)
            {
                transform.Translate(directionVectorY);
                yDistance -= changeInY;
                if (originalSignY != Mathf.Sign(yDistance))
                    yDistance = 0;
            }
        }
        else
            waitTime--;
    }
}
