using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour
{

    public static QuestHandler Instance => m_instance;
    private static QuestHandler m_instance;

    [SerializeField] private QuestCubes qCubes;
    [SerializeField] private GameObject cubeContainer;

    int currentQuestKey;

    int maxCubeNumber;
    int cubeNumber = 0;
    private void Start()
    {
        m_instance = this;
        Quest.InitQuests();
        
    }


    public void QuestAccepted()
    {
        GameDataResolver.Instance.SetGameDataVariable((uint)2, true);
    }

    public void UpdateQuest()
    {
        cubeNumber += 1;

        QuestUI.Instance.UpdateQuestUI(cubeNumber);
        if (cubeNumber == maxCubeNumber)
        {
            //Quest Complete
        }
    }

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
