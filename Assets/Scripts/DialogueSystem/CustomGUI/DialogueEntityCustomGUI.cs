using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DialogueSystem.EntityNPC;
using DialogueUtility;
using System.IO;

#if (UNITY_EDITOR)
/// <summary>
/// This is used to override which inspector variables are shown depending on what trigger type the user selects for the enum
/// Note: Editor only and pust be contained within the if unity editor, or you will be unable to build your project!
/// </summary>
[CustomEditor(typeof(DialogueEntity))]
public class TestXMLEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Call normal GUI 
        base.OnInspectorGUI();

        // Reference the variables in the script
        DialogueEntity script = (DialogueEntity)target;



        //When GUI button is pressed in inspector, open directory where streaming assets are
        if (GUILayout.Button("XML Path"))
        {
            string path = EditorUtility.OpenFilePanel("Select XML File", Application.streamingAssetsPath, "xml");

            string pathWithoutExt = Path.ChangeExtension(path, null);
            string[] directories = pathWithoutExt.Split('/');
            List<string> trimmedDirs = new();


            //Normal file paths dont work in build, hence the need for some string manipulation to sort dirs into array and then add '/' chars back and combine into filepath
            bool shouldAdd = false;
            foreach (var dir in directories)
            {
                if (dir == "StreamingAssets")
                {
                    //Start adding dirs to list once it reaches streaming assets
                    shouldAdd = true;
                }

                else if (shouldAdd && dir != "StreamingAssets")
                {
                    trimmedDirs.Add(dir);
                }

            }
            string combinedPath = "";
            foreach (var trimmedDir in trimmedDirs)
            {

                combinedPath = combinedPath + trimmedDir + "/";

            }

            combinedPath = combinedPath.Remove(combinedPath.Length - 1);
            script.SetFileName(combinedPath);

        }

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


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Player Distance", GUILayout.MaxWidth(180));
                script.distance = EditorGUILayout.FloatField(script.distance);
                EditorGUILayout.EndHorizontal();
                break;

            case TriggerType.TriggerExit:
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

        }

        if (script.sequenceType == SequenceType.PlayerResponse)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Player response node", GUILayout.MaxWidth(180));
            script.playerResponseNodes = EditorGUILayout.ObjectField(script.playerResponseNodes, typeof(PlayerResponse), true) as PlayerResponse;
            script.triggerType = TriggerType.Radius;
            EditorGUILayout.EndHorizontal();
        }

        if (script.is3D)
        {
            //If dialogue is 3D then show the necessary parameters needed for setting up a 3D audio event in fmod
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Emitter Object", GUILayout.MaxWidth(180));
            script.objectToAttachTo = EditorGUILayout.ObjectField(script.objectToAttachTo, typeof(Transform), true) as Transform;
            EditorGUILayout.EndHorizontal();

        }


    }

}

#endif
