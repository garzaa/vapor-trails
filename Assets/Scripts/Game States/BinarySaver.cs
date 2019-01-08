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
		string folderPath = Path.Combine(Application.persistentDataPath, folderName);
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string dataPath = Path.Combine(folderPath, slot + fileExtension);
		SaveCharacter(existingSave, dataPath);
	}

	public void LoadGame(int slot=1) {
		string folderPath = Path.Combine(Application.persistentDataPath, folderName);
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string dataPath = Path.Combine(folderPath, slot + fileExtension);
		this.existingSave.LoadFromSerializableSave(LoadCharacter(dataPath));
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