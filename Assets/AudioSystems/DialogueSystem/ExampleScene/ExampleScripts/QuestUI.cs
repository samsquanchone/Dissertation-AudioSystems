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

    QuestData _questData;
    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;
        questNameText.gameObject.SetActive(false);
        questObjectiveText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void SetQuestUI(QuestData questData)
    {
        questNameText.gameObject.SetActive(true);
        questObjectiveText.gameObject.SetActive(true);

        _questData = questData;
        questNameText.text = questData.questName;
        questObjectiveText.text = questData.questObjective + " 0/" + questData.cubeAmount;
    }

    public void UpdateQuestUI(int cubesCollected)
    {
        questObjectiveText.text = _questData.questObjective + $":  {cubesCollected}/" + _questData.cubeAmount;

        if (cubesCollected == 6)
        {
            questObjectiveText.text = "Return boxes to John or Dorothy";
        }
    }

    public void DisableQuestUI()
    {
        questNameText.gameObject.SetActive(false);
        questObjectiveText.gameObject.SetActive(false);
    }
    
}
