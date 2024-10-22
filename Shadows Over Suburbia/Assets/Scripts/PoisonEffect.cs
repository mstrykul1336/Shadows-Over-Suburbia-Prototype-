using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;  

public class PoisonEffect
{
    public PlayerController player;
    public int nightsRemaining;
    private int lastNightCycle;

    public PoisonEffect(PlayerController player, int duration)
    {
       
        this.player = player;
        this.nightsRemaining = duration;
        this.lastNightCycle = GameManager.instance.NightCycles;
    }

    public void ApplyPoison()
    {
        // Apply poison only if a new night has started
        if (GameManager.instance.NightCycles > lastNightCycle)
        {
            lastNightCycle = GameManager.instance.NightCycles;
            nightsRemaining--;

            // Apply poison damage
            player.photonView.RPC("TakePoisonDamage", RpcTarget.All, 1); // Apply 0.25 damage

            Debug.Log($"{player.photonPlayer.NickName} is poisoned! {nightsRemaining} nights remaining.");

            // Remove poison if it has run out
            if (nightsRemaining <= 0)
            {
                GameManager.instance.poisonedPlayers.Remove(this);
                Debug.Log($"{player.photonPlayer.NickName} is no longer poisoned.");
            }
        }
    }
}
