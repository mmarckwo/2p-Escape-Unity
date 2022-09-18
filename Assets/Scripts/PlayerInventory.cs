using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public Camera playerCam;
    private float playerXRotation;

    private PlayerScript playerScript;

    private string[] inventory = new string[2] {"", ""};

    private int itemSelect = 0; // item 1 held by default. 
    public float throwSpeed = 12f;
    private bool onPickupCooldown = false;

    public GameObject Flashlight;
    public GameObject Umbrella;
    public int umbrellaFloatStrength = 5; 
    public GameObject Hammer;
    public GameObject Teleporter;

    private GameObject flashlightHold;
    private GameObject umbrellaHold;
    private GameObject hammerHold;
    private GameObject teleporterHold;

    // Start is called before the first frame update
    void Start()
    {
        flashlightHold = this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        umbrellaHold = this.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        hammerHold = this.transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
        teleporterHold = this.transform.GetChild(0).GetChild(0).GetChild(3).gameObject;

        // get playerscript to change umbrella float. 
        playerScript = gameObject.GetComponent<PlayerScript>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1")) {
            StopHolding();
            itemSelect = 0;
            HoldItem(inventory[itemSelect]);
        }

        if (Input.GetKeyDown("2"))
        {
            StopHolding();
            itemSelect = 1;
            HoldItem(inventory[itemSelect]);
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
            flashlightHold.GetComponent<Flashlight_USE>().toggleLight();
            return; 
        }

        if (itemName == "Umbrella")
        {
            umbrellaHold.GetComponent<Umbrella_USE>().toggleCanopy();

            if (umbrellaHold.GetComponent<Umbrella_USE>().status == false)
            {
                playerScript.umbrellaFloat = 1;
            } 
            else
            {
                playerScript.umbrellaFloat = umbrellaFloatStrength;
                playerScript.UmbrellaFall();
            }
            return; 
        }

        if (itemName == "Hammer")
        {
            Debug.Log("use hammer");
            return; 
        }

        if (itemName == "Teleporter")
        {
            teleporterHold.GetComponent<Teleporter_USE>().useTeleporter(this.gameObject);
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

            StopHolding();

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

    void StopHolding()
    {
        flashlightHold.SetActive(false);
        umbrellaHold.SetActive(false);
        hammerHold.SetActive(false);
        teleporterHold.SetActive(false);

        // reset umbrella float since player isn't holding anything. 
        playerScript.umbrellaFloat = 1;
    }

    void HoldItem(string itemName)
    {
        if (itemName == "") return;

        if (itemName == "Flashlight")
        {
            flashlightHold.SetActive(true);
        }

        if (itemName == "Umbrella")
        {
            umbrellaHold.SetActive(true);
        }

        if (itemName == "Hammer")
        {
            hammerHold.SetActive(true);
        }

        if (itemName == "Teleporter")
        {
            teleporterHold.SetActive(true);
        }
    }

    IEnumerator pickupCooldown(float timer)
    {
        onPickupCooldown = true;
        yield return new WaitForSeconds(timer);
        onPickupCooldown = false;
    }
}