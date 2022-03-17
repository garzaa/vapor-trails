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

	virtual public string GetID() {
		if (useGlobalName) return gameObject.name + ": " + GetType().Name;
		else return SceneManager.GetActiveScene().name + "/" + gameObject.GetHierarchicalName() + ": " + GetType().Name;
	}

	protected void OnEnable() {
		properties = LoadObjectState();
		SetDefaults();
	}

	public void Reset() {
		properties.Clear();
		SetDefaults();
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
		Dictionary<string, object> o = GlobalController.GetPersistentObject(this);
		if (o == null) return new Dictionary<string, object>();
		return o;
	}

	public void OnBeforeSave() {
		GlobalController.SavePersistentObject(this);
	}

	protected bool HasProperty(string key) {
		return properties.ContainsKey(key);
	}

	protected void SetProperty(string key, object obj) {
		properties[key] = obj;
	}

	protected T GetProperty<T>(string key) {
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
			return ((JArray) v).ToObject<List<T>>();
		}
	}
}
