using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    Vector3 rollMoveDir;
    Quaternion rollMoveRot;

    PlayerMovement m;
    void Awake(){
        m = GetComponent<PlayerMovement>();
    }

    void FixedUpdate(){
        if      (m.rolling)      ForcedRollMovement();
        if (m.backstepping) ForcedBackStepMovement();
    }

    void LateUpdate(){

        CheckNextAction();

        if(!m.moving && Input.GetButtonDown("Backstep")) {
            if(m.inputQ.InMiddleOfAction()) m.inputQ.QueueInput("Backstep");
            else StartBackStep();
        }
    }

    void CheckNextAction(){
        string nextAction = m.inputQ.CheckQueue();
        if(m.inputQ.CheckQueue() == "Roll"){
            if(m.moveInput.magnitude == 0) nextAction = "Backstep";
            else StartRoll(m.moveInput);
        }
        if(nextAction == "Backstep") StartBackStep();
    }

    //////////////////////////////////////////
    // ROLL
    //////////////////////////////////////////

    public void StartRoll(Vector2 moveInput){
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
       transform.position += rollMoveDir * m.moveSpeed * Time.deltaTime;
    }


    //Invoked from animationevent
    public void EndRoll(){
        Debug.Log("endroll");
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
        transform.position -= transform.forward * m.moveSpeed * Time.deltaTime;
    }

    public void StartLightAttackMovement(){
        m.moveSpeed = 0;
        m.canMove = false;
    }

}
