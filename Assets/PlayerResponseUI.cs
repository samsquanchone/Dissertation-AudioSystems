using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// THIS STUFF SHOULD NOT BE ON UI 
/// </summary>
/// 


///Refactor this with an event/observer system. UI handling getting a bit coupled and hard to maintain!
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
            _text.SetActive(false);
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
            DialogueManager.Instance.PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null);

            //This should be refactored out of here
            if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[0].transitionNode != null)
            {

                SetNewResponses(0);
                GeneratePlayerResponses(currentResponseNode);
               
            }

        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DialogueManager.Instance.SetCurrentResponse(currentResponseNode.playerResponses[1]);
            Entity npc = DialogueManager.Instance.GetNPCHashElement(currentResponseNode.npcID);
            Dictionary<uint, Line> line = new();
            line.Add(0, npc.lines[currentResponseNode.playerResponses[1].npcLineID]);
            DialogueManager.Instance.PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null); //NEED TO MOV THIS OR SORT TRANSFORM

            //This should be refactored out of here
            if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[1].transitionNode != null)
            {

                SetNewResponses(1);
                GeneratePlayerResponses(currentResponseNode);

            }
        }
    }

    private void SetNewResponses(int responseIndex)
    {
        //NOTE MAY NEED TO ADD CHECK NODE CONDITION AS WELL
        //PUT THIS IN A CO-ROUTINE AS WELL AS ANY PARSING NOT AT START, AS IT CAN BE EXPENSIVE!!
        currentResponseNode = currentResponseNode.playerResponses[responseIndex].transitionNode;
        foreach (var response in currentResponseNode.playerResponses)
        {
            //Pass value that game designer has inputted into the private dynamic (dynamics cant be shown inspector, hence this method around it)
            response.condition.conditionValue = StringValidation.ConvertStringToDataType<dynamic>(response.condition.conditionToParse);
            Debug.Log(response.condition.conditionToParse);
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
