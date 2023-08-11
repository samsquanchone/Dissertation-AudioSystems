using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu()]
public class PlayerResponse : ScriptableObject
{
    public string playerName;
    public string responseNodeName;
    public NodeTransitionMode nodeTransitionMode; //Maybe just get rid of this, can have natural node progression and line back and fourth, back to bass node
    public uint npcID;
    
    public List<PlayerResponseData> playerResponses;
    public FMODUnity.EventReference fmodEvent;

    public PlayerResponse transitionTo;
    public NodeCondition tranistonCondition;

 
}


public enum NodeTransitionMode {CHOICE, DATA };

[System.Serializable]
public class PlayerResponseData
{
   
    
    public string responseText;
    public List<NodeCondition> condition;
    public uint npcLineID;
    public PlayerResponse transitionNode;
    public bool isExitNode;
    public List<UnityEvent> eventsList; //Use this to be able to give quest ect ect
    [HideInInspector] public bool conditionsTrue = false;

}


//Would just use the Condition object, however it contains data types that are not shown in inspector by default
[System.Serializable]
public class NodeCondition
{
    public string gameDataConditionName;
    public ConditionalLogicType triggerCondition;
    public string conditionToParse;
    public dynamic conditionValue;
    public uint gameDataKey;
}
