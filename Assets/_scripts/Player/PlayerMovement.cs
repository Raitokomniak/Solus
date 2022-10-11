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
    
    public float runThreshold = 0.2f;


    public MovementProperties(){}
}


public class PlayerMovement : MonoBehaviour
{   
    public InputQueueing inputQ;
    public PlayerHandler player;
    
    PlayerTurnAnimation pTurn;
    PlayerDodge pDodge;
    
    public MovementProperties m;
    
   // GroundDetect groundDetect;

    //inputs
    public Vector2 moveInput;
    public Vector2 moveInputRaw;
    public Vector2 lastInputRaw;

    //vectors
    public Vector3 moveDir;
    public float moveSpeed;
    Quaternion lookRot;

    public bool canMove, idling, moving, running, sprinting, backstepping, rolling, strafeRoll, strafing, turning;


    Timer sprintTimer;
    Timer idleTimer;




    void Awake(){
        m = new MovementProperties();
        pTurn = GetComponent<PlayerTurnAnimation>();
        player = Game.control.player;
    
        sprintTimer = new Timer(0.5f);
        idleTimer = new Timer(4f);

        inputQ = GetComponent<InputQueueing>();
        canMove = true;

        moveInput = new Vector2();
        lastInputRaw = new Vector2(-100, -100);
    }

    //////////////////////////////////////////
    // UPDATES
    //////////////////////////////////////////

    void LateUpdate() {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveInputRaw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moving = moveInput.magnitude > 0;

        DetermineSprint();
        CheckIdling();
    }

    void FixedUpdate() {
        if (!strafing) if(CanMove()) Move();
        CheckRestraints();
    }

    bool CanMove(){
        if(rolling) return false;
        if(backstepping) return false;
        if(Game.control.player.attack.restrictedMovement) return false;
        return true;
    }

    void Move(){
        moveDir = (player.cameraT.right*moveInput.x) + (Vector3.Cross(player.cameraT.right, Vector3.up) * moveInput.y);//normalized
        transform.position += moveInput.magnitude * transform.forward * moveSpeed * Time.deltaTime;
        DetermineMoveSpeed();
        Rotate();
    }

    void Rotate(){
        if(Game.control.player.rb.velocity != Vector3.zero && Game.control.player.rb.velocity.magnitude > 0) lookRot = Quaternion.LookRotation(Game.control.player.rb.velocity); 
        
        if(moveDir.magnitude <= 0) lookRot = Quaternion.LookRotation(transform.forward);
        else lookRot = Quaternion.LookRotation(moveDir);
        
        float turnS = 0;
        if(turning) turnS = Game.control.player.animator.GetFloat("TurnSpeed") * m.turnSpeed * Time.deltaTime;
        else turnS = m.turnSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, turnS);
    }
    
    void DetermineSprint(){
        if(!moving) return;
        if(!sprinting && Input.GetButton("Sprint")) sprintTimer.Tick();
        
        if(!sprintTimer.TimeOut() && Input.GetButtonUp("Roll")) {
            sprintTimer.Reset();
            if(inputQ.InMiddleOfAction()) inputQ.QueueInput("Roll");
            else pDodge.StartRoll(moveInput);
        }
        if(sprintTimer.TimeOut()) {
            sprinting = Input.GetButton("Sprint");
            if(!sprinting) sprintTimer.Reset();
        }
    }

    void DetermineMoveSpeed(){
        float speed = moveInput.magnitude;
        running = speed >= m.runThreshold;
        if(running) moveSpeed = m.runSpeed;
        if(sprinting) moveSpeed = m.sprintSpeed;

        Game.control.player.Animate("Running", (Mathf.Abs(moveInput.x) > 0.2 || Mathf.Abs(moveInput.y) > 0.2f));
        Game.control.player.Animate("Sprinting", sprinting);
    }
    
    void CheckRestraints(){
        if(moveInput.magnitude == 0) Game.control.player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        else Game.control.player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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

}
