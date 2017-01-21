using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTree
{
    public delegate bool NodeRule(GameManager manager);

    public class DialogueNode
    {
        public readonly NodeRule Rule;
        public readonly string Dialogue;
        public readonly float DialogueTime;

        public List<DialogueNode> Children = new List<DialogueNode>();

        public DialogueNode(NodeRule rule, string dialogue, float dialogueTime)
        {
            this.Rule = rule;
            this.Dialogue = dialogue;
            this.DialogueTime = dialogueTime;
        }

        public DialogueNode AddTo(DialogueNode parent)
        {
            parent.Children.Add(this);
            return this;
        }
    }

    private DialogueNode rootNode;
    public DialogueNode RootNode
    {
        get { return this.rootNode; }
        set { this.rootNode = this.currentNode = value; }
    }
    private DialogueNode currentNode;

    private float currentTime = 0.0f;

    public GameManager GameManager;

    public delegate void NextDialogueHandler(DialogueTree tree);
    public event NextDialogueHandler OnNextDialogue;

    public DialogueTree(GameManager gameManager)
    {
        this.GameManager = gameManager;
        /*
        this.RootNode = new DialogueNode((manager) => { return true; }, "Introduction dialogue", 4.0f);
        this.currentNode = this.RootNode;

        var child1 = new DialogueNode((manager) => { return manager.CurrentReaction > 2; }, "High response", 3.0f).AddTo(this.RootNode);
        var child2 = new DialogueNode((manager) => { return manager.CurrentReaction <= 2; }, "Low response", 3.0f).AddTo(this.RootNode);
        */
	}

	public void Update ()
    {
	    if (this.currentNode == null)
        {
            return;
        }

        this.currentTime += Time.deltaTime;
        if (this.currentTime >= this.currentNode.DialogueTime)
        {
            this.FindNextDialogueNode();
        }
	}

    public string CurrentDialogue
    {
        get
        {
            if (this.currentNode == null)
            {
                return "";
            }

            return this.currentNode.Dialogue;
        }
    }

    private void FindNextDialogueNode()
    {
        if (this.currentNode.Children.Count == 0)
        {
            this.SetCurrentNode(null);
            return;
        }

        this.currentTime = 0.0f;

        foreach (var child in this.currentNode.Children)
        {
            if (child.Rule(this.GameManager))
            {
                this.SetCurrentNode(child);
                break;
            }
        }
    }

    private void SetCurrentNode(DialogueNode node)
    {
        this.currentNode = node;
        if (this.OnNextDialogue != null)
        {
            this.OnNextDialogue(this);
        }
    }
}
