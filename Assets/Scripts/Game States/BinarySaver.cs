using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinarySaver : MonoBehaviour
{
    const string folderName = "saveData";
    const string fileExtension = ".dat";
	Save existingSave;

	void Start() {
		existingSave = GetComponent<Save>();
	}

	public void SaveGame(int slot=1) {
		string folderPath = GetFolderPath();
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string dataPath = Path.Combine(folderPath, slot + fileExtension);
		SaveCharacter(existingSave, dataPath);
	}

	public void LoadGame(int slot=1) {
		string folderPath = GetFolderPath();
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string dataPath = GetSavePath(slot);
		this.existingSave.LoadFromSerializableSave(LoadCharacter(dataPath));
	}

    public bool HasFinishedGame(int slot=1) {
        if (!HasSavedGame()) return false;
        string folderPath = GetFolderPath();
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string dataPath = GetSavePath(slot);
		SerializableSave s = LoadCharacter(dataPath);
        return s.gameFlags.Contains(GameFlag.BeatGame);
    }

    public void NewGamePlus(int slot=1) {
        string folderPath = GetFolderPath();
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string dataPath = GetSavePath(slot);
		this.existingSave.LoadNewGamePlus(LoadCharacter(dataPath), slot);
    }

    string GetFolderPath() {
        return Path.Combine(Application.persistentDataPath, folderName);
    }

    string GetSavePath(int slot) {
        return Path.Combine(GetFolderPath(), slot + fileExtension);
    }

    public bool HasSavedGame(int slot=1) {
        try {
            LoadCharacter(GetSavePath(slot));
        } catch (FileNotFoundException) {
            return false;
        }
        return true;
    }

    void SaveCharacter(Save save, string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, save.MakeSerializableSave());
        }
    }

    SerializableSave LoadCharacter(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            return (SerializableSave)binaryFormatter.Deserialize(fileStream);
        }
    }

    static string[] GetFilePaths()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        return Directory.GetFiles(folderPath, "*" + fileExtension);
    }
}