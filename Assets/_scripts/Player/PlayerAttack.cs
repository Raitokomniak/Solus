using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    InputQueueing inputQ;
    PlayerHandler player;

    public bool attacking;
    public bool restrictedMovement;

    public bool canCombo;

    string combo;

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

            if(canCombo) ComboAttack();
        }
    }

    void ComboAttack(){
        player.Animate(combo);
        attacking = true;
    }

    public void EnableCombo(string combo){
        this.combo = combo;
        canCombo = true;
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
        canCombo = false;
    }
}
