using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SubtitleManager : MonoBehaviour, IDialogueObserver
{
    [SerializeField] private GameObject subtitleContainer;
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text entityNameText;
    [SerializeField] private TMP_Text dialogueLineText;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private GameObject playerResponseObject;



    /// <summary>
    /// When the subject (Dialogue manager) invokes its listeners and passes the dialogue state, we can invoke the respetive list of user defined evvents
    /// </summary>
    /// <param name="state"> Current state of the dialogue system</param>
    public void OnNotify(DialogueState state)
    {
        switch (state)
        {
            case DialogueState.DialogueStart:
                ShowSubtitleInterface();
                break;

            case DialogueState.DialogueEnd:
                CloseSubtitleInterace();
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

            case DialogueState.PlayerResponse:

                CloseSubtitleInterace();
                break;
        }
    }

    private void Start()
    {
        DialogueManager.Instance.AddObserver(this);
        CloseSubtitleInterace();
        CloseParentInterface();
    }
    public void QueueDialogue(string line, string name, float length) //Need to change to object so i can see multiple line attributes 
    {

        StartCoroutine(LineTimer(line, name, length));
    }

    IEnumerator LineTimer(string line, string npcName, float length)
    {
        dialogueLineText.text = line;
        entityNameText.text = npcName;

        yield return new WaitForSecondsRealtime(length);

        CloseSubtitleInterace();

        //Tell the dialogue manager the line has ended so event system can be invoked
        DialogueManager.Instance.DialogueEnded();
    }

    private void HideAllUI()
    {
        dialogueContainer.SetActive(false);
        interactText.gameObject.SetActive(false);
        playerResponseObject.SetActive(false);
        subtitleContainer.SetActive(false);
    }

    private void ShowNPCInteractUI()
    {
        subtitleContainer.SetActive(true);
        interactText.gameObject.SetActive(true);
    }

    private void HideInteractionUI()
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

    private void CloseSubtitleInterace()
    {
        dialogueContainer.SetActive(false);
    }

    private void CloseParentInterface()
    {
        subtitleContainer.SetActive(false);
    }

    private void ShowSubtitleInterface()
    {
        subtitleContainer.SetActive(true);
        dialogueContainer.SetActive(true);
        
       
    }
}

public class SubTitleData
{
    public string actorName;
    public string lineText;
    public float lineLength;
}
