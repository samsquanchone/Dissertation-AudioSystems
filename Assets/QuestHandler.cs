using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour
{
    


    private void Start()
    {
        Quest.InitQuests();
    }


    public void QuestAccepted()
    {
        DialogueManager.Instance.resolver.SetGameDataVariable((uint)2, true);
    }

    public void SetQuest(int questKey)
    {

        QuestUI.Instance.SetQuestUI(Quest.GetQuest(questKey));
        
    }
}

public static class Quest
{
    public static Dictionary<int, QuestData> questDictionary = new();

    public static void InitQuests()
    {
        QuestData q1 = new() { questName = "Box collector", questObjective = "Collect all boxes" };
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
}
