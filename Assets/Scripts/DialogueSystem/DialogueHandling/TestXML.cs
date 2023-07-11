using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DialogueUtility;

public enum TriggerType {TriggerEnter, Collision, Radius, TriggerExit} //Alow user to determine how the dialogue from the xml file is triggered within the game engine 
public enum ColliderType {BoxCollider, SphereCollider, CapsuleCollider, Invalid} //Used to determine what collider type the user has attached to the triggerObject
public class TestXML : MonoBehaviour
{

    public TriggerType triggerType;
 
    [SerializeField] private string xmlFileName;
    

    [SerializeField] private FMODUnity.EventReference eventName;

    [SerializeField] private string entityID;
    [SerializeField] private string entityName;

    [SerializeField] private Entity entity;

    public int[] intParams;
    public float[] floatParams;


    public int healthParams;

    private DialogueHandler programmerCallback;


    public bool is3D = false;

    //Dynamic parameters
    [HideInInspector] public Transform  triggerObject;
    [HideInInspector] public Transform triggeringObject;
    [HideInInspector] public Transform collisionObject;
    [HideInInspector] public float radius;
    [HideInInspector] public Transform origin;
    [HideInInspector] public bool debugDraw = false; //Use gizmos to draw what is provided by the user supplied trigger object param 
    [HideInInspector] public Color debugColor;
    // Start is called before the first frame update
    void Start()
    {
        entity = DataManager.LoadXMLDialogueData(xmlFileName);
        DialogueManager.Instance.AddEntityToHashTable(entity);
        entityID = entity.id;
        entityName = entity.name;
    }

    void InitializeXMLFile()
    {
        
    }

    private void OnDrawGizmos()
    {   

        if (debugDraw)
        {
            switch(GetColliderType(triggerObject))
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

    public void OnNotify()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == collisionObject.name && triggerType == TriggerType.Collision)
        {
            DialogueManager.Instance.PlayDialogueSequence(entity.lines, entity.name);
            programmerCallback = new (entity.lines[0].key, eventName, null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == triggeringObject.transform && triggerType == TriggerType.TriggerEnter)
        {
            DialogueManager.Instance.PlayDialogueSequence(entity.lines, entity.name);
            programmerCallback = new(entity.lines[0].key, eventName, null);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == triggeringObject.transform && triggerType == TriggerType.TriggerExit)
        {
            DialogueManager.Instance.PlayDialogueSequence(entity.lines, entity.name);
            programmerCallback = new(entity.lines[0].key, eventName, null);
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




#if (UNITY_EDITOR)
/// <summary>
/// This is used to override which inspector variables are shown depending on what trigger type the user selects for the enum
/// Note: Editor only and pust be contained within the if unity editor, or you will be unable to build your project!
/// </summary>
[CustomEditor(typeof(TestXML))]
    public class TestXMLEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            // Call normal GUI 
            base.OnInspectorGUI();

            // Reference the variables in the script
            TestXML script = (TestXML)target;

     

        EditorGUILayout.LabelField("Trigger Parameters", EditorStyles.boldLabel); //Set header within the inspector

       

            // Set up the shown variables relating to trigger enter enum type
            switch (script.triggerType)
            {

                case TriggerType.TriggerEnter:

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Trigger Object", GUILayout.MaxWidth(180));
                    script.triggerObject = EditorGUILayout.ObjectField(script.triggerObject, typeof(Transform), true) as Transform;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Triggering Object", GUILayout.MaxWidth(180));
                    script.triggeringObject = EditorGUILayout.ObjectField(script.triggeringObject, typeof(Transform), true) as Transform;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Debug", GUILayout.MaxWidth(180));
                    script.debugDraw = EditorGUILayout.Toggle(script.debugDraw);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Debug Colour", GUILayout.MaxWidth(180));
                    script.debugColor = EditorGUILayout.ColorField(script.debugColor);
                    EditorGUILayout.EndHorizontal();
                    break;

                case TriggerType.Collision:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Collision Object", GUILayout.MaxWidth(180));
                    script.collisionObject = EditorGUILayout.ObjectField(script.collisionObject, typeof(Transform), true) as Transform;
                    EditorGUILayout.EndHorizontal();
                    break;

                case TriggerType.Radius:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Radius", GUILayout.MaxWidth(180));
                    script.radius = EditorGUILayout.FloatField(script.radius);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Origin Object", GUILayout.MaxWidth(180));
                    script.triggerObject = EditorGUILayout.ObjectField(script.triggerObject, typeof(Transform), true) as Transform;
                    EditorGUILayout.EndHorizontal();
                    break;

                case TriggerType.TriggerExit:
                    // Set up the shown variables relating to trigger enter enum type
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Trigger Object", GUILayout.MaxWidth(180));
                    script.triggerObject = EditorGUILayout.ObjectField(script.triggerObject, typeof(Transform), true) as Transform;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Triggering Object", GUILayout.MaxWidth(180));
                    script.origin = EditorGUILayout.ObjectField(script.origin, typeof(Transform), true) as Transform;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Debug", GUILayout.MaxWidth(180));
                    script.debugDraw = EditorGUILayout.Toggle(script.debugDraw);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Debug Colour", GUILayout.MaxWidth(180));
                    script.debugColor = EditorGUILayout.ColorField(script.debugColor);
                    EditorGUILayout.EndHorizontal();


                    break;

            }

            if (script.is3D)
            {
                //If dialogue is 3D then show the necessary parameters needed for setting up a 3D audio event in fmod
            }


        }

    }

#endif

