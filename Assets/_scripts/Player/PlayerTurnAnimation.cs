using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnAnimation : MonoBehaviour
{
    bool checkingstop;
    //Timer turnAnimTimer = new Timer(0.5f);

    PlayerMovement m;
    Quaternion lookRot;
    void Awake(){
        m = GetComponent<PlayerMovement>();
        InvokeRepeating("CheckTurn", 0, .05f);
    }

    void LateUpdate(){
        
    }

    void FixedUpdate(){
       DetermineTurnAnimation();
       if(m.turning) m.moveSpeed = m.player.animator.GetFloat("TurnSpeed") * 5;
    }

    void CheckTurn(){
       lookRot = Quaternion.LookRotation(m.moveDir);
    }

    public void DetermineTurnAnimation(){
        if(!m.sprinting) return;
        if(m.turning) return;
        
        float targetY = lookRot.eulerAngles.y;
        float rotY = transform.rotation.eulerAngles.y;
        float rotDiff = Mathf.Abs(targetY - rotY);

        Debug.Log(targetY + " target vs rot " + rotY);
        
        if(targetY > 345 && rotY > 0) rotDiff -= 345;
        if(rotY > 345 && targetY > 0) rotDiff -= 345;

        if(rotDiff > 30){
            Debug.Log("diff " + rotDiff);
            Turn();
        }
    }

    void Turn(){
        m.player.Animate("Turning", true);
        Game.control.player.Animate("Turn180Run");
        
        m.turning = true;
       // m.running = false;
        Debug.Log("turn");
    }

    public void EndTurn(){
        m.turning = false;
        m.running = m.moveInput.magnitude > 0;
        m.player.Animate("Turning", false);
    }

    
    IEnumerator CheckIfStopped(){
        yield return new WaitForSeconds(1f);
        m.lastInputRaw.x = -100;
    }
}
