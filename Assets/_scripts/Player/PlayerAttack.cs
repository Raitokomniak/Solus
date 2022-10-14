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

    Timer heavyAxisTimer;

    string combo;


    void Awake(){
        inputQ = GetComponent<InputQueueing>();
        heavyAxisTimer = new Timer(0.5f);
    }

    bool HeavyInput(){
        if(!heavyAxisTimer.Ticking() && Input.GetAxis("HeavyAttack") > 0) {
            heavyAxisTimer.Start();
            return true;
        }
        return false;
    }

    void LateUpdate(){
        if(player == null && Game.control != null) player = Game.control.player;
        if(m == null) m = player.movement;

        CheckNextAction();
        if(heavyAxisTimer.Ticking()) heavyAxisTimer.Tick();
        if(heavyAxisTimer.TimeOut()) {
            heavyAxisTimer.Reset();
            heavyAxisTimer.Stop();
        }

        if(!attacking) {
            canCombo = false;
            Game.control.player.animator.ResetTrigger("LightLightCombo");
        }

        if(m.InMiddleOfMovementAction()){
            if(Input.GetButtonDown("LightAttack")) 
                inputQ.QueueInput("RollAttack");
            else if(HeavyInput()) {
                inputQ.QueueInput("HeavyAttack");
            }
        }
        else if(InMiddleOfCombatAction()) {
            if(Input.GetButtonDown("LightAttack")){
                if(canCombo) ComboAttack();
                else inputQ.QueueInput("ComboAttack");
            }
            else if(HeavyInput()){
                if(canCombo) HeavyAttack();
                else inputQ.QueueInput("HeavyAttack");
            }
        }
        else if(!attacking){
            if(Input.GetButtonDown("LightAttack")) LightAttack();
            else if(HeavyInput()) HeavyAttack();
        }

/*
        if(Input.GetButtonDown("LightAttack")){
            if(m.InMiddleOfMovementAction())  inputQ.QueueInput("RollAttack");
            else if(InMiddleOfCombatAction()) {
                if(canCombo) ComboAttack();
                else inputQ.QueueInput("ComboAttack");
            }
            else if(!attacking) LightAttack();
        }
        else if(!heavyAxisTimer.Ticking() && Input.GetAxis("HeavyAttack") > 0){
            heavyAxisTimer.Start();
            if(m.InMiddleOfMovementAction())  {
                Debug.Log("inmiddleofmovement");
                inputQ.QueueInput("HeavyAttack");
            }
            else if(InMiddleOfCombatAction()) {
                Debug.Log("inmiddleofcombat");
                if(canCombo) HeavyAttack();
                else inputQ.QueueInput("HeavyAttack");
            }
            else if(!attacking) {
                Debug.Log("not attacking");
                HeavyAttack();
            }
        }*/
    }

    public bool CanChain(){
        if(attacking && canCombo) return true;
        return false;
    }

    void ComboAttack(){
      //  Debug.Log("comboattack");
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
            if(InMiddleOfCombatAction() && canCombo){
                if(nextAction == "HeavyAttack"){
                    HeavyAttack();
                    m.inputQ.ClearQueue();
                }
            }
        }
        else if(m.canChain){
            if(inputQ.CheckQueue() == "RollAttack"){
         //       Debug.Log("rollattack");
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

    void HeavyAttack(){
        Debug.Log("here");
     //   m.canChain = false;
        player.Animate("HeavyAttack");
        attacking = true;
    }

    public void EndAttack(){
        attacking = false;
        canCombo = false;
        if(middleOfCombo) middleOfCombo = false;
    }
}
