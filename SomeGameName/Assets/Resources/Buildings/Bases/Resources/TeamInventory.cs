using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamInventory : MonoBehaviour {

    static Dictionary<string, int> items;

    public bool showInventory = false;
    public bool playerInBounds = false;

    public Rect inventoryRect;
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
    public Bounds bounds;
    Texture2D draggingTexture = null;


    List<Texture2D> selectedItems;

    public GUIStyle normalButtonSkin;
    public GUIStyle activeButtonSkin;

    // Use this for initializations
    void Start () {
        items = new Dictionary<string, int>();
        selectedItems = new List<Texture2D>();
        inventoryRect = new Rect(new Vector2(Screen.width * inventoryRectStartXPercent, Screen.height * inventoryRectStartYPercent), new Vector2(Screen.width * inventoryRectWidthPercent, Screen.height * inventoryRectHeightPercent));
        textureSize = new Vector2(inventoryRect.size.x * texturePercentage, inventoryRect.size.x * texturePercentage);
        textureOffset = new Vector2(inventoryRect.size.x * (1 - texturePercentage * columns) / (columns * 2), inventoryRect.size.y * yMargin);
        var col = GetComponent<BoxCollider>();
        bounds = col.bounds;
        bounds = new Bounds(bounds.center, bounds.size);
        col.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (Manager.Players.Count == 0)
            return;
        playerInBounds = bounds.Contains(Manager.Players.First().transform.position);
        if (Input.GetKeyDown(KeyCode.O) && playerInBounds && !showInventory)
            showInventory = true;
        else if (!playerInBounds || (showInventory && Input.GetKeyDown(KeyCode.O)))
            showInventory = false;


    }

    public void AddItem(string item)
    {
        if (items.ContainsKey(item))
            items[item]++;
        else
            items.Add(item, 1);
    }

    public void RemoveItem(string item)
    {
        if (items.ContainsKey(item))
            items[item]--;

        if (items[item] == 0)
            items.Remove(item);
    }

    private void OnGUI()
    {
        if (!showInventory)
            return;

        GUI.skin.box.wordWrap = true;
       
        GUI.Box(inventoryRect, string.Empty);
        float xDisplacement = 0;
        float yDisplacement = 0;
        int nextColumn = 0;
        var textures = Manager.ResourceTextures;
        bool setHover = false;
        string hoverText = null;
        Rect hoverRect = new Rect();

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
            GUI.Box(new Rect(currentPosition, textureSize), string.Empty, selectedItems.Any(item => item.name == texture.name) ? activeButtonSkin : normalButtonSkin);//, new Rect(new Vector2(inventoryRect.x + xDisplacement, inventoryRect.y + yDisplacement)));            

            var currButton = new Rect(new Vector2(currentPosition.x - buttonRadius, currentPosition.y - buttonRadius), new Vector2(textureSize.x + buttonRadius * 2, textureSize.y + buttonRadius * 2));


      
            if (GUI.Button(currButton, texture))
            { }
            if (Input.GetMouseButtonDown(0) && currButton.Contains(new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, Input.mousePosition.z)))
            {
                if (draggingTexture == null)
                {
                    draggingTexture = texture;
                }

            }


            if (currButton.Contains(Event.current.mousePosition))
            {
                hoverText = char.ToUpper(texture.name[0]) + texture.name.Substring(1) + " (" + items[Inventory.FormatKey(texture.name, ObjectType.Resource)] + ")";
                hoverRect = new Rect(Event.current.mousePosition + new Vector2(10, 0), new Vector2(100, 25));
                setHover = true;
               
            }

            nextColumn++;
            if (nextColumn >= columns)
            {
                xDisplacement = 0f;
                yDisplacement += textureSize.y + textureOffset.y * 2f;
                nextColumn = 0;
            }
            else
            {
                xDisplacement += textureSize.x + textureOffset.x * 2f;
            }



        }

        if (setHover)
        {
            //var hover = transform.Find("InventoryHoverText").GetComponent<HoverText>();
            //hover.showText = false;
            GUI.Box(hoverRect, hoverText);
        }
    }
}
