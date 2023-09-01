using System.Collections.Generic;
using UnityEngine;
using DialogueUtility;
using Unity.Collections;
using Unity.Jobs;
using System.Collections;
using FMODUnity;
using DialogueSystem.DataResolver;
using DialogueSystem.DataStructure;

/// <summary>
/// Enum used with the event system so difffrent parts of the code base can make decisions on an event invoke based on the enum state passed with the invoke of dialogue obserbers
/// </summary>
public enum DialogueState { ConversationStart, ConversationEnd, DialogueStart, DialogueEnd, TransitionNode, InteractShow, PlayerResponse, InteractHide };


public interface DialogueSubject
{
    List<IDialogueObserver> dialogueObservers { get; set; }

    public void AddObserver(IDialogueObserver observer);
    public void RemoveObserver(IDialogueObserver observer);

    public void NotifyObservers(DialogueState state, SequenceType sequenceType, int instanceID);
}

/// <summary>
/// This is where the handling of a sequence of dialogue is handled, e.g. if just a random the queue size would be one, 
/// where with sequence it would be either: time of clip + buffer, or on input e.g. space 
/// </summary>
public class DialogueManager : MonoBehaviour, DialogueSubject
{

    ///Singleton decleration
    public static DialogueManager Instance => m_instance;
    private static DialogueManager m_instance;

    public List<IDialogueObserver> dialogueObservers { get; set; }
    public SubtitleManager subtitleManager;
    private LookAtNPC lookAt;
    public Transform player;


    [SerializeField] List<KeyCode> responseInputKeys = new(6);
    [SerializeField] KeyCode escDialogueKey;

    DialogueState state = DialogueState.ConversationEnd;
    private HashTable npcDialogueHashTable = new();
    private PlayerResponse currentResponseNode; //Not implement but should maybe handle this here instead of response UI script
    PlayerResponseData currentResponse;

    int currentEntityID; //Used so we can use the event system to only effect currently in effect dialogue entity

    bool isConversationActive = false;

    private void Awake()
    {
        //Singleton instatantiation
        m_instance = this;

        //Create a new list of dialogue observers
        dialogueObservers = new();
    }

    private void Start()
    {
        lookAt = GetComponent<LookAtNPC>(); //Get reference to the look at script 
    }

    /// <summary>
    /// Aims to avoid concurrancy issues with event system, 
    /// If ID matches currently interacted ID then only code will be executated for that script instance!
    /// </summary>
    /// <param name="id"> instance if of the currently interacted with NPC</param>
    public void SetInteractNPCID(int id)
    {
        currentEntityID = id;
    }

    /// <summary>
    /// Allows us to get the ID of a currently interacted with NPC, to dictate when to execute event based behaviour within other scripts 
    /// </summary>
    /// <returns></returns>
    public int GetCurrentInteractNPCID()
    {
        return currentEntityID;
    }


    /// <summary>
    /// Mainly used for player Input (Note: for the prototype currently utilising the default input system, plans to add new input system capabilities in the future
    /// </summary>
    private void Update()
    {
        if (state == DialogueState.PlayerResponse)
            if (Input.GetKeyDown(responseInputKeys[0]))
            {
                if (currentResponseNode.playerResponses.Count > 0 && currentResponseNode.playerResponses[0].conditionsTrue)
                {
                    SetCurrentResponse(currentResponseNode.playerResponses[0]);
                    //For now as there is one switch case in dalogue manager handling what to call, crate a temp single entry dic so we can pass that to the switch case function
                    Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                    Dictionary<uint, Line> line = new();
                    line.Add(0, npc.lines[currentResponseNode.playerResponses[0].npcLineID]);
                    PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null, currentEntityID);

                    //Transition node
                    if (currentResponseNode.playerResponses[0].transitionNode != null)
                    {

                        SetNewResponses(0);
                        NotifyObservers(DialogueState.DialogueStart, SequenceType.PlayerResponse, currentEntityID);

                    }
                }
            }

            if (Input.GetKeyDown(responseInputKeys[1]))
            {
                if (currentResponseNode.playerResponses.Count > 1 && currentResponseNode.playerResponses[1].conditionsTrue)
                {
                    SetCurrentResponse(currentResponseNode.playerResponses[1]);
                    Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                    Dictionary<uint, Line> line = new();
                    line.Add(0, npc.lines[currentResponseNode.playerResponses[1].npcLineID]);
                    PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null, currentEntityID); //NEED TO MOV THIS OR SORT TRANSFORM

                    //This should be refactored out of here
                    if (currentResponseNode.playerResponses[1].transitionNode != null)
                    {
                        SetNewResponses(1);
                        NotifyObservers(DialogueState.DialogueStart, SequenceType.PlayerResponse, currentEntityID);
                    }
                }
            }


            if (Input.GetKeyDown(responseInputKeys[2]))
            {
                if (currentResponseNode.playerResponses.Count > 2 && currentResponseNode.playerResponses[2].conditionsTrue)
                {
                    SetCurrentResponse(currentResponseNode.playerResponses[2]);
                    Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                    Dictionary<uint, Line> line = new();
                    line.Add(0, npc.lines[currentResponseNode.playerResponses[2].npcLineID]);
                    PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null, currentEntityID); //NEED TO MOV THIS OR SORT TRANSFORM

                   
                    if (currentResponseNode.playerResponses[2].transitionNode != null)
                    {

                        SetNewResponses(2);
                        NotifyObservers(DialogueState.DialogueStart, SequenceType.PlayerResponse, currentEntityID);

                    }
                }
            }


            if (Input.GetKeyDown(responseInputKeys[3]))
            {
                if (currentResponseNode.playerResponses.Count > 3 && currentResponseNode.playerResponses[3].conditionsTrue)
                {
                    SetCurrentResponse(currentResponseNode.playerResponses[3]);
                    Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                    Dictionary<uint, Line> line = new();
                    line.Add(0, npc.lines[currentResponseNode.playerResponses[3].npcLineID]);
                    PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null, currentEntityID); //NEED TO MOV THIS OR SORT TRANSFORM

               
                    if (currentResponseNode.playerResponses[3].transitionNode != null)
                    {

                        SetNewResponses(3);
                        NotifyObservers(DialogueState.DialogueStart, SequenceType.PlayerResponse, currentEntityID);

                    }
                }
            }


            if (Input.GetKeyDown(responseInputKeys[4]))
            {
                if (currentResponseNode.playerResponses.Count > 4 && currentResponseNode.playerResponses[4].conditionsTrue)
                {
                    SetCurrentResponse(currentResponseNode.playerResponses[4]);
                    Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                    Dictionary<uint, Line> line = new();
                    line.Add(0, npc.lines[currentResponseNode.playerResponses[4].npcLineID]);
                    PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null, currentEntityID); //NEED TO MOV THIS OR SORT TRANSFORM

                    //This should be refactored out of here
                    if (currentResponseNode.playerResponses[4].transitionNode != null)
                    {

                        SetNewResponses(4);
                        NotifyObservers(DialogueState.DialogueStart, SequenceType.PlayerResponse, currentEntityID);

                    }
                }
            }


            if (Input.GetKeyDown(responseInputKeys[5]))
            {
                if (currentResponseNode.playerResponses.Count > 5 && currentResponseNode.playerResponses[5].conditionsTrue)
                {
                    SetCurrentResponse(currentResponseNode.playerResponses[5]);
                    Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                    Dictionary<uint, Line> line = new();
                    line.Add(0, npc.lines[currentResponseNode.playerResponses[5].npcLineID]);
                    PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null, currentEntityID); //NEED TO MOV THIS OR SORT TRANSFORM

                    //This should be refactored out of here
                    if (currentResponseNode.playerResponses[5].transitionNode != null)
                    {

                        SetNewResponses(5);
                        NotifyObservers(DialogueState.DialogueStart, SequenceType.PlayerResponse, currentEntityID);

                    }
                }
            }
            else if (Input.GetKeyDown(escDialogueKey))
            {
                ExitConversation();
            }
    }

    /// <summary>
    /// Instantiate new player responses, created from the transition node of the player selected response index
    /// </summary>
    /// <param name="responseIndex">Reponse chosen from the list of responses for a response node</param>
    public void SetNewResponses(int responseIndex)
    {
        currentResponseNode = currentResponseNode.playerResponses[responseIndex].transitionNode;
        InstantiatePlayerResponseInterface(currentResponseNode, currentEntityID);
    }

    /// <summary>
    /// Called by each instance of an entity dialogue script. Note, this is called on start when an XML file is deserialized into its data containers
    /// </summary>
    /// <param name="entity"></param>
    public void AddEntityToHashTable(Entity entity)
    {
        //Add an entitys dialogue information within the scene to the hash table 
        npcDialogueHashTable.hashTable.Add(entity.id, entity);

        Entity _entity = (Entity)npcDialogueHashTable.hashTable[entity.id]; //NOTE NEED A BETTER WAY OF READING CONDITION ID: MAYBE ADD SOUND DESIGNER ID

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


    /// <summary>
    /// Globally accessible to allow notification of when a dialogue has finished, so the manager can notify its observers 
    /// </summary>
    public void ExitConversation()
    {
        isConversationActive = false;
        state = DialogueState.ConversationEnd;
        NotifyObservers(DialogueState.ConversationEnd, SequenceType.PlayerResponse, currentEntityID);
    }

    /// <summary>
    /// Notify observers that the interact UI is active 
    /// </summary>
    public void ShowInteractUI()
    {
        NotifyObservers(DialogueState.InteractShow, SequenceType.PlayerResponse, currentEntityID);
    }

    /// <summary>
    /// Notify observers that the interact UI has been disabled 
    /// </summary>
    public void HideInteractUI()
    {
        NotifyObservers(DialogueState.InteractHide, SequenceType.PlayerResponse, currentEntityID);
    }


    /// <summary>
    /// Get the current state of a conversation
    /// </summary>
    /// <returns></returns>
    public bool GetConversationState()
    {
        return isConversationActive;
    }

    /// <summary>
    /// Look at NPC when interacted with 
    /// </summary>
    /// <param name="npcTransform"> transform of the NPC being interacted with </param>
    public void LookAtNPC(Transform npcTransform)
    {
        Transform playerPos = player.transform;
        lookAt.LookAtTarget(playerPos, npcTransform);
    }


    /// <summary>
    /// Checks the conditions of currently of responses to instantiate, will check conditions and re-organise the list depending on which response has its conditoin set
    /// </summary>
    /// <param name="playerResponseNode">Response node to instantiate </param>
    /// <param name="instanceID">The ID of current NPC player is interacting with</param>
    public void InstantiatePlayerResponseInterface(PlayerResponse playerResponseNode, int instanceID)
    {
        currentEntityID = instanceID;
        isConversationActive = true;
        currentResponseNode = playerResponseNode;


        int i = 0;
        int conditionsTrueAmount = 0;
        List<PlayerResponseData> conditionsMetResponses = new();
        List<PlayerResponseData> conditionsNotMetResponses = new();

        //Check conditions for all new responses, then add to new list if condtitions met, then use altered list as new node respone list
        foreach (var response in playerResponseNode.playerResponses)
        {
            //As utilising SO, they have a serializable nature, hence needing to reset them 
            response.conditionsTrue = false;
            conditionsTrueAmount = 0;


            //If no conditions for response then set the UI
            if (response.condition.Count == 0)
            {
                response.conditionsTrue = true;
                conditionsMetResponses.Add(response);
            }

            //We need to ensure every condition is true to allow a response to show
            foreach (var condition in response.condition)
            {


                //Check current condition of current responses about to be generated
                if (CheckDialogueCondition<dynamic>(condition))
                {
                    conditionsTrueAmount++;

                }
                if (conditionsTrueAmount == response.condition.Count) //IF conditions true amount == conditions amount then we can trigger
                {
                    response.conditionsTrue = true;
                    conditionsMetResponses.Add(response);
                }


            }

            if (conditionsTrueAmount != response.condition.Count && response.condition.Count != 0)
            {
                conditionsNotMetResponses.Add(response);
            }

        }

        foreach (var _response in conditionsNotMetResponses)
        {
            conditionsMetResponses.Add(_response);
        }
        currentResponseNode.playerResponses = conditionsMetResponses;

        NotifyObservers(DialogueState.ConversationStart, SequenceType.PlayerResponse, currentEntityID);

        state = DialogueState.PlayerResponse;
    }


    /// <summary>
    /// Trigger dialogue of a sequence type, each dialogue line will be seqeunced, waiting the length of the dialogue clip before playing the next clip.
    /// Note: this type does not take conditions into account, even if stated within the XML file
    /// </summary>
    /// <param name="entityName">Name of the NPC who is triggering dialogue, will be provided to the subtitle system</param>
    /// <param name="lineSequence"> Dictionaty of dialogue lines contained within the sequence</param>
    /// <param name="sequenceType"></param>
    /// <param name="eventName">FMOD event with programmer instrument that will play the dialogue</param>
    /// <param name="transformToAttachTo"></param>
    /// <param name="instanceID"> Instance ID of the NPC triggering dialogue</param>
    public void PlayDialogueSequence(string entityName, Dictionary<uint, Line> lineSequence, SequenceType sequenceType, EventReference eventName, Transform transformToAttachTo, int instanceID)
    {

        currentEntityID = instanceID;

        isConversationActive = true;
        switch (sequenceType)
        {
            case SequenceType.Sequential:
                StartCoroutine(DialogueSequenceTimer(entityName, lineSequence, eventName, transformToAttachTo));
                break;

            case SequenceType.RandomOneShot:
                PlayRandomDialogue(entityName, lineSequence, eventName, transformToAttachTo);
                break;

            case SequenceType.PlayerResponse:
                StartCoroutine(DialogueResponseTimer(entityName, lineSequence[0], eventName, transformToAttachTo)); //Is one shot with one element id set to 0
                break;
        }


    }

    private IEnumerator DialogueSequenceTimer(string _name, Dictionary<uint, Line> lineSequence, EventReference eventName, Transform transformToAttachTo)
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
                        float diaLength = GetDialogueLength(line.Value.key, eventName);

                        DialogueHandler programmerCallback = new(line.Value.key, eventName, transformToAttachTo); //Make programmer deceleration in function, to make memory management better!!!
                        subtitleManager.QueueDialogue(line.Value.line, _name, diaLength, SequenceType.Sequential);
                        NotifyObservers(DialogueState.DialogueStart, SequenceType.Sequential, currentEntityID);

                        yield return new WaitForSecondsRealtime(diaLength); //We want a dialogue call back for info to get the length of an audio table clip.... UNITY WHY CANT I USE A DOUBLE >_< 
                    }
                }
            }
        }

        ExitConversation();

        yield return 0;
    }

    /// <summary>
    /// Allows external scripts to set the internal state of the manager for a current dialogue 
    /// </summary>
    /// <param name="_state"> Dialogue state to set the internal state to</param>
    public void SetState(DialogueState _state)
    {
        state = _state;
    }

    /// <summary>
    /// Sets the current response node of a player resonse 
    /// </summary>
    /// <param name="response"></param>
    public void SetCurrentResponse(PlayerResponseData response)
    {
        currentResponse = response;

    }


    /// <summary>
    /// Allows external classes to check the current response node
    /// </summary>
    /// <returns> Returns the current response node</returns>
    public PlayerResponse GetCurrentResponseNode()
    {
        return currentResponseNode;
    }


    /// <summary>
    /// This function is utilised to handle player response type dialogue
    /// </summary>
    /// <param name="_name"> name of the NPC triggering dialogue for, is provided to the subtilte manager </param>
    /// <param name="npcLine"> current line that will be trigger with the player response</param>
    /// <param name="eventName">fmod event</param>
    /// <param name="transformToAttachTo"> transform that will be provided to FMOD if 3D event</param>
    /// <returns></returns>
    private IEnumerator DialogueResponseTimer(string _name, Line npcLine, EventReference eventName, Transform transformToAttachTo)
    {

        state = DialogueState.DialogueStart;

        float diaLength = GetDialogueLength(npcLine.key, eventName);

        if (diaLength == 0) { diaLength = 2; } //On first trigger on occasion the callback with fmod does not calculate accurate length,  this just ensures there is a default val the first time
        Debug.Log("dialength: " + diaLength);

        DialogueHandler programmerCallback = new(npcLine.key, eventName, transformToAttachTo);

        subtitleManager.QueueDialogue(npcLine.line, _name, diaLength, SequenceType.PlayerResponse);
        NotifyObservers(DialogueState.DialogueStart, SequenceType.PlayerResponse, currentEntityID);
        yield return new WaitForSeconds(diaLength);


        //Iterate through all responses event list and invoke all events
        foreach (var _event in currentResponse.eventsList)
        {
            _event.Invoke();
        }

        if (!currentResponse.isExitNode)
        {
            NotifyObservers(DialogueState.DialogueEnd, SequenceType.PlayerResponse, currentEntityID);
            NotifyObservers(DialogueState.PlayerResponse, SequenceType.PlayerResponse, currentEntityID);
            state = DialogueState.PlayerResponse;
        }
    }


    /// <summary>
    /// This function handles the triggering of a random one shot type. 
    /// It checks conditions of XML lines for entity and complies list of triggerable lines to get a random index to trigger
    /// </summary>
    /// <param name="entityName">Name of the NPC</param>
    /// <param name="lineSequence">List of lines</param>
    /// <param name="eventName"> Fmod event</param>
    /// <param name="transformToAttachTo">Transform to attach to if 3D dialogue type</param>
    private void PlayRandomDialogue(string entityName, Dictionary<uint, Line> lineSequence, EventReference eventName, Transform transformToAttachTo)
    {
        List<Line> triggerableLines = new();

        //Check each condition and create a list of triggerable lines 
        foreach (var line in lineSequence)
        {
            int conditionTrueAmount = 0;

            //Check each condition, if all true then add to list of triggerable one-shots
            foreach (var condition in line.Value.conditions)
            {
                if (CheckDialogueCondition(condition.Value))
                {
                    conditionTrueAmount++;

                }
            }
            if (conditionTrueAmount == line.Value.conditions.Count)
            {
                triggerableLines.Add(line.Value);
            }
            //If line has no conditions then add 
            else if (line.Value.conditions.Count == 0)
            {
                triggerableLines.Add(line.Value);
            }
        }

        //If there are triggerable lines, get a random index and trigger it
        if (triggerableLines.Count != 0)
        {
            int indexToPlay = Random.Range(0, triggerableLines.Count);

            float diaLength = GetDialogueLength(triggerableLines[indexToPlay].key, eventName);

            if (diaLength == 0) { diaLength = 2; } //Just a fallback state incase FMOD can't calculate the length
            Debug.Log("dialength: " + diaLength);

            DialogueHandler programmerCallback = new(triggerableLines[indexToPlay].key, eventName, transformToAttachTo);
            NotifyObservers(DialogueState.DialogueStart, SequenceType.RandomOneShot, currentEntityID);
            subtitleManager.QueueDialogue(triggerableLines[indexToPlay].line, entityName, diaLength, SequenceType.RandomOneShot);

        }
    }

    /// <summary>
    /// Utilise the Unity Job system (threading), to ensure that we get the length before a dialogue is triggered
    /// </summary>
    /// <param name="dialogueKey"> key for the dialogue line to get length for </param>
    /// <param name="eventName"> fmod event with programmer instrument on for callback</param>
    /// <returns></returns>
    private float GetDialogueLength(string dialogueKey, EventReference eventName)
    {
        //Create native array to store threading result in
        NativeArray<float> result = new(1, Allocator.TempJob);

        //Set up a new job system instance
        DialogueJobSystem jobSystem = new(dialogueKey, eventName);

        //Create a dialogue length calc thread 
        DialogueJobSystem.CalculateDialogueLength dialogueLength = new();
        dialogueLength.result = result;

        //Schedule thread
        JobHandle handle = dialogueLength.Schedule();

        //Wait for thread to complete 
        handle.Complete();

        //Store result of thread task and convert from MS to S
        float diaLength = result[0] / 1000;

        //Free memory 
        result.Dispose();

        return diaLength;
    }

    /// <summary>
    /// Used to notify the dialogue system that a dialogue line has ended so the manager can notify its observers 
    /// </summary>
    /// <param name="sequenceType">Sequence type for the triggerd line</param>
    public void DialogueEnded(SequenceType sequenceType)
    {
        NotifyObservers(DialogueState.DialogueEnd, sequenceType, currentEntityID);

        if (currentResponse != null)
        {
            if (currentResponse.isExitNode && sequenceType == SequenceType.PlayerResponse)
            {
                NotifyObservers(DialogueState.ConversationEnd, sequenceType, currentEntityID);
            }
        }
      
    }

    /// <summary>
    /// Used to check the conditions of a line. Note: two objects for conditions, both XML and Unity authored (very similar, but enum is exposed to users for unity one), same variable names so generics to cast type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="diaCondition"></param>
    /// <returns></returns>
    public bool CheckDialogueCondition<T>(T diaCondition)
    {

        /*Player response data is Scriptable object driven, while the NPC XML system is driven by files and the data is contained in more complex collections, which can't be shown within the inspector.
         Generic object casting used to determine type and provide respective conditions to the game condition query, without needing to repeat this function for a seperate data structure */
        dynamic condition = diaCondition;
        if (typeof(T).Equals(typeof(Condition)))
        {
            condition = (Condition)(object)diaCondition;
        }
        else if (typeof(T).Equals(typeof(NodeCondition)))
        {
            condition = (NodeCondition)(object)diaCondition;
        }

        GameDataReturnType dataResolveType = GameDataResolver.Instance.QuerryGameData<dynamic>(condition.gameDataKey, out dynamic gameDataVal);

        switch (condition.triggerCondition)
        {
            case ConditionalLogicType.GreaterThan:
                if (gameDataVal > condition.conditionValue) { return true; }
                break;

            case ConditionalLogicType.LessThan:
                if (gameDataVal < condition.conditionValue) { return true; }
                break;

            case ConditionalLogicType.True:
                if (gameDataVal) { return true; }
                break;

            case ConditionalLogicType.False:
                if (!gameDataVal) { return true; }
                break;

        }

        return false;
    }

    /// <summary>
    /// Function that allows external classes that inherit from IDialogueObserver to add themselves to this managers list of observers,
    /// to provide event based notifications to them
    /// </summary>
    /// <param name="observer">An observer which inherits from the observer interface</param>
    public void AddObserver(IDialogueObserver observer)
    {
        dialogueObservers.Add(observer);
    }

    /// <summary>
    /// Function that allows external classes to remove themselves as observers. Generally done when scripts are destroyed
    /// </summary>
    /// <param name="observer"> Observer to remove </param>
    public void RemoveObserver(IDialogueObserver observer)
    {
        dialogueObservers.Remove(observer);
    }


    /// <summary>
    /// Function that will notify observers when something critical happens, providing arguments that allow observers to determine which behaviour to invoke
    /// </summary>
    /// <param name="state"> current dialogue state e.g. dialogue line started</param>
    /// <param name="sequenceType">Sequence type for the current conversation</param>
    /// <param name="instanceID">Unique ID for instance of entity script that is being interacted with, to increase memory management and performance</param>
    public void NotifyObservers(DialogueState state, SequenceType sequenceType, int instanceID)
    {
        foreach (var observer in dialogueObservers)
        {
            observer.OnNotify(state, sequenceType, currentEntityID);
        }
    }
}
