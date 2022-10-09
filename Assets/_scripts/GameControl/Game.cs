using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game control;
    public PlayerHandler player;

    public CameraMovement cam;

    void Awake(){
        if (control	 == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}

        player = GameObject.FindWithTag("Player").GetComponent<PlayerHandler>();
        cam = Camera.main.GetComponent<CameraMovement>();
    }
}
