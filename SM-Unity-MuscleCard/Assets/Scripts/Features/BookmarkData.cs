using UnityEngine;
using System;
using System.Collections.Generic;
using MiniJSON;

[Serializable]
public class BookmarkData
{
	public string DataName;
	public string ObjectName;
	public string ObjectId;
	public string[] HiddenParts;
	public Vector3 CameraTransform;
	public Vector3 CameraRotation;
	public float CameraZoom;
	public byte[] ImageData;
	public Color BackgroundColor;
	public string SelectedMeshName;
	//------------------------------------------------------------------------------
	public BookmarkData()
	{

	}
	//------------------------------------------------------------------------------
	public BookmarkData( string jsonText )
	{
		Dictionary<string,object> dict = (Dictionary<string,object>)Json.Deserialize(jsonText);
		// 
		this.DataName		= (string)dict["DataName"];
		this.ObjectName		= (string)dict["ObjectName"];
		this.ObjectId		= (string)dict["ObjectId"];
		// 
		var val = dict["HiddenParts"];
		List<String> partsList = new List<string>();
		foreach( object o in (List<object>)val ){
			partsList.Add(o.ToString());
		}
		this.HiddenParts	= partsList.ToArray();
		// 
		Func<object,Vector3> cnv = v => {
			List<object> list = (List<object>)v;
			return new Vector3(float.Parse(list[0].ToString()),float.Parse(list[1].ToString()),float.Parse(list[2].ToString()));
		};
		this.CameraTransform	= cnv( dict["CameraTransform"] );
		this.CameraRotation		= cnv( dict["CameraRotation"] );
		this.CameraZoom			= float.Parse(dict["CameraZoom"].ToString());
		this.ImageData			= Convert.FromBase64String((String)dict["ImageData"]);
		this.BackgroundColor	= ColorDialog.HexToColor(dict["BackgroundColor"].ToString());
		if( dict.ContainsKey("SelectedMeshName") && dict["SelectedMeshName"] != null ){
			this.SelectedMeshName	= dict["SelectedMeshName"].ToString();
		}
	}
	//------------------------------------------------------------------------------
	public String ToJSON()
	{
		Func<Vector3,object[]> cnv = v => {
			return new object[]{ v.x.ToString(), v.y.ToString(), v.z.ToString() };
		};
		Dictionary<string,object> dict = new Dictionary<string,object>();
		// 
		dict["DataName"]	     =  this.DataName;
		dict["ObjectName"]       =  this.ObjectName	;
		dict["ObjectId"]         =  this.ObjectId;
		dict["HiddenParts"] 	 =  this.HiddenParts;
		dict["CameraTransform"]	 =  cnv(this.CameraTransform);
		dict["CameraRotation"]	 =  cnv(this.CameraRotation);
		dict["CameraZoom"]  	 =  this.CameraZoom.ToString();
		dict["ImageData"]	     =  Convert.ToBase64String(this.ImageData);
		dict["BackgroundColor"]	 =  ColorDialog.ColorToHex(this.BackgroundColor);
		dict["SelectedMeshName"] =  this.SelectedMeshName;
		//
		return Json.Serialize(dict);
	}
	//------------------------------------------------------------------------------
	public Texture2D GetImage()
	{
		Texture2D tex = new Texture2D(120,120);
		tex.LoadImage(ImageData);
		return tex;
	}
}
