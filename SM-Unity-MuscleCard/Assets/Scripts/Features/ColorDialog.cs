//==============================================================================
// 
// ColorDialog.cs
// 
// 背景色選択ダイアログクラス
// 
//==============================================================================
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class ColorDialog : MonoBehaviour
{
	public GameObject Containter;
	public GameObject ColorPreview;
	Color _pickedColor;
	public Color PickedColor {
		get {
			return _pickedColor;
		}
		set {
			_pickedColor = value;
			ColorPreview.GetComponent<Image>().color = _pickedColor;
			UIManager.Instance.SelectionManager.mainCamera.backgroundColor = PickedColor;
		}
	}
	//------------------------------------------------------------------------------
	// 
	//------------------------------------------------------------------------------
	void Awake()
	{
		// ScrollContentの子要素をすべて削除
		foreach( Transform t in Containter.transform ){
			Destroy(t.gameObject);
		}

		for(int i=0; i<MuscleData.colorList.Length; i++){
			GameObject itemObj = (GameObject)GameObject.Instantiate(Resources.Load("UI/ColorChip"),Vector3.zero,Quaternion.identity);

			itemObj.GetComponent<Image>().color = HexToColor(MuscleData.colorList[i]);
			// Parent
			((RectTransform)itemObj.transform).SetParent(Containter.transform);
			itemObj.name = (14*i).ToString();
			// Scaleがゼロクリアされる問題を修正
			itemObj.transform.localScale = Vector3.one;
			// OnClick
			Button btn = itemObj.GetComponent<Button>();
			btn.onClick.AddListener( () => {
				PickedColor = btn.GetComponent<Image>().color;
			});

		}


		// 部位辞書
//		for( int i=0; i<6; i++ ){
//			for( int j=0; j<16; j++ ){
//				GameObject itemObj = (GameObject)GameObject.Instantiate(Resources.Load("UI/ColorChip"),Vector3.zero,Quaternion.identity);
//				// 
//				Color c;
//				// 
//				if( i == 0 ){
//					// 1列目は標準16色
//					c = new Color[]{
//						Color.black, Color.gray, new Color(0.75f,0.75f,0.75f), Color.white,
//						Color.red, Color.magenta, Color.yellow, Color.green, Color.blue,  Color.cyan,
//						new Color(0.5f,0f,0f),
//						new Color(0.5f,0f,0.5f), 
//						new Color(0.5f,0.5f,0f), 
//						new Color(0f,0.5f,0f),
//						new Color(0f,0f,0.5f),
//						new Color(0f,0.5f,0.5f),
//					}[j];
//				}else{
//					c =  hsv2rgv(j/16f,1f,1f-(5-i)/6f);
//				}
//				itemObj.GetComponent<Image>().color = c;
//				// Parent
//				((RectTransform)itemObj.transform).SetParent(Containter.transform);
//				itemObj.name = (14*i+j).ToString();
//				// Scaleがゼロクリアされる問題を修正
//				itemObj.transform.localScale = Vector3.one;
//				// OnClick
//				Button btn = itemObj.GetComponent<Button>();
//				btn.onClick.AddListener( () => {
//					PickedColor = btn.GetComponent<Image>().color;
//				});
//			}
//		}
		// 
//		PickedColor = Camera.main.backgroundColor;
		PickedColor =  UIManager.Instance.SelectionManager.mainCamera.backgroundColor;
	}
	//------------------------------------------------------------------------------
	// 
	//------------------------------------------------------------------------------
	public void SetColorToCamera()
	{

		UIManager.Instance.SelectionManager.mainCamera.backgroundColor = PickedColor;

//		Camera.main.backgroundColor = PickedColor;
	}
	//------------------------------------------------------------------------------
	// 
	//------------------------------------------------------------------------------
	Color hsv2rgv( float h, float s, float v )
	{
		h = Mathf.Clamp01(h);
		s = Mathf.Clamp01(s);
		v = Mathf.Clamp01(v);
		// 
		float h_ = h*6f-Mathf.Floor(h*6f);
		float m = v * (1f-s);
		float n = v * (1f-s*h_);
		if( ( Mathf.FloorToInt(h*6) & 1 ) == 0 ){
			n = v * (1f-s*(1f-h_));
		}
		// 
		switch(Mathf.FloorToInt(h*6)){
			case 0: return new Color(v,n,m);
			case 1: return new Color(n,v,m);
			case 2: return new Color(m,v,n);
			case 3: return new Color(m,n,v);
			case 4: return new Color(n,m,v);
			case 5: return new Color(v,m,n);
			default: return Color.black;
		}
	}
	//------------------------------------------------------------------------------
	public static string ColorToHex( Color c )
	{
		return ((int)(c.r*255f)).ToString("X2") + ((int)(c.g*255f)).ToString("X2") + ((int)(c.b*255f)).ToString("X2");
	}
 	//------------------------------------------------------------------------------
	public static Color HexToColor( string hex )
	{
		if( hex == null ) return Color.black;
		hex = hex.ToLower();
		if( hex.Length < 6 ){
			hex = hex + "000000";
		}
		try{
			byte r = byte.Parse( hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber );
			byte g = byte.Parse( hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber );
			byte b = byte.Parse( hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber );
			return new Color(r/255f,g/255f,b/255f);
		}catch( Exception ){
			return Color.black;
		}
	}

}
