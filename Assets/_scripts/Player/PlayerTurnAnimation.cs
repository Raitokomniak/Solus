using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnAnimation : MonoBehaviour
{
    bool checkingstop;
    //Timer turnAnimTimer = new Timer(0.5f);

    PlayerMovement m;
    void Awake(){
        m = GetComponent<PlayerMovement>();
    }

    void LateUpdate(){
        DetermineTurnAnimation();
    }

    public void DetermineTurnAnimation(){
        
        if(!m.running) return;

        IEnumerator checkStop = CheckIfStopped();
        if(m.moveInputRaw.x == 0) {
            StopCoroutine(checkStop);
            StartCoroutine(checkStop);
        }

        if(m.moveInputRaw.x != 0 && m.moveInputRaw.x != m.lastInputRaw.x) {

            if(m.lastInputRaw.x == -100) {
                m.lastInputRaw.x = m.moveInputRaw.x;
                return;
            }
           // Debug.Log("last x " + lastInputRaw.x);
            m.lastInputRaw.x = m.moveInputRaw.x;
           // Debug.Log("new x " + moveInputRaw.x);

            if(m.moveInput.y < .2f){
                Game.control.player.Animate("Turn180Run");
                m.turning = true;
                m.running = false;
                Debug.Log("turn");
            }
        }
    }

    public void EndTurn(){
        m.turning = false;
        m.running = m.moveInput.magnitude > 0;
    }

    
    IEnumerator CheckIfStopped(){
        yield return new WaitForSeconds(1f);
        m.lastInputRaw.x = -100;
    }
}
