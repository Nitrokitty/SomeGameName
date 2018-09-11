using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Scorpion : MonoBehaviour
{
    public bool canWanderThroughRegions = false;
    public int damage = 5;
    public int health = 10;
    public float playerAttackRadius = 5f;
    public float playerRadiusVision = 5f;
    public float spawnRate = 5f;
    public float speed = 5f;
    public float deathTimer = 2.5f;
   

    bool oldCanBegin = false;
    bool timedDestroySet = false;
    CharacterController characterController;
    Texture2D healthBarFull;
    Texture2D healthBarEmpty;
    ScorpionObject scorpionObject;
    Collider collider;
    HealthBar healthBar;
    Vector3 deathDirection;
    Vector3 currentDeathDirection = Vector3.zero;
    Quaternion deathStartRotation;
    Quaternion currentDeathRotation;
    Quaternion deathTargetRotation;
    float deathStartTime = -1;

    void Awake()
    {
        
        characterController = GetComponent<CharacterController>();
        collider = GetComponent<BoxCollider>();
        healthBarFull = (Texture2D)Resources.Load("fullHealth");
        healthBarEmpty = (Texture2D)Resources.Load("emptyHealth");
       
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.totalHealth = health;
        healthBar.enabled = false;

        var properties = GetComponent<EnemySetupProps>();
        scorpionObject = new ScorpionObject(GetComponentInParent<Stats>(), health, damage, speed, canWanderThroughRegions, spawnRate, playerRadiusVision, playerAttackRadius, properties.rarity, properties.primaryRegion);
    }

    void Update()
    {
        if(!scorpionObject.IsAlive)
        {
            if (deathStartTime < 0)
                OnDeath();
            else
                DeathTransform();
            return;
        }

       

       if(!oldCanBegin && scorpionObject.CanBegin)
        {
            scorpionObject.SetRandomDirection(transform);
            scorpionObject.SetRandomStartingPosition(transform);
        }
        if (scorpionObject.CanBegin && scorpionObject.IsAlive && Manager.CurrentState == States.GameIsGoing)
        {
            if (!scorpionObject.IsAttacking)
                scorpionObject.Move(characterController, transform);
            else
            {
                if (!healthBar.isActiveAndEnabled)
                    healthBar.enabled = true;
                healthBar.currentHealth = scorpionObject.Health;
                scorpionObject.Attack(characterController, transform);
            }
        }
        oldCanBegin = scorpionObject.CanBegin;

        scorpionObject.Update(transform);

        //DEBUG
        if(Manager.DEBUG && scorpionObject.CanBegin)
        {
            scorpionObject.Speed = speed;
            scorpionObject.AttackStartingRange = playerAttackRadius;
            healthBar.totalHealth = health;
        }
    }

    //public void OnGUI()
    //{
    //    if (scorpionObject.CanBegin && scorpionObject.IsAlive && ScorpionObject.GameManager.GameIsGoing)
    //    {
    //        if (scorpionObject.IsAttacking)
    //            scorpionObject.OnGUIAttack(transform, collider);
    //    }
    //}

    public void SetWanderingBounds()
    {
        scorpionObject.SetWanderingBounds();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!scorpionObject.IsAlive || Manager.CurrentState != States.GameIsGoing)
            return;
       
        if (collider.gameObject.tag == "Player")
        {
            
            scorpionObject.IsRunningAtPlayer = false;
            collider.gameObject.GetComponent<Stats>().TakeDamage(scorpionObject.Damage);
        }
        else if (collider.gameObject.tag == "Weapon")
        {
            scorpionObject.IsRunningAtPlayer = false;
            if (collider.gameObject.name.ToLower().Contains("sword"))
            {
                SwordAbilities swordAbilities = collider.gameObject.GetComponent<SwordAbilities>();
                if (swordAbilities.State == SwordAbilities.SwordState.Resting)
                    collider.GetComponentInParent<Stats>().TakeDamage(scorpionObject.Damage);
                else if (swordAbilities.State != SwordAbilities.SwordState.Defending && swordAbilities.State != SwordAbilities.SwordState.ReturnDefending)
                    scorpionObject.TakeDamage(swordAbilities.Damage);
            }
        } else if(collider.gameObject.tag == "Enemy"){
            scorpionObject.SetRandomDirection(transform);
        }

        scorpionObject.OnTriggerEnter(collider, transform);

        if (!scorpionObject.IsAlive)
        {
            deathDirection = GetComponent<BoxCollider>().ClosestPoint(collider.ClosestPoint(transform.position));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        scorpionObject.OnTriggerEnter(collision.collider, transform);
    }

    private void OnDestroy()
    {        
        scorpionObject.DropItem(transform);
    }


    public void OnDeath()
    {

        deathStartTime = Time.time;
        healthBar.enabled = false;

        currentDeathDirection = Vector3.zero;
        currentDeathRotation = Quaternion.Euler(Vector3.zero);

        deathStartRotation = transform.rotation;
        deathTargetRotation = Quaternion.Euler(deathStartRotation.eulerAngles.x, deathStartRotation.eulerAngles.y, 180);        

        GameObject.Destroy(gameObject, deathTimer);
    }

    public void DeathTransform()
    {        
        var frac = (Time.time - deathStartTime) / (deathTimer < .75f? deathTimer*.5f: .5f);
        characterController.SimpleMove(frac < 1 ? currentDeathDirection.normalized : Vector3.zero);
        transform.rotation = currentDeathRotation;
        currentDeathDirection = Vector3.Lerp(Vector3.zero, deathDirection, frac);
        currentDeathRotation = Quaternion.Lerp(deathStartRotation, deathTargetRotation, frac);
    }
}

