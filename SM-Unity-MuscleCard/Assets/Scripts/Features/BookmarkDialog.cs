using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class BookmarkDialog : MonoBehaviour
{

//	String FileName = "";
	public GameObject ItemPrefab;

	public GameObject MsgPanel;
	public GameObject ButtonPanel;
	public GameObject ListBase;
	public SaveDialog SaveDialog;
	public Button UserDataButton;
	public Button PresetDataButton;
	public Text UserDataButtonText;
	public Text PresetDataButtonText;
	public BookmarkListItem _selectedItem;

	public BookmarkListItem SelectedItem {
		get { return _selectedItem; }
		set {
			// 
			if (_selectedItem != null) {
				_selectedItem.Selected = false;
			}
			// 
			_selectedItem = value;

			ButtonPanel.SetActive (_selectedItem != null);
		}
	}

//	public void OnPointerClick( PointerEventData eventData )
//	{
//		UIManager.Instance.BodyPartsListUI.OnItemClick(this);
//	}

	//
	String saveDirPath {
		
		get { return Application.persistentDataPath + "/saveData"; }
	}
		
	//------------------------------------------------------------------------------
	public void OpenSaveDialog ()
	{
		this.SaveDialog.gameObject.SetActive (true);

		this.SaveDialog.TextInput.text = "";

//			this.SaveDialog.TextInput.text = DateTime.Now.ToString ("yyyymmdd_hhmmss");

		this.SaveDialog.ErrorMessageText.text = "";

	}

	//------------------------------------------------------------------------------
	public void OpenLoadDialog ()
	{

		if (!Directory.Exists (saveDirPath)) {
			Directory.CreateDirectory (saveDirPath);
		}

		UpdateItemList (false);
		SelectedItem = null;
		SwitchDataButtonsHighlight (false);

//		Debug.Log ("" +saveDirPath);
//		this.gameObject.SetActive (true);
	}

	//------------------------------------------------------------------------------
	public void SwitchDataButtonsHighlight (bool isPresetData)
	{
		UserDataButton.image.color = !isPresetData ? new Color32( 0xFF, 0xFF, 0xFF, 0xFF ) : new Color32( 0xEC, 0x49, 0x2D, 0xFF ); 
		PresetDataButton.image.color = isPresetData ? new Color32( 0xFF, 0xFF, 0xFF, 0xFF )  : new Color32( 0xEC, 0x49, 0x2D, 0xFF ); 
		UserDataButtonText.color = !isPresetData ? new Color32 ( 0xEC, 0x49, 0x2D, 0xFF ) : new Color32( 0xFF, 0xFF, 0xFF, 0xFF ); 
		PresetDataButtonText.color = isPresetData ? new Color32 ( 0xEC, 0x49, 0x2D, 0xFF ) : new Color32( 0xFF, 0xFF, 0xFF, 0xFF ); 
	}

	//------------------------------------------------------------------------------
	public void OnSaveButonClick ()
	{
		if (Save ()) {
			this.SaveDialog.gameObject.SetActive (false);
		}
	}

	//------------------------------------------------------------------------------
	public bool Save ()
	{
		String dataName = this.SaveDialog.TextInput.text;

		if (dataName == null || dataName.Length == 0) {
			this.SaveDialog.ErrorMessageText.text = "Please enter a name.";
			return false;
		} else {
			foreach (char c in Path.GetInvalidFileNameChars()) {
				if (dataName.Contains (c.ToString ())) {
					this.SaveDialog.ErrorMessageText.text = "Please remove unavailable character.";
					return false;
				}
			}
		}

		String fileName = dataName + ".txt";

		BookmarkData data = new BookmarkData ();

		data.DataName = dataName;
		data.ObjectName = MuscleData.GameObjectName;
		data.ObjectId = MuscleData.GameObjectID;

		if (UIManager.Instance.SelectionManager.panelDialog.activeSelf) {
			data.SelectedMeshName = UIManager.Instance.SelectionManager.SelectedMeshName;
		} else {

			data.SelectedMeshName = "";
		}

		data.HiddenParts = UIManager.Instance.SelectionManager.GetHiddenMeshList ();

		data.CameraTransform = UIManager.Instance.CameraController.CameraTransform;
		data.CameraRotation = UIManager.Instance.CameraController.CameraRotation;
		data.CameraZoom = UIManager.Instance.CameraController.CameraZoom;
		data.BackgroundColor = Camera.main.backgroundColor;

		Texture2D tex = UIManager.Instance.CameraController.CaptureScreen ();
		data.ImageData = tex.EncodeToJPG (90);


		// 
		if (!Directory.Exists (saveDirPath)) {
			Directory.CreateDirectory (saveDirPath);
		}
		if (File.Exists (saveDirPath + "/" + fileName)) {
			this.SaveDialog.ErrorMessageText.text = "This name is already used.";

			return false;
		}
		try {
			File.WriteAllText (saveDirPath + "/" + fileName, data.ToJSON ());

		} catch (Exception e) {
			Debug.LogError (e.StackTrace);
			this.SaveDialog.ErrorMessageText.text = "System error has occurred.";

			return false;
		}
		return true;
	}
		
	//------------------------------------------------------------------------------
	public void Load ()
	{
		if (SelectedItem.IsInvalid) {
			return;
		}

		BookmarkData data = SelectedItem.Data;

//		FileName = data.DataName;
//		UIManager.Instance.SelectionManager.selectMesh(UIManager.Instance.SelectionManager.SelectedMeshName);

		UIManager.Instance.SelectionManager.Reset();
		UIManager.Instance.SelectionManager.SetHiddenMeshList (data.HiddenParts);
	

		UIManager.Instance.CameraController.Reset();
		UIManager.Instance.CameraController.CameraTransform	 =  data.CameraTransform;
		UIManager.Instance.CameraController.CameraRotation	 =  data.CameraRotation;
		UIManager.Instance.CameraController.CameraZoom		 =  data.CameraZoom;
		Camera.main.backgroundColor                          =  data.BackgroundColor;
		UIManager.Instance.CameraController.UpdateCamera();

		UIManager.Instance.SelectionManager.selectMesh(data.SelectedMeshName);

		StartCoroutine(ExicuteCodeAfter ());
	}

	public void OnOpenButtonClick ()
	{
		if (this.SelectedItem != null) {
			Load ();
		}
	}

	//------------------------------------------------------------------------------
//	public void OnDeleteButtonClick ()
//	{
//		
//		foreach (String path in GetBookmarkDataFilePaths()) {
//			File.Delete (path);
//		}
//		UpdateItemList (false);
//		SelectedItem = null;
//	}
	//------------------------------------------------------------------------------
		public void OnDeleteButtonClick()
		{
			if( SelectedItem == null || SelectedItem.DataPath == null || SelectedItem.DataPath.Length <= 0 ){
				return;
			}
			File.Delete( SelectedItem.DataPath );
			UpdateItemList(false);
			SelectedItem = null;
		}

	//------------------------------------------------------------------------------
	string[] GetBookmarkDataFilePaths ()
	{
		FileInfo[] infos = new DirectoryInfo (saveDirPath).GetFiles ("*.txt", SearchOption.TopDirectoryOnly);
		// 
		Array.Sort (infos, (a, b) => {
			return a.CreationTime.CompareTo (b.CreationTime);
		});
		List<string> paths = new List<string> ();
		foreach (FileInfo f in infos) {
			paths.Add (f.FullName);
		}
		return paths.ToArray ();
	}

	//------------------------------------------------------------------------------
	public void UpdateItemList (bool isPreset)
	{
		int i = 0;
		// 子要素をすべて削除
		foreach (Transform t in ListBase.transform) {
			Destroy (t.gameObject);
		}
		// 
		Func<string,BookmarkListItem> loadItem = (jsonData) => {

			BookmarkData data  = new BookmarkData (jsonData);

			if(data.ObjectName== MuscleData.GameObjectName){

			GameObject itemObj = (GameObject)GameObject.Instantiate (Resources.Load ("UI/BookmarkListItem"), Vector3.zero, Quaternion.identity);

			// Parent
			((RectTransform)itemObj.transform).SetParent (ListBase.transform);
			// Item
			BookmarkListItem item = itemObj.GetComponent<BookmarkListItem> ();
	
			try {
				item.SetData (data);

			} catch (Exception e) {
				
				Debug.Log (e.Message);
				item.IsInvalid = true;
				item.TextLabel.text = "-";
			}
			return item;
		}
			return null;
		};
			
		if (!isPreset) {
			string[] paths = GetBookmarkDataFilePaths ();
			foreach (string path in paths) {
				String jsonData = File.ReadAllText (path);
				BookmarkListItem item = loadItem (jsonData);

				if (item != null) {
					item.DataPath = path;
					i++;
				}
			}
		} else {
			var assets = Resources.LoadAll ("preset");
			foreach (TextAsset asset in assets) {
				BookmarkListItem item = loadItem (asset.text);
				if (item != null) {
					item.DataPath = null;
				}
			}
		}
		SelectedItem = null;
		SwitchDataButtonsHighlight (isPreset);

		if (i < 1) {
			MsgPanel.SetActive (true);
		} else {
			MsgPanel.SetActive (false);
		}
	}

	private IEnumerator ExicuteCodeAfter ()
	{
		yield return new WaitForSeconds (0.5f);
		this.gameObject.SetActive (false);
	}
}
