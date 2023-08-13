using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueUtility;

public class SubtitleManager : MonoBehaviour, IDialogueObserver
{
    [SerializeField] private GameObject subtitleContainer;
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TMP_Text entityNameText;
    [SerializeField] private TMP_Text dialogueLineText;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private GameObject playerResponseObject;
    [SerializeField] private GameObject responsePanel;



    /// <summary>
    /// When the subject (Dialogue manager) invokes its listeners and passes the dialogue state, we can invoke the respetive list of user defined evvents
    /// </summary>
    /// <param name="state"> Current state of the dialogue system</param>
    public void OnNotify(DialogueState state, SequenceType sequenceType, int instanceID)
    {
        switch (state)
        {
            case DialogueState.DialogueStart:
                ShowSubtitleInterface();
                break;

            case DialogueState.DialogueEnd:
                CloseSubtitleInterace();

                if(sequenceType != SequenceType.PlayerResponse)
                CloseParentInterface();
                break;

            case DialogueState.ConversationStart:
                HideInteractionUI();
                OpenParentInterface();
                if(sequenceType != SequenceType.PlayerResponse)
                ShowSubtitleInterface();
                break;

            case DialogueState.ConversationEnd:
                HideAllUI();
                break;

            case DialogueState.InteractShow:
                ShowNPCInteractUI();
                break;

            case DialogueState.InteractHide:
                HideInteractionUI();
                break;

            case DialogueState.PlayerResponse:
                OpenParentInterface();
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

    /// <summary>
    /// This function interacts with the dialogue manager and is used to send required data for sequencing subtitle for dialogue
    /// </summary>
    /// <param name="line"> The text of the dialogue line from the dialogue line being triggered</param>
    /// <param name="name"> Name of the NPC that the dialogue is being triggered for</param>
    /// <param name="length"> The length of the dialogue file calcualted by fmods sound.getLength </param>
    /// <param name="sequenceType"> The type of sequence the dialogue entity triggering the dialogue uses </param>
    public void QueueDialogue(string line, string name, float length, SequenceType sequenceType) //Need to change to object so i can see multiple line attributes 
    {
        StartCoroutine(LineTimer(line, name, length, sequenceType));
    }

    /// <summary>
    /// Sets the UI for the line and then waits the dialogue length and then notifys the dialogue manager that the dialogue has finised
    /// 
    /// Same parameters as QueueDialogue function
    /// </summary>
    /// <param name="line"></param>
    /// <param name="npcName"></param>
    /// <param name="length"></param>
    /// <param name="sequenceType"></param>
    /// <returns></returns>
    IEnumerator LineTimer(string line, string npcName, float length, SequenceType sequenceType)
    {
        dialogueLineText.text = line;
        entityNameText.text = npcName;

        yield return new WaitForSecondsRealtime(length);

        CloseSubtitleInterace();

        //Tell the dialogue manager the line has ended so event system can be invoked
        DialogueManager.Instance.DialogueEnded(sequenceType);
    }

    /// <summary>
    /// Hides all UI in this script, used for when a conversation ends
    /// </summary>
    private void HideAllUI()
    {
        dialogueContainer.SetActive(false);
        interactText.gameObject.SetActive(false);
        playerResponseObject.SetActive(false);
        subtitleContainer.SetActive(false);
    }

    /// <summary>
    /// Shows the interact UI when the player is within range of a dialogue entity that uses a Radius trigger type
    /// </summary>
    private void ShowNPCInteractUI()
    {

        subtitleContainer.SetActive(true);
        if (!responsePanel.activeInHierarchy && !dialogueContainer.activeInHierarchy)
            interactText.gameObject.SetActive(true);
    }
    /// <summary>
    /// Hides the interact UI when the player is within range of a dialogue entity that uses a Radius trigger type
    /// </summary>
    private void HideInteractionUI()
    {
    
        interactText.gameObject.SetActive(false);
        if(!dialogueContainer.activeInHierarchy || !responsePanel.activeInHierarchy)
        subtitleContainer.SetActive(false);
    }
    public bool IsInteractPanelActive()
    {
        if (interactText.isActiveAndEnabled)
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

    public void OpenParentInterface()
    {
        subtitleContainer.SetActive(true);
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
