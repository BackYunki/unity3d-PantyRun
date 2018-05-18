using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AutoLightBehavior : NetworkBehaviour
{
    // 조명 프리팹
    public Light bulb;
    // 불 꺼지는 시간
    [Tooltip("불꺼지는 시간")]
    public float time = 5.0f;
    // 트리거
    private Collider col;
    // 측정기
    private float savetime = 0;
    // 출입여부
    private bool isExit = false;

    // 트리거 안에 들어와 있을 때
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
        if (isExit == false)
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
