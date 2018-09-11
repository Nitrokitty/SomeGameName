using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour {

    Dictionary<string, int> items;

    bool showInventory = false;

    Rect inventoryRect;
    public float inventoryRectWidthPercent = .25f;
    public float inventoryRectHeightPercent = .65f;
    public float inventoryRectStartXPercent = 0f;
    public float inventoryRectStartYPercent = .35f;
    public float columns = 2f;
    public float columnOffsetPercent = .05f;

    public float texturePercentage = .2f;
    public float yMargin = .01f;
    public float buttonRadius = 2f;
    Vector2 textureSize;
    Vector2 textureOffset;
    public TeamInventory teamInventory;
    public Vector3 forge;
    int maxDistanceFromForge = 5;

    List<Texture2D> selectedItems;
    List<Texture2D> equiptedItems;

    public GUIStyle normalButtonSkin;
    public GUIStyle activeButtonSkin;
    public GUIStyle equiptedButtonSkin;

    Texture2D draggingTexture = null;


    Stats stats;

    static Dictionary<EquipmentType, List<CraftedResourcesType>> craftedItems;

    void Awake()
    {
        items = new Dictionary<string, int>();
        selectedItems = new List<Texture2D>();
        equiptedItems = new List<Texture2D>();
        craftedItems = new Dictionary<EquipmentType, List<CraftedResourcesType>>();
    }

   
 
       

        // Use this for initialization
    void Start() {
        inventoryRect = new Rect(new Vector2(Screen.width * inventoryRectStartXPercent, Screen.height * inventoryRectStartYPercent), new Vector2(Screen.width * inventoryRectWidthPercent, Screen.height * inventoryRectHeightPercent));
        textureSize = new Vector2(inventoryRect.size.x * texturePercentage, inventoryRect.size.x * texturePercentage);
        textureOffset = new Vector2(inventoryRect.size.x * (1 - texturePercentage * columns) / (columns * 2), inventoryRect.size.y * yMargin);
        stats = GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.B))
            showInventory = !showInventory;

        equiptedItems = stats.GetEquiptment().Select((e) => Manager.ResourceTextures.FirstOrDefault((t) => t.name == Char.ToLower(e.ToString()[0]) + e.ToString().Substring(1))).ToList();

        if (showInventory && Input.GetMouseButtonUp(0) && draggingTexture != null)
        {           
            if(teamInventory.playerInBounds && teamInventory.showInventory)
            {
                var center = Camera.main.WorldToScreenPoint(teamInventory.gameObject.transform.position);
                Rect screenRect = BoundsToScreenRect(teamInventory.bounds);
                var mouse = Input.mousePosition;
                mouse.y = Screen.height - mouse.y;
                if (teamInventory.inventoryRect.Contains(mouse))
                {
                    var key = FormatKey(draggingTexture.name, ObjectType.Resource);
                    teamInventory.AddItem(key);
                    items.Remove(key);
                }
            }
            draggingTexture = null;
        }
        else if(showInventory && Input.GetMouseButtonDown(1) && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            string craftedItem;
            if (TryCombine(selectedItems.ToArray(), out craftedItem))
            {
                AddItem(craftedItem);
                RemoveItems(selectedItems.Select(i => i.name).ToArray(), selectedItems.Select(i => ObjectType.Resource).ToArray());

            }
            selectedItems.Clear();
        }
       

        items = items.Count > 1 ? items.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : items;

        if (Manager.DEBUG)
        {
            inventoryRect = new Rect(new Vector2(Screen.width * inventoryRectStartXPercent, Screen.height * inventoryRectStartYPercent), new Vector2(Screen.width * inventoryRectWidthPercent, Screen.height * inventoryRectHeightPercent));
            textureSize = new Vector2(inventoryRect.size.x * texturePercentage, inventoryRect.size.x * texturePercentage);
            textureOffset = new Vector2(inventoryRect.size.x * (1 - texturePercentage * columns) / (columns * 2), inventoryRect.size.y * .05f);
        }
    }

    public int selGridInt = 0;
    public string[] selStrings = new string[] { "Grid 1", "Grid 2", "Grid 3", "Grid 4" };

    private void OnGUI()
    {
        if (!showInventory)
            return;

        GUI.skin.box.wordWrap = true;
        //GUI.BeginScrollView(new Rect(inventoryRect.position, new Vector2(inventoryRect.size.x * 1.5f, inventoryRect.size.y * 1.5f)), Vector3.zero, inventoryRect);
        GUI.Box(inventoryRect, string.Empty);
        float xDisplacement = 0;
        float yDisplacement = 0;
        int nextColumn = 0;
        var textures = Manager.ResourceTextures;
        bool setHover = false;
        string hoverText = null;
        Rect hoverRect = new Rect();
        //List<string> names = new List<string>();
        
        foreach (var i in items)
        {
            
            Vector2 currentPosition = new Vector2(inventoryRect.x + xDisplacement, inventoryRect.y + yDisplacement) + textureOffset;

            if (currentPosition.y > inventoryRect.max.y)
                break;

            if (Manager.ResourceTextures.Count == 0)
                return;

            var texture = textures.FirstOrDefault(r => r.name.ToLower() == i.Key.ToLower()); //new Vector2(inventoryRect.size.x * .4f, inventoryRect.size.y * .1f)
            if (texture == null)
            {
                texture = textures.FirstOrDefault(r => r.name.ToLower().Contains(i.Key.ToLower()));
                if (texture == null)
                    return;
            }
            if(selectedItems.Any(item => item.name == texture.name))
                GUI.Box(new Rect(currentPosition, textureSize), string.Empty,  activeButtonSkin);//, new Rect(new Vector2(inventoryRect.x + xDisplacement, inventoryRect.y + yDisplacement)));            
            else if(equiptedItems.Any(item => item.name == texture.name))
                GUI.Box(new Rect(currentPosition, textureSize), string.Empty, equiptedButtonSkin);
            else
                GUI.Box(new Rect(currentPosition, textureSize), string.Empty, normalButtonSkin);

            var currButton = new Rect(new Vector2(currentPosition.x - buttonRadius, currentPosition.y - buttonRadius), new Vector2(textureSize.x + buttonRadius * 2, textureSize.y + buttonRadius * 2));


            

            if (Input.GetMouseButtonDown(0) && currButton.Contains(new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, Input.mousePosition.z)))
            {
                if (draggingTexture == null)
                {
                    draggingTexture = texture;
                }

            }

            if (GUI.Button(currButton, texture))
            {
                var e = Event.current;
                if (e.button == 1 && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    if (selectedItems.Any(item => item.name == texture.name))
                    {
                        foreach (var item in selectedItems)
                        {
                            if (item.name == texture.name)
                            {
                                selectedItems.Remove(item);
                                break;
                            }
                        }
                    }
                    else
                    {
                        selectedItems.Add(texture);
                    }
                } else if(e.button == 1 && Enum.GetNames(typeof(EquipmentType)).Any((s) => Char.ToLower(s[0]) + s.Substring(1) == texture.name))
                {

                    var item = (EquipmentType)Enum.Parse(typeof(EquipmentType), Char.ToUpper(texture.name[0]) + texture.name.Substring(1));
                    if (equiptedItems.Any(t => t.name == texture.name))
                        stats.RemoveEquiptemnet(item);
                    else
                        stats.AddEquiptment(item, craftedItems[item]);
                }
            }

            //names.Add(texture.name[0] + texture.name.Substring(1) + " (" + items[FormatKey(texture.name, ObjectType.Resource)] + ")");

            if (currButton.Contains(Event.current.mousePosition))
            {
                hoverText = char.ToUpper(texture.name[0]) + texture.name.Substring(1) + " (" + items[FormatKey(texture.name, ObjectType.Resource)] + ")";
                hoverRect = new Rect(Event.current.mousePosition + new Vector2(10, 0), new Vector2(100, 25));
                setHover = true;
                //setHover = true;
                //var hover = transform.Find("InventoryHoverText").GetComponent<HoverText>();
                //hover.text = char.ToUpper(texture.name[0]) + texture.name.Substring(1);
                //hover.position = Event.current.mousePosition;
                //hover.showText = true;
            }

            nextColumn++;
            if (nextColumn >= columns)
            {
                xDisplacement = 0f;
                yDisplacement += textureSize.y + textureOffset.y * 2f;
                nextColumn = 0;
            } else
            {
                xDisplacement += textureSize.x + textureOffset.x * 2f;
            }



        }

        //selGridInt = GUI.SelectionGrid(inventoryRect, selGridInt, names.ToArray(), 2);

        if (draggingTexture != null)
        {
            var pos = Event.current.mousePosition;
            var size = new Vector2(textureSize.x + buttonRadius * 2, textureSize.y + buttonRadius * 2);
            pos.x -= size.x * .5f;
            pos.y -= size.y * .5f;
            var r = new Rect(pos, size);
            GUI.DrawTexture(r, draggingTexture);
        }

        //GUI.EndScrollView();
        if (setHover)
        {
            //var hover = transform.Find("InventoryHoverText").GetComponent<HoverText>();
            //hover.showText = false;
            GUI.Box(hoverRect, hoverText);
        }
    }

    public bool TryCombine(Texture2D[] textures, out string name)
    {
        name = string.Empty;

        if (Vector3.Distance(transform.position, forge) > maxDistanceFromForge)
            return false;
        if (textures.Count() == 0)
            return false;

        List<GameObject> prefabs = new List<GameObject>();
        List<CraftedResourcesType> craftedResources = new List<CraftedResourcesType>();
        GameObject r;
        
        
        foreach (var texture in textures)
        {
            if (texture.name.ToLower().Contains("s_") || texture.name.ToLower().Contains("b_"))
                craftedResources.Add((CraftedResourcesType)Enum.Parse(typeof(CraftedResourcesType), texture.name));
            else
            {
                r = Manager.GetReource(texture.name);
                if (r == null)
                    return false;
                prefabs.Add(r);
            }
            
        }

        if (prefabs.Count != 0 && craftedResources.Count != 0)
            return false;
        else if (prefabs.Count > 0)
            return TryCraftResource(prefabs, ref name);
        else
            return TryCraftEquipment(craftedResources, ref name);
    }

    bool TryCraftEquipment(List<CraftedResourcesType> craftedResources, ref string name)
    {
        var steelQuantities = new Dictionary<EquipmentType, int>() { { EquipmentType.ChestPlate, 3 }, { EquipmentType.SteelGloves, 1 } };
        var batteryQuantities = new Dictionary<EquipmentType, int>();
        var steelCount = 0;
        var batteryCount = 0;
        foreach (var r in craftedResources)
        {
            switch(r)
            {
                case CraftedResourcesType.B_CarbonZinc:
                case CraftedResourcesType.B_Lithium:
                case CraftedResourcesType.B_Plasma:                    
                    batteryCount++;
                    break;
                default:
                    steelCount++;
                    break;
            }
        }


        if (batteryCount > 0)
            return false;

        var key = steelQuantities.Keys.FirstOrDefault((k) => steelQuantities[k] == steelCount);
        if(steelQuantities[key] > steelCount)
        {
            key = steelQuantities.Keys.FirstOrDefault((k) => steelQuantities[k] <= steelCount);
        }
        if (steelQuantities[key] > steelCount)
            return false;

        name = Char.ToLower(key.ToString()[0]) + key.ToString().Substring(1);

        craftedItems.Add(key, craftedResources.Take(steelCount + batteryCount).ToList());

        return true;
    }

    static bool TryCraftResource(List<GameObject> prefabs, ref string name)
    {
        ResourceBase tempR;
        List<CraftedResourcesType> possibleCraftedResources = null;
        var resourceTypes = new List<ResourceTypes>();


        foreach (var p in prefabs)
        {
            tempR = Manager.GetInactiveCompoent<ResourceBase>(p);
            resourceTypes.Add(tempR.type);
            if (possibleCraftedResources == null)
            {
                possibleCraftedResources = ResourceBase.GetBuildsInto(tempR.type);
            }
            else
            {
                var tempPossibleCraftedResources = new List<CraftedResourcesType>();
                foreach (var currentR in ResourceBase.GetBuildsInto(tempR.type))
                {
                    if (possibleCraftedResources.Contains(currentR))
                        tempPossibleCraftedResources.Add(currentR);
                }
                if (tempPossibleCraftedResources.Count == 0)
                    return false;
                possibleCraftedResources = tempPossibleCraftedResources;
            }
        }

        CraftedResourcesType? material = null;


        foreach (var currentCraftedResource in possibleCraftedResources)
        {
            var currentRequirements = CraftedResources.GetRequirements(currentCraftedResource);
            bool isCorrectMaterial = true;
            foreach (var requirment in currentRequirements)
            {
                if (!resourceTypes.Contains(requirment))
                {
                    isCorrectMaterial = false;
                    break;
                }
            }
            if (isCorrectMaterial)
            {
                material = currentCraftedResource;
                break;
            }
        }


        if (material == null)
            return false;

        name = material.Value.ToString();

        return true;
    }

    public bool RemoveItem(string name, ObjectType type)
    {
        var key = FormatKey(name, type);

        if (!InventoryContainsKey(key))
            return false;

        if (items[key] == 1)
            items.Remove(key);
        else
            items[key]--;

        return true;
    }

    public bool RemoveItems(string[] names, ObjectType[] types)
    {
        if (names.Length != types.Length)
            return false;

        bool success;
        for (int i = 0; i < names.Length; i++)
        {
            success = RemoveItem(names[i], types[i]);
            if (!success)
                return false;
        }
        return true;
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == ObjectType.Resource.ToString())
        {
            if (InventoryContainsKey(col.gameObject.name))
            {
                var key = FormatKey(col.gameObject.name, ObjectType.Resource);
                items[key]++;
            }
            else
            {
                items.Add(FormatKey(col.gameObject.name, ObjectType.Resource), 1);
            }

        }
        Debug.Log(InventoryToString());
    }

    public void AddItem(string name)
    {
        if (InventoryContainsKey(name))
        {
            var key = FormatKey(name, ObjectType.Resource);
            items[key]++;
        }
        else
        {
            items.Add(FormatKey(name, ObjectType.Resource), 1);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (Time.timeSinceLevelLoad < 2)
            return;

        GameObject gameObj;
        if (col.gameObject.name.Contains("Afterburner"))
            gameObj = col.gameObject.transform.parent.gameObject;
        else
            gameObj = col.gameObject;
        if (gameObj.tag == ObjectType.Resource.ToString())
        {
            AddItem(gameObj.name);

        }
        Debug.Log(InventoryToString());
    }

    public static string FormatKey(string key, ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Resource:
                var i = key.IndexOfAny(new char[] { ' ', '(' });
                var k = i <= 0 ? key : key.Substring(0, i);
                return char.ToUpper(k[0]) + k.Substring(1);
        }
        return key;
    }

    public bool InventoryContainsKey(string key)
    {

        foreach (var k in items.Keys)
        {
            if (k.Contains(key) || key.Contains(k))
                return true;
        }
        return false;

    }

    public string InventoryToString()
    {
        var s = "";
        foreach (var i in items)
            s += i.Key + ": " + i.Value + ", ";
        if (s == string.Empty)
            return "";
        return s.Substring(0, s.Length - 2);
    }

    public Rect BoundsToScreenRect(Bounds bounds)
    {
        // Get mesh origin and farthest extent (this works best with simple convex meshes)
        Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, 0f));
        Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, 0f)) - origin;
        extent = new Vector3(extent.x * .5f, extent.y * .5f, extent.z * .5f);

        // Create rect in screen space and return - does not account for camera perspective
        var s_x = extent.x - origin.x;
        s_x  = s_x < 0? -s_x : s_x;
        var s_y = origin.y - extent.y;
        s_y = s_y < 0 ? -s_y : s_y;
        return new Rect(origin.x, Screen.height - origin.y, s_x, s_y);
    }

    //class InventoryItem
    //{
    //    public InventoryItem (Texture2D texture, int row, int column)
    //    {
    //        Texture = texture;
    //        Row = row;
    //        Column = column;
    //    }

    //    public int Row
    //    {
    //        get;
    //        private set;
    //    }

    //    public int Column
    //    {
    //        get;
    //        private set;
    //    }

    //    public Texture2D Texture
    //    {
    //        get;
    //        private set;
    //    }
    //}
}
