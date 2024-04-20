using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class LoadStrings : MonoBehaviour
{
    public static LoadStrings instance;
    GameManager gameManager;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this; 
    }
    void Start()
    {
        gameManager = GameManager.instance;
        LoadDictionary("questions.txt", DictName.QuestionList);
        LoadDictionary("npc1answer.txt", DictName.Npc1Answers);
        LoadDictionary("npc2answer.txt", DictName.Npc2Answers);
        LoadDictionary("npc3answer.txt", DictName.Npc3Answers);
    }

    public void LoadDictionary(string filename, DictName dictname)
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();

        
        string[] allWords = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, filename));
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
