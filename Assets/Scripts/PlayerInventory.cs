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
    [Networked(OnChanged = nameof(OnHoldChanged))]
    public string networkedInventory { get; set; }

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
                itemSelect = 0;
                HoldItem(inventory[itemSelect]);
            }

            if (networkInputData.isSecondItemButtonPressed)
            {
                itemSelect = 1;
                HoldItem(inventory[itemSelect]);
            }

            // use item selected at slot.
            if (networkInputData.isUseButtonPressed)
                UseItem(inventory[itemSelect]);

            // throw item selected at slot.
            if (networkInputData.isThrowButtonPressed)
                ThrowItem(inventory[itemSelect], itemSelect, networkInputData.aimForwardVector);

            // make flashlight rotate to let players aim vertically.
            if (flashlightHold.activeInHierarchy)
            {
                flashlightHold.transform.rotation = Quaternion.LookRotation(networkInputData.aimForwardVector);
            }
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
            if (flashlightHold.activeInHierarchy == false) return;

            flashlightHold.GetComponent<Flashlight_USE>().toggleLight();
            return; 
        }

        if (itemName == "Umbrella")
        {
            if (umbrellaHold.activeInHierarchy == false) return;

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
            if (teleporterHold.activeInHierarchy == false) return;

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

            // ok.
            if (itemName == "Flashlight")
            {
                if (flashlightHold.activeInHierarchy == false) return;

                Runner.Spawn(Flashlight, transform.position + transform.forward, Quaternion.LookRotation(aimForwardVector), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Flashlight_USE>().Init(throwSpeed);
                });
            }

            if (itemName == "Umbrella")
            {
                if (umbrellaHold.activeInHierarchy == false) return;

                Runner.Spawn(Umbrella, transform.position + transform.forward, Quaternion.LookRotation(aimForwardVector), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Umbrella_USE>().Init(throwSpeed);
                });
            }

            if (itemName == "Hammer")
            {
                GameObject ThrownItem = Instantiate(Hammer, transform.position + transform.forward, playerCam.transform.rotation);

                ThrownItem.GetComponent<Rigidbody>().AddRelativeForce(0, 0, throwSpeed, ForceMode.Impulse);
            }

            if (itemName == "Teleporter")
            {
                if (teleporterHold.activeInHierarchy == false) return;

                Runner.Spawn(Teleporter, transform.position + transform.forward, Quaternion.LookRotation(aimForwardVector), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Teleporter_USE>().Init(throwSpeed);
                });
            }

            inventory[index] = "";
            networkedInventory = "";

            // when a player throws an item, they can't pick up their own item immediately or hold. 
            StartCoroutine(pickupCooldown(0.2f));
        }

    }

    //static void OnHoldChanged(Changed<PlayerInventory> changed)
    //{
    //    bool isHoldingCurrent = changed.Behaviour.isHolding;

    //    // load the old value.
    //    changed.LoadOld();

    //    bool isHoldingOld = changed.Behaviour.isHolding;

    //    if (isHoldingCurrent && !isHoldingOld)
    //    {
    //        changed.Behaviour.HoldItemRemote();
    //    }   
    //}

    //void HoldItemRemote()
    //{
    //    if (!Object.HasInputAuthority)
    //    {
    //        HoldItem(networkedInventory);
    //    }
    //}

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
            //networkedInventory = inventory[0];
            return false;
        }

        // if slot 2 is empty, put the item in slot 2. 
        if (inventory[1] == "")
        {
            inventory[1] = itemName;
            //networkedInventory = inventory[1];
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

    // local call.
    void HoldItem(string itemName)
    {
        if (itemName == "") 
        {
            networkedInventory = "";
            return;
        } 

        if (itemName == "Flashlight")
        {
            networkedInventory = "Flashlight";
        }

        if (itemName == "Umbrella")
        {
            networkedInventory = "Umbrella";
        }

        if (itemName == "Hammer")
        {
            networkedInventory = "Hammer";
            hammerHold.SetActive(true);
        }

        if (itemName == "Teleporter")
        {
            networkedInventory = "Teleporter";
        }
    }

    // networked call.
    static void OnHoldChanged(Changed<PlayerInventory> changed)
    {
        if (changed.Behaviour.networkedInventory == "")
        {
            changed.Behaviour.StopHolding();
            return;
        }

        if (changed.Behaviour.networkedInventory == "Flashlight")
        {
            changed.Behaviour.StopHolding();
            changed.Behaviour.flashlightHold.SetActive(true);
        }

        if (changed.Behaviour.networkedInventory == "Umbrella")
        {
            changed.Behaviour.StopHolding();
            changed.Behaviour.umbrellaHold.GetComponent<Umbrella_USE>().networkStatus = false;
            changed.Behaviour.umbrellaHold.SetActive(true);
        }

        // HAMMER HERE.

        if (changed.Behaviour.networkedInventory == "Teleporter")
        {
            changed.Behaviour.StopHolding();
            changed.Behaviour.teleporterHold.SetActive(true);
        }
    }

    IEnumerator pickupCooldown(float timer)
    {
        onPickupCooldown = true;
        yield return new WaitForSeconds(timer);
        onPickupCooldown = false;
    }
}
