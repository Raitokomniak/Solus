using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementProperties {

    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public float turnSpeed = 15f;
    
    public float runThreshold = 0.8f;

    public MovementProperties(){}
}

public class PlayerMovement : MonoBehaviour
{
    MovementProperties m;
    public Animator animator;
    public Transform cameraT;
    Vector3 moveDir;

    float moveSpeed;
    bool walking;
    bool running;
    bool sprinting;




    




    void Awake(){
        m = new MovementProperties();
       cameraT = Camera.main.gameObject.transform;
    }

    void FixedUpdate() {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Move(input);
       
    }

    void Move(Vector2 input){
        moveDir = (cameraT.right*input.x) + (Vector3.Cross(cameraT.right, Vector3.up) * input.y).normalized;
        // = new Vector3(input.x, 0, input.y);

        if(moveDir.magnitude != 0) { 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), m.turnSpeed * Time.deltaTime);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        
        DetermineMoveSpeed(input);
    }

    void DetermineMoveSpeed(Vector2 input){
        float speed = input.magnitude;
//        Debug.Log(speed);

        walking = speed > 0 && speed < m.runThreshold;
        running = speed >= m.runThreshold;
        sprinting = Input.GetButton("Sprint");

        if(walking) moveSpeed = m.walkSpeed;
        if(running) moveSpeed = m.runSpeed;
        if(sprinting) moveSpeed = m.sprintSpeed;
      
        animator.SetBool("Walking", walking);
        animator.SetBool("Running", running);
        animator.SetBool("Sprinting", sprinting);
    }
}
