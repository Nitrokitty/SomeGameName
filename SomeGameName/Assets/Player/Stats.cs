using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stats : MonoBehaviour {

    
    public int CurrentHealth = 100;
    public int StartingHealth = 100;
    public float Armor = 0;
    public int Strength = 10;
    Dictionary<Effects, int> Resistances;
    Dictionary<Effects, int> DamageEffects;
    public Dictionary<Effects, float> ResistancePercentages;
    Dictionary<Effects, float> DamageEffectsPercentages;
    public GUIStyle style;
    Dictionary<EquipmentType, List<Effects>> equiptedEffects;
    Dictionary<EquipmentType, float> equiptedArmor;


    public int xOffset, yOffset;
    int damageToShow = 0;
    bool showDamage = false;
    float startTime;
    float duration = 1;

    // Use this for initialization
    void Start () {
        Resistances = new Dictionary<Effects, int>();
        DamageEffects = new Dictionary<Effects, int>();
        ResistancePercentages = new Dictionary<Effects, float>();
        DamageEffectsPercentages = new Dictionary<Effects, float>();
        equiptedEffects = new Dictionary<EquipmentType, List<Effects>>();
        equiptedArmor = new Dictionary<EquipmentType, float>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddEquiptment(EquipmentType equiptment, List<CraftedResourcesType> resournces)
    {
        var effects = new List<Effects>();
        var armor = 0f;
        foreach(var r in resournces)
        {           
            switch(r)
            {
                case CraftedResourcesType.S_304:
                    armor+=2;                    
                    break;
                case CraftedResourcesType.S_316:
                    effects.Add(Effects.Corrosion);
                    armor += 1.5f*2;
                    break;
                case CraftedResourcesType.S_316Ti:
                    effects.Add(Effects.Heat);
                    armor += 1.5f*2;
                    break;
                case CraftedResourcesType.S_430:
                    effects.Add(Effects.Cold);
                    armor += 1.5f*2;
                    break;
                case CraftedResourcesType.S_440C:
                    armor += 2f*2;
                    break;
            }
        }

        equiptedEffects.Add(equiptment, effects);
        equiptedArmor.Add(equiptment, armor);
        Debug.Log(armor);
        Armor += armor;
        foreach (var r in effects)
            AddResistance(r);
    }

    public void RemoveEquiptemnet(EquipmentType equiptment)
    {
        if (!equiptedArmor.ContainsKey(equiptment))
            return;

        Armor -= equiptedArmor[equiptment];
        foreach (var r in equiptedEffects[equiptment])
            RemoveResistance(r);
        equiptedArmor.Remove(equiptment);
        equiptedEffects.Remove(equiptment);
    }

    public List<EquipmentType> GetEquiptment()
    {
        if (equiptedArmor.Count == 0)
            return new List<EquipmentType>();
        return equiptedArmor.Keys.ToList();
    }

    public void CalculateDamageEffects()
    {
        DamageEffectsPercentages.Clear();
        foreach (var r in DamageEffects.Keys)
        {
            var value = DamageEffects[r];
            if (value == 1)
                DamageEffectsPercentages.Add(r, 1.1f);
            else
                DamageEffectsPercentages.Add(r, (.01f * Mathf.Floor((float)(10f*(1f+value*0.415f))))+1);
        }
    }

    public void CalculateResistances()
    {
        ResistancePercentages.Clear();
        foreach (var r in Resistances.Keys)
        {
            var value = Resistances[r];
            if (value == 1)
                ResistancePercentages.Add(r, .9f);
            else
                ResistancePercentages.Add(r, 1 - (.01f * Mathf.Floor((float)(10f * (1f + value * 0.415f)))));
        }
    }

    public void AddResistance(Effects effect)
    {
        if (Resistances.ContainsKey(effect))
            Resistances[effect]++;
        else
            Resistances.Add(effect, 1);

        CalculateResistances();
    }

    public void RemoveResistance(Effects effect)
    {
        if (Resistances.ContainsKey(effect))
        {
            Resistances[effect]--;      
            if (Resistances[effect] == 0)
                Resistances.Remove(effect);
        }

        CalculateResistances();
    }

    public void AddDamageEffect(Effects effect)
    {
        if (DamageEffects.ContainsKey(effect))
            DamageEffects[effect]++;
        else
            DamageEffects.Add(effect, 1);

        CalculateDamageEffects();
    }

    public void RemoveDamageEffect(Effects effect)
    {
        if (DamageEffects.ContainsKey(effect))
        {
            DamageEffects[effect]--;
            if (DamageEffects[effect] == 0)
                DamageEffects.Remove(effect);
        }
        CalculateDamageEffects();
    }

    public int GetDamage()
    {
        return Strength;
    }

    public Dictionary<Effects, int> GetEffectDamage()
    {
        var damage = new Dictionary<Effects, int>();
        if (DamageEffectsPercentages.Count == 0)
            return damage;
        foreach(var e in DamageEffectsPercentages.Keys)
        {
            damage.Add(e, (int)Mathf.Floor(DamageEffectsPercentages[e] * Strength));
        }

        return damage;
    }

    public void TakeDamage(int damage)
    {
        startTime = Time.time;
        showDamage = true;
        damageToShow = (int)(damage * (1 - (Armor / 10f)));
        Debug.Log("DMG: " + (damage * (1 - (Armor / 10f))));
        CurrentHealth -= damage;
    }

    public void TakeDamage(int damage, Effects effect)
    {
        if (!Resistances.ContainsKey(effect))
            TakeDamage(damage);
        else
        {
            CurrentHealth -= ((int)Mathf.Ceil(ResistancePercentages[effect] * damage));
        }
    }


    private void OnGUI()
    {
        if (!showDamage)
            return;

        if(Time.time < startTime + duration)
        {
            GUI.Box(new Rect(xOffset, yOffset, 10, 10), "-" + damageToShow.ToString(), style);
        } else
        {
            showDamage = false;
        }
    }
}
