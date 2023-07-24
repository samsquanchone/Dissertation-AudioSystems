using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu()]
public class PlayerResponse : ScriptableObject
{
    public string playerName;
    public string responseNodeName;
    public uint responseNodeID;
    public NodeTransitionMode nodeTransitionMode;
    public uint npcID;
    
    public List<PlayerResponseData> playerResponses;
    public FMODUnity.EventReference fmodEvent;

    public NodeTransitionCondition tranistonCondition;
  



    public void Start()
    {
        foreach (var response in this.playerResponses)
        {
            //Pass value that game designer has inputted into the private dynamic (dynamics cant be shown inspector, hence this method around it)
            response.conditionPassedValue = StringValidation.ConvertStringToDataType<dynamic>(response.conditionValue);
            Debug.Log(response.conditionPassedValue);
        }
    }


}


public enum NodeTransitionMode {CHOICE, DATA };

[System.Serializable]
public class PlayerResponseData
{
    public DialogueUtility.SequenceType sequenceType;
    public ConditionalLogicType conditiontype;
    public string conditionValue;
    public dynamic conditionPassedValue;
    public string responseText;
    public string gameDataConditionKey;
    public uint npcLineID;
    public uint conditionToNodeID;
    public PlayerResponse transitionNode;
    public bool isExitNode;

}

[System.Serializable]
public class NodeTransitionCondition
{
    public string gameDataConditionName;
    public ConditionalLogicType conditiontype;
    public dynamic conditionValue;
    public uint gameDataKey;
}
