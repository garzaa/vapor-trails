using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System;

public abstract class PersistentObject : MonoBehaviour, ISaveListener {

	[Tooltip("If checked, objects with this name share data between scenes.")]
	public bool useGlobalName;

	protected bool hasSavedData {
		get { return properties.Count > 0; }
	}

	Dictionary<string, object> properties = new Dictionary<string, object>();
	
	protected void OnEnable() {
		properties = LoadObjectState();
		SetDefaults();
	}

	void OnDestroy() {
		// if this isn't here, they sync to the save when a new scene loads and dirty a "clean" chapter or disk save
		if (TransitionManager.dirty) {
			return;
		}
		SyncToSave();
	}

	public void OnBeforeSave() {
		SyncToSave();
	}

	protected void SyncToSave() {
		SaveManager.SavePersistentObject(this);
	}

	public void Reload() {
		OnEnable();
	}

	public string GetID() {
		if (useGlobalName) return "globalObjects/" + gameObject.name + "/" + GetType().Name;
		else return SceneManager.GetActiveScene().name + "/" + gameObject.GetHierarchicalName() + "/" + GetType().Name;
	}

	public string[] GetPath() {
		List<string> l = new List<string>(GetID().Split('/'));
		l.RemoveAt(l.Count - 1);
		return l.ToArray();
	}

	public string GetName() {
		string[] s = GetID().Split('/');
		return s[s.Length-1];
	}

	protected abstract void SetDefaults();

	protected void SetDefault(string key, object val) {
		if (!properties.ContainsKey(key)) {
			properties[key] = val;
		}
	}

	public Dictionary<string, object> GetAllProperties() {
		return properties;
	}

	protected Dictionary<string, object> LoadObjectState() {
		Dictionary<string, object> o = SaveManager.GetPersistentObject(this);
		if (o == null) return new Dictionary<string, object>();
		return o;
	}

	protected bool HasProperty(string key) {
		return properties.ContainsKey(key);
	}

	protected void SetProperty(string key, object obj) {
		properties[key] = obj;
	}

	protected T GetProperty<T>(string key) {
		if (properties[key].GetType().Equals(typeof(JObject))) {
			properties[key] = (properties[key] as JObject).ToObject<T>();
		}
		return (T) properties[key];
	}

	protected float GetFloat(string key) {
		var v = properties[key];
		try {
			return (float) v;
		} catch (InvalidCastException) {
			return Convert.ToSingle((System.Double) v);
		}
	}

	protected int GetInt(string key) {
		var v = properties[key];
		try {
			return (int) v;
		} catch (InvalidCastException) {
			return Convert.ToInt32((Int64) properties[key]);
		}
	}

	protected List<T> GetList<T>(string key) {
		var v = properties[key];
		try {
			return (List<T>) v;
		} catch (InvalidCastException) {
			return (v as JArray).ToObject<List<T>>();
		}
	}
}
