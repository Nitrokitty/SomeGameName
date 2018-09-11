using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour {

    public int BaseDamage = 0;
    public List<Effects> Resistances = new List<Effects>();
    public List<Effects> DamageEffects = new List<Effects>();
    public int StrengthModifier = 0;
    public int HealthModifier = 0;

    GameObject player;
    bool playerStatsSet = false;

    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        if (playerStatsSet)
            return;
		if(player == null)
        {
            if (!TrySetPlayer())
                return;
            var stats = player.GetComponent<Stats>();
            stats.Strength += StrengthModifier + BaseDamage;
            stats.StartingHealth += HealthModifier;
            
            foreach (var r in Resistances)
                stats.AddResistance(r);

            foreach (var e in DamageEffects)
                stats.AddDamageEffect(e);

            playerStatsSet = true;
        }        
	}

    bool TrySetPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
            return false;
        foreach (var p in players)
            if (p.GetComponent<Movement>().isLocalPlayer)
                player = p;
        return player != null;
            
    }
}
