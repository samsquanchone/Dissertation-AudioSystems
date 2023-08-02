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
    public AttenuationMode attenuationMode;
    [HideInInspector] public float overrideMaxDistance;
    [Header("FMOD Event")]
    [SerializeField]
    private EventReference audioRef;
    private EventInstance Audio;
    private EventDescription AudioDes;
    [SerializeField] private string parameterName;


    [SerializeField] private StudioListener Listener;
    private PLAYBACK_STATE pb;

    [Header("Occlusion Options")]
    [SerializeField]
    [Range(0f, 10f)]
    private float SoundOcclusionWidening = 1f;
    [SerializeField]
    [Range(0f, 10f)]
    private float PlayerOcclusionWidening = 1f;
    [SerializeField]
    private LayerMask OcclusionLayer;

    private bool AudioIsVirtual;
    private float MaxDistance;
    private float ListenerDistance;
    private float lineCastHitCount = 0f;
    private Color colour;

    private float minDistance;

   

    private void Start()
    {
        Audio = RuntimeManager.CreateInstance(audioRef);
        RuntimeManager.AttachInstanceToGameObject(Audio, GetComponent<Transform>(), GetComponent<Rigidbody>());
        Audio.start();
        Audio.release();

        /*If the designer is using a natural roll off, allow event description to handle min max distance.
          Else if designer is overriding attenuation, then use override value for max distance */
        if (this.attenuationMode == AttenuationMode.Auto)
        {
            AudioDes = RuntimeManager.GetEventDescription(audioRef);
            AudioDes.getMinMaxDistance(out minDistance, out MaxDistance);
        }
        else if (this.attenuationMode == AttenuationMode.User)
        {
            MaxDistance = overrideMaxDistance;
        }
        

    }

    private void FixedUpdate()
    {
        Audio.isVirtual(out AudioIsVirtual);
        Audio.getPlaybackState(out pb);
        ListenerDistance = Vector3.Distance(transform.position, Listener.transform.position);

        if (!AudioIsVirtual && pb == PLAYBACK_STATE.PLAYING && ListenerDistance <= MaxDistance)
            OccludeBetween(transform.position, Listener.transform.position);

        lineCastHitCount = 0f;
    }

    private void OccludeBetween(Vector3 sound, Vector3 listener)
    {
        Vector3 SoundLeft = CalculatePoint(sound, listener, SoundOcclusionWidening, true);
        Vector3 SoundRight = CalculatePoint(sound, listener, SoundOcclusionWidening, false);

        Vector3 SoundAbove = new Vector3(sound.x, sound.y + SoundOcclusionWidening, sound.z);
        Vector3 SoundBelow = new Vector3(sound.x, sound.y - SoundOcclusionWidening, sound.z);

        Vector3 ListenerLeft = CalculatePoint(listener, sound, PlayerOcclusionWidening, true);
        Vector3 ListenerRight = CalculatePoint(listener, sound, PlayerOcclusionWidening, false);

        Vector3 ListenerAbove = new Vector3(listener.x, listener.y + PlayerOcclusionWidening * 0.5f, listener.z);
        Vector3 ListenerBelow = new Vector3(listener.x, listener.y - PlayerOcclusionWidening * 0.5f, listener.z);

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

        if (PlayerOcclusionWidening == 0f || SoundOcclusionWidening == 0f)
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
    /// <param name="overrideDistance"></param>
    public void UpdateDistanceOnOverride(float overrideDistance)
    {
        MaxDistance = overrideMaxDistance;
    }

    private Vector3 CalculatePoint(Vector3 a, Vector3 b, float m, bool posOrneg)
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
        float mn = (m / n);
        if (posOrneg)
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

    private void SetParameter()
    {
        Audio.setParameterByName(parameterName, lineCastHitCount / 11);
    }
}



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