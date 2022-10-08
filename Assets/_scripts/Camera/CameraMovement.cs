using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    Vector3 offSet;

    Quaternion targetRot;
    float rotationX;
    float rotationY;


    void Awake(){
        offSet = new Vector3(0,-1.5f,3);
    }

    void LateUpdate(){
        if(Input.GetButtonDown("Target")) Center();
    
        FollowAxes();
        FollowPlayer();
    }

    void FollowAxes(){
        rotationX -= Input.GetAxis("Mouse Y");
        rotationX = Mathf.Clamp(rotationX, -25, 45);
        rotationY += Input.GetAxis("Mouse X");
    }

    void FollowPlayer(){
        targetRot = Quaternion.Euler(rotationX, rotationY, 0);
        transform.rotation = targetRot;
        transform.position = player.position  - targetRot * offSet;
    }

    //Centers camera behind player
    void Center(){
        rotationX = 0;
        rotationY = player.rotation.eulerAngles.y;
        FollowPlayer();
    }
}
