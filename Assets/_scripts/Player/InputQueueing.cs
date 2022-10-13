using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputQueueing : MonoBehaviour
{
    List<string> inputQueue;
    Timer inputTimer;

    public bool rollAttack;

    void Awake(){
        inputQueue = new List<string>();
        inputTimer = new Timer(1f);
    }

    //////////////////////////////////////////
    // INPUT QUEUEING
    //////////////////////////////////////////

    public bool InMiddleOfAction(){
        if(Game.control.player.movement.rolling) return true;
        if(Game.control.player.movement.backstepping) return true;
        if(Game.control.player.attack.attacking) return true;
        return false;
    }
    
    public string CheckQueue(){
        if(inputQueue.Count > 0){
            CalculateInputLifeTime();
            if(!InMiddleOfAction()) return CheckQueueForInputs();
        }
        return "";
    }

    
    public void QueueInput(string input){
        if(Game.control.player.movement.rolling && input == "LightAttack") {
            Debug.Log("turn to roll attakc");
            input = "RollAttack";
        }
        inputQueue.Add(input);
    }


    public string CheckQueueForInputs(){
        if(inputQueue.Count > 0){
            string nextAction = inputQueue[0];
            inputQueue.Clear();
            return nextAction;
        }
        return "";
    }

    public void CalculateInputLifeTime(){
        if(!inputTimer.TimeOut()) inputTimer.Tick();
        else if(inputTimer.TimeOut()) {   
            inputQueue.Clear();
            inputTimer.Reset();
        }
    }

}
