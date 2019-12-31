using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class control : MonoBehaviour
{  
    public Vector3 DribbleDirec;
    public float lastRotionY=-90f;
    public bool ballonfoot;	
    private bool isMoving;
    private Animator anim;									
    public GameObject GoalPost;
    private player[] otherPlayers;
    private Vector3 BallPos;
    private Vector3 BallPos2;
    public GameObject Ball;
    public GameObject 발;
    public bool HaveBall=false;
   // float moveDelay=0.1f;
    float timer=0;
    public float speed=15f;
    private Rigidbody rigdbody;
    public Rigidbody Ballphyscis;
    Vector3 movement;
    Vector3 lastmovement;
    Quaternion Direction;
    passing pag=passing.Short;
    
    player passingTarget;
    Transform shotdirec;

    public GameObject backup;
   
    public enum passing
    {
    Long,
    Short,
    Tshort,
    Tlong
    }
   public static float CalculateAngle(Vector3 from, Vector3 to)
   {    
    return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
   }

   public void move(Vector3 dir,bool running)
   {
       if(!HaveBall)
       {
        Vector3 movement=dir;

        movement=movement.normalized*speed*Time.deltaTime;  

        if(!running)
        {
        rigdbody.velocity=Vector3.Lerp(rigdbody.velocity,movement*1.1f,Time.deltaTime*10);
        }
        else
        {
        rigdbody.velocity=movement*1.3f;
        }

        }
        else if(HaveBall)
        {

        DribbleDirec=transform.forward;

        Vector3 moving=dir;

        if(!Mathf.Approximately(moving.magnitude,0))
        {

         movement=(Ball.transform.position-transform.position).normalized*speed*Time.deltaTime;

        if(Vector3.Distance(transform.position,Ball.transform.position)<1f)
        { 

            Quaternion rot=Quaternion.Euler(0,Vector3.SignedAngle(Vector3.forward,moving.normalized,Vector3.up),0);

            if(Mathf.Abs(rot.eulerAngles.y-transform.eulerAngles.y)>3)
            {

             DribbleDirec=moving;
             Direction=rot;

             if(!running)
             Ball.GetComponent<Rigidbody>().velocity=DribbleDirec.normalized *4.95f;
             else
             Ball.GetComponent<Rigidbody>().velocity=DribbleDirec.normalized *5.95f;

             Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right, ForceMode.Impulse);

         }
        }
        }
        else
        {
            if(Vector3.Distance(transform.position,Ball.transform.position)>1f)
            movement=(Ball.transform.position-transform.position).normalized*speed*Time.deltaTime;
            else
            {           
                 movement=Vector3.zero;
                 Ball.GetComponent<Rigidbody>().velocity=Vector3.zero;
                 Ball.gameObject.GetComponent<Rigidbody>().angularVelocity=Vector3.zero;
            }
        }
        if(!running)
        {
        rigdbody.velocity=Vector3.Lerp(rigdbody.velocity,movement,Time.deltaTime*10);
        }else
        {
        rigdbody.velocity=movement*1.24f;
        } 										
   }
   }
   public void shot()
   {
            if(Vector3.Distance(transform.position,Ball.transform.position)<1f)
           { 
             anim.SetTrigger("shot");
           }
   }
  public void pass(player target,passing type)
   { 
          if(Vector3.Distance(transform.position,Ball.transform.position)<1f)
           { 
             anim.SetTrigger("pass");
             pag=type;
             passingTarget=target;
           }
          
   }
    void Start(){
     
    }
    void Awake(){
        anim = GetComponent<Animator> ();
        rigdbody=GetComponent<Rigidbody>();
        otherPlayers = FindObjectsOfType<player>().Where(b=>true).ToArray();
    }

    void Update(){
    
    }
    void FixedUpdate(){
        GetComponent<BoxCollider>().enabled=!HaveBall;
        if(HaveBall)
        rigdbody.constraints=(RigidbodyConstraints.FreezePositionY|RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ);
        else
        rigdbody.constraints=(RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ);

        anim.SetBool("Walk",!(rigdbody.velocity.magnitude<1));
        anim.SetBool("Run",Input.GetKey(KeyCode.E));
      
      
        float h=Input.GetAxisRaw("Horizontal");
        float v=Input.GetAxisRaw("Vertical");

        move(new Vector3(h,0,v),Input.GetKey(KeyCode.E));

        if(!HaveBall){
        var lookPos = Ball.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
        }
        else if(HaveBall)
        {
        if(!(Ball.GetComponent<Rigidbody>().velocity.magnitude<1))
        transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(Ball.GetComponent<Rigidbody>().velocity.normalized),Time.deltaTime *10.5f);
        }
        
       

        if(HaveBall){
            if(Input.GetKey(KeyCode.X))
            {
            float horizontal1 = Input.GetAxis("Horizontal");
            float vertical1 = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(horizontal1, 0f, vertical1);
            if(Mathf.Approximately(direction.magnitude,0f))direction=transform.forward;
            Vector3 dirToTarget = (GoalPost.transform.position - transform.position).normalized;
            shotdirec=new[]{GoalPost.transform.GetChild(0),GoalPost.transform.GetChild(1),GoalPost.transform.GetChild(2)}
            .OrderBy(t => Vector3.Angle(direction, Vector3.Normalize(t.transform.position - transform.position)))
            .FirstOrDefault();
             if (!(Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos((150 / 2) * Mathf.Deg2Rad)))
             {
               shotdirec=null;
             }
             shot();
            }
            else if(Input.GetKey(KeyCode.S))
            {
             float horizontal1 = Input.GetAxis("Horizontal");
             float vertical1 = Input.GetAxis("Vertical");
             Vector3 direction = new Vector3(horizontal1, 0f, vertical1);
             if(Mathf.Approximately(direction.magnitude,0f))direction=transform.forward;
             var targetPlayer=FindPlayerInDirection(direction);
             Vector3 dirToTarget = (targetPlayer.transform.position - transform.position).normalized;
             if (Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos((150 / 2) * Mathf.Deg2Rad)){
              pass(targetPlayer,passing.Short);
             }
             else
             {
              pass(null,passing.Short); 
             }
            }
            else if(Input.GetKey(KeyCode.W))
            {
             float horizontal1 = Input.GetAxis("Horizontal");
             float vertical1 = Input.GetAxis("Vertical");
             Vector3 direction = new Vector3(horizontal1, 0f, vertical1);
             if(Mathf.Approximately(direction.magnitude,0f))direction=transform.forward;
             var targetPlayer=FindPlayerInDirection(direction);
             Vector3 dirToTarget = (targetPlayer.transform.position - transform.position).normalized;
             if (Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos((150 / 2) * Mathf.Deg2Rad)){
              pass(targetPlayer,passing.Tshort);
             }
             else
             {
              pass(null,passing.Short); 
             }
            }
            else if(Input.GetKey(KeyCode.A))
            {
            float horizontal1 = Input.GetAxis("Horizontal");
             float vertical1 = Input.GetAxis("Vertical");
             Vector3 direction = new Vector3(horizontal1, 0f, vertical1);
             if(Mathf.Approximately(direction.magnitude,0f))direction=transform.forward;
             var targetPlayer = FindPlayerInDirection(direction);
             Vector3 dirToTarget = (targetPlayer.transform.position - transform.position).normalized;
             if (Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos((150 / 2) * Mathf.Deg2Rad)){
              pass(targetPlayer,passing.Long);
             }
             else
             {
              pass(null,passing.Long); 
             }
            }
             else if(Input.GetKey(KeyCode.Q))
            {
            float horizontal1 = Input.GetAxis("Horizontal");
             float vertical1 = Input.GetAxis("Vertical");
             Vector3 direction = new Vector3(horizontal1, 0f, vertical1);
             if(Mathf.Approximately(direction.magnitude,0f))direction=transform.forward;
             var targetPlayer = FindPlayerInDirection(direction);
             Vector3 dirToTarget = (targetPlayer.transform.position - transform.position).normalized;
             if (Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos((150 / 2) * Mathf.Deg2Rad)){
              pass(targetPlayer,passing.Tlong);
             }
             else
             {
              pass(null,passing.Long); 
             }
            }
            }
        if(HaveBall){
           if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("pass")){
            HaveBall=false; 
            anim.ResetTrigger("pass");
            if(pag==passing.Short){
             if (passingTarget!=null){

             if(Vector3.Distance(transform.position,passingTarget.transform.position)>25)
             Ball.GetComponent<SoccerBall>().spd=25f;
             else
             Ball.GetComponent<SoccerBall>().spd=15f;

             backup.transform.position=passingTarget.transform.position;
             Ball.GetComponent<SoccerBall>().TargetTransform=backup.transform;
             Ball.GetComponent<SoccerBall>().Target=passingTarget;
             Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
             }else{
                Ball.GetComponent<Rigidbody>().AddForce(transform.forward*12,ForceMode.Impulse);
                Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
            }
            }else if(pag==passing.Tshort){
             if (passingTarget!=null){

             if(Vector3.Distance(transform.position,passingTarget.transform.position)>25)
             Ball.GetComponent<SoccerBall>().spd=30f;
             else
             Ball.GetComponent<SoccerBall>().spd=25f;
          
             backup.transform.position=passingTarget.transform.position+passingTarget.GetComponent<Rigidbody>().velocity*2;
             Ball.GetComponent<SoccerBall>().TargetTransform=backup.transform;
             Ball.GetComponent<SoccerBall>().Target=passingTarget;
             Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
            
             }else{
                Ball.GetComponent<Rigidbody>().AddForce(transform.forward*20,ForceMode.Impulse);
                Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
            }
            } 
            else if(pag==passing.Long){
             if (passingTarget!=null){
               var rigid = GetComponent<Rigidbody>();
 
        Vector3 p =passingTarget.transform.position;
        float gravity = Physics.gravity.magnitude;
        float angle = 45f * Mathf.Deg2Rad;
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(Ball.transform.position.x, 0,Ball.transform.position.z);
        float distance = Vector3.Distance(planarTarget, planarPostion);
        float yOffset = Ball.transform.position.y - (p.y);
        if(distance>35)yOffset = Ball.transform.position.y - (p.y+10);
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
        float angleBetweenObjects = Vector3.SignedAngle(planarTarget - planarPostion,Vector3.forward,Vector3.up);
        Vector3 finalVelocity = Quaternion.AngleAxis(-angleBetweenObjects, Vector3.up) * velocity;
        Ball.GetComponent<Rigidbody>().AddForce(finalVelocity* Ball.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
             }else{
                Ball.GetComponent<Rigidbody>().AddForce(new Vector3(0,10f,0),ForceMode.Impulse);
                Ball.GetComponent<Rigidbody>().AddForce(transform.forward*8,ForceMode.Impulse);
                Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
            }
            }
            else if(pag==passing.Tlong){
             if (passingTarget!=null){
               var rigid = GetComponent<Rigidbody>();
 
        Vector3 p =passingTarget.transform.position;
        float gravity = Physics.gravity.magnitude;
        float angle = 50f * Mathf.Deg2Rad;
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        planarTarget=planarTarget+passingTarget.GetComponent<Rigidbody>().velocity*4.3f;
        Vector3 planarPostion = new Vector3(Ball.transform.position.x, 0,Ball.transform.position.z);
        float distance = Vector3.Distance(planarTarget, planarPostion);
        float yOffset = Ball.transform.position.y - (p.y);
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
        float angleBetweenObjects = Vector3.SignedAngle(planarTarget - planarPostion,Vector3.forward,Vector3.up);
        Vector3 finalVelocity = Quaternion.AngleAxis(-angleBetweenObjects, Vector3.up) * velocity;
        Ball.GetComponent<Rigidbody>().AddForce(finalVelocity* Ball.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
             }else{
                Ball.GetComponent<Rigidbody>().AddForce(new Vector3(0,10f,0),ForceMode.Impulse);
                Ball.GetComponent<Rigidbody>().AddForce(transform.forward*8,ForceMode.Impulse);
                Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
            }
            }
           }
 
            if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("shot")){
            HaveBall=false;
            anim.ResetTrigger("shot");
             if (shotdirec!=null){
                Vector3 Direc=(shotdirec.transform.position-transform.position).normalized;
                transform.rotation= Quaternion.Lerp(transform.rotation,Quaternion.Euler(Direc.x,Direc.y,Direc.z), 12.0f * Time.deltaTime); 
                Ball.GetComponent<Rigidbody>().AddForce(new Vector3(Direc.x*33,(Direc.y+0.2f)*32,Direc.z*33),ForceMode.Impulse);
                Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
             }else{
                 Ball.GetComponent<Rigidbody>().AddForce(new Vector3(transform.forward.x*32,(transform.forward.y+0.2f)*30,transform.forward.z*32),ForceMode.Impulse);
                 Ball.GetComponent<Rigidbody>().AddTorque(transform.root.right*1500, ForceMode.Impulse);
             }
           }
        }
    }
   private player FindPlayerInDirection(Vector3 direction)
    {
        var closestAngle = otherPlayers
            .OrderBy(t => Vector3.Angle(direction, DirectionTo(t)))
            .FirstOrDefault();
        return closestAngle;
    }
    private Vector3 DirectionTo(player player)
    {
        return Vector3.Normalize(player.transform.position - transform.position);
    }
      void OnTriggerEnter(Collider col){
        if(col.gameObject.name=="Soccer Ball"&& !transform.root.GetComponent<control>().HaveBall){
            col.gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
        }
      
    }
}

