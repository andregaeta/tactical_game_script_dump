using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public CharacterInventory characterInventory;
    string savePath;

    private void Start()
    {
        savePath = Application.persistentDataPath + "/saves";
        characterInventory = GameState.Instance.activeCharacterInventory;
    }
    public void Save()
    {
        SaveData saveData = new SaveData();
        foreach (Character character in characterInventory.characters)
        {
            saveData.characters.Add(character);
        }

        //

        string jsonSaveData = JsonUtility.ToJson(saveData, true);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        binaryFormatter.Serialize(file, jsonSaveData);
        file.Close();
    }

    public void Load()
    {
        if (!File.Exists(savePath)) return;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Open(savePath, FileMode.Open);
        string jsonSaveData = binaryFormatter.Deserialize(file).ToString();
        SaveData saveData = new SaveData();
        JsonUtility.FromJsonOverwrite(jsonSaveData, saveData);
        file.Close();

        //

        characterInventory.characters.Clear();
        foreach (Character character in saveData.characters)
        {
            characterInventory.characters.Add(character);
        }
        characterInventory.UpdateSkillsByID();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Save();
        if (Input.GetKeyDown(KeyCode.L))
            Load();
    }

}


public class SaveData
{
    public List<Character> characters;

    public SaveData()
    {
        characters = new List<Character>();
    }
}