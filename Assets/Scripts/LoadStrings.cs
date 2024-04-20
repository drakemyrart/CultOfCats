using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LoadStrings : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

    public void LoadDictionary(string filename, DictName dictname)
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();

        TextAsset file = new TextAsset(filename);
        var content = file.text;
        string[] allWords = content.Split("\n");
        int key = 1;
        foreach (string word in allWords) 
        {
            dict[key] = word;
            key++;
        }

        if (dictname == DictName.QuestionList)
        {
            gameManager.QuestionList = dict;
            
        }
        else if (dictname == DictName.Npc1Answers)
        {
            gameManager.Npc1Answers = dict;

        }
        else if (dictname == DictName.Npc2Answers)
        {
            gameManager.Npc2Answers = dict;

        }
        else if (dictname == DictName.Npc3Answers)
        {
            gameManager.Npc3Answers = dict;

        }
    }
}
