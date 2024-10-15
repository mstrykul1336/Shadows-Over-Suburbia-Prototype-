using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    public Player photonPlayer;
    public GameManager.Role playerRole { get; private set; }
    public GameManager.Alignment playerAlignment { get; private set; }
    public bool dead;

    public float maxHearts = 2;  
    public float currentHearts;

    private PlayerHealthUI healthUI;
    public Button poisonButton;         
    public TMP_Dropdown playerDropdown;
    public GameObject nightcanvas;
    public bool hasAttacked; 

    public int gold;
    public TextMeshProUGUI goldText;

    public List<string> playerInventory = new List<string>(); 
    public GameObject inventoryCanvas; 
    private GameObject localinventoryUIInstance;
    private bool isInventoryOpen = false;
    public Ability playerAbility { get; private set; }
    private TMP_Dropdown itemDropdown;
    private Button UseItemButton;
    //public TMP_Dropdown attackDropdown; 
    public GameObject attackDropdownUI; 
    //public Button attackButtonknife;
    //ABILITIES
    public Button healButton;
    public GameObject mayorspawn;
    public GameObject mayorbasementspawn;
    public Button takeToBasementButton;
    //public Button poisonButton;  
    private GameManager gameManager;

    //ITEMS
    public enum ItemType
    {
        Potion,
        Knife,
        Shield
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        healButton.gameObject.SetActive(false);
        takeToBasementButton.gameObject.SetActive(false);
        poisonButton.gameObject.SetActive(false);
       // PhotonNetwork.LocalPlayer.TagObject = this.gameObject; // Where this script is attached to the player GameObject
        // SetHealthByRole();
        // healthUI = GetComponentInChildren<PlayerHealthUI>();
        // if (healthUI != null)
        // {
        //     healthUI.UpdateHealthUI();
        // }
        //currentHearts = maxHearts;
        poisonButton.onClick.AddListener(HandleAttack);
        healButton.onClick.AddListener(HandleHeal);
        takeToBasementButton.onClick.AddListener(TakeToBasement);
        gameManager = FindObjectOfType<GameManager>();
        healthUI = GetComponentInChildren<PlayerHealthUI>();
        // SetHealthByRole();
        currentHearts = maxHearts;
        // if (healthUI != null)
        // {
        //     healthUI.UpdateHealthUI();
        // }
        hasAttacked = false; 
        gold = 1;
        goldText.text = gold.ToString(); 
        CreateInventoryCanvas();

        Button useItemButton = localinventoryUIInstance.transform.Find("UseItemButton").GetComponent<Button>();
        TMP_Dropdown itemDropdown = localinventoryUIInstance.transform.Find("ItemDropdown").GetComponent<TMP_Dropdown>();

        useItemButton.onClick.AddListener(UseSelectedItem);
    }


    public void PopulatePlayerDropdown()
    {
        playerDropdown.ClearOptions();  // Clear any existing options
        Debug.Log("PopulatePlayerDropdown called!");
        var options = new List<string>();

        foreach (PlayerController player in GameManager.instance.players)
        {
            // Exclude the local player and dead players from the dropdown
            if (player != null && !player.photonView.IsMine && !player.dead)
            {
                options.Add(player.photonPlayer.NickName); 
                Debug.Log("Added player: " + player.photonPlayer.NickName);// Add player name to options
            }
        }

        playerDropdown.AddOptions(options);
    }

    public void PopulateLocalPlayerDropdown()
    {
        playerDropdown.ClearOptions();  // Clear any existing options
        Debug.Log("PopulatePlayerDropdown called!");
        var options = new List<string>();

        foreach (PlayerController player in GameManager.instance.players)
        {
            // Exclude the local player and dead players from the dropdown
            if (player != null && !player.dead)
            {
                options.Add(player.photonPlayer.NickName); 
                Debug.Log("Added player: " + player.photonPlayer.NickName);// Add player name to options
            }
        }

        playerDropdown.AddOptions(options);
    }



     // This method allows RoleAssigner to set the player's role and alignment
    
    public void SetRoleAndAlignment(GameManager.Role role, GameManager.Alignment alignment)
    {
        if(!photonView.IsMine)
        {
            return;
        }
        playerRole = role;
        playerAlignment = alignment;

        playerAbility = GetAbilityForRole(role, alignment);
        // Handle any other logic based on role and alignment if necessary
        Debug.Log($"Player Role: {playerRole}, Alignment: {playerAlignment}");
        Debug.Log($"Ability: {playerAbility}");
        Debug.Log("WORKING");

         if (role == GameManager.Role.OldMan)
        {
            GameManager.instance.oldManPlayer = this;
            Debug.Log("Old Man assigned to: " + photonPlayer.NickName);
        }
        SetHealthByRole();
        currentHearts = maxHearts;
        if (healthUI != null)
        {
            healthUI.UpdateHealthUI();
        }
        
    }


    public string GetRole()
    {
        return playerRole.ToString();
    }



    public void SetHealthByRole()
    {
        
        // if(!photonView.IsMine)
        // {
        //     return;
        // }
        string role = GetRole();  
        Debug.Log("Setting health for role: " + role); 
        switch (role)
        {
            case "Mayor":
                maxHearts = 4;
                Debug.Log("Setting for Mayor");
                break;
            case "Baker":
                maxHearts = 3;
                break;
            case "Villager":
            case "Assistant":
            case "Medic":
            case "Detective":
            case "Clairvoyant":
            case "OldMan":
                maxHearts = 2;
                break;
            default:
                maxHearts = 2; // Default value for unknown roles
                break;
        }
        
    
        Debug.Log("Max hearts set to: " + maxHearts);
         
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        
    }
   
    public void NightUI()
    {
        if(!photonView.IsMine)
        {
            return;
        }
        Debug.Log("Night UI activated for: " + photonView.Owner.NickName + " with role: " + playerRole);
        nightcanvas.SetActive(true);
        //attackButton.gameObject.SetActive(true);
        ResetButtonStates();
        if (playerRole == GameManager.Role.Medic)  // Check if the player's role is Medic
        {
            healButton.gameObject.SetActive(true); 
            playerDropdown.gameObject.SetActive(true); // Activate the heal button only for the Medic
            PopulateLocalPlayerDropdown();  // Populate the dropdown with valid players
        }
        else
        {
            healButton.gameObject.SetActive(false); 
        }

        if (playerRole == GameManager.Role.Mayor) 
        {
            takeToBasementButton.gameObject.SetActive(true); 
            playerDropdown.gameObject.SetActive(true); // Show the take to basement button   
            
        }
        else
        {
            takeToBasementButton.gameObject.SetActive(false);
            playerDropdown.gameObject.SetActive(true); // Hide the button for other roles
        }
        if (playerRole == GameManager.Role.Baker)  // Change this to your desired role
        {
            poisonButton.gameObject.SetActive(true);
            playerDropdown.gameObject.SetActive(true);  // Show the take to basement button   
            
        }
        else
        {
            poisonButton.gameObject.SetActive(false);
        }
      
        PopulatePlayerDropdown();
    }

    private void ResetButtonStates()
    {
        // Deactivate all buttons before setting the new state
        healButton.gameObject.SetActive(false);
        takeToBasementButton.gameObject.SetActive(false);
        poisonButton.gameObject.SetActive(false);
    }

    public void NightUIOff()
    {
        nightcanvas.SetActive(false);
        hasAttacked = false; 
    }

    private void CreateInventoryCanvas()
    {
        if (localinventoryUIInstance == null)
        {
            localinventoryUIInstance = Instantiate(inventoryCanvas, Vector3.zero, Quaternion.identity);
            localinventoryUIInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
            itemDropdown = localinventoryUIInstance.transform.Find("ItemDropdown").GetComponent<TMP_Dropdown>();
            localinventoryUIInstance.SetActive(false);
            attackDropdownUI = localinventoryUIInstance.transform.Find("attackDropdownUI").gameObject;
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        localinventoryUIInstance.SetActive(isInventoryOpen); // Toggle visibility
        UpdateInventoryUI(); // Update the inventory when toggling
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (isInventoryOpen)
        {
            if (localPlayer != null )
            {
                localPlayer.DisablePlayerControls();
            }
        }
        else
        {
            if (localPlayer != null)
            {
                localPlayer.EnablePlayerControls(); 
            }
        }
    }
   

    // public void ToDieDebug()
    // {
    //     TakeDamage(4);
    // }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (dead)
            return;

        currentHearts -= damage;

        if (healthUI != null)
        {
            healthUI.UpdateHealthUI();
        }

        if (currentHearts <= 0)
        {
            currentHearts = 0;
            HandlePlayerDeath();
        }
        
    }

    [PunRPC]
    public void TakePoisonDamage(int damageinQuarters)
    {
        if (dead)
            return;
        Debug.Log($"Poison damage taken by {photonPlayer.NickName}");
        currentHearts -= ((float)damageinQuarters / 4);

        if (healthUI != null)
        {
            healthUI.UpdateHealthUI();
        }

        if (currentHearts <= 0)
        {
            currentHearts = 0;
            HandlePlayerDeath();
        }
        
    }
    [PunRPC]
    public void GiveGold()
    {
        gold += 1;
        UpdateGold();
    }

    public void UpdateGold()
    {
        goldText.text = gold.ToString();
    }

    private void HandleAttack()
    {
        if (playerDropdown.options.Count == 0 || playerDropdown.value < 0)
        {
            Debug.Log("No valid player selected to attack.");
            return;  // No valid players to attack
        }
        string selectedPlayerName = playerDropdown.options[playerDropdown.value].text;
        if (hasAttacked)
        {
            Debug.Log("You can only attack once");
            return;
        }
        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer.NickName == selectedPlayerName)
            {
                // Call attack on the selected player
                hasAttacked = true;
                poisonButton.gameObject.SetActive(false);
                photonView.RPC("Poison", RpcTarget.All, player.photonPlayer);
                playerDropdown.gameObject.SetActive(false);
                break;
            }
        }
    }
    private void HandleHeal()
    {
        if (playerDropdown.options.Count == 0 || playerDropdown.value < 0)
        {
            Debug.Log("No valid player selected to heal.");
            return;  // No valid players to heal
        }

        string selectedPlayerName = playerDropdown.options[playerDropdown.value].text;
        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer.NickName == selectedPlayerName)
            {
                // Call heal on the selected player
                Debug.Log($"Healing {selectedPlayerName}");
                photonView.RPC("Heal", RpcTarget.All, player.photonPlayer);
                break;
            }
        }
    }


    // -=-=---=-=-===--=-
    // Abilities here:
    // -=-=---=-=-===--=-

    public enum Ability
    {
        None,
        MayorBasement,
        DesMayorBasement,
        Heal,
        FakeHeal,
        PoisonedBrownies,
        Interrogate,
        UnlawfulInterrogate,
        Psychic,
        Connections,
        Brainwash,
        VoterFraud
        
    }

    private Ability GetAbilityForRole(GameManager.Role role, GameManager.Alignment alignment)
    {
        switch (role)
        {
            case GameManager.Role.Mayor:
                return alignment == GameManager.Alignment.Helpful ? Ability.MayorBasement : Ability.DesMayorBasement;
            case GameManager.Role.Baker:
                return alignment == GameManager.Alignment.Helpful ? Ability.PoisonedBrownies : Ability.PoisonedBrownies;
            case GameManager.Role.Detective:
                return alignment == GameManager.Alignment.Helpful ? Ability.Interrogate : Ability.UnlawfulInterrogate;
            case GameManager.Role.Medic:
                return alignment == GameManager.Alignment.Helpful ? Ability.Heal : Ability.FakeHeal;
            case GameManager.Role.Clairvoyant:
                return alignment == GameManager.Alignment.Helpful ? Ability.Psychic : Ability.Psychic;
            case GameManager.Role.Assistant:
                return alignment == GameManager.Alignment.Helpful ? Ability.Connections : Ability.Brainwash;
            case GameManager.Role.OldMan:
                return alignment == GameManager.Alignment.Helpful ? Ability.VoterFraud : Ability.VoterFraud;
            // Define abilities for other roles
            default:
                return Ability.None;
        }
    }

    [PunRPC]
    public void Attack(Player photonPlayer)
    {
        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer == photonPlayer)
            {
                Debug.Log($"Attacking {player.photonPlayer.NickName}");
                player.photonView.RPC("TakeDamage", RpcTarget.All, 1); // 1 damage, adjust as necessary
                break;
            }
        }
    }

    [PunRPC]
    public void Poison(Player photonPlayer)
    {
        if (photonPlayer == photonView.Owner) // Ensure this is the correct player
        {
            Debug.Log($"Poisoning {photonPlayer.NickName}");
            GameManager.instance.poisonedPlayers.Add(new PoisonEffect(this, 3)); // Add poison for 3 nights to the current player
            GameManager.instance.ApplyPoisonEffects();
        }
    }

    [PunRPC]
    public void Heal(Player photonPlayer)
    {
        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer == photonPlayer)
            {
                Debug.Log($"Healing {player.photonPlayer.NickName}");
                
                // Heal logic: Restore 1 heart, but not above maxHearts
                player.currentHearts = Mathf.Min(player.currentHearts + 1, player.maxHearts);
                playerDropdown.gameObject.SetActive(false);
                healButton.gameObject.SetActive(false);

                if (player.healthUI != null)
                {
                    player.healthUI.UpdateHealthUI();
                }
                break;
            }
        }
    }

    void HandlePlayerDeath()
    {
        // Handle what happens when a player dies
        dead = true;
        Debug.Log($"{photonView.Owner.NickName} has died!");
        GameManager.instance.NotifyPlayerDeath(photonView.Owner.ActorNumber);

        if (playerRole == GameManager.Role.OldMan)
        {
            Debug.Log("Old Man has been killed!");
            GameManager.instance.OldManDied(); // Call GameManager to handle Old Man's death
        }
    }

    private void TakeToBasement()
    {
        if (playerDropdown.value < 0 || playerDropdown.options.Count == 0)
        {
            Debug.Log("No valid player selected to take to the basement.");
            return;
        }

        string selectedPlayerName = playerDropdown.options[playerDropdown.value].text;

        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer.NickName == selectedPlayerName)
            {
                // Call the RPC to take this player to the basement
                photonView.RPC("TeleportToBasement", RpcTarget.All, player.photonPlayer);
                break;
            }
        }
    }

    [PunRPC]
    public void TeleportToBasement(Player targetPlayer)
    {
        PlayerController targetController = GameManager.instance.players.FirstOrDefault(pc => pc.photonPlayer == targetPlayer);

        if (targetController != null)
        {
            // Disable the night UI for the target player
            targetController.NightUIOff();

            // Teleport both players
            transform.position = gameManager.mayorspawn.transform.position;  // Move this player to their basement
            targetController.transform.position = gameManager.mayorbasementspawn.transform.position;  // Move the target to a random spawn point
            takeToBasementButton.gameObject.SetActive(false);
            playerDropdown.gameObject.SetActive(false);
        }
    }

    // [PunRPC]
    // void ReceiveAttack(int damageAmount)
    // {
    //     Debug.Log("Recieving attack");
    //     // This RPC should be called on the target player's controller
    //     TakeDamage(damageAmount);
    // }

     // -=-=---=-=-===--=-
     // Inventory:
    // -=-=---=-=-===--=-
    public void AddItemToInventory(string itemName)
    {
        playerInventory.Add(itemName);
        Debug.Log($"{itemName} added to inventory");
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        if (localinventoryUIInstance != null)
        {
            attackDropdownUI.SetActive(false);
            TextMeshProUGUI inventoryText = localinventoryUIInstance.GetComponentInChildren<TextMeshProUGUI>();
            inventoryText.text = "Items in Inventory:\n";
            TMP_Dropdown itemDropdown = localinventoryUIInstance.transform.Find("ItemDropdown").GetComponent<TMP_Dropdown>();
            itemDropdown.ClearOptions();

            // foreach (string item in playerInventory)
            // {
            //     inventoryText.text += item + "\n";
            // }
            if (playerInventory.Count > 0)
            {
                itemDropdown.AddOptions(playerInventory);
            }
            else
            {
                Debug.Log("Inventory is empty.");
            }

            if (HasItemInInventory("Knife")) // Implement this method to check for the knife
            {
                attackDropdownUI.SetActive(true); 
                PopulateAttackDropdown(); // Populate the dropdown with players to attack
            }
        }
    }

    private bool HasItemInInventory(string itemName )
    {
        return playerInventory.Contains(itemName);
    }

    [PunRPC]
    public void UseItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Potion:
                UsePotion();
                break;
            case ItemType.Knife:
                UseKnife();
                break;
            case ItemType.Shield:
                UseShield();
                break;
            default:
                Debug.Log("Unknown item type.");
                break;
        }
    }

    private void UsePotion()
    {
        if (currentHearts < maxHearts) 
        {
            currentHearts += 1; 
            if (currentHearts > maxHearts)
                currentHearts = maxHearts; 

            if (healthUI != null)
            {
                healthUI.UpdateHealthUI();
            }
            Debug.Log($"{photonPlayer.NickName} used a Potion. Current Hearts: {currentHearts}");
        }
        else
        {
            Debug.Log($"{photonPlayer.NickName} tried to use a Potion, but health is full.");
        }
    }

    private void UseShield()
    {
       if (!IsNight())
        {
            Debug.Log("Shield can only be used at night!");
            return;
        }

        // Implement shield logic here, e.g., mark player as protected for the night
        Debug.Log("Shield used! You are protected for the night.");
    }

    private void UseKnife()
    {
        if (!IsNight())
        {
            Debug.Log("Knife can only be used at night!");
            return;
        }

        // Show the attack dropdown and populate it
        attackDropdownUI.SetActive(true);
        PopulateAttackDropdown(); 
    }

    public void UseSelectedItem()
    {
        if (itemDropdown.value < 0)
        {
            Debug.Log("No item selected.");
            return; // No valid selection
        }

        string selectedItem = itemDropdown.options[itemDropdown.value].text; // Get selected item name
        ItemType itemType;

        // Check if the selected item is valid
        if (Enum.TryParse(selectedItem, out itemType)) // Try to convert string to ItemType
        {
            photonView.RPC("UseItem", RpcTarget.All, itemType); // Call the RPC to use the item across the network
        }
        else
        {
            Debug.Log($"Invalid item selected: {selectedItem}");
        }
    }

    private bool IsNight()
    {
        // Implement your logic to check if it's night
        return GameManager.instance.isNightCycle; // Assuming you have an isNightCycle variable in your GameManager
    }
    private void PopulateAttackDropdown()
    {
        if (localinventoryUIInstance != null)
        {
        // Find the attackDropdown component in the instantiated inventory UI
        TMP_Dropdown attackDropdown = localinventoryUIInstance.transform.Find("attackDropdown").GetComponent<TMP_Dropdown>();
            if (attackDropdown != null) // Check if the dropdown was found
            {
                attackDropdown.ClearOptions(); // Clear existing options
                var options = new List<string>();

                // Populate the dropdown with players that can be attacked
                foreach (var player in GameManager.instance.players)
                {
                    if (player != null && player.photonView.IsMine == false && !player.dead)
                    {
                        options.Add(player.photonPlayer.NickName); // Add player's nickname
                    }
                }

                attackDropdown.AddOptions(options); // Add the options to the dropdown
            }
        }
    }

    public void ExecuteAttack()
    {
        TMP_Dropdown attackDropdown = localinventoryUIInstance.transform.Find("attackDropdown").GetComponent<TMP_Dropdown>();
        if (attackDropdown.value < 0 || attackDropdown.options.Count == 0)
        {
            Debug.Log("No valid player selected to attack.");
            return;
        }

        string selectedPlayerName = attackDropdown.options[attackDropdown.value].text;

        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer.NickName == selectedPlayerName)
            {
                // Call a method to deal damage to the selected player
                player.photonView.RPC("TakeDamage", RpcTarget.All, 1); // Assuming knife deals 1 damage
                break;
            }
        }
        attackDropdownUI.SetActive(false);
    }


    
}
