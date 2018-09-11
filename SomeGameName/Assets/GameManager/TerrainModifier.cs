using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TerrainModifier : MonoBehaviour
{

    GenerateHeightMap heightMapGenerator;
    Terrain terrain;
    public Texture2D desertSand;
    public Texture2D grassLand;
    public Texture2D mountainGrey;
    public Texture2D mountainGrass;
    public Texture2D mountainSnow;
    
    public static Cicle Grasslands;

    bool useSingleMap = false;
    void Awake()
    {
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        var length = terrain.terrainData.heightmapResolution;
        var height = terrain.terrainData.heightmapHeight;
        //heightMapGenerator = new GenerateHeightMap(length, length);

        SetTextures();

        var regions = new List<RegionBase>() { new Desert(length, height, Corners.TopRight), new Mountains(length, height, Corners.TopLeft), new Desert(length, height, Corners.BottomLeft), new Desert(length, height, Corners.BottomRight) };
        
        var map = new float[length, length];



        if (useSingleMap)
        {
            var currentMap = regions.First((r) => r.Corner == Corners.TopLeft).GetMap();
            Copy(RotateArrayRight(RotateArrayRight(currentMap)), ref map, Corners.TopRight);
            Copy(RotateArrayLeft(currentMap), ref map, Corners.TopLeft);
            Copy(RotateArrayRight(currentMap), ref map, Corners.BottomRight);
            Copy(currentMap, ref map, Corners.BottomLeft);
        }
        else
        {
            foreach(var r in regions)
            {
                var currentMap = r.GetMap();
                switch(r.Corner)
                {
                    case Corners.BottomLeft:
                        Copy(currentMap, ref map, Corners.BottomLeft);
                        break;
                    case Corners.BottomRight:
                        Copy(RotateArrayRight(currentMap), ref map, Corners.BottomRight);
                        break;
                    case Corners.TopLeft:
                        Copy(RotateArrayLeft(currentMap), ref map, Corners.TopLeft);
                        break;
                    default:
                        Copy(RotateArrayRight(RotateArrayRight(currentMap)), ref map, Corners.TopRight);
                        break;
                }
            }
        }

   
        terrain.terrainData.SetHeights(0, 0, map);

        //TopLeft
       

        Grasslands = new Cicle(2*(regions.First().GrassLandsRadius/ terrain.terrainData.heightmapResolution)* terrain.terrainData.size.x, new Vector2(terrain.terrainData.size.x*.5f, terrain.terrainData.size.z*.5f));

        var mapAlignment = new Dictionary<Corners, Regions>();
        foreach (var r in regions)
            mapAlignment.Add(r.Corner, (Regions)Enum.Parse(typeof(Regions), r.GetType().Name.ToString()));

        var manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>();
        manager.MapAlignment = mapAlignment;

        var maxHeight = -1f;
        for (int j = 0; j < map.GetLength(1); j++)
        {
            for (int i = 0; i < map.GetLength(0); i++) { 
                if (map[j,i] > maxHeight)
                    maxHeight = map[j, i];
             }
        }

        var terrainMap = regions.First().GetTerrainMap(terrain.terrainData);
        regions.First().SetGrasslands(ref terrainMap);
        foreach (var region in regions)
            region.AssignTextures(mapAlignment.Keys.First(k=>mapAlignment[k] == region.Region), ref terrainMap);

         terrain.terrainData.SetAlphamaps(0, 0, terrainMap);

        

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetTextures()
    {
        var s = new SplatPrototype();
        s.texture = desertSand;
        var g = new SplatPrototype();
        g.texture = grassLand;
        var mGrass = new SplatPrototype();
        mGrass.texture = mountainGrass;
        var mGrey = new SplatPrototype();
        mGrey.texture = mountainGrey;
        var gSnow = new SplatPrototype();
        gSnow.texture = mountainSnow;


        terrain.terrainData.splatPrototypes = new SplatPrototype[5] { s, g, mGrass, mGrey, gSnow };
    }

    float[,] RotateArrayRight(float[,] array)
    {
        var length = array.GetLength(0);
        var newArray = new float[length, length];

        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                newArray[i, length - j-1] = array[j, i];
            }
        }
        return newArray;
    }

    float[,] RotateArrayLeft(float[,] array)
    {
        var length = array.GetLength(0);
        var newArray = new float[length, length];

        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                newArray[length - i - 1, j] = array[j, i];
            }
        }
        return newArray;
    }

    //float[,] FlipHorizontaly(float [,] )

    float[,] FlipDiagonal(float[,] array)
    {
        var length = array.GetLength(0);
        var newArray = new float[length, length];

        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                newArray[length - i - 1, length - j - 1] = array[j, i];
            }
        }
        return newArray;
    }

    void Copy(float[,] source, ref float[,] destination, Corners corner)
    {
        var length = source.GetLength(0);

        int startJ, starti;
        if (corner == Corners.TopRight)
        {
            startJ = 0;
            starti = 0;
        }
        else if (corner == Corners.BottomRight)
        {
            startJ = length;
            starti = 0;
        }
        else if (corner == Corners.TopLeft)
        {
            startJ = 0;
            starti = length;
        }
        else
        {
            startJ = length;
            starti = length;
        }
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                destination[j + startJ, i + starti] = source[j, i];
            }
        }

    }


}

public enum TextureIndexes
{
    Sand,
    Grasslands,
    MountainGrass,
    MountainGrey,    
    MountainSnow
}
