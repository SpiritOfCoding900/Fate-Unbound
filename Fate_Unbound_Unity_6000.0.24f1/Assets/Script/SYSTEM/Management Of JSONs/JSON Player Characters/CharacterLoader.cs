using UnityEngine;
using System.Collections.Generic;



[System.Serializable]
public class CharacterClass
{
    public string className;
    public int MaxHP;
    public float moveSpeed;
    public string description;

    public int ID;

    public int starterWeaponPrimary;
    public int starterWeaponSecondary;
}



[System.Serializable]
public class CharacterClassList
{
    public List<CharacterClass> classes;
}



public class CharacterLoader : SimpleSingleton<CharacterLoader>
{
    public CharacterClassList myClassList = new CharacterClassList();

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("characterClasses");
        if (jsonFile != null)
        {
            myClassList = JsonUtility.FromJson<CharacterClassList>(jsonFile.text);
        }
        else
        {
            Debug.LogError("Could not find player.json in Resources folder.");
        }
    }
}
