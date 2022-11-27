using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class PlayerInventory : NetworkBehaviour
{

    public Camera playerCam;
    private float playerXRotation;

    private PlayerScript playerScript;

    public GameObject lockedKeyDoor;

    [SerializeField]
    private string[] inventory = new string[2] {"", ""};
    [Networked(OnChanged = nameof(OnHoldChanged))]
    public string networkedInventory { get; set; }
    [Networked]
    private string itemToAdd { get; set; }
    [Networked]
    private int slotToAdd { get; set; }
    [Networked(OnChanged = nameof(OnSlotAddChange))]
    private bool toggleInventoryAddition { get; set; }

    private int itemSelect = 0; // item 1 held by default. 
    public float throwSpeed = 12f;
    TickTimer pickupCooldownTimer = TickTimer.None;
    private bool onPickupCooldown = false;

    public GameObject Flashlight;
    public GameObject Umbrella;
    public int umbrellaFloatStrength = 5; 
    public GameObject Hammer;
    public GameObject Teleporter;
    public GameObject portal;
    public GameObject Key;

    private GameObject flashlightHold;
    private GameObject umbrellaHold;
    private GameObject hammerHold;
    private GameObject teleporterHold;
    private GameObject keyHold;

    [Header("UI")]
    public Image inventorySlotImg1;
    public Image inventorySlotImg2;
    public TMP_Text slot1Text;
    public TMP_Text slot2Text;

    public Sprite flashlightSprite;
    public Sprite umbrellaSprite;
    public Sprite hammerSprite;
    public Sprite teleporterSprite;
    public Sprite keySprite;
    public Sprite noneSprite;

    void Awake()
    {
        flashlightHold = this.transform.GetChild(3).GetChild(0).gameObject;
        umbrellaHold = this.transform.GetChild(3).GetChild(1).gameObject;
        hammerHold = this.transform.GetChild(3).GetChild(2).gameObject;
        teleporterHold = this.transform.GetChild(3).GetChild(3).gameObject;
        keyHold = this.transform.GetChild(3).GetChild(4).gameObject;

        // get playerscript to change umbrella float and get control enable state. 
        playerScript = gameObject.GetComponent<PlayerScript>();

    }

    public override void FixedUpdateNetwork()
    {
        if (!playerScript.controlsEnabled) return;

        // get the input from the network.
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFirstItemButtonPressed)
            {
                itemSelect = 0;

                slot1Text.fontStyle = FontStyles.Bold | FontStyles.Italic;
                slot2Text.fontStyle = FontStyles.Normal;

                HoldItem(inventory[itemSelect]);
            }

            if (networkInputData.isSecondItemButtonPressed)
            {
                itemSelect = 1;

                slot2Text.fontStyle = FontStyles.Bold | FontStyles.Italic;
                slot1Text.fontStyle = FontStyles.Normal;

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

        if (pickupCooldownTimer.ExpiredOrNotRunning(Runner))
        {
            onPickupCooldown = false;
            pickupCooldownTimer = TickTimer.None;
        }
    }

    void UseItem(string itemName)
    {
        if (!Object.HasStateAuthority) return;

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
            return; 
        }

        if (itemName == "Teleporter")
        {
            if (teleporterHold.activeInHierarchy == false) return;

            teleporterHold.GetComponent<Teleporter_USE>().useTeleporter(this.gameObject, transform.position);
            return; 
        }

        if (itemName == "Key")
        {
            if (keyHold.activeInHierarchy == false) return;
            // if the player uses the key and there is an object assigned to lockedKeyDoor and if lockedKeyDoor is awaiting for a key to be used. 
            if (keyHold.GetComponent<Key_USE>().useKey() && (lockedKeyDoor ?? false) && lockedKeyDoor.GetComponent<LockedKeyDoor>().awaitForKey) 
            {
                lockedKeyDoor.GetComponent<LockedKeyDoor>().doorStatus = !lockedKeyDoor.GetComponent<LockedKeyDoor>().doorStatus;
                lockedKeyDoor = null;

                slotToAdd = itemSelect;
                itemToAdd = "";
                toggleInventoryAddition = !toggleInventoryAddition;
                networkedInventory = "";

                // remove UI text?
            };
            return;
        }
    }

    void ThrowItem(string itemName, int index, Vector3 aimForwardVector)
    {
        //inventory[index] = "";
        //networkedInventory = "";
        //onPickupCooldown = true;
        //checkDuplicate(itemName);
        if (!Object.HasStateAuthority) return;

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
                Runner.Spawn(Hammer, transform.position + transform.forward, Quaternion.LookRotation(aimForwardVector), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Hammer_USE>().Init(throwSpeed);
                });
            }

            if (itemName == "Teleporter")
            {
                if (teleporterHold.activeInHierarchy == false) return;

                Runner.Spawn(Teleporter, transform.position + transform.forward, Quaternion.LookRotation(aimForwardVector), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Teleporter_USE>().Init(throwSpeed);
                });
            }

            if (itemName == "Key")
            {
                Runner.Spawn(Key, transform.position + transform.forward, Quaternion.LookRotation(aimForwardVector), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Key_USE>().Init(throwSpeed);
                });
            }

            //inventory[index] = "";

            slotToAdd = index;
            itemToAdd = "";
            toggleInventoryAddition = !toggleInventoryAddition;
            networkedInventory = "";
            //setInventoryUI(index, "");

            // when a player throws an item, they can't pick up their own item immediately or hold. 
            //StartCoroutine(pickupCooldown(0.2f));
            onPickupCooldown = true;
            pickupCooldownTimer = TickTimer.CreateFromSeconds(Runner, 0.2f);
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
            //inventory[0] = itemName;
            //setInventoryUI(0, itemName);
            slotToAdd = 0;
            itemToAdd = itemName;
            toggleInventoryAddition = !toggleInventoryAddition;

            //networkedInventory = inventory[0];
            return false;
        }
        // if slot 2 is empty, put the item in slot 2.
        else if (inventory[1] == "")
        {
            //inventory[1] = itemName;
            //setInventoryUI(1, itemName);

            slotToAdd = 1;
            itemToAdd = itemName;
            toggleInventoryAddition = !toggleInventoryAddition;

            //checkDuplicate(itemName);
            
            //networkedInventory = inventory[1];
            return false;
        }

        return true;
    }

    public bool ForceAddItem(string itemName, int addingToSlot)
    {
        // force item into slot 1. 
        if (addingToSlot == 0)
        {
            slotToAdd = 0;
            itemToAdd = itemName;
            toggleInventoryAddition = !toggleInventoryAddition;
        }
        // force item into slot 2.
        else if (addingToSlot == 1)
        {
            slotToAdd = 1;
            itemToAdd = itemName;
            toggleInventoryAddition = !toggleInventoryAddition;
        }

        return true;
    }

    static void OnSlotAddChange(Changed<PlayerInventory> changed)
    {
        changed.Behaviour.inventory[changed.Behaviour.slotToAdd] = changed.Behaviour.itemToAdd;
        changed.Behaviour.setInventoryUI(changed.Behaviour.slotToAdd, changed.Behaviour.itemToAdd);
    }

    void setInventoryUI(int index, string itemName)
    {
        if (index == 0)
        {
            if (itemName == "Flashlight")
            {
                inventorySlotImg1.GetComponent<Image>().sprite = flashlightSprite;
            } 
            else if (itemName == "Umbrella")
            {
                inventorySlotImg1.GetComponent<Image>().sprite = umbrellaSprite;
            } 
            else if (itemName == "Hammer")
            {
                inventorySlotImg1.GetComponent<Image>().sprite = hammerSprite;
            } 
            else if (itemName == "Teleporter")
            {
                inventorySlotImg1.GetComponent<Image>().sprite = teleporterSprite;
            }
            else if (itemName == "Key")
            {
                inventorySlotImg1.GetComponent<Image>().sprite = keySprite;
            }
            else if (itemName == "")
            {
                inventorySlotImg1.GetComponent<Image>().sprite = noneSprite;
                slot1Text.fontStyle = FontStyles.Normal;
            }
        } 
        else if (index == 1)
        {
            if (itemName == "Flashlight")
            {
                inventorySlotImg2.GetComponent<Image>().sprite = flashlightSprite;
            }
            else if (itemName == "Umbrella")
            {
                inventorySlotImg2.GetComponent<Image>().sprite = umbrellaSprite;
            }
            else if (itemName == "Hammer")
            {
                inventorySlotImg2.GetComponent<Image>().sprite = hammerSprite;
            }
            else if (itemName == "Teleporter")
            {
                inventorySlotImg2.GetComponent<Image>().sprite = teleporterSprite;
            }
            else if (itemName == "Key")
            {
                inventorySlotImg2.GetComponent<Image>().sprite = keySprite;
            }
            else if (itemName == "")
            {
                inventorySlotImg2.GetComponent<Image>().sprite = noneSprite;
                slot2Text.fontStyle = FontStyles.Normal;
            }
        }
    }

    public void StopHolding()
    {
        flashlightHold.SetActive(false);
        umbrellaHold.SetActive(false);
        hammerHold.SetActive(false);
        teleporterHold.SetActive(false);
        keyHold.SetActive(false);

        // reset umbrella float since player isn't holding anything. 
        playerScript.umbrellaFloat = 1;
    }

    // local call.
    void HoldItem(string itemName)
    {
        if (itemName == "") 
        {
            networkedInventory = "";
            slot1Text.fontStyle = FontStyles.Normal;
            slot2Text.fontStyle = FontStyles.Normal;
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

        if (itemName == "Key")
        {
            networkedInventory = "Key";
        }
    }

    // networked call.
    static void OnHoldChanged(Changed<PlayerInventory> changed)
    {
        if (changed.Behaviour.networkedInventory == "")
        {
            changed.Behaviour.StopHolding();
            //changed.Behaviour.setInventoryUI(changed.Behaviour.itemSelect, changed.Behaviour.networkedInventory);
            //changed.Behaviour.slot1Text.fontStyle = FontStyles.Normal;
            //changed.Behaviour.slot2Text.fontStyle = FontStyles.Normal;
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

        if (changed.Behaviour.networkedInventory == "Hammer")
        {
            changed.Behaviour.StopHolding();
            changed.Behaviour.hammerHold.SetActive(true);
        }

        if (changed.Behaviour.networkedInventory == "Teleporter")
        {
            changed.Behaviour.StopHolding();
            changed.Behaviour.teleporterHold.GetComponent<Teleporter_USE>().networkStatusObj = GameObject.FindGameObjectWithTag("Portal");

            // (this fixes a specific scenario)
            // run this twice because it corrects the color the client sees the host holding
            // when the client first sets a portal down
            // and gives the teleporter to the host
            // without the host having set down a portal first.
            changed.Behaviour.teleporterHold.GetComponent<Teleporter_USE>().toggleSearch = !changed.Behaviour.teleporterHold.GetComponent<Teleporter_USE>().toggleSearch;
            changed.Behaviour.teleporterHold.GetComponent<Teleporter_USE>().toggleSearch = !changed.Behaviour.teleporterHold.GetComponent<Teleporter_USE>().toggleSearch;

            changed.Behaviour.teleporterHold.SetActive(true);
        }

        if (changed.Behaviour.networkedInventory == "Key")
        {
            changed.Behaviour.StopHolding();
            changed.Behaviour.keyHold.SetActive(true);
        }
    }

    //IEnumerator pickupCooldown(float timer)
    //{
    //    onPickupCooldown = true;
    //    yield return new WaitForSeconds(timer);
    //    onPickupCooldown = false;
    //}
}
