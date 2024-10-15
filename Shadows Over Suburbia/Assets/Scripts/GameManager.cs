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

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public Transform[] spawnPoints;
    private int playersInGame;
    public static GameManager instance;
    public float timerDuration = 5f;
    public TextMeshProUGUI timerText;
    public float timeRemaining;
    public PlayerController[] players;
    public float postGameTime = 50;
    public float firstDayTimeRemaining = 10;
    public GameObject mayorspawn;        
    public GameObject mayorbasementspawn; 



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
    public TextMeshProUGUI dayText;
    public PlayerController oldManPlayer;
   

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
        timeRemaining = timerDuration;
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        votes = new int[players.Length];
        hasVoted = new bool[PhotonNetwork.PlayerList.Length];
        UpdateDayText();
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     AssignRoles();
        // }
        //UpdatePlayerList();
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
        // Wait for a short period to ensure all clients have updated their players array
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }
    }
    
   
    public void SpawnPlayer()
    {
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
            photonView.RPC("AddPlayerToList", RpcTarget.AllBuffered, playerIndex, playercontroller.photonView.ViewID);
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
        playercontroller.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
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
            SceneManager.LoadScene(sceneBuildIndex: 1);
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
        
        // Assign the roles to the players
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            int playerId = player.Key;
            Role assignedRole = roles[playerId - 1]; 
           //Alignment randomAlignment = RandomizeAlignment();
            Alignment assignedAlignment = (assignedRole == Role.OldMan) ? Alignment.Neutral : RandomizeAlignment();

            playerRoles[playerId] = assignedRole;
            playerAlignments[playerId] = assignedAlignment;

            // Send the role to the player
            photonView.RPC("ReceiveRoleandAlignment", player.Value, assignedRole, assignedAlignment);
        }

       
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
        }
        else
        {
            Debug.LogError($"PlayerController for {PhotonNetwork.LocalPlayer.NickName} is null!");
        }
        CreatePlayerCanvas(role, alignment);
        PlayerList playerList = FindObjectOfType<PlayerList>();
        playerList.UpdatePlayerList();
            
            
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


    void UpdateDayText()
    {
        if (dayText != null)
        {
            dayText.text = $"Day {DayCycles}"; // Update the text to show current day
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
            // Start the night cycle after 30 seconds
            if (firstDayTimeRemaining > 0)
            {
                firstDayTimeRemaining -= Time.deltaTime; // Decrease the first day timer
                timerText.text = $"{firstDayTimeRemaining:F2} seconds until night cycle"; 
            }
            else 
            {
                Debug.Log("30 seconds elapsed. Starting night cycle.");
                StartNightCycle(); // Start night cycle after 30 seconds
            }
            return;
        }

        // Continue with regular voting logic
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"{timeRemaining:F2}";
        }
        else if (!votingActive)
        {
            Debug.Log("Time is up! Start voting!");
            StartVoting();
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

            StartCoroutine(VotingDelay());
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

    void InitializePlayerDropdown(TMP_Dropdown playerDropdown)
    {
        playerDropdown.ClearOptions();
        foreach (var player in PhotonNetwork.PlayerList)
        {
             if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                string optionText = $"{player.NickName} (#{player.ActorNumber})";
                playerDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
            }
        }

        // Refresh the dropdown after adding options
        playerDropdown.RefreshShownValue();
    }

    public void CastVote(int playerIndex)
    {
        if (hasVoted[PhotonNetwork.LocalPlayer.ActorNumber - 1])
        {
            Debug.Log("You have already voted.");
            return;
        }
        Debug.Log($"Attempting to cast vote for player at index {playerIndex}");

        // Check if voting is active
        if (votingActive)
        {
            // Validate playerIndex
            if (playerIndex < 0 || playerIndex >= votes.Length) 
            {
                Debug.LogError($"Invalid player index: {playerIndex}. Must be between 0 and {votes.Length - 1}.");
                return; // Exit the method if index is invalid
            }

            int localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            if (!hasVoted[localPlayerIndex])
            {
                votes[playerIndex]++; // Increment the vote count for the selected player
                hasVoted[localPlayerIndex] = true; // Mark this player as having voted
                Debug.Log($"Player {playerIndex} received a vote!");
                Destroy(playerVotingCanvas);
                //CheckVoteResults(); // Check if we need to process the voting results
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
            StartNightCycle();
        }
    
    // Reset for next voting session
        //ResetForNextVoting();
    }
    private IEnumerator VotingDelay()
    {
        // Wait for 30 seconds
        yield return new WaitForSeconds(30f);
        
        // After the delay, call EndVoting
        EndVoting();
    }

    public void StartNightCycle()
    {
        Debug.Log("Starting night cycle");
        isNightCycle = true;
        NightCycles += 1;
        ApplyPoisonEffects();
    
        SpawnRemainingPlayers(); // Teleport players if needed
        if (playerVotingCanvas != null)
        {
            Destroy(playerVotingCanvas);
            playerVotingCanvas = null;
        }
        roleRevealText.text = "";
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
       
        Invoke(nameof(StartDayCycle), 30f);

    }

    void StartDayCycle()
    {
        isNightCycle = false;
        DayCycles += 1;
        UpdateDayText();
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
            if (player != null)
            {
                // Give gold to each player
                player.GiveGold();
                Debug.Log($"Giving gold to {player.photonPlayer.NickName}");

                // Call NightUIOff for each player
                player.NightUIOff();
            }
        }

        SpawnDayPlayers();
        CheckWinCondition();
        StartVotingTimer();
    }

    void SpawnDayPlayers()
    {
        foreach (var player in players)
        {
            if (player != null) // Ensure the player is not null
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
        timeRemaining = timerDuration; // Reset the timer
    
        Debug.Log("Timer started for voting.");
    }
    void SpawnRemainingPlayers()
    {
        Debug.Log("Spawning players");
        foreach (var player in players)
        {
            if (player != null) // Ensure the player is not null
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

        //CheckWinCondition();
        votesProcessed = true;

    }

    private IEnumerator EliminationDelay()
    {
        // Wait for a specified duration (e.g., 3 seconds)
        yield return new WaitForSeconds(5f);
        
        // Reveal role and remove player after delay
        RevealRoleAndRemovePlayer(votedOutPlayerIndex);
    }
    
    void RevealRoleAndRemovePlayer(int playerIndex)
    {
        Debug.Log("Revealing player and role");
        PlayerController targetPlayer = players[playerIndex];
        // Here you need to get the role of the player and display it
        if (targetPlayer != null)
        {
            // Get the player's role and display it
            string role = targetPlayer.GetRole();
            roleRevealText.text = $"Player {playerIndex + 1} was voted out! Their role was: {role}";
            targetPlayer.dead = true;

            // Only the Master Client can destroy and disconnect players
            if (PhotonNetwork.IsMasterClient)
            {
                // Destroy the player's GameObject across the network
                PhotonNetwork.Destroy(targetPlayer.gameObject);

                // Get the PhotonPlayer object associated with the player
                Photon.Realtime.Player photonPlayer = targetPlayer.photonView.Owner;

                // Forcefully disconnect the player if they are still connected
                if (photonPlayer != null)
                {
                    PhotonNetwork.CloseConnection(photonPlayer);
                    Debug.Log("Player " + photonPlayer.NickName + " has been forcefully disconnected.");
                }
            }
        }
       
    }
    public void NotifyPlayerDeath(int playerId)
    {
        // Get the player's nickname
        string playerName = PhotonNetwork.CurrentRoom.Players[playerId].NickName;

        // Display death message
        if (deathMessageText != null)
        {
            deathMessageText.text = $"{playerName} has died during the night.";
        }

        // Start coroutine to hide the message after a delay
        StartCoroutine(HideDeathMessageAndDestroyPlayer(playerId, 5f));
    }

    private IEnumerator HideDeathMessageAndDestroyPlayer(int playerId, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (deathMessageText != null)
        {
            deathMessageText.gameObject.SetActive(false); // Hide the message
        }
        var playerController = GetPlayer(playerId);

        if (playerController != null)
        {
            // Check if the current client is the Master Client
            if (PhotonNetwork.IsMasterClient)
            {
                // Check if the current client is not the owner of the PhotonView
                if (!playerController.photonView.IsMine)
                {
                    // Take ownership of the PhotonView if the original owner has left
                    playerController.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                }

                // Now destroy the object since the Master Client has ownership
                PhotonNetwork.Destroy(playerController.gameObject);
            }
            else
            {
                Debug.LogError("Only the Master Client can destroy players' GameObjects.");
            }
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
            winningSideName = "Old Man (Neutral)";
            playerNames = new string[] { oldManPlayer.photonPlayer.NickName }; // Assuming oldManPlayer has a PhotonPlayer reference
        }

        Debug.Log($"{winningSideName} side has won!");
        // Show the win UI
        WinUIManager winUIManager = FindObjectOfType<WinUIManager>();
        if (winUIManager != null)
        {
            winUIManager.ShowWinUI(winningSideName, playerNames);
        }

        Invoke("GoBackToMenu", postGameTime);
    }

    private string[] GetPlayerNamesByAlignment(GameManager.Alignment alignment)
    {
        List<string> playerNames = new List<string>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (GameManager.playerAlignments.TryGetValue(player.ActorNumber, out GameManager.Alignment playerAlignment))
            {
                if (playerAlignment == alignment)
                {
                    playerNames.Add(player.NickName);
                }
            }
        }

        return playerNames.ToArray();
    }

    void GoBackToMenu()
    {
        NetworkManager.instance.ChangeScene("Title");
    }

    void CheckWinCondition()
    {
        bool hasHelpful = false;
        bool hasDestructive = false;
       

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (GameManager.playerAlignments.TryGetValue(player.ActorNumber, out GameManager.Alignment alignment))
            {
                if (alignment == GameManager.Alignment.Helpful)
                {
                    hasHelpful = true;
                }
                else if (alignment == GameManager.Alignment.Destructive)
                {
                    hasDestructive = true;
                }
            }
        }
        if (oldManPlayer != null && oldManPlayer.dead)
        {
            Debug.Log("Old Man has died. Old Man wins!");
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Neutral); // Declare Old Man (Neutral) as the winner
            return; // Stop further win checks
        }

        if (oldManPlayer != null && !oldManPlayer.dead)
        {
            Debug.Log("Old Man is still alive. No win for Helpful or Destructive yet.");
            return; // Don't declare a win yet
        }

        // Determine the winning side
        if (hasHelpful && !hasDestructive)
        {
            // Helpful side wins
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Helpful);
        }
        else if (hasDestructive && !hasHelpful)
        {
            // Destructive side wins
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Destructive);
        }
        else if (!hasHelpful && !hasDestructive && oldManPlayer != null && oldManPlayer.dead)
        {
            // If both sides are gone and Old Man is dead, declare a win for Old Man
            photonView.RPC("WinGame", RpcTarget.All, (int)GameManager.Alignment.Neutral);
        }
        else
        {
            // Additional case to handle when both Helpful and Destructive are alive, or none are alive
            Debug.Log("No clear winner yet.");
        }
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
        for (int i = poisonedPlayers.Count - 1; i >= 0; i--) 
        {
            var effect = poisonedPlayers[i];
            if (effect.nightsRemaining > 0)
            {
                Debug.Log($"Applying poison to {effect.player.photonPlayer.NickName}. Nights remaining: {effect.nightsRemaining}");
                effect.player.photonView.RPC("TakePoisonDamage", RpcTarget.All, 1); // Apply 0.25 damage
                effect.nightsRemaining--;

                // Remove the effect if no nights remain
                if (effect.nightsRemaining == 0)
                {
                    poisonedPlayers.RemoveAt(i); // Safely remove from list
                    Debug.Log($"{effect.player.photonPlayer.NickName} is no longer poisoned.");
                }
            }
        }
    }

}
