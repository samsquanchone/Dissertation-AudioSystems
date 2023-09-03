using System.Collections;

public class HashTable
{
    public Hashtable hashTable;
    public HashTable()
    {
        //Create a new hash table when we crearte an instance of this object
        hashTable = new();
    }
    public void AddDataToHashTable<T>(uint key, T data)
    {
       hashTable.Add(key, data);
    }

    public void UpdateGameDataValue<T>(uint key, T data)
    {
        hashTable[key] = data;
    }

    public void RemoveGameDataFromHashTable(uint key)
    {
        hashTable.Remove(key);
    }
}
