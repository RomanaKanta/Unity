//==============================================================================
// 
// BodyPartsDataManager.cs
// 
// 部位データ管理クラス
// 
//==============================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

//------------------------------------------------------------------------------
public class BodyPartsDataManager
{
	//------------------------------------------------------------------------------
	static BodyPartsDataManager instance = new BodyPartsDataManager();
	static public BodyPartsDataManager Instance { get { return instance; } }
	// 
	BodyPartsDataManager()
	{
		bodyPartsDict = readTSV("new_model_list");
//		bodyPartsDict = readTSV("all_data_file");
	}
	//------------------------------------------------------------------------------
	public Dictionary<int,string> MeshNameDict;
	public string GetMeshName( int colorIndex )
	{
		return ( MeshNameDict==null || !MeshNameDict.ContainsKey(colorIndex) ) ? null : MeshNameDict[colorIndex];
	}
	// 
	Dictionary<string,BodyParts> bodyPartsDict;
	public Dictionary<string,BodyParts> BodyPartsDict {
		get { return bodyPartsDict; }
	}
	//------------------------------------------------------------------------------
	public BodyParts GetBodyParts( string kid )
	{
		return ( kid!=null && bodyPartsDict.ContainsKey(kid) ) ? bodyPartsDict[kid] : null;
	}
	//------------------------------------------------------------------------------
	static public string MeshNameToKinID( string meshName )
	{
		if( meshName == null ){
			return null;
		}
		MatchCollection mc = Regex.Matches(meshName,"K[0-9]{5}");
		return (mc.Count>0) ? mc[0].Value : null;
	}
	//------------------------------------------------------------------------------
	// L_K00000形式の名称からMeshRendererを取得
	//------------------------------------------------------------------------------
	public MeshRenderer GetMesh( string meshName, bool isSub )
	{
		BodyParts bp = GetBodyParts(MeshNameToKinID(meshName));
		if( bp == null ){
			return null;
		}
		if( bp.Mesh != null ){
			return isSub ? bp.SubMesh : bp.Mesh;
		}else{
			if( meshName.Contains("L_") ){
				return isSub ? bp.SubLeftMesh : bp.LeftMesh;
			}else{
				return isSub ? bp.SubRightMesh : bp.RightMesh;
			}
		}
	}
	//------------------------------------------------------------------------------
	// TSVファイルから、Dic<KinID,BodyPart>
	//------------------------------------------------------------------------------
	Dictionary<string,BodyParts> readTSV( string path )
	{
		List<BodyParts>	partList	= new List<BodyParts>();
		List<string>	keyList		= new List<string>();
		// 
		TextAsset ta = (TextAsset)Resources.Load(path);
		{
			// データ読み込み
			string[] lines = ta.text.Split('\n');
			for( int i=1; i<lines.Length; i++ ){
				string line = lines[i];
				string[] s = line.Split( new char[]{'\t'} );
				// 不正データ
				if( s.Length < 12 || s[0].Length <= 0 ){
//					Debug.LogWarning("### readTSV Error Line : len="+s.Length+" "+line);
					continue;
				}
				// 
				BodyParts part = new BodyParts();
				part.KinID			= s[0];
				// 階層によって部位名称が格納されているセルが異なる
				for( int j=0; j<4; j++ ){
					if( s[1+j].Length > 0 ){
						part.PartsName	= s[1+j];
						part.LayerIndex	= j;
						break;
					}
				}
				part.Pronunciation	= s[5];
				part.EnglishName	= s[6];
				part.EnglishPronunciation = s[7];
				part.Origin			= s[8];
				part.Insertion		= s[9];
				part.Nerve			= s[10];
				part.Actions		= s[11];
				if( s.Length >= 15 ){
					part.NerveImage		= s[13];
					part.ActionsMovie	= s[14];
				}
				// 
				partList.Add(part);
				keyList.Add(part.KinID);
			}
		}
			
		// 階層情報を付加
		BodyParts[] parts = partList.ToArray();
		for( int i=0; i<parts.Length-1; i++ ){
			List<string> list = new List<string>();
			for( int j=i+1; j<parts.Length; j++ ){
				// 自身と同一階層以上のものが検出されたら走査終了
				if( parts[j].LayerIndex <= parts[i].LayerIndex ){
					break;
				}
				// 直下の階層
				if( parts[j].LayerIndex == parts[i].LayerIndex+1 ){
					parts[j].ParentID = parts[i].KinID;
					list.Add( parts[j].KinID );
				}
			}
			parts[i].ChildIDs = list;
		}
		// 
		Dictionary<string,BodyParts> bodyPartsDict = new Dictionary<string,BodyParts>();
		foreach( BodyParts bp in parts ){
			bodyPartsDict[bp.KinID] = bp;
		}
		return bodyPartsDict;
	}
}
