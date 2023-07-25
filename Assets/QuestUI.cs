using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance => m_instance;
    private static QuestUI m_instance;

    public TMP_Text questNameText;
    public TMP_Text questObjectiveText;
    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;
    }

    // Update is called once per frame
    public void SetQuestUI(QuestData questData)
    {
        questNameText.text = questData.questName;
        questObjectiveText.text = questData.questObjective;
    }
}
