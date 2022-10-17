using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    Vector3 rollMoveDir;
    Quaternion rollMoveRot;

    PlayerMovement m;
    PlayerAttack a;

    void Awake(){
        m = GetComponent<PlayerMovement>();
        a = GetComponent<PlayerAttack>();
    }

    void FixedUpdate(){
        if      (m.rolling)      ForcedRollMovement();
        if (m.backstepping) ForcedBackStepMovement();

        if(a.attacking){
            m.rolling = false;
            m.backstepping = false;
        }

        if(!m.groundDetect.grounded && m.backstepping) EndBackStep();
    }
    

    void LateUpdate(){

        CheckNextAction();

        if(Game.control.player.resources.CanExecute("Backstep") && !m.moving && Input.GetButtonDown("Backstep")) {
            if(m.InMiddleOfMovementAction()  && !m.canChain)
                m.inputQ.QueueInput("Backstep");
            else if(m.InMiddleOfMovementAction() && m.canChain)
                StartBackStep();
            else if(a.InMiddleOfCombatAction() && !a.CanChain())
                m.inputQ.QueueInput("Backstep");
            else 
                StartBackStep();
        }
    }

    void CheckNextAction(){
        if(!m.InMiddleOfMovementAction()){
            string nextAction = m.inputQ.CheckQueue();
            if(nextAction == "Roll"){
            // Debug.Log("next is roll");
            
                if(m.moveInput.magnitude == 0) nextAction = "Backstep";
                else {
                    if(a.InMiddleOfCombatAction() && a.CanChain()){
                        StartRoll(m.moveInput);
                    }
                }
                m.inputQ.ClearQueue();
            }
            if(nextAction == "Backstep") {
                if(a.InMiddleOfCombatAction() && a.CanChain()){
                    StartBackStep();
                    m.inputQ.ClearQueue();
                }
                else if(!a.InMiddleOfCombatAction()){
                    StartBackStep();
                    m.inputQ.ClearQueue();
                }
            }
        }
    }

    


    //////////////////////////////////////////
    // ROLL
    //////////////////////////////////////////

    public void StartRoll(Vector2 moveInput){
//        Debug.Log("startroll");
        Game.control.player.resources.UseStaminaForAction("Roll");
        if(a.attacking) a.attacking = false;
        a.canCombo = false;

        m.canChain = false;
        transform.rotation = Quaternion.LookRotation(m.moveDir);
        m.strafeRoll = m.strafing;
        m.strafing = false;
        m.canMove = false;
        m.rolling = true;
        Game.control.player.Animate("Roll");
        
        if(moveInput.x < 0) moveInput.x = -1;
        if(moveInput.x > 0) moveInput.x = 1;
        if(moveInput.y < 0) moveInput.y = -1;
        if(moveInput.y > 0) moveInput.y = 1;
        rollMoveDir = (m.player.cameraT.right*moveInput.x) + (Vector3.Cross(m.player.cameraT.right, Vector3.up) * moveInput.y).normalized;
    }
    
    void ForcedRollMovement(){
       m.moveSpeed = Game.control.player.animator.GetFloat("RollSpeed") * m.properties.rollSpeed * 2;
       transform.rotation = Quaternion.LookRotation(m.moveDir);
       //transform.position += rollMoveDir * m.moveSpeed * Time.deltaTime;
       //GetComponent<Rigidbody>().velocity = rollMoveDir * m.moveSpeed;
       if(Game.control.player.movement.groundDetect.grounded) GetComponent<Rigidbody>().velocity = transform.forward * m.moveSpeed;
       else GetComponent<Rigidbody>().velocity = transform.forward * m.moveSpeed / 3;

       m.ForceGravity();
    }


    //Invoked from animationevent
    public void EndRoll(){
    //   Debug.Log("endroll");
        m.rolling = false;
        rollMoveDir = Vector3.zero;
        
        if(m.strafeRoll) {
            m.strafing = true;
            m.strafeRoll = false;
        }
    }

    public void StoreMoveDir(Vector3 dir){
        rollMoveRot = Quaternion.LookRotation(dir);
    }


    //////////////////////////////////////////
    // BACKSTEP
    //////////////////////////////////////////

    void StartBackStep(){
        Game.control.player.resources.UseStaminaForAction("Backstep");
        if(a.attacking) a.attacking = false;
        a.canCombo = false;

        m.canChain = false;
        m.backstepping = true;
        m.canMove = false;
        m.moveSpeed = m.properties.backstepSpeed;
        Game.control.player.Animate("Backstep");
    }

    //Invoked from animation event
    void EndBackStep(){
        m.backstepping = false;
    }
    
    void ForcedBackStepMovement(){
        m.moveSpeed = Game.control.player.animator.GetFloat("BackstepSpeed") * m.properties.backstepSpeed;
        //transform.position -= transform.forward * m.moveSpeed * Time.deltaTime;
        GetComponent<Rigidbody>().velocity = -(transform.forward * m.moveSpeed);
        m.ForceGravity();
    }

    public void StartLightAttackMovement(){
        m.moveSpeed = 0;
        m.canMove = false;
    }

}
