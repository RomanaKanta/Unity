using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BookmarkListItem : MonoBehaviour, IPointerClickHandler
{
	public Text TextLabel;
	public Image PreviewImage;
	static readonly int TEXTURE_SIZE = 256;
	// 
	public bool _selected;
	public bool Selected
	{
		get { return _selected; }
		set {
			_selected = value;
			Image img = GetComponent<Image>();
			img.color = _selected ? new Color(0.8f,0.8f,0.8f,1f) : Color.white;
		}
	}
	public BookmarkData Data;
	public string DataPath;
	public bool IsInvalid = false;
	private int clickCount = 0;
	//------------------------------------------------------------------------------
	public void OnPointerClick( PointerEventData eventData )
	{
//		BookmarkDialog ui = GetComponentInParent<BookmarkDialog>();
//		ui.SelectedItem = this;

		if (UIManager.Instance.BookmarkDialog.SelectedItem != this) {
			clickCount = 0;
		}

		UIManager.Instance.BookmarkDialog.SelectedItem = this;

		Selected = true;
		// DoubleClickの時はロード開始
		clickCount++;
		if( clickCount >= 2 ){
//			ui.Load();
			UIManager.Instance.BookmarkDialog.Load();
			clickCount = 0;
		}
	}
		
	//------------------------------------------------------------------------------
	public void SetData( BookmarkData data ){
		
		PreviewImage.sprite = Sprite.Create(data.GetImage(),new Rect(0,0,TEXTURE_SIZE,TEXTURE_SIZE),Vector2.zero);
		TextLabel.text = data.DataName;
		Data = data;
	}
}
