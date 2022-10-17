using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    PlayerMovement m;
    public Transform strafeTarget;

    void Awake(){
        m = GetComponent<PlayerMovement>();
    }

    void FixedUpdate(){
        if(!m.init) return;
        if(m.strafing && CanStrafe()) Strafe();
        

    }
    


    bool CanStrafe(){
        Debug.Log(m.rolling + "rolling");
        if(!Game.control.cam.targeting) return false;
        if(m.rolling) return false;
        if(m.backstepping) return false;
        if(Game.control.player.attack.attacking) return false;
        return true;
    }

    public void ReleaseTarget(){
        m.strafing = false;
        m.strafeRoll = false;
        IEnumerator waitroll = StrafeWaitForRoll(false);
        StartCoroutine(waitroll);
        
    }

    public void TargetEnemy(Transform target){
        if(target!=null) transform.rotation = Quaternion.LookRotation(target.position);
        m.strafing = true;
        strafeTarget = target;
        
        IEnumerator waitroll = StrafeWaitForRoll(true);
        StartCoroutine(waitroll);
    }

    void EndAllStrafeAnimations(){
        Game.control.player.Animate("StrafeR", false);
        Game.control.player.Animate("StrafeL", false);
        Game.control.player.Animate("StrafeB", false);
        Game.control.player.Animate("StrafeF", false);
        Game.control.player.Animate("StrafeI", false);
        //Game.control.player.Animate("EndStrafe");
    }

    IEnumerator StrafeWaitForRoll(bool tostrafe){
        if(m.InMiddleOfMovementAction()){
            yield return new WaitUntil(() => m.rolling == false);
            yield return new WaitUntil(() => m.backstepping == false);
        }

        if(tostrafe){
            Game.control.player.Animate("StartStrafe");
            Game.control.player.Animate("Running", false);
        }
        else {
            EndAllStrafeAnimations();
        }
    }

    void Strafe(){
//if(Game.control.player.attack.restrictedMovement) return;
        if(m.InMiddleOfMovementAction()) return;
        m.moveInput = m.moveInput.normalized;
        m.moveDir = (m.player.cameraT.right*m.moveInput.x) + (Vector3.Cross(m.player.cameraT.right, Vector3.up) * m.moveInput.y).normalized;
        if(strafeTarget == null) return;
        Vector3 vectorToTarget = transform.position - strafeTarget.position;
        Quaternion targetRot = Quaternion.LookRotation(strafeTarget.position - transform.position);
        transform.position += targetRot * new Vector3(m.moveInput.x, 0, m.moveInput.y) * m.moveSpeed * Time.deltaTime;
        transform.rotation = targetRot;
        Vector3 correctedRot = new Vector3(0,transform.rotation.eulerAngles.y,0);
        transform.rotation = Quaternion.Euler(correctedRot);

        Game.control.player.Animate("StrafeR", m.moveInput.x > 0);
        Game.control.player.Animate("StrafeL", m.moveInput.x < 0);
        Game.control.player.Animate("StrafeB", m.moveInput.y < 0);
        Game.control.player.Animate("StrafeF", m.moveInput.y > 0);
        Game.control.player.Animate("StrafeI", true);
        
        if(!m.rolling && !m.backstepping){
            if(m.moveInput.y < 0 && m.moveInput.x == 0) m.moveSpeed = m.properties.walkSpeed;
            else m.moveSpeed = m.properties.runSpeed;
        }
    }
}
