using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Events;
using FMOD.Studio;

public class Footstepcontroller : MonoBehaviour
{
    public float minimumVelocityWalkThreshold;
    public float minumumVelocityRunThreshold;

    public float walkCooldown;
    public float runCooldown;

    private float cooldown;


    public EventReference foodstepSFX;
    [SerializeField] private Rigidbody rb;
    public GameObject playerObject;
    private enum CURRENT_TERRAIN { Grass, Rock, Mud, Concrete };

    [SerializeField]
    private CURRENT_TERRAIN currentTerrain;

    private FMOD.Studio.EventInstance foosteps;

    private bool canTrigger = true;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

        if (rb.velocity.magnitude > 2 && canTrigger)
        {
            cooldown = walkCooldown;
            DetermineTerrain();
            SelectAndPlayFootstep();
        }
        else if (rb.velocity.magnitude > 3 && canTrigger)
        {
            cooldown = runCooldown;
            DetermineTerrain();
            SelectAndPlayFootstep();
        }
    }

    private void DetermineTerrain()
    {
        RaycastHit[] hit;

        hit = Physics.RaycastAll(transform.position, Vector3.down, 10.0f);

        foreach (RaycastHit rayhit in hit)
        {
            if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Rock"))
            {
                currentTerrain = CURRENT_TERRAIN.Rock;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Mud"))
            {
                currentTerrain = CURRENT_TERRAIN.Mud;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
            {
                currentTerrain = CURRENT_TERRAIN.Grass;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Concrete"))
            {
                currentTerrain = CURRENT_TERRAIN.Concrete;
            }
        }
    }

    public void SelectAndPlayFootstep()
    {
        switch (currentTerrain)
        {
            case CURRENT_TERRAIN.Rock:
                PlayFootstep();
                canTrigger = false;
                break;

            case CURRENT_TERRAIN.Mud:
                PlayFootstep();
                canTrigger = false;
                break;

            case CURRENT_TERRAIN.Grass:
                PlayFootstep();
                canTrigger = false;
                break;

            case CURRENT_TERRAIN.Concrete:
                PlayFootstep();
                canTrigger = false;
                break;

            default:
                PlayFootstep();
                canTrigger = false;
                break;
        }
        StartCoroutine(FootstepCooldownRoutine(cooldown));
    }

    private void PlayFootstep()
    {
       AudioPlayback.PlayOneShotWithParameters(foodstepSFX, this.transform, ("Surface", currentTerrain.ToString()));
    }

    private IEnumerator FootstepCooldownRoutine(float _cooldown)
    {
        yield return new WaitForSecondsRealtime(_cooldown);
        canTrigger = true;
    }
}
