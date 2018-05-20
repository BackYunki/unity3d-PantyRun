using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoapDown : MonoBehaviour {

    public GameObject loap;
    float currentLength = 0;
    float loapSize = 8;
    float loapSpeed = 0.01f;
    
	void OnEnable () {
        
        InvokeRepeating("Loapdown", 0, loapSpeed);
        
	}

    void Loapdown()
    {

        currentLength += loapSpeed;
        loap.transform.Translate(0, -loapSpeed, 0);

        if(currentLength >= loapSize)
        {
            CancelInvoke();
            this.enabled = false;
        }
    }
	
}
