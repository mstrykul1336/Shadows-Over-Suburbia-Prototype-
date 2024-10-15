using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect
{
    public PlayerController player;
    public int nightsRemaining;

    public PoisonEffect(PlayerController player, int duration)
    {
        this.player = player;
        this.nightsRemaining = duration;
    }
}
