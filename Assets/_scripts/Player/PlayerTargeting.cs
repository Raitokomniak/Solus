using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : PlayerMovement
{
    Transform strafeTarget;

    void FixedUpdate(){
        if(strafing) Strafe();
    }


    bool CanStrafe(){
        Debug.Log(rolling + "rolling");
        if(!Game.control.cam.targeting) return false;
        if(rolling) return false;
        return true;
    }

    public void ReleaseTarget(){
        strafing = false;
        strafeRoll = false;
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
        if(inputQ.InMiddleOfAction()){
            yield return new WaitUntil(() => rolling == false);
            yield return new WaitUntil(() => backstepping == false);
        }

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
        moveDir = (player.cameraT.right*moveInput.x) + (Vector3.Cross(player.cameraT.right, Vector3.up) * moveInput.y).normalized;
        if(strafeTarget == null) return;
        Vector3 vectorToTarget = transform.position - strafeTarget.position;
        Quaternion targetRot = Quaternion.LookRotation(strafeTarget.position - transform.position);
        transform.position += targetRot * new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.rotation = targetRot;
        Vector3 correctedRot = new Vector3(0,transform.rotation.eulerAngles.y,0);
        transform.rotation = Quaternion.Euler(correctedRot);

        Game.control.player.Animate("StrafeR", moveInput.x > 0);
        Game.control.player.Animate("StrafeL", moveInput.x < 0);
        Game.control.player.Animate("StrafeB", moveInput.y < 0);
        Game.control.player.Animate("StrafeF", moveInput.y > 0);
        
        if(!rolling && !backstepping){
            if(moveInput.y < 0 && moveInput.x == 0) moveSpeed = m.walkSpeed;
            else moveSpeed = m.runSpeed;
        }
    }
}
