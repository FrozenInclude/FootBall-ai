using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public player Target;

    public Transform test;
  
    public Transform TargetTransform;

    public float spd=25f;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 testv=(test.position-transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(testv*20,ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
    }
   
    void FixedUpdate()
    {
        if(Target!=null){
       var d = (TargetTransform.position-transform.position).normalized;
       transform.forward=d;
       var vec=new Vector3(d.x,Vector3.down.y*0.1f,d.z);
     
       GetComponent<Rigidbody>().velocity=vec*spd;
       GetComponent<Rigidbody>().AddTorque(transform.root.right*500, ForceMode.Impulse);

      //transform.Rotate(Vector3.up,45.0f);
        }
    }
       void OnCollisionEnter(Collision col){
         Target=null;
    }

}
