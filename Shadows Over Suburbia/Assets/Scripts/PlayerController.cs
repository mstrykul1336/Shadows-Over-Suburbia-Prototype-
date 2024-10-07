using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    public Player photonPlayer;
    public GameManager.Role playerRole { get; private set; }
    public GameManager.Alignment playerAlignment { get; private set; }
    public bool dead;

    public int maxHearts = 2;  
    public int currentHearts;

    private PlayerHealthUI healthUI;
    public Button attackButton;         
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

    // Start is called before the first frame update
    void Start()
    {
       // PhotonNetwork.LocalPlayer.TagObject = this.gameObject; // Where this script is attached to the player GameObject
        // SetHealthByRole();
        // healthUI = GetComponentInChildren<PlayerHealthUI>();
        // if (healthUI != null)
        // {
        //     healthUI.UpdateHealthUI();
        // }
        //currentHearts = maxHearts;
        attackButton.onClick.AddListener(HandleAttack);
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
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;
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


     // This method allows RoleAssigner to set the player's role and alignment
    
    public void SetRoleAndAlignment(GameManager.Role role, GameManager.Alignment alignment)
    {
        // if(!photonView.IsMine)
        // {
        //     return;
        // }
        playerRole = role;
        playerAlignment = alignment;

        playerAbility = GetAbilityForRole(role, alignment);
        // Handle any other logic based on role and alignment if necessary
        Debug.Log($"Player Role: {playerRole}, Alignment: {playerAlignment}");
        Debug.Log($"Ability: {playerAbility}");
        Debug.Log("WORKING");
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
        Debug.Log("Night UI activated for: " + photonView.Owner.NickName);
        nightcanvas.SetActive(true);
        attackButton.gameObject.SetActive(true);
        playerDropdown.gameObject.SetActive(true);
        PopulatePlayerDropdown();
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
            localinventoryUIInstance.transform.SetParent(GameObject.Find("Canvas").transform, false); // Assuming the canvas for the UI is called "Canvas"
            localinventoryUIInstance.SetActive(false); // Make sure it's hidden initially
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        localinventoryUIInstance.SetActive(isInventoryOpen); // Toggle visibility
        UpdateInventoryUI(); // Update the inventory when toggling
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
                photonView.RPC("Attack", RpcTarget.All, player.photonPlayer);
                hasAttacked = true;
                attackButton.gameObject.SetActive(false);
                playerDropdown.gameObject.SetActive(false);
                break;
            }
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


    void HandlePlayerDeath()
    {
        // Handle what happens when a player dies
        dead = true;
        Debug.Log($"{photonView.Owner.NickName} has died!");
        GameManager.instance.NotifyPlayerDeath(photonView.Owner.ActorNumber);
    }

    // [PunRPC]
    // void ReceiveAttack(int damageAmount)
    // {
    //     Debug.Log("Recieving attack");
    //     // This RPC should be called on the target player's controller
    //     TakeDamage(damageAmount);
    // }

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
            TextMeshProUGUI inventoryText = localinventoryUIInstance.GetComponentInChildren<TextMeshProUGUI>();
            inventoryText.text = "Items in Inventory:\n";

            foreach (string item in playerInventory)
            {
                inventoryText.text += item + "\n";
            }
        }
    }


    public enum Ability
    {
        None,
        MayorBasement,
        DesMayorBasement,
        Heal,
        FakeHeal,
        PoisonedBrownies,
        Interrogate
        // will be adding more later as I come up with them
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
                return alignment == GameManager.Alignment.Helpful ? Ability.Interrogate : Ability.Interrogate;
            case GameManager.Role.Medic:
                return alignment == GameManager.Alignment.Helpful ? Ability.Heal : Ability.FakeHeal;
            // Define abilities for other roles
            default:
                return Ability.None;
        }
    }
    
}
