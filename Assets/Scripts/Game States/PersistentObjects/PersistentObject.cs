using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentObject : MonoBehaviour {

	public Hashtable persistentProperties;

	string id;

	public string GetID() {
		if (id == null) {
			id = SceneManager.GetActiveScene().name + "/" + this.name;;
		}
		return id;
	}

	// this is called after the global Awake so the initial save data will always be present
	public virtual void Start() {
	
	}

	public Hashtable GetProperties() {
		return persistentProperties;
	}

	public SerializedPersistentObject MakeSerialized() {
		return new SerializedPersistentObject(this);
	}

	public virtual void ConstructFromDeserialized(SerializedPersistentObject s) {
		this.persistentProperties = s.persistentProperties;
	}

	protected SerializedPersistentObject LoadObjectState() {
		return GlobalController.GetPersistentObject(GetID());
	}
	
	public string StoredObjectName() {
		return System.Guid.NewGuid().ToString();
	}

	// to be called from subclasses
	protected virtual void UpdateObjectState() {

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