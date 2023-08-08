using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// THIS STUFF SHOULD NOT BE ON UI 
/// </summary>
/// 


///Refactor this with an event/observer system. UI handling getting a bit coupled and hard to maintain!
public class PlayerResponseUI : MonoBehaviour, IDialogueObserver
{

   public PlayerResponse currentResponseNode;
    bool he = false;
    public GameObject templateResponseText;
    public GameObject responsePanel;

    private List<GameObject> responseText = new();


    void Start()
    {
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
               
                break;

            case DialogueState.TransitionNode:
                GeneratePlayerResponses();
                break;
        }
    }

    private void GeneratePlayerResponses()
    {
        PlayerResponse playerResponseNode = DialogueManager.Instance.GetCurrentResponseNode();
        currentResponseNode = playerResponseNode;
        Cursor.lockState = CursorLockMode.Confined;
        int x = 1;
        foreach (var response in currentResponseNode.playerResponses)
        {
            responseText[x - 1].SetActive(true);
            responseText[x - 1].GetComponent<TMPro.TMP_Text>().text = x + ".) " + response.responseText;
            x++;
        }
       // responsePanel.SetActive(true);
    }

    private void HideCurrentResponseInterface()
    {
        responsePanel.SetActive(false);
    }
    private void ShowCurrentResponseInterface()
    {

        responsePanel.SetActive(true);
    }
    private void ClearPlayerResponses()
    {
        //Remove from memory when this script is destroyed
        responseText.Clear();
    }

    private void OnDestroy()
    {
        ClearPlayerResponses();
    }
}
