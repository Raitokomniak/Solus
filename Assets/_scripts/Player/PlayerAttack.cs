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

    void FixedUpdate(){
        if(attacking) ForcedAttackMovement();
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

        if(Game.control.player.resources.CanExecute("LightAttack") || Game.control.player.resources.CanExecute("HeavyAttack")){
            if(m.InMiddleOfMovementAction()){
                if(Input.GetButtonDown("LightAttack")) 
                    inputQ.QueueInput("RollAttack");
                else if(HeavyInput()) {
                    inputQ.QueueInput("HeavyAttack");
                }
            }
            else if(Game.control.player.resources.CanExecute("LightAttack") && InMiddleOfCombatAction()) {
                if(Input.GetButtonDown("LightAttack")){
                    if(canCombo) ComboAttack();
                    else inputQ.QueueInput("ComboAttack");
                }
                else if(HeavyInput()){
                    if(canCombo) Attack("HeavyAttack");
                    else inputQ.QueueInput("HeavyAttack");
                }
            }
            else if(!attacking){
                if(Input.GetButtonDown("LightAttack")) Attack("LightAttack");
                else if(HeavyInput()) Attack("HeavyAttack");
            }
        }
    }

    public bool CanChain(){
        if(attacking && canCombo) return true;
        return false;
    }

    void ComboAttack(){
        Game.control.player.resources.UseStaminaForAction("LightAttack");
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
                Attack("LightAttack");
                m.inputQ.ClearQueue();
            }
            if(Game.control.player.resources.CanExecute("HeavyAttack") && InMiddleOfCombatAction() && canCombo){
                if(nextAction == "HeavyAttack"){
                    Attack("HeavyAttack");
                    m.inputQ.ClearQueue();
                }
            }
        }
        else if(m.canChain){
            if(Game.control.player.resources.CanExecute("LightAttack") && inputQ.CheckQueue() == "RollAttack"){
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
        Game.control.player.resources.UseStaminaForAction("LightAttack");
        player.Animate("RollAttack");
        attacking = true;
    }

    void Attack(string weight){
       // if(m.strafing) transform.rotation = Quaternion.LookRotation(transform.forward);
        Game.control.player.resources.UseStaminaForAction(weight);
        player.Animate(weight);
        attacking = true;
    }

    void ForcedAttackMovement(){
       m.moveSpeed = Game.control.player.animator.GetFloat("MoveSpeed") * m.properties.rollSpeed * 2;
       if(m.moveDir.magnitude != 0) transform.rotation = Quaternion.LookRotation(m.moveDir);
       if(Game.control.player.movement.groundDetect.grounded) GetComponent<Rigidbody>().velocity = transform.forward * m.moveSpeed;
       else GetComponent<Rigidbody>().velocity = transform.forward * m.moveSpeed / 3;
       m.ForceGravity();
    }

/*
    void LightAttack(){
        Game.control.player.resources.UseStamina("LightAttack");
        player.Animate("LightAttack");
        attacking = true;
    }

    void HeavyAttack(){
        Game.control.player.resources.UseStamina("HeavyAttack");
        player.Animate("HeavyAttack");
        attacking = true;
    }*/

    public void EndAttack(){
        attacking = false;
        canCombo = false;
        if(middleOfCombo) middleOfCombo = false;
    }
}
