using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public Transform[] spawnPoints;
    private int playersInGame;
    public static GameManager instance;
    public float timerDuration = 15f;
    public TextMeshProUGUI timerText;
    public float timeRemaining;
    public PlayerController[] players;
    public float postGameTime = 20f;
    


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
    public GameObject nightOptionsCanvasPrefab; 
    private GameObject nightOptionsCanvas;
    //public PlayerAttackUI playerattackUI;
    public List<GameObject> playerObjects = new List<GameObject>();

    public TextMeshProUGUI winMessageText;
    public TextMeshProUGUI deathMessageText; 


    
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
    
   
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        Debug.Log("Spawning player.");
        //playerObjects[PhotonNetwork.LocalPlayer.ActorNumber] = playerObj;
       // playerObj.name = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";
        //PhotonNetwork.LocalPlayer.TagObject = playerObj;
        playerObjects.Add(playerObj);
        playerObj.name = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}"; 
        // initialize the player
        playerObj.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    GameObject GetPlayerObject(int index)
    {
        if (index >= 0 && index < playerObjects.Count)
        {
            return playerObjects[index]; // Access player by index
        }
        else
        {
            Debug.LogError($"Player at index {index} not found.");
            return null; // Return null if index is out of range
        }
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

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     Debug.Log($"{newPlayer.NickName} has entered the room.");
    //     photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    // }

    void Update()
    {
         // Only update the timer if voting is active
        if (isNightCycle)
        {
            return; // Skip updates during night cycle
        }

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
    }
    void StartVoting()
    {
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

            // Start a delayed call to make some time for votes
            Invoke(nameof(EndVoting), 15f);
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
        players = new PlayerController[PhotonNetwork.PlayerList.Length]; // Refresh player list
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
        Vector3 canvasPosition = new Vector3(0, 0, 0); 
        Quaternion canvasRotation = Quaternion.identity;
        nightOptionsCanvas = PhotonNetwork.Instantiate(nightOptionsCanvasPrefab.name,canvasPosition, canvasRotation, 0 );
        nightOptionsCanvas.transform.SetParent(PhotonNetwork.LocalPlayer.TagObject as Transform, false);
        // AttackManager attackManager = GetComponent<AttackManager>();
        // if (attackManager == null)
        // {
        //     Debug.LogError("AttackManager component not found on this GameObject.");
        //     return; // Exit if AttackManager is not found
        // }
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerController controller = GetComponent<PlayerController>();
            AttackManager attackManager = GetComponent<AttackManager>();
            attackManager.SetupPlayerUI(controller);
            // int playerIndex = player.ActorNumber - 1; // Assuming ActorNumber starts from 1
            // GameObject playerObject = GetPlayerObject(playerIndex);
            // if (playerObject != null)
            // {
            //     PlayerController controller = playerObject.GetComponent<PlayerController>();
            //     AttackManager attackManager = GetComponent<AttackManager>();
            //     attackManager.SetupPlayerUI(controller);
            // }
            // else
            // {
            //     Debug.Log($"Error: Could not find GameObject for player {player.ActorNumber}");
            // }
            //GameObject playerObject = player.TagObject as GameObject; // Get the player's GameObject
            // if (playerObject != null)
            // {
            //     // PlayerController controller = playerObject.GetComponent<PlayerController>();
            //     // AttackManager attackManager = GetComponent<AttackManager>();
            //     // attackManager.SetupPlayerUI(controller); // Setup UI for each player
            //     PlayerController controller = playerObject.GetComponent<PlayerController>();
            //     if (controller != null)
            //     {
            //         attackManager.SetupPlayerUI(controller); // Setup UI for each player
            //     }
            //     else
            //     {
            //         Debug.LogError($"PlayerController not found for player {player.NickName} (#{player.ActorNumber})");
            //     }
            // }
            // else
            // {
            //     Debug.LogError($"Player {player.NickName} (#{player.ActorNumber}) has a null TagObject.");
            // }
            // if (playerObjects.TryGetValue(player.ActorNumber, out GameObject playerObject))
            // {
            //     PlayerController controller = playerObject.GetComponent<PlayerController>();
            //     AttackManager attackManager = GetComponent<AttackManager>();
            //     attackManager.SetupPlayerUI(controller); // Setup UI for each player
            // }
            // else
            // {
            //     Debug.LogError($"Player {player.ActorNumber} not found in playerObjects.");
            // }
            
        }
        
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (localPlayer != null)
        {
            localPlayer.DisablePlayerControls();
        }
       
        Invoke(nameof(StartDayCycle), 30f);

    }

    void StartDayCycle()
    {
        isNightCycle = false;
        var localPlayer = FindObjectOfType<FirstPersonController>();
        if (localPlayer != null)
        {
            localPlayer.EnablePlayerControls();
        }
        if (nightOptionsCanvas != null)
        {
            PlayerAttackUI playerAttackUI = nightOptionsCanvas.GetComponent<PlayerAttackUI>();
            if (playerAttackUI != null)
            {
                playerAttackUI.gameObject.SetActive(false); // Hide Attack UI
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
        votedOutPlayerIndex = -1;

        for (int i = 0; i < votes.Length; i++)
        {
            if (votes[i] > maxVotes)
            {
                maxVotes = votes[i];
                votedOutPlayerIndex = i;
            }
        }

        if (maxVotes > 0) // If there were any votes
        {
            Debug.Log($"Player {votedOutPlayerIndex} has been voted out with {maxVotes} votes!");
            RevealRoleAndRemovePlayer(votedOutPlayerIndex);
        }
        CheckWinCondition();
        votesProcessed = true;

    }
    void RevealRoleAndRemovePlayer(int playerIndex)
    {
        Debug.Log("Revealing player and role");
        PlayerController targetPlayer = players[playerIndex];
        // Here you need to get the role of the player and display it
        string role = players[playerIndex].GetRole(); 
        roleRevealText.text = $"Player {playerIndex + 1} was voted out! Their role was: {role}";
        PhotonNetwork.Destroy(targetPlayer.gameObject);
       
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
            PhotonNetwork.Destroy(playerController.gameObject);
        }
    }


    [PunRPC]
    void WinGame(int winningSide)
    {
        if (winningSide == (int)RoleAssigner.Alignment.Helpful)
        {
            // Handle Helpful win (e.g., show a message, transition to end screen)
            Debug.Log("Helpful side has won!");
            winMessageText.text = "Helpful won!";

        }
        else if (winningSide == (int)RoleAssigner.Alignment.Destructive)
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
            if (RoleAssigner.playerAlignments.TryGetValue(player.ActorNumber, out RoleAssigner.Alignment alignment))
            {
                if (alignment == RoleAssigner.Alignment.Helpful)
                {
                    hasHelpful = true;
                }
                else if (alignment == RoleAssigner.Alignment.Destructive)
                {
                    hasDestructive = true;
                }
            }
        }

        // Determine the winning side
        if (hasHelpful && !hasDestructive)
        {
            // Helpful side wins
            photonView.RPC("WinGame", RpcTarget.All, (int)RoleAssigner.Alignment.Helpful);
        }
        else if (hasDestructive && !hasHelpful)
        {
            // Destructive side wins
            photonView.RPC("WinGame", RpcTarget.All, (int)RoleAssigner.Alignment.Destructive);
        }
    }
    

}
