using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun
{
    public int maxHearts = 2;  
    public int currentHearts;

    private PlayerHealthUI healthUI;

    void Start()
    {
        // Set maxHearts based on player role
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

    [PunRPC]
    public void TakeDamage(int damage)
    {
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
        Debug.Log($"{photonView.Owner.NickName} has died!");
    }
}