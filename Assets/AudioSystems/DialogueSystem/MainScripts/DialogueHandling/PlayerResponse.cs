using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Data structures for player response info
/// 
/// Author: Sam Scott
/// </summary>
[System.Serializable]
[CreateAssetMenu()]
public class PlayerResponse : ScriptableObject
{
    [Tooltip ("Name of the Player, will be provided to the subtitle system")]
    public string playerName;

    [Tooltip("Name of the node, to aid in identification, no technical purpose")]
    public string responseNodeName;

    [Tooltip("Dialogue hashtable ID for the NPC you would like the player to interact with")]
    public uint npcID;

    [Tooltip("Player responses you would like to author (Note: CURRENT MAXIMUM OF 6 RESPONSES SUPPORTED)")]
    public List<PlayerResponseData> playerResponses;

    [Tooltip("The FMOD event that will be utilised for triggering dialogue through the programmer instrument)")]
    public FMODUnity.EventReference fmodEvent;

    [Tooltip("the next node that you would like to transition to when its condition is met")]
    public PlayerResponse transitionTo;

    [Tooltip("Condition to transition the node")]
    public NodeCondition tranistonCondition;

}

[System.Serializable]
public class PlayerResponseData
{
    [Tooltip("Text for the player response, will be provided to the subtitle system")]
    public string responseText;

    [Tooltip("List of conditions that will need to be met before a response will be shown (Note: Currently conditions are only of an OR logic type and do not support AND types)")]
    public List<NodeCondition> condition;

    [Tooltip("ID of the desired NPC line that you would like this response to trigger from its hash table entry")]
    public uint npcLineID;

    [Tooltip("Response node you would like the response to branch to. Note: Upon exiting the conversation, the node will return to its base node (Unless transition condition has been met)")]
    public PlayerResponse transitionNode;

    [Tooltip("Will exit conversation automatically if enabled")]
    public bool isExitNode;

    [Tooltip("Allows you to invoke seperate scripts when response triggered. Note: must be prefab'd behaviour due to the nature of scriptable objects")]
    public List<UnityEvent> eventsList; //Use this to be able to give quest ect ect
    [HideInInspector] public bool conditionsTrue = false; //Used for determining whether conditions are met for a response so the UI can show it or not
}


//Would just use the Condition object, however it contains data types that are not shown in inspector by default
[System.Serializable]
public class NodeCondition
{
    [Tooltip("Name of the game data, for human identification purposes only")]
    public string gameDataConditionName;

    [Tooltip("Conditional logic that you would like the condition to utilise. Note: do not use unkown type, this is for internal system purposes and not authoring!")]
    public ConditionalLogicType triggerCondition;

    [Tooltip("Value to parse: int,float, bool. Note: please populate even if using bool")]
    public string conditionToParse;
    public dynamic conditionValue;

    [Tooltip("Game data hashtable ID for checking a condition with its respective game data value")]
    public uint gameDataKey;
}
