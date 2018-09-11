using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CraftedResources : MonoBehaviour {

    public CraftedResourcesType type;
    ResourceBase additionalResource;

    void Start()
    {
        Type = type;        
    }
    
    protected CraftedResources(CraftedResourcesType type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto)
    {

        if (!IsValidType(type))
            throw new InvalidOperationException("The given type if crafted resource ('" + type.ToString() + "') is not valid for the casted type");
        else
        {
            Type = type;
            BuildsInto = buildsInto ?? new List<Components>();
            Requires = requires ?? new Dictionary<ResourceTypes, int>();
        }
    }

    protected CraftedResources(CraftedResourcesType type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, ResourceBase additionalResource)
        : this(type, requires, buildsInto)
    {
        if (!IsValidAdditionalResource(additionalResource))
            throw new InvalidOperationException("Can not match '" + additionalResource.Name + "' with '" + Name);
        else
            this.additionalResource = additionalResource;
    }

    public virtual string Name
    {
        get { return Type.ToString().Split('_')[1] + (additionalResource != null ? " ('" + additionalResource.Name + "')" : string.Empty); }
    }

    protected List<Components> BuildsInto
    {
        get;
        private set;
    }

    protected Dictionary<ResourceTypes, int> Requires
    {
        get;
        private set;
    }

    public CraftedResourcesType Type
    {
        get;
        private set;
    }

    public ResourceTypes? AdditionalResource
    {
        get
        {
            if (additionalResource == null)
                return null;
            return additionalResource.Type;
        }
    }

    public string Description
    {
        get;
        set;
    }

    protected virtual bool IsValidAdditionalResource(ResourceBase resource) { return false;  }

    protected virtual bool IsValidType(CraftedResourcesType type) { return false; }

    public static ResourceTypes[] GetRequirements(CraftedResourcesType type)
    {
        var requirements = new List<ResourceTypes>();
        if(type.ToString().ToLower()[0] == 's')
            requirements.AddRange(new ResourceTypes[] { ResourceTypes.Iron, ResourceTypes.Magnesium, ResourceTypes.Chromite });
        else if (type.ToString().ToLower()[0] == 'b')
            requirements.AddRange(new ResourceTypes[] { ResourceTypes.Coal, ResourceTypes.Cobalt });

        switch (type)
        {            
            case CraftedResourcesType.S_316:
                requirements.Add(ResourceTypes.Molybdenum);
                break;
            case CraftedResourcesType.S_316Ti:
                requirements.Add(ResourceTypes.Titanium);
                break;
            case CraftedResourcesType.S_430:
                requirements.Add(ResourceTypes.Nickel);
                break;
            case CraftedResourcesType.S_440C:
                requirements.Add(ResourceTypes.Carbon);
                break;           
            case CraftedResourcesType.B_Lithium:
                requirements.Add(ResourceTypes.Lithium);
                break;
            case CraftedResourcesType.B_Plasma:
                requirements.Add(ResourceTypes.Plasma);
                break;                
        }

        return requirements.ToArray();
    }
}

public class Steel : CraftedResources
{
    ResourceBase additionalResource;

    public Steel(CraftedResourcesType type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, Gauges strength, Effects resistance, Gauges shineyness)
        : base(type, requires, buildsInto)
    {
        Strength = strength;
        Resistance = resistance;
        Shineyness = shineyness;
    }

    public Steel(CraftedResourcesType type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, Gauges strength, Effects resistance, Gauges shineyness, ResourceBase additionalResource)
        : base(type, requires, buildsInto, additionalResource)
    {
        Strength = strength;
        Resistance = resistance;
        Shineyness = shineyness;
    }

    protected override bool IsValidAdditionalResource(ResourceBase resource)
    {
        switch (resource.Type)
        {
            case ResourceTypes.Molybdenum:
                return Type == CraftedResourcesType.S_316;
            case ResourceTypes.Titanium:
                return Type == CraftedResourcesType.S_316Ti;
            case ResourceTypes.Nickel:
                return Type == CraftedResourcesType.S_430;
            case ResourceTypes.Carbon:
                return Type == CraftedResourcesType.S_440C;
        }
        return false;
    }

    protected override bool IsValidType(CraftedResourcesType type)
    {
        return type.ToString().Split('_')[0] == "S";
    }

    public override string Name
    {
        get { return "Steel: "; }
    }

    public Gauges Strength
    {
        get;
        private set;
    }

    public Effects Resistance
    {
        get;
        private set;
    }

    public Gauges Shineyness
    {
        get;
        private set;
    }
}

public class Battery : CraftedResources
{
    ResourceBase additionalResource;

    public Battery(CraftedResourcesType type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, Gauges power, bool isRechargable)
        : base(type, requires, buildsInto)
    {
        Power = power;
        IsRechargable = isRechargable;
    }

    public Battery(CraftedResourcesType type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, ResourceBase additionalResource, Gauges power, bool isRechargable)
        : base(type, requires, buildsInto, additionalResource)
    {
        Power = power;
        IsRechargable = isRechargable;
    }

    protected override bool IsValidAdditionalResource(ResourceBase resource)
    {
        switch (resource.Type)
        {
            case ResourceTypes.Lithium:
                return Type == CraftedResourcesType.B_Lithium;
            case ResourceTypes.Plasma:
                return Type == CraftedResourcesType.B_Plasma;
        }
        return false;
    }

    protected override bool IsValidType(CraftedResourcesType type)
    {
        return type.ToString().Split('_')[0] == "B";
    }

    public new string Name
    {
        get { return "Battery: " + base.Name; }
    }

    public Gauges Power
    {
        get;
        private set;
    }

    public bool IsRechargable
    {
        get;
        private set;
    }
}


public enum EquipmentType
{
    ChestPlate,
    SteelGloves
}

public enum CraftedResourcesType
{
    S_316,
    S_304,
    S_316Ti,
    S_430,
    S_440C,
    B_Lithium,
    B_CarbonZinc,
    B_Plasma
}