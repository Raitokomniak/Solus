using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputQueueing : MonoBehaviour
{
    string nextAction;
    Timer inputTimer;

    void Awake(){
        inputTimer = new Timer(1f);
    }

    void Update(){
        CalculateInputLifeTime();
    }

    public void ClearQueue(){ nextAction = ""; }

    public string CheckQueue(){ return nextAction; }

    public void QueueInput(string input){
//        Debug.Log("queue " + input);
        nextAction = input; 
    }


    public void CalculateInputLifeTime(){
        if(nextAction != ""){
            if(!inputTimer.TimeOut()) inputTimer.Tick();
            else if(inputTimer.TimeOut()) {
                nextAction = "";
                inputTimer.Reset();
            }
        }
    }

}
