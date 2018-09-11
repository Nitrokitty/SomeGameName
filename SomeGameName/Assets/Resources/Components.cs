using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseComponent
{
    ResourceBase additionalResource;

    public BaseComponent(Components type, Dictionary<CraftedResourcesType, int> requires)
    {

        if (!IsValidType(type))
            throw new InvalidOperationException("The given type if crafted resource ('" + type.ToString() + "') is not valid for the casted type");
        else
        {
            Type = type;
            Requires = requires ?? new Dictionary<CraftedResourcesType, int>();
        }
    }

    public BaseComponent(Components type, Dictionary<CraftedResourcesType, int> requires, ResourceBase additionalResource)
        : this(type, requires)
    {
        if (!IsValidAdditionalResource(additionalResource))
            throw new InvalidOperationException("Can not match '" + additionalResource.Name + "' with '" + Name);
        else
            this.additionalResource = additionalResource;
    }

    public virtual string Name
    {
        get { return Type.ToString(); }
    }

    protected List<Components> BuildsInto
    {
        get;
        private set;
    }

    protected Dictionary<CraftedResourcesType, int> Requires
    {
        get;
        private set;
    }

    public Components Type
    {
        get;
        private set;
    }

    protected abstract bool IsValidAdditionalResource(ResourceBase resource);

    protected abstract bool IsValidType(Components type);
}

public class Droid : BaseComponent
{
    ResourceBase additionalResource;

    public Droid(Components type, Dictionary<CraftedResourcesType, int> requires, List<Components> buildsInto, Gauges damage, Gauges health, bool canLeaveBase)
        : base(type, requires)
    {
        Damage = damage;
        Health = health;
        CanLeaveBase = canLeaveBase;
    }

    public Droid(Components type, Dictionary<CraftedResourcesType, int> requires, List<Components> buildsInto, Gauges damage, Gauges health, bool canLeaveBase, ResourceBase additionalResource)
        : base(type, requires, additionalResource)
    {
        Damage = damage;
        Health = health;
        CanLeaveBase = canLeaveBase;
    }

    protected override bool IsValidAdditionalResource(ResourceBase resource)
    {
        switch (resource.Type)
        {
            case ResourceTypes.Lithium:
                return Type == Components.Warden;
            case ResourceTypes.Plasma:
                return Type == Components.Ruroni;
            case ResourceTypes.Titanium:
                return Type == Components.Vigil;
            case ResourceTypes.Nickel:
                return Type == Components.Amigo;
            case ResourceTypes.Molybdenum:
                return Type == Components.Brick;
        }
        return false;
    }

    protected override bool IsValidType(Components type)
    {
        return new List<Components>() { Components.Warden, Components.Ruroni, Components.Vigil, Components.Amigo, Components.Brick }.Contains(type);
    }

    public new string Name
    {
        get { return "Droid: " + base.Name; }
    }

    public Gauges Damage
    {
        get;
        private set;
    }

    public Gauges Health
    {
        get;
        private set;
    }

    public bool CanLeaveBase
    {
        get;
        private set;
    }
}

public class Armor : BaseComponent
{
    Dictionary<Effects, int> resistances;
    int strength = -1;

    public Armor(Components type, Dictionary<CraftedResourcesType, int> requires)
        : base(type, requires)
    {
       
    }

    protected override bool IsValidAdditionalResource(ResourceBase resource)
    {       
        return false;
    }

    protected override bool IsValidType(Components type)
    {
        return new List<Components>() { Components.Chest, Components.Arms, Components.Legs, Components.Head }.Contains(type);
    }

    public new string Name
    {
        get { return "Armor: " + base.Name; }
    }

