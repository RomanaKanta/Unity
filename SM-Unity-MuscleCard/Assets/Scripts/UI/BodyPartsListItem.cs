using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BodyPartsListItem : MonoBehaviour, IPointerClickHandler
{
	public GameObject BaseObject;
	public BodyParts BodyParts;
	public Text LabelText;
	public Toggle ToggleLeft;
	public Toggle ToggleRight;
	public Toggle ToggleCenter;
	public Sprite toggleSpriteOpened;
	public Sprite toggleSpriteClosed;
	public Image toggleImage;
	// 
	public BodyPartsListItem Parent;
	public List<BodyPartsListItem> Children = new List<BodyPartsListItem>();
	// 
	public bool IsExpanded;
	//--------------------------------------------------------------
	public Toggle GetToggle( BodyParts.PartsTypeEnum partsType )
	{
		if( partsType == global::BodyParts.PartsTypeEnum.Left ){
			return ToggleLeft;
		}else if( partsType == global::BodyParts.PartsTypeEnum.Right ){
			return ToggleRight;
		}else if( partsType == global::BodyParts.PartsTypeEnum.Center ){
			return ToggleCenter;
		}else{
			return null;
		}
	}
	//--------------------------------------------------------------
	public void SetIndentLevel( int level )
	{
		RectTransform rt = (RectTransform)BaseObject.transform;
		Image img = BaseObject.GetComponent<Image>();
		// 
		rt.offsetMin = new Vector2( level * 30, 0 );
		img.color = new Color(88,88,88,0.4f-level*0.15f);
	}
	//--------------------------------------------------------------
	public int VisibleItemCount
	{
		get{
			int count = (this.enabled) ? 1 : 0;
			// 
			Children.ForEach( c => {
				count += c.VisibleItemCount;
			});
			// 
			return count;
		}
	}
	//--------------------------------------------------------------
	public void OnToggleClick( Toggle toggle )
	{
		if( toggle == null ){
			return;
		}
		BodyParts.PartsTypeEnum partsType;

		if(toggle.name.Contains("Left")){
			partsType = BodyParts.PartsTypeEnum.Left;
		}else if((toggle.name.Contains("Right"))){
			 partsType = BodyParts.PartsTypeEnum.Right;
		}else{
			partsType = BodyParts.PartsTypeEnum.Right;
		}

//		BodyParts.PartsTypeEnum partsType = toggle.name.Contains("Left") ? BodyParts.PartsTypeEnum.Left : BodyParts.PartsTypeEnum.Right;
		UIManager.Instance.BodyPartsListUI.OnItemToggleClick(this,partsType,toggle.isOn);
	}
	//--------------------------------------------------------------
	public void OnPointerClick( PointerEventData eventData )
	{
		UIManager.Instance.BodyPartsListUI.OnItemClick(this);
	}
	//--------------------------------------------------------------
	public void Expand()
	{
		Children.ForEach( item => {
			item.gameObject.SetActive(true);
		});
		IsExpanded = true;
		if( toggleImage.sprite != null ){
			toggleImage.overrideSprite = toggleSpriteOpened;
		}
	}
	//--------------------------------------------------------------
	public void Fold()
	{
		Children.ForEach( item => {
			item.gameObject.SetActive(false);
			item.Fold();
		});
		IsExpanded = false;
		if( toggleImage.sprite != null ){
			toggleImage.overrideSprite = toggleSpriteClosed;
		}
	}
	//--------------------------------------------------------------
	public void UpdateToggle()
	{
		ToggleLeft.isOn =  Children.Exists( i => i.ToggleLeft.isOn );
		ToggleRight.isOn =  Children.Exists( i => i.ToggleRight.isOn );
		ToggleCenter.isOn =  Children.Exists( i => i.ToggleCenter.isOn );



		// 
		if( Parent != null ){
			Parent.UpdateToggle();
		}
	}
}
