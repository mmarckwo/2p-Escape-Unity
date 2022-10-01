using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerInventory : NetworkBehaviour
{

    public Camera playerCam;
    private float playerXRotation;

    private PlayerScript playerScript;

    private string[] inventory = new string[2] {"", ""};

    private int itemSelect = 0; // item 1 held by default. 
    public float throwSpeed = 12f;
    [Networked(OnChanged = nameof(OnHoldChanged))]
    public bool isHolding { get; set; }
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

    void Awake()
    {
        flashlightHold = this.transform.GetChild(3).GetChild(0).gameObject;
        umbrellaHold = this.transform.GetChild(3).GetChild(1).gameObject;
        hammerHold = this.transform.GetChild(3).GetChild(2).gameObject;
        teleporterHold = this.transform.GetChild(3).GetChild(3).gameObject;

        // get playerscript to change umbrella float. 
        playerScript = gameObject.GetComponent<PlayerScript>();

    }

    public override void FixedUpdateNetwork()
    {
        // get the input from the network.
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFirstItemButtonPressed)
            {
                StopHolding();
                itemSelect = 0;
                HoldItem(inventory[itemSelect]);
            }

            if (networkInputData.isSecondItemButtonPressed)
            {
                StopHolding();
                itemSelect = 1;
                HoldItem(inventory[itemSelect]);
            }

            // use item selected at slot.
            if (networkInputData.isUseButtonPressed)
                UseItem(inventory[itemSelect]);

            // throw item selected at slot.
            if (networkInputData.isThrowButtonPressed)
                ThrowItem(inventory[itemSelect], itemSelect, networkInputData.aimForwardVector);
        }

        //if (Input.GetKeyDown("1")) {
        //    StopHolding();
        //    itemSelect = 0;
        //    HoldItem(inventory[itemSelect]);
        //}

        //if (Input.GetKeyDown("2"))
        //{
        //    StopHolding();
        //    itemSelect = 1;
        //    HoldItem(inventory[itemSelect]);
        //}

        //// use item selected at slot. 
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    UseItem(inventory[itemSelect]);
        //}

        //// throw item selected at slot. 
        //if (Input.GetButtonDown("Fire2"))
        //{
        //    ThrowItem(inventory[itemSelect], itemSelect);
        //}
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

            if (umbrellaHold.GetComponent<Umbrella_USE>().networkStatus == false)
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

    void ThrowItem(string itemName, int index, Vector3 aimForwardVector)
    {

        if (itemName == "")
        {
            Debug.Log("no item to throw");
        }
        else
        {
            inventory[index] = "";

            // bad.
            if (itemName == "Flashlight")
            {
                GameObject ThrownItem = Instantiate(Flashlight, transform.position + transform.forward, playerCam.transform.rotation); // playerCam.transform.rotation

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            if (itemName == "Umbrella")
            {
                //GameObject ThrownItem = Instantiate(Umbrella, transform.position + transform.forward, playerCam.transform.rotation);

                Runner.Spawn(Umbrella, transform.position + transform.forward, playerCam.transform.rotation, Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Umbrella_USE>().Init(throwSpeed);
                });

                //ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            if (itemName == "Hammer")
            {
                GameObject ThrownItem = Instantiate(Hammer, transform.position + transform.forward, playerCam.transform.rotation);

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            if (itemName == "Teleporter")
            {
                GameObject ThrownItem = Instantiate(Teleporter, transform.position + transform.forward, playerCam.transform.rotation);

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            StopHolding();

            // when a player throws an item, they can't pick up their own item immediately or hold. 
            StartCoroutine(pickupAndHoldCooldown(0.2f));
        }

    }

    static void OnHoldChanged(Changed<PlayerInventory> changed)
    {
        bool isHoldingCurrent = changed.Behaviour.isHolding;

        // load the old value.
        changed.LoadOld();

        bool isHoldingOld = changed.Behaviour.isHolding;

        if (isHoldingCurrent && !isHoldingOld)
            changed.Behaviour.HoldItemRemote();
    }

    void HoldItemRemote()
    {
        if (Object.HasInputAuthority)
            HoldItem(inventory[itemSelect]);
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
            Debug.Log("added " + itemName + " to " + this.gameObject + " on slot 0");
            return false;
        }

        // if slot 2 is empty, put the item in slot 2. 
        if (inventory[1] == "")
        {
            inventory[1] = itemName;
            Debug.Log("added " + itemName + " to " + this.gameObject + " on slot 0");
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

    IEnumerator pickupAndHoldCooldown(float timer)
    {
        onPickupCooldown = true;
        isHolding = true;
        yield return new WaitForSeconds(timer);
        isHolding = false;
        onPickupCooldown = false;
    }
}
