using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEditor;


public enum AttenuationMode {Auto, User};

public class SpatializedAudio : MonoBehaviour
{
    [Header("Attenuation Mode")]
    [Tooltip("This dictates whether to use default spatialiser attenuation(Inverse-square) or a overriden custom attenuation. Note: if override mode then override max distance parameter will appear and require populating with a value")]
    public AttenuationMode attenuationMode;

    [HideInInspector] public float overrideMaxDistance;
    [Header("FMOD Event")]
    [SerializeField]
    [Tooltip("Fmod audio event you would like the occlusion system to work with")]
    private EventReference audioRef;
    private EventInstance audioEvent;
    private EventDescription audioDes;

    [Tooltip("Name of the occlusion parameter controlling LPF cut-off in FMOD")]
    [SerializeField] private string parameterName;

    [Tooltip("Listener object that will be utilised to apply attenuation, typically player or player camera")]
    [SerializeField] private StudioListener Listener;
    private PLAYBACK_STATE pb;

    [Header("Occlusion Options")]
    [Tooltip("This will change the width that sound source distribute rays")]
    [SerializeField]
    [Range(0f, 10f)]
    private float soundOcclusionWidening = 1f;

    [Tooltip("This will increase the width between the players ears")]
    [SerializeField]
    [Range(0f, 10f)]
    private float playerOcclusionWidening = 1f;

    [Tooltip("Used to select which layers to calculate occlusion when rays interact with objects of that layer type")]
    [SerializeField]
    private LayerMask OcclusionLayer;

    
    /// <summary>
    /// Non-designer variables, therefore not exposed in inspector
    /// </summary>
    private bool isAudioVirtual;
    private float maxDistance;
    private float listenerDistance;
    private float lineCastHitCount = 0f;
    private Color colour;
    private float minDistance;

   

    private void Start()
    {
        //Set up the fmod event
        audioEvent = RuntimeManager.CreateInstance(audioRef);
        RuntimeManager.AttachInstanceToGameObject(audioEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
        audioEvent.start();
        audioEvent.release();

        /*If the designer is using a natural roll off, allow event description to handle min max distance.
          Else if designer is overriding attenuation, then use override value for max distance */
        if (this.attenuationMode == AttenuationMode.Auto)
        {
            audioDes = RuntimeManager.GetEventDescription(audioRef);
            audioDes.getMinMaxDistance(out minDistance, out maxDistance);
        }
        else if (this.attenuationMode == AttenuationMode.User)
        {
            maxDistance = overrideMaxDistance;
        }
        

    }

    //Get FMOD info and distance info to provide to occlusion function
    private void FixedUpdate()
    {
        audioEvent.isVirtual(out isAudioVirtual);
        audioEvent.getPlaybackState(out pb);
        listenerDistance = Vector3.Distance(transform.position, Listener.transform.position);

        if (!isAudioVirtual && pb == PLAYBACK_STATE.PLAYING && listenerDistance <= maxDistance)
            OccludeBetween(transform.position, Listener.transform.position);

        lineCastHitCount = 0f;
    }

    /// <summary>
    /// Calculate vectors and pass to line cast function
    /// </summary>
    /// <param name="sound"> sound source vector 3 vals </param>
    /// <param name="listener"> listener vector 3 vals </param>
    private void OccludeBetween(Vector3 sound, Vector3 listener)
    {
        Vector3 SoundLeft = CalculateXZVector(sound, listener, soundOcclusionWidening, true);
        Vector3 SoundRight = CalculateXZVector(sound, listener, soundOcclusionWidening, false);

        Vector3 SoundAbove = new Vector3(sound.x, sound.y + soundOcclusionWidening, sound.z);
        Vector3 SoundBelow = new Vector3(sound.x, sound.y - soundOcclusionWidening, sound.z);

        Vector3 ListenerLeft = CalculateXZVector(listener, sound, playerOcclusionWidening, true);
        Vector3 ListenerRight = CalculateXZVector(listener, sound, playerOcclusionWidening, false);

        Vector3 ListenerAbove = new Vector3(listener.x, listener.y + playerOcclusionWidening * 0.5f, listener.z);
        Vector3 ListenerBelow = new Vector3(listener.x, listener.y - playerOcclusionWidening * 0.5f, listener.z);

        CastLine(SoundLeft, ListenerLeft);
        CastLine(SoundLeft, listener);
        CastLine(SoundLeft, ListenerRight);

        CastLine(sound, ListenerLeft);
        CastLine(sound, listener);
        CastLine(sound, ListenerRight);

        CastLine(SoundRight, ListenerLeft);
        CastLine(SoundRight, listener);
        CastLine(SoundRight, ListenerRight);

        CastLine(SoundAbove, ListenerAbove);
        CastLine(SoundBelow, ListenerBelow);

        if (playerOcclusionWidening == 0f || soundOcclusionWidening == 0f)
        {
            colour = Color.blue;
        }
        else
        {
            colour = Color.green;
        }

        SetParameter();
    }

    /// <summary>
    /// This is used to update the attenuation mode if the game is running and debugging during run and not in the editor while the game is not running in the editor
    /// </summary>
    /// <param name="overrideDistance"> Used for overriding the fmod defaul max distance for larger sounds</param>
    public void UpdateDistanceOnOverride(float overrideDistance)
    {
        maxDistance = overrideMaxDistance;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"> used to represent </param>
    /// <param name="b"></param>
    /// <param name="m"></param>
    /// <param name="absolute"></param>
    /// <returns>(XYZ)</returns>
    private Vector3 CalculateXZVector(Vector3 a, Vector3 b, float m, bool absolute)
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
        float mn = (m / n);
        if (absolute)
        {
            x = a.x + (mn * (a.z - b.z));
            z = a.z - (mn * (a.x - b.x));
        }
        else
        {
            x = a.x - (mn * (a.z - b.z));
            z = a.z + (mn * (a.x - b.x));
        }
        return new Vector3(x, a.y, z);
    }


    /// <summary>
    /// Handles the casting of lines 
    /// </summary>
    /// <param name="Start">Origin of line cast</param>
    /// <param name="End">End pos of line cast</param>
    private void CastLine(Vector3 Start, Vector3 End)
    {
        RaycastHit hit;
        Physics.Linecast(Start, End, out hit, OcclusionLayer);

        if (hit.collider)
        {
            lineCastHitCount++;
            Debug.DrawLine(Start, End, Color.red);
        }
        else
            Debug.DrawLine(Start, End, colour);
    }


    /// <summary>
    /// Sets the occlusion val for the FMOD param
    /// </summary>
    private void SetParameter()
    {
        audioEvent.setParameterByName(parameterName, lineCastHitCount / 11);
    }
}


/// <summary>
/// Editor override for attenuation mode, will show override max distance var when override mode is selected as attenuation mode
/// </summary>
#if (UNITY_EDITOR)
[CustomEditor(typeof(SpatializedAudio))]
public class SpatializedAudioEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpatializedAudio script = (SpatializedAudio)target;

        switch(script.attenuationMode)
        {
            case AttenuationMode.User:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Override Max Distance value", GUILayout.MaxWidth(180));
                script.overrideMaxDistance = EditorGUILayout.FloatField(script.overrideMaxDistance);
                EditorGUILayout.EndHorizontal();
                script.UpdateDistanceOnOverride(script.overrideMaxDistance);
                break;
        }

    }
}
#endif