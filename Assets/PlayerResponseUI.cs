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

    private void OnDestroy()
    {
        ClearPlayerResponses();
    }
}
