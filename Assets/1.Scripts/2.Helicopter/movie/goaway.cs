using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goaway : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InvokeRepeating("heli_move", 3, 0.03f);
	}
    private void heli_move()
    {
        float x = transform.position.x - 1, y = transform.position.y, z = transform.position.z;
        transform.position = new Vector3(x, y, z);
    }
    
}
