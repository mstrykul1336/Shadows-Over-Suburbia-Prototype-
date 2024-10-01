using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerAttackUI : MonoBehaviour
{
    public Button attackButton;         // Reference to your attack button
    public TMP_Dropdown playerDropdown; // Reference to your dropdown for targeting

    private AttackManager attackManager; // Reference to the AttackManager
    private PlayerController playerController; // Reference to this player's controller
    private List<Player> playerList = new List<Player>(); // List to hold current players

    public void Initialize(PlayerController controller, AttackManager manager)
    {
        playerController = controller;
        attackManager = manager;

        attackButton.onClick.AddListener(AttackSelectedPlayer);
    }
    public void SetupUI()
    {
        UpdatePlayerDropdown(); // Populate the dropdown with player options
        gameObject.SetActive(true); // Make sure the UI is active
    }

    void UpdatePlayerDropdown()
    {
       // playerList.Clear(); // Clear the list
        //playerDropdown.ClearOptions(); // Clear existing options

        // Populate the dropdown with current players
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) // Skip the local player
            {
                Debug.Log("Adding stuff to dropdown");
                playerList.Add(player); // Add the player to the list
                string optionText = $"{player.NickName} (#{player.ActorNumber})";
                playerDropdown.options.Add(new TMP_Dropdown.OptionData(optionText)); // Add to dropdown
            }
        }

        playerDropdown.RefreshShownValue(); // Refresh the dropdown to show updated options
    }

    void AttackSelectedPlayer()
    {
        int selectedPlayerIndex = playerDropdown.value;
        Player targetPlayer = PhotonNetwork.PlayerList[selectedPlayerIndex]; // Get the selected player

        // Perform the attack using AttackManager
        attackManager.AttackPlayer(targetPlayer); // Delegate the attack action to AttackManager
    }
}
