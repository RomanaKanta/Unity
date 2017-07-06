using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Category : MonoBehaviour
{
	private AsyncOperation async = null;
	Rect emptyLoadingRect = new Rect ();
	Rect fullLoadingRect = new Rect ();
	public Texture emptyProgressBar; // Set this in inspector.
	public Texture fullProgressBar; // Set this in inspector.

	public Button categoryAll;
	public Button categoryHead;
	public Button categoryNeck;
	public Button categoryThorex;
	public Button categoryAbdomen;
	public Button categoryBack;
	public Button categoryUpperLimb;
	public Button categoryLowerLimb;

//	public Text loaderText;
	private bool loadScene = false;

//	//public Button backButton;
//	Dictionary<string,BodyParts> dict;
//
//	public void Awake ()
//	{
//		dict = BodyPartsDataManager.Instance.BodyPartsDict;
//	}

	// Use this for initialization
	void Start ()
	{
		emptyLoadingRect.x = (float)(((Screen.width)/2) - 315);
		emptyLoadingRect.y = (float)(Screen.height - 90);
		emptyLoadingRect.width = (float)(630);
		emptyLoadingRect.height = (float)(60);

		emptyProgressBar = Resources.Load ("images/loader_container") as Texture;
		fullProgressBar = Resources.Load ("images/loader") as Texture;

		Screen.orientation = ScreenOrientation.Portrait;
	
		categoryAll.onClick.AddListener (() => getCategoryData ("All", "000"));

		categoryHead.onClick.AddListener (() => getCategoryData ("HeadCube", "K10000")); // SkeletonSkull
		categoryNeck.onClick.AddListener (() => getCategoryData ("NeckCube", "K10340"));
		categoryThorex.onClick.AddListener (() => getCategoryData ("ChestCube", "K10600"));
		categoryAbdomen.onClick.AddListener (() => getCategoryData ("AbdomenCube", "K10740"));
		categoryBack.onClick.AddListener (() => getCategoryData ("BackCube", "K10840"));
		categoryUpperLimb.onClick.AddListener (() => getCategoryData ("Upper_limbsCube", "K11390"));
		categoryLowerLimb.onClick.AddListener (() => getCategoryData ("Lower_limbsCube", "K11920"));
	}
		
	private void getCategoryData (string category, string id)
	{
		MuscleData.GameObjectName = category;
		MuscleData.GameObjectID = id;
		loadScene = true;
	}

	private IEnumerator LoadScene() {

		async = SceneManager.LoadSceneAsync("ModelViewer");
		yield return async;
	}

	void OnGUI() {
		if (async != null) {
			GUI.DrawTexture (emptyLoadingRect, emptyProgressBar);

			fullLoadingRect.x = emptyLoadingRect.x+15;
			fullLoadingRect.y = emptyLoadingRect.y +15;
			fullLoadingRect.width = (float)(emptyLoadingRect.width * async.progress);
//		fullLoadingRect.width = (float)(emptyLoadingRect.width * 0.9);
			fullLoadingRect.height = emptyLoadingRect.height - 30;

			GUI.DrawTexture(fullLoadingRect, fullProgressBar);

		float width = 60f;
		float height = 20f;
		float left = emptyLoadingRect.x + (emptyLoadingRect.width/2 - 30);
		float top = emptyLoadingRect.y +20;

		GUIStyle myStyle = new GUIStyle();
		myStyle.fontSize = 20;

		if (async.progress > 0.6) {
			myStyle.normal.textColor = Color.black;
		} else {
			myStyle.normal.textColor = Color.white;
		}
				
		GUI.Label (new Rect(left, top, width, height), "Loading...",myStyle);
		}
	}

	private void onBack ()
	{
		OnBackButtonPressed ();
	}

	// Update is called once per frame
	void Update ()
	{
		
		if (Input.GetKey (KeyCode.Escape)) {
			OnBackButtonPressed ();
		}
			
		if (loadScene == true) {
			
			loadScene = false;
			StartCoroutine(LoadScene());
		}
	}

	void OnBackButtonPressed ()
	{
		Application.Quit ();
	}
}
