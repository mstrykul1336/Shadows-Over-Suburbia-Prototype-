using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    public Player photonPlayer;
    public GameManager.Role playerRole { get; private set; }
    public GameManager.Alignment playerAlignment { get; private set; }
    public bool dead = false;
    public bool IsOldMan { get; set; }

    public float maxHearts = 2;  
    public float currentHearts;

    private PlayerHealthUI healthUI;
    public Button poisonButton;         
    public TMP_Dropdown playerDropdown;
    public GameObject nightcanvas;
    public bool hasAttacked; 
    public Button vilattackButton;   
    public Button Investigatebutton;   

    public int gold;
    public TextMeshProUGUI goldText;

    public List<string> playerInventory = new List<string>(); 
    public GameObject inventoryCanvas; 
    private GameObject localinventoryUIInstance;
    private bool isInventoryOpen = false;
    public Ability playerAbility { get; private set; }
    private TMP_Dropdown itemDropdown;
    private Button UseItemButton;
    public Button playerAttackButton;
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
    public FirstPersonController playerControls;
    public List<string> nightActions = new List<string>();

    public Button Psychicsbutton;
    public TMP_Dropdown playerDropdown1;
    public TMP_Dropdown playerDropdown2;
    public TMP_Dropdown attackplayerDropdown;
    public TextMeshProUGUI investigateText;
    private PlayerController playerToInvestigate = null;

    public Button ConnectionsButton;
    public TextMeshProUGUI psychicText;
    public TextMeshProUGUI connectionsText;
    public float investigationDelay = 15f;
    public bool isProtected = false;
    public TextMeshProUGUI attackMessageText;
    public TextMeshProUGUI healMessageText;
    //public ChatManager ChatManager;

    public TMP_InputField journalInput; 
    private string journalContent = "";
    public GameObject journalUI;
    public TextMeshProUGUI journalDisplay;
    public Button toggleJournalButton;
    //public Button toggleSaveButton;
    public bool isJournalOpen = false;
    public GameObject ghostModel;
    public GameObject roleAbilityCanvas; 
    private bool isAbilityCanvasActive = false;
    public Button SettingsButton;
    public GameObject SettingsCanvas;
    private bool isSettingsCanvasActive = false;
    public bool canChat = true;

    public Transform[] spectatorSpawnPoints;
    public MeshRenderer capsuleMeshRenderer;
    public GameObject ghostVision;
    public GameObject chatPrefab; 
    private GameObject chatInstance; 
    private ChatManager ChatManager;
  

    public GameObject goblin;
    public GameObject elf;
    public GameObject villager1;
    public GameObject villager2;
    public GameObject citizen;
    public GameObject witch;
    public Image profilePicture;
    public Sprite mayorImage;
    public Sprite villagerSprite;
    public Sprite assistantSprite;
    public Sprite ClairvoyantSprite;
    public Sprite oldManSprite;
    public Sprite detectiveSprite;
    public Sprite MedicSprite;
    public Sprite bakerSprite;
    private Sprite currentProfilePicture;

    public AudioMixer audioMixer; 
    public Slider musicSlider;    
    public Slider sfxSlider;


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
        Debug.Log($"Initialized PlayerController for {photonPlayer.NickName} with ID: {id}");
    }

    // Start is called before the first frame update
    public void Start()
    {
        healButton.gameObject.SetActive(false);
        takeToBasementButton.gameObject.SetActive(false);
        poisonButton.gameObject.SetActive(false);
        vilattackButton.gameObject.SetActive(false);
        Investigatebutton.gameObject.SetActive(false);
        Psychicsbutton.gameObject.SetActive(false);
        playerDropdown1.gameObject.SetActive(false);
        playerDropdown2.gameObject.SetActive(false);
        ConnectionsButton.gameObject.SetActive(false);
        journalUI.SetActive(false);
        chatInstance = Instantiate(chatPrefab, transform);
        Debug.Log("Chat prefab instantiated for player: " + photonView.Owner.NickName); // Instantiate as a child of the player object
        if (!photonView.IsMine)
        {
            chatInstance.SetActive(false); // Disable chat for non-local players
            Debug.Log("Chat disabled for non-local player: " + photonView.Owner.NickName);
        }
        else
        {
            ChatManager = chatInstance.GetComponent<ChatManager>(); // Get the ChatManager component
            ChatManager.chatPanel.SetActive(true); // Make sure the chat panel is active for the local player
            Debug.Log("Chat enabled for local player: " + photonView.Owner.NickName);
        }
       // PhotonNetwork.LocalPlayer.TagObject = this.gameObject; // Where this script is attached to the player GameObject
        // SetHealthByRole();
        // healthUI = GetComponentInChildren<PlayerHealthUI>();
        // if (healthUI != null)
        // {
        //     healthUI.UpdateHealthUI();
        // }
        //currentHearts = maxHearts;
        poisonButton.onClick.AddListener(HandleAttack);
        playerAttackButton.onClick.AddListener(HandlePlayerAttack);
        vilattackButton.onClick.AddListener(HandleVilAttack);
        healButton.onClick.AddListener(HandleHeal);
        takeToBasementButton.onClick.AddListener(TakeToBasement);
        Investigatebutton.onClick.AddListener(HandleInvestigate);
        Psychicsbutton.onClick.AddListener(HandlePsychic);
        ConnectionsButton.onClick.AddListener(HandleConnections);
        toggleJournalButton.onClick.AddListener(ToggleJournal);
        //toggleSaveButton.onClick.AddListener(SaveJournalEntry);
        SettingsButton.onClick.AddListener(ToggleSettings);
        journalInput.onEndEdit.AddListener(OnJournalInputEndEdit);
        gameManager = FindObjectOfType<GameManager>();
        healthUI = GetComponentInChildren<PlayerHealthUI>();
        playerControls = FindObjectOfType<FirstPersonController>();
        ChatManager = FindObjectOfType<ChatManager>();
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

        float musicVolume, sfxVolume;
        audioMixer.GetFloat("MusicVolume", out musicVolume);
        audioMixer.GetFloat("SFXVolume", out sfxVolume);

        musicSlider.value = Mathf.Pow(10, musicVolume / 20); // Convert from dB to linear
        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20);

        // Add listener to sliders
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        // Convert linear value to decibels and set the mixer parameter
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        // Convert linear value to decibels and set the mixer parameter
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
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

    public void PopulateAttackPlayerDropdown()
    {
        attackplayerDropdown.ClearOptions();  // Clear any existing options
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

        attackplayerDropdown.AddOptions(options);
    }

    public void PopulatePlayerPsychicDropdown()
    {
        playerDropdown1.ClearOptions();  
        playerDropdown2.ClearOptions();  // Clear any existing options
        Debug.Log("PopulatePlayerPsychicDropdown called!");
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

        playerDropdown1.AddOptions(options);
        playerDropdown2.AddOptions(options);
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
    
    [PunRPC]
    public void SetPlayerModel(int index)
    {
        if (capsuleMeshRenderer != null)
        {
            capsuleMeshRenderer.enabled = false;  // Hide the capsule by turning off its MeshRenderer
        }

        if (index == 0)
        {
            goblin.gameObject.SetActive(true);
        }
        else if (index == 1)
        {
            citizen.gameObject.SetActive(true);
        }
        else if (index == 2)
        {
            elf.gameObject.SetActive(true);
        }
        else if (index == 4)
        {
            villager1.gameObject.SetActive(true);
        }
        else if (index == 5)
        {
            villager2.gameObject.SetActive(true);
        }
        else if (index == 3)
        {
            witch.gameObject.SetActive(true);
        }
        else
        {
            villager2.gameObject.SetActive(true);
        }

    }



    

    public void SetRoleAndAlignment(GameManager.Role role, GameManager.Alignment alignment)
    {
        if(!photonView.IsMine)
        {
            return;
        }
        playerRole = role;
        playerAlignment = alignment;
        GameManager.playerRoles[photonView.Owner.ActorNumber] = playerRole;
        photonView.RPC("SetAlignment", RpcTarget.All, alignment);
        photonView.RPC("SetRole", RpcTarget.All, role);
        playerAbility = GetAbilityForRole(role, alignment);
        // Handle any other logic based on role and alignment if necessary
        Debug.Log($"Player Role: {playerRole}, Alignment: {playerAlignment}");
        Debug.Log($"Ability: {playerAbility}");
        Debug.Log("WORKING");
        if (role == GameManager.Role.OldMan)
        {
            GameManager.instance.oldManPlayer = this;
            Debug.Log("Old Man assigned to: " + photonPlayer.NickName);
            IsOldMan = true;

        }
        else 
        {
            IsOldMan = false;
        }
        SetHealthByRole();
        currentHearts = maxHearts;
        if (healthUI != null)
        {
            healthUI.UpdateHealthUI();
        }
        SetProfilePictureByRole();

    }
    [PunRPC]
    public void SetAlignment(GameManager.Alignment alignment)
    {
        this.playerAlignment = alignment; 
    }

    [PunRPC]
    public void SetRole(GameManager.Role role)
    {
        this.playerRole = role; 
    }

    public string GetRole()
    {
        return playerRole.ToString();
    }

    public void SetProfilePictureByRole()
    {
        string role = GetRole(); 
        switch (role)
        {
            case "Mayor":
                profilePicture.sprite = mayorImage;
                currentProfilePicture = mayorImage;
                break;
            case "Baker":
                profilePicture.sprite = bakerSprite;
                currentProfilePicture = bakerSprite;
                break;
            case "Villager":
                profilePicture.sprite = villagerSprite;
                currentProfilePicture = villagerSprite;
                break;
            case "Assistant":
                profilePicture.sprite = assistantSprite;
                currentProfilePicture = assistantSprite;
                break;
            case "Medic":
                profilePicture.sprite = MedicSprite;
                currentProfilePicture = MedicSprite;
                break;
            case "Detective":
                profilePicture.sprite = detectiveSprite;
                currentProfilePicture = detectiveSprite;
                break;
            case "Clairvoyant":
                profilePicture.sprite = ClairvoyantSprite;
                currentProfilePicture = ClairvoyantSprite;
                break;

            case "OldMan":
                profilePicture.sprite = oldManSprite;
                currentProfilePicture = oldManSprite;
                break;
            default:
                profilePicture.sprite = villagerSprite;
                currentProfilePicture = villagerSprite;
                break;
        }

    }

    // public Sprite GetCurrentProfilePicture()
    // {
    //     return currentProfilePicture;
    // }


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

        if (Input.GetKeyDown(KeyCode.I) && !isJournalOpen)
        {
            ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Toggle controls
            if (playerControls.AreControlsEnabled())
            {
                playerControls.DisablePlayerControls();
            }
            else
            {
                playerControls.EnablePlayerControls();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            ToggleAbilityCanvas();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            playerControls.ToggleCameraView();
        }

        
    }

    public void ToggleAbilityCanvas()
    {
        isAbilityCanvasActive = !isAbilityCanvasActive; // Toggle the state

       
        roleAbilityCanvas.SetActive(isAbilityCanvasActive); // Set the canvas active/inactive
    }

    public void ToggleSettings()
    {
        isSettingsCanvasActive= !isSettingsCanvasActive;

        
        SettingsCanvas.SetActive(isSettingsCanvasActive); 
    }
   
    public void NightUI()
    {
        if (dead) return; 
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
            playerAttackButton.gameObject.SetActive(true);
            //playerDropdown.gameObject.SetActive(true);  
            attackplayerDropdown.gameObject.SetActive(true);
            PopulateAttackPlayerDropdown();
        }

        else if (playerRole == GameManager.Role.Mayor) 
        {
            takeToBasementButton.gameObject.SetActive(true); 
            playerDropdown.gameObject.SetActive(true);
            PopulatePlayerDropdown(); 
            playerAttackButton.gameObject.SetActive(true);
            //playerDropdown.gameObject.SetActive(true);  
            attackplayerDropdown.gameObject.SetActive(true);
            PopulateAttackPlayerDropdown();
            
        }
        else if (playerRole == GameManager.Role.Baker)  // Change this to your desired role
        {
            poisonButton.gameObject.SetActive(true);
            playerDropdown.gameObject.SetActive(true); 
            PopulatePlayerDropdown(); 
            playerAttackButton.gameObject.SetActive(true);
            //playerDropdown.gameObject.SetActive(true);  
            attackplayerDropdown.gameObject.SetActive(true);
            PopulateAttackPlayerDropdown();
            
        }
        else if (playerRole == GameManager.Role.Villager)  
        {
            vilattackButton.gameObject.SetActive(true);
            playerDropdown.gameObject.SetActive(true);  
            PopulatePlayerDropdown();
            
        }
        else if (playerRole == GameManager.Role.Detective)  
        {
            Investigatebutton.gameObject.SetActive(true);
            investigateText.gameObject.SetActive(true);
            playerDropdown.gameObject.SetActive(true); 
            investigateText.gameObject.SetActive(true); 
            PopulatePlayerDropdown();
            playerAttackButton.gameObject.SetActive(true);
            //playerDropdown.gameObject.SetActive(true);  
            attackplayerDropdown.gameObject.SetActive(true);
            PopulateAttackPlayerDropdown();
            
        }
        else if (playerRole == GameManager.Role.Clairvoyant)  
        {
            Psychicsbutton.gameObject.SetActive(true);
            psychicText.gameObject.SetActive(true);
            playerDropdown1.gameObject.SetActive(true);  
            playerDropdown2.gameObject.SetActive(true);   
            PopulatePlayerPsychicDropdown();
            playerAttackButton.gameObject.SetActive(true);
            //playerDropdown.gameObject.SetActive(true);  
            attackplayerDropdown.gameObject.SetActive(true);
            PopulateAttackPlayerDropdown();
        }
        else if (playerRole == GameManager.Role.Assistant)  
        {
            ConnectionsButton.gameObject.SetActive(true);
            connectionsText.gameObject.SetActive(true);
            playerDropdown.gameObject.SetActive(true);   
            PopulatePlayerDropdown(); 
            playerAttackButton.gameObject.SetActive(true);
            //playerDropdown.gameObject.SetActive(true);  
            attackplayerDropdown.gameObject.SetActive(true);
            PopulateAttackPlayerDropdown();
        }
        else if (playerRole == GameManager.Role.OldMan) 
        {
            playerAttackButton.gameObject.SetActive(true);
            //playerDropdown.gameObject.SetActive(true);  
            attackplayerDropdown.gameObject.SetActive(true);
            PopulateAttackPlayerDropdown();
        }
        
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
        if (dead) return; 
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
        if (isProtected)
        {
            Debug.Log($"{photonPlayer.NickName} is protected by a shield. No damage taken.");
            ChatManager.DisplayLocalMessage($"{photonPlayer.NickName} is protected by a shield. No damage taken.");
            isProtected = false; // Remove shield protection after blocking one attack
            return;
        }
        Debug.Log($"Poison damage taken by {photonPlayer.NickName}");
        ChatManager.DisplayLocalMessage($"Poison damage taken by {photonPlayer.NickName}");
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
    public void TakeAttackDamage(int damageinQuarters)
    {
        if (dead)
            return;
        if (isProtected)
        {
            Debug.Log($"{photonPlayer.NickName} is protected by a shield. No damage taken.");
            ChatManager.DisplayLocalMessage($"{photonPlayer.NickName} is protected by a shield. No damage taken.");
            isProtected = false; // Remove shield protection after blocking one attack
            return;
        }
        Debug.Log($"Attack damage taken by {photonPlayer.NickName}");
        ChatManager.DisplayLocalMessage($"Attack damage taken by {photonPlayer.NickName}");
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
    [PunRPC]
    public void NotifyAttack(Photon.Realtime.Player player)
    {
        // Update the UI to show that the player was attacked
        if (photonView.Owner == player)
        {
            //Debug.Log($"NotifyAttack called for player: {player.NickName}");
            //UpdateAttackMessage("Attacked!");
            ChatManager.DisplayLocalMessage($"{player.NickName} was attacked!");
        }
    }
    [PunRPC]
    public void NotifyHeal(Photon.Realtime.Player player)
    {
        // Update the UI to show that the player was healed
        if (photonView.Owner == player)
        {
            ChatManager.DisplayLocalMessage($"{player.NickName} was healed!");
        }
    }
    
    // private void UpdateAttackMessage(string message)
    // {
    //     // Assuming you have a TextMeshProUGUI reference for the attack message
    //     if (attackMessageText != null)
    //     {
    //         attackMessageText.text = message;
    //         attackMessageText.gameObject.SetActive(true); // Show the message
    //         StartCoroutine(HideMessageAfterDelay(3f)); 
    //     }
    // }

    // private void UpdateHealMessage(string message)
    // {
    //     // Assuming you have a TextMeshProUGUI reference for the healing message
    //     if (healMessageText != null)
    //     {
    //         healMessageText.text = message;
    //         healMessageText.gameObject.SetActive(true); // Show the message
    //         StartCoroutine(HideMessageAfterDelay(3f)); 
    //     }
    // }

    // private IEnumerator HideMessageAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     attackMessageText.gameObject.SetActive(false); 
    //     healMessageText.gameObject.SetActive(false);
    // }

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
                RecordAction($"Attacked {selectedPlayerName}");
                photonView.RPC("Poison", RpcTarget.All, player.photonPlayer);
                photonView.RPC("NotifyAttack", RpcTarget.All,  player.photonView.Owner);
                playerDropdown.gameObject.SetActive(false);
                break;
            }
        }
    }

    private void HandlePlayerAttack()
    {
        if (attackplayerDropdown.options.Count == 0 || attackplayerDropdown.value < 0)
        {
            Debug.Log("No valid player selected to attack.");
            return;  // No valid players to attack
        }
        string selectedPlayerName = attackplayerDropdown.options[attackplayerDropdown.value].text;
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
                playerAttackButton.gameObject.SetActive(false);
                RecordAction($"Attacked {selectedPlayerName}");
                photonView.RPC("Attack", RpcTarget.All, player.photonPlayer);
                photonView.RPC("NotifyAttack", RpcTarget.All,  player.photonView.Owner);
                attackplayerDropdown.gameObject.SetActive(false);
                break;
            }
        }
    }

    private void HandleVilAttack()
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
        hasAttacked = true;
        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer.NickName == selectedPlayerName)
            {
                // Call attack on the selected player
                vilattackButton.gameObject.SetActive(false);
                RecordAction($"Attacked {selectedPlayerName}");
                photonView.RPC("Attack", RpcTarget.All, player.photonPlayer);
                photonView.RPC("NotifyAttack", RpcTarget.All, player.photonView.Owner);
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
                RecordAction($"Healed {selectedPlayerName}");
                Debug.Log($"Healing {selectedPlayerName}");
                photonView.RPC("NotifyHeal", RpcTarget.All, player.photonView.Owner);
                photonView.RPC("Heal", RpcTarget.All, player.photonPlayer);
                break;
            }
        }
    }

    private void HandleInvestigate()
    {
        if (playerDropdown.options.Count == 0 || playerDropdown.value < 0)
        {
            Debug.Log("No valid player selected to investigate.");
            return;  
        }
        string selectedPlayerName = playerDropdown.options[playerDropdown.value].text;
        if (hasAttacked)
        {
            Debug.Log("You can only take one action per night.");
            return;
        }
        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer.NickName == selectedPlayerName)
            {
                playerToInvestigate = player;
                hasAttacked = true;
                Investigatebutton.gameObject.SetActive(false);
                photonView.RPC("Investigate", RpcTarget.All, player.photonPlayer);
                ChatManager.DisplayLocalMessage($"Investigated {selectedPlayerName}");
                playerDropdown.gameObject.SetActive(false);
                break;
            }
        }
    }

    private void HandlePsychic()
    {
        if (playerDropdown1.options.Count == 0 || playerDropdown1.value < 0 ||
            playerDropdown2.options.Count == 0 || playerDropdown2.value < 0)
        {
            Debug.Log("No valid players selected for connection check.");
            return;  // No valid players to check
        }

        string playerName1 = playerDropdown1.options[playerDropdown1.value].text;
        string playerName2 = playerDropdown2.options[playerDropdown2.value].text;

        // Find the two players in the GameManager's player list
        PlayerController player1 = GameManager.instance.players.FirstOrDefault(p => p.photonPlayer.NickName == playerName1);
        PlayerController player2 = GameManager.instance.players.FirstOrDefault(p => p.photonPlayer.NickName == playerName2);

        if (player1 != null && player2 != null)
        {

            GameManager.Alignment alignment1 = player1.playerAlignment;
            GameManager.Alignment alignment2 = player2.playerAlignment;

            string result = CheckConnection(player1.playerRole, player1.playerAlignment, player2.playerRole, player2.playerAlignment);
            Debug.Log($"Player1: {playerName1}, Role: {player1.playerRole}, Alignment: {player1.playerAlignment}");
            Debug.Log($"Player2: {playerName2}, Role: {player2.playerRole}, Alignment: {player2.playerAlignment}");
            ChatManager.DisplayLocalMessage($"{playerName1} and {playerName2} are: {result}");
            Debug.Log($"{playerName1} and {playerName2} are: {result}");
            //psychicText.text = $"{playerName1} and {playerName2} are: {result}";
            RecordAction($"Checked connection between {playerName1} and {playerName2}.");
            Psychicsbutton.gameObject.SetActive(false);
            playerDropdown1.gameObject.SetActive(false);
            playerDropdown2.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Players not found.");
        }
    }

    private void HandleConnections()
    {
        if (playerDropdown.options.Count == 0 || playerDropdown.value < 0 )
        {
            Debug.Log("No valid players selected for connection check.");
            return;  // No valid players to check
        }

        string selectedPlayerName = playerDropdown.options[playerDropdown.value].text;

        // Find the two players in the GameManager's player list
        PlayerController selectedPlayer = GameManager.instance.players.FirstOrDefault(p => p.photonPlayer.NickName == selectedPlayerName);
        
        if (selectedPlayer != null)
        {
            GameManager.Alignment playerAlignment = selectedPlayer.playerAlignment;
            // Debug to check player's alignment and role directly from PlayerController
            Debug.Log($"Player: {selectedPlayerName}, Role: {selectedPlayer.playerRole}, Alignment: {selectedPlayer.playerAlignment}");

            //string alignment = selectedPlayer.playerAlignment.ToString();
            //debug.Log($"{selectedPlayerName}'s alignment is: {alignment}");

            //connectionsText.text = $"{selectedPlayerName}'s alignment is: {playerAlignment}";
            ChatManager.DisplayLocalMessage( $"{selectedPlayerName}'s alignment is: {playerAlignment}");
            RecordAction($"Checked alignment for {selectedPlayerName}.");
            ConnectionsButton.gameObject.SetActive(false);
            playerDropdown.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Player not found.");
        }
    }



    public void RecordAction(string action)
    {
        nightActions.Add(action);  
        Debug.Log($"Recorded action: {action}");
    }

    public void ClearNightActions()
    {
        nightActions.Clear();
        Debug.Log("Night actions cleared.");
    }

    private string CheckConnection(GameManager.Role role1, GameManager.Alignment alignment1, GameManager.Role role2, GameManager.Alignment alignment2)
    {
        if (alignment1 == alignment2)
        {
            return "Friendly";
        }
        else
        {
            return "Enemies";
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
        if (!PhotonNetwork.IsMasterClient) return;
        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer == photonPlayer)
            {
                Debug.Log($"Attacking {player.photonPlayer.NickName}");
                player.photonView.RPC("TakeAttackDamage", photonPlayer, 1); 
                break;
            }
        }
    }

    [PunRPC]
    public void Poison(Player photonPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        // if (photonPlayer == photonView.Owner) // Ensure this is the correct player
        // {
        //     Debug.Log($"Poisoning {photonPlayer.NickName}");
        //     GameManager.instance.poisonedPlayers.Add(new PoisonEffect(this, 3)); // Add poison for 3 nights to the current player
        //     GameManager.instance.ApplyPoisonEffects();
        // }

        foreach (var player in GameManager.instance.players)
        {
            if (player != null && player.photonPlayer == photonPlayer)
            {
                Debug.Log($"Poisoning {player.photonPlayer.NickName}");
                GameManager.instance.poisonedPlayers.Add(new PoisonEffect(player, 3)); // Apply poison to the target player
                GameManager.instance.ApplyPoisonEffects(); // Apply poison logic globally
                break;
            }
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

    [PunRPC]
    public void Investigate(Photon.Realtime.Player targetPlayer, PhotonMessageInfo info)
    {
        // Start a coroutine to delay the investigation
        StartCoroutine(DelayedInvestigationCoroutine(targetPlayer, info));
    }

    private IEnumerator DelayedInvestigationCoroutine(Photon.Realtime.Player targetPlayer, PhotonMessageInfo info)
    {
        float investigationDelay = 5f; // Delay time in seconds
        yield return new WaitForSeconds(investigationDelay); // Wait for the specified delay

        // Find the player to investigate
        foreach (var player in GameManager.instance.players)
        {
            if (player.photonPlayer == targetPlayer)
            {
                string actionLog = "Actions taken by " + player.photonPlayer.NickName + ":\n";
                //investigateText.text = "Actions taken by " + player.photonPlayer.NickName + ":\n";
                
                // Get their night actions
                foreach (string action in player.nightActions)
                {
                    actionLog += action + "\n";  // Append each action to the log
                    //investigateText.text += action + "\n";
                }
                if (ChatManager != null)
                {
                    ChatManager.DisplayLocalMessage(actionLog);
                }

                // Send the action log back to the investigator
                photonView.RPC("ReceiveInvestigateResults", info.Sender, actionLog); 
                break;
            }
        }
    }
    // [PunRPC]
    // public void Investigate(Photon.Realtime.Player targetPlayer, PhotonMessageInfo info)
    // {
    //     // Find the player to investigate
    //     foreach (var player in GameManager.instance.players)
    //     {
    //         if (player.photonPlayer == targetPlayer)
    //         {
    //             string actionLog = "Actions taken by " + player.photonPlayer.NickName + ":\n";
    //             investigateText.text = "Actions taken by " + player.photonPlayer.NickName + ":\n";
                
    //             // Get their night actions
    //             foreach (string action in player.nightActions)
    //             {
    //                 actionLog += action + "\n";  // Append each action to the log
    //                 investigateText.text += action + "\n";
    //             }

    //             // Send the action log back to the investigator
    //             photonView.RPC("ReceiveInvestigateResults", info.Sender, actionLog); 
    //             break;
    //         }
    //     }
    // }

    [PunRPC]
    public void ReceiveInvestigateResults(string actionLog)
    {
        // Display the actions of the investigated player
        Debug.Log(actionLog);
        //investigateText.text = actionLog;
    }


    void HandlePlayerDeath()
    {
        // Handle what happens when a player dies
        dead = true;
        Debug.Log($"{photonView.Owner.NickName} has died!");
        GameManager.instance.NotifyPlayerDeath(photonView.Owner.ActorNumber);
        DisplayJournalOnDeath();

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
                RecordAction($"Took {selectedPlayerName} to the basement.");
                ChatManager.DisplayLocalMessage($"Took {selectedPlayerName} to the basement.");
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
            ChatManager.DisplayLocalMessage($"{photonPlayer.NickName} used a Potion. Current Hearts: {currentHearts}");
            playerInventory.Remove("Potion");
        }
        else
        {
            Debug.Log($"{photonPlayer.NickName} tried to use a Potion, but health is full.");
            ChatManager.DisplayLocalMessage($"{photonPlayer.NickName} tried to use a Potion, but health is full.");
        }
    }

    private void UseShield()
    {
       if (!IsNight())
        {
            Debug.Log("Shield can only be used at night!");
            ChatManager.DisplayLocalMessage("Shield can only be used at night!");

            return;
        }

        isProtected = true; // Player is protected
        Debug.Log($"{photonPlayer.NickName} used a Shield. You are protected for the night.");
        ChatManager.DisplayLocalMessage($"{photonPlayer.NickName} used a Shield. You are protected for the night.");
    
        // Optional: You may want to disable the shield after one use
        playerInventory.Remove("Shield");
        UpdateInventoryUI();
    }

    private void UseKnife()
    {
        if (!IsNight())
        {
            ChatManager.DisplayLocalMessage("Knife can only be used at night!");
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
            ChatManager.DisplayLocalMessage("No item selected.");
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
                ChatManager.DisplayLocalMessage($"Attacked {selectedPlayerName}");
                playerInventory.Remove("Knife");
                break;
            }
        }
        attackDropdownUI.SetActive(false);
    }

    public void ToggleJournal()
    {
        if (dead) return; 
        journalUI.SetActive(!journalUI.activeSelf);
        if (journalUI.activeSelf)
        {
            journalDisplay.text = "Journal Entries:\n" + journalContent;// Load previous content into input field
            isJournalOpen = true;
        }
        else
        {
            isJournalOpen = false;
        }
    }
    private void OnJournalInputEndEdit(string inputText)
    {
        
        if (!string.IsNullOrEmpty(inputText))
        {
            journalContent += inputText + "\n";
        
            // Update the displayed journal text
            journalDisplay.text = "Journal Entries:\n" + journalContent;
        
            
            journalInput.text = "";
        }
    }


    // public void SaveJournalEntry()
    // {
    //     journalContent = journalInput.text; // Save current input
    //     journalInput.text = ""; // Clear input field for new entries
    // }

    // Method to display the journal content when the player dies
    public void DisplayJournalOnDeath()
    {
      gameManager.ShowDeathJournal(journalContent);
    }

    public void BecomeSpectator()
    {
        dead = true;
        SwitchToGhostModel();
        DisableAbilities();
        ghostVision.SetActive(true);
        Transform spawnPoint = spectatorSpawnPoints[UnityEngine.Random.Range(0, spectatorSpawnPoints.Length)];

    // Set player position to the spawn point location
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    private void SwitchToGhostModel()
    {
        if (capsuleMeshRenderer != null)
        {
            capsuleMeshRenderer.enabled = false;  // Hide the capsule by turning off its MeshRenderer
        }

        // Enable the ghost model
        if (ghostModel != null)
        {
            ghostModel.SetActive(true);           // Activate the ghost model
        }
    }

    private void DisableAbilities()
    {
        if (localinventoryUIInstance != null){
            localinventoryUIInstance.SetActive(false); 
        }// Disable Inventory UI
    
        if (journalUI != null){
            journalUI.SetActive(false); 
        }  // Disable Journal UI
    
        if (nightcanvas != null){
            nightcanvas.SetActive(false);
        }
        canChat = false;

    }
    [PunRPC]
    public void ReceiveGlobalMessage(string message, string senderName)
    {
        // Assuming you have a reference to your ChatManager
        ChatManager chatManager = FindObjectOfType<ChatManager>();
        if (chatManager != null)
        {
            chatManager.DisplayRemoteMessage(message, senderName); // Call a method in ChatManager to display the message
        }
        else
        {
            Debug.LogError("ChatManager not found!");
        }
    }
 


    
}
