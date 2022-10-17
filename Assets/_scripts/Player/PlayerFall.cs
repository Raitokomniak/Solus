using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    PlayerMovement m;
    Timer fallTimer, landingTimer;
    bool hardLanding, softLanding;

    void Awake(){
        m = GetComponent<PlayerMovement>();
        fallTimer = new Timer(0.05f);
    }

    void Update(){
        if(m.rolling || m.backstepping) EndLanding();
        FallTimer();
    }
    
    void FixedUpdate(){
        DetectFall();
    }
        
    void FallTimer(){
        if(m.falling && fallTimer.Ticking()){
            fallTimer.Tick();
            if(fallTimer.TimeOut()){
                GetComponent<Collider>().isTrigger = false;
                fallTimer.Stop();
            }
        }
        else if(fallTimer.Ticking() && m.groundDetect.grounded){
            fallTimer.Stop();
            fallTimer.Reset();
        }
    }

    void DetectFall(){
        if(!m.falling && !m.groundDetect.grounded) {
         //   m.rb.AddForce(transform.forward * 2);
            if(!fallTimer.Ticking()) {
                fallTimer = new Timer(0.6f);
                m.rb.drag -= fallTimer.count;
                GetComponent<Collider>().isTrigger = true;
                fallTimer.Start();
            }
            else {
                fallTimer.Tick();
                if(fallTimer.TimeOut()){
                    Fall();
                    fallTimer.Stop();
                    fallTimer.Reset();
                }
            }
        }
        else if(m.falling && !m.groundDetect.grounded) {
            if(!softLanding && !hardLanding && landingTimer.Ticking()) {
                landingTimer.Tick();
                if(landingTimer.TimeOut()){
                    Debug.Log("timeout");
                    softLanding = true;
                    landingTimer.Stop();
                    landingTimer.Reset();
                    landingTimer = new Timer(0.5f);
                    landingTimer.Start();
                }
            }
            else if(softLanding && !hardLanding && landingTimer.Ticking()){
                landingTimer.Tick();
                if(landingTimer.TimeOut()){
                    Debug.Log("timeout");
                    softLanding = false;
                    hardLanding = true;
                    landingTimer.Stop();
                    landingTimer.Reset();
                    landingTimer = new Timer(0.5f);
                }
            }
        }
        else if(m.falling && m.groundDetect.grounded) EndFall();
        
    }

    public void Fall(){
        if(m.rb.velocity.y > 0) return;

        m.falling = true;
        GetComponent<Collider>().isTrigger = false;
        hardLanding = false;
        m.backstepping = false;
        m.rolling = false;
        
        fallTimer = new Timer(.1f);
        fallTimer.Start();
        landingTimer = new Timer(0.5f);
        landingTimer.Start();
        Game.control.player.Animate("Falling", true);
        Game.control.player.Animate("Drop");
    }

    void EndFall(){
        m.falling = false;
        
        if(landingTimer.Ticking()) {
            landingTimer.Stop();
            landingTimer.Reset();
        }
        m.landing = true;
        Game.control.player.Animate("Falling", false);
        Game.control.player.Animate("Landing", true);
        if(softLanding) Game.control.player.Animate("LandSoft");
        else if(hardLanding) Game.control.player.Animate("LandHard");
        else m.landing = false;
        softLanding = false;
        hardLanding = false;
    }

    public void EndLanding(){
        Game.control.player.Animate("Landing", false);
        m.landing = false;
        m.falling = false;
    }
}
