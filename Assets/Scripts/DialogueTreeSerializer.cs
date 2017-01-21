using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;


public class DialogueTreeSerializer
{
    #region Helpers
    private class TextData
    {
        public readonly string Text;
        public readonly float Duration;

        public TextData(string text, float duration)
        {
            this.Text = text;
            this.Duration = duration;
        }
    }

    private class TextMap : Dictionary<string, TextData>
    {
        public TextMap() : base (StringComparer.OrdinalIgnoreCase) { }
    }
    private static TextMap LoadTextMap(string filename)
    {
        var map = new TextMap();

        var currentId = "";
        var currentDuration = 0.0f;
        List<string> currentNodeText = null;

        var assetPath = Application.dataPath + Path.DirectorySeparatorChar + "Dialogue" + 
            Path.DirectorySeparatorChar + filename;

        foreach (var line in File.ReadAllLines(assetPath))
        {
            if (line.Length == 0 || (line.Length >= 2 && line[0] == '/' && line[1] == '/'))
            {
                continue;
            }

            if (line[0] == '#')
            {
                if (currentId != "")
                {
                    map[currentId] = new TextData(string.Join("\n", currentNodeText.ToArray()), currentDuration);
                }
                currentNodeText = new List<string>();

                var headerSplit = line.Substring(1).Split(' ');
                currentId = headerSplit[0];
                currentDuration = float.Parse(headerSplit[1]);
            }
            else
            {
                currentNodeText.Add(line);
            }
        }

        if (currentId != "")
        {
            map[currentId] = new TextData(string.Join("\n", currentNodeText.ToArray()), currentDuration);
        }

        return map;
    }

    private class Loader
    {
        private readonly TextMap text;

        public Loader(string filename)
        {
            this.text = LoadTextMap(filename);
        }

        public DialogueTree.DialogueNode Create(DialogueTree.NodeRule rule, string id, DialogueTree.DialogueNode parent)
        {
            var textData = this.text[id];
            var result = new DialogueTree.DialogueNode(rule, textData.Text, textData.Duration);
            if (parent != null)
            {
                result.AddTo(parent);
            }
            return result;
        }
    }
    #endregion

    private const float MidReaction = 2;
    private const float VeryPositiveReaction = 4;
    private const float VeryNegativeReaction = 1;

    private static DialogueTree.NodeRule AlwaysYes = (manager) => { return true; };
    private static DialogueTree.NodeRule PositiveMidRule = (manager) => { return manager.CurrentReaction > MidReaction; };
    private static DialogueTree.NodeRule NegativeMidRule = (manager) => { return manager.CurrentReaction <= MidReaction; };

    public static void Load(DialogueTree tree, string filename)
    {
        var loader = new Loader(filename);

        tree.RootNode = loader.Create(AlwaysYes, "intro1", null);
        var intro1 = loader.Create(AlwaysYes, "intro2", tree.RootNode);
        var intro2 = loader.Create(AlwaysYes, "intro3", intro1);

        var corny1 = loader.Create(NegativeMidRule, "corny1", intro2);
        var creepy1 = loader.Create(PositiveMidRule, "creepy1", intro2);
        var superCreepy1 = loader.Create((manager) => { return manager.CurrentReaction > MidReaction && manager.DeltaReaction > VeryPositiveReaction; }, "superCreepy1", intro2);
    }


}
