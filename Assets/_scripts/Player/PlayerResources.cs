using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerResources : MonoBehaviour
{
    PlayerHandler player;
    int currentStamina;


    public void Init(){
        player = Game.control.player;
        currentStamina = player.stats.maxStamina;
        Game.control.playerUI.UpdateStaminaText(currentStamina);
    }

    public void UseStamina(){
        
    }
}