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
            if(m.InMiddleOfMovementAction())    inputQ.QueueInput("RollAttack");
            else if(InMiddleOfCombatAction()) inputQ.QueueInput("ComboAttack");
            else if(canCombo) ComboAttack();
            else if(!attacking) LightAttack();
        }
    }

    public bool CanChain(){
        if(attacking && canCombo) return true;
        return false;
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

        if(inputQ.CheckQueue() == "ComboAttack"){
            ComboAttack();
            m.inputQ.ClearQueue();
       }
    }

    public void DisableCombo(){
       canCombo = false;
    }

    void CheckNextAction(){

        if(!m.InMiddleOfMovementAction()){
            string nextAction = inputQ.CheckQueue();
            if(nextAction == "LightAttack") {
                LightAttack();
                m.inputQ.ClearQueue();
            }
            /* if(nextAction == "ComboAttack"){
                IEnumerator waitForCombo = WaitForComboEnabled();
                StartCoroutine(waitForCombo);
            }*/
            
        }
        else if(m.canChain){
            string nextAction = inputQ.CheckQueue();
            if(nextAction == "RollAttack"){
                Debug.Log("rollattack");
                RollAttack();
                m.inputQ.ClearQueue();
            }
        }

    }


    public bool InMiddleOfCombatAction(){
        if(Game.control.player.attack.attacking) return true;
        return false;
    }

    
    IEnumerator WaitForComboEnabled(){
        yield return new WaitUntil(() => canCombo == true);
        Debug.Log("next c");
        ComboAttack();
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
