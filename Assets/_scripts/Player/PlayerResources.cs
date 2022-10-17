using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaminaRequirements {
    public int backstepStamina = 30;
    public int rollStamina = 30;
    public int lightAttackStamina = 30;
    public int heavyAttackStamina = 60;
    public int sprintStamina = 1;
}

public class PlayerResources : MonoBehaviour
{
    public StaminaRequirements staReq;
    PlayerHandler player;
    int currentStamina;

    bool drainingStamina = false;

    public bool staminaSystemDisabled;


    public void Init(){
        player = Game.control.player;
        staReq = new StaminaRequirements();
        currentStamina = player.stats.maxStamina;
        Game.control.playerUI.UpdateStaminaText(currentStamina);
        InvokeRepeating("RegainStamina", 0, .025f);
    }


    public void RegainStamina(){
        if(player.movement.InMiddleOfMovementAction() || player.attack.InMiddleOfCombatAction()) return;
        if(currentStamina >= player.stats.maxStamina) return;
        if(drainingStamina) return;

        currentStamina += 1;
        Game.control.playerUI.UpdateStaminaText(currentStamina);
    }

    public void UseStaminaForAction(string action){
        int value = 0;

        if(action == "Backstep")        value = staReq.backstepStamina;
        if(action == "Roll")            value = staReq.rollStamina;
        if(action == "LightAttack")     value = staReq.lightAttackStamina;
        if(action == "HeavyAttack")     value = staReq.heavyAttackStamina;
        if(action == "SprintStamina")   value = staReq.sprintStamina;
        
        ReduceStamina(value);
    }

    void ReduceStamina(int value){
        currentStamina -= value;
        if(currentStamina < 0) currentStamina = 0;
        Game.control.playerUI.UpdateStaminaText(currentStamina);
    }

    void DrainStamina(){
        drainingStamina = true;
        ReduceStamina(1);
    }

    public void SprintStaminaDrain(bool toggle){
        if(toggle && !drainingStamina) InvokeRepeating("DrainStamina", 0, 0.1f);
        if(!toggle) {
            CancelInvoke("DrainStamina");
            drainingStamina = false;
        }
    }

    public bool CanExecute(string action){
        if(staminaSystemDisabled) return true;
        
        if(action == "Backstep"      && currentStamina >= staReq.backstepStamina)    return true;
        if(action == "Roll"          && currentStamina >= staReq.rollStamina)        return true;
        if(action == "LightAttack"   && currentStamina >= staReq.lightAttackStamina) return true;
        if(action == "HeavyAttack"   && currentStamina >= staReq.heavyAttackStamina) return true;
        if(action == "SprintStamina" && currentStamina >= staReq.sprintStamina)      return true;
        return false;
    }
}