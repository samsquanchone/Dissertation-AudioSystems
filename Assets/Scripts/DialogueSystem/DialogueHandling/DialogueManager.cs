using System.Collections.Generic;
using UnityEngine;
using DialogueUtility;
using Unity.Collections;
using Unity.Jobs;

/// <summary>
/// Enum used with the event system so difffrent parts of the code base can make decisions on an event invoke based on the enum state passed with the invoke of dialogue obserbers
/// </summary>
public enum DialogueState {ConversationStart, ConversationEnd, DialogueStart, DialogueEnd, TransitionNode, InteractShow, PlayerResponse};


public interface DialogueSubject
{
    List<IDialogueObserver> dialogueObservers { get; set; }

    public void AddObserver(IDialogueObserver observer);
    public void RemoveObserver(IDialogueObserver observer);

    public void NotifyObservers(DialogueState state);
    
}

/// <summary>
/// This is where the handling of a sequence of dialogue is handled, e.g. if just a random the queue size would be one, 
/// where with sequence it would be either: time of clip + buffer, or on input e.g. space 
/// </summary>
public class DialogueManager : MonoBehaviour, DialogueSubject
{
    public static DialogueManager Instance => m_instance;
    private static DialogueManager m_instance;
    public List<IDialogueObserver> dialogueObservers { get; set; }
    public SubtitleManager subtitleManager;
    private LookAtNPC lookAt;
    public Transform player;

    DialogueState state = DialogueState.ConversationEnd;
    private HashTable npcDialogueHashTable = new();
    private PlayerResponse currentResponseNode; //Not implement but should maybe handle this here instead of response UI script
    PlayerResponseData currentResponse;

    bool isConversationActive = false;

    private void Awake()
    {
        m_instance = this;
        dialogueObservers = new();
    }

    private void Start()
    {
      
       
        lookAt = GetComponent<LookAtNPC>();

    }

    private void Update()
    {
        if(state == DialogueState.PlayerResponse)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentResponseNode.playerResponses.Count > 0)
            {
                SetCurrentResponse(currentResponseNode.playerResponses[0]);
                //For now as there is one switch case in dalogue manager handling what to call, crate a temp single entry dic so we can pass that to the switch case function
                Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                Dictionary<uint, Line> line = new();
                line.Add(0, npc.lines[currentResponseNode.playerResponses[0].npcLineID]);
                PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null);

