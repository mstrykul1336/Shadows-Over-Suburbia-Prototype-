using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public Transform[] spawnPoints;
    private int playersInGame;
    public static GameManager instance;
    [SerializeField] public float timerDuration = 20f;
    public TextMeshProUGUI timerText;
    [SerializeField] public float timeRemaining;
    public PlayerController[] players;
    [SerializeField] public float postGameTime = 20f;
    [SerializeField] public float firstDayTimeRemaining = 10;
    public GameObject mayorspawn;        
    public GameObject mayorbasementspawn; 
    public TextMeshProUGUI firstdayText;
    public double firstDayStartTime;
    public double votingStartTime;

    public Camera mainCamera; 
    public Material daySkybox;
    public Material nightSkybox;
    public Animator cameraAnimator; 
    public Animator lightsanimator;



    private bool votingActive = false; 
    private int[] votes;
    private int votedOutPlayerIndex = -1;
    private bool[] hasVoted;
    
    private GameObject playerVotingCanvas;
    public GameObject votingCanvasPrefab;
    public TextMeshProUGUI roleRevealText;
    private bool votesProcessed = false;

    public Transform[] spawnPointsNight;
    public bool isNightCycle = false; 
   // public GameObject nightOptionsCanvasPrefab; 
    //private GameObject nightOptionsCanvas;
    //public PlayerAttackUI playerattackUI;
    //public List<GameObject> playerObjects = new List<GameObject>();

    public TextMeshProUGUI winMessageText;
    public TextMeshProUGUI deathMessageText; 
    public int DayCycles = 0;
    public int NightCycles = 0;
    public TextMeshProUGUI cycleText;
    public TextMeshProUGUI dayCounterText;
    public Image cycleIcon;
    public Sprite sunIcon;  
    public Sprite moonIcon; 
    public PlayerController oldManPlayer;

   // public Alignment playerAlignment;
    public Image deathImage; 
    public List<Sprite> deathImages;
    public Image deathPictureImage;
    public List<Sprite> deathPictureSprites;

    private bool isWinnerDeclared = false;
    public GameObject journalDisplayUI;
    public TextMeshProUGUI journalDisplayText;
    public float deathImageDuration = 10f;

    private bool isNominatingPhase = true; // Track if we are in the nominating phase
    private int nominatedPlayerIndex = -1;
 
    public float nightCycleTimeRemaining = 50f;
    private double nightCycleStartTime; 

    public float dayCycleDuration = 60f; // Duration of the day cycle in seconds
    private double dayCycleStartTime;
    public float nightCycleDuration = 60f;


    public Image deathProfileImage;
    public Sprite mayorDeathImage;
    public Sprite villagerDeathImage;
    public Sprite assistantDeathImage;
    public Sprite clairvoyantDeathImage;
    public Sprite oldManDeathImage;
    public Sprite detectiveDeathImage;
    public Sprite medicDeathImage;
    public Sprite bakerDeathImage;
    //private Sprite currentProfilePicture;
    public AudioSource timerAudio;
    public AudioSource morningMusic;
    public AudioSource nightMusic;
    public AudioSource votingMusic;
    public AudioSource daySounds;
    public AudioSource nightSounds;
    public TextMeshProUGUI cutsceneText;


   

    [Header("Effects")]
    public List<PoisonEffect> poisonedPlayers = new List<PoisonEffect>();

   

    public enum Role
    {
        Mayor,
        Assistant,
        Baker,
        Medic,
        Villager,
        Detective,
        OldMan,
        Clairvoyant
    }
    public enum Alignment
    {
        Destructive, 
        Helpful,
        Neutral
    }
    public static Dictionary<int, Role> playerRoles = new Dictionary<int, Role>();
    public static Dictionary<int, Alignment> playerAlignments = new Dictionary<int, Alignment>();

    private int totalPlayers;
    public GameObject playerCanvasPrefab;


    
    void Awake()
    {
        instance = this;
    }


  
    void Start()
    {
        isWinnerDeclared = false;
        timeRemaining = timerDuration;
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        votes = new int[players.Length];
        hasVoted = new bool[PhotonNetwork.PlayerList.Length];
        UpdateCycleUI();
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     AssignRoles();
        // }
        //UpdatePlayerList();
       //firstDayStartTime = PhotonNetwork.Time;
        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }
    }



    [PunRPC]
    void ImInGame()
    {
        Debug.Log("Player has joined the game!");
        playersInGame++;
        Debug.Log($"Players in game: {playersInGame}/{PhotonNetwork.PlayerList.Length}");
        // if (playersInGame == PhotonNetwork.PlayerList.Length)
        // {
        //     Debug.Log("All players are in the game. Spawning player.");
        //     SpawnPlayer();
        // }

         if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("All players are in the game. Spawning players.");
            // Loop through each player and spawn them
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.IsLocal)
                {
                    SpawnPlayer();
                }
            }
            StartCoroutine(WaitAndAssignRoles());
        }
    }

    private IEnumerator WaitAndAssignRoles()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;
        // Wait for a short period to ensure all clients have updated their players array
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }
    }
    
   
    public void SpawnPlayer()
    {
        //int modelIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["ModelIndex"];
        int modelIndex = (int?)PhotonNetwork.LocalPlayer.CustomProperties["ModelIndex"] ?? 0;
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        Debug.Log("Spawning player.");
        //playerObjects[PhotonNetwork.LocalPlayer.ActorNumber] = playerObj;
       // playerObj.name = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";
        //PhotonNetwork.LocalPlayer.TagObject = playerObj;
       // playerObjects.Add(playerObj);
        //playerObj.name = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}"; 
        // initialize the player
        PlayerController playercontroller = playerObj.GetComponent<PlayerController>();
        if (playercontroller != null)
        {
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Ensure this maps correctly
            players[playerIndex] = playercontroller;
            Debug.Log($"PlayerController assigned for player {PhotonNetwork.LocalPlayer.ActorNumber} at index {playerIndex}");
           // playercontroller.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
            playercontroller.photonView.RPC("SetPlayerModel", RpcTarget.AllBuffered, modelIndex);
            photonView.RPC("AddPlayerToList", RpcTarget.AllBuffered, playerIndex, playercontroller.photonView.ViewID);
            playercontroller.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        else
        {
            Debug.LogError("PlayerController is missing on the spawned player!");
        }

        // Log the current state of the array
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
                Debug.Log($"Player at index {i}: {players[i].gameObject.name}");
            else
                Debug.Log($"Player at index {i} is null.");
        }
        //photonView.RPC("AddPlayerToList", RpcTarget.AllBuffered, playerIndex, playercontroller.photonView.ViewID);
       // playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void AddPlayerToList(int playerIndex, int viewID)
    {
        // Find the PlayerController using the ViewID
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView != null)
        {
            PlayerController playercontroller = photonView.GetComponent<PlayerController>();
            if (playercontroller != null)
            {
                // Add the player to the correct index in the players array
                players[playerIndex] = playercontroller;
                Debug.Log($"Player at index {playerIndex}: {playercontroller.gameObject.name}");
            }
            else
            {
                Debug.LogError("PlayerController component not found on PhotonView!");
            }
        }
        else
        {
            Debug.LogError("PhotonView not found with viewID: " + viewID);
        }
    }

    // [PunRPC]
    // void AddPlayerToList(int playerIndex, int viewID)
    // {
    //     // Find the PlayerController using the ViewID
    //     PlayerController playercontroller = PhotonView.Find(viewID).GetComponent<PlayerController>();

    //     // Add the player to the correct index in the players array
    //     players[playerIndex] = playercontroller;

    //     // Log the array state for debugging
    //     Debug.Log($"Player at index {playerIndex}: {playercontroller.gameObject.name}");
    // }

    public PlayerController GetPlayer(int playerId)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.id == playerId)
                return player;
        }
        return null;
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObj)
                return player;
        }
        return null;
    }

    private void AssignRoles()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        if (totalPlayers < 1)
        {
            Debug.Log("There are not enough players for this game.");
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }

        // Create a list of available roles, excluding Mayor
        List<Role> availableRoles = new List<Role>
        {
            Role.Assistant,
            Role.Baker,
            Role.Medic,
            Role.Villager,
            Role.Detective,
            Role.OldMan,
            Role.Clairvoyant
        };

        // Prepare the list of roles to assign (Mayor is always added)
        List<Role> roles = new List<Role> { Role.Mayor };

        if (totalPlayers == 4)
        {
            roles.Add(Role.Baker);
        }
        else if (totalPlayers == 5)
        {
            roles.Add(Role.Baker);
            roles.Add(Role.Medic);
        }
        else if (totalPlayers >= 6)
        {
            roles.Add(Role.Detective);
            roles.Add(Role.Baker);
            roles.Add(Role.Medic);
        }

        // Fill remaining spots with random roles excluding Mayor
        for (int i = roles.Count; i < totalPlayers; i++)
        {
            // Ensure roles are random and unique
            if (availableRoles.Count > 0)
            {
                // Pick a random role from the available list
                int randomIndex = UnityEngine.Random.Range(0, availableRoles.Count);
                Role randomRole = availableRoles[randomIndex];
                roles.Add(randomRole);
                availableRoles.RemoveAt(randomIndex); // Remove to avoid duplication
            }
            else
            {
                // Default to Villager if we've run out of unique roles
                roles.Add(Role.Villager);
            }
        }

        Shuffle(roles); // Shuffle the roles to randomize assignments
        int index = 0;
        // Assign the roles to the players
        foreach (var playerEntry in PhotonNetwork.CurrentRoom.Players)
        {
           // int playerId = player.Key;
           //Role assignedRole = roles[playerId - 1]; 
            int playerId = playerEntry.Key; // Use ActorNumber as the ID
            Player player = playerEntry.Value; // Get the Player object

            Role assignedRole = roles[index];
           //Alignment randomAlignment = RandomizeAlignment();
            Alignment assignedAlignment = (assignedRole == Role.OldMan) ? Alignment.Neutral : RandomizeAlignment();

            playerRoles[playerId] = assignedRole;
            playerAlignments[playerId] = assignedAlignment;

            // Send the role to the player
            PhotonHashtable playerProperties = new PhotonHashtable
            {
                { "PlayerName", player.NickName },
                { "Role", assignedRole.ToString() },
                { "Alignment", assignedAlignment.ToString() }
                //{ "ModelIndex", selectedModelIndex }
            };

            // Setting player properties in Photon
            player.SetCustomProperties(playerProperties);
            //Debug.Log($"Player Name: {playerName}, Role: {role}, Alignment: {alignment}");
            photonView.RPC("ReceiveRoleandAlignment", player, assignedRole, assignedAlignment);
            Debug.Log($"Assigned Role: {assignedRole}, Alignment: {assignedAlignment} for Player: {player}");
            StartCoroutine(DelayedGetProperties(player));
            index++;
        }

       
    }
    private IEnumerator DelayedGetProperties(Player player)
    {
        yield return new WaitForSeconds(1f);  // Add a delay to allow properties to sync
        GetPlayerProperties(player);
    }

  
    private void Shuffle(List<Role> roles)
    {
        for (int i = 0; i < roles.Count; i++)
        {
            Role temp = roles[i];
            int randomIndex = UnityEngine.Random.Range(i, roles.Count);
            roles[i] = roles[randomIndex];
            roles[randomIndex] = temp;
        }
    }

    private Alignment RandomizeAlignment()
    {
        int randomValue = UnityEngine.Random.Range(0, 2); // 0 or 1
        return (randomValue == 0) ? Alignment.Destructive : Alignment.Helpful;
    }

    // public void SetPlayerProperties(string playerName, string role, string alignment)
    // {
    //     PhotonHashtable playerProperties = new PhotonHashtable
    //     {
    //         { "PlayerName", playerName },
    //         { "Role", role },
    //         { "Alignment", alignment }
    //         //{ "ModelIndex", selectedModelIndex }
    //     };

    //     // Setting player properties in Photon
    //     PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    //     Debug.Log($"Player Name: {playerName}, Role: {role}, Alignment: {alignment}");
    // }

    public void GetPlayerProperties(Player player)
    {
         foreach (var property in player.CustomProperties)
        {
            Debug.Log($"Property Key: {property.Key}, Value: {property.Value}");
        }

        if (player.CustomProperties.TryGetValue("PlayerName", out object playerName) &&
            player.CustomProperties.TryGetValue("Role", out object role) &&
            player.CustomProperties.TryGetValue("Alignment", out object alignment))
        {
            Debug.Log($"Get Player Name: {playerName}, Role: {role}, Alignment: {alignment}");
        }
        else
        {
            Debug.LogWarning("One or more properties are missing.");
        }
    }

    [PunRPC]
    private void ReceiveRoleandAlignment(Role role, Alignment alignment)
    {
        Debug.Log($"RPC called for player {PhotonNetwork.LocalPlayer.NickName} with Role: {role}, Alignment: {alignment}");
        //CreatePlayerCanvas(role, alignment);
         Debug.Log($"PhotonView Ownership Check: photonView.Owner = {photonView.Owner.NickName}, LocalPlayer = {PhotonNetwork.LocalPlayer.NickName}");

        // Notify the PlayerController about the role and alignment
        // GameObject playerControllerObj = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        // PlayerController playerController = playerControllerObj?.GetComponent<PlayerController>();
        // // PlayerController playerController = GetComponent<PlayerController>();
        // if (playerController != null)
        // {
        //     playerController.SetRoleAndAlignment(role, alignment);
        // }
        //PlayerController playerController = GetComponent<PlayerController>();
        // foreach (PlayerController player in players)
        // {
        //     if (player != null)
        //     {
        //         Debug.Log($"Setting role and alignment for player: {player.photonPlayer.NickName}");
        //         player.SetRoleAndAlignment(role, alignment);
        //     }
        // }
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Ensure this maps correctly
        PlayerController localPlayerController = players[playerIndex];

        if (localPlayerController != null)
        {
            Debug.Log($"Calling SetRoleAndAlignment for local player {PhotonNetwork.LocalPlayer.NickName}");
            localPlayerController.SetRoleAndAlignment(role, alignment);
            //photonView.RPC("SetRoleAndAlignmentRPC", RpcTarget.All, role, alignment);
        }
        else
        {
            Debug.LogError($"PlayerController for {PhotonNetwork.LocalPlayer.NickName} is null!");
        }
        CreatePlayerCanvas(role, alignment);
        PlayerList playerList = FindObjectOfType<PlayerList>();
       // playerList.UpdatePlayerList();
       photonView.RPC("UpdatePlayerList", RpcTarget.All);
            
            
    }

    private void CreatePlayerCanvas(Role role, Alignment alignment)
    {
        GameObject playerCanvas = Instantiate(playerCanvasPrefab);
        playerCanvas.transform.SetParent(PhotonNetwork.LocalPlayer.TagObject as Transform, false);

        TextMeshProUGUI roleText = playerCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (roleText != null)
        {
            roleText.text = $"Your Role: {role}";
            if (alignment == Alignment.Destructive)
            {
                roleText.color = Color.red;
            }
            else if (alignment == Alignment.Helpful)
            {
                roleText.color = Color.green; // Green for Helpful
            }
            else if (alignment == Alignment.Neutral)
            {
                roleText.color = Color.gray; // Gray for Neutral
            }
        }
    }

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     Debug.Log($"{newPlayer.NickName} has entered the room.");
    //     photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    // }


    void UpdateCycleUI()
    {
        if (isNightCycle)
        {
            cycleText.text = $"Night";
            dayCounterText.text = $"{NightCycles} ";
            cycleIcon.sprite = moonIcon;
        }
        else
        {
            cycleText.text = $"Day";
            dayCounterText.text = $"{DayCycles}";
            cycleIcon.sprite = sunIcon;
        }
    }
    
    void Update()
    {
         // Only update the timer if voting is active
        if (isNightCycle)
        {
            return; // Skip updates during night cycle
        }
        if (DayCycles == 0)
        {
            if (firstDayStartTime == 0)
            {
                firstDayStartTime = PhotonNetwork.Time; // Initialize start time once
            }
            double elapsedFirstDayTime = PhotonNetwork.Time - firstDayStartTime;
            firstDayTimeRemaining = 30f - (float)elapsedFirstDayTime;
            // Start the night cycle after 30 seconds
            if (firstDayStartTime > 0)
            {
                //firstDayTimeRemaining -= Time.deltaTime; // Decrease the first day timer
                //firstDayTimeRemaining = 30f - (float)(PhotonNetwork.Time);
                firstdayText.text = $"{firstDayTimeRemaining:F2} seconds until night cycle";
            }
             if (elapsedFirstDayTime <= 1.5f) 
            {
                PlayerList playerList = FindObjectOfType<PlayerList>();
                if (playerList != null)
                {
                    photonView.RPC("UpdatePlayerList", RpcTarget.All);
                }
            }
            if (firstDayTimeRemaining <= 0)
            {
                Debug.Log("30 seconds elapsed. Starting night cycle.");
                StartNightCycle(); // Start night cycle after 30 seconds
                //votingStartTime = PhotonNetwork.Time;
                timerAudio.Play();
                firstDayStartTime = 0; // Reset the start time for next cycle
            }
            return;
        }

        //Continue with regular voting logic
        if (votingStartTime > 0) // Ensure votingStartTime has been set
        {
            double elapsedVotingTime = PhotonNetwork.Time - votingStartTime;
            timeRemaining = 30f - (float)elapsedVotingTime;

            if (timeRemaining > 0)
            {
                timerText.text = $"{timeRemaining:F2}";
            }
            else if (!votingActive && timeRemaining <= 0)
            {
                Debug.Log("Time is up! Start voting!");
                StartVoting();
                timerAudio.Play();
                votingMusic.Play();
                votingStartTime = 0f; // Reset after voting ends
            }
        }

        // if (timeRemaining > 0)
        // {
        //     timeRemaining -= Time.deltaTime;
        //     timerText.text = $"{timeRemaining:F2}";
        // }
        // else if (!votingActive)
        // {
        //     Debug.Log("Time is up! Start voting!");
        //     StartVoting();
        // }
    }
    void StartVoting()
    {
        if (DayCycles == 0)
        {
            Debug.Log("Voting cannot start on the first day cycle.");
            return;
        }
        if (!votingActive)
        {
            votingActive = true; 
            Debug.Log("Voting is active");
            votesProcessed = false;
            var localPlayer = FindObjectOfType<FirstPersonController>();
            if (votes == null || votes.Length != PhotonNetwork.PlayerList.Length) {
                votes = new int[PhotonNetwork.PlayerList.Length]; // Initialize once
                hasVoted = new bool[PhotonNetwork.PlayerList.Length]; // Initialize once
            } 
            else{
                Array.Clear(votes, 0, votes.Length); // Reset votes
                Array.Clear(hasVoted, 0, hasVoted.Length); // Reset hasVoted
            }
            if (localPlayer != null)
            {
                localPlayer.DisablePlayerControls();
            }
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                CreateVotingCanvas();
            }

           
            if (!PhotonNetwork.IsMasterClient) return;
           // StartCoroutine(VotingDelay());
            StartCoroutine(NominatingPhase());
        }
    }
    void CreateVotingCanvas()
    {
        playerVotingCanvas = Instantiate(votingCanvasPrefab);
        playerVotingCanvas.transform.SetParent(PhotonNetwork.LocalPlayer.TagObject as Transform, false);

        // Get the TMP_Dropdown and Button components from the voting canvas
        TMP_Dropdown playerDropdown = playerVotingCanvas.GetComponentInChildren<TMP_Dropdown>();
        Button voteButton = playerVotingCanvas.GetComponentInChildren<Button>();

        // Initialize the dropdown with player names and numbers
        InitializePlayerDropdown(playerDropdown);

        // Add listener to cast vote when button is clicked
        voteButton.onClick.AddListener(() => CastVote(playerDropdown.value));
    }
    // private void StartVotingPhase()
    // {
    //     isNominatingPhase = false; // Switch to voting phase
    //     Debug.Log("Voting phase has started.");

    //     // Show nominated player to everyone
    //     NotifyAllPlayersOfNomination();

    //     // Give the nominated player 30 seconds to make their case
    //     StartCoroutine(NomineeSpeechPhase());

    //     // Set up the voting canvas for the second round of voting
    //     CreateVotingCanvasForVoting();
    // }

    private void NotifyAllPlayersOfNomination()
    {
        // Notify all players about who is nominated
        Debug.Log($"Player {nominatedPlayerIndex} has been nominated for voting.");
    }

    private void CreateVotingCanvasForVoting()
    {
        // Similar to CreateVotingCanvas but for the second round
        playerVotingCanvas = Instantiate(votingCanvasPrefab);
        playerVotingCanvas.transform.SetParent(PhotonNetwork.LocalPlayer.TagObject as Transform, false);
        TMP_Dropdown playerDropdown = playerVotingCanvas.GetComponentInChildren<TMP_Dropdown>();
        Button voteButton = playerVotingCanvas.GetComponentInChildren<Button>();

        InitializePlayerDropdown(playerDropdown); // Reinitialize for voting

        // Add listener to cast vote when button is clicked
        voteButton.onClick.AddListener(() => CastVote(playerDropdown.value));
    }


    void InitializePlayerDropdown(TMP_Dropdown playerDropdown)
    {
        playerDropdown.ClearOptions();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerController playerController = GetPlayerControllerById(player.ActorNumber);
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && (playerController == null || !playerController.dead))
            {
                string optionText = $"{player.NickName} (#{player.ActorNumber})";
                playerDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
            }
        }

        // Refresh the dropdown after adding options
        playerDropdown.RefreshShownValue();
    }
    private PlayerController GetPlayerControllerById(int actorNumber)
    {
        foreach (var pc in players) // Assuming 'players' is a list of all PlayerController instances
        {
            if (pc.photonView.Owner.ActorNumber == actorNumber)
            {
                return pc;
            }
        }
        return null;
    }

    public void CastVote(int playerIndex)
    {
        // if (hasVoted[PhotonNetwork.LocalPlayer.ActorNumber - 1])
        // {
        //     Debug.Log("You have already voted.");
        //     return;
        // }
        // Debug.Log($"Attempting to cast vote for player at index {playerIndex}");

        // // Check if voting is active
        // if (votingActive)
        // {
        //     // Validate playerIndex
        //     if (playerIndex < 0 || playerIndex >= votes.Length) 
        //     {
        //         Debug.LogError($"Invalid player index: {playerIndex}. Must be between 0 and {votes.Length - 1}.");
        //         return; // Exit the method if index is invalid
        //     }

        //     int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        //     if (!hasVoted[localPlayerIndex])
        //     {
        //         votes[playerIndex]++; // Increment the vote count for the selected player
        //         hasVoted[localPlayerIndex] = true; // Mark this player as having voted
        //         Debug.Log($"Player {playerIndex} received a vote!");
        //         Destroy(playerVotingCanvas);
        //         //CheckVoteResults(); // Check if we need to process the voting results
        //     }
        //     else
        //     {
        //         Debug.Log("You have already voted.");
        //     }
        // }
        // else
        // {
        //     Debug.Log("Voting is not active.");
        // }

        if (isNominatingPhase && votingActive)
        {
            int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

            if (!hasVoted[localPlayerIndex])
            {
                // Cast first vote
                photonView.RPC("ReceiveVote", RpcTarget.All, playerIndex);

                // Check if this player is the Old Man and allow them to vote again
                if (players[localPlayerIndex].IsOldMan)
                {
                    // Allow the Old Man to vote twice
                    Debug.Log("Old Man is voting again.");
                    
                    // You may want to give the player an option to vote for a second player,
                    // or automatically cast a second vote for the same player
                    photonView.RPC("ReceiveVote", RpcTarget.All, playerIndex);
                }

                // Mark this player as having voted
                hasVoted[localPlayerIndex] = true;
                Debug.Log($"Player {playerIndex} received a vote!");

                // Hide the voting UI or disable voting buttons
                Destroy(playerVotingCanvas);

                // Optionally, call CheckVoteResults() here to tally votes if voting is done
            }
            else
            {
                Debug.Log("You have already voted.");
            }
        }
        else
        {
            Debug.Log("Voting is not active.");
        }
    }

    [PunRPC]
    void ReceiveVote(int playerIndex)
    {
        votes[playerIndex]++;
        Debug.Log($"Player {playerIndex} received a vote!");
    }

    void ResetForNextVoting()
    {
        Debug.Log("Resetting for next voting session.");

        // Reset timer
        timeRemaining = timerDuration;

        // Reset voting state and votes array
        votingActive = false;
        votes = new int[PhotonNetwork.PlayerList.Length]; // Reset vote count

        // Remove previous voting canvas
        if (playerVotingCanvas != null)
        {
            Destroy(playerVotingCanvas);
            playerVotingCanvas = null;
        }

        // Update player list in case some players are voted out
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length]; 
    }

    void EndVoting()
    {
        if (votingActive) // Check if voting is still active
        {
            votingActive = false;
            Debug.Log("Voting has ended");

            // Process the vote results before resetting
            CheckVoteResults();
            
            if (playerVotingCanvas != null)
            {
                Destroy(playerVotingCanvas);
                playerVotingCanvas = null; 
            }

            // Enable player controls again
            var localPlayer = FindObjectOfType<FirstPersonController>();
            if (localPlayer != null)
            {
                localPlayer.EnablePlayerControls();
            }
            

            // Teleport remaining players
            StartCoroutine(EndVotingNight());
        }
    
    // Reset for next voting session
        //ResetForNextVoting();
    }
    private IEnumerator NominatingPhase()
    {
        // Wait for 30 seconds
        yield return new WaitForSeconds(30f);
        
        // After the delay, call EndVoting
        EndVoting();
        //StartVotingPhase();
    }

    private IEnumerator EndVotingNight()
    {
        // Wait for 30 seconds
        yield return new WaitForSeconds(10f);
        
        // After the delay, call EndVoting
        StartNightCycle();
        //StartVotingPhase();
    }

    public void StartNightCycle()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            double startTime = PhotonNetwork.Time; // Use PhotonNetwork.Time to ensure consistency
            photonView.RPC(nameof(StartNightCycleSync), RpcTarget.All, startTime); // Broadcast to all clients
        }
    }
    public void StartDayCycle()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            double startTime = PhotonNetwork.Time; // Use PhotonNetwork.Time to ensure consistency
            photonView.RPC(nameof(StartDayCycleSync), RpcTarget.All, startTime); // Broadcast to all clients
        }
    }

    [PunRPC]
    public void StartDayCycleSync(double startTime)
    {
        dayCycleStartTime = startTime;
        StartCoroutine(DayCycleRoutine());
    }
    

    [PunRPC]
    public void StartNightCycleSync(double startTime)
    {
        nightCycleStartTime = startTime;
        StartCoroutine(NightCycleRoutine());
    }
    public IEnumerator NightCycleRoutine()
    {
        Debug.Log("Starting night cycle");
        nightSounds.Play();
        SwitchToMainCamera(nightSkybox);
        cutsceneText.text = "The night falls!";
        Invoke(nameof(ReturnToPlayerCamera), 10f);
        isNightCycle = true;
        NightCycles += 1;
        ApplyPoisonEffects();
        nightMusic.Play();
        if (firstdayText.gameObject.activeSelf)
        {
            firstdayText.gameObject.SetActive(false); // Deactivate the text object
        }
    
        SpawnRemainingPlayers(); // Teleport players if needed
        if (playerVotingCanvas != null)
        {
            Destroy(playerVotingCanvas);
            playerVotingCanvas = null;
        }
        yield return new WaitForSeconds(10f);
        cutsceneText.text = "";

        // Update cycle UI after delay
        UpdateCycleUI();
        // Vector3 canvasPosition = new Vector3(0, 0, 0); 
        // Quaternion canvasRotation = Quaternion.identity;
        // nightOptionsCanvas = PhotonNetwork.Instantiate(nightOptionsCanvasPrefab.name,canvasPosition, canvasRotation, 0 );
        // nightOptionsCanvas.transform.SetParent(PhotonNetwork.LocalPlayer.TagObject as Transform, false);
        // AttackManager attackManager = GetComponent<AttackManager>();
        // if (attackManager == null)
        // {
        //     Debug.LogError("AttackManager component not found on this GameObject.");
        //     return; // Exit if AttackManager is not found
        // }
        foreach (PlayerController player in players)
        {
            // if (player != null)
            player.NightUI();
            // if (photonView.IsMine)
            // {
            //     player.NightUI();
            // }
            // var Playercontroller = GetComponent<PlayerController>();
           
        }
        
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (localPlayer != null )
        {
            localPlayer.DisablePlayerControls();
        }
        // while (nightCycleTimeRemaining > 0)
        // {
        //     timerText.text = $"{nightCycleTimeRemaining:F2}"; // Display the time remaining
        //     yield return new WaitForSeconds(1f); // Update every second
        //     nightCycleTimeRemaining -= 1f; // Decrease the time remaining by 1 second
        // }

        while (true)
        {
            // Calculate time remaining based on start time
            double elapsed = PhotonNetwork.Time - nightCycleStartTime;
            nightCycleTimeRemaining = nightCycleDuration - (float)elapsed;

            // Update UI with the synchronized remaining time
            if (nightCycleTimeRemaining > 0)
            {
                timerText.text = $"{nightCycleTimeRemaining:F2}";
                yield return new WaitForSeconds(1f); // Update every second
            }
            else
            {
                // End the night cycle once time is up
                timerText.text = "0.00";
                break;
            }
        }
       
        StartDayCycle();

    }

    public IEnumerator DayCycleRoutine()
    {
        SwitchToMainCamera(daySkybox);
        cutsceneText.text = "The day awakens!";
        daySounds.Play();
        Invoke(nameof(ReturnToPlayerCamera), 10f);
        isNightCycle = false;
        DayCycles += 1;
        UpdateCycleUI();
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (localPlayer != null)
        {
            localPlayer.EnablePlayerControls();
        }
        // if (nightOptionsCanvas != null)
        // {
        //     PlayerAttackUI playerAttackUI = nightOptionsCanvas.GetComponent<PlayerAttackUI>();
        //     if (playerAttackUI != null)
        //     {
        //         playerAttackUI.gameObject.SetActive(false); // Hide Attack UI
        //     }
        // }
        // foreach (PlayerController player in players)
        // {
        //     if (photonView.IsMine && player != null)
        //     {
        //         player.NightUIOff();
        //         player.GiveGold();
        //     }
        // }
        foreach (PlayerController player in players)
        {
            if (player != null && !player.dead)
            {
                // Give gold to each player
                player.GiveGold();
                Debug.Log($"Giving gold to {player.photonPlayer.NickName}");

                // Call NightUIOff for each player
                player.NightUIOff();
                //clear the actions list for detective
                player.ClearNightActions();
                cutsceneText.text = "";
            }
        }
        //yield return new WaitForSeconds(10f);
        SpawnDayPlayers();
        morningMusic.Play();
        StartCoroutine(CheckWinCondition());
        while (true)
        {
            // Calculate time remaining based on start time
            double elapsed = PhotonNetwork.Time - dayCycleStartTime;
            float dayCycleTimeRemaining = dayCycleDuration - (float)elapsed;

            // Update UI with the synchronized remaining time
            if (dayCycleTimeRemaining > 0)
            {
                timerText.text = $"{dayCycleTimeRemaining:F2}";
                yield return new WaitForSeconds(1f); // Update every second
            }
            else
            {
                // End the day cycle once time is up
                timerText.text = "0.00";
                break;
            }
        }

        StartVotingTimer(); 
    }

    

    void SwitchToMainCamera(Material skybox)
    {
        if (cameraAnimator != null)
        {
            cameraAnimator.SetTrigger("StartNightCycleTrigger"); 
        }
        if (lightsanimator != null)
        {
            lightsanimator.SetTrigger("StartLights"); 
        }
        RenderSettings.skybox = skybox;
        mainCamera.enabled = true;

        foreach (var player in players)
        {
            var playerCam = player.GetComponentInChildren<Camera>();
            if (playerCam != null)
            {
                playerCam.enabled = false;
            }
        }
    }

    void ReturnToPlayerCamera()
    {
        mainCamera.enabled = false;

        foreach (var player in players)
        {
            var photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                var playerCam = player.GetComponentInChildren<Camera>();
                if (playerCam != null)
                {
                    playerCam.enabled = true;
                }
            }
            else
            {
                var playerCam = player.GetComponentInChildren<Camera>();
                if (playerCam != null)
                {
                    playerCam.enabled = false;
                }
            }
        }
    }

    void SpawnDayPlayers()
    {
        foreach (var player in players)
        {
            if (player != null && !player.dead) // Ensure the player is not null
            {
                // Get a random spawn point from the spawnPoints array
                Vector3 teleportLocation = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;

                // Teleport the player to the new location
                player.transform.position = teleportLocation;

                // Optionally, you might want to call an RPC to update other clients about the player's new position
                player.GetComponent<PhotonView>().RPC("UpdatePosition", RpcTarget.Others, teleportLocation);
            }
        }
    }
    void StartVotingTimer()
    {
        //timeRemaining = timerDuration; // Reset the timer
        votingStartTime = PhotonNetwork.Time;
    
        Debug.Log("Timer started for voting.");
    }
    void SpawnRemainingPlayers()
    {
        Debug.Log("Spawning players");
        foreach (var player in players)
        {
            if (player != null && !player.dead) // Ensure the player is not null
            {
                 PhotonView playerPhotonView = player.GetComponent<PhotonView>();
                 if (playerPhotonView != null && playerPhotonView.IsMine)
                 {
                    // Get a random spawn point from the spawnPoints array
                    Vector3 teleportLocation = spawnPointsNight[UnityEngine.Random.Range(0, spawnPointsNight.Length)].position;
                    // Teleport the player to the new location
                    player.transform.position = teleportLocation;
                    Debug.Log($"{player.name} is has been teleported to {teleportLocation}");

                    // Optionally, you might want to call an RPC to update other clients about the player's new position
                    player.GetComponent<PhotonView>().RPC("UpdatePosition", RpcTarget.Others, teleportLocation);
                 }
            }
        }
    }

    [PunRPC]
    public void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    void CheckVoteResults()
    {
        Debug.Log("Check vote results!");
        if (votesProcessed)
            return;
        int maxVotes = 0;
        // votedOutPlayerIndex = -1;

        // for (int i = 0; i < votes.Length; i++)
        // {
        //     if (votes[i] > maxVotes)
        //     {
        //         maxVotes = votes[i];
        //         votedOutPlayerIndex = i;
        //     }
        // }

        // if (maxVotes > 0) // If there were any votes
        // {
        //     Debug.Log($"Player {votedOutPlayerIndex} has been voted out with {maxVotes} votes!");
        //     RevealRoleAndRemovePlayer(votedOutPlayerIndex);
        // }
        List<int> potentialWinners = new List<int>();

        for (int i = 0; i < votes.Length; i++)
        {
            if (votes[i] > maxVotes)
            {
                maxVotes = votes[i];
                potentialWinners.Clear(); // Clear previous winners
                potentialWinners.Add(i); // Add new winner
            }
            else if (votes[i] == maxVotes && maxVotes > 0)
            {
                potentialWinners.Add(i); // Tie found
            }
        }

        // Handle results based on the number of potential winners
        if (potentialWinners.Count == 1)
        {
            votedOutPlayerIndex = potentialWinners[0];
            Debug.Log($"Player {votedOutPlayerIndex} has been voted out with {maxVotes} votes!");
            StartCoroutine(EliminationDelay());
        }
        else if (potentialWinners.Count > 1)
        {
            Debug.Log("Tie detected, restarting voting.");
            ResetForNextVoting(); // Reset to allow for a new vote
            StartVoting(); // Start a new voting session
        }

        StartCoroutine(CheckWinCondition());
        votesProcessed = true;

    }

    private IEnumerator EliminationDelay()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;
        // Wait for a specified duration (e.g., 3 seconds)
        yield return new WaitForSeconds(5f);
        
        // Reveal role and remove player after delay
        RevealRoleAndRemovePlayer(votedOutPlayerIndex);
    }
    
    void RevealRoleAndRemovePlayer(int playerIndex)
    {
        Debug.Log("Revealing player and role");
        //PlayerController targetPlayer = players[playerIndex];
        // // Here you need to get the role of the player and display it
        // if (targetPlayer != null)
        // {
        //     // Get the player's role and display it
        //     string role = targetPlayer.GetRole();
        //     roleRevealText.text = $"Player {playerIndex + 1} was voted out! Their role was: {role}";
        //     //targetPlayer.dead = true;
        //     targetPlayer.BecomeSpectator();
        //     Debug.Log($"{targetPlayer.photonView.Owner.NickName} has been turned into a spectator.");

        //     // Only the Master Client can destroy and disconnect players
        //     // if (PhotonNetwork.IsMasterClient)
        //     // {
        //     //     // Destroy the player's GameObject across the network
        //     //     PhotonNetwork.Destroy(targetPlayer.gameObject);

        //     //     // Get the PhotonPlayer object associated with the player
        //     //     Photon.Realtime.Player photonPlayer = targetPlayer.photonView.Owner;

        //     //     // Forcefully disconnect the player if they are still connected
        //     //     if (photonPlayer != null)
        //     //     {
        //     //         PhotonNetwork.CloseConnection(photonPlayer);
        //     //         Debug.Log("Player " + photonPlayer.NickName + " has been forcefully disconnected.");
        //     //     }
        //     // }
        // }

        PlayerController targetPlayer = players[playerIndex];
    
        if (targetPlayer != null)
        {
            // Access the player's role from Photon Hashtable properties
            if (targetPlayer.photonView.Owner.CustomProperties.TryGetValue("Role", out object role))
            {
                roleRevealText.text = $"Player {playerIndex + 1} was voted out! Their role was: {role}";
                // Mark player as spectator
                targetPlayer.BecomeSpectator();
                Debug.Log($"{targetPlayer.photonView.Owner.NickName} has been turned into a spectator.");
            }
            else
            {
                Debug.LogWarning("Role not found in player properties.");
            }
        }
       
    }
    public void NotifyPlayerDeath(int playerId)
    {
        string playerName = PhotonNetwork.CurrentRoom.Players[playerId].NickName;
        PlayerController targetPlayer = players[playerId];
        // Get the player's nickname
        // string playerName = PhotonNetwork.CurrentRoom.Players[playerId].NickName;
        // PlayerController targetPlayer = players[playerId];
        // string role = targetPlayer.GetRole();

        // // Display death message
        // if (deathMessageText != null)
        // {

        //     deathMessageText.text = $"{playerName} has died during the night. Their role was {role}!";
        //     deathMessageText.gameObject.SetActive(true); 
        // }
        // if (deathImage != null && deathImages.Count > 0)
        // {
        //     // Select a random image
        //     int randomIndex = UnityEngine.Random.Range(0, deathImages.Count);
        //     deathImage.sprite = deathImages[randomIndex];
        //     deathImage.gameObject.SetActive(true); // Show the image
        // }

        // // Start coroutine to hide the message after a delay
        // if (!PhotonNetwork.IsMasterClient) return;
        // StartCoroutine(HideDeathMessageAndDestroyPlayer(playerId, 10f));
        if (targetPlayer.photonView.Owner.CustomProperties.TryGetValue("Role", out object role))
        {
            if (deathMessageText != null)
            {
                deathMessageText.text = $"{playerName} has died during the night. Their role was {role}!";
                deathMessageText.gameObject.SetActive(true); 
            }

            if (deathImage != null && deathImages.Count > 0)
            {
                Sprite roleDeathImage = GetRoleDeathImage(role.ToString());
                int randomIndex = UnityEngine.Random.Range(0, deathImages.Count);
                deathImage.sprite = deathImages[randomIndex];
                deathImage.gameObject.SetActive(true);
                deathProfileImage.gameObject.SetActive(true);
                deathProfileImage.sprite = roleDeathImage;
            }

            // Start coroutine to hide the message after a delay
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(HideDeathMessageAndDestroyPlayer(playerId, 10f));
            }
        }
        else
        {
            Debug.LogWarning("Role not found in player properties.");
        }
    }

    private Sprite GetRoleDeathImage(string role)
    {
        // Check for the player's role and return the appropriate sprite
        switch (role)
        {
            case "Mayor":
                return mayorDeathImage; // Replace with the actual mayor image
            case "Baker":
                return bakerDeathImage; // Replace with the actual baker image
            case "Villager":
                return villagerDeathImage; // Replace with the actual villager image
            case "Assistant":
                return assistantDeathImage; // Replace with the actual assistant image
            case "Medic":
                return medicDeathImage; // Replace with the actual medic image
            case "Detective":
                return detectiveDeathImage; // Replace with the actual detective image
            case "Clairvoyant":
                return clairvoyantDeathImage; // Replace with the actual clairvoyant image
            case "OldMan":
                return oldManDeathImage; // Replace with the actual old man image
            default:
                return null; // Return null if no specific image is found for the role
        }
    }

    private IEnumerator HideDeathMessageAndDestroyPlayer(int playerId, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (deathMessageText != null)
        {
            deathMessageText.gameObject.SetActive(false);
            deathImage.gameObject.SetActive(false); 
        }
        var playerController = GetPlayer(playerId);

        if (playerController != null)
        {
            playerController.BecomeSpectator();
            // // Check if the current client is the Master Client
            // if (PhotonNetwork.IsMasterClient)
            // {
            //     // Check if the current client is not the owner of the PhotonView
            //     if (!playerController.photonView.IsMine)
            //     {
            //         // Take ownership of the PhotonView if the original owner has left
            //         playerController.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            //     }

            //     // Now destroy the object since the Master Client has ownership
            //     PhotonNetwork.Destroy(playerController.gameObject);
            // }
            // else
            // {
            //     Debug.LogError("Only the Master Client can destroy players' GameObjects.");
            // }
        }
    }

    public void OldManDied()
    {
        // The Old Man (neutral role) wins if they are killed
        photonView.RPC("WinGame", RpcTarget.All, (int)Alignment.Neutral);
    }

    // [PunRPC]
    // void WinGame(int winningSide)
    // {
    //     if (winningSide == (int)GameManager.Alignment.Helpful)
    //     {
    //         // Handle Helpful win (e.g., show a message, transition to end screen)
    //         Debug.Log("Helpful side has won!");
    //         winMessageText.text = "Helpful won!";

    //     }
    //     else if (winningSide == (int)GameManager.Alignment.Destructive)
    //     {
    //         // Handle Destructive win (e.g., show a message, transition to end screen)
    //         Debug.Log("Destructive side has won!");
    //         winMessageText.text = "Destructive won!";
    //     }
    //     else if (winningSide == (int)GameManager.Alignment.Neutral)
    //     {
    //         Debug.Log("The Old Man (Neutral) has won by being killed!");
    //         winMessageText.text = "Old Man (Neutral) wins!";
    //     }

    //     Invoke("GoBackToMenu", postGameTime);
    // }

    [PunRPC]
    void WinGame(int winningSide)
    {
        //Time.timeScale = 0; 
        string winningSideName = "";
        string[] playerNames = new string[0];

        if (winningSide == (int)GameManager.Alignment.Helpful)
        {
            winningSideName = "Helpful";
            playerNames = GetPlayerNamesByAlignment(GameManager.Alignment.Helpful);
        }
        else if (winningSide == (int)GameManager.Alignment.Destructive)
        {
            winningSideName = "Destructive";
            playerNames = GetPlayerNamesByAlignment(GameManager.Alignment.Destructive);
        }
        else if (winningSide == (int)GameManager.Alignment.Neutral)
        {
            winningSideName = "Neutral";
            playerNames = new string[] { oldManPlayer.photonPlayer.NickName }; // Assuming oldManPlayer has a PhotonPlayer reference
        }

        Debug.Log($"{winningSideName} side has won!");
        // Show the win UI
        WinUIManager winUIManager = FindObjectOfType<WinUIManager>();
        if (winUIManager != null)
        {
            Debug.Log("Showing Win screen");
            winUIManager.ShowWinUI(winningSideName, playerNames);
        }
        else {
            Debug.Log("WinUI not found");
        }

        Invoke("GoBackToMenu", postGameTime);
    }

    private string[] GetPlayerNamesByAlignment(GameManager.Alignment alignment)
    {
        // List<string> playerNames = new List<string>();

        // foreach (var player in PhotonNetwork.PlayerList)
        // {
        //     if (GameManager.playerAlignments.TryGetValue(player.ActorNumber, out GameManager.Alignment playerAlignment))
        //     {
        //         if (playerAlignment == alignment)
        //         {
        //             playerNames.Add(player.NickName);
        //         }
        //     }
        // }

        // return playerNames.ToArray();
        List<string> playerNames = new List<string>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            // PlayerController playerController = player.TagObject as PlayerController; // Get the PlayerController component directly
            // if (playerController != null && playerController.playerAlignment == alignment) // Check the alignment property in PlayerController
            // {
            //     playerNames.Add(player.NickName);
            // }
            if (player.CustomProperties.TryGetValue("Alignment", out object playerAlignment) && playerAlignment.ToString() == alignment.ToString())
            {
                playerNames.Add(player.NickName);
            }
        }

        return playerNames.ToArray();
    }

    void GoBackToMenu()
    {
        NetworkManager.instance.ChangeScene("Title");
    }

    IEnumerator CheckWinCondition()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;
        bool hasHelpful = false;
        bool hasDestructive = false;
        bool oldManAlive = oldManPlayer != null && !oldManPlayer.dead;
        bool hasNeutral = false;
        if (isWinnerDeclared) // Early exit if a winner has been declared
        {
            yield break;
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"Checking player: {player.NickName}");
            
            if (player.CustomProperties.TryGetValue("Alignment", out object alignment))
            {
                if (alignment.ToString() == GameManager.Alignment.Helpful.ToString())
                {
                    hasHelpful = true;
                    Debug.Log($"{player.NickName} is Helpful.");
                }
                else if (alignment.ToString() == GameManager.Alignment.Destructive.ToString())
                {
                    hasDestructive = true;
                    Debug.Log($"{player.NickName} is Destructive.");
                }
                else if (alignment.ToString() == GameManager.Alignment.Neutral.ToString())
                {
                    hasNeutral = true;
                    Debug.Log($"{player.NickName} is Neutral.");
                }
                Debug.Log($"{player.NickName}: Alignment = {alignment}");
            }
            else
            {
                Debug.LogWarning($"{player.NickName}: Alignment property missing or not synchronized.");
            }

            // if (player.CustomProperties.TryGetValue("Alignment", out object alignment))
            // {
            //     Debug.Log($"{player.NickName}: Alignment = {alignment}");
            // }
            // else
            // {
            //     Debug.LogWarning($"{player.NickName}: Alignment property missing.");
            //     //alignment = GameManager.Alignment.Neutral;
            // }

            // // Check alignment
            // if (alignment.ToString() == GameManager.Alignment.Helpful.ToString())
            // {
            //     hasHelpful = true;
            // }
            // else if (alignment.ToString() == GameManager.Alignment.Destructive.ToString())
            // {
            //     hasDestructive = true;
            // }
            // else if (alignment.ToString() == GameManager.Alignment.Neutral.ToString())
            // {
            //     hasNeutral = true;
            // }
        }
        Debug.Log($"Alignment check complete. Helpful: {hasHelpful}, Destructive: {hasDestructive}, Old Man Alive: {oldManAlive}");
        yield return new WaitForSeconds(2f); 
        DetermineWinner(hasHelpful, hasDestructive, hasNeutral, oldManAlive);
        // if (oldManPlayer != null && oldManPlayer.dead)
        // {
        //     Debug.Log("Old Man has died. Old Man wins!");
        //     isWinnerDeclared = true;
        //     photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Neutral); // Declare Old Man (Neutral) as the winner
        //     yield break; // Stop further win checks
        // }

        // // if (oldManAlive)
        // // {
        // //     Debug.Log("Old Man is still alive. No win for Helpful or Destructive yet.");
        // //     return; // Don't declare a win yet
        // // }

        // if (hasHelpful && hasDestructive)
        // {
        //     Debug.Log("Both Helpful and Destructive sides are still alive. No winner yet.");
        //     yield break; // No winner if both alignments are present
        // }

        // // Determine the winning side
        // if (hasHelpful && !hasDestructive)
        // {
        //     Debug.Log("Helpful side wins!");
        //     // Helpful side wins
        //     isWinnerDeclared = true;
        //     photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Helpful);
        // }
        // if (hasDestructive && !hasHelpful)
        // {
        //     Debug.Log("Destructive side wins!");
        //     // Destructive side wins
        //     isWinnerDeclared = true;
        //     photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Destructive);
        // }

        // if (!hasHelpful && !hasDestructive && oldManPlayer != null && oldManPlayer.dead)
        // {
        //     Debug.Log("Old Man wins due to lack of competition!");
        //     isWinnerDeclared = true;
        //     photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Neutral);
        //     yield break;
        // }
        

        //     // If we reach this point, there may be no players left of either alignment
        // Debug.Log("No clear winner yet."); 
    }

    void DetermineWinner(bool hasHelpful, bool hasDestructive, bool hasNeutral, bool oldManAlive)
    {
        if (oldManPlayer != null && oldManPlayer.dead)
        {
            Debug.Log("Old Man has died. Old Man wins!");
            isWinnerDeclared = true;
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Neutral); // Old Man wins
            return; // Stop further win checks
        }

        if (hasHelpful && hasDestructive)
        {
            Debug.Log("Both Helpful and Destructive sides are still alive. No winner yet.");
            return; // No winner if both alignments are present
        }

        // Determine the winning side
        if (hasHelpful && !hasDestructive)
        {
            Debug.Log("Helpful side wins!");
            isWinnerDeclared = true;
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Helpful);
            return;
        }

        if (hasDestructive && !hasHelpful)
        {
            Debug.Log("Destructive side wins!");
            isWinnerDeclared = true;
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Destructive);
            return;
        }

        if (!hasHelpful && !hasDestructive && oldManPlayer != null && oldManPlayer.dead)
        {
            Debug.Log("Old Man wins due to lack of competition!");
            isWinnerDeclared = true;
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Neutral);
            return;
        }

        // If no winner is determined
        Debug.Log("No clear winner yet.");
    }
   
    // -=-=-
    // Ability and Effects
    // -=-=-==-
    // [PunRPC]
    // public void Poison(Player photonPlayer)
    // {
    //     foreach (var player in players)
    //     {
    //         if (player != null && player.photonPlayer == photonPlayer)
    //         {
    //             Debug.Log($"Poisoning {player.photonPlayer.NickName}");
    //             poisonedPlayers.Add(new PoisonEffect(player, 3)); // Poison for 3 nights
    //             break;
    //         }
    //     }
    // }

    public void ApplyPoisonEffects()
    {
        // for (int i = poisonedPlayers.Count - 1; i >= 0; i--) 
        // {
        //     var effect = poisonedPlayers[i];
        //     if (effect.nightsRemaining > 0)
        //     {
        //         Debug.Log($"Applying poison to {effect.player.photonPlayer.NickName}. Nights remaining: {effect.nightsRemaining}");
        //         effect.player.photonView.RPC("TakePoisonDamage", RpcTarget.All, 1); // Apply 0.25 damage
        //         effect.nightsRemaining--;

        //         // Remove the effect if no nights remain
        //         if (effect.nightsRemaining == 0)
        //         {
        //             poisonedPlayers.RemoveAt(i); // Safely remove from list
        //             Debug.Log($"{effect.player.photonPlayer.NickName} is no longer poisoned.");
        //         }
        //     }
        // }
        foreach (var poisonEffect in poisonedPlayers)
        {
            poisonEffect.ApplyPoison(); // Apply poison damage
        }
    }

    public void ShowDeathJournal(string journalContent)
    {
        journalDisplayText.text = "Journal Entries:\n" + journalContent;
        journalDisplayUI.SetActive(true);

        // Start a coroutine to hide it after 15 seconds
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(HideDeathJournalAfterTime(journalContent));
    }

    // Coroutine to hide the journal after the specified time
    private IEnumerator HideDeathJournalAfterTime(string journalContent)
    {
        yield return new WaitForSeconds(deathImageDuration);

        // Set the journal text and display the UI
        journalDisplayText.text = "Journal Entries:\n" + journalContent;
        journalDisplayUI.SetActive(true);

        // Wait for 15 seconds to display the journal, then hide it
        yield return new WaitForSeconds(15f);
        journalDisplayUI.SetActive(false);
    }

} 






