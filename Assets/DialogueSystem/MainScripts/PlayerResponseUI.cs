using DialogueUtility;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class sets up UI elements relating to player responses and sets relevant UI to enabled/disabled.
/// The behaviour for this class is handled through the OnNotify function that is called by the subject (DialogueSystem) when anything of note happens
/// The enum dialogue state is passed when subscribed events are invoked (Such as this one) and switch case with dialogue state is used to delegate behaviour within the script
/// </summary>
public class PlayerResponseUI : MonoBehaviour, IDialogueObserver
{
    [SerializeField] private GameObject templateResponseText;
    public GameObject responsePanel;

    private List<GameObject> responseText = new();
    [SerializeField] private TMP_Text playerNameText;


    /// <summary>
    /// When the subject (Dialogue manager) invokes its listeners and passes the dialogue state, we can invoke the respetive list of user defined evvents
    /// </summary>
    /// <param name="state"> Current state of the dialogue system</param>
    public void OnNotify(DialogueState state, SequenceType sequenceType, int instanceID)
    {
        if (sequenceType == SequenceType.PlayerResponse)
        {
            switch (state)
            {
                case DialogueState.DialogueStart:
                    HideCurrentResponseInterface();
                    break;

                case DialogueState.DialogueEnd:
                    ShowCurrentResponseInterface();
                    break;

                case DialogueState.ConversationStart:
                    GeneratePlayerResponses();
                    ShowCurrentResponseInterface();
                    break;

                case DialogueState.ConversationEnd:
                    HideCurrentResponseInterface();
                    break;

            }
        }
    }

    void Start()
    {

        DialogueManager.Instance.AddObserver(this); //Add self to dialogue managers list of observers
        Initialize();

    }
    private void Initialize()
    {
        templateResponseText.SetActive(false);
        responseText.Capacity = 6; //Set max response size
        for (int i = 0; i < responseText.Capacity; i++)
        {
            //templateResponseText.SetActive(true);
            GameObject _text = Instantiate(templateResponseText, this.transform);
            responseText.Add(_text);
            _text.SetActive(false);
        }

        responsePanel.SetActive(false);
    }

    private void GeneratePlayerResponses()
    {

        Cursor.lockState = CursorLockMode.Confined;
        int x = 1;
        int i = 0;

        ClearPlayerResponses(); //Reset responses to be re-written

        playerNameText.text = DialogueManager.Instance.GetCurrentResponseNode().playerName;

        foreach (var response in DialogueManager.Instance.GetCurrentResponseNode().playerResponses)
        {
            //If the responses conditions are true then generate the UI response
            if (response.conditionsTrue )
            {

                responseText[i].SetActive(true);
                responseText[i].GetComponent<TMPro.TMP_Text>().text = x + ".) " + response.responseText;
                x++;
                i++;
            }

        }
    }

    private void ClearPlayerResponses()
    {
        foreach (var response in responseText)
        {
            //If the responses conditions are true then generate the UI response
            response.SetActive(true);
            response.GetComponent<TMPro.TMP_Text>().text = "";
            response.SetActive(false);

        }
    }

    private void HideCurrentResponseInterface()
    {
        responsePanel.SetActive(false);
    }
    private void ShowCurrentResponseInterface()
    {

        responsePanel.SetActive(true);
        GeneratePlayerResponses();
    }
}
