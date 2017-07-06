//==============================================================================
// 
// SelectionManager.cs
// 
// 画面上の表示部位選択処理
// 
//==============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
	//--------------------------------- int ---------------------------------------------
	static readonly int TEXTURE_SIZE = 256;
	static readonly int CURRING_MASK_INDEX = 16;

	//---------------------------string------------------------------------
	private string gameObjectName;
	public string SelectedMeshName = null;
	private string videoPath = "S12300_V_010.mp4";
	private string imagePath = "K10620_C_000";

	//---------------------------------bool----------------------------------
	private bool isSingleTap = true;
	private bool isPinchZoom = true;
	private bool isRoation = true;
	private bool isMove = true;
	public bool isSelected = false;
	bool bUpdate;
	bool play = false;
	bool openDialog = false;
	bool IsOnDialog = false;
	bool IsActionAvailable;
	bool IsTouchOnButton;


	//---------------------Camera-------------------------------
	public Camera mainCamera;
	public Camera subCam;

	//-----------------------GameObject----------------------------------
	public GameObject masterModel;
	public GameObject selectedModel;
	public GameObject subModel;
	public GameObject panelDialog;
	public GameObject panelList;
	public GameObject menuObject;
	public GameObject colorDialog;
	public GameObject bookMarkDialog;
	public GameObject saveDialog;
	public GameObject imageDialog;

	//----------------------------Button-----------------------------------
	public Button buttonBack;
	public Button buttonMenuOpen;
	public Button buttonAudio;
	public Button buttonImage;
	public Button buttonVideo;
	public Button buttonUndo;
	public Button buttonCloseList;

	//---------------------------Image--------------------------------------
	public Image detailImage;
	public Sprite ImageSprite;

	//-----------------Animator----------------------------------
	public Animator dialogPanelAnimator;

	//-------------------Texture-------------------------------
	public Texture2D colorMapTex, xrayTex;
	public Texture2D capturedTexture;
	public RenderTexture renderTexture;

	public Material SelectedPartsMaterial;

	public Shader ColoredPartsShader;

	//------------------------------MeshRenderer----------------------------------
	MeshRenderer selectedParts;
	private MeshRenderer selectedMeshNerve;
	List<MeshRenderer> hiddenMeshList = new List<MeshRenderer> ();

	public Text textTitle;
	public Text textInformation;
	public Text textInformation1;
	public Text textInformation2;
	public Text textInformation3;
	public Text textInformation4;
	public GameObject panel1;
	public GameObject panel2;
	public GameObject panel3;
	public GameObject panel4;
	public GameObject scrollPanel;



	private Vector2 touchFinger;

	public AudioClip sound;

	public AudioSource source;

	[Tooltip ("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;

	[Tooltip ("How quickly smoothly this GameObject moves toward the target position")]
	public float Sharpness = 10.0f;

	// This stores the finger that's currently dragging this GameObject
	private LeanFinger draggingFinger;

	//====================================methods==============================================
	void Start ()
	{
		changeUIProperty (panelList, false);

		sound = Resources.Load ("Audio/05_Muscular_System") as AudioClip;

		source.clip = sound;

		source.playOnAwake = false;

		Screen.orientation = ScreenOrientation.Portrait;

		buttonBack.onClick.AddListener (() => OnBackButtonPressed ());
	}

	void Awake ()
	{
		initialize ();

	}

	protected virtual void  LateUpdate()
	{
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
			// Check if finger is over a UI element
			if (EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId)) {
				IsTouchOnButton = true;
			} else {
				IsTouchOnButton = false;
			}
		}

		TouchInsideDialog(panelDialog);

		if(hiddenMeshList.Count > 0){

			buttonUndo.gameObject.SetActive (true);
		}else{
			buttonUndo.gameObject.SetActive (false);
		}

		if (!panelList.activeSelf && !IsOnDialog && !colorDialog.activeSelf && !bookMarkDialog.activeSelf 
			&& !saveDialog.activeSelf && !imageDialog.activeSelf) {

			IsActionAvailable = true;
		} else {
			IsActionAvailable = false;
		}

		UIManager.Instance.CameraController.SetActionActive(IsActionAvailable);

		if (IsActionAvailable && !IsTouchOnButton) {

			if (bUpdate) {
				renderSubCamera ();
				bUpdate = false;
			}

			if (Input.GetKey (KeyCode.Escape)) {
				OnBackButtonPressed ();
			}
		}
	}

	void initialize ()
	{
//		MuscleData.GameObjectName = "HeadCube"; 
//		MuscleData.GameObjectID = "K10000";

//		MuscleData.GameObjectName = "All";


		gameObjectName = MuscleData.GameObjectName;
	
		if (gameObjectName != "All") {

			selectedModel = Instantiate (GameObject.Find (gameObjectName), masterModel.transform.position, Quaternion.identity);
			subModel = duplicateSubModel (selectedModel);

			if (gameObjectName == "HeadCube") {

				selectedModel.transform.position = new Vector3 (0, -0.01f, 0);
				subModel.transform.position = new Vector3 (0, -0.01f, 0);
				UIManager.Instance.CameraController.SetCameraDefaultZoom (0.06f);

			} else if (gameObjectName == "NeckCube") {
				
				UIManager.Instance.CameraController.SetCameraDefaultZoom (0.17f);

			} else if (gameObjectName == "ChestCube") {
				
				selectedModel.transform.position = new Vector3 (0, -0.02f, 0);
				subModel.transform.position = new Vector3 (0, -0.02f, 0);
				UIManager.Instance.CameraController.SetCameraDefaultZoom (0.17f);

			} else if (gameObjectName == "AbdomenCube") {
				
				selectedModel.transform.position = new Vector3 (0, -0.02f, 0);
				subModel.transform.position = new Vector3 (0, -0.02f, 0);
				UIManager.Instance.CameraController.SetCameraDefaultZoom (0.23f);

			} else if (gameObjectName == "Upper_limbsCube") {
				
				selectedModel.transform.position = new Vector3 (0, -0.02f, 0);
				subModel.transform.position = new Vector3 (0, -0.02f, 0);
				UIManager.Instance.CameraController.SetCameraDefaultZoom (0.21f);

			} else if (gameObjectName == "Lower_limbsCube") {
				selectedModel.transform.position = new Vector3 (0, -0.01f, 0);
				subModel.transform.position = new Vector3 (0, -0.01f, 0);
				UIManager.Instance.CameraController.SetCameraDefaultZoom (0.34f);

			} else if (gameObjectName == "BackCube") {
				
				selectedModel.transform.position = new Vector3 (0, -0.015f, 0);
				subModel.transform.position = new Vector3 (0, -0.015f, 0);
				UIManager.Instance.CameraController.SetCameraDefaultZoom (0.28f);

			}
			if (gameObjectName != "BackCube") {

				selectedModel.transform.Rotate (0,180,0);
				subModel.transform.Rotate (0,180,0);

			}
		} else {

			selectedModel = Instantiate (masterModel, masterModel.transform.position, Quaternion.identity);
			subModel = duplicateSubModel (selectedModel);
			UIManager.Instance.CameraController.SetCameraDefaultZoom (0.38f);
		}

//		UIManager.Instance.CameraController.SetObject(selectedModel);

		masterModel.SetActive (false);

		renderTexture = new RenderTexture (TEXTURE_SIZE, TEXTURE_SIZE, 32);
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.anisoLevel = 0;
		renderTexture.useMipMap = false;
		// 
		colorMapTex = new Texture2D (TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, false);
		colorMapTex.wrapMode = TextureWrapMode.Clamp;
		colorMapTex.anisoLevel = 0;
		// camera
		subCam.targetTexture	= renderTexture;
		subCam.cullingMask = 1 << CURRING_MASK_INDEX;
		Camera.main.cullingMask	= -1 ^ 1 << CURRING_MASK_INDEX;

		UpdateSubBuffer ();
}

	//====================== Dialog Actions (Image, video, sound) ==============================
	public void OnImageClickListener ()
	{
		if (imagePath != "") {
			imageDialog.SetActive (true);
			detailImage.sprite = ImageSprite;
		}
	}

	public void OnVideoClickListener ()
	{
		if (videoPath != "") {
			StartCoroutine (PlayStreamingVideo (videoPath));
		}
	}

	private IEnumerator PlayStreamingVideo (string fileName)
	{
		Handheld.PlayFullScreenMovie (fileName, Color.black, FullScreenMovieControlMode.CancelOnInput);
		yield return null;

	}
		
	public void PlaySound ()
	{
		play = !play;

		if (play) {
			source.PlayOneShot (sound);
		} else {
			source.Stop ();
		}
	}

	//============================= button functions =============================
	void OnBackButtonPressed ()
	{
		if (!panelList.activeSelf) {
			SceneManager.LoadScene ("Home");
		} 
	}

	public void onMenuOpen(Animator anim){
		menuObject.SetActive (true);
		anim.SetBool ("isMenuOpen",true);
		buttonMenuOpen.gameObject.SetActive (false);

	}

	public void onMenuClose(Animator anim){
		
		anim.SetBool ("isMenuOpen",false);
		WaitForMenuAnimationEnd (0.3f);
		buttonMenuOpen.gameObject.SetActive (true);
	}

	public void onReset(){
		int total = hiddenMeshList.Count;
		for(int i = 0; i<total; i++){
				OnUndo ();
			}
	
		if(panelDialog.activeSelf){
			Debug.Log ("" + SelectedMeshName);
			selectMesh (SelectedMeshName);
		}
	
		UIManager.Instance.BodyPartsListUI.resetSelectedItems ();

	}

	public void OnUndo(){

		if (hiddenMeshList.Count > 0 && IsActionAvailable) {
			MeshRenderer selectedMesh = hiddenMeshList [hiddenMeshList.Count - 1];

			bool center = false;
			string STR = "";
			for (int i = 0; i < MuscleData.centerMucsle.Length; i++) {

				if (selectedMesh.name.Contains (MuscleData.centerMucsle [i])) {
					STR = MuscleData.centerMucsle [i];
					center = true;

				} 
			}

			if (center) {
				UIManager.Instance.BodyPartsListUI.setCenterToogleFalse (selectedMesh.name, true);
				SetCenterMeshVisible (STR, true);
				
			} else {
				UIManager.Instance.BodyPartsListUI.setToogleFalse (selectedMesh.name, true);
				SetMeshVisible (selectedMesh.name, true);
			}


		}
	}


	//==================================== Sliding drawer ================================
	public void OnShowMuscleList (Animator anim)
	{
		changeUIProperty (panelList, true);
		anim.SetBool ("IsDiaplay",true);
	}

	public void OnHideMuscleList(Animator anim) {

		anim.SetBool ("IsDiaplay",false);
		StartCoroutine(WaitForAnimation (panelList,0.5f));

	}


	//=============================== Animaiton ==================================
	private IEnumerator WaitForAnimation (GameObject gameObject, float delaytime)
	{
		yield return new WaitForSeconds(delaytime);
		changeUIProperty (gameObject, false);
	}
		
	private IEnumerator AnimationDelay (GameObject gameObject, float delaytime)
	{
		yield return new WaitForSeconds(delaytime);
		dialogPanelAnimator.SetBool ("DialogOpen", true);
	}

	private IEnumerator WaitForMenuAnimationEnd (float delaytime)
	{
		yield return new WaitForSeconds(delaytime);
		menuObject.SetActive (false);


	}
		

	//=================================== Info in Dialog =========================================
	public void SetInfo (BodyParts bodyParts)
	{
		string text = "";

		if (bodyParts != null) {

			textTitle.text = "" + bodyParts.PartsName;

			textInformation1.text = "" + bodyParts.Origin;
			textInformation2.text = "" + bodyParts.Insertion;
			textInformation3.text = "" + bodyParts.Nerve;
			textInformation4.text = "" + bodyParts.Actions;

//			Debug.Log( "textInformation1   " + LayoutUtility.GetPreferredHeight(textInformation1.rectTransform));

			panel1.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1000f,
			LayoutUtility.GetPreferredHeight(textInformation1.rectTransform));
			
			panel2.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1000f,
				LayoutUtility.GetPreferredHeight(textInformation2.rectTransform));
			
			panel3.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1000f,
				LayoutUtility.GetPreferredHeight(textInformation3.rectTransform));
			
			panel4.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1000f,
				LayoutUtility.GetPreferredHeight(textInformation4.rectTransform));

			panel1.GetComponent<Transform> ().position = new Vector3 (40.0f,panel1.GetComponent<Transform> ().position.y,0.0f);

			float pos2 = panel1.GetComponent<Transform> ().position.y - LayoutUtility.GetPreferredHeight (textInformation1.rectTransform) - 15;

			panel2.GetComponent<Transform> ().position = new Vector3 (40.0f,pos2,0.0f);

			float pos3 = panel2.GetComponent<Transform> ().position.y - LayoutUtility.GetPreferredHeight (textInformation2.rectTransform) - 15;

			panel3.GetComponent<Transform> ().position = new Vector3 (40.0f,pos3,0.0f);

			float pos4 = panel3.GetComponent<Transform> ().position.y - LayoutUtility.GetPreferredHeight (textInformation3.rectTransform) - 15;

			panel4.GetComponent<Transform> ().position = new Vector3 (40.0f,pos4,0.0f);


			float totalHt = LayoutUtility.GetPreferredHeight (textInformation1.rectTransform) +
			                LayoutUtility.GetPreferredHeight (textInformation2.rectTransform) +
			                LayoutUtility.GetPreferredHeight (textInformation3.rectTransform) +
			                LayoutUtility.GetPreferredHeight (textInformation4.rectTransform) + 80;

			scrollPanel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1000f,totalHt);

