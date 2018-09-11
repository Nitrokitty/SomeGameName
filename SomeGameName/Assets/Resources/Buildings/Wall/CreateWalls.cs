using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWalls : MonoBehaviour
{

    
    public void BuildWalls(GameObject wall)
    {
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
}
