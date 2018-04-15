using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBehavior : MonoBehaviour {

    public AudioClip openSfx;
    public AudioClip closeSfx;

    public float smooth = 2.0f;
    public float openAngle = 90.0f;

    private bool isOpen = false;
    private bool isEnter = false;

    private Vector3 defaultRot;
    private Vector3 openRot;

	// Use this for initialization
	void Start () {
        defaultRot = transform.eulerAngles;
        openRot = defaultRot + new Vector3(0, openAngle, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (isOpen)
        {
            GetComponents<BoxCollider>()[0].enabled = false;
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        }
        else
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaultRot, Time.deltaTime * smooth);
            GetComponents<BoxCollider>()[0].enabled = true;
        }

        if (Input.GetKeyDown("f") && isEnter)
        {
            isOpen = !isOpen;

            if(isOpen)
            {
                GetComponent<AudioSource>().PlayOneShot(openSfx);
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(closeSfx);
            }
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            isEnter = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            isEnter = false;
        }
    }
}
