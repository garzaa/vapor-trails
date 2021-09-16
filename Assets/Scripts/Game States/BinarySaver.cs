using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using System.Collections.Generic;

public class BinarySaver : MonoBehaviour {
    const string folder = "saves";
    const string extension = ".dat";

    static BinaryFormatter binaryFormatter = new BinaryFormatter();

    void OnEnable() {
        if (!Directory.Exists(GetFolderPath())) {
            Directory.CreateDirectory(GetFolderPath());
        }
    }

    public static void SaveFile(Save save, int slot) {
        save.BeforeSerialize();

        using (FileStream fileStream = File.Open(GetSavePath(slot), FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, save);
        }
    }

    public static Save LoadFile(int slot) {
        Save save;
        using (FileStream fileStream = File.Open(GetSavePath(slot), FileMode.Open))
        {
            save = (Save) binaryFormatter.Deserialize(fileStream);
        }
        save.AfterDeserialize();
        return save;
    }

    public static bool HasFile(int slot) {
        if (!File.Exists(GetSavePath(slot))) return false;
        try {
            Save s = LoadFile(slot);
            if (!CompatibleVersions(s)) {
                return false;
            }
            return true;
        } catch (Exception) {
            // deal with legacy saves/changed formats
            return false;
        }
    }

    static string GetFolderPath() {
        return Path.Combine(Application.persistentDataPath, folder);
    }
    
    static string GetSavePath(int slot) {
        return Path.Combine(GetFolderPath(), slot+extension);
    }

    public static bool HasFinishedGame() {
        // eventually: load all possible slots if possible, check for a saved game where there's some BeatGame flag
        // or is new game plus even desirable? it'd add a bunch of annoying shit to code and worry about
        // honestly: no
        return false;
    }

    public static bool CompatibleVersions(Save save) {
        string[] saveVersion = save.version.Split('.');
        string[] currentVersion = GlobalController.GetCurrentVersion().Split('.');

        return saveVersion[0].Equals(currentVersion[0]) && saveVersion[1].Equals(currentVersion[1]);
    }
}
