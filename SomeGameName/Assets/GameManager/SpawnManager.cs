using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour {
    static Vector3 currentPosition;

    public static void SpawnTrees(Dictionary<Regions, Rect> regionPositions)
    {
        List<TreeBase> treeClasses = new List<TreeBase>();
        foreach (var r in regionPositions.Keys)
        {
            treeClasses.Add(new TreeBase(regionPositions[r], r, r == Regions.Desert ? RatingScale.Two : RatingScale.Eight));
        }

        foreach (var t in treeClasses)
            t.SpawnTrees();
    }

    public static void SpawnWalls()
    {
        var wall = Resources.LoadAll<GameObject>("Buildings/Wall").First(w => w.GetType().Name.Contains("GameObject"));
        var terrain = Terrain.activeTerrain;
        var tData = terrain.terrainData;

        var parent = GameObject.Find("All Walls").transform;

        var terrainLength = tData.size.z;
        Vector3 position;
        GameObject currentObj;
        Vector3 rotateRight = new Vector3(0, 90, 0);
        int j = 0;
        int i = 0;
        while (i < tData.size.x)
        {
            position = new Vector3(i, 0, 0);
            position.y += terrain.SampleHeight(position);
            Instantiate(wall, position, Quaternion.Euler(Vector3.zero), parent);

            position = new Vector3(i, 0, terrainLength);
            position.y += terrain.SampleHeight(position);
            currentObj = Instantiate(wall, position, Quaternion.Euler(Vector3.zero), parent);

            position = new Vector3(0, 0, i);
            position.y += terrain.SampleHeight(position);
            Instantiate(wall, position, Quaternion.Euler(rotateRight), parent);

            position = new Vector3(terrainLength, 0, i);
            position.y += terrain.SampleHeight(position);
            Instantiate(wall, position, Quaternion.Euler(rotateRight), parent);

            var bounds = ((currentObj.GetComponent(typeof(BoxCollider)) as BoxCollider).bounds);

            i += (int)(bounds.size.x);
        }

        position = new Vector3(i, 0, j);
        position.y += terrain.SampleHeight(position);
        Instantiate(wall, position, Quaternion.Euler(Vector3.zero), parent);

        position = new Vector3(i, 0, terrainLength);
        position.y += terrain.SampleHeight(position);
        currentObj = Instantiate(wall, position, Quaternion.Euler(Vector3.zero), parent);

        position = new Vector3(0, 0, i);
        position.y += terrain.SampleHeight(position);
        Instantiate(wall, position, Quaternion.Euler(rotateRight), parent);

        position = new Vector3(terrainLength, 0, i);
        position.y += terrain.SampleHeight(position);
        currentObj = Instantiate(wall, position, Quaternion.Euler(rotateRight), parent);
    }

    public static void SpawnBases(Dictionary<Regions, Rect> regionPositions)
    {
        var rand = new System.Random();
        var baseSpawner = new BaseSpawn();
        var firstBaseRegion = regionPositions.Keys.ToArray()[rand.Next() % regionPositions.Count];
        Manager.Bases.Add(baseSpawner.SpawnBase(regionPositions[firstBaseRegion]));
        Manager.Teams.Add(new Team(global::Teams.Alpha, Manager.Bases.First()));
        Regions secondBaseRegion;
        do
        {
            secondBaseRegion = regionPositions.Keys.ToArray()[rand.Next() % regionPositions.Count];
        } while (secondBaseRegion == firstBaseRegion);

        Manager.Bases.Add(baseSpawner.SpawnBase(regionPositions[secondBaseRegion]));
        Manager.Teams.Add(new Team(global::Teams.Beta, Manager.Bases.Last()));        
    }

    public static void SpawnEnemies(Dictionary<Regions, Rect> regionPositions, int numberOfCommonEnemiesAtOneTime)
    {              
        foreach (var region in regionPositions.Keys)
        {
            for (int i = 0; i < numberOfCommonEnemiesAtOneTime; i++)           
            {
                var enemy = GetCommonEnemy(region);
                if (enemy != null)
                {
                    var obj = Instantiate(enemy, GetNextPosition(), Quaternion.Euler(Vector3.zero));
                    var scorpionCollider = obj.GetComponent<BoxCollider>();

                    foreach (var b in Manager.Bases)
                    {
                        var baseCollider = b.GetComponent<BoxCollider>();
                        if (baseCollider.bounds.Contains(scorpionCollider.bounds.min) || baseCollider.bounds.Contains(scorpionCollider.bounds.max)
                            || baseCollider.bounds.Contains(new Vector3(scorpionCollider.bounds.min.x, scorpionCollider.bounds.min.y, scorpionCollider.bounds.max.z)) || baseCollider.bounds.Contains(new Vector3(scorpionCollider.bounds.max.x, scorpionCollider.bounds.max.y, scorpionCollider.bounds.min.z)))
                        {
                            GameObject.Destroy(obj);
                            i--;
                            break;
                        }
                    }
                }

            }

        }
    }

    static GameObject GetCommonEnemy(Regions region)
    {
        var rand = new System.Random();
        var regionEnemies = new List<GameObject>();
        foreach (var e in Manager.EnemyPrefabs)
        {
            var properties = Manager.GetInactiveCompoent<EnemySetupProps>(e);
            if (properties.primaryRegion == region && properties.rarity == Rarity.Common)
            {
                regionEnemies.Add(e);
            }
        }
        return regionEnemies.Count == 0 ? null : regionEnemies[rand.Next() % regionEnemies.Count];
    }

    static Vector3 GetNextPosition()
    {
        var rand = new System.Random();
        if (currentPosition == null)
        {
            currentPosition = Vector3.zero;
            return currentPosition;
        }

        var i = rand.Next() % 100 * 10;

        if (currentPosition.x < 499)
        {
            currentPosition = new Vector3(currentPosition.x + i, 0, currentPosition.z);
        }
        else
        {
            currentPosition = new Vector3(0, 0, currentPosition.z + i);
        }


        return currentPosition;
    }
}
