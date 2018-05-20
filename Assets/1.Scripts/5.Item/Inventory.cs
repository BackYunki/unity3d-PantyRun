using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public enum Item{flaregun, heist, ensnare, tasergun, clocking, spray, trap, key, id_card}
    public string[] item = { "flaregun", "heist", "ensnare", "tasergun", "clocking", "spray", "trap", "key", "id_card" };
    bool[] hasItem = { false , false, false , false , false , false , false , false, false };
    int itemNum = 0;
    int oldNum = 0;
    GameObject Canvas;

    // Update is called once per frame
    private void Start()
    {
        Canvas = GameObject.Find("Canvas");
    }
    void Update () {
        if (Input.GetButtonDown("item")) {
            SwapItem();
        }
	}

    public void PutItem(int num)
    {
        itemNum = num;
        hasItem[num] = true;
        Enable();
        if (itemNum == oldNum) return;
        Disable();
    }

    private void SwapItem()
    {
        for (int i = 0; i < hasItem.Length - 2; i++)
        {
            itemNum = (itemNum + 1) % (hasItem.Length - 1);
            if (hasItem[itemNum] == true)
            {
                if (itemNum == oldNum)
                {
                    break;
                }
                Enable();
                Disable();
            }
        }
    }

    private void Enable()
    {
        switch (itemNum)
        {
            case -1:
                break;
            case (int)Item.flaregun:
                Canvas.transform.Find("flaregun").gameObject.SetActive(true);
                break;
            case (int)Item.heist:
                Canvas.transform.Find("heist").gameObject.SetActive(true);
                break;
            case (int)Item.ensnare:
                Canvas.transform.Find("ensnare").gameObject.SetActive(true);
                break;
            case (int)Item.tasergun:
                Canvas.transform.Find("tasergun").gameObject.SetActive(true);
                break;
            case (int)Item.clocking:
                Canvas.transform.Find("clocking").gameObject.SetActive(true);
                break;
            case (int)Item.spray:
                Canvas.transform.Find("spray").gameObject.SetActive(true);
                break;
            case (int)Item.trap:
                Canvas.transform.Find("trap").gameObject.SetActive(true);
                break;
            case (int)Item.key:
                Canvas.transform.Find("key").gameObject.SetActive(true);
                break;
            case (int)Item.id_card:
                Canvas.transform.Find("id_card").gameObject.SetActive(true);
                itemNum = oldNum;
                break;
        }
    }
    private void Disable()
    {
        switch (oldNum)
        {
            case -1:
                oldNum = itemNum;
                break;
            case (int)Item.flaregun:
                Canvas.transform.Find("flaregun").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
            case (int)Item.heist:
                Canvas.transform.Find("heist").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
            case (int)Item.ensnare:
                Canvas.transform.Find("ensnare").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
            case (int)Item.tasergun:
                Canvas.transform.Find("tasergun").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
            case (int)Item.clocking:
                Canvas.transform.Find("clocking").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
            case (int)Item.spray:
                Canvas.transform.Find("spray").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
            case (int)Item.trap:
                Canvas.transform.Find("trap").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
            case (int)Item.key:
                Canvas.transform.Find("key").gameObject.SetActive(false);
                oldNum = itemNum;
                break;
        }
    }

}
