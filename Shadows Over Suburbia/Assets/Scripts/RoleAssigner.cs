using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class RoleAssigner : MonoBehaviourPunCallbacks
{
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

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
        }
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
                int randomIndex = Random.Range(0, availableRoles.Count);
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
            int randomIndex = Random.Range(i, roles.Count);
            roles[i] = roles[randomIndex];
            roles[randomIndex] = temp;
        }
    }

    private Alignment RandomizeAlignment()
    {
        int randomValue = Random.Range(0, 2); // 0 or 1
        return (randomValue == 0) ? Alignment.Destructive : Alignment.Helpful;
    }

    [PunRPC]
    private void ReceiveRoleandAlignment(Role role, Alignment alignment)
    {
        Debug.Log($"Assigned Role: {role}, Alignment: {alignment}");
        CreatePlayerCanvas(role, alignment);

        // Notify the PlayerController about the role and alignment
        GameObject playerControllerObj = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        PlayerController playerController = playerControllerObj?.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetRoleAndAlignment(role, alignment);
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

    // Update is called once per frame
    void Update()
    {
    }
}

