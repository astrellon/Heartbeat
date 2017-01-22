using System;
using System.Collections.Generic;
using UnityEngine;

public class MainUIImage : MonoBehaviour
{
    private enum State
    {
        Blank,
        FadeOut,
        FadeIn,
        Visible
    }

    private SpriteRenderer UISPrite;

    private SpriteRenderer nextSprite = null;
    private bool newSprite = false;
    private State currentState = State.Blank;
    private float stateTime = 0.0f;

    public bool Started = false;

    private const float FadeTime = 0.5f;

    [Serializable]
    public struct NamedSprite
    {
        public string Name;
        public SpriteRenderer Sprite;
    }

    public List<NamedSprite> Images = new List<NamedSprite>();
    private Dictionary<string, SpriteRenderer> spriteMap = new Dictionary<string, SpriteRenderer>(StringComparer.OrdinalIgnoreCase);

	// Use this for initialization
	void Start ()
    {
        this.UISPrite = this.GetComponent<SpriteRenderer>();
	}

    void Awake()
    {
        foreach (var namedSprite in this.Images)
        {
            this.spriteMap[namedSprite.Name] = namedSprite.Sprite;
            namedSprite.Sprite.color = FadeColour(namedSprite.Sprite.color, 0);
        }
    }

    public void NextSprite(string spriteName)
    {
        this.nextSprite = null;
        this.spriteMap.TryGetValue(spriteName, out this.nextSprite);
        this.newSprite = true;
    }
    /*
    public void NextSprite(Sprite sprite)
    {
        this.nextSprite = sprite;
        this.newSprite = true;
    }
    */
	
	// Update is called once per frame
	void Update ()
    {
        if (!this.Started)
        {
            return;
        }

        this.stateTime += Time.deltaTime;

        switch (this.currentState)
        {
            case State.Blank:
                if (this.newSprite)
                {
                    this.UISPrite = this.nextSprite;
                    if (this.UISPrite != null)
                    {
                        this.ChangeState(State.FadeIn);
                        this.UISPrite.color = FadeColour(this.UISPrite.color, 0);
                    }
                    this.nextSprite = null;
                    this.newSprite = false;
                }
                break;

            case State.FadeIn:
                if (this.stateTime > FadeTime)
                {
                    this.ChangeState(State.Visible);
                    this.UISPrite.color = FadeColour(this.UISPrite.color, 1.0f);
                    return;
                }
                this.UISPrite.color = FadeColour(this.UISPrite.color, this.stateTime / FadeTime);
                break;

            case State.FadeOut:
                if (this.stateTime > FadeTime)
                {
                    this.ChangeState(State.Blank);
                    this.UISPrite.color = FadeColour(this.UISPrite.color, 0.0f);
                    return;
                }
                this.UISPrite.color = FadeColour(this.UISPrite.color, 1.0f - (this.stateTime / FadeTime));
                break;

            case State.Visible:
                if (this.newSprite)
                {
                    this.ChangeState(State.FadeOut);
                }
                break;
        }
	}

    private void ChangeState(State state)
    {
        this.currentState = state;
        this.stateTime = 0.0f;
    }

    private Color FadeColour(Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, Mathf.Min(alpha, 1.0f));
    }
}
