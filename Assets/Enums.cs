using System.Collections.Generic;

public enum GameState
{
    Start = 1,
    Wait,
    PickQuestion,
    AIPickQuestion,
    Answer,
    AIAnswer,
    Reveal,
    Choice,
    Result,
}

public enum DictName
{
    QuestionList = 1,
    Npc1Answers,
    Npc2Answers,
    Npc3Answers,
    Endings,
}
