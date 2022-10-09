using UnityEngine;

public class Timer {
    float cap;
    float count = 0;

    public Timer(float cap){
        this.cap = cap;
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
}