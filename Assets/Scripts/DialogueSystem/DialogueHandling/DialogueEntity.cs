using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueUtility;
using UnityEngine.Events;



public interface IDialogueObserver
{
    public void OnNotify(DialogueState state);
}


//events with enum for different state of dialogue
namespace DialogueSystem.EntityNPC
{
    public enum TriggerType { TriggerEnter, Collision, Radius, TriggerExit } //Alow user to determine how the dialogue from the xml file is triggered within the game engine 
    public enum ColliderType { BoxCollider, SphereCollider, CapsuleCollider, Invalid } //Used to determine what collider type the user has attached to the triggerObject


    public class DialogueEntity : MonoBehaviour, IDialogueObserver
    {
        [Tooltip("This dictates how the dialogue is triggered. For trigger enter it will trigger when a specified object enters a specified box, or spherical collider. For trigger exit, it is the same as trigger enter but trigger when the specfied object leaves the trigger. Collision mode will trigger when a specified object collides with the object this script is attached to. Radius mode will trigger when the attached player object is in a specified radius, and the specified interact key is pressed. PLEASE NOTE: YOU WILL ONLY SEE PARAMETERS RELATED TO THE SELECTED TRIGGER TPYE, AS VARIABLES WILL BE OVERRIDEN IN THE INSPECTOR. PLEASE ENSURE TRIGGER PARAMETERS ARE DEFINED")]
        public TriggerType triggerType;

        [Tooltip("Dictates how dialogue is sequenced. For all dialogue in a XML file to be played in one sequence, select sequential. For a single trigger of a random line within the xml file, thats conditions are met, select random-one shot.   For player response sequence to dictate what line is played, select play response.")]
        public SequenceType sequenceType;

        [Tooltip("The filepath to the XML file, use the XML file path button below to browse from file. (Note: XML file must be within StreamingAssets, it can however be nested in as many subfolders of StreamingAssets as desired))")]
        [SerializeField] string xmlFilePath;

        [SerializeField] private FMODUnity.EventReference eventName;

        [Tooltip("Object that XML file is de-serialized into. (Note: ID and name of entity from file show to give you indication of success of XML deserialization. Entity condition data is not shown within the inpsector, but can be seen while debugging in your IDE through inspecting the dialogue hash table in the dialogue mananager)")]
        [SerializeField] private Entity entity;

        [Tooltip("Link functions you would like to trigger when a converstation starts e.g. locking player movement")]
        [SerializeField] private List<UnityEvent> conversationStartEvent;
        [Tooltip("Link functions you would like to trigger when a converstation ends e.g. unlocking player movement")]
        [SerializeField] private List<UnityEvent> conversationEndEvent;
        [Tooltip("Link functions you would like to trigger when a dialogue within the conversation starts e.g. starting animations")]
        [SerializeField] private List<UnityEvent> dialogueStartEvents;
        [Tooltip("Link functions you would like to trigger when a dialogue within the conversation starts e.g. ending animations")]
        [SerializeField] private List<UnityEvent> dialogueEndEvents;


        [HideInInspector] public Transform objectToAttachTo = null;
        public GameObject player;

        public float distance;
        public bool is3D = false;
        bool hasGeneratedResponseInterface = false;
        //Dynamic parameters
        [HideInInspector] public Transform triggerObject;
        [HideInInspector] public Transform triggeringObject;
        [HideInInspector] public Transform collisionObject;
        [HideInInspector] public float radius;
        [HideInInspector] public Transform origin;
        [HideInInspector] public bool debugDraw = false; //Use gizmos to draw what is provided by the user supplied trigger object param 
        [HideInInspector] public Color debugColor;
        [HideInInspector] public PlayerResponse playerResponseNodes; //Could instead just have a list of UINTs that are used to obtain player response node. OR JUST HAVE ONE OBJECT TYPE THAT IS PLAYER RESPONSE AS A BASE NODE, THEN LET LOWER LEVEL CODE HANDLE ALL TRANSITIONS SET BY SCRIPTABLES
                                                                     // Start is called before the first frame update
        void Start()
        {
            this.entity = DataManager.LoadXMLDialogueData(xmlFilePath);
            DialogueManager.Instance.AddEntityToHashTable(entity);
            
            //Add the instance of an entity to the dialogue managers list of obserbers
            DialogueManager.Instance.AddObserver(this);
            
            if (this.sequenceType == SequenceType.PlayerResponse)
            {

                //Pass value that game designer has inputted into the private dynamic (dynamics cant be shown inspector, hence this method around it)
                this.playerResponseNodes.tranistonCondition.conditionValue = StringValidation.ConvertStringToDataType<dynamic>(playerResponseNodes.tranistonCondition.conditionToParse);
                // Debug.Log(response.condition.conditionToParse);

            }

        }

