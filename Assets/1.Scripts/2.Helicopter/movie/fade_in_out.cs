using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade_in_out : MonoBehaviour
{
    public UnityEngine.UI.Image fade;
    float fade_in = 0.0f;
    float fade_out = 1.0f;
    float fades = 1.0f;
    float time = 0;

    bool is_fade_in = false;

    void Start()
    {
        InvokeRepeating("fade_In", 3, 0.1f);
        
        InvokeRepeating("fade_Out", 8, 0.1f);
    }

    private void fade_In()
    {
        if (fades > fade_in && !is_fade_in)
        {
            fades -= 0.1f;
            fade.color = new Color(0, 0, 0, fades);
        } else if(fades <= 0.0f)
        {
            is_fade_in = true;
        }
        
    }

    void fade_Out()
    {
        if (fades < fade_out)
        {
            fades += 0.1f;
            fade.color = new Color(0, 0, 0, fades);
        }
    }
}