using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Base" && Manager.CurrentState == States.Setup)
            Destroy(gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Base" && Manager.CurrentState == States.Setup)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Base" && Manager.CurrentState == States.Setup)
            Destroy(gameObject);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Base")
            Destroy(gameObject);
    }

}
