using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatEffect : MonoBehaviour {

    private ParticleSystem particles;
    public Color BaseColour;

	// Use this for initialization
	void Start ()
    {
        this.particles = this.GetComponent<ParticleSystem>();

        var particleMain = this.particles.main;
        particleMain.startRotationZ = new ParticleSystem.MinMaxCurve(0, 360);

        var currentColours = this.particles.colorOverLifetime.color;
        var gradient = new Gradient();
        var colourKeys = new []{
            new GradientColorKey(this.BaseColour, 0),
            new GradientColorKey(this.BaseColour, 1.0f)
        };
        gradient.SetKeys(colourKeys, currentColours.gradient.alphaKeys);
        var newColours = new ParticleSystem.MinMaxGradient(gradient);

        var col = this.particles.colorOverLifetime;
        col.color = gradient;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
