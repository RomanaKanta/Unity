//==============================================================================
// 
// BodyParts.cs
// 
// 部位クラス 
// 
//==============================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BodyParts
{
	// 部位種別
	public enum PartsTypeEnum {
		Left,
		Right,
		Center
	};
	// 
	public string	KinID;				// KIN ID
	public string	PartsName;			// 部位名称
	public int		LayerIndex;			// 階層番号
	public string	Pronunciation;		// よみ
	public string	EnglishName;		// 英語名称
	public string	EnglishPronunciation;	// 英語よみ
	public string	Origin;				// 起始
	public string	Insertion;			// 停止
	public string	Nerve;				// 支配神経
	public string	Actions;			// 作用
	public string	NerveImage;			// 支配神経画像
	public string	ActionsMovie;		// 作用動画
	// 
	public PartsTypeEnum PartsType;
	// 
	public string	ParentID;			// 親ID
	public List<string>	ChildIDs;		// 子ID
	//------------------------------------------------------------------------------
	// GameObject, MeshRenderer
	//------------------------------------------------------------------------------
	GameObject _gameObject;
	public GameObject GameObject {
		get{ return _gameObject; }
		set{
			_gameObject = value;
			MeshList = new List<MeshRenderer>( _gameObject.GetComponentsInChildren<MeshRenderer>() );
		}
	}
	//------------------------------------------------------------------------------
	public List<MeshRenderer>	MeshList = new List<MeshRenderer>();
	public MeshRenderer Mesh	{		get{ return (MeshList.Count==1) ? MeshList[0] : null; } }
	public MeshRenderer LeftMesh{		get{ return (MeshList.Count==2) ? MeshList[0] : null; } }
	public MeshRenderer RightMesh{		get{ return (MeshList.Count==2) ? MeshList[1] : null; } }
	//------------------------------------------------------------------------------
	GameObject subGameObject;
	public GameObject SubGameObject {
		get{ return subGameObject; }
		set{
			subGameObject = value;
			SubMeshList = new List<MeshRenderer>( subGameObject.GetComponentsInChildren<MeshRenderer>() );
		}
	}
	//------------------------------------------------------------------------------
	public List<MeshRenderer>	SubMeshList = new List<MeshRenderer>();
	public MeshRenderer SubMesh{		get{ return (SubMeshList.Count==1) ? SubMeshList[0] : null; } }
	public MeshRenderer SubLeftMesh{	get{ return (SubMeshList.Count==2) ? SubMeshList[0] : null; } }
	public MeshRenderer SubRightMesh{	get{ return (SubMeshList.Count==2) ? SubMeshList[1] : null; } }
	//------------------------------------------------------------------------------
	// MeshRendererを取得
	//------------------------------------------------------------------------------
	public MeshRenderer GetMeshRenderer( PartsTypeEnum partsType, bool isSubMesh )
	{
		if( partsType == PartsTypeEnum.Center ){
			return isSubMesh ? SubMesh : Mesh;
		}else if( partsType == PartsTypeEnum.Left ){
			return isSubMesh ? SubLeftMesh : LeftMesh;
		}else{
			return isSubMesh ? SubRightMesh : RightMesh;
		}
	}
}
