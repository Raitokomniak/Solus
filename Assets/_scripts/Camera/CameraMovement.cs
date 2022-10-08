using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    Vector3 offSet;

    Vector3 defaultPos;
    Quaternion defaultRot;

    Quaternion targetRot;
    float rotationX;
    float rotationY;


    bool debug;

    void Awake(){
        offSet = new Vector3(0,-1.5f,3);
        defaultPos = transform.position;
        defaultRot = transform.rotation;
    }

    void LateUpdate(){
        if(Input.GetButtonDown("Target")) Center();

        rotationX -= Input.GetAxis("Mouse Y");
        rotationX = Mathf.Clamp(rotationX, -25, 45);
        rotationY += Input.GetAxis("Mouse X");

        targetRot = Quaternion.Euler(rotationX, rotationY, 0);
        transform.rotation = targetRot;
        transform.position = player.position  - targetRot * offSet;
    }

    void Center(){
        rotationX = 0;
        rotationY = player.rotation.eulerAngles.y;
        targetRot = Quaternion.Euler(rotationX, rotationY, 0);
        transform.rotation = targetRot;
        transform.position = player.position - targetRot * offSet;
    }
}