public class ScorpionObject : RoamingEnemy
{
   
    public ScorpionObject(Stats stats, int health, int damage, float speed, bool canWanderThroughRegions, float spawnRate, float playerVisionRadius, float attackStartingRange, Rarity rarity, Regions primaryRegion)
        : base(stats, health, damage, speed, spawnRate, rarity, primaryRegion, playerVisionRadius, canWanderThroughRegions)
    {
        IsRunningAtPlayer = false;
        AttackStartingRange = attackStartingRange;
    }

    public float AttackStartingRange
    {
        get;
        set;
    }

    public bool IsRunningAtPlayer
    {
        get;
        internal set;
    }

    public override void Attack(CharacterController characterController, Transform transform)
    {
        if(TargetPlayer == null || TargetPlayerCollider == null)
        {
            IsAttacking = false;
            return;
        }        
        
        transform.rotation = Quaternion.Euler(RemoveX(Quaternion.LookRotation(TargetPlayer.transform.position - transform.position).eulerAngles));
        var distance = Vector3.Distance(TargetPlayer.transform.position, transform.position);

        if (!IsRunningAtPlayer && distance <= AttackStartingRange)
        {
            var newPos = RemoveY(transform.TransformDirection(-Vector3.forward));

            newPos *= Speed * Time.deltaTime;
            characterController.SimpleMove(newPos + Gravity);           
        } else
        {
            IsRunningAtPlayer = true;
            var newPos = RemoveY(transform.TransformDirection(Vector3.forward));

            newPos *= Speed * Time.deltaTime * 3f;
            characterController.SimpleMove(newPos + Gravity);
        }

    }

    public void Update(Transform transform)
    {
        
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, Vector3.up), out hit, 5f) && hit.transform.gameObject.tag == "Base")
        {
            IsAttacking = false;
            SetRandomDirection(transform);

        }
    }

    public void OnGUIAttack(Transform transform, Collider collider)
    {
        //DrawHealthBar(TargetPlayerCamera, transform, collider);
    }

    public override void DropItem(Transform transform)
    {
        var probabilityOfRare = Rarity == Rarity.Common ? .05f : Rarity == Rarity.Uncommon ? .45f : 1f;
        var resource = Manager.GetResource(PrimaryRegion, probabilityOfRare);
        if(resource == null)
            Debug.Log("No suitable resource to drop found");
        else
            GameObject.Instantiate(resource, new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + transform.position.y, transform.position.z), Quaternion.Euler(Vector3.zero));                
    }
}
