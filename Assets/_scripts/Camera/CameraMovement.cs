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

    Transform target;
    public bool targeting;

    bool camlocked;
    float tempz;

    void Awake(){
        offSet = new Vector3(0,-1.5f,3);
    }

    void LateUpdate(){
        if(Input.GetButtonDown("Target")) {
            if(!targeting && Game.control.player.EnemiesInRange()) TargetEnemy(Game.control.player.targetingRange.GetClosestEnemy());
           // else if(!targeting && Game.control.player.EnemiesInRange()) Center();
            else if(!targeting && !Game.control.player.EnemiesInRange()) Center();
            else if(targeting) ReleaseTarget();
        }

        if(targeting) FollowTarget();
        else {
            FollowPlayer();
            FollowAxes();
        }
    }

    void FollowAxes(){
        rotationX -= Input.GetAxis("Mouse Y");
        rotationX = Mathf.Clamp(rotationX, -25, 45);
        rotationY += Input.GetAxis("Mouse X");
    }

    void FollowTarget(){
        Vector3 playerDistanceVector = (player.position - target.position);
        float playerDistanceToEnemy = playerDistanceVector.magnitude;
        Vector3 camDistanceVector = transform.position - (target.position + new Vector3(0,-playerDistanceToEnemy,playerDistanceToEnemy));
        float camDistanceToEnemy = camDistanceVector.magnitude;
        
        targetRot = Quaternion.LookRotation(transform.position - target.position);
        transform.position = player.position - (targetRot * new Vector3(0,-2,-4));
        transform.LookAt(target);
        
        //correct rotation
        if(transform.rotation.eulerAngles.x> 19.5f) transform.rotation = Quaternion.Euler(19.5f, transform.rotation.eulerAngles.y, 0);
        //correct y pos
        if(transform.position.y > 2) transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        //correct z pos
        camlocked = playerDistanceToEnemy < 5;
        if(camlocked) tempz = transform.position.z;
        if(playerDistanceToEnemy < 4) transform.position = new Vector3(transform.position.x,transform.position.y,tempz);
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


    //Target enemy
    void TargetEnemy(Transform enemy){
        targeting = true;
        target = enemy;
        Game.control.player.movement.TargetEnemy(enemy);
        Debug.Log("target");
    }

    public void ReleaseTarget(){
        targeting = false;
        Game.control.player.movement.ReleaseTarget();
        Center();
    }
}
