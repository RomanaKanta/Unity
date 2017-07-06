//==============================================================================
// 
// BodyPartsListUI.cs
// 
// 部位一覧UIクラス
// 
//==============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BodyPartsListUI : Singleton<BodyPartsListUI>
{
	//------------------------------------------------------------------------------
	public VerticalLayoutGroup ScrollContent;
	public Dictionary<string,BodyPartsListItem> Items = new Dictionary<string,BodyPartsListItem>();
	public bool Silent = false;		// Toggle更新時に3Dモデルを更新しない

	private BodyPartsListItem selectedItem;
	//------------------------------------------------------------------------------
	public void Awake()
	{

		//		Debug.Log("Awake ");
		// ScrollContentの子要素をすべて削除
		foreach( Transform t in ScrollContent.transform ){
			Destroy(t.gameObject);
		}
		// 部位辞書
		Dictionary<string,BodyParts> dict = BodyPartsDataManager.Instance.BodyPartsDict;
		// リストUI生成
		foreach( KeyValuePair<string,BodyParts> kv in dict ){
			GameObject itemObj = (GameObject)GameObject.Instantiate(Resources.Load("UI/ListItem"),Vector3.zero,Quaternion.identity);
			// Parent
			((RectTransform)itemObj.transform).SetParent(ScrollContent.transform);
			itemObj.name = kv.Key;
			// Item初期化
			BodyPartsListItem item = itemObj.GetComponent<BodyPartsListItem>();
			// 
			item.BodyParts = kv.Value;
//						item.LabelText.text = kv.Value.PartsName + "  " + kv.Value.KinID;
			item.LabelText.text = kv.Value.PartsName;
			item.SetIndentLevel(kv.Value.LayerIndex);



			item.ToggleCenter.gameObject.SetActive (false);
			item.ToggleLeft.gameObject.SetActive (true);
			item.ToggleRight.gameObject.SetActive (true);

			for(int i=0; i<MuscleData.centerMucsle.Length; i++){

				if (MuscleData.centerMucsle [i].Equals (item.BodyParts.KinID)) {

					item.ToggleCenter.gameObject.SetActive (true);
					item.ToggleLeft.gameObject.SetActive (false);
					item.ToggleRight.gameObject.SetActive (false);
				}

			}



			if (MuscleData.GameObjectName != "All") {
				itemObj.SetActive (false);
				//				item.ToggleLeft.isOn = false;
				//				item.ToggleRight.isOn = false;

				if (MuscleData.GameObjectID == kv.Value.KinID) {
					itemObj.SetActive (true);
					item.ToggleLeft.isOn = true;
					item.ToggleRight.isOn = true;
					item.ToggleCenter.isOn = true;

					selectedItem = item;

				}
			}


			// 最上位階層のみ表示する
			if( kv.Value.LayerIndex > 0 ){
				item.gameObject.SetActive(false);
			}
			// 
			Items[kv.Key] = item;
			// Scaleがゼロクリアされる問題を修正
			itemObj.transform.localScale = Vector3.one;
		}
		// 親子階層
		foreach( BodyPartsListItem item in Items.Values ){


			if( item.BodyParts.ParentID != null ){
				item.Parent = Items[item.BodyParts.ParentID];
			}
			if( item.BodyParts.ChildIDs != null ){
				item.BodyParts.ChildIDs.ForEach( id => {
					item.Children.Add( Items[id] );
				});
			}
			// 子要素を持つ項目は伸縮アイコンを表示
			if( item.Children.Count > 0 ){
				item.toggleImage.enabled = true;
			}
		}

		if (MuscleData.GameObjectName != "All") {

			ExpandItem (selectedItem);
		}
	}

	private void ExpandItem( BodyPartsListItem item ){
		OnItemClick (item);
		if (item.Children.Count > 0) {
			item.Children.ForEach( child => {
				ExpandItem (child);
			});
		}
	}

	//------------------------------------------------------------------------------
	// クリック時処理
	//------------------------------------------------------------------------------
	public void OnItemClick( BodyPartsListItem item )
	{
		if( item.IsExpanded ){
			item.Fold();
		}else{
			item.Expand();
		}
	}

	//******************************************************************************

	public void setToogleFalse(string meshName, bool visibility){
		string kin = BodyPartsDataManager.MeshNameToKinID(meshName);

		foreach( BodyPartsListItem item in Items.Values ){

			if( meshName.Contains("R_") ){

				if( item.BodyParts.KinID == kin ){
					item.ToggleRight.isOn = visibility;
				}
			}else{
				if( item.BodyParts.KinID == kin ){
					item.ToggleLeft.isOn = visibility;
				}
			}
		}
	}

	public void setCenterToogleFalse(string meshName, bool visibility){
		string kin = BodyPartsDataManager.MeshNameToKinID(meshName);

		foreach( BodyPartsListItem item in Items.Values ){

				if( item.BodyParts.KinID == kin ){
				item.ToggleCenter.isOn = visibility;
				}
		}
	}

	//******************************************************************************


	//------------------------------------------------------------------------------
	// チェックボックス処理
	//------------------------------------------------------------------------------


	public void OnItemToggleClick( BodyPartsListItem item, BodyParts.PartsTypeEnum partsType, bool isOn )
	{
		if (Silent) {
			return;
		}

		// 部位表示切り替え
		MeshRenderer mr = item.BodyParts.GetMeshRenderer(partsType,false);
		if( mr != null ){


			bool center = false;
			string STR = "";
			for (int i = 0; i < MuscleData.centerMucsle.Length; i++) {

				if (mr.name.Contains (MuscleData.centerMucsle [i])) {
					STR = MuscleData.centerMucsle [i];
					center = true;

				} 
			}

			if (center) {
				UIManager.Instance.SelectionManager.SetCenterMeshVisible(STR,isOn);
			} else {

				UIManager.Instance.SelectionManager.SetMeshVisible(mr.name,isOn);
			}


		}

		// 子要素の処理
		item.Children.ForEach (i => {
			i.GetToggle (partsType).isOn = isOn;
		});
	}

	//------------------------------------------------------------------------------
	// 選択部位の表示切り替え
	//------------------------------------------------------------------------------
	public void SetItemVisibility( string meshName, bool visible )
	{
		//		Debug.Log("### SetItemVisibility "+meshName+" "+visible);
		string kid = BodyPartsDataManager.MeshNameToKinID(meshName);
		BodyPartsListItem item = Items[kid];
		// 
		if( meshName.Contains("L_") ){
			item.GetToggle(BodyParts.PartsTypeEnum.Left).isOn = visible;
		}else if( meshName.Contains("R_") ){
			item.GetToggle(BodyParts.PartsTypeEnum.Right).isOn = visible;
		}else{
			item.GetToggle(BodyParts.PartsTypeEnum.Left).isOn = visible;
			item.GetToggle(BodyParts.PartsTypeEnum.Right).isOn = visible;
		}

		if(item.GetToggle(BodyParts.PartsTypeEnum.Left).isOn == item.GetToggle(BodyParts.PartsTypeEnum.Right).isOn){
			item.GetToggle (BodyParts.PartsTypeEnum.Center).isOn = item.GetToggle (BodyParts.PartsTypeEnum.Right).isOn;
		}

		// 
		if( item.Parent != null ){
			item.Parent.UpdateToggle();
		}
	}

	public void SetCenterItemVisibility( string meshName, bool visible )
	{
		string kid_r = BodyPartsDataManager.MeshNameToKinID("R_"+meshName);
		BodyPartsListItem item_r = Items[kid_r];

		item_r.GetToggle(BodyParts.PartsTypeEnum.Center).isOn = visible;
		item_r.GetToggle(BodyParts.PartsTypeEnum.Left).isOn = visible;
		item_r.GetToggle(BodyParts.PartsTypeEnum.Right).isOn = visible;

		if( item_r.Parent != null ){
			item_r.Parent.UpdateToggle();
		}
	}
	//------------------------------------------------------------------------------
	public void ToggleVisible()
	{
		gameObject.SetActive( ! gameObject.activeSelf );
	}
	//------------------------------------------------------------------------------
	// 表示状態の初期化
	//------------------------------------------------------------------------------
	public void resetItems()
	{
		Silent = true;
		foreach( BodyPartsListItem item in Items.Values ){
			item.ToggleLeft.isOn = true;
			item.ToggleRight.isOn = true;
			item.ToggleCenter.isOn = true;
			item.Fold();
		}
		Silent = false;
	}

	public void resetSelectedItems()
	{
		Silent = true;
		foreach( BodyPartsListItem item in Items.Values ){
			item.ToggleLeft.isOn = true;
			item.ToggleRight.isOn = true;
			item.ToggleCenter.isOn = true;
			//			item.Fold();
		}
		Silent = false;
	}
	//------------------------------------------------------------------------------
	// 表示状態の反映
	//------------------------------------------------------------------------------
	public void SetItemVisibilitySetting( List<string> hiddenMeshes )
	{
		resetItems();
		// 
		hiddenMeshes.ForEach( name => {
			// 3Dモデルの更新も行われる
			SetItemVisibility(name,false);
		});
	}
	//------------------------------------------------------------------------------
	// 表示状態の読み込み
	//------------------------------------------------------------------------------
	public List<string> GetItemVisibilitySetting()
	{
		List<string> hiddenMeshes = new List<string>();
		foreach( BodyPartsListItem item in Items.Values ){
			// 
			if( item.Children.Count > 0 ){
				continue;
			}
			// 
			if( !item.ToggleLeft.isOn ){
				hiddenMeshes.Add( "L_"+item.BodyParts.KinID );
			}
			if( !item.ToggleRight.isOn ){
				hiddenMeshes.Add( "R_"+item.BodyParts.KinID );
			}
		}
		return hiddenMeshes;
	}
	public void save()
	{
		tmp = GetItemVisibilitySetting();
	}
	List<string> tmp;
	public void load()
	{
		SetItemVisibilitySetting(tmp);
	}
}
