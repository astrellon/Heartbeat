﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIText : MonoBehaviour
{
    private enum State
    {
        Blank,
        FadeOut,
        FadeIn,
        Visible
    }

    private Text UIText;

    private string nextText = "";
    private bool newText = false;
    private State currentState = State.Blank;
    private float stateTime = 0.0f;

    public bool Started = false;

    private const float FadeTime = 0.3f;

	// Use this for initialization
	void Start ()
    {
        this.UIText = this.GetComponent<Text>();
	}

    public void NextString(string text)
    {
        this.nextText = text;
        this.newText = true;
    }
	
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
                if (this.newText)
                {
                    this.ChangeState(State.FadeIn);
                    this.UIText.text = this.nextText;
                    this.nextText = "";
                    this.newText = false;
                }
                break;

            case State.FadeIn:
                if (this.stateTime > FadeTime)
                {
                    this.ChangeState(State.Visible);
                    this.UIText.color = FadeColour(this.UIText.color, 1.0f);
                    return;
                }
                this.UIText.color = FadeColour(this.UIText.color, this.stateTime / FadeTime);
                break;

            case State.FadeOut:
                if (this.stateTime > FadeTime)
                {
                    this.ChangeState(State.Blank);
                    this.UIText.color = FadeColour(this.UIText.color, 0.0f);
                    return;
                }
                this.UIText.color = FadeColour(this.UIText.color, 1.0f - (this.stateTime / FadeTime));
                break;

            case State.Visible:
                if (this.newText)
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
