using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    int currentQuest = 1;

    bool inStateOfChoice = false;
    bool answeringDone = false;
    bool firstQuest = true;

    [SerializeField]
    GameObject[] catPrefabs = null;

    [SerializeField]
    GameObject catLeaderPrefab = null;

    public Dictionary<int, string> Actors;    
    public Dictionary<string, int> ActorKeys;
    
    public Dictionary<int, string> ActorQuestions;
    
    public Dictionary<int, string> Actor1Answers;
    public Dictionary<int, string> Actor2Answers;
    public Dictionary<int, string> Actor3Answers;
    public Dictionary<int, string> Actor4Answers;
    public Dictionary<int, string> Actor5Answers;

    public Dictionary<int, int> Actor1Choices;
    public Dictionary<int, int> Actor2Choices;


    public Dictionary<int, string> QuestionList;
    public Dictionary<string, int> QuestionListKeys;

    public Dictionary<int, string> Npc1Answers;
    
    public Dictionary<int, string> Npc2Answers;
    
    public Dictionary<int, string> Npc3Answers;

    List<int> questionsUsed = new List<int>();
    List<string> questionOptions = new List<string>();

    public List<GameObject> speechBubbles = new List<GameObject>();

    UIManager uiManager;

    [SerializeField] GameObject panel_finalResult;
    [SerializeField] GameObject result_escape;
    [SerializeField] GameObject result_fail;
    [SerializeField] GameObject result_win;
    [SerializeField] GameObject result_stay;

    [SerializeField] bool spaceWasPressed = false;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        Actors = new Dictionary<int, string>();
        ActorKeys = new Dictionary<string, int>();
        ActorQuestions = new Dictionary<int, string>();
        Actor1Answers = new Dictionary<int, string>();
        Actor2Answers = new Dictionary<int, string>();
        Actor3Answers = new Dictionary<int, string>();
        Actor4Answers = new Dictionary<int, string>();
        Actor5Answers = new Dictionary<int, string>();
        Npc1Answers = new Dictionary<int, string>();
        Npc2Answers = new Dictionary<int, string>();
        Npc3Answers = new Dictionary<int, string>();
        Actor1Choices = new Dictionary<int, int>();
        Actor2Choices = new Dictionary<int, int>();

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
                if (spaceWasPressed)
                {
                    spaceWasPressed = false;
                    ChangeGameState(GameState.Wait);
                    break;
                }
                break;
            
            //The inbetween state
            case GameState.Wait:
                UIResetInWait();
                uiManager.ShowHotseatSwitch();

                //Debug.Log("Wait");
                if (spaceWasPressed)
                {
                    spaceWasPressed = false;
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
                if (!inStateOfChoice)
                {
                    Debug.Log("AIPickQuestion");
                    inStateOfChoice = true;
                    RandomizeQuistionOptionsAI();
                    break;
                }
                

                
                break;

            //The player answer state
            case GameState.Answer:
                uiManager.ShowAnswerSubmit();
                if (!inStateOfChoice)
                {
                    if (firstQuest)
                    {
                        firstQuest = false;
                        LoadAnswerQuest();
                    }
                    inStateOfChoice = true;
                    Debug.Log("Answer");
                    break;
                }

                break;

            //The ai answer state
            case GameState.AIAnswer:
                if (!inStateOfChoice)
                {
                    Debug.Log("AIAnswer");
                    inStateOfChoice = true;
                    RandomizeAnswerAI();
                    break;
                }

                break;

            //The question and answer state
            case GameState.Reveal:
                if (!inStateOfChoice)
                {
                    Debug.Log("Reveal");
                    inStateOfChoice = true;
                    ShowAnswers();
                    break;
                }
                if (inStateOfChoice)
                {
                    if (currentQuest > 5)
                    {
                        inStateOfChoice = false;
                        currentQuest = 1;
                        NextGameState = GameState.Choice;
                        ChangeGameState(GameState.Wait);
                    }
                    if (spaceWasPressed)
                    {
                        currentQuest++;
                        spaceWasPressed = false;
                        ShowAnswers();
                    }
                    
                }

                break;

            //The player choice state
            case GameState.Choice:
                uiManager.ShowFinalChoice();
                if (!inStateOfChoice)
                {
                    Debug.Log("Choice");
                    CurrentActor = 1;
                    inStateOfChoice = true;                    
                    break;
                }
                break;
                

            //The choice result state
            case GameState.Result:
                panel_finalResult.SetActive(true);
                if (!inStateOfChoice)
                {
                    Debug.Log("Result");
                    inStateOfChoice = true;
                    ResolveChoice();
                    break;
                }
                break;
        }
    }

    // hacky method to replace the Input.GetKeyDown(KeyCode.Space) in the Update loop with a UI button press
    // this method is called by UIManager
    public void ButtonPressesSpace()
    {
        spaceWasPressed = true;
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
                        keys.Add(key);
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

    void RandomizeQuistionOptionsAI()
    {
        string quest = "";
        int key;
        int count = 1;
        if (QuestionList.Count > 0)
        {
            while (count < 2)
            {
                
                    key = Random.Range(1, QuestionList.Count);
                    if (!questionsUsed.Contains(key))
                    {
                        quest = QuestionList[key];
                                      
                        count++;
                        Debug.Log(key);
                    }

              
            }
            LockInQuestions(quest);
        }
    }
    void RandomizeAnswerAI()
    {
        string quest = "";
        int key;
        if (CurrentActor == 3)
        {
            key = Random.Range(1, Npc1Answers.Count);
            quest = Npc1Answers[key];

        }
        else if (CurrentActor == 4)
        {
            key = Random.Range(1, Npc2Answers.Count);
            quest = Npc2Answers[key];
        }
        else if (CurrentActor == 5)
        {
            key = Random.Range(1, Npc3Answers.Count);
            quest = Npc3Answers[key];
        }
        LockInAnswers(quest);
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
            Debug.Log(ActorQuestions.Count);
            inStateOfChoice = false;
            NextGameState = GameState.PickQuestion;
            ChangeGameState(GameState.Wait);
        }
        else if (CurrentActor < (ActorAmount + 1))
        {
            Debug.Log(ActorQuestions.Count);
            inStateOfChoice = false;
            uiManager.HideQuestionSelect();
            NextGameState = GameState.AIPickQuestion;
            ChangeGameState(NextGameState);
        }
        else
        {
            Debug.Log(ActorQuestions.Count);
            inStateOfChoice = false;
            uiManager.HideQuestionSelect();
            currentIteration = 0;
            CurrentActor = 1;
            NextGameState = GameState.Answer;
            ChangeGameState(GameState.Wait);
        }

    }

    public void LockInAnswers(string answer)
    {
        currentIteration++;
        inStateOfChoice = false;
        uiManager.ResetAnswers();

        if (CurrentActor == 1)
        {
            if (currentQuest < 5)
            {
                Actor1Answers[currentQuest] = answer;
                currentQuest++;
                LoadAnswerQuest();
            }
            else if(currentQuest > 4)
            {
                Actor1Answers[currentQuest] = answer;
                CurrentActor++;
                currentQuest = 1;
                answeringDone = false;
                firstQuest = true;
                NextGameState = GameState.Answer;
                ChangeGameState(GameState.Wait);
            }
            
            
        }
        else if(CurrentActor == 2)
        {

            if (currentQuest < 5)
            {
                Actor2Answers[currentQuest] = answer;
                currentQuest++;
                LoadAnswerQuest();
            }
            else if(currentQuest > 4)
            {
                Actor2Answers[currentQuest] = answer;
                CurrentActor++;
                currentQuest = 1;
                answeringDone = false;
                firstQuest = true;
                NextGameState = GameState.AIAnswer;
                ChangeGameState(NextGameState);
            }
        }
        else if(CurrentActor == 3)
        {
            if (currentQuest < 5)
            {
                Actor3Answers[currentQuest] = answer;
                currentQuest++;
                RandomizeAnswerAI();
            }
            else if (currentQuest > 4)
            {
                Actor3Answers[currentQuest] = answer;
                CurrentActor++;
                currentQuest = 1;
                answeringDone = false;
                firstQuest = true;
                NextGameState = GameState.AIAnswer;
                ChangeGameState(NextGameState);
            }
        }
        else if (CurrentActor == 4)
        {
            if (currentQuest < 5)
            {
                Actor4Answers[currentQuest] = answer;
                currentQuest++;
                RandomizeAnswerAI();
            }
            else if (currentQuest > 4)
            {
                Actor4Answers[currentQuest] = answer;
                CurrentActor++;
                currentQuest = 1;
                answeringDone = false;
                firstQuest = true;
                NextGameState = GameState.AIAnswer;
                ChangeGameState(NextGameState);
            }
        }
        else if (CurrentActor == 5)
        {
            if (currentQuest < 5)
            {
                Actor5Answers[currentQuest] = answer;
                currentQuest++;
                RandomizeAnswerAI();
            }
            else if (currentQuest > 4)
            {
                Actor5Answers[currentQuest] = answer;
                CurrentActor = 1;
                currentQuest = 1;
                answeringDone = false;
                firstQuest = true;
                NextGameState = GameState.Reveal;
                ChangeGameState(GameState.Wait);
            }
        }
    }

    private void LoadAnswerQuest()
    {
        string quest = ActorQuestions[currentQuest];
        Debug.Log(quest);
        uiManager.LoadNextQuestion(quest);
    }

    private void ShowAnswers()
    {
        foreach (GameObject bubble in speechBubbles)
        {
            bubble.SetActive(true);
        }
        

        GameObject cultLeader = catLeaderPrefab;
        TMP_Text quest = cultLeader.GetComponentInChildren<TMP_Text>();
        quest.text = "";
        if (currentQuest < 6)
        {
            quest.text = ActorQuestions[currentQuest];
            uiManager.ShowcContinueButton();
        }
        for (int i = 0; i < 5; i++)
        {
            if (currentQuest > 5)
            {
                inStateOfChoice = false;
                currentQuest = 1;
                NextGameState = GameState.Choice;
                ChangeGameState(GameState.Wait);
            }
            GameObject obj = catPrefabs[i];
            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
            text.text = "";

            if (i == 0)
            {
                string answer = Actor1Answers[currentQuest];
                text.text = answer;
            }
            else if (i == 1)
            {
                string answer = Actor2Answers[currentQuest];
                text.text = answer;
            }
            else if (i == 2)
            {
                string answer = Actor3Answers[currentQuest];
                text.text = answer;
            }
            else if (i == 3)
            {
                string answer = Actor4Answers[currentQuest];
                text.text = answer;
            }
            else if (i == 4)
            {
                string answer = Actor5Answers[currentQuest];
                text.text = answer; 
            }
        }
    }

    public void LockDecision(int actor, int choice)
    {
        if(CurrentActor < 2)
        {
            Debug.Log("Current Player chosen: " + CurrentActor +actor +choice);
            Actor1Choices[1] = actor;
            Actor1Choices[2] = choice;
            Debug.Log(Actor1Choices[1] + " " + Actor1Choices[2]);
            CurrentActor ++;
            NextGameState = GameState.Choice;
            ChangeGameState(GameState.Wait);


        }
        else if (CurrentActor > 1)
        {
            Debug.Log("Current Player chosen: " + CurrentActor + actor + choice);
            Actor2Choices[1] = actor;
            Actor2Choices[2] = choice;
            Debug.Log(Actor2Choices[1] + " " + Actor2Choices[2]);
            inStateOfChoice = false;
            NextGameState = GameState.Result;
            ChangeGameState(GameState.Wait);


        }
    }

    void ResolveChoice()
    {
        


        if (Actor1Choices[1] == 1 && Actor2Choices[1] == 1)
        {
            if (Actor1Choices[2] == 1 && Actor2Choices[2] == 1)
            {
                //escape
                result_escape.SetActive(true);
                uiManager.SetResolutionText("Reaffirming your humanity, two human players manage to escape from the ammonia-scented pits and make their way into the sunlight. Taking a moment to savor the feeling on your skin, you turn to each other, and neither is really sure who first asks... Do you like dogs?");
            }
            else if (Actor1Choices[2] == 1 && Actor2Choices[2] == 2)
            {
                //fail
                result_fail.SetActive(true);
                uiManager.SetResolutionText("Maybe neither of you ever really believed in the Cult of Cat, but having failed to be certain of your allies, you're doubts are revealed as foolish. The eldritch feline beast rises like a fuzzy kaiju before you, cat tails whipping back and forth around the gaping maw that descends toward you to swallow you up. That means he's excited, says one of the cultists as the razor sharp hooked shaped teeth tear into your flesh. Those are the last words you ever hear.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 1)
            {
                //win
                result_win.SetActive(true);
                uiManager.SetResolutionText("One of you looks on without a hint of shame as the other is dragged off. An all-too-human wail echoes off the walls as the darkness closes around the traitor. Another member leans in, They didn't even sound like a cat. Who even invited them? The faithful turn their backs to each other in a display of feline solidarity, and a rumbling purr echoes through the chamber.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 2)
            {
                //fail
                result_fail.SetActive(true);
                uiManager.SetResolutionText("Maybe neither of you ever really believed in the Cult of Cat, but having failed to be certain of your allies, you're doubts are revealed as foolish. The eldritch feline beast rises like a fuzzy kaiju before you, cat tails whipping back and forth around the gaping maw that descends toward you to swallow you up. That means he's excited, says one of the cultists as the razor sharp hooked shaped teeth tear into your flesh. Those are the last words you ever hear.");
            }

        }
        else if (Actor1Choices[1] == 1 && Actor2Choices[1] == 2)
        {
            if (Actor1Choices[2] == 1 && Actor2Choices[2] == 1)
            {
                //stay
                result_stay.SetActive(true);
                uiManager.SetResolutionText("You creep to the exit with your chosen accomplice. Your first hint that you have chosen poorly is the dry hissing that greets you as you turn the corner. One of you freezes, and the hooded figures move past you to the one who has failed their loyalty test. A deep growl overwhelms the hissing and a twisted graceful form leaps past the faithful, tearing pitilessly into the obvious traitor. Such is the price of disloyalty.");
            }
            else if (Actor1Choices[2] == 1 && Actor2Choices[2] == 2)
            {
                //stay
                result_stay.SetActive(true);
                uiManager.SetResolutionText("You creep to the exit with your chosen accomplice. Your first hint that you have chosen poorly is the dry hissing that greets you as you turn the corner. One of you freezes, and the hooded figures move past you to the one who has failed their loyalty test. A deep growl overwhelms the hissing and a twisted graceful form leaps past the faithful, tearing pitilessly into the obvious traitor. Such is the price of disloyalty.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 1)
            {
                //win
                result_win.SetActive(true);
                uiManager.SetResolutionText("One of you looks on without a hint of shame as the other is dragged off. An all-too-human wail echoes off the walls as the darkness closes around the traitor. Another member leans in, They didn't even sound like a cat. Who even invited them? The faithful turn their backs to each other in a display of feline solidarity, and a rumbling purr echoes through the chamber.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 2)
            {
                //stay
                result_stay.SetActive(true);
                uiManager.SetResolutionText("");
            }
        }
        else if (Actor1Choices[1] == 2 && Actor2Choices[1] == 1)
        {
            if (Actor1Choices[2] == 1 && Actor2Choices[2] == 1)
            {
                //stay
                result_stay.SetActive(true);
                uiManager.SetResolutionText("You creep to the exit with your chosen accomplice. Your first hint that you have chosen poorly is the dry hissing that greets you as you turn the corner. One of you freezes, and the hooded figures move past you to the one who has failed their loyalty test. A deep growl overwhelms the hissing and a twisted graceful form leaps past the faithful, tearing pitilessly into the obvious traitor. Such is the price of disloyalty.");
            }
            else if (Actor1Choices[2] == 1 && Actor2Choices[2] == 2)
            {
                //stay
                result_stay.SetActive(true);
                uiManager.SetResolutionText("You creep to the exit with your chosen accomplice. Your first hint that you have chosen poorly is the dry hissing that greets you as you turn the corner. One of you freezes, and the hooded figures move past you to the one who has failed their loyalty test. A deep growl overwhelms the hissing and a twisted graceful form leaps past the faithful, tearing pitilessly into the obvious traitor. Such is the price of disloyalty.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 1)
            {
                //win
                result_win.SetActive(true);
                uiManager.SetResolutionText("One of you looks on without a hint of shame as the other is dragged off. An all-too-human wail echoes off the walls as the darkness closes around the traitor. Another member leans in, They didn't even sound like a cat. Who even invited them? The faithful turn their backs to each other in a display of feline solidarity, and a rumbling purr echoes through the chamber.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 2)
            {
                //stay
                result_stay.SetActive(true);
                uiManager.SetResolutionText("You creep to the exit with your chosen accomplice. Your first hint that you have chosen poorly is the dry hissing that greets you as you turn the corner. One of you freezes, and the hooded figures move past you to the one who has failed their loyalty test. A deep growl overwhelms the hissing and a twisted graceful form leaps past the faithful, tearing pitilessly into the obvious traitor. Such is the price of disloyalty.");
            }
        }
        else if (Actor1Choices[1] == 2 && Actor2Choices[1] == 2)
        {
            if (Actor1Choices[2] == 1 && Actor2Choices[2] == 1)
            {
                //fail
                result_fail.SetActive(true);
                uiManager.SetResolutionText("Maybe neither of you ever really believed in the Cult of Cat, but having failed to be certain of your allies, you're doubts are revealed as foolish. The eldritch feline beast rises like a fuzzy kaiju before you, cat tails whipping back and forth around the gaping maw that descends toward you to swallow you up. That means he's excited, says one of the cultists as the razor sharp hooked shaped teeth tear into your flesh. Those are the last words you ever hear.");
            }
            else if (Actor1Choices[2] == 1 && Actor2Choices[2] == 2)
            {
                //fail
                result_fail.SetActive(true);
                uiManager.SetResolutionText("Maybe neither of you ever really believed in the Cult of Cat, but having failed to be certain of your allies, you're doubts are revealed as foolish. The eldritch feline beast rises like a fuzzy kaiju before you, cat tails whipping back and forth around the gaping maw that descends toward you to swallow you up. That means he's excited, says one of the cultists as the razor sharp hooked shaped teeth tear into your flesh. Those are the last words you ever hear.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 1)
            {
                //fail
                result_fail.SetActive(true);
                uiManager.SetResolutionText("Maybe neither of you ever really believed in the Cult of Cat, but having failed to be certain of your allies, you're doubts are revealed as foolish. The eldritch feline beast rises like a fuzzy kaiju before you, cat tails whipping back and forth around the gaping maw that descends toward you to swallow you up. That means he's excited, says one of the cultists as the razor sharp hooked shaped teeth tear into your flesh. Those are the last words you ever hear.");
            }
            else if (Actor1Choices[2] == 2 && Actor2Choices[2] == 2)
            {
                //fail
                result_fail.SetActive(true);
                uiManager.SetResolutionText("Maybe neither of you ever really believed in the Cult of Cat, but having failed to be certain of your allies, you're doubts are revealed as foolish. The eldritch feline beast rises like a fuzzy kaiju before you, cat tails whipping back and forth around the gaping maw that descends toward you to swallow you up. That means he's excited, says one of the cultists as the razor sharp hooked shaped teeth tear into your flesh. Those are the last words you ever hear.");
            }
        }
    }
}
