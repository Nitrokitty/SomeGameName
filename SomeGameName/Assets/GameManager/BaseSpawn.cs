using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseSpawn  {

    System.Random rand;
    Terrain terrain;
    TerrainData tData;
    Object baseLock = new Object();
    
    public static Dictionary<Regions, GameObject> SpawnedBases;
    static Dictionary<GameObject, Bounds> baseBouonds;

    public BaseSpawn()
    {
        rand = new System.Random();
        terrain = Terrain.activeTerrain;
        tData = terrain.terrainData;
        if(SpawnedBases == null)
        {
            lock (baseLock)
            {
                SpawnedBases = new Dictionary<Regions, GameObject>();
                baseBouonds = new Dictionary<GameObject, Bounds>();
                for (int i = 0; i < Manager.BasePrefabs.Count; i++)
                {
                    var currBase = Manager.BasePrefabs[i];
                    var obj = GameObject.Instantiate(currBase);
                    baseBouonds.Add(Manager.BasePrefabs[i], obj.GetComponent<BoxCollider>().bounds);
                    GameObject.Destroy(obj);
                }
            }
        }
    }

    public GameObject SpawnBase(Rect bounds)
    {
        var test = false;
        GameObject obj;
        do
        {
            //var x = rand.Next() % tData.bounds.size.x;
            //var z = rand.Next() % tData.bounds.size.z;
            var x = rand.Next() % (bounds.width -7) + bounds.xMin + 7;
            var z = rand.Next() % (bounds.height -7) + bounds.yMin + 7;
            Vector3 position = new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, z)), z);



            Bounds biggestBase;
            float maxDimension = -1;
            float currentDimenstion;

            //foreach (var currentBounds in baseBouonds)
            //{           
            //    if ((currentDimenstion = GetBiggestDimension(currentBounds.extents)) > maxDimension)
            //    {
            //        maxDimension = currentDimenstion;
            //        biggestBase = currentBounds;
            //    }
            //}

            //var zone = new Cicle(maxDimension, new Vector2(position.x, position.z));

            //float maxHeightInArea = -1;        
            //Vector3 maxPointInArea = Vector3.zero;
            //Vector3 closestMinPointInArea = Vector3.zero;
            //float minDistance = 1000000;
            //float maxDistance = 1000000;

            var height = terrain.SampleHeight(new Vector3(x, 0, z));

            //for (int j = 0; j < maxDimension; j++)
            //{
            //    for (int i = 0; i < maxDimension; i++)
            //    {

            //        var height = terrain.SampleHeight(new Vector3(i, 0, j));
            //        var currentPoint = new Vector3(i, height, j);
            //        if (height > maxHeightInArea)
            //        {
            //            if (Vector3.Distance(currentPoint, position) < maxDistance)
            //                maxDistance = Vector3.Distance(currentPoint, position);
            //            maxHeightInArea = height;
            //            maxPointInArea =currentPoint;
            //        } else if(height  == 0)
            //        {
            //            if (Vector3.Distance(currentPoint, position) < minDistance)
            //                minDistance = Vector3.Distance(currentPoint, position);

            //        } 
            //    }
            //}

            GameObject baseToSpawn;

            if (height >= RegionBase.TerrainMaxHeight * tData.heightmapHeight * .5)
                baseToSpawn = Manager.BasePrefabs.FirstOrDefault(p => p.name.ToLower().Contains("platform"));
            else
                baseToSpawn = Manager.BasePrefabs.FirstOrDefault(p => p.name.ToLower().Contains("flat"));

            if (baseToSpawn == null)
                baseToSpawn = Manager.BasePrefabs.First();

            //if()

            obj = GameObject.Instantiate(baseToSpawn, position, Quaternion.Euler(new Vector3(0, rand.Next() % 360, 0)));

            var testBounds = obj.GetComponent<BoxCollider>().bounds;
            test = testBounds.max.x > tData.bounds.max.x || testBounds.max.z > tData.bounds.max.z || testBounds.min.x < tData.bounds.min.x || testBounds.min.z < tData.bounds.min.z;
            if (test)
            {
                GameObject.Destroy(obj);                
            }
        } while (test);

        //if (maxHeightInArea == minDistance)
        //{
        //    CreateBase(position);
        //    return;
        //} if(minDistance < maxDistance) {

        //}
        return obj;
    }

    void CreateBase(Vector3 position)
    {

    }

    float GetBiggestDimension(Vector3 vec)
    {       
        return vec.x < vec.z? vec.z : vec.x;
    }
}
