using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SubtitleManager : MonoBehaviour
{
    [SerializeField] private GameObject subtitleContainer;
    [SerializeField] private TMP_Text entityNameText;
    [SerializeField] private TMP_Text dialogueLineText;


    private void Start()
    {
        subtitleContainer.SetActive(false);
    }
    public void QueueDialogue(IEnumerable<Line> dialogueQueue, string name) //Need to change to object so i can see multiple line attributes 
    {
        //Check if queue is empty or not to be able to stop triggers
        subtitleContainer.SetActive(true);
        foreach (var q in dialogueQueue)
        {
            StartCoroutine(LineTimer(q.line, name));
            Debug.Log(q);
        }
    }

    IEnumerator LineTimer(string line, string npcName)
    {
        dialogueLineText.text = line;
        entityNameText.text = npcName;
        yield return new WaitForSeconds(3f);
        subtitleContainer.SetActive(false);
    }


}
