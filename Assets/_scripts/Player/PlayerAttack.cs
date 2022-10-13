using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerMovement m;
    InputQueueing inputQ;
    PlayerHandler player;

    public bool attacking;
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
            if(canCombo) ComboAttack();
            if(!middleOfCombo){
                if(inputQ.InMiddleOfAction())
                    inputQ.QueueInput("LightAttack");
                else LightAttack();
            }
            
        }
    }

    void ComboAttack(){
        Debug.Log("comboattack");
        player.Animate(combo);
        attacking = true;
        middleOfCombo = true;
    }

    public void EnableCombo(string combo){
       this.combo = combo;
       canCombo = true;
    }

    public void DisableCombo(){
       canCombo = false;
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

    public void EndAttack(){
        attacking = false;
        canCombo = false;
        if(middleOfCombo) middleOfCombo = false;
    }
}
