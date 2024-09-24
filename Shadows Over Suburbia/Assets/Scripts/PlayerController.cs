using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    public Player photonPlayer;
    public RoleAssigner.Role playerRole { get; private set; }
    public RoleAssigner.Alignment playerAlignment { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
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
}
