using UnityEngine;
using TMPro; // Include the TextMeshPro namespace
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerList : MonoBehaviourPunCallbacks
{
    public GameObject playerListEntryPrefab; // Assign the player entry prefab in the Inspector
    public Transform playerListContainer; // The parent object to hold all player entries

    private void Start()
    {
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        // Clear existing entries
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        // Get the players in the room
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];

            // Instantiate a new entry for each player
            GameObject entry = Instantiate(playerListEntryPrefab, playerListContainer);

            // Get references to the TextMeshPro components
            TMP_Text playerNameText = entry.transform.Find("PlayerNameText").GetComponent<TMP_Text>();
            TMP_Text playerNumberText = entry.transform.Find("PlayerNumberText").GetComponent<TMP_Text>();

            // Set the player name
            string displayName = player.NickName;

            // Check if the player is the Mayor and add the special character
            if (RoleAssigner.playerRoles.TryGetValue(player.ActorNumber, out RoleAssigner.Role role) && role == RoleAssigner.Role.Mayor)
            {
                displayName += " \u2605"; // Use your special character here
            }

            playerNameText.text = displayName;

            // Set the player number (1-based index)
            playerNumberText.text = (i + 1).ToString(); // Display the player number (1, 2, 3, ...)
        }
    }
}
