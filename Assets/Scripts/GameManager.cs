using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public float TimeUnit = 2.0f; // In seconds
    public float CurrentReaction = -1;
    public float DeltaReaction = 0.0f;
    public float PreviousReaction = -1;

    public float ReactionRatio
    {
        get { return Mathf.Clamp(this.CurrentReaction / 6.0f, 0.0f, 1.0f); }
    }

    private int beatsPerTime = 0;
    private float currentTimePeriod = 0.0f;

    private const int FullBeatThreshold = 4;
    private int fullBeatCounter = 1;

    private DialogueTree dialogueTree;

    public delegate void HeartbeatHandler(bool fullBeat);
    public event HeartbeatHandler OnHeartbeat;

    public delegate void StartGameHandler();
    public event StartGameHandler OnStartGame;

    public MainUIText UIText;
    public MainUIImage UIImage;

    public Text TitleText;
    public Text InstructionsText;

    public bool IntroMode = true;
    private bool setText = false;
    private float startTime = 0.0f;

    public AudioSource HeartbeatSound;

    private void Awake()
    {
        this.TitleText.color = FadeColour(this.TitleText.color, 0);
        this.InstructionsText.color = FadeColour(this.InstructionsText.color, 0);
    }

	// Use this for initialization
	private void Start ()
    {
        this.dialogueTree = new DialogueTree(this);
        DialogueTreeSerializer.Load(this.dialogueTree, "dialogue.txt");
        this.dialogueTree.OnNextDialogue += DialogueTree_OnNextDialogue;
	}

    private void StartGame()
    {
        this.IntroMode = false;
        this.startTime = Time.time;

        this.UIText.Started = true;
        this.UIImage.Started = true;

        if (this.OnStartGame != null)
        {
            this.OnStartGame();
        }
    }

    private void DialogueTree_OnNextDialogue(DialogueTree tree)
    {
        this.UIText.NextString(tree.CurrentDialogue);
        this.UIImage.NextSprite(tree.CurrentSprite);
    }

    // Update is called once per frame
    private void Update ()
    {
        var sinceStart = Time.time - this.startTime;
        if (this.IntroMode)
        {
            this.TitleText.color = FadeColour(this.TitleText.color, Time.time - 1.0f);
            this.InstructionsText.color = FadeColour(this.InstructionsText.color, Time.time - 2.0f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.StartGame();
            }
        }
        else if (sinceStart < 2)
        {
            this.TitleText.color = FadeColour(this.TitleText.color, 1 - sinceStart);
            this.InstructionsText.color = FadeColour(this.InstructionsText.color, 1 - sinceStart);
        }
        else
        {
            if (!this.setText)
            {
                this.UIText.NextString(this.dialogueTree.CurrentDialogue);
                this.UIImage.NextSprite(this.dialogueTree.CurrentSprite);
                this.setText = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.DoHeartbeat();
            }

            this.currentTimePeriod += Time.deltaTime;

            if (this.currentTimePeriod > this.TimeUnit)
            {
                this.NextTimeUnit();
            }

            this.dialogueTree.Update();
        }
	}

    private void DoHeartbeat()
    {
        this.beatsPerTime++;
        this.fullBeatCounter++;
        var isFullBeat = (this.fullBeatCounter % FullBeatThreshold) == 0;

        this.HeartbeatSound.Play();

        if (this.OnHeartbeat != null)
        {
            this.OnHeartbeat(isFullBeat);
        }
    }

    /*
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), "Reac: " + this.CurrentReaction);
        GUI.Label(new Rect(10, 40, 200, 30), "D Reac: " + this.DeltaReaction);
        GUI.Label(new Rect(10, 70, 200, 30), "Beats: " + this.beatsPerTime);
        GUI.Label(new Rect(10, 100, 200, 30), "Dialogue: " + this.dialogueTree.CurrentDialogue);
    }
    */

    private void NextTimeUnit()
    {
        this.currentTimePeriod = 0.0f;
        this.PreviousReaction = this.CurrentReaction;
        this.CurrentReaction = (float)this.beatsPerTime / TimeUnit;
        this.beatsPerTime = 0;

        if (this.PreviousReaction >= 0.0f)
        {
            this.DeltaReaction = this.CurrentReaction - this.PreviousReaction;
        }
    }

    private Color FadeColour(Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));
    }
}
