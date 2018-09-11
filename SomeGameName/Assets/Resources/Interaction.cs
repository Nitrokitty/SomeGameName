using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {

    GameObject player;
    ResourceSpawn respawnComponent;
    public float delayTime = 0.5f;
    float currentTime;
    // Use this for initialization
    void Start () {
        respawnComponent = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceSpawn>();
        currentTime = delayTime;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (currentTime >= delayTime)
            {
                if (tag == "Resource")
                {
                    respawnComponent.Respawn<ResourceBase>(this.gameObject.GetComponent<ResourceBase>());
                }
                Destroy(gameObject);
                currentTime = 0;
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
        //else
        //    Debug.Log(other.gameObject.tag);
    }

}