                //Transition node
                if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[0].transitionNode != null)
                {

                    SetNewResponses(0);
                    NotifyObservers(DialogueState.TransitionNode);

                }
            }
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentResponseNode.playerResponses.Count > 1)
            {
                SetCurrentResponse(currentResponseNode.playerResponses[1]);
                Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                Dictionary<uint, Line> line = new();
                line.Add(0, npc.lines[currentResponseNode.playerResponses[1].npcLineID]);
                PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null); //NEED TO MOV THIS OR SORT TRANSFORM

                //This should be refactored out of here
                if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[1].transitionNode != null)
                {

                    SetNewResponses(1);
                    NotifyObservers(DialogueState.TransitionNode);

                }
            }
        }


        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentResponseNode.playerResponses.Count > 2)
            {
                SetCurrentResponse(currentResponseNode.playerResponses[2]);
                Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                Dictionary<uint, Line> line = new();
                line.Add(0, npc.lines[currentResponseNode.playerResponses[2].npcLineID]);
                PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null); //NEED TO MOV THIS OR SORT TRANSFORM

                //This should be refactored out of here
                if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[2].transitionNode != null)
                {

                    SetNewResponses(2);
                    NotifyObservers(DialogueState.TransitionNode);

                }
            }
        }


        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (currentResponseNode.playerResponses.Count > 3)
            {
                SetCurrentResponse(currentResponseNode.playerResponses[3]);
                Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                Dictionary<uint, Line> line = new();
                line.Add(0, npc.lines[currentResponseNode.playerResponses[3].npcLineID]);
                PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null); //NEED TO MOV THIS OR SORT TRANSFORM

                //This should be refactored out of here
                if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[3].transitionNode != null)
                {

                    SetNewResponses(3);
                    NotifyObservers(DialogueState.TransitionNode);

                }
            }
        }


        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (currentResponseNode.playerResponses.Count > 4)
            {
                SetCurrentResponse(currentResponseNode.playerResponses[4]);
                Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                Dictionary<uint, Line> line = new();
                line.Add(0, npc.lines[currentResponseNode.playerResponses[4].npcLineID]);
                PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null); //NEED TO MOV THIS OR SORT TRANSFORM

                //This should be refactored out of here
                if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[4].transitionNode != null)
                {

                    SetNewResponses(4);
                    NotifyObservers(DialogueState.TransitionNode);

                }
            }
        }


        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (currentResponseNode.playerResponses.Count > 5)
            {
                SetCurrentResponse(currentResponseNode.playerResponses[5]);
                Entity npc = GetNPCHashElement(currentResponseNode.npcID);
                Dictionary<uint, Line> line = new();
                line.Add(0, npc.lines[currentResponseNode.playerResponses[5].npcLineID]);
                PlayDialogueSequence(npc.name, line, DialogueUtility.SequenceType.PlayerResponse, currentResponseNode.fmodEvent, null); //NEED TO MOV THIS OR SORT TRANSFORM

                //This should be refactored out of here
                if (currentResponseNode.nodeTransitionMode == NodeTransitionMode.CHOICE && currentResponseNode.playerResponses[5].transitionNode != null)
                {

                    SetNewResponses(5);
                    NotifyObservers(DialogueState.TransitionNode);

                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitConversation();
        }
    }


    public void SetNewResponses(int responseIndex)
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

    public void AddEntityToHashTable(Entity entity)
    { 
        //Add an entitys dialogue information within the scene to the hash table 
        npcDialogueHashTable.hashTable.Add(uint.Parse(entity.id), entity);

        Entity _entity = (Entity)npcDialogueHashTable.hashTable[uint.Parse(entity.id)]; //NOTE NEED A BETTER WAY OF READING CONDITION ID: MAYBE ADD SOUND DESIGNER ID
       
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
        state = DialogueState.ConversationEnd;
        NotifyObservers(DialogueState.ConversationEnd);
        
    }


    public void ShowInteractUI()
    {
        NotifyObservers(DialogueState.InteractShow);
        ///subtitleManager.ShowNPCInteractUI();
    }

    public bool GetConversationState()
    {
        return isConversationActive;
    }

    public void LookAtNPC(Transform npcTransform)
    {
        lookAt.LookAtTarget(player, npcTransform);
    }
    public void InstantiatePlayerResponseInterface(PlayerResponse playerResponseNode)
    {
        
        isConversationActive = true;
        currentResponseNode = playerResponseNode;
       
        NotifyObservers(DialogueState.ConversationStart);
        state = DialogueState.PlayerResponse;
       
  
    }

    public void PlayDialogueSequence(string entityName, Dictionary<uint, Line> lineSequence, SequenceType sequenceType, FMODUnity.EventReference eventName, Transform transformToAttachTo)
    {



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
                Debug.Log("Set up");
                break;
        }


    }

    private System.Collections.IEnumerator DialogueSequenceTimer(string _name, Dictionary<uint, Line> lineSequence, FMODUnity.EventReference eventName, Transform transformToAttachTo)
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
                        //Create native array to store threading result in
                        NativeArray<float> result = new NativeArray<float>(1, Allocator.TempJob);

                        //Set up a new job system instance
                        DialogueJobSystem jobSystem = new(line.Value.key, eventName);

                       
                       
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
                        //  Debug.Log("Length: " + diaInfoCallback.GetDialogueLength());
                        DialogueHandler programmerCallback = new(line.Value.key, eventName, transformToAttachTo); //Make programmer deceleration in function, to make memory management better!!!
                        subtitleManager.QueueDialogue(line.Value.line, _name, diaLength);


                        yield return new WaitForSecondsRealtime((float)diaLength); //We want a dialogue call back for info to get the length of an audio table clip.... UNITY WHY CANT I USE A DOUBLE >_< 
                    }


                }


            }
        }

        ExitConversation();

        yield return 0;
    }


    public void SetState(DialogueState _state)
    {
        state = _state;
    }
    public void SetCurrentResponse(PlayerResponseData response)
    {
        currentResponse = response;

    }

    public PlayerResponse GetCurrentResponseNode()
    {
        return currentResponseNode;
    }

    private System.Collections.IEnumerator DialogueResponseTimer(string _name, Line npcLine, FMODUnity.EventReference eventName, Transform transformToAttachTo)
    {

        NotifyObservers(DialogueState.DialogueStart);
        state = DialogueState.DialogueStart;
       // playerResponseUI.HideCurrentResponseInterface();

        //Create native array to store threading result in
        NativeArray<float> result = new NativeArray<float>(1, Allocator.TempJob);

        //Set up a new job system instance
        DialogueJobSystem jobSystem = new(npcLine.key, eventName);
        
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


        DialogueInfoHandler diaInfoCallback = new(npcLine.key, eventName);


        if (diaLength == 0) { diaLength = 2; } //On first trigger on occasion the callback with fmod does not happen, this just ensures there is a default val the first time
        Debug.Log("dialength: " + diaLength);


        DialogueHandler programmerCallback = new(npcLine.key, eventName, transformToAttachTo);

        subtitleManager.QueueDialogue(npcLine.line, _name, diaLength);
        yield return new WaitForSeconds((float)diaLength);


        //Iterate through all responses event list and invoke all events
        foreach (var _event in currentResponse.eventsList)
        {
            _event.Invoke();
        }

        if (!currentResponse.isExitNode)
        {
            NotifyObservers(DialogueState.DialogueEnd);
            state = DialogueState.PlayerResponse;
        }

        else if (currentResponse.isExitNode)
        {
            ExitConversation();
        }
    }

    private void PlayRandomDialogue(string entityName, Dictionary<uint, Line> lineSequence, FMODUnity.EventReference eventName, Transform transformToAttachTo)
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

            //If line has no conditions then add 
            if (line.Value.conditions.Count == 0)
            {
                triggerableLines.Add(line.Value);
            }
        }

        

        int indexToPlay = Random.Range(0, triggerableLines.Count);
        DialogueHandler programmerCallback = new(triggerableLines[indexToPlay].key, eventName, transformToAttachTo); //Make programmer deceleration in function, to make memory management better!!!
        subtitleManager.QueueDialogue(triggerableLines[indexToPlay].line, entityName, 3f);
        //  Debug.Log("Length: " + diaInfoCallback.GetDialogueLength());
    }

    

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

        Debug.Log("Resolution: " + dataResolveType.ToString() + " /  gameDataVal: " + gameDataVal + "ConditionVAL: " + condition.conditionValue);
        return false;
    }

    public void AddObserver(IDialogueObserver observer)
    {
        dialogueObservers.Add(observer);
    }

    public void RemoveObserver(IDialogueObserver observer)
    {
        dialogueObservers.Remove(observer);
    }

    public void NotifyObservers(DialogueState state)
    {
        foreach (var observer in dialogueObservers)
        {
            observer.OnNotify(state);
        }
    }
}
