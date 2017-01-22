using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatEffectManager : MonoBehaviour
{
    public Gradient BeatEffectColours;
    public GameObject BeatEffectPrefab;

    private GameManager gameManager;
	// Use this for initialization

	void Start () {
		
        this.gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        this.gameManager.OnHeartbeat += GameManager_OnHeartbeat;
	}

    private void GameManager_OnHeartbeat(bool fullBeat)
    {
        this.CreateBeatEffect();
    }

    private void CreateBeatEffect()
    {
        var beatEffectObject = GameObject.Instantiate(this.BeatEffectPrefab);
        beatEffectObject.transform.parent = transform;

        var beatEffect = beatEffectObject.GetComponent<BeatEffect>();
        var colour = this.BeatEffectColours.Evaluate(this.gameManager.ReactionRatio);
        beatEffect.BaseColour = colour;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
