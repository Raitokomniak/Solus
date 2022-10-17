using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour
{
    public bool grounded = true;
    
    void FixedUpdate(){
         RaycastHit hit;

         Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up), Color.red);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, .4f))
        {   
//            Debug.Log(hit.collider.tag);
            if(hit.collider.tag == "Ground") grounded = true;
        }
        else grounded = false;

        GetComponent<Collider>().isTrigger = !grounded;
       // Debug.Log("grounded " + grounded);
    }
    private void OnCollisionStay(Collision other) {
        if(other.gameObject.tag == "Ground") grounded = true;
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag == "Ground") grounded = true;
    }
}
