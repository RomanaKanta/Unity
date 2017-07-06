using UnityEngine;
using System.Collections;

public class PinchZoom : MonoBehaviour//NOTE: This script has been updated to V2 after video recording
{


	private float speed = 2.0f;
	private float zoomSpeed = 2.0f;

	public float minX = -360.0f;
	public float maxX = 360.0f;

	public float minY = -45.0f;
	public float maxY = 45.0f;

	public float sensX = 100.0f;
	public float sensY = 100.0f;

	float rotationY = 0.0f;
	float rotationX = 0.0f;

	void Update () {

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		transform.Translate(0, scroll * zoomSpeed, scroll * zoomSpeed, Space.World);

		if (Input.GetKey(KeyCode.RightArrow)){
			transform.position += Vector3.right * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
			transform.position += Vector3.left * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.UpArrow)){
			transform.position += Vector3.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow)){
			transform.position += Vector3.back * speed * Time.deltaTime;
		}

		if (Input.GetMouseButton (0)) {
			rotationX += Input.GetAxis ("Mouse X") * sensX * Time.deltaTime;
			rotationY += Input.GetAxis ("Mouse Y") * sensY * Time.deltaTime;
			rotationY = Mathf.Clamp (rotationY, minY, maxY);
			transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
		}
	}
//	public float perspectiveZoomSpeed = 2.5f;        // The rate of change of the field of view in perspective mode.
//	public float orthoZoomSpeed = 9.5f;        // The rate of change of the orthographic size in orthographic mode.
//	Camera camera;
//	void Start ()
//	{
//		camera =  GetComponent<Camera>();
//
//	}
//	void Update()
//	{
//		// If there are two touches on the device...
//		if (Input.touchCount == 2)
//		{
//			// Store both touches.
//			Touch touchZero = Input.GetTouch(0);
//			Touch touchOne = Input.GetTouch(1);
//
//			// Find the position in the previous frame of each touch.
//			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
//			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
//
//			// Find the magnitude of the vector (the distance) between the touches in each frame.
//			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
//			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
//
//			// Find the difference in the distances between each frame.
//			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
//
//
//				// Otherwise change the field of view based on the change in distance between the touches.
//				camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
//
//				// Clamp the field of view to make sure it's between 0 and 180.
//				camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 10.1f, 100.9f);
//
//		}
//	}
}
