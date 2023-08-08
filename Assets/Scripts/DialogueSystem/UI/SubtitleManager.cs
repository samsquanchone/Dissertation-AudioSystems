using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SubtitleManager : MonoBehaviour, IDialogueObserver
{
    //For refactoring, maybe just make this a singleton so it can easily be accessed without references. There should be as little manual referencing as needed

    public GameObject subtitleContainer;
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text entityNameText;
    [SerializeField] private TMP_Text dialogueLineText;
    [SerializeField] private TMP_Text interactText;
    public GameObject playerResponseObject;

    private void Start()
    {
        DialogueManager.Instance.AddObserver(this);
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

        ShowSubtitleInterface();

        yield return new WaitForSecondsRealtime(length);

        CloseSubtitleInterace();
    }

    public void HideAllUI()
    {
        dialogueContainer.SetActive(false);
        interactText.gameObject.SetActive(false);
        playerResponseObject.SetActive(false);
        subtitleContainer.SetActive(false);
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
        if (interactText.isActiveAndEnabled || dialogueContainer.activeInHierarchy)
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
        dialogueContainer.SetActive(false);
    }

    public void ShowSubtitleInterface()
    {
        subtitleContainer.SetActive(true);
        dialogueContainer.SetActive(true);
    }

    /// <summary>
    /// When the subject (Dialogue manager) invokes its listeners and passes the dialogue state, we can invoke the respetive list of user defined evvents
    /// </summary>
    /// <param name="state"> Current state of the dialogue system</param>
    public void OnNotify(DialogueState state)
    {
        switch (state)
        {
            case DialogueState.DialogueStart:

                break;

            case DialogueState.DialogueEnd:

                break;

            case DialogueState.ConversationStart:
                HideInteractionUI();
                ShowSubtitleInterface();
                break;

            case DialogueState.ConversationEnd:
                HideAllUI();
                break;

            case DialogueState.InteractShow:
                ShowNPCInteractUI();
                break;
        }
    }
}

public class SubTitleData
{
    public string actorName;
    public string lineText;
    public float lineLength;
}
