using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A small class to contain player properties
public class MovementProperties {

    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public float turnSpeed = 15f;
    public float rollSpeed = 8.5f;
    public float backstepSpeed = 6f;
    
    public float runThreshold = 0.1f;


    public MovementProperties(){}
}


public class PlayerMovement : MonoBehaviour
{
    InputQueueing inputQ;

    MovementProperties m;

    GroundDetect groundDetect;

    public Transform cameraT;
    public Transform strafeTarget;

    Vector3 moveDir;
    Vector2 moveInput;
    float moveSpeed;
    Quaternion lookRot;

    public bool canMove, idling, moving, running, sprinting, rolling, backstepping, backstepMoving, strafing, strafeRoll;

    Vector3 rollMoveDir;
    Timer rollTimer;

    Timer sprintTimer;
    Timer idleTimer;



    void Awake(){
        m = new MovementProperties();
        cameraT = Camera.main.gameObject.transform;
        
        rollTimer = new Timer(1.5f);
        sprintTimer = new Timer(0.5f);
        idleTimer = new Timer(4f);

        inputQ = GetComponent<InputQueueing>();
        canMove = true;
    }

    //////////////////////////////////////////
    // UPDATES
    //////////////////////////////////////////

    void LateUpdate() {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moving = moveInput.magnitude > 0;
        CheckNextAction();
        DetermineSprint();
        CheckIdling();

        Game.control.player.Animate("Running", moveInput.magnitude > 0);
        
        if(rolling) RollFailSafe();
        if(!rolling && !backstepMoving) canMove = true;

        /*
        if(!moving && Input.GetButtonDown("Backstep")) {
            if(InMiddleOfAction()) QueueInput("Backstep");
            else StartBackStep();
        }
        */
    }

