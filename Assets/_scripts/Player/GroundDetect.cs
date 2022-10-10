using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour
{
    public bool grounded;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Ground") grounded = true;
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Ground") grounded = false;
    }
}
