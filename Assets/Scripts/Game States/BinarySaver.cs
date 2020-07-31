using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using System.Text;

public class BinarySaver : MonoBehaviour {
    const string folder = "saves";
    const string extension = ".dat";

    BinaryFormatter binaryFormatter = new BinaryFormatter();

    void OnEnable() {
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }
    }

    public void SaveFile(Save save, int slot) {
        save.BeforeSerialize();

        using (FileStream fileStream = File.Open(GetSavePath(slot), FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, save);
        }
    }

    public Save LoadFile(int slot) {
        Save save;
        using (FileStream fileStream = File.Open(GetSavePath(slot), FileMode.Open))
        {
            save = (Save) binaryFormatter.Deserialize(fileStream);
        }
        save.AfterDeserialize();
        return save;
    }

    public bool HasFile(int slot) {
        if (!File.Exists(GetSavePath(slot))) return false;
        try {
            Save s = LoadFile(slot);
            return true;
        } catch (Exception) {
            // deal with legacy saves/changed formats
            return false;
        }
    }

    string GetFolderPath() {
        return Path.Combine(Application.persistentDataPath, folder);
    }
    
    string GetSavePath(int slot) {
        return Path.Combine(GetFolderPath(), slot+extension);
    }

    public bool HasFinishedGame() {
        return false;
    }

    public void NewGamePlus() {

    }
}
