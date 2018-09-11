using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeBase : MonoBehaviour {
    System.Random rand;
    int pSum = 0;
    static Transform parent;


    public TreeBase(Rect spawnBounds, Regions region, RatingScale spawnRate)
    {
        rand = new System.Random();
        SpawnBounds = spawnBounds;
        SpawnRate = spawnRate;
        parent = GameObject.Find("All Trees").transform;
        Prefabs = new Dictionary<GameObject, float>();
        var tempPrefabs = Manager.Trees.Where(t => t.name.ToLower().Contains(region.ToString().ToLower()));
        foreach(var p in tempPrefabs)
        {
            var curr = int.Parse(p.name.Split('_').Last());
            
            Prefabs.Add(p, curr + pSum);

            pSum += curr;
        }
    }

    public RatingScale SpawnRate
    {
        get;
        private set;
    }

    public Rect SpawnBounds
    {
        get;
        private set;
    }

    protected Dictionary<GameObject, float> Prefabs
    {
        get; private set;
    }

    public void SpawnTrees()
    {
        if (SpawnRate == RatingScale.Zero || SpawnRate == RatingScale.One)
            return;

        bool spawnNext = false;
        for(int j = 0; j < SpawnBounds.size.y; j++)
        {
            for(int i = 0; i < SpawnBounds.size.x; i++)
            {
                if(spawnNext || rand.Next()%1000 < (int)SpawnRate)
                {                                       
                    var pos = new Vector3(SpawnBounds.min.x + i, 0, SpawnBounds.min.y + j);
                    pos.y = Terrain.activeTerrain.SampleHeight(pos);

                    var treeObj = GameObject.Instantiate(GetRandomTree(), pos, Quaternion.Euler(Vector3.zero), parent);

                }

            }
        }
    }

    GameObject GetRandomTree()
    {
        GameObject obj = null;
        var tree = rand.Next() % pSum;
        foreach (var p in Prefabs.Keys)
        {
            if (Prefabs[p] > tree)
            {
                break;
            }

            obj = p;
        }

        return obj ?? Prefabs.Keys.First() ;
    }

    
}



