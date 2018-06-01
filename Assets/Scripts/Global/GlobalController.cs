using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalController : MonoBehaviour {

	public static GlobalController gc;
	public GameObject titleText;

	void Awake()
	{
		gc = this;
	}

	public static void ShowTitleText(string title, string subTitle = null) {
		
	}
}
