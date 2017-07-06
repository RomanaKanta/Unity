using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Lean.Touch;

public class CameraController : MonoBehaviour
{
	// 
	float defaultCameraSize		= 0.25f;
	public float CameraZoom;
	public static float CAMERA_ZOOM_MIN = 0.02f;
	public static float CAMERA_ZOOM_MAX = 0.5f;
	static public float RotationSensitivity = 0.3f;
	static public float TransformSensitivity = 0.005f;
	private float mOldFingerDelta = 0;
	private const float mFingerDistanceEpsilon = 1.0f;

	// 
	public Vector3 CameraTransform;
	public Vector3 CameraRotation;
	public Vector3 TmpCameraTransform;
	public Vector3 TmpCameraRotation;
	Vector3 dragStartPos;
	Vector3 defaultCameraPos	= new Vector3(0,0,-10);	

	// 
	public RenderTexture renderTexture;
	public Texture2D capturedTexture;
	public const int TEXTURE_SIZE = 256;

	bool isActive = true;

	public static Color DEFAULT_BG_COLOR = new Color(0x26/255f,0x32/255f,0x38/255f);

	Camera _camera;

	//---------------------------------------------------------------
	public Camera Camera {
		get {
			if( _camera == null ){
				_camera = GetComponent<Camera>();
			}
			return _camera;
		}
	}

	//----------------------------------------------------------------
	void Awake()
	{
		renderTexture = new RenderTexture( TEXTURE_SIZE, TEXTURE_SIZE, 32 );
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.anisoLevel = 0;
		renderTexture.useMipMap = false;
		// 
		capturedTexture = new Texture2D( TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.RGB24, false );
		capturedTexture.wrapMode = TextureWrapMode.Clamp;
		capturedTexture.anisoLevel = 0;
	}

	//----------------------------------------------------------------
	void Start()
	{
		Reset();
	}

	//----------------------------------------------------------------

//	public GameObject mMoveObject;
//
//	public void SetObject(GameObject mObject){
//
//		this.mMoveObject = mObject;
//	}

	void LateUpdate(){

		if (Input.touchCount == 1 && isActive) {
			
			OnCameraRotation ();

		} else if (Input.touchCount == 2 && isActive) {

			UnityEngine.Vector2 deltaFingerVec = Input.touches [0].position - Input.touches [1].position;

			float deltaFingerValue =
				Mathf.Sqrt (deltaFingerVec.x * deltaFingerVec.x + deltaFingerVec.y * deltaFingerVec.y);

			// move
			bool moved = false;
			{
				if (Input.touches [0].deltaPosition.x < 0 && Input.touches [1].deltaPosition.x < 0) {
					moved = true;
				} else if (Input.touches [0].deltaPosition.x > 0 && Input.touches [1].deltaPosition.x > 0) {
					moved = true;
				}

				if (Input.touches [0].deltaPosition.y < 0 && Input.touches [1].deltaPosition.y < 0) {
					moved = true;
				} else if (Input.touches [0].deltaPosition.y > 0 && Input.touches [1].deltaPosition.y > 0) {
					moved = true;
				}
				OnCameraMove ();

			}

			// zoom
			if (!moved && Mathf.Abs (deltaFingerValue - mOldFingerDelta) > mFingerDistanceEpsilon) {
				
				OnCameraZoom ();

//				Vector3 dir = mMoveObject.transform.position - transform.position;
//				float dist = Mathf.Abs(dir.magnitude);
//
//				if (deltaFingerValue - mOldFingerDelta > 0)
//				{
//					if (dist > Minimum)
//					{
//						mMoveObject.transform.Translate(-dir * WheelSensitivity * Time.deltaTime * deltaFingerValue, Space.World);
//					}
//				}
//
//				if (deltaFingerValue - mOldFingerDelta < 0)
//				{
//					if (dist < Maximum)
//					{
//						mMoveObject.transform.Translate(dir * WheelSensitivity * Time.deltaTime * deltaFingerValue, Space.World);
//					}
//				}

			} 
				
			mOldFingerDelta = deltaFingerValue;
		} else {
		}
	}

	Vector2 rotfirstPressPos;
	Vector2 rotsecondPressPos;
	Vector3 rotcurrentSwipe;

	private void OnCameraRotation ()
	{
		if (Input.touchCount == 1 && Input.touchCount < 2) {

			Touch touch = Input.touches [0];

			if (touch.phase == TouchPhase.Began) {

				rotfirstPressPos = new Vector2 (touch.position.x, touch.position.y);
			}
			if (touch.phase == TouchPhase.Moved) {

				rotsecondPressPos = new Vector2(touch.position.x, touch.position.y);

				rotcurrentSwipe = new Vector3 (rotsecondPressPos.x - rotfirstPressPos.x, rotsecondPressPos.y - rotfirstPressPos.y);

				TmpCameraRotation = rotcurrentSwipe * RotationSensitivity;

				UpdateCamera();
			}

			if(touch.phase == TouchPhase.Ended){
				CameraRotation += TmpCameraRotation;
				TmpCameraRotation = Vector3.zero;
			}
		}				   
	}


