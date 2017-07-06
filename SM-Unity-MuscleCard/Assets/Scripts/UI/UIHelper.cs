using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHelper : MonoBehaviour
{
	//------------------------------------------------------------------------------
	public bool Visible {
		set {
			CanvasGroup cg = GetComponent<CanvasGroup>();
			cg.blocksRaycasts = cg.interactable = value;
			cg.alpha = value ? 1 : 0;
		}
		get {
			CanvasGroup cg = GetComponent<CanvasGroup>();
			return cg.blocksRaycasts;
		}
	}
	//------------------------------------------------------------------------------
	void Awake()
	{
		if( GetComponent<CanvasGroup>() == null ){
			gameObject.AddComponent<CanvasGroup>();
		}
	}
	//------------------------------------------------------------------------------
	public void ToggleVisible()
	{
		Visible = !Visible;
	}
	//------------------------------------------------------------------------------
	public void FadeIn()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		StartCoroutine( FadeAnimation(cg,0,1,0.1f) );
	}
	//------------------------------------------------------------------------------
	public void FadeOut()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		StartCoroutine( FadeAnimation(cg,1,0,0.1f) );
	}
	//------------------------------------------------------------------------------
	private IEnumerator FadeAnimation( CanvasGroup cg, float a0, float a1, float duration )
	{
		float t0 = Time.time;
		// 
		cg.alpha = a0;
		if( a1 != 0 ){
			cg.blocksRaycasts = cg.interactable = true;
		}
		// 
		while( Time.time <= t0+duration ){
			float t = (Time.time-t0) / duration;
			cg.alpha = Mathf.Lerp(a0,a1,t);
			yield return new WaitForSeconds(0.02f);
		}
		// 
		cg.alpha = a1;
		if( a1 == 0 ){
			cg.blocksRaycasts = cg.interactable = false;
		}
		// 
		yield break;
	}
}
