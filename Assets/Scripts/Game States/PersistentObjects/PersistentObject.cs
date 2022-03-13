using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentObject : MonoBehaviour {

	public Hashtable persistentProperties = new Hashtable();

	protected string id;

	virtual public string GetID() {
		if (id == null) {
			id = SceneManager.GetActiveScene().path + "/" + gameObject.GetHierarchicalName();
		}
		return id;
	}

	// this is called after the global Awake so the initial save data will always be present
	virtual protected void Start() {
		SerializedPersistentObject o = LoadObjectState();
        ConstructFromSerialized(o);
	}

	public Hashtable GetProperties() {
		return persistentProperties;
	}

	public SerializedPersistentObject MakeSerialized() {
		return new SerializedPersistentObject(this);
	}

	virtual public void ConstructFromSerialized(SerializedPersistentObject s) {
		if (s == null) {
			return;
		}
		this.persistentProperties = s.persistentProperties;
	}

	protected SerializedPersistentObject LoadObjectState() {
		return GlobalController.GetPersistentObject(GetID());
	}
	
	public string StoredObjectName() {
		return System.Guid.NewGuid().ToString();
	}

	// to be called from subclasses
	virtual protected void UpdateObjectState() {

	}

	protected void SaveObjectState() {
		GlobalController.SavePersistentObject(this.MakeSerialized());
	}
}

[System.Serializable]
public class SerializedPersistentObject {
	public readonly string id;
	public readonly Hashtable persistentProperties;
	public SerializedPersistentObject(PersistentObject p) {
		this.persistentProperties = p.GetProperties();
		this.id = p.GetID();
	}
}
