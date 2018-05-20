using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoapUp : MonoBehaviour {

    public GameObject loap;

    GameObject player;
    MoveBehaviour playerMove;
    Rigidbody playerRigidboby;
    bool is_down;
    bool is_entered = false;
    float loapSpeed = 0.01f;

    private void OnTriggerEnter(Collider col)
    {
        //check the person hanging on the rope
        if (is_entered)
        {
            return;
        }
        
        is_entered = true;
        //check loap is down
        is_down = !GetComponentInParent<LoapDown>().enabled;

        if (!is_down)
        {
            is_entered = false;
            return;
        }

        //check collider is player
        if (col.tag == "Player")
        {
            player = col.gameObject;

            //playerMove = col.GetComponent<MoveBehaviour>();
            //playerMove.setHanging(true);
            col.GetComponent<Rigidbody>().isKinematic=true;

            StartCoroutine("Delay");
        }
        else
        {
            is_entered = false;
        }
    }

    IEnumerator Delay()
    {
        
        while (transform.localPosition.y < -1.5f)
        {
            transform.Translate(0, loapSpeed, 0);
            player.transform.Translate(0, loapSpeed, 0);
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadScene(3);
    }
}
