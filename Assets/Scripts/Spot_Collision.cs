using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot_Collision : MonoBehaviour
{
    public bool isCollision; //충돌여부

    void Start()
    {
        isCollision = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("충돌함"+ collision.gameObject.tag);
        if (/*collision.gameObject.name == "Plane" ||*/collision.gameObject.tag == "leg")
        {
            //Debug.Log("이름:" + collision.gameObject.name);
            //isCollision = true;
        }
    }
}
