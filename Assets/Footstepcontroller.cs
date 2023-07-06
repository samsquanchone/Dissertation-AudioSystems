using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Events;

public class Footstepcontroller : MonoBehaviour
{
    [SerializeField] List<UnityEvent> eventList; //Move to dialogue system 
    [SerializeField] private Rigidbody rb;
    private enum CURRENT_TERRAIN { GRASS, GRAVEL, WOOD_FLOOR, WATER };

    [SerializeField]
    private CURRENT_TERRAIN currentTerrain;

    private FMOD.Studio.EventInstance foosteps;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.velocity.magnitude > 0)
            Debug.Log("sadsdsa");
        DetermineTerrain();
    }

    private void DetermineTerrain()
    {
        RaycastHit[] hit;

        hit = Physics.RaycastAll(transform.position, Vector3.down, 10.0f);

        foreach (RaycastHit rayhit in hit)
        {
            if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Gravel"))
            {
                currentTerrain = CURRENT_TERRAIN.GRAVEL;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Wood"))
            {
                currentTerrain = CURRENT_TERRAIN.WOOD_FLOOR;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
            {
                currentTerrain = CURRENT_TERRAIN.GRASS;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                currentTerrain = CURRENT_TERRAIN.WATER;
            }
        }
    }

    public void SelectAndPlayFootstep()
    {
        switch (currentTerrain)
        {
            case CURRENT_TERRAIN.GRAVEL:
                PlayFootstep(1);
                break;

            case CURRENT_TERRAIN.GRASS:
                PlayFootstep(0);
                break;

            case CURRENT_TERRAIN.WOOD_FLOOR:
                PlayFootstep(2);
                break;

            case CURRENT_TERRAIN.WATER:
                PlayFootstep(3);
                break;

            default:
                PlayFootstep(0);
                break;
        }
    }

    private void PlayFootstep(int terrain)
    {
        foosteps = FMODUnity.RuntimeManager.CreateInstance("event:/Footsteps");
        foosteps.setParameterByName("Terrain", terrain);
        foosteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        foosteps.start();
        foosteps.release();
    }
}
