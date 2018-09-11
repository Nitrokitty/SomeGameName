using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



public class ResourceBase : MonoBehaviour {

    public readonly int PrimaryRegionAbundance = 75;
    public readonly int OffRegionAbundance = 45;
    public ResourceTypes type;
    public Rarity rarity;
    List<CraftedResourcesType> buildsInto;
    public Regions primaryRegion;
    public Regions currentRegion;
    public int baseSpawnRate = 30;
    Dictionary<Regions, int> abundance;

    void Start()
    {
        Type = type;        
        Rarity = rarity;
        PrimaryRegion = primaryRegion;
        CurrentRegion = currentRegion;
    }

    /// <summary>
    /// For each region, provides/returns an integer value that represents
    /// the abundance of the resource in that particular region. If you do
    /// not provide a region or the abundance is not between 1 and 100,
    /// the defaults will be filled in. 
    /// This is independent of the rarity of the object.
    /// </summary>
    protected Dictionary<Regions, int> Abundance
    {
        get
        {
            if(abundance == null)
            {
                abundance = new Dictionary<Regions, int>();
                foreach (var region in Enum.GetValues(typeof(Regions)))
                {
                    var r = (Regions)region;
                    abundance.Add(r, r == PrimaryRegion ? PrimaryRegionAbundance : OffRegionAbundance);
                }
            }
            return abundance;
        }

        set
        {
            abundance = value;

            foreach(var region in Enum.GetValues(typeof(Regions)))
            {
                var r = (Regions)region;
                if (!abundance.ContainsKey(r))
                {
                    abundance.Add(r, r == PrimaryRegion ? PrimaryRegionAbundance : OffRegionAbundance);
                } else if(abundance[r] < 1 || abundance[r] > 100)
                {
                    abundance[r] = r == PrimaryRegion ? PrimaryRegionAbundance : OffRegionAbundance;
                }
                
            }
        }
    }

    /// <summary>
    /// This is the number of seconds that we should wait before spawining
    /// an item of this type. 
    /// It is dependent on the rarity and region.
    /// </summary>
    public int GetSpawnRate(Regions region)
    {
        return (int)Math.Ceiling((double) (baseSpawnRate * (1 / (((int)Rarity) *.01)) * (region == PrimaryRegion?1:2))); 
    }

    public string Name
    {
        get { return Type.ToString(); }
    }

    public Rarity Rarity
    {
        get;
        private set;
    }

    public static List<EquipmentType> GetBuildsInto(CraftedResourcesType type)
    {
        return Enum.GetNames(typeof(EquipmentType)).Select((s) => (EquipmentType)Enum.Parse(typeof(EquipmentType), s)).ToList();
    }

    public static List<CraftedResourcesType> GetBuildsInto(ResourceTypes type)
    {

            var buildsInto = new List<CraftedResourcesType>();

            switch(type)
            {
                case ResourceTypes.Iron:
                case ResourceTypes.Chromite:
                case ResourceTypes.Magnesium:
                    buildsInto.Add(CraftedResourcesType.S_304);
                    buildsInto.Add(CraftedResourcesType.S_316);
                    buildsInto.Add(CraftedResourcesType.S_316Ti);
                    buildsInto.Add(CraftedResourcesType.S_430);
                    buildsInto.Add(CraftedResourcesType.S_440C);
                    break;
                case ResourceTypes.Coal:
                case ResourceTypes.Cobalt:
                    buildsInto.Add(CraftedResourcesType.B_CarbonZinc);
                    buildsInto.Add(CraftedResourcesType.B_Lithium);
                    buildsInto.Add(CraftedResourcesType.B_Plasma);
                    break;
                case ResourceTypes.Molybdenum:
                    buildsInto.Add(CraftedResourcesType.S_316);
                    break;
                case ResourceTypes.Titanium:
                    buildsInto.Add(CraftedResourcesType.S_316Ti);
                    break;
                case ResourceTypes.Nickel:
                    buildsInto.Add(CraftedResourcesType.S_430);
                    break;
                case ResourceTypes.Carbon:
                    buildsInto.Add(CraftedResourcesType.S_440C);
                    break;
                case ResourceTypes.Lithium:
                    buildsInto.Add(CraftedResourcesType.B_Lithium);
                    break;
                case ResourceTypes.Plasma:
                    buildsInto.Add(CraftedResourcesType.B_Plasma);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return buildsInto;
        
    }

    public ResourceTypes Type
    {
        get;
        private set;
    }

    public Regions PrimaryRegion
    {
        get;
        private set;
    }

    public Regions CurrentRegion
    {
        get;
        private set;
    }

    public Regions GetRandomSpawnRegion()
    {
        var rA = Enum.GetValues(typeof(Regions));
        var rL = new List<Regions>();
        foreach(var r in rA)
        {
            var rCast = (Regions)r;
            if(rCast != PrimaryRegion)
            {
                rL.Add(rCast);
                rL.Add(PrimaryRegion);
            }
        }
        return (Regions)rL.ToArray().GetValue((new System.Random()).Next() % rL.Count);
    }

    public static Vector3 GetRandomSpawnPoint(float y = -1)
    {
        float precision = 1000f;
        var terrain = GameObject.FindGameObjectWithTag("Terrain");
        var terrainComponent = terrain.GetComponent<Terrain>();

        var size = terrainComponent.terrainData.size;
        var min = terrainComponent.terrainData.bounds.min;
        var max = terrainComponent.terrainData.bounds.max;

        min = new Vector3(min.x * precision, min.y * precision, min.z * precision);
        max = new Vector3(max.x * precision, max.y * precision, max.z * precision);
        var diff = max - min;


        var r = new System.Random();

        var randomInts = new int[6];
        for(int i = 0; i < 6; i++)        
            randomInts[i] = r.Next();

        var pos = new Vector3(((randomInts[0] % diff.x) + min.x) * (1f/precision), ((randomInts[0] % diff.y) + min.y) * (1f/precision), ((randomInts[0] % diff.z) + min.z) * (1f/precision));

        if (y > 0)
            pos = new Vector3(pos.x, y, pos.z);

        return pos;
    }

    /// <summary>
    ///  Returns an integer value between 1 and 100 that represents how abundant this
    ///  resource is in the given region based on it's rarity.
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    public int GetAbundance(Regions region)
    {
        return (int) (Abundance[region] * ((int)Rarity/100f));
    }
}

public enum ResourceTypes
{
    Molybdenum,
    Titanium,
    Nickel,
    Carbon,
    Iron,
    Chromite,
    Magnesium,
    Coal,
    Cobalt,
    Lithium,
    Plasma,
    Silicon
}

public enum Rarity
{
    Common = 100,
    Uncommon = 50,
    Rare = 25
}

public enum Regions
{
    Mountains,
    Swamps,
    Coast,
    Desert,
    Forest
}

public enum Gauges
{
    Low,
    Medium,
    High
}

public enum Effects
{
    Corrosion, 
    Heat,
    Cold
}

public enum ObjectType
{
    Resource,
    CraftedResource,
    Component
}