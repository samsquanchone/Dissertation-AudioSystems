using System.Collections.Generic;
using UnityEngine;
using DialogueUtility;




/// <summary>
/// This is where the handling of a sequence of dialogue is handled, e.g. if just a random the queue size would be one, 
/// where with sequence it would be either: time of clip + buffer, or on input e.g. space 
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance => m_instance;
    private static DialogueManager m_instance;
    public SubtitleManager subtitleManager;
    public PlayerResponseUI playerResponseUI;
    public List<TestXML> npcs = new List<TestXML>();

    public int test;
    public string stringtest;
    public float floatTest;

    //Create an instace of our hashTableObject with abstracted hash handling functionality, for both npcsDialogue + gamedata
    private HashTable gameDataHashTable = new();
    private HashTable npcDialogueHashTable = new();

    public GameDataResolver resolver;

    private PlayerResponse currentResponseNode; //Not implement but should maybe handle this here instead of response UI script
    PlayerResponseData currentResponse;

    bool isConversationActive = false;

    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        playerResponseUI.Initialize();
       
    }

    public void AddEntityToHashTable(Entity entity)
    {
        //Add an entitys dialogue information within the scene to the hash table 
        npcDialogueHashTable.hashTable.Add(uint.Parse(entity.id), entity);

        Entity _entity = (Entity)npcDialogueHashTable.hashTable[uint.Parse(entity.id)]; //NOTE NEED A BETTER WAY OF READING CONDITION ID: MAYBE ADD SOUND DESIGNER ID
        Debug.Log("line id: " +  _entity.lines[(uint)1].lineID);


    }

    /// <summary>
    /// Returns an entity object from npcentity hash table
    /// </summary>
    /// <param name="entityKey"></param>
    ///  Key used to access specified hash element for desired npc
    /// <returns> Returns an entity object which is key element from hashtable</returns>
    public Entity GetNPCHashElement(uint entityKey)
    {
        Entity entity = (Entity)npcDialogueHashTable.hashTable[entityKey];

        return entity;
    }

    public void ExitConversation()
    {
        isConversationActive = false;
        subtitleManager.HideAllUI();
        Debug.Log("QUIT");

    }


    public void ShowInteractUI()
    {

        subtitleManager.ShowNPCInteractUI();
    }

    public bool GetConversationState()
    {
        return isConversationActive;
    }

    public void InstantiatePlayerResponseInterface(PlayerResponse playerResponseNode)
    {
        isConversationActive = true;
        currentResponseNode = playerResponseNode;
        subtitleManager.HideInteractionUI();
        playerResponseUI.GeneratePlayerResponses(playerResponseNode);
        playerResponseUI.ShowCurrentResponseInterface();
        subtitleManager.ShowSubtitleInterface();
    }

    public void PlayDialogueSequence(string entityName, Dictionary<uint, Line> lineSequence, SequenceType sequenceType, FMODUnity.EventReference eventName)
    {



        // Queue dialogueLineSequence = new();


        isConversationActive = true;
        switch (sequenceType)
        {
            case SequenceType.Sequential:
                StartCoroutine(DialogueSequenceTimer(entityName, lineSequence, eventName));
                break;

            case SequenceType.RandomOneShot:
                PlayRandomDialogue(entityName, lineSequence, eventName);
                break;

            case SequenceType.PlayerResponse:
                StartCoroutine(DialogueResponseTimer(entityName, lineSequence[0], eventName)); //Is one shot with one element id set to 0
                Debug.Log("Set up");
                break;
        }

        //Create a queue from the list of dialogue lines passed to the manager
        foreach (var line in lineSequence)
        {



            //Add to queue for each line in an entity 
            //dialogueLineSequence.Enqueue(line);
            //Could handle dialogue here and use a coroutine to to queue the dialogue and text properlly, get fmod sounds length

        }

        // dialogueLineSequence.Elemen

        //Print queue after line is populated 
        //subtitleManager.QueueDialogue(dialogueLineSequence, npcName);

    }

    private System.Collections.IEnumerator DialogueSequenceTimer(string _name, Dictionary<uint, Line> lineSequence, FMODUnity.EventReference eventName)
    {

        foreach (var line in lineSequence)
        {
            int i = 0;
            foreach (var condition in line.Value.conditions)
            {

                if (CheckDialogueCondition(condition.Value))
                {
                    i++;
                    if (i == line.Value.conditions.Count) //IF all conditions true i.e at end element + all conditions are met 
                    {
                        //Play dialogue
                        DialogueInfoHandler diaInfoCallback = new(line.Value.key, eventName);

                        double diaLength = diaInfoCallback.GetDialogueLength() / 1000; //This will return MS, for now we use seconds conversion, but if there are timing issues, maybe revert back to MS
                        //  Debug.Log("Length: " + diaInfoCallback.GetDialogueLength());
                        DialogueHandler programmerCallback = new(line.Value.key, eventName, null); //Make programmer deceleration in function, to make memory management better!!!
                        subtitleManager.QueueDialogue(line.Value.line, _name, (float)diaLength);


                        yield return new WaitForSeconds((float)diaLength); //We want a dialogue call back for info to get the length of an audio table clip.... UNITY WHY CANT I USE A DOUBLE >_< 
                    }


                }


            }
        }

        ExitConversation();

        yield return 0;
    }

    public void SetCurrentResponse(PlayerResponseData response)
    {
        currentResponse = response;
    }

    private System.Collections.IEnumerator DialogueResponseTimer(string _name, Line npcLine, FMODUnity.EventReference eventName)
    {
        playerResponseUI.HideCurrentResponseInterface();


        DialogueInfoHandler diaInfoCallback = new(npcLine.key, eventName);

        double diaLength = diaInfoCallback.GetDialogueLength() / 1000; //This will return MS, for now we use seconds conversion, but if there are timing issues, maybe revert back to MS


        if (diaLength == 0) { diaLength = 2; } //On first trigger on occasion the callback with fmod does not happen, this just ensures there is a default val the first time
        Debug.Log("dialength: " + diaLength);


        DialogueHandler programmerCallback = new(npcLine.key, eventName, null); //Make programmer deceleration in function, to make memory management better!!!

        subtitleManager.QueueDialogue(npcLine.line, _name, (float)diaLength);
        yield return new WaitForSeconds((float)diaLength);


        if (!playerResponseUI.IsExitResponse(currentResponse))
        {

            playerResponseUI.ShowCurrentResponseInterface();
        }

        else if (playerResponseUI.IsExitResponse(currentResponse))
        {
            ExitConversation();
        }
    }

    private void PlayRandomDialogue(string entityName, Dictionary<uint, Line> lineSequence, FMODUnity.EventReference eventName)
    {
        List<Line> triggerableLines = new();

        foreach (var line in lineSequence)
        {
            foreach (var condition in line.Value.conditions)
            {
                if (CheckDialogueCondition(condition.Value))
                {
                    triggerableLines.Add(line.Value);
                    Debug.Log("Key for lne: " + line.Value.key);
                }
            }
        }

        int indexToPlay = Random.Range(0, triggerableLines.Count);
        DialogueHandler programmerCallback = new(triggerableLines[indexToPlay].key, eventName, null); //Make programmer deceleration in function, to make memory management better!!!
        subtitleManager.QueueDialogue(triggerableLines[indexToPlay].line, entityName, 3f);
        //  Debug.Log("Length: " + diaInfoCallback.GetDialogueLength());
    }

    

    public bool CheckDialogueCondition(Condition diaCondition)
    {
        //GameDataResolver resolver = new();
        GameDataReturnType dataResolveType = resolver.QuerryGameData<dynamic>(diaCondition.gameDataKey, out dynamic gameDataVal);

        switch (diaCondition.triggerCondition)
        {
            case ConditionalLogicType.GreaterThan:
                if (gameDataVal > diaCondition.conditionValue) { return true; }
                break;

            case ConditionalLogicType.LessThan:
                if (gameDataVal < diaCondition.conditionValue) { return true; }
                break;

            case ConditionalLogicType.True:
                if (gameDataVal) { return true; }
                break;

            case ConditionalLogicType.False:
                if (!gameDataVal) { return true; }
                break;
        }

        Debug.Log("Resolution: " + dataResolveType.ToString() + " /  gameDataVal: " + gameDataVal + "ConditionVAL: " + diaCondition.conditionValue);
        return false;
    }
}
