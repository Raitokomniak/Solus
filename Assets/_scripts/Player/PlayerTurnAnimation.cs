using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnAnimation : MonoBehaviour
{
    PlayerMovement m;
    Quaternion lookRot;
    void Awake(){
        m = GetComponent<PlayerMovement>();
        InvokeRepeating("CheckTurn", 0, .05f);
    }

    void FixedUpdate(){
       DetermineTurnAnimation();
       if(m.turning) m.moveSpeed = m.player.animator.GetFloat("TurnSpeed") * 5;
    }

    void CheckTurn(){
       if(m.moveDir.magnitude != 0) lookRot = Quaternion.LookRotation(m.moveDir);
    }

    public void DetermineTurnAnimation(){
        if(!m.sprinting) return;
        if(m.turning) return;
        
        //get diffs in rotation from 12 frames ago
        float targetY = lookRot.eulerAngles.y;
        float rotY = transform.rotation.eulerAngles.y;
        float rotDiff = Mathf.Abs(targetY - rotY);

        //if at 360-0 border, correct
        if(targetY > 345 && rotY > 0) rotDiff -= 345;
        if(rotY > 345 && targetY > 0) rotDiff -= 345;

        if(rotDiff > 30) Turn();
    }

    void Turn(){
        m.player.Animate("Turning", true);
        Game.control.player.Animate("Turn180Run");
        m.turning = true;
    }

    public void EndTurn(){
        m.turning = false;
        m.running = m.moveInput.magnitude > 0;
        m.player.Animate("Turning", false);
    }

}
