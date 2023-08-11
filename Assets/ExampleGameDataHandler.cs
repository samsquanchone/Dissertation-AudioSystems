using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleGameDataHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Creating new game data objects
        GameDataValue test = new((uint)200);
        GameDataValue questAcceptedBool = new((bool)false);
        GameDataValue spokenToBenBool = new((bool)false);
        GameDataValue questCompletedBool = new((bool)false);

        //Adding the objects to this scenes game data hash table (In the game data resolver class)
        GameDataResolver.Instance.AddGameDataToHashTable((uint)1, test);
        GameDataResolver.Instance.AddGameDataToHashTable((uint)2, questAcceptedBool);
        GameDataResolver.Instance.AddGameDataToHashTable((uint)3, spokenToBenBool);
        GameDataResolver.Instance.AddGameDataToHashTable((uint)4, questCompletedBool);

        //Set a game data value from the created hashtable 
        GameDataResolver.Instance.SetGameDataVariable((uint)1, 25);
    }

}
