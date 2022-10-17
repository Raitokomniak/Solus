using UnityEngine;

public class Timer {
    float cap;
    public float count = 0;
    bool timerOn;

    public Timer(float cap){
        this.cap = cap;
    }

    public void Start(){
        timerOn = true;
    }
    public bool TimeOut(){
        if(count > cap) return true;
        return false;
    }

    public void Tick(){
        count+=Time.deltaTime;
    }

    public void Reset(){
        count = 0;
    }

    public void Stop(){
        timerOn = false;
    }

    public bool Ticking(){
        return timerOn;
    }
}