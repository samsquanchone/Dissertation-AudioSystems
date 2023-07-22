using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// THIS STUFF SHOULD NOT BE ON UI 
/// </summary>
public class PlayerResponseUI : MonoBehaviour
{

    PlayerResponse currentResponseNode;
    bool he = false;
    public GameObject templateResponseText;
    public GameObject responsePanel;

    public void Initialize()
    {
        templateResponseText.SetActive(false);
    }

    public void GeneratePlayerResponses(PlayerResponse playerResponseNode)
    {
        currentResponseNode = playerResponseNode;
        Cursor.lockState = CursorLockMode.Confined;
        int x = 1;
        foreach (var response in currentResponseNode.playerResponses)
        {
            GameObject bar = templateResponseText;
            Instantiate(bar, this.transform);
            bar.GetComponent<TMPro.TMP_Text>().text = x + ". " + response.responseText;
            bar.SetActive(true);
            x++;
        }
        responsePanel.SetActive(true);
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
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {   
            //For now as there is one switch case in dalogue manager handling what to call, crate a temp single entry dic so we can pass that to the switch case function
            Entity npc = DialogueManager.Instance.GetNPCHashElement(currentResponseNode.npcID);
            Dictionary<uint, Line> line = new();
            line.Add(0 , npc.lines[currentResponseNode.playerResponses[0].npcLineID]);
            DialogueManager.Instance.PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Entity npc = DialogueManager.Instance.GetNPCHashElement(currentResponseNode.npcID);
            Dictionary<uint, Line> line = new();
            line.Add(0, npc.lines[currentResponseNode.playerResponses[1].npcLineID]);
            DialogueManager.Instance.PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent);
        }
    }
}
