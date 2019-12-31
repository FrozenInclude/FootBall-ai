using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foot : MonoBehaviour
{
    bool isColliding = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isColliding = false;
    }
    void OnTriggerEnter(Collider col){
        if(isColliding) return;
        isColliding = true;
        //Debug.Log(transform.root.GetComponent<control>().HaveBall);
        if(col.gameObject.name=="Soccer Ball"&& !transform.root.GetComponent<control>().HaveBall){
            transform.root.GetComponent<control>().HaveBall=true;
            transform.root.GetComponent<control>().Ballphyscis=col.gameObject.GetComponent<Rigidbody>();
            transform.root.GetComponent<control>().Ball=col.gameObject;
            col.gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
            col.gameObject.transform.position=transform.root.GetComponent<control>().발.transform.position;
        }
        if(transform.root.GetComponent<control>().HaveBall){
        if(col.gameObject.name=="Soccer Ball"){
            transform.root.GetComponent<control>().ballonfoot=true;
            Vector3 shootDir = transform.root.GetComponent<control>().DribbleDirec; //calculate delta vector
             //shootDir=transform.root.transform.forward;
             shootDir.Normalize(); //normalize
           if (transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Walk")){
           col.gameObject.GetComponent<Rigidbody>().velocity=shootDir *7.05f;
           col.gameObject.GetComponent<Rigidbody>().AddTorque(transform.root.right*500, ForceMode.Impulse);
           }else if(transform.root.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run")){
           col.gameObject.GetComponent<Rigidbody>().velocity=shootDir *10.5f;
           col.gameObject.GetComponent<Rigidbody>().AddTorque(transform.root.right*500, ForceMode.Impulse);
           }
        }
        else{
            transform.root.GetComponent<control>().ballonfoot=false;
        }
        }
    }
}
