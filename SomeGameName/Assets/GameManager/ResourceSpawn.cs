using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;

public class ResourceSpawn : MonoBehaviour
{

    List<QueueNode> spawnQueue;
    public List<GameObject> resourceObjectPrefabs;

    // Use this for initialization
    void Start()
    {
        spawnQueue = new List<QueueNode>();

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnQueue.Count == 0)
            return;

        int i = 0;
        while (i < spawnQueue.Count)
        {
            spawnQueue[i].Timer -= Time.deltaTime;
            //Debug.Log(spawnQueue[i].Timer);
            if (spawnQueue[i].Timer <= 25)
            {
                var resc = Instantiate(spawnQueue[i].Object);
                var spawnPoint = ResourceBase.GetRandomSpawnPoint();
                spawnPoint = new Vector3(spawnPoint.x, 2 + Terrain.activeTerrain.SampleHeight(spawnPoint), spawnPoint.z);
                resc.transform.position = spawnPoint;
                spawnQueue.Remove(spawnQueue[i]);
            }
            else
                i++;
        }

    }


    public void Respawn<T>(T resource)
        where T : ResourceBase
    {
        foreach (var r in resourceObjectPrefabs)
        {            
            if (r.name == resource.Type.ToString())
            {
                var rescBase = r.GetComponent<ResourceBase>();
                Debug.Log("Spawn Rate: " + resource.GetSpawnRate(resource.PrimaryRegion));
                spawnQueue.Add(new QueueNode(r, resource, (double)resource.GetSpawnRate(resource.PrimaryRegion)));
                break;
            }
        }
    }
}


public class QueueNode
{
    public QueueNode(GameObject gameObject, ResourceBase oldObject, double timer)
    {
        Object = gameObject;
        Timer = timer;
        OldObject = oldObject;
    }

    public GameObject Object
    {
        get;
        private set;
    }

    public ResourceBase OldObject
    {
        get;
        private set;
    }

    public double Timer
    {
        get;
        set;
    }
}
