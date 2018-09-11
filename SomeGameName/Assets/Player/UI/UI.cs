using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UI : MonoBehaviour {

    Texture2D full;
    Texture2D empty;
    Texture2D background;

    public float yOutterOffsetPercentage = .05f;
    public float xOutterOffsetPercentage = .01f;
    public float width = .25f;
    public float height = .05f;
    public float yInnerOffsetPercentage = .01f;
    public float xInnerOffsetPercentage = .002f;

    Rect backgroundRect;
    Rect fullRect;
    Rect emptyRect;
    Stats stats;
    Vector2 originalSize;

    float Health
    {
        get { return stats.CurrentHealth; }
    }

    float PercentHealth
    {
        get { return stats.CurrentHealth/(float)stats.StartingHealth; }
    }

    void SetRectangles()
    {
        backgroundRect = new Rect(new Vector2(Screen.width * xOutterOffsetPercentage, Screen.height * yOutterOffsetPercentage), new Vector2(Screen.width * width, Screen.height * height));
        originalSize = new Vector2(backgroundRect.size.x * (1 - xInnerOffsetPercentage) - 2 * Screen.width * xInnerOffsetPercentage, backgroundRect.size.y * (1 - yInnerOffsetPercentage) - 2* Screen.height * yInnerOffsetPercentage);
        fullRect = new Rect(new Vector2(backgroundRect.position.x + Screen.width * xInnerOffsetPercentage, backgroundRect.position.y + Screen.height * yInnerOffsetPercentage), originalSize);
        emptyRect = new Rect(new Vector2(backgroundRect.position.x + Screen.width * xInnerOffsetPercentage, backgroundRect.position.y + Screen.height * yInnerOffsetPercentage), originalSize);
    }

    void Awake()
    {
        var textures = Resources.LoadAll("Health").Where((o) => o.GetType().FullName.Contains("Texture2D"));
        foreach(var t in textures)
        {
            if (t.name.Contains("full"))
                full = t as Texture2D;
            else if (t.name.Contains("empty"))
                empty = t as Texture2D;
            else if (t.name.Contains("black"))
                background = t as Texture2D;
        }
    }

    // Use this for initialization
    void Start () {

        SetRectangles();
    }
	
	// Update is called once per frame
	void Update () {
		if(stats == null)
        {
            var players = Manager.Players;
            if (players.Count == 0 || players.Count((p) => p== null) == players.Count)
                return;
            foreach(var p in players)
            {
                var mvt = p.GetComponent<Movement>();
                if (mvt.IsLocalPlayer)
                {
                    stats = p.GetComponent<Stats>();
                    break;
                }
            }
        }
        if (stats == null)
            return;
        fullRect.size = new Vector2(PercentHealth * originalSize.x, originalSize.y);

       
    }

    void OnGUI()
    {
        if (stats == null)
            return;
        GUI.DrawTexture(backgroundRect, background);
        GUI.DrawTexture(emptyRect, empty);
        GUI.DrawTexture(fullRect, full);
    }
}
