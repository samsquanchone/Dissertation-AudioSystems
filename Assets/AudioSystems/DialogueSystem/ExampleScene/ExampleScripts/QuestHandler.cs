using DialogueSystem.DataResolver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Example of how dialogue system can invoke seperate behaviour. In this case quest setting functionality
/// </summary>
public class QuestHandler : MonoBehaviour
{

    //Declare singleton
    public static QuestHandler Instance => m_instance;
    private static QuestHandler m_instance;

    [SerializeField] private QuestCubes qCubes;
    [SerializeField] private GameObject cubeContainer;

    int currentQuestKey;

    int maxCubeNumber = 6;
    int cubeNumber = 0;
    private void Start()
    {
        //Instantiate singleton
        m_instance = this;

        //Initialise quest collection
        Quest.InitQuests();
        
    }


    /// <summary>
    /// Function that a dialogue will invoke to set a quest
    /// </summary>
    public void QuestAccepted()
    {
        GameDataResolver.Instance.SetGameDataVariable((uint)2, true);
    }


    /// <summary>
    /// For noting the amount of cubes collected by the player for the quest
    /// </summary>
    public void UpdateQuest()
    {
        cubeNumber += 1;

        QuestUI.Instance.UpdateQuestUI(cubeNumber);
        if (cubeNumber == maxCubeNumber)
        {
            //Quest Complete
            GameDataResolver.Instance.SetGameDataVariable(4, true);

        }
    }

    /// <summary>
    /// Function invoked if the player turns in the quest to john
    /// </summary>
    public void QuestTurnedIntoJohn()
    {
        GameDataResolver.Instance.SetGameDataVariable(5, true);
        QuestUI.Instance.DisableQuestUI();
    }


    /// <summary>
    /// Function invoked if the player turns in the quest to Dorothy
    /// </summary>
    public void QuestTurnedIntoDora()
    {
        GameDataResolver.Instance.SetGameDataVariable(6, true);
        QuestUI.Instance.DisableQuestUI();
    }

    /// <summary>
    /// Function invoked once the player talks to ben utilising his sequence dialogue type
    /// </summary>

    public void BenTalkedTo()
    {
        GameDataResolver.Instance.SetGameDataVariable(3, true);
    }


    /// <summary>
    /// Function invoked once the player has spoken to Dorothy, after the quest has been acceped from John
    /// </summary>
    public void SpokenToDora()
    {
        GameDataResolver.Instance.SetGameDataVariable(7, true);
    }


    /// <summary>
    /// Example of how to handle a collection of quest objects with the dialogue system, can set the Quest and provide a key on invoke of the function
    /// </summary>
    /// <param name="questKey"></param>
    public void SetQuest(int questKey)
    {
        currentQuestKey = questKey;
        QuestUI.Instance.SetQuestUI(Quest.GetQuest(questKey));

        GameObject cubeContainer = GameObject.Find("QuestCubes");

        maxCubeNumber = Quest.GetQuest(questKey).cubeAmount;

        foreach (var cube in qCubes.cubes)
        {
            Instantiate(cube, cubeContainer.transform);
            cube.SetActive(true);
        }
        
        
    }
}

public static class Quest
{
    public static Dictionary<int, QuestData> questDictionary = new();

    public static void InitQuests()
    {
        QuestData q1 = new() { questName = "Box collector", questObjective = "Collect boxes: ", cubeAmount = 6 };
        QuestData q2 = new() { questName = "Circle collector", questObjective = "Collect all circles" };


        questDictionary.Add(1, q1);
        questDictionary.Add(2, q2);
    }

    public static QuestData GetQuest(int key)
    {
        if (questDictionary.ContainsKey(key))
        {
            return questDictionary[key];
        }
        else
        {
            return null;
        }
    }
}

public class QuestData
{
    public string questName;
    public string questObjective;
    public int cubeAmount;
}