    void FixedUpdate() {
        
        if      (rolling)      ForcedRollMovement();
        else if (backstepping && backstepMoving) ForcedBackStepMovement();

        
            
        if (!CanStrafe()) {
                if(CanMove()) Move();
                
            }
            else if(CanStrafe()) Strafe();

            

        if(moveInput.magnitude == 0) Game.control.player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        else Game.control.player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    //////////////////////////////////////////
    // BASICS
    //////////////////////////////////////////

    void CheckNextAction(){
        string nextAction = inputQ.CheckQueue();
        if(nextAction == "Roll") {
            if(moveInput.magnitude == 0) nextAction = "Backstep";
            else StartRoll();
        }
        if(nextAction == "Backstep") StartBackStep();
    }

    bool CanMove(){
//        Debug.Log("strafing " + strafing + " rolling " + rolling + " restricted " + Game.control.player.attack.restrictedMovement);
        //if(strafing) return false;
        if(rolling) return false;
        if(Game.control.player.attack.restrictedMovement) return false;
        return true;
    }

    void Move(){
        moveDir = (cameraT.right*moveInput.x) + (Vector3.Cross(cameraT.right, Vector3.up) * moveInput.y).normalized;
        transform.position += moveInput.magnitude * transform.forward * moveSpeed * Time.deltaTime;
        DetermineMoveSpeed();
        Rotate();
    }

    void Rotate(){
        if(Game.control.player.rb.velocity != Vector3.zero && Game.control.player.rb.velocity.magnitude > 0) lookRot = Quaternion.LookRotation(Game.control.player.rb.velocity); 
        if(moveDir.magnitude <= 0) lookRot = Quaternion.LookRotation(transform.forward);
        else lookRot = Quaternion.LookRotation(moveDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, m.turnSpeed * Time.deltaTime);
    }

    void CorrectRotationForRoll(){
        moveDir = (cameraT.right*moveInput.x) + (Vector3.Cross(cameraT.right, Vector3.up) * moveInput.y).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), m.turnSpeed);
    }

    
    void DetermineSprint(){
        if(!moving) return;
        if(!sprinting && Input.GetButton("Sprint")) sprintTimer.Tick();
        
        if(!sprintTimer.TimeOut() && Input.GetButtonUp("Roll")) {
            sprintTimer.Reset();
            if(inputQ.InMiddleOfAction()) inputQ.QueueInput("Roll");
            else StartRoll();
        }
        if(sprintTimer.TimeOut()) {
            sprinting = Input.GetButton("Sprint");
            if(!sprinting) sprintTimer.Reset();
        }
    }

    void DetermineMoveSpeed(){
        float speed = moveInput.magnitude;
   //     walking = speed > 0 && speed < m.runThreshold;
        running = speed >= m.runThreshold;

     //   if(walking) moveSpeed = m.walkSpeed;
        if(running) moveSpeed = m.runSpeed;
        if(sprinting) moveSpeed = m.sprintSpeed;
      
     //   animator.SetBool("Walking", walking);
     
        Game.control.player.Animate("Running", running);
        Game.control.player.Animate("Sprinting", sprinting);
    }


    //////////////////////////////////////////
    // TARGETING
    //////////////////////////////////////////

    bool CanStrafe(){
        if(!Game.control.cam.targeting) return false;
        if(rolling) return false;
        return true;
    }

    public void ReleaseTarget(){
        strafing = false;
        IEnumerator waitroll = StrafeWaitForRoll(false);
        StartCoroutine(waitroll);
        
    }

    public void TargetEnemy(Transform target){
        if(target!=null) transform.rotation = Quaternion.LookRotation(target.position);
        strafing = true;
        strafeTarget = target;
        
        IEnumerator waitroll = StrafeWaitForRoll(true);
        StartCoroutine(waitroll);
    }

    IEnumerator StrafeWaitForRoll(bool tostrafe){
        if(inputQ.InMiddleOfAction()){yield return new WaitUntil(() => rolling == false);}

        if(tostrafe){
            Game.control.player.Animate("StartStrafe");
            Game.control.player.Animate("StrafeI", true);
            Game.control.player.Animate("Running", false);
        }
        else {
            Game.control.player.Animate("StrafeI", false);
            Game.control.player.Animate("EndStrafe");
        }
    }

    void Strafe(){
        moveInput = moveInput.normalized;
        moveDir = (cameraT.right*moveInput.x) + (Vector3.Cross(cameraT.right, Vector3.up) * moveInput.y).normalized;
        if(strafeTarget == null) return;
        Vector3 vectorToTarget = transform.position - strafeTarget.position;
        Quaternion targetRot = Quaternion.LookRotation(strafeTarget.position - transform.position);
        transform.position += targetRot * new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.rotation = targetRot;
        Vector3 correctedRot = new Vector3(0,transform.rotation.eulerAngles.y,0);
        transform.rotation = Quaternion.Euler(correctedRot);

        //DetermineMoveSpeed();

        Game.control.player.Animate("StrafeR", moveInput.x > 0);
        Game.control.player.Animate("StrafeL", moveInput.x < 0);
        Game.control.player.Animate("StrafeB", moveInput.y < 0);
        Game.control.player.Animate("StrafeF", moveInput.y > 0);
        
        if(!rolling && !backstepping){
            if(moveInput.y < 0 && moveInput.x == 0) moveSpeed = m.walkSpeed;
            else moveSpeed = m.runSpeed;
        }
    }


    void CheckIdling(){
        if(!idling && !Input.anyKeyDown && moveInput.magnitude == 0) {
            idleTimer.Tick();
            if(idleTimer.TimeOut()) {
                idling = true;
                Game.control.player.Animate("Idling", true);
            }
        }
        else if(idling && Input.anyKeyDown || moveInput.magnitude > 0){
            idling = false;
            Game.control.player.Animate("Idling", false);
            idleTimer.Reset();
        }
    }

    //invoked from animation event
    public void CanMove(int toggle){
        canMove = toggle > 0;
    }

    

    //////////////////////////////////////////
    // BACKSTEP
    //////////////////////////////////////////

    void StartBackStep(){
        backstepping = true;
        canMove = false;
        moveSpeed = m.backstepSpeed;
        Game.control.player.Animate("Backstep");
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
    void EndBackStep(){
        backstepping = false;
        backstepMoving = false;
    }
    
    void ForcedBackStepMovement(){
        transform.position -= transform.forward * moveSpeed * Time.deltaTime;
    }

    
    //////////////////////////////////////////
    // ROLLING
    //////////////////////////////////////////

    void StartRoll(){
        CorrectRotationForRoll();
        strafeRoll = strafing;
        strafing = false;
        rollTimer.Reset();
        canMove = false;
        rolling = true;
        moveSpeed = m.rollSpeed;
        Game.control.player.Animate("Roll");
        rollMoveDir = moveDir;
    }
    
    void ForcedRollMovement(){
       // transform.position += transform.forward * moveSpeed * Time.deltaTime;
       transform.position += rollMoveDir * moveSpeed * Time.deltaTime;
    }

    void StrafeRollMovement(){
        transform.position += rollMoveDir * moveSpeed * Time.deltaTime;
    }
    

    //Invoked from animation event
    public void MidRoll(){
        moveSpeed = m.rollSpeed / 2;
        
    }

    //Invoked from animationevent
    public void EndRoll(){
        rolling = false;
        rollMoveDir = Vector3.zero;
        rollTimer.Reset();
        if(strafeRoll) {
            strafing = true;
            strafeRoll = false;
        }
    }

    void RollFailSafe(){
        rollTimer.Tick();
        if(rollTimer.TimeOut()) EndRoll();
    }

    ///////////////////////////////////

    public void StartLightAttackMovement(){
        moveSpeed = 0;
        canMove = false;
    }
    


    /* not sure if good design
    public bool ReleaseCam(){
        if(strafeRoll) return true;
        return false;
    }
    */
}
