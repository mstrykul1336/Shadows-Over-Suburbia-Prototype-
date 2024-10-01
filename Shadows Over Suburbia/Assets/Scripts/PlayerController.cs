using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    public Player photonPlayer;
    public RoleAssigner.Role playerRole { get; private set; }
    public RoleAssigner.Alignment playerAlignment { get; private set; }
    public bool dead;

    public int maxHearts = 2;  
    public int currentHearts;

    private PlayerHealthUI healthUI;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LocalPlayer.TagObject = this.gameObject; // Where this script is attached to the player GameObject

        SetHealthByRole();
        currentHearts = maxHearts;
        healthUI = GetComponentInChildren<PlayerHealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateHealthUI();
        }
    }

    void SetHealthByRole()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        string role = GetComponent<PlayerController>().GetRole();  
        switch (role)
        {
            case "Mayor":
                maxHearts = 4;
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
         
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;
    }
  
    // This method allows RoleAssigner to set the player's role and alignment
    public void SetRoleAndAlignment(RoleAssigner.Role role, RoleAssigner.Alignment alignment)
    {
        playerRole = role;
        playerAlignment = alignment;

        // Handle any other logic based on role and alignment if necessary
        Debug.Log($"Player Role: {playerRole}, Alignment: {playerAlignment}");
    }
    public string GetRole()
    {
        return playerRole.ToString();
    }

    public void ToDieDebug()
    {
        TakeDamage(4);
    }

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

    void HandlePlayerDeath()
    {
        // Handle what happens when a player dies
        dead = true;
        Debug.Log($"{photonView.Owner.NickName} has died!");
        GameManager.instance.NotifyPlayerDeath(photonView.Owner.ActorNumber);
    }

    [PunRPC]
    void ReceiveAttack(int damageAmount)
    {
        Debug.Log("Recieving attack");
        // This RPC should be called on the target player's controller
        TakeDamage(damageAmount);
    }

}
