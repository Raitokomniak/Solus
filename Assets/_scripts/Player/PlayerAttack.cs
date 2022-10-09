using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    InputQueueing inputQ;
    PlayerHandler player;

    public bool attacking;
    public bool restrictedMovement;

    void Awake(){
        inputQ = GetComponent<InputQueueing>();
    }

    void LateUpdate(){
        if(player == null && Game.control != null) player = Game.control.player;

        CheckNextAction();

        if(Input.GetButtonDown("LightAttack")){
            if(inputQ.InMiddleOfAction())
                inputQ.QueueInput("LightAttack");
            else LightAttack();
        }
    }

    void CheckNextAction(){
        string nextAction = inputQ.CheckQueue();
        if(nextAction == "LightAttack") LightAttack();
    }

    void LightAttack(){
        player.Animate("LightAttack");
        attacking = true;
    }

    void RestrictMovement(int toggle){
        restrictedMovement = toggle > 0;
    }

    public void EndAttack(){
        attacking = false;
    }
}
