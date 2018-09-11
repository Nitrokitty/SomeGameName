using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

   
    public float damage = 5f;
    public bool IsAlive = true;
    
    Stats stats;
   
    
	// Use this for initialization
	void Start () {
        stats = GetComponent<Stats>();

    }

    // Update is called once per frame
    void Update() {
        if (stats == null)
            return;
        if (stats.CurrentHealth <= 0)
            IsAlive = false;
        if (!IsAlive)
            OnDeath();

    }     

    public void OnDeath()
    {
        Destroy(this);
    }
}
