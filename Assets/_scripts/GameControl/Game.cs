using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game control;
    public PlayerHandler player;
    public PlayerUI playerUI;

    
    public CameraMovement cam;

    bool started = false;

    Vector3 playerOrigPos;

    void Awake(){
        if (control	 == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}

        player = GameObject.FindWithTag("Player").GetComponent<PlayerHandler>();
        playerOrigPos = player.transform.position;
        player.gameObject.SetActive(false);
        playerUI = GetComponent<PlayerUI>();
        cam = Camera.main.GetComponent<CameraMovement>();
        player.Init();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Return)){
            StartGame();
            player.transform.position = playerOrigPos;
            player.Init();
        }
    }

    void StartGame(){
        started = true;
        player.gameObject.SetActive(true);
    }


}
