using UnityEngine;
using System.Collections;

public class AutoDestroyParticleSystem : MonoBehaviour
{

    public ParticleSystem System;
	// Use this for initialization
	void Start ()
    {
        if (System == null)
        {
            System = GetComponent<ParticleSystem>();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!System.IsAlive())
        {
            Destroy(gameObject);
        }	
	}
}
