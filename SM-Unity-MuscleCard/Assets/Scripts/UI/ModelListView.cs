using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelListView : MonoBehaviour {

	public Button buttonBack;

	void Start ()
	{

		buttonBack.onClick.AddListener (() => OnBackButtonPressed ());

	}

	protected virtual void Update ()
	{

		if (Input.GetKey (KeyCode.Escape)) {
			OnBackButtonPressed ();
		}


	}

	public void OnBackButtonPressed ()
	{
		// add code here to handle the back button press
		Application.LoadLevel ("ModelViewer");
//		Destroy(this.gameObject);
		//Debug.Log ("OnBackButtonPressed");

	}
}
