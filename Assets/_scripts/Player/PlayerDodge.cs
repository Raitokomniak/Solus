using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : PlayerMovement
{
    Vector3 rollMoveDir;

    void FixedUpdate(){
        if      (rolling)      ForcedRollMovement();
        if (backstepping) ForcedBackStepMovement();
    }

    void LateUpdate(){
        CheckNextAction();

        if(!moving && Input.GetButtonDown("Backstep")) {
            if(inputQ.InMiddleOfAction()) inputQ.QueueInput("Backstep");
            else StartBackStep();
        }
    }

    void CheckNextAction(){
        string nextAction = inputQ.CheckQueue();
        if(inputQ.CheckQueue() == "Roll"){
            if(moveInput.magnitude == 0) nextAction = "Backstep";
            else StartRoll(moveInput);
        }
        if(nextAction == "Backstep") StartBackStep();
    }

    //////////////////////////////////////////
    // ROLL
    //////////////////////////////////////////

    public void StartRoll(Vector2 moveInput){
        CorrectRotationForRoll(moveInput);
        strafeRoll = strafing;
        strafing = false;
        canMove = false;
        rolling = true;
        Game.control.player.Animate("Roll");
        
        if(moveInput.x < 0) moveInput.x = -1;
        if(moveInput.x > 0) moveInput.x = 1;
        if(moveInput.y < 0) moveInput.y = -1;
        if(moveInput.y > 0) moveInput.y = 1;
        rollMoveDir = (player.cameraT.right*moveInput.x) + (Vector3.Cross(player.cameraT.right, Vector3.up) * moveInput.y).normalized;
    }
    
    void ForcedRollMovement(){
       moveSpeed = Game.control.player.animator.GetFloat("RollSpeed") * m.rollSpeed * 2;
       transform.position += rollMoveDir * moveSpeed * Time.deltaTime;
    }


    //Invoked from animationevent
    public void EndRoll(){
        rolling = false;
        rollMoveDir = Vector3.zero;
        
        if(strafeRoll) {
            strafing = true;
            strafeRoll = false;
        }
    }

    void CorrectRotationForRoll(Vector2 moveInput){
        moveDir = (player.cameraT.right*moveInput.x) + (Vector3.Cross(player.cameraT.right, Vector3.up) * moveInput.y).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), m.turnSpeed);
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
    void EndBackStep(){
        backstepping = false;
    }
    
    void ForcedBackStepMovement(){
        moveSpeed = Game.control.player.animator.GetFloat("BackstepSpeed") * m.backstepSpeed;
        transform.position -= transform.forward * moveSpeed * Time.deltaTime;
    }

    public void StartLightAttackMovement(){
        moveSpeed = 0;
        canMove = false;
    }

}
