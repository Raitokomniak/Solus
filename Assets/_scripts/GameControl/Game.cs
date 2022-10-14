using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game control;
    public PlayerHandler player;

    public CameraMovement cam;

    bool started = false;

    void Awake(){
        if (control	 == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}

        player = GameObject.FindWithTag("Player").GetComponent<PlayerHandler>();
        player.gameObject.SetActive(false);
        cam = Camera.main.GetComponent<CameraMovement>();
    }

    void Update(){
        if(!started && Input.GetKeyDown(KeyCode.Return)){
            StartGame();
        }
    }

    void StartGame(){
        started = true;
        player.gameObject.SetActive(true);
    }
}
