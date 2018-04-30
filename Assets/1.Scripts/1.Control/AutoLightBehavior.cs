using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AutoLightBehavior : NetworkBehaviour
{

    public Light bulb;
    [Tooltip("불꺼지는 시간")]
    public float time = 5.0f;
    private Collider col;
    private float savetime = 0;
    private bool isExit = false;

    void OnTriggerStay(Collider col)
    {
        this.col = col;
        isExit = false;
    }

    void OnTriggerExit(Collider col)
    {
        isExit = true;
    }

    void Update()
    {
        if( isExit == false )
        {
            if (col != null && col.GetComponentInChildren<MoveBehaviour>().IsMoving())
            {
                savetime = 0;
                bulb.enabled = true;
            }
        }        

        savetime += Time.deltaTime;

        if (savetime > 5.0f)
        {
            bulb.enabled = false;
            savetime = 0;
        }
    }
}
