using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverText : MonoBehaviour {

    public string text;
    public bool showText;
    public float width;
    public Vector2 position;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnGUI()
    {
        if (showText)
            GUI.Box(new Rect(position + new Vector2(10, 0), new Vector2(width, width)), text);
    }
}
