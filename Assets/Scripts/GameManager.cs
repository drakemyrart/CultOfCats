using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameState CurrentGameState;
    public static GameState NextGameState;
    public static GameState PreviousGameState;
    public static int CurrentPlayer = 1;
    public static int NextPlayer = 2;
    public static int PreviousPlayer = 0;
    public static int PlayerAmount = 2;
    public static int NpcCount = 3;
    public static int CurrentActor = 1;
    public static int ActorAmount = 5;
    int currentIteration = 0;

    bool inStateOfChoice = false;

    public Dictionary<int, string> Actors;    
    public Dictionary<string, int> ActorKeys;
    
    public Dictionary<int, string> ActorQuestions;
    
    public Dictionary<int, string> ActorAnswer;
    
    public Dictionary<int, string> QuestionList;
    public Dictionary<string, int> QuestionListKeys;

    public Dictionary<int, string> Npc1Answers;
    
    public Dictionary<int, string> Npc2Answers;
    
    public Dictionary<int, string> Npc3Answers;

    List<int> questionsUsed = new List<int>();
    List<string> questionOptions = new List<string>();

    UIManager uiManager;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        Actors = new Dictionary<int, string>();
        ActorKeys = new Dictionary<string, int>();
        ActorQuestions = new Dictionary<int, string>();
        ActorAnswer = new Dictionary<int, string>();

        ChangeGameState(GameState.Start);
        ChangeNextGameState(GameState.PickQuestion);

    }

    private void Start()
    {
            uiManager = UIManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentGameState)
        {
            // the intro state
            case GameState.Start:
                //TODO put in info dump;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeGameState(GameState.Wait);
                    break;
                }
                break;
            
            //The inbetween state
            case GameState.Wait:
                UIResetInWait();
                uiManager.ShowHotseatSwitch();

                //Debug.Log("Wait");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    uiManager.HideHotseatSwitch();
                    ChangeGameState (NextGameState); 
                    break;
                }

                break;

            //The player q pick state
            case GameState.PickQuestion:
                
                uiManager.ShowQuestionSelect();
                if (!inStateOfChoice)
                {
                    Debug.Log("PickQuestion");
                    inStateOfChoice = true;
                    RandomizeQuistionOptions();
                    break;
                }


                break;

            //The ai q pick state 
            case GameState.AIPickQuestion:
                currentIteration = 0;
                CurrentPlayer = 1;
                NextPlayer = 2;

                ChangeNextGameState(GameState.Answer);
                ChangeGameState(GameState.Wait);
                break;

            //The player answer state
            case GameState.Answer:
               
                break;

            //The ai answer state
            case GameState.AIAnswer:
                currentIteration = 0;
                CurrentPlayer = 1;
                NextPlayer = 2;

                ChangeNextGameState(GameState.Reveal);
                ChangeGameState(GameState.Wait);
                break;

            //The question and answer state
            case GameState.Reveal:

                ChangeNextGameState(GameState.Reveal);
                ChangeGameState(GameState.Wait);
                break;

            //The player choice state
            case GameState.Choice:
                
                break;
                

            //The choice result state
            case GameState.Result:

                break;
        }
    }

    public void ChangeGameState(GameState state)
    {
        CurrentGameState = state;
    }

    public void ChangeNextGameState(GameState state)
    {
        NextGameState = state;
    }

    public void ChangePreviuosGameState(GameState state)
    {
        PreviousGameState = state;
    }

    void UIResetInWait()
    {
        uiManager.HideQuestionSelect();
        uiManager.HideAnswerSubmit();
        uiManager.HideFinalChoice();
    }

    void RandomizeQuistionOptions()
    {
        int key;
        int count = 1;
        List<int> keys = new List<int>();
        if(questionOptions.Count > 0) 
        {
            questionOptions.Clear();
        }

        if (QuestionList.Count > 0)
        {
            while (count < 6)
            {
                if(count == 1)
                {
                    key = Random.Range(1, QuestionList.Count);
                    string quest = QuestionList[key];
                    questionOptions.Add(quest);
                    keys.Add(key);
                    count++;
                    Debug.Log(key);
                }
                else if (count > questionOptions.Count)
                {
                    
                    key = Random.Range(1, QuestionList.Count);
                    if (!keys.Contains(key) && !questionsUsed.Contains(key))
                    {
                        string quest = QuestionList[key];
                        questionOptions.Add(quest);
                        count++;
                        Debug.Log(key);
                    }
                    
                }
            }
            if(questionOptions.Count > 0) 
            {
                uiManager.SetQuestionOptions(questionOptions);
            }
            
            
        }

    }

    public void LockInQuestions(string quest)
    {
        int questKey = 0;
        ActorQuestions[CurrentActor] = quest;
        currentIteration++;
        CurrentActor++;
        questKey = QuestionListKeys[quest];
        questionsUsed.Add(questKey);
        if (CurrentActor < (PlayerAmount+1))
        {
            NextGameState = GameState.PickQuestion;
            ChangeGameState(GameState.Wait);
            inStateOfChoice = false;
        }
        else if (CurrentActor < (ActorAmount + 1))
        {
            NextGameState = GameState.AIPickQuestion;
        }
        else
        {
            NextGameState = GameState.Answer;
            ChangeGameState(GameState.Wait);
        }

    }

}
