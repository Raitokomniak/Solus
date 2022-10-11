using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnAnimation : PlayerMovement
{
    PlayerMovement movement;

    bool checkingstop;
    //Timer turnAnimTimer = new Timer(0.5f);

    private void Awake() {
        movement = GetComponent<PlayerMovement>();
    }

    void LateUpdate(){
        DetermineTurnAnimation();
    }

    public void DetermineTurnAnimation(){
        
        if(!running) return;

        IEnumerator checkStop = CheckIfStopped();
        if(moveInputRaw.x == 0) {
            StopCoroutine(checkStop);
            StartCoroutine(checkStop);
        }

        if(moveInputRaw.x != 0 && moveInputRaw.x != lastInputRaw.x) {

            if(lastInputRaw.x == -100) {
                lastInputRaw.x = moveInputRaw.x;
                return;
            }
           // Debug.Log("last x " + lastInputRaw.x);
            lastInputRaw.x = moveInputRaw.x;
           // Debug.Log("new x " + moveInputRaw.x);

            if(moveInput.y < .2f){
                Game.control.player.Animate("Turn180Run");
                turning = true;
                running = false;
                Debug.Log("turn");
            }
        }
    }

    public void EndTurn(){
        turning = false;
        running = moveInput.magnitude > 0;
    }

    
    IEnumerator CheckIfStopped(){
        yield return new WaitForSeconds(1f);
        lastInputRaw.x = -100;
    }
}
