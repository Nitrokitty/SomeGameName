using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class RoamingEnemy : EnemyBase    
{     

    public RoamingEnemy(Stats stats, int health, int damage, float speed, float spawnRate, Rarity rarity, Regions primaryRegion, float playerVisionRadius, bool canWanderThroughRegions)
        : base(stats, health, damage, speed, spawnRate, rarity, primaryRegion)
    {
        PlayerVisionRadius = playerVisionRadius;
        CanWanderThroughRegions = CanWanderThroughRegions;
        CanBegin = false;
        IsAttacking = false;
    }

    public bool CanBegin
    {
        get;
        private set;
    }

    public float CanWanderThroughRegions
    {
        get;
        private set;
    }

    public Vector3 Direction
    {
        get;
        protected set;
    }

    public bool IsAttacking
    {
        get;
        protected set;
    }

    public float PlayerVisionRadius
    {
        get;
        private set;
    }    

    public GameObject TargetPlayer
    {
        get;
        protected set;
    }

    public Camera TargetPlayerCamera
    {
        get;
        protected set;
    }

    public Rect WanderingBounds
    {
        get;
        set;
    }

    public Collider TargetPlayerCollider
    {
        get;
        protected set;
    }

    public void SetRandomDirection(Transform transform)
    {
        RaycastHit hit;
        Ray ray;
        int attempts = 0; 
        do
        {
            attempts++;
            Direction = new Vector3(Random.Next() % 100, 0, Random.Next() % 100);
            Direction = new Vector3(Direction.x * (Random.Next() % 2 == 0 ? -1 : 1), 0, Direction.z * (Random.Next() % 2 == 0 ? -1 : 1));
            ray = new Ray(transform.position, Direction);
        }
        while ((!Physics.Raycast(ray, out hit, 1f) || (hit.transform.gameObject.tag != "Wall" && hit.transform.gameObject.tag != "Base" && WanderingBounds.Contains(ray.GetPoint(2f)) && !TerrainModifier.Grasslands.PointIsInCircle(new Vector2(ray.GetPoint(1f).x, ray.GetPoint(1f).z)))) && attempts < 10);
        var forward = transform.position + transform.forward;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, ((Direction.x/Direction.z)>0? 1 : -1) * Vector3.Angle(forward, ray.direction), transform.rotation.z));
    }

    public void SetRandomStartingPosition(Transform transform)
    {
        var startingPosition = new Vector3(Random.Next() % WanderingBounds.width + WanderingBounds.xMin, 0, Random.Next() % WanderingBounds.height + WanderingBounds.yMin);
        bool keepChecking;
        do
        {
            keepChecking = false;
            foreach (var p in Manager.Players)
            {
                if (Mathf.Abs(startingPosition.x - p.transform.position.x) < PlayerVisionRadius || Mathf.Abs(startingPosition.y - p.transform.position.y) < PlayerVisionRadius || Mathf.Abs(startingPosition.z - p.transform.position.z) < PlayerVisionRadius)
                {
                    keepChecking = true;
                    startingPosition = new Vector3(Random.Next() % WanderingBounds.width + WanderingBounds.xMin, 0, Random.Next() % WanderingBounds.height + WanderingBounds.yMin);
                }
        }
        } while (keepChecking);

        transform.position = new Vector3(startingPosition.x, Terrain.activeTerrain.SampleHeight(startingPosition) + 2f, startingPosition.z);
    }

    public void SetWanderingBounds()
    {
        var regions = GameManager.RegionPositions;

        if (regions.Keys.Contains(PrimaryRegion))
        {
            WanderingBounds = regions[PrimaryRegion];
            CanBegin = true;
        }
    }

    internal Rect GetWanderingBounds()
    {
        return WanderingBounds;
    }

    public override void Move(CharacterController characterController, Transform transform)
    {
        if (IsInVisionOfPlayer(transform))
        {
            IsAttacking = true;
            return;
        }
        
        var newPos = RemoveY(transform.TransformDirection(Vector3.forward));
        newPos *= Speed * Time.deltaTime;
        characterController.SimpleMove(newPos + Gravity);

        var projectedPosition = characterController.velocity + transform.position;
        if (projectedPosition.x < WanderingBounds.xMin || projectedPosition.x > WanderingBounds.xMax || projectedPosition.z < WanderingBounds.yMin || projectedPosition.z > WanderingBounds.yMax || TerrainModifier.Grasslands.PointIsInCircle(new Vector2(projectedPosition.x, projectedPosition.z)))        
            SetRandomDirection(transform);
        
    }

    public bool IsInVisionOfPlayer(Transform transform)
    {
        TargetPlayer = null;
        
        foreach (var p in Manager.Players)
        {
            if(Vector3.Distance(transform.position, p.transform.position) < PlayerVisionRadius)
            {
                TargetPlayer = p;
                TargetPlayerCollider = TargetPlayer.GetComponent<Collider>();
                TargetPlayerCamera = TargetPlayer.GetComponentInChildren<Camera>();
                return true;
            }
        }
        return false;
    }

    public void OnTriggerEnter(Collider collider, Transform transform)
    {
        if(collider.tag == "Base" || collider.gameObject.tag == "Wall")
        {
            IsAttacking = false;
            Health = StartHealth;
            SetRandomDirection(transform);
        }
    }

}
