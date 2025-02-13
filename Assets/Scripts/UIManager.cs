using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

enum FinalChoiceCharacter
{
    None,
    Player,
    NPC
}
enum FinalChoiceDecision
{
    None,
    Escape,
    Report
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    GameManager gameManager;
    // Question
    [SerializeField] GameObject panel_questionSelect;
    [SerializeField] TMP_Dropdown dropdown_selectQuestion;
    [SerializeField] Button SubmitButton;
    [SerializeField] string questionSelected;

    // Answer
    [SerializeField] GameObject panel_answerSubmit;
    [SerializeField] TMP_InputField inputfield_submitAnswer;
    [SerializeField] TMP_Text text_question;
    [SerializeField] string answerSubmitted;

    // Hotseat
    [SerializeField] GameObject panel_HotseatSwitch;

    // Final Choice
    [SerializeField] GameObject panel_FinalChoice;
    [SerializeField] Sprite player1Sprite;
    [SerializeField] Sprite player2Sprite;
    [SerializeField] FinalChoiceCharacter playerChoiceCharacter;
    [SerializeField] FinalChoiceDecision playerChoiceDecision;

    // Continue button
    [SerializeField] GameObject panel_ContinueButton;

    // Resolution screen
    [SerializeField] TMP_Text text_resolution;

    private void Awake()
    {
            instance = this;
    }
    private void Start()
    {
        //testQuestions = new List<string>() { "Q1", "Q2", "Q3" };
        gameManager = GameManager.instance;
        HideHotseatSwitch(); 
        HideQuestionSelect();   
        HideAnswerSubmit();
        HideFinalChoice();
    }

    public void OnButtonContinue()
    {
        gameManager.ButtonPressesSpace();
    }
    public void ShowcContinueButton()
    {
        panel_ContinueButton.SetActive(true);
    }

    // ------- Hotseat Change --------------------------
    public void ShowHotseatSwitch()
    {
        panel_HotseatSwitch.SetActive(true);
    }
    public void HideHotseatSwitch()
    {
        panel_HotseatSwitch.SetActive(false);
    }


    // ------- Question Selection ----------------------

    public void ShowQuestionSelect()
    {
        panel_questionSelect.SetActive(true);
    }
    public void HideQuestionSelect()
    {
        panel_questionSelect.SetActive(false);
    }
    public void SetQuestionOptions(List<string> questions)
    {
        dropdown_selectQuestion.options.Clear();
        foreach (string t in questions)
        {
            dropdown_selectQuestion.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }
    }

    public void SubmitSelectedQuestion()
    {
        int answerSelected = 0;
        answerSelected = dropdown_selectQuestion.value;
        questionSelected = dropdown_selectQuestion.options[dropdown_selectQuestion.value].text;
        gameManager.LockInQuestions(questionSelected);
        //Debug.Log("Player selected: " + questionSelected);
    }

    // ------- Answer Submission ----------------------

    public void ShowAnswerSubmit()
    {
        
        panel_answerSubmit.SetActive(true);
        
    }
    public void HideAnswerSubmit()
    {
        panel_answerSubmit.SetActive(false);
    }

    public void ResetAnswers()
    {
        inputfield_submitAnswer.text = "";
    }

    public void LoadNextQuestion(string quest)
    {
        text_question.text = quest;
    }

    public void SetAnswer()
    {
        // OnValueChanged and OnEndEdit will update this, it's the InputField
        answerSubmitted = inputfield_submitAnswer.text;
    }
    public void ConfirmAnswer()
    {
        // This is the button that confirms the submitted answer
        gameManager.LockInAnswers(answerSubmitted);
        Debug.Log("Player answered: " + answerSubmitted);

    }

    // ------- Final Choice: Select other human, then Escape or Report --------------------------
    // this will show the images of the 4 others, needs to be dynamic to swap out one player's image for the other
    // ------- Final Choice change --------------------------
    public void ShowFinalChoice()
    {
        panel_FinalChoice.SetActive(true);
    }
    public void HideFinalChoice()
    {
        panel_FinalChoice.SetActive(false);
    }

    public void StartDecision()
    {
        playerChoiceCharacter = 0;
        playerChoiceCharacter = 0;
    }

    public void SelectTargetCharacter(int character)
    {
        if (character == 0) // 0 is the value for any NPC
        {
            playerChoiceCharacter = FinalChoiceCharacter.NPC;
        }
        else if (character == 1)    // 1 is the value for the other player
        {
            playerChoiceCharacter = FinalChoiceCharacter.Player;
        }
    }
    public void SelectDecision(int decision)
    {
        if (decision == 0) // 0 is the value for Escape
        {
            playerChoiceDecision = FinalChoiceDecision.Escape;
        }
        else if (decision == 1)    // 1 is the value for Report
        {
            playerChoiceDecision = FinalChoiceDecision.Report;
        }
        SendDecision();
    }

    public void SendDecision()
    {
        gameManager.LockDecision((int)playerChoiceCharacter, (int)playerChoiceDecision);
    }

    public void SetResolutionText(string resolution)
    {
        text_resolution.text = resolution;
    }

}
