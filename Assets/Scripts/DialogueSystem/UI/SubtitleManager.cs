using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SubtitleManager : MonoBehaviour
{
    //For refactoring, maybe just make this a singleton so it can easily be accessed without references. There should be as little manual referencing as needed

    private GameObject subtitleContainer;
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text entityNameText;
    [SerializeField] private TMP_Text dialogueLineText;
    [SerializeField] private TMP_Text interactText;
    public GameObject playerResponseObject;

    private void Start()
    {
        subtitleContainer = this.gameObject;
        CloseSubtitleInterace();
    }
    public void QueueDialogue(string line, string name, float length) //Need to change to object so i can see multiple line attributes 
    {
        
        StartCoroutine(LineTimer(line, name, length));
    }

    IEnumerator LineTimer(string line, string npcName, float length)
    {
        dialogueLineText.text = line;
        entityNameText.text = npcName;
     
        dialogueContainer.SetActive(true);
        ShowSubtitleInterface();
        yield return new WaitForSeconds(length);
        subtitleContainer.SetActive(false);
        dialogueContainer.SetActive(false);
    }

    public void ShowNPCInteractUI()
    {
        subtitleContainer.SetActive(true);
        interactText.gameObject.SetActive(true);
    }

    public void HideInteractionUI()
    {
        subtitleContainer.SetActive(false);
        interactText.gameObject.SetActive(false);
    }

    public bool IsInteractPanelActive()
    {
        if (interactText.isActiveAndEnabled || dialogueContainer.activeInHierarchy || DialogueManager.Instance.playerResponseUI.responsePanel.activeInHierarchy)
        {
            return true;
        }

        else
        {
            return false;
        }

    }

    public void CloseSubtitleInterace()
    {
        subtitleContainer.SetActive(false);
    }

    public void ShowSubtitleInterface()
    {
        subtitleContainer.SetActive(true);
    }


}
