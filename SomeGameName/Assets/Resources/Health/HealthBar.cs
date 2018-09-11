using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HealthBar : MonoBehaviour {


    public float currentHealth;
    public float totalHealth;

    bool thisObjectIsFull = true;
    GameObject full;
    GameObject empty;
    SpriteRenderer renderer;
    Vector3 startingScale;
    // Use this for initialization
    void Awake () {
        if (name.ToLower().Contains("full"))
            full = this.gameObject;
        else
        {
            empty = this.gameObject;
            thisObjectIsFull = false;
        }
        foreach(Transform t in gameObject.transform.parent)
        {
            if(t != transform && t.gameObject.name.ToLower().Contains("health"))
            {
                if (!thisObjectIsFull)
                    full = t.gameObject;
                else
                    empty = t.gameObject;
                break;
            }
        }
        renderer = GetComponent<SpriteRenderer>();
        startingScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
       
        full.transform.localScale = new Vector3(startingScale.x * (currentHealth/totalHealth), startingScale.y, startingScale.z);
       // empty.transform.localScale = new Vector3(startingScale.x * (1-(currentHealth / totalHealth)), startingScale.y, startingScale.z);
    }

    public void OnEnable()
    {
        if(thisObjectIsFull)
        {
            empty.SetActive(true);
        }
        else
            full.SetActive(true);
        renderer.enabled = true;
    }

    public void OnDisable()
    {
        if (thisObjectIsFull)
        {
            empty.SetActive(false);
        }
        else
            full.SetActive(false);
        renderer.enabled = false;

    }


}