        public void SetFileName(string fileName)
        {
            xmlFilePath = fileName;
        }
        private void OnDrawGizmos()
        {

            if (debugDraw)
            {
                switch (GetColliderType(triggerObject))
                {
                    //DrawBoxGizmo based off user attached box collider 
                    case ColliderType.BoxCollider:
                        Gizmos.color = debugColor;
                        Gizmos.DrawCube(triggerObject.gameObject.GetComponent<BoxCollider>().bounds.center, triggerObject.gameObject.GetComponent<BoxCollider>().bounds.size);
                        break;

                    //Draw sphere gizmo based off user attached sphere collider 
                    case ColliderType.SphereCollider:
                        Gizmos.color = debugColor;
                        Gizmos.DrawSphere(triggerObject.gameObject.GetComponent<SphereCollider>().bounds.center, triggerObject.gameObject.GetComponent<SphereCollider>().radius);
                        break;


                    case ColliderType.Invalid:
                        Debug.Log("The collider type attached to the trigger object is not supported. Please switch to a supported collider type!");
                        break;

                }
            }
        }


        /// <summary>
        /// THIS DEFFO NEEDS REFACTORING! MAYBE EVENT BASED, AS WELL AS NOT AS MANY NESTED STATEMENTS!!!!!
        /// </summary>
        private void Update()
        {
            if (this.sequenceType == SequenceType.PlayerResponse)
            {
                this.distance = Vector3.Distance(this.gameObject.transform.position, player.transform.position);


                if (Vector3.Distance(this.gameObject.transform.position, player.transform.position) < this.radius && !this.hasGeneratedResponseInterface)
                {
                    DialogueManager.Instance.ShowInteractUI();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PlayerInteract();
                    }

                }

                else if (DialogueManager.Instance.subtitleManager.IsInteractPanelActive() && (Vector3.Distance(this.gameObject.transform.position, player.transform.position) > 12) || !DialogueManager.Instance.GetConversationState() && hasGeneratedResponseInterface)
                {
                    this.hasGeneratedResponseInterface = false;
                    DialogueManager.Instance.ExitConversation();
                }
            }

            else if (this.sequenceType != SequenceType.PlayerResponse && this.triggerType == TriggerType.Radius)
            {
                this.distance = Vector3.Distance(this.gameObject.transform.position, player.transform.position);


                if (Vector3.Distance(this.gameObject.transform.position, player.transform.position) < this.radius && !this.hasGeneratedResponseInterface)
                {
                    DialogueManager.Instance.ShowInteractUI();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        
                    }
                }
            }
        }

        private void PlayerInteract()
        {
            if (DialogueManager.Instance.CheckDialogueCondition<NodeCondition>(playerResponseNodes.tranistonCondition)) { playerResponseNodes = playerResponseNodes.transitionTo; } //IF the node condition is met then transition to the next response node
            DialogueManager.Instance.LookAtNPC(this.transform);
            DialogueManager.Instance.InstantiatePlayerResponseInterface(playerResponseNodes);
            this.hasGeneratedResponseInterface = true;

        }

        /// <summary>
        /// When the subject (Dialogue manager) invokes its listeners and passes the dialogue state, we can invoke the respetive list of user defined evvents
        /// </summary>
        /// <param name="state"> Current state of the dialogue system</param>
        public void OnNotify(DialogueState state)
        {
            switch (state)
            {
                case DialogueState.DialogueStart:
                    foreach (var _event in dialogueStartEvents) { _event.Invoke(); }
                    break;

                case DialogueState.DialogueEnd:
                    foreach (var _event in dialogueEndEvents) { _event.Invoke(); }
                    break;

                case DialogueState.ConversationStart:
                    foreach (var _event in conversationStartEvent) { _event.Invoke(); }
                    break;

                case DialogueState.ConversationEnd:
                    foreach (var _event in conversationEndEvent) { _event.Invoke(); }
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (this.triggerType == TriggerType.Collision)
                if (collision.gameObject.name == this.collisionObject.name)
                {
                    DialogueManager.Instance.PlayDialogueSequence(this.entity.name, entity.lines, this.sequenceType, this.eventName, objectToAttachTo);

                }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (this.triggerType == TriggerType.TriggerEnter)
                if (other.transform == this.triggeringObject.transform)
                {
                    DialogueManager.Instance.PlayDialogueSequence(this.entity.name, entity.lines, this.sequenceType, this.eventName, objectToAttachTo);

                }
        }

        private void OnTriggerExit(Collider other)
        {
            if (this.triggerType == TriggerType.TriggerExit)
                if (other.transform == triggeringObject.transform)
                {
                    DialogueManager.Instance.PlayDialogueSequence(this.entity.name, entity.lines, this.sequenceType, this.eventName, objectToAttachTo);

                }
        }


        private ColliderType GetColliderType(Transform triggerObj)
        {
            if (triggerObj.GetComponent<BoxCollider>() != null)
            {
                return ColliderType.BoxCollider;
            }

            if (triggerObj.GetComponent<SphereCollider>() != null)
            {
                return ColliderType.SphereCollider;
            }

            if (triggerObject.GetComponent<CapsuleCollider>() != null)
            {
                return ColliderType.CapsuleCollider;
            }

            else
            {
                return ColliderType.Invalid;
            }
        }
    }

    public class LineData
    {
        public string key;
        public string line;
        public List<string> conditionsList;
    }

}