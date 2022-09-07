using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public Camera playerCam;
    private float playerXRotation;

    private string[] inventory = new string[2] {"", ""};

    private int itemSelect = 0; // item 1 held by default. 
    public float throwSpeed = 12f;
    private bool onPickupCooldown = false;

    public GameObject Flashlight;
    public GameObject Umbrella;
    public GameObject Hammer;
    public GameObject Teleporter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1")) {
            itemSelect = 0;
        }

        if (Input.GetKeyDown("2"))
        {
            itemSelect = 1;
        }

        // use item selected at slot. 
        if (Input.GetButtonDown("Fire1"))
        {
            UseItem(inventory[itemSelect]);
        }

        // throw item selected at slot. 
        if (Input.GetButtonDown("Fire2"))
        {
            ThrowItem(inventory[itemSelect], itemSelect);
        }
    }

    void UseItem(string itemName)
    {
        if (itemName == "")
        {
            Debug.Log("no item to use");
            return;
        }

        if (itemName == "Flashlight")
        {
            Debug.Log("use flashlight");
            return; 
        }

        if (itemName == "Umbrella")
        {
            Debug.Log("use umbrella");
            return; 
        }

        if (itemName == "Hammer")
        {
            Debug.Log("use hammer");
            return; 
        }

        if (itemName == "Teleporter")
        {
            Debug.Log("use teleporter");
            return; 
        }
    }

    void ThrowItem(string itemName, int index)
    {
        if (itemName == "")
        {
            Debug.Log("no item to throw");
        }
        else
        {
            inventory[index] = "";

            // TEMPORARY AND BAD REMOVE WHEN YOU CAN.
            if (itemName == "Flashlight")
            {
                GameObject ThrownItem = Instantiate(Flashlight, transform.position, playerCam.transform.rotation); 

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            if (itemName == "Umbrella")
            {
                GameObject ThrownItem = Instantiate(Umbrella, transform.position, playerCam.transform.rotation);

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            if (itemName == "Hammer")
            {
                GameObject ThrownItem = Instantiate(Hammer, transform.position, playerCam.transform.rotation);

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            if (itemName == "Teleporter")
            {
                GameObject ThrownItem = Instantiate(Teleporter, transform.position, playerCam.transform.rotation);

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            // when a player throws an item, they can't pick up their own item immediately. 
            StartCoroutine(pickupCooldown(0.2f));
        }

    }

    public bool CheckCollectItem(string itemName)
    {
        // if the player is on cooldown to pick up their own item (true), return. 
        if (onPickupCooldown)
        {
            return true;
        }

        // if inventory is full (true), return. 
        // if not, put item in open slot. 
        if (inventory[0] != "" && inventory[1] != "")
        {
            return true; 
        }

        // if slot 1 is empty, put the item in slot 1. 
        if (inventory[0] == "")
        {
            inventory[0] = itemName;
            return false;
        }

        // if slot 2 is empty, put the item in slot 2. 
        if (inventory[1] == "")
        {
            inventory[1] = itemName;
            return false;
        }

        return true;
    }

    IEnumerator pickupCooldown(float timer)
    {
        onPickupCooldown = true;
        yield return new WaitForSeconds(timer);
        onPickupCooldown = false;
    }
}
