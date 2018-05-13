using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loapDown : MonoBehaviour {

    GameObject loap;
    float currentLength = 0;
    float loapSize = 8;
    float loapSpeed = 0.01f;

	// Use this for initialization
	void OnEnable () {

        loap = GameObject.Find("cable_end");

        InvokeRepeating("loapdown", 0, loapSpeed);
        
	}

    void loapdown()
    {

        currentLength += loapSpeed;
        loap.transform.Translate(0, -loapSpeed, 0);

        if(currentLength >= loapSize)
        {
            CancelInvoke();
        }
    }
	
}
