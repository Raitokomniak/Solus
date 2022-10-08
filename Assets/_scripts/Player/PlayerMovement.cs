using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A small class to contain player properties
public class MovementProperties {

    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public float turnSpeed = 15f;
    public float rollSpeed = 9f;
    public float backstepSpeed = 6f;
    
    public float runThreshold = 0.1f;
    public float sprintThreshold = 0.5f;

    public float idleThreshold = 4f;

    public MovementProperties(){}
}

public class PlayerMovement : MonoBehaviour
{
    MovementProperties m;
    public Animator animator;
    Rigidbody rb;
    public Transform cameraT;
    Vector3 moveDir;
    Vector2 moveInput;
    Quaternion lookRot;

    List<string> inputQueue;
    float inputLifeTime = .5f;
    float inputTimer = 0;

    float moveSpeed;

    bool canMove = true;

   // bool walking;
    bool idling, moving, running, sprinting, rolling, backstepping, backstepMoving;

    float sprintTimer = 0;

    float idleTimer = 0;



    void Awake(){
        m = new MovementProperties();
        cameraT = Camera.main.gameObject.transform;
        inputQueue = new List<string>();
        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate() {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moving = moveInput.magnitude > 0;

        CheckQueue();
            
        if(!moving && Input.GetButtonDown("Backstep")) {
            if(InMiddleOfAction()) QueueInput("Backstep");
            else StartBackStep();
        }
        DetermineSprint();

        CheckIdling();
    }

    void FixedUpdate() {
        if      (rolling)      ForcedRollMovement();
        else if (backstepping && backstepMoving) ForcedBackStepMovement();

        if(canMove) Move();
        Rotate();
    }

    void CheckIdling(){
        if(!idling && !Input.anyKeyDown && moveInput.magnitude == 0) {
            idleTimer += Time.deltaTime;
            if(idleTimer > m.idleThreshold) {
                idling = true;
                animator.SetBool("Idling", true);
            }
        }
        else if(idling && Input.anyKeyDown || moveInput.magnitude > 0){
            idling = false;
            animator.SetBool("Idling", false);
            idleTimer = 0;
        }
    }

    //invoked from animation event
    public void CanMove(int toggle){
        canMove = toggle > 0;
    }


    bool InMiddleOfAction(){
        if(rolling || backstepping){
            Debug.Log("isrolling " + rolling);
            Debug.Log("isbackstepping " + backstepping);
        }
        if(rolling) return true;
        if(backstepping) return true;
        return false;
    }
    
    void CheckQueue(){
        if(inputQueue.Count > 0){
            CalculateInputLifeTime();
            if(!InMiddleOfAction()) CheckQueueForInputs();
        }
    }

    void QueueInput(string input){
        inputQueue.Add(input);
        Debug.Log("queued input " + input);
    }


    void CheckQueueForInputs(){
        if(inputQueue.Count > 0){
            string nextAction = inputQueue[0];
            Debug.Log("nextaction will be " + nextAction);
            if(nextAction == "Roll") {
                if(moveInput.magnitude == 0) nextAction = "Backstep";
                else StartRoll();
            }
            if(nextAction == "Backstep") StartBackStep();
            inputQueue.Clear();
        }
    }

    void CalculateInputLifeTime(){
        if(inputTimer < inputLifeTime) inputTimer += Time.deltaTime;
        else if(inputTimer > inputLifeTime) {   
            inputQueue.Clear();
            inputTimer = 0;
        }
    }


  
    void Move(){
        moveDir = (cameraT.right*moveInput.x) + (Vector3.Cross(cameraT.right, Vector3.up) * moveInput.y).normalized;
        transform.position += moveInput.magnitude * transform.forward * moveSpeed * Time.deltaTime;
        DetermineMoveSpeed(moveInput);
    }

    void Rotate(){
        if(rb.velocity.magnitude > 0) lookRot = Quaternion.LookRotation(rb.velocity); 
        if(moveDir.magnitude <= 0) lookRot = Quaternion.LookRotation(transform.forward);
        else lookRot = Quaternion.LookRotation(moveDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, m.turnSpeed * Time.deltaTime);
    }


    void DetermineSprint(){
        if(!moving) return;
        if(!sprinting && Input.GetButton("Sprint")) sprintTimer += Time.deltaTime;
        
        if(sprintTimer < m.sprintThreshold && Input.GetButtonUp("Roll")) {
            sprintTimer = 0;
            if(InMiddleOfAction()) QueueInput("Roll");
            else StartRoll();
        }
        if(sprintTimer >= m.sprintThreshold) {
            sprinting = Input.GetButton("Sprint");
            if(!sprinting) sprintTimer = 0;
        }
    }

    //////////////////////////////////////////
    // Backstepping

    void StartBackStep(){
        backstepping = true;
        canMove = false;
        moveSpeed = m.backstepSpeed;
        animator.SetTrigger("Backstep");
    }

    //Invoked from animation event
    //CHANGE SPEED INTO A CURVE
    void StartBackStepMovement(){
        backstepMoving = true;
    }

    //Invoked from animation event
    //CHANGE SPEED INTO A CURVE
    void MidBackStep(){
        moveSpeed = 0;
    }

    //Invoked from animation event
    //CHANGE SPEED INTO A CURVE
    void EndBackStep(){
        backstepping = false;
        backstepMoving = false;
    }
    
    void ForcedBackStepMovement(){
        transform.position -= transform.forward * moveSpeed * Time.deltaTime;
    }

    //////////////////////////////////////////
    // ROLLING

    void StartRoll(){
        canMove = false;
        rolling = true;
        moveSpeed = m.rollSpeed;
        animator.SetTrigger("Roll");
    }
    
    void ForcedRollMovement(){
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    

    //Invoked from animation event
    //CHANGE SPEED INTO A CURVE
    public void MidRoll(){
        moveSpeed = m.rollSpeed / 2;
    }

    //Invoked from animationevent
    //CHANGE SPEED INTO A CURVE
    public void EndRoll(){
        rolling = false;
    }

    void DetermineMoveSpeed(Vector2 input){
        float speed = input.magnitude;
   //     walking = speed > 0 && speed < m.runThreshold;
        running = speed >= m.runThreshold;

     //   if(walking) moveSpeed = m.walkSpeed;
        if(running) moveSpeed = m.runSpeed;
        if(sprinting) moveSpeed = m.sprintSpeed;
      
     //   animator.SetBool("Walking", walking);
        animator.SetBool("Running", running);
        animator.SetBool("Sprinting", sprinting);
    }
}