//			Debug.Log( "p1   " +pos2 );
//			Debug.Log( "panel1   " + panel1.GetComponent<Transform> ().position.y );
//			Debug.Log( "panel2   " + panel2.GetComponent<Transform> ().position.y );
//			Debug.Log( "panel3   " + panel3.GetComponent<Transform> ().position.y );
//			Debug.Log( "panel4   " + panel4.GetComponent<Transform> ().position.y );

//			Debug.Log (" 2  " + bodyParts.Insertion);
//			Debug.Log (" 3  " + bodyParts.Nerve);
//			Debug.Log (" 4  " + bodyParts.Actions);


//			text += "[起始]  " + bodyParts.Origin + "\n";
////			text += "[起始]" + bodyParts.Origin + "\n";
//			text += "[停止]  " + bodyParts.Insertion + "\n";
//			text += "[支配神経]  " + bodyParts.Nerve + "\n";
//			text += "[作用]  " + bodyParts.Actions + "\n";



		} 


//		textInformation.text = text;

		imagePath = bodyParts.NerveImage;
		videoPath = bodyParts.ActionsMovie;

		if (imagePath == "") {

			var spriteImageDisable = Resources.Load<Sprite> ("images/button_photo_disabled");
			buttonImage.image.overrideSprite = spriteImageDisable;
		} else {

			var spriteImageDisable = Resources.Load<Sprite> ("images/button_photo");
			buttonImage.image.overrideSprite = spriteImageDisable;
			ImageSprite = Resources.Load<Sprite> ("images/nerve/" + imagePath) as Sprite;
		}

		if (videoPath == "") {

			var spriteVideoDisable = Resources.Load<Sprite> ("images/button_video_disabled");
			buttonVideo.image.overrideSprite = spriteVideoDisable;

		} else {

			var spriteVideoDisable = Resources.Load<Sprite> ("images/button_video");
			buttonVideo.image.overrideSprite = spriteVideoDisable;
		}
	}

	//============================== touch detect ===================================
	private void TouchInsideDialog(GameObject panel) {

		if (Input.GetMouseButton (0) && panel.activeSelf && RectTransformUtility.RectangleContainsScreenPoint (
				panel.GetComponent<RectTransform> (), 
				Input.mousePosition, 
				null)) {

				IsOnDialog = true;

			} else {

				IsOnDialog = false;
			}
	}

	//=============================== select on finger tap ===============================
	protected virtual void OnEnable ()
	{
		LeanTouch.OnFingerTap += OnFingerTap;
		LeanTouch.OnFingerDown += OnFingerDown;
		LeanTouch.OnFingerUp += OnFingerUp;
	}

	protected virtual void OnDisable ()
	{
		LeanTouch.OnFingerTap -= OnFingerTap;
		LeanTouch.OnFingerDown -= OnFingerDown;
		LeanTouch.OnFingerUp -= OnFingerUp;
	}

	private void OnFingerTap (LeanFinger finger)
	{
		touchFinger = finger.ScreenPosition;
		isSingleTap = true;
		StartCoroutine (timer ());

		if (finger.TapCount == 2 && IsActionAvailable  && !IsTouchOnButton) {
			isSingleTap = false;
			StopCoroutine (timer ());
//			OnHideShowNerve ();


			int colorIndex = colorToIndex (getColor (colorMapTex));

			if (colorIndex != 0) {
				string meshName = BodyPartsDataManager.Instance.GetMeshName (colorIndex);
				bool center = false;
				string STR = "";
				for (int i = 0; i < MuscleData.centerMucsle.Length; i++) {

					if (meshName.Contains (MuscleData.centerMucsle [i])) {
						STR = MuscleData.centerMucsle [i];
						center = true;

					} 
				}

				if (center) {
					OnHideShowCenterNerve (STR);
				} else {

					OnHideShowNerve (meshName);
				}
			}
		}
	}

	private void OnFingerDown (LeanFinger finger)
	{
		draggingFinger = finger;

	}

	private void OnFingerUp (LeanFinger finger)
	{
		if (finger == draggingFinger) {
			draggingFinger = null;
		}
	}

	//================================== Muscle Select/Hide ================================ 
	private void OnSelectionNurve ()
	{
		int colorIndex = colorToIndex (getColor (colorMapTex));

		if (colorIndex != 0) {
			string meshName = BodyPartsDataManager.Instance.GetMeshName (colorIndex);
				
			selectMesh (meshName);
			UpdateSubBuffer ();

		} else {

			if (selectedParts != null) {

				if (selectedParts.GetComponent<Renderer> ().materials.Length > 1) {
					Material[] ms = new Material[selectedParts.GetComponent<Renderer> ().materials.Length - 1];
					System.Array.Copy (selectedParts.GetComponent<Renderer> ().materials, ms, ms.Length);
					selectedParts.GetComponent<Renderer> ().materials = ms;
					selectedParts = null;
				}

				if(selectedParts_L != null){
					if (selectedParts_L.GetComponent<Renderer> ().materials.Length > 1) {
						Material[] ms_l = new Material[selectedParts_L.GetComponent<Renderer> ().materials.Length - 1];
						System.Array.Copy (selectedParts_L.GetComponent<Renderer> ().materials, ms_l, ms_l.Length);
						selectedParts_L.GetComponent<Renderer> ().materials = ms_l;
						selectedParts_L = null;
					}
				}

				dialogPanelAnimator.SetBool ("DialogOpen", false);

				StartCoroutine (WaitForAnimation (panelDialog, 0.5f));

			}
		}
	}
		
		private void OnHideShowCenterNerve (string STR)
	{

			MeshRenderer mr_r = BodyPartsDataManager.Instance.GetMesh ("R_"+STR, false);
			MeshRenderer mr_l = BodyPartsDataManager.Instance.GetMesh ("L_"+STR, false);

		UIManager.Instance.BodyPartsListUI.setCenterToogleFalse ("R_"+STR, false);

			if (mr_r != null) {

				if (mr_r == selectedMeshNerve) {
				SetCenterMeshVisible (STR, true);

				} else {

					selectedMeshNerve = mr_r;
					if (STR  == SelectedMeshName) {

						selectMesh (STR );
					}

				SetCenterMeshVisible (STR, false);

				}
			} 
			UpdateSubBuffer ();
		
	}


	private void OnHideShowNerve (string meshName)
	{

			MeshRenderer mr = BodyPartsDataManager.Instance.GetMesh (meshName, false);

			UIManager.Instance.BodyPartsListUI.setToogleFalse (meshName, false);

			if (mr != null) {

				if (mr == selectedMeshNerve) {
					SetMeshVisible (meshName, true);

				} else {
					
					selectedMeshNerve = mr;
					if (meshName == SelectedMeshName) {

						selectMesh (meshName);
					}

					SetMeshVisible (meshName, false);

				}
			} 
			UpdateSubBuffer ();
	}


	public void SetCenterMeshVisible (string meshName, bool visible)
	{
		
		MeshRenderer mr_r0 = BodyPartsDataManager.Instance.GetMesh ("R_"+meshName, false);	// main
		MeshRenderer mr_r1 = BodyPartsDataManager.Instance.GetMesh ("R_"+meshName, true);	// sub

		MeshRenderer mr_l0 = BodyPartsDataManager.Instance.GetMesh ("L_"+meshName, false);	// main
		MeshRenderer mr_l1 = BodyPartsDataManager.Instance.GetMesh ("L_"+meshName, true);	// sub

		mr_r0.enabled = mr_r1.enabled = visible;
		mr_l0.enabled = mr_l1.enabled = visible;

		// 
		UIManager.Instance.BodyPartsListUI.Silent = true;
		UIManager.Instance.BodyPartsListUI.SetCenterItemVisibility(meshName,visible);
		UIManager.Instance.BodyPartsListUI.Silent = false;
		// 

		hiddenMeshList.RemoveAll (mr => mr == mr_r0);
		hiddenMeshList.RemoveAll (mr => mr == mr_l0);
		if (!visible) {
			hiddenMeshList.Add (mr_r0);
			hiddenMeshList.Add (mr_l0);
		}

		UpdateSubBuffer ();
	}


	int colorToIndex (Color color)
	{
		return Mathf.RoundToInt (color.r * 10f) * 100 + Mathf.RoundToInt (color.g * 10f) * 10 + Mathf.RoundToInt (color.b * 10f);
	}

	Color getColor (Texture2D tex)
	{
		float x = touchFinger.x / (float)Screen.width;
		float y = touchFinger.y / (float)Screen.height;
		float r = (float)Screen.height / (float)Screen.width;
		// 
		if (Screen.width > Screen.height) {
			y = y * r + (1f - r) / 2f;
		} else {
			r = 1f / r;
			x = x * r + (1f - r) / 2f;
		}
		return tex.GetPixel ((int)(x * tex.width), (int)(y * tex.height));
	}

	MeshRenderer selectedParts_L = null;
	public void selectCenterMesh (string meshName)
	{
		MeshRenderer mr_R = BodyPartsDataManager.Instance.GetMesh ("R_"+meshName, false);
		MeshRenderer mr_L = BodyPartsDataManager.Instance.GetMesh ("L_"+meshName, false);



		BodyParts bp = null;
		if (mr_R != null) {
			string kid = mr_R.transform.parent.name;
		
			bp = BodyPartsDataManager.Instance.GetBodyParts (kid);
		}

		if (bp != null) {

			bool isAlreadySelected = false;
			if (mr_R == selectedParts && mr_L == selectedParts_L) {
				isAlreadySelected = true;
			}

			if (isAlreadySelected) {

				if (selectedParts != null && selectedParts_L != null) {

						
						if (selectedParts.GetComponent<Renderer> ().materials.Length > 1) {
							Material[] ms = new Material[selectedParts.GetComponent<Renderer> ().materials.Length - 1];
							System.Array.Copy (selectedParts.GetComponent<Renderer> ().materials, ms, ms.Length);
							selectedParts.GetComponent<Renderer> ().materials = ms;
							selectedParts = null;
						}

					if (selectedParts_L.GetComponent<Renderer> ().materials.Length > 1) {
						Material[] ms_l = new Material[selectedParts_L.GetComponent<Renderer> ().materials.Length - 1];
						System.Array.Copy (selectedParts_L.GetComponent<Renderer> ().materials, ms_l, ms_l.Length);
						selectedParts_L.GetComponent<Renderer> ().materials = ms_l;
						selectedParts_L = null;
					}

					dialogPanelAnimator.SetBool ("DialogOpen", false);

					StartCoroutine (WaitForAnimation (panelDialog, 0.5f));


				}
			} else {

				if (selectedParts != null) {

					if (selectedParts.GetComponent<Renderer> ().materials.Length > 1) {
						Material[] ms = new Material[selectedParts.GetComponent<Renderer> ().materials.Length - 1];
						System.Array.Copy (selectedParts.GetComponent<Renderer> ().materials, ms, ms.Length);
						selectedParts.GetComponent<Renderer> ().materials = ms;
						selectedParts = null;
					}


					if (selectedParts_L != null && selectedParts_L.GetComponent<Renderer> ().materials.Length > 1) {
						Material[] ms_l = new Material[selectedParts_L.GetComponent<Renderer> ().materials.Length - 1];
						System.Array.Copy (selectedParts_L.GetComponent<Renderer> ().materials, ms_l, ms_l.Length);
						selectedParts_L.GetComponent<Renderer> ().materials = ms_l;
						selectedParts_L = null;
					}
				}

				if (mr_R != null && mr_L != null) {

					Material[] ms_r = new Material[mr_R.GetComponent<Renderer> ().materials.Length + 1];
					System.Array.Copy (mr_R.GetComponent<Renderer> ().materials, ms_r, ms_r.Length - 1);
					ms_r [ms_r.Length - 1] = new Material (SelectedPartsMaterial);
					mr_R.GetComponent<Renderer> ().materials = ms_r;

					Material[] ms_l = new Material[mr_L.GetComponent<Renderer> ().materials.Length + 1];
					System.Array.Copy (mr_L.GetComponent<Renderer> ().materials, ms_l, ms_l.Length - 1);
					ms_l [ms_l.Length - 1] = new Material (SelectedPartsMaterial);
					mr_L.GetComponent<Renderer> ().materials = ms_l;
				
					selectedParts = mr_R;
					selectedParts_L = mr_L;
				
				}

					if (panelDialog.activeSelf) {

						dialogPanelAnimator.SetBool ("DialogOpen", false);
						StartCoroutine (AnimationDelay (panelDialog, 0.5f));
					} else {

						changeUIProperty (panelDialog, true);
						dialogPanelAnimator.SetBool ("DialogOpen", true);
					}

			}
			SetInfo (bp);
			SelectedMeshName = meshName;
		}
	}



	public void selectMesh (string meshName)
	{
//		Debug.Log (meshName);
		bool center = false;
		string STR = "";
		for(int i=0; i<MuscleData.centerMucsle.Length; i++){
		
			if (meshName.Contains (MuscleData.centerMucsle [i])) {
				STR = MuscleData.centerMucsle [i];
				center = true;

			} 
		}

			if(center){
			selectCenterMesh (STR);

		} else {

			MeshRenderer mr = BodyPartsDataManager.Instance.GetMesh (meshName, false);

			BodyParts bp = null;
			if (mr != null) {
				string kid = mr.transform.parent.name;
		
				bp = BodyPartsDataManager.Instance.GetBodyParts (kid);
			}

			if (bp != null) {

				bool isAlreadySelected = false;
				if (mr == selectedParts) {
					isAlreadySelected = true;
				}

				if (isAlreadySelected) {

					if (selectedParts != null) {

						if (selectedParts.GetComponent<Renderer> ().materials.Length > 1) {
							Material[] ms = new Material[selectedParts.GetComponent<Renderer> ().materials.Length - 1];
							System.Array.Copy (selectedParts.GetComponent<Renderer> ().materials, ms, ms.Length);
							selectedParts.GetComponent<Renderer> ().materials = ms;
							selectedParts = null;
						}

						if(selectedParts_L != null){
							if (selectedParts_L.GetComponent<Renderer> ().materials.Length > 1) {
								Material[] ms_l = new Material[selectedParts_L.GetComponent<Renderer> ().materials.Length - 1];
								System.Array.Copy (selectedParts_L.GetComponent<Renderer> ().materials, ms_l, ms_l.Length);
								selectedParts_L.GetComponent<Renderer> ().materials = ms_l;
								selectedParts_L = null;
							}
						}

						dialogPanelAnimator.SetBool ("DialogOpen", false);

						StartCoroutine (WaitForAnimation (panelDialog, 0.5f));

					}
				} else {
				
					if (selectedParts != null) {

						if (selectedParts.GetComponent<Renderer> ().materials.Length > 1) {
							Material[] ms = new Material[selectedParts.GetComponent<Renderer> ().materials.Length - 1];
							System.Array.Copy (selectedParts.GetComponent<Renderer> ().materials, ms, ms.Length);
							selectedParts.GetComponent<Renderer> ().materials = ms;
							selectedParts = null;
						}

						if(selectedParts_L != null){
							if (selectedParts_L.GetComponent<Renderer> ().materials.Length > 1) {
								Material[] ms_l = new Material[selectedParts_L.GetComponent<Renderer> ().materials.Length - 1];
								System.Array.Copy (selectedParts_L.GetComponent<Renderer> ().materials, ms_l, ms_l.Length);
								selectedParts_L.GetComponent<Renderer> ().materials = ms_l;
								selectedParts_L = null;
							}
						}
					}
			
					if (mr != null) {

						Material[] ms = new Material[mr.GetComponent<Renderer> ().materials.Length + 1];
						System.Array.Copy (mr.GetComponent<Renderer> ().materials, ms, ms.Length - 1);
						ms [ms.Length - 1] = new Material (SelectedPartsMaterial);
						mr.GetComponent<Renderer> ().materials = ms;
						selectedParts = mr;
					}

					if (panelDialog.activeSelf) {

						dialogPanelAnimator.SetBool ("DialogOpen", false);
						StartCoroutine (AnimationDelay (panelDialog, 0.5f));
					} else {

						changeUIProperty (panelDialog, true);
						dialogPanelAnimator.SetBool ("DialogOpen", true);
					}
				}
				SetInfo (bp);
				SelectedMeshName = meshName;
			}
		}
	}

	public void SetMeshVisible (string meshName, bool visible)
	{
		MeshRenderer mr0 = BodyPartsDataManager.Instance.GetMesh (meshName, false);	// main
		MeshRenderer mr1 = BodyPartsDataManager.Instance.GetMesh (meshName, true);	// sub

		mr0.enabled = mr1.enabled = visible;

		// 
		UIManager.Instance.BodyPartsListUI.Silent = true;
		UIManager.Instance.BodyPartsListUI.SetItemVisibility(meshName,visible);
		UIManager.Instance.BodyPartsListUI.Silent = false;
		// 

		hiddenMeshList.RemoveAll (mr => mr == mr0);
		if (!visible) {
			hiddenMeshList.Add (mr0);
		}

		UpdateSubBuffer ();
	}

	//================================== Other functions ====================================
	void changeUIProperty (GameObject gameObject, bool disable)
	{
		gameObject.SetActive (disable);
	}

	//---------------------------------------------------------------------------------------
	public void UpdateSubBuffer ()
	{
		bUpdate = true;
	}

	//-----------------------------------------------------------------------------------------
	void renderSubCamera ()
	{
		int w = Screen.width;
		int h = Screen.height;

		subCam.orthographicSize = Camera.main.orthographicSize * Mathf.Max (1f, (float)w / (float)h);
		RenderTexture.active = subCam.targetTexture;
		subModel.SetActive (true);
		subCam.Render ();
		subModel.SetActive (false);
		colorMapTex.ReadPixels (new Rect (0, 0, colorMapTex.width, colorMapTex.height), 0, 0);
		colorMapTex.Apply ();
		RenderTexture.active = null;
	}

	//------------------------------------------------------------------------------------------
	public GameObject duplicateSubModel (GameObject master)
	{
		Dictionary<int,string> meshNameDict = new Dictionary<int,string> ();

		int index = 1;
		foreach (MeshRenderer mr in master.GetComponentsInChildren<MeshRenderer>()) {
			meshNameDict.Add (index, mr.name);
		
			string kid = mr.transform.parent.gameObject.name;


			BodyParts bp = BodyPartsDataManager.Instance.GetBodyParts (kid);
		

			if (bp != null && bp.GameObject == null) {
				bp.GameObject = mr.transform.parent.gameObject;
			}
			index++;
		}
			
		GameObject subModel = (GameObject)GameObject.Instantiate (master, Vector3.zero, Quaternion.identity);
		subModel.name = "subModel";
		subModel.transform.parent = master.transform.parent;

		index = 1;
		Material baseMaterial = new Material (ColoredPartsShader);
		foreach (MeshRenderer mr in subModel.GetComponentsInChildren<MeshRenderer>()) {
			
			Material m = new Material (baseMaterial);
			m.color = indexToColor (index);
			mr.materials = new Material[]{ m };
			mr.gameObject.layer = CURRING_MASK_INDEX;

			string kid = mr.transform.parent.gameObject.name;
			BodyParts bp = BodyPartsDataManager.Instance.GetBodyParts (kid);

			if (bp != null && bp.SubGameObject == null) {
				bp.SubGameObject = mr.transform.parent.gameObject;
			}

			index++;
		}

		subModel.SetActive (false);
 
		BodyPartsDataManager.Instance.MeshNameDict = meshNameDict;

		return subModel;
	}

	//-----------------------------------------------------------------------------------------
	private bool IsChildOf (Transform target)
	{
		if (target != null) {
			if (target == selectedModel.transform) {
				return true;
			}
			return IsChildOf (target.parent);
		}
		return false;
	}
		
	//-----------------------------------------------------------------------------------------
	private IEnumerator timer ()
	{
		yield return new WaitForSeconds (0.3f);

		if (isSingleTap) {

			if (IsActionAvailable && !IsTouchOnButton) {
				OnSelectionNurve ();
			}
			}
	}

	//------------------------------------------------------------------------------------------
	public void Reset ()
	{
		if (selectedParts != null) {

			if (selectedParts.GetComponent<Renderer> ().materials.Length > 1) {
				Material[] ms = new Material[selectedParts.GetComponent<Renderer> ().materials.Length - 1];
				System.Array.Copy (selectedParts.GetComponent<Renderer> ().materials, ms, ms.Length);
				selectedParts.GetComponent<Renderer> ().materials = ms;
				selectedParts = null;
			}

			if(selectedParts_L != null){
				if (selectedParts_L.GetComponent<Renderer> ().materials.Length > 1) {
					Material[] ms_l = new Material[selectedParts_L.GetComponent<Renderer> ().materials.Length - 1];
					System.Array.Copy (selectedParts_L.GetComponent<Renderer> ().materials, ms_l, ms_l.Length);
					selectedParts_L.GetComponent<Renderer> ().materials = ms_l;
					selectedParts_L = null;
				}
			}

		}

		string gameObjectName = MuscleData.GameObjectName;

		int index = 0;
		int startIndex = 0;
		int endIndex = 35;


		if (gameObjectName == "HeadCube") {

			startIndex = 0;
			endIndex = 35;

		} else if (gameObjectName == "NeckCube") {

			startIndex = 36;
			endIndex = 60;

		} else if (gameObjectName == "ChestCube") {

			startIndex = 61;
			endIndex = 75;

		} else if (gameObjectName == "AbdomenCube") {

			startIndex = 76;
			endIndex = 85;

		} else if (gameObjectName == "Upper_limbsCube") {

			startIndex = 113;
			endIndex = 166;

		} else if (gameObjectName == "Lower_limbsCube") {

			startIndex = 167;
			endIndex = 234;

		} else if (gameObjectName == "BackCube") {

			startIndex = 86;
			endIndex = 112;

		} else {

			startIndex = 0;
			endIndex = 234;
		}

		UIManager.Instance.BodyPartsListUI.resetItems ();

		foreach (BodyParts bp in BodyPartsDataManager.Instance.BodyPartsDict.Values) {
			index++;

			if (index >= startIndex && index <= endIndex) {

				bp.MeshList.ForEach (mesh => mesh.enabled = true);
				bp.SubMeshList.ForEach (mesh => mesh.enabled = true);

			}

		}

		hiddenMeshList.Clear ();

		changeUIProperty (panelDialog, false);

		UpdateSubBuffer ();
	}
		
	//---------------------------------------------------------------------------------------------
	Color indexToColor (int value)
	{
		return new Color ((float)(value % 1000 / 100) * 0.1f, (float)(value % 100 / 10) * 0.1f, (float)(value % 10) * 0.1f);
	}

	//--------------------------------------------------------------------------------------------
	public string[] GetHiddenMeshList ()
	{
		List<string> dest = new List<string> ();
	
		hiddenMeshList.ForEach (mr => dest.Add (mr.name));

		return dest.ToArray ();
	}

	//---------------------------------------------------------------------------------------------
	public void SetHiddenMeshList (string[] hiddenList)
	{
		Reset ();

		foreach (string meshName in hiddenList) {
			UIManager.Instance.BodyPartsListUI.setToogleFalse (meshName, false);
			SetMeshVisible (meshName, false);
		}
	}

}