	Vector2 firstPressPos;
	Vector2 secondPressPos;
	Vector3 currentSwipe;

	private void OnCameraMove ()
	{
		if (Input.touchCount == 2 ) {

			Touch touch = Input.touches [1];

			if (touch.phase == TouchPhase.Began) {

				firstPressPos = new Vector2 (touch.position.x, touch.position.y);
			}
			if (touch.phase == TouchPhase.Moved) {

				secondPressPos = new Vector2(touch.position.x, touch.position.y);

				currentSwipe = new Vector3 (secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

				float adj = (CameraZoom-CAMERA_ZOOM_MIN) / (CAMERA_ZOOM_MAX-CAMERA_ZOOM_MIN);		// 0.0～1.0

				float rate = (Mathf.Pow (10f, adj) / 4f) * 0.05f;

				TmpCameraTransform = currentSwipe * TransformSensitivity * rate;
				UpdateCamera ();
			}

			if(touch.phase == TouchPhase.Ended){
				CameraTransform += TmpCameraTransform;
				TmpCameraTransform = Vector3.zero;
			}
		}				   
	}

	[Tooltip("Ignore fingers with StartedOverGui?")]
	public bool IgnoreGuiFingers = true;

	[Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]

	public float WheelSensitivity = 1.9f;

	[Tooltip("The target FOV/Size")]
	public float Target = 10.0f;

	[Tooltip("The minimum FOV/Size we want to zoom to")]
	public float Minimum = 0.02f;

	[Tooltip("The maximum FOV/Size we want to zoom to")]
	public float Maximum = 0.5f;

	[Tooltip("How quickly the zoom reaches the target value")]
	public float Dampening = 10.0f;

//	Vector2 zoomfirstPressPos;
//	Vector2 zoomsecondPressPos;
//	Vector3 zoomcurrentSwipe;

	public void OnCameraZoom ()
	{
//		// Get the fingers we want to use
//		var fingers = LeanTouch.GetFingers(IgnoreGuiFingers, 2);
//
//		// Scale the current value based on the pinch ratio
//		Target *= LeanGesture.GetPinchRatio(fingers, WheelSensitivity);
//
//		// Clamp the current value to min/max values
//		Target = Mathf.Clamp(Target, Minimum, Maximum); 
//
//		// The framerate independent damping factor
//		var factor = 1.0f - Mathf.Exp(-Dampening * Time.deltaTime);
//
//		// Store the current size/fov in a temp variable
//		var current = CameraZoom;
//
//		current = Mathf.Lerp(current, Target, factor);

		// Get the fingers we want to use
		var fingers = LeanTouch.GetFingers(IgnoreGuiFingers, 2);

		// Store the current size/fov in a temp variable
		var current = CameraZoom;

		// Scale the current value based on the pinch ratio
		current *= LeanGesture.GetPinchRatio(fingers, WheelSensitivity);

		// Clamp the current value to min/max values
		current = Mathf.Clamp(current, Minimum, Maximum);


		CameraZoom = current;
		UpdateCamera();				   
	}

	public void Reset()
	{
		CameraTransform		= Vector3.zero;
		CameraRotation		= Vector3.zero;
		TmpCameraTransform	= Vector3.zero;
		TmpCameraRotation	= Vector3.zero;
		CameraZoom			= defaultCameraSize;
		this.Camera.backgroundColor = DEFAULT_BG_COLOR;
		UIManager.Instance.ColorDialog.PickedColor = DEFAULT_BG_COLOR;
		UpdateCamera();
	}

	public Texture2D CaptureScreen()
	{
		Camera.main.targetTexture = renderTexture;
		RenderTexture.active = Camera.main.targetTexture;
		Camera.main.Render();
		capturedTexture.ReadPixels(new Rect(0,0,capturedTexture.width,capturedTexture.height),0,0);
		capturedTexture.Apply();
		RenderTexture.active = null;
		Camera.main.targetTexture = null;

		return capturedTexture;
	}

	public void UpdateCamera()
	{
		Vector3 trn = CameraTransform + TmpCameraTransform;

		Vector3 rot = CameraRotation + TmpCameraRotation;
		rot.y = Mathf.Max(-89,Mathf.Min(rot.y,89));
		// 
		transform.position = Quaternion.Euler(0,rot.x,0) * Quaternion.Euler(-rot.y,0,0) * defaultCameraPos;
		transform.rotation = Quaternion.LookRotation(-transform.position);
		transform.Translate( -trn );
		this.Camera.orthographicSize = CameraZoom;

		UIManager.Instance.SelectionManager.UpdateSubBuffer();
	}

	public void SetCameraDefaultZoom(float CamZoom){
		defaultCameraSize	= CamZoom;
	}

	public void SetActionActive(bool active){
		isActive  = active;
	}
}