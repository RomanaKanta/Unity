using UnityEngine;
using UnityEditor;
using System.Collections;

public class KinNaviUtil : MonoBehaviour
{
	//------------------------------------------------------------------------------
	[MenuItem("KinNavi/Add MeshCollider")]
	static void addMeshCollider()
	{
		foreach( GameObject obj in Selection.gameObjects ){
			foreach( MeshFilter mf in obj.GetComponentsInChildren<MeshFilter>() ){
				if( mf.gameObject.GetComponent<MeshCollider>() == null ){
					mf.gameObject.AddComponent<MeshCollider>();
					Debug.Log(mf.gameObject.name);
				}
			}
		}
	}
	//------------------------------------------------------------------------------
	[MenuItem("KinNavi/Remove MeshCollider")]
	static void removeMeshCollider()
	{
		foreach( GameObject obj in Selection.gameObjects ){
			foreach( MeshCollider mc in obj.GetComponentsInChildren<MeshCollider>() ){
				DestroyImmediate(mc);
			}
		}
	}
	//------------------------------------------------------------------------------
	[MenuItem("KinNavi/Random Material")]
	static void randomMaterial()
	{
		foreach( GameObject obj in Selection.gameObjects ){
			foreach( Renderer ren in obj.GetComponentsInChildren<Renderer>() ){
				ren.materials = new Material[]{ (Material)Instantiate(Resources.Load<Material>("test")) };
				ren.material.color = new Color(Random.value,Random.value,Random.value);
			}
		}
	}
}
