using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class AttackManager : MonoBehaviourPun
{
    public GameObject playerUIPrefab; // Reference to the player UI prefab

    //private List<PlayerController> players = new List<PlayerController>(); // List of PlayerControllers
        private List<GameObject> playerObjects = new List<GameObject>();

    // void Start()
    // {
    //     // Initialize players and their UI
    //     foreach (var player in PhotonNetwork.PlayerList)
    //     {
    //         GameObject playerObject = player.TagObject as GameObject; // Get the player's GameObject
    //         if (playerObject != null)
    //         {
    //             PlayerController controller = playerObject.GetComponent<PlayerController>();
    //             players.Add(controller);

    //             // // Instantiate the player UI prefab and set it up
    //             // GameObject uiInstance = Instantiate(playerUIPrefab);
    //             // PlayerAttackUI playerUI = uiInstance.GetComponent<PlayerAttackUI>();
    //             // Debug.Log("Calling setup");
    //             // playerUI.SetupUI();
    //             // playerUI.Initialize(controller, this);
    //         }
    //         else {
    //             Debug.Log("SDomething is wrong");
    //         }
    //     }
    // }

    void Start()
     {
    //     InitializePlayers();
    }

    public void SetupPlayerUI(PlayerController controller)
    {
        // Instantiate the player UI prefab and set it up for the given controller
        GameObject uiInstance = Instantiate(playerUIPrefab);
        PlayerAttackUI playerUI = uiInstance.GetComponent<PlayerAttackUI>();
        
        if (playerUI != null)
        {
            playerUI.Initialize(controller, this);
            playerUI.SetupUI(); // Only call this when it's the night cycle
        }
    }
    // public void InitializePlayers()
    // {
    //     // Initialize players and their UI
    //     foreach (var player in PhotonNetwork.PlayerList)
    //     {
    //         GameObject playerObject = FindPlayerObject(player.ActorNumber);
    //         if (playerObject != null)
    //         {
    //             playerObjects.Add(playerObject);
    //         }
    //         else
    //         {
    //             Debug.LogError($"Player object for {player.ActorNumber} not found.");
    //         }
    //     }
    // }
    // GameObject FindPlayerObject(int index)
    // {
    //     if (index >= 0 && index < playerObjects.Count)
    //     {
    //         return playerObjects[index]; // Access player by index
    //     }
    //     else
    //     {
    //         Debug.LogError($"Player at index {index} not found.");
    //         return null; // Return null if index is out of range
    //     }
    // }


    public void AttackPlayer(Player targetPlayer)
    {
        // Get the target player's PlayerController by ID
        PlayerController targetController = GetPlayerControllerById(targetPlayer.ActorNumber);
        
        if (targetController != null)
        {
            Debug.Log("Attacking player.");
            // Perform the attack using RPC
            photonView.RPC("ReceiveAttack", targetPlayer, 1); // Call the RPC to deal damage
        }
    }

    PlayerController GetPlayerControllerById(int actorNumber)
    {
        // Loop through all players to find the corresponding PlayerController
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber)
            {
                GameObject playerObject = player.TagObject as GameObject; // Assuming you set the player's GameObject in the Player's TagObject
                if (playerObject != null)
                {
                    return playerObject.GetComponent<PlayerController>(); // Return the PlayerController
                }
            }
        }
        return null; // Not found
    }

    [PunRPC]
    void ReceiveAttack(int damageAmount)
    {
        // This RPC should be called on the target player's controller
        PlayerController targetController = GetComponent<PlayerController>();
        if (targetController != null)
        {
            targetController.TakeDamage(damageAmount); // Call method to reduce health
            Debug.Log($"{photonView.Owner.NickName} attacked {targetController.photonPlayer.NickName} for {damageAmount} damage.");
        }
    }
}