    public Dictionary<Effects, int> Resistances
    {
        get
        {
            if(resistances == null)
            {
                resistances = new Dictionary<Effects, int>();
                var count = new Dictionary<Effects, int>();

                foreach (var i in Requires)
                {
                    var e = GetEffect(i.Key);
                    if (e != null)
                    {
                        if (count.ContainsKey(e.Value))
                            count[e.Value] = count[e.Value] + 1;
                        else
                            count.Add(e.Value, 1);
                    }
                }
                foreach(var e in count)
                {
                    resistances.Add(e.Key, e.Value == 1 ? 10 : (int)(Math.Floor(10 * (1 + (0.83 * e.Value * .5)))));
                }
            }
            return resistances;
        }
    }

    static Effects? GetEffect(CraftedResourcesType resource)
    {
        switch(resource)
        {
            case CraftedResourcesType.S_316:
                return Effects.Corrosion;
            case CraftedResourcesType.S_316Ti:
                return Effects.Heat;
            case CraftedResourcesType.S_430:
                return Effects.Cold;
        }
        return null;
    }

    public int Strength
    {
        get
        {
            if (strength < 0)
            {
                strength = 0;
                var count = new Dictionary<CraftedResourcesType, int>();

                foreach (var i in Requires)
                {
                    if (count.ContainsKey(i.Key))
                        count[i.Key] = i.Value + 1;
                    else
                        count.Add(i.Key, 1);
                    
                }
                foreach (var e in count)
                {
                    strength += e.Value == 1 ? 10 : (int)(Math.Floor(10 * (1 + (0.83 * e.Value * .5))));
                }
            }
            return strength;
        }

    }
}

public class Weapon : BaseComponent
{
    Dictionary<Effects, int> effects;

    public Weapon(Components type, Dictionary<CraftedResourcesType, int> requires, Gauges attackSpeed, Gauges damage, bool requiresProficiency, bool isTwohanded)
        : base(type, requires)
    {
        RequiresProficiency = requiresProficiency;
        IsTwohanded = isTwohanded;
        AttackSpeed = attackSpeed;
        Damage = damage;
    }

    public bool RequiresProficiency
    {
        get;
        protected set;
    }

    public bool IsTwohanded
    {
        get;
        protected set;
    }

    public Gauges AttackSpeed
    {
        get;
        protected set;
    }

    public Gauges Damage
    {
        get;
        protected set;
    }

    protected override bool IsValidAdditionalResource(ResourceBase resource)
    {
        return false;
    }

    protected override bool IsValidType(Components type)
    {
        return new List<Components>() { Components.Sword, Components.ShortSword, Components.LongSword, Components.PistolGun, Components.PistolGun, Components.Rifle }.Contains(type);
    }

    public new string Name
    {
        get { return "Weapon: " + base.Name + (IsTwohanded? " (2H)":""); }
    }

    public Dictionary<Effects, int> DamageEffects
    {
        get
        {
            if (effects == null)
            {
                effects = new Dictionary<Effects, int>();
                var count = new Dictionary<Effects, int>();

                foreach (var i in Requires)
                {
                    var e = GetEffect(i.Key);
                    if (e != null)
                    {
                        if (count.ContainsKey(e.Value))
                            count[e.Value] = count[e.Value] + 1;
                        else
                            count.Add(e.Value, 1);
                    }
                }
                foreach (var e in count)
                {
                    effects.Add(e.Key, e.Value == 1 ? 10 : (int)(Math.Floor(10 * (1 + (0.83 * e.Value * .5)))));
                }
            }
            return effects;
        }
    }

    static Effects? GetEffect(CraftedResourcesType resource)
    {
        switch (resource)
        {
            case CraftedResourcesType.S_316:
                return Effects.Corrosion;
            case CraftedResourcesType.S_316Ti:
                return Effects.Heat;
            case CraftedResourcesType.S_430:
                return Effects.Cold;
        }
        return null;
    }
}


public enum Components
{
    Chest,
    Arms,
    Legs,
    Head,
    ShortSword,
    Sword,
    LongSword,
    PistolGun,
    Rifle,
    Warden,
    Ruroni,
    Vigil,
    Amigo,
    Brick
}