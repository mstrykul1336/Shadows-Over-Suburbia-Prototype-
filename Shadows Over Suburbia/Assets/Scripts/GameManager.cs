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
    public float postGameTime = 20f;
    public float firstDayTimeRemaining = 30f;



    private bool votingActive = false; 
    private int[] votes;
    private int votedOutPlayerIndex = -1;
    private bool[] hasVoted;
    
    private GameObject playerVotingCanvas;
    public GameObject votingCanvasPrefab;
    public TextMeshProUGUI roleRevealText;
    private bool votesProcessed = false;

    public Transform[] spawnPointsNight;
    private bool isNightCycle = false; 
   // public GameObject nightOptionsCanvasPrefab; 
    //private GameObject nightOptionsCanvas;
    //public PlayerAttackUI playerattackUI;
    //public List<GameObject> playerObjects = new List<GameObject>();

    public TextMeshProUGUI winMessageText;
    public TextMeshProUGUI deathMessageText; 
    public int DayCycles = 0;
    public TextMeshProUGUI dayText;

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
        Helpful
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
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }
        //UpdatePlayerList();
    }



    [PunRPC]
    void ImInGame()
    {
        Debug.Log("Player has joined the game!");
        playersInGame++;
        Debug.Log($"Players in game: {playersInGame}/{PhotonNetwork.PlayerList.Length}");
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("All players are in the game. Spawning player.");
            SpawnPlayer();
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
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }


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
            Alignment randomAlignment = RandomizeAlignment();

            playerRoles[playerId] = assignedRole;
            playerAlignments[playerId] = randomAlignment;

            // Send the role to the player
            photonView.RPC("ReceiveRoleandAlignment", player.Value, assignedRole, randomAlignment);
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
        Debug.Log($"Assigned Role: {role}, Alignment: {alignment}");
        CreatePlayerCanvas(role, alignment);

        // Notify the PlayerController about the role and alignment
        // GameObject playerControllerObj = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        // PlayerController playerController = playerControllerObj?.GetComponent<PlayerController>();
        // // PlayerController playerController = GetComponent<PlayerController>();
        // if (playerController != null)
        // {
        //     playerController.SetRoleAndAlignment(role, alignment);
        // }
        //PlayerController playerController = GetComponent<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                Debug.Log($"Setting role and alignment for player: {player.photonPlayer.NickName}");
                player.SetRoleAndAlignment(role, alignment);
            }
        }
        
        
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
            else
            {
                roleText.color = Color.green;
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
            string optionText = $"{player.NickName} (#{player.ActorNumber})";
            playerDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
        }

        // Refresh the dropdown after adding options
        playerDropdown.RefreshShownValue();
    }

    public void CastVote(int playerIndex)
    {
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

                CheckVoteResults(); // Check if we need to process the voting results
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
        foreach (PlayerController player in players)
        {
            if (photonView.IsMine && player != null)
            {
                player.NightUIOff();
                player.GiveGold();
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


    [PunRPC]
    void WinGame(int winningSide)
    {
        if (winningSide == (int)GameManager.Alignment.Helpful)
        {
            // Handle Helpful win (e.g., show a message, transition to end screen)
            Debug.Log("Helpful side has won!");
            winMessageText.text = "Helpful won!";

        }
        else if (winningSide == (int)GameManager.Alignment.Destructive)
        {
            // Handle Destructive win (e.g., show a message, transition to end screen)
            Debug.Log("Destructive side has won!");
            winMessageText.text = "Destructive won!";
        }

        Invoke("GoBackToMenu", postGameTime);
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
    }
    

}
