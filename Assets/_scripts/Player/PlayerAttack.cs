using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerMovement m;
    InputQueueing inputQ;
    PlayerHandler player;

    public bool attacking;
    public bool restrictedMovement, restrictedRotation;

    public bool canCombo;
    public bool middleOfCombo;

    string combo;

    void Awake(){
        inputQ = GetComponent<InputQueueing>();
    }

    void LateUpdate(){
        if(player == null && Game.control != null) player = Game.control.player;
        if(m == null) m = player.movement;

        CheckNextAction();

        if(Input.GetButtonDown("LightAttack")){
            
            if(!middleOfCombo){
                if(inputQ.InMiddleOfAction())
                    inputQ.QueueInput("LightAttack");
                else LightAttack();
            }
            if(canCombo) ComboAttack();
        }

       /* if(attacking){
            transform.rotation = Quaternion.LookRotation(m.moveDir);
        }*/
    }

    void ComboAttack(){
        player.Animate(combo);
        attacking = true;
        middleOfCombo = true;
    }

    public void EnableCombo(string combo){
        this.combo = combo;
        canCombo = true;
    }
    void CheckNextAction(){
        string nextAction = inputQ.CheckQueue();
        if(nextAction == "LightAttack") {
            LightAttack();
        }
        else if(nextAction == "RollAttack"){
           RollAttack();
        }
    }

    void RollAttack(){
        player.Animate("RollAttack");
        attacking = true;
    }

    void LightAttack(){
        player.Animate("LightAttack");
        attacking = true;
    }

    void RestrictMovement(int toggle){
        restrictedMovement = toggle > 0;
    }

    void RestrictRotation(int toggle){
        restrictedRotation = toggle > 0;
    }

    public void EndAttack(){
        attacking = false;
        canCombo = false;
        middleOfCombo = false;
    }
}
