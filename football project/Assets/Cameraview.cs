using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameraview : MonoBehaviour
{
    public Transform target;
    public float height=20.0f;
    public float distance=15.0f;

    public float Yloca;

    // Start is called before the first frame update
    void Start()
    {
       Yloca=target.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position=target.position;
        transform.position=new Vector3(transform.position.x,Yloca+height,transform.position.z-distance);
        transform.LookAt(target.position);
         
    }
}
