using System;
using System.Linq;
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
        public readonly string SpriteFilename;

        public TextData(string text, float duration, string spriteFilename)
        {
            this.Text = text;
            this.Duration = duration;
            this.SpriteFilename = spriteFilename;
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
        var currentSprite = "";
        List<string> currentNodeText = null;

        var textAsset = Resources.Load<TextAsset>("Dialogue/" + Path.GetFileNameWithoutExtension(filename));
        var lineReader = new StringReader(textAsset.text);

        string line = null;
        do
        {
            line = lineReader.ReadLine();
            if (line == null)
            {
                break;
            }

            if (line.Length == 0 || (line.Length >= 2 && line[0] == '/' && line[1] == '/'))
            {
                continue;
            }

            if (line[0] == '#')
            {
                if (currentId != "")
                {
                    map[currentId] = new TextData(string.Join("\n", currentNodeText.ToArray()), currentDuration, currentSprite);
                }
                currentNodeText = new List<string>();

                var temp = line.Substring(1).Split(' ');
                var headerSplit = new List<string>(temp.Where(x => x.Length < 2 || (x.Length >= 2 && x[0] != '/' && x[1] != '/')));

                currentId = headerSplit[0];
                currentDuration = float.Parse(headerSplit[1]);
                currentSprite = headerSplit.Count > 2 ? headerSplit[2] : "";
            }
            else
            {
                currentNodeText.Add(line);
            }
        } while (line != null);

        if (currentId != "")
        {
            map[currentId] = new TextData(string.Join("\n", currentNodeText.ToArray()), currentDuration, currentSprite);
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

            /*
            Sprite sprite = null;
            if (!string.IsNullOrEmpty(textData.SpriteFilename))
            {
                //sprite = Resources.Load<Sprite>("Images/" + textData.SpriteFilename);
                var image = Resources.Load<Texture2D>("Images/" + textData.SpriteFilename);
                sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            }
            */
            var result = new DialogueTree.DialogueNode(rule, textData.Text, textData.SpriteFilename, textData.Duration);
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

        tree.RootNode = loader.Create(AlwaysYes, "intro_1", null);
        var intro2 = loader.Create(AlwaysYes, "intro_2", tree.RootNode);
        var intro3 = loader.Create(AlwaysYes, "intro_3", intro2);
        var intro4 = loader.Create(AlwaysYes, "intro_4", intro3);

        var corny1 = loader.Create(NegativeMidRule, "corny_1", intro4);
        var corny2 = loader.Create(AlwaysYes, "corny_2", corny1);
        var corny3 = loader.Create(AlwaysYes, "corny_3", corny2);
        var corny4 = loader.Create(AlwaysYes, "corny_4", corny3);
        var corny5 = loader.Create(AlwaysYes, "corny_5", corny4);
        var corny6 = loader.Create(AlwaysYes, "corny_6", corny5);
        var corny7 = loader.Create(AlwaysYes, "corny_7", corny6);
        var corny8 = loader.Create(AlwaysYes, "corny_8", corny7);
        var corny9 = loader.Create(AlwaysYes, "corny_9", corny8);

        var tv1 = loader.Create(NegativeMidRule, "tv_1", corny9);
        var tv2 = loader.Create(AlwaysYes, "tv_2", tv1);
        var tv3 = loader.Create(AlwaysYes, "tv_3", tv2);
        var tv4 = loader.Create(AlwaysYes, "tv_4", tv3);
        var tv5 = loader.Create(AlwaysYes, "tv_5", tv4);
        var tv6 = loader.Create(AlwaysYes, "tv_6", tv5);
        var tv7 = loader.Create(AlwaysYes, "tv_7", tv6);
        var tv8 = loader.Create(AlwaysYes, "tv_8", tv7);
        var tv9 = loader.Create(AlwaysYes, "tv_9", tv8);

        var cheese1 = loader.Create(PositiveMidRule, "cheese_1", corny9);
        var cheese2 = loader.Create(AlwaysYes, "cheese_2", cheese1);
        var cheese3 = loader.Create(AlwaysYes, "cheese_3", cheese2);
        var cheese4 = loader.Create(AlwaysYes, "cheese_4", cheese3);
        var cheese5 = loader.Create(AlwaysYes, "cheese_5", cheese4);
        var cheese6 = loader.Create(AlwaysYes, "cheese_6", cheese5);
        var cheese7 = loader.Create(AlwaysYes, "cheese_7", cheese6);
        var cheese8 = loader.Create(AlwaysYes, "cheese_8", cheese7);
        var cheese9 = loader.Create(AlwaysYes, "cheese_9", cheese8);
        var cheese10 = loader.Create(AlwaysYes, "cheese_10", cheese9);
        var cheese11 = loader.Create(AlwaysYes, "cheese_11", cheese10);
        var cheese12 = loader.Create(AlwaysYes, "cheese_12", cheese11);

        var tc1 = loader.Create(AlwaysYes, "tc_1", cheese12);

        var creepy1 = loader.Create(PositiveMidRule, "creepy_1", intro4);
        var creepy2 = loader.Create(AlwaysYes, "creepy_2", creepy1);
        var creepy3 = loader.Create(AlwaysYes, "creepy_3", creepy2);
        var creepy4 = loader.Create(AlwaysYes, "creepy_4", creepy3);
        var creepy5 = loader.Create(AlwaysYes, "creepy_5", creepy4);

        var marriage1 = loader.Create(NegativeMidRule, "marriage_1", creepy4);
        var marriage2 = loader.Create(AlwaysYes, "marriage_2", marriage1);
        var marriage3 = loader.Create(AlwaysYes, "marriage_3", marriage2);
        var marriage4 = loader.Create(AlwaysYes, "marriage_4", marriage3);

        var sex1 = loader.Create(PositiveMidRule, "sex_1", creepy4);
        var sex2 = loader.Create(AlwaysYes, "sex_2", sex1);
        var sex3 = loader.Create(AlwaysYes, "sex_3", sex2);
        var sex4 = loader.Create(AlwaysYes, "sex_4", sex3);
        var sex5 = loader.Create(AlwaysYes, "sex_5", sex4);

        //var superCreepy1 = loader.Create((manager) => { return manager.CurrentReaction > MidReaction && manager.DeltaReaction > VeryPositiveReaction; }, "superCreepy1", intro2);
    }


}
