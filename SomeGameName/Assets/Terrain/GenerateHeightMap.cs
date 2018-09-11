using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateHeightMap {

    string fileName = "generatedHeightMap";
    string extension = "raw";
    float max = .1f;
    float r;

    public GenerateHeightMap(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        r = mapHeight * mapHeight * .33f;
    }

    public GenerateHeightMap(Vector3 mapDimensions)
        :this((int)mapDimensions.x, (int)mapDimensions.y)
    {

    }

    public int MapWidth
    {
        get;
        private set;
    }

    public int MapHeight
    {
        get;
        private set;
    }

    public float[,] GetMap()
    {
        var map = new float[MapHeight, MapWidth];

        for (int i = 0; i < MapHeight; i++)
        {
            for(int j = 0; j < MapWidth; j++)
            {
                map[j, i] = GetValue(j, i);
            }
        }
        return map;
    }

    float GetValue(int x, int y)
    {
        //(x – h)2 + (y – k)2 = r2        
        var leftSide = Mathf.Pow(x - MapWidth * 0.5f, 2);
        var rightSide = Mathf.Pow(y - MapHeight * 0.5f, 2);
        if (leftSide + rightSide <= r)
        {
            var val = (leftSide + rightSide) / r;

            return max-(val * max);
        }
        return 0;
    }
}
