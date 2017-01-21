using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public float TimeUnit = 2.0f; // In seconds
    public float CurrentReaction = -1;
    public float DeltaReaction = 0.0f;
    public float PreviousReaction = -1;

    private int beatsPerTime = 0;
    private float currentTimePeriod = 0.0f;

    private DialogueTree dialogueTree;

    public delegate void HeartbeatHandler();
    public event HeartbeatHandler OnHeartbeat;

    private MainUIText mainUIText;

	// Use this for initialization
	private void Start ()
    {
        this.mainUIText = GameObject.FindGameObjectWithTag("MainUIText").GetComponent<MainUIText>();

        this.dialogueTree = new DialogueTree(this);
        DialogueTreeSerializer.Load(this.dialogueTree, "dialogue.txt");
        this.dialogueTree.OnNextDialogue += DialogueTree_OnNextDialogue;

        this.mainUIText.NextString(this.dialogueTree.CurrentDialogue);
	}

    private void DialogueTree_OnNextDialogue(DialogueTree tree)
    {
        this.mainUIText.NextString(tree.CurrentDialogue);
    }

    // Update is called once per frame
    private void Update ()
    {
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

    private void DoHeartbeat()
    {
        this.beatsPerTime++;

        if (this.OnHeartbeat != null)
        {
            this.OnHeartbeat();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), "Reac: " + this.CurrentReaction);
        GUI.Label(new Rect(10, 40, 200, 30), "D Reac: " + this.DeltaReaction);
        GUI.Label(new Rect(10, 70, 200, 30), "Beats: " + this.beatsPerTime);
        GUI.Label(new Rect(10, 100, 200, 30), "Dialogue: " + this.dialogueTree.CurrentDialogue);
    }

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
}
