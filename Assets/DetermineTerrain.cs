using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TerrainType {Grass, Mud, Rock, Concrete};
public class DetermineTerrain : MonoBehaviour
{
    public TerrainType DetermineShellTerrain()
    {
        RaycastHit[] hit;
        TerrainType terrain = TerrainType.Concrete; //Ded
        hit = Physics.RaycastAll(transform.position, Vector3.down, 10.0f);

        foreach (RaycastHit rayhit in hit)
        {
            if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Rock"))
            {
                terrain = TerrainType.Rock;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Mud"))
            {
                terrain = TerrainType.Mud;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
            {
                terrain = TerrainType.Grass;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Concrete"))
            {
                terrain = TerrainType.Concrete;
            }
        }

        return terrain;
    }
}
