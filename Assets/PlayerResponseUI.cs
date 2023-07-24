using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// THIS STUFF SHOULD NOT BE ON UI 
/// </summary>
public class PlayerResponseUI : MonoBehaviour
{

   public PlayerResponse currentResponseNode;
    bool he = false;
    public GameObject templateResponseText;
    public GameObject responsePanel;

    private List<GameObject> responseText = new();

    public void Initialize()
    {
        templateResponseText.SetActive(false);
        responseText.Capacity = 6; //Set max response size
        for (int i = 0; i < responseText.Capacity; i++)
        {
            //templateResponseText.SetActive(true);
            GameObject _text = Instantiate(templateResponseText, this.transform);
            responseText.Add(_text);
            _text.SetActive(true);
            
        }

        
    }

    public void GeneratePlayerResponses(PlayerResponse playerResponseNode)
    {
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

   

    public void FixedUpdate()
    {
  
    }

    public void HideCurrentResponseInterface()
    {
        responsePanel.SetActive(false);
    }
    public void ShowCurrentResponseInterface()
    {
        responsePanel.SetActive(true);
    }
    public void ClearPlayerResponses()
    {
        //Remove from memory when this script is destroyed
        responseText.Clear();
    }

    public bool IsExitResponse(PlayerResponseData response)
    {
        switch (response.isExitNode)
        {
            case true:

                return true;

            case false:
                return false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            DialogueManager.Instance.SetCurrentResponse(currentResponseNode.playerResponses[0]);
            //For now as there is one switch case in dalogue manager handling what to call, crate a temp single entry dic so we can pass that to the switch case function
            Entity npc = DialogueManager.Instance.GetNPCHashElement(currentResponseNode.npcID);
            Dictionary<uint, Line> line = new();
            line.Add(0 , npc.lines[currentResponseNode.playerResponses[0].npcLineID]);
            DialogueManager.Instance.PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent);

            if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[0].transitionNode != null)
            {  
                currentResponseNode = currentResponseNode.playerResponses[0].transitionNode;
                GeneratePlayerResponses(currentResponseNode);
               
            }

        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DialogueManager.Instance.SetCurrentResponse(currentResponseNode.playerResponses[1]);
            Entity npc = DialogueManager.Instance.GetNPCHashElement(currentResponseNode.npcID);
            Dictionary<uint, Line> line = new();
            line.Add(0, npc.lines[currentResponseNode.playerResponses[1].npcLineID]);
            DialogueManager.Instance.PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent);
        }
    }

    public void TransitionToPlayerResponseNode()
    {
        
    }

    private void OnDestroy()
    {
        ClearPlayerResponses();
    }
}
