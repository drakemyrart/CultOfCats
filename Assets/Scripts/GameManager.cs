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
    int currentIteration = 0;

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
                //Debug.Log("PickQuestion");
                uiManager.ShowQuestionSelect();
                RandomizeQuistionOptions();


                if (currentIteration == CurrentPlayer)
                {
                    PreviousPlayer = CurrentPlayer;
                    CurrentPlayer = NextPlayer;
                    NextPlayer++;
                    
                }
                
                
                //check before going to wait
                if(CurrentPlayer == PlayerAmount)
                {
                    ChangeNextGameState (GameState.AIPickQuestion);

                }
                else
                {
                    ChangeNextGameState (CurrentGameState);
                    ChangePreviuosGameState (GameState.Wait);
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
                if (currentIteration == CurrentPlayer)
                {
                    PreviousPlayer = CurrentPlayer;
                    CurrentPlayer = NextPlayer;
                    NextPlayer++;
                    currentIteration++;
                }
                else
                {
                    currentIteration++;
                }

                //check before going to wait
                if (CurrentPlayer == PlayerAmount)
                {
                    ChangeNextGameState(GameState.AIAnswer);
                }
                else
                {
                    ChangeNextGameState(CurrentGameState);
                    ChangePreviuosGameState(GameState.Wait);
                }
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
                if (currentIteration == CurrentPlayer)
                {
                    PreviousPlayer = CurrentPlayer;
                    CurrentPlayer = NextPlayer;
                    NextPlayer++;
                    currentIteration++;
                }
                else
                {
                    currentIteration++;
                }

                //check before going to wait
                if (CurrentPlayer == PlayerAmount)
                {
                    ChangeNextGameState(GameState.AIPickQuestion);

                }
                else
                {
                    ChangeNextGameState(CurrentGameState);
                    ChangePreviuosGameState(GameState.Wait);
                }
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
        questionOptions.Clear();
        int count = 1;
        List<int> questionsInUse = new List<int>();
        while (true) 
        {
            if (count > 5) 
            {
                break;
            }
            int key = 0;
            int questionCount = 0;
            questionCount = QuestionList.Count;

            if (questionOptions.Count < 1)
            {
                key = Random.Range(1, questionCount);
                string quest = QuestionList[key];
                questionOptions.Add(quest);
                count++;
            }
            
            if (questionsInUse.Count > 0 && !questionsInUse.Contains(key))
            {
                string quest = QuestionList[key];
                questionOptions.Add(quest);
                count++;
            } 
            else
            {
                key = Random.Range(1, questionCount);
            }
            
              
        }

        uiManager.SetQuestionOptions(questionOptions);
        
    }

}
