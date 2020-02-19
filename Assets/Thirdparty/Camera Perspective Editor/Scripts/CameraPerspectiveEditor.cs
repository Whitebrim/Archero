using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraPerspectiveEditor : MonoBehaviour
{
	//Lens Shift (Far-Plane Offset)
	public Vector2 lensShift = Vector2.zero;
	public bool lensShiftProportionalToAspect = false;
	
	//Lens Tilt
	public Vector2 lensTilt = Vector2.zero;

	//Position Shift
	public Vector2 positionShift = Vector2.zero;
	
	//Skew
	public Vector2 skew = Vector2.zero;
	
	//Aspect Scale
	public Vector2 aspectScale = Vector2.one;

	//Clipping Plane Skew
	public Vector2 clippingPlaneSkew = Vector2.zero;

	//Dolly Zoom (Trombone)
	[Range (0f, 1f)]
	public float
		dollyZoom = 0f;
	public float dollyZoomFocalDistance = -1f;
	public Transform dollyZoomFocalTarget;

	//-----------------------

	private Camera thisCamera;
	private Matrix4x4 targetMatrix;
	private Vector2 baseAspectScale;
	private float dollyZoomDenominatorCache;
	private float invertedNormalizedDollyZoomFocalDistance;
	private Matrix4x4 pointToRayMatrix;
	private Vector3 pointConversionPosition;

	//-----------------------

	public void Awake ()
	{
		thisCamera = GetComponent(typeof(Camera)) as Camera;
		
		if (dollyZoomFocalDistance < 0f)
		{
			dollyZoomFocalDistance = (thisCamera.farClipPlane - thisCamera.nearClipPlane) * 0.5f;
		}
	}

	//-----------------------

	public void OnPreCull ()
	{
		//---
		//BEGIN Handle Camera Changes
		//---

		//Update if camera has changed
		if (CheckForCameraUpdate())
		{
			thisCamera.ResetProjectionMatrix();
			targetMatrix = thisCamera.projectionMatrix;

			baseAspectScale.x = targetMatrix.m00;
			baseAspectScale.y = targetMatrix.m11;
		}

		//---
		//END Handle Camera Changes
		//---

		//---
		//BEGIN Assign Matrix Customization
		//---
		
		//Lens Shift (Far-Plane Offset)
		if (thisCamera.orthographic)
		{
			targetMatrix.m02 = lensShift.x / thisCamera.orthographicSize / thisCamera.aspect;
			targetMatrix.m12 = lensShift.y / thisCamera.orthographicSize;
		}
		else
		{
			targetMatrix.m02 = lensShift.x * 2f;
			targetMatrix.m12 = lensShift.y * 2f;
			
			if (!lensShiftProportionalToAspect)
			{
				targetMatrix.m02 /= thisCamera.aspect;
			}
		}
		
		//Lens Tilt
		targetMatrix.m30 = lensTilt.x;
		targetMatrix.m31 = lensTilt.y;
		
		//Position Shift 
		targetMatrix.m03 = positionShift.x * 2f;
		targetMatrix.m13 = positionShift.y * 2f;
		
		//Skew
		targetMatrix.m01 = skew.x * 2f;
		targetMatrix.m10 = skew.y * 2f;
		
		//Aspect Scale
		targetMatrix.m00 = baseAspectScale.x * aspectScale.x;
		targetMatrix.m11 = baseAspectScale.y * aspectScale.y;
		
		//Clipping Plane Skew
		targetMatrix.m20 = clippingPlaneSkew.x * 2f;
		targetMatrix.m21 = clippingPlaneSkew.y * 2f;
		
		//Dolly Zoom (Trombone)
		if (!thisCamera.orthographic)
		{
			dollyZoom = Mathf.Clamp01(dollyZoom);
			if (dollyZoomFocalTarget)
			{
				dollyZoomFocalDistance = thisCamera.WorldToViewportPoint(dollyZoomFocalTarget.position).z;
			}
			dollyZoomFocalDistance = Mathf.Clamp(dollyZoomFocalDistance, currentNearClipPlane, currentFarClipPlane);
			targetMatrix.m32 = Mathf.Lerp(-1f, 0f, dollyZoom);
			targetMatrix.m33 = Mathf.Lerp(0f, dollyZoomFocalDistance, dollyZoom);
			dollyZoomDenominatorCache = (currentFarClipPlane - currentNearClipPlane);
			invertedNormalizedDollyZoomFocalDistance = 1f - ((dollyZoomFocalDistance - currentNearClipPlane) / dollyZoomDenominatorCache);
			targetMatrix.m22 = ((-(currentNearClipPlane + currentFarClipPlane) / dollyZoomDenominatorCache) - dollyZoom) + (2f * invertedNormalizedDollyZoomFocalDistance * dollyZoom);
			targetMatrix.m23 = (((-2f * currentNearClipPlane * currentFarClipPlane) / dollyZoomDenominatorCache) - (currentFarClipPlane * dollyZoom)) + ((currentNearClipPlane + currentFarClipPlane) * invertedNormalizedDollyZoomFocalDistance * dollyZoom);
		}
		
		//---
		//END Assign Matrix Customization
		//---
		
		//Apply Matrix Customization
		thisCamera.projectionMatrix = targetMatrix;
	}

	//-----------------------

	public Ray ScreenPointToRay (Vector3 position)
	{
		pointConversionPosition = position;

		pointConversionPosition.x = pointConversionPosition.x / thisCamera.pixelWidth;
		pointConversionPosition.y = pointConversionPosition.y / thisCamera.pixelHeight;

		return ViewportPointToRay(pointConversionPosition);
	}

	public Ray ViewportPointToRay (Vector3 position)
	{
		Ray newRay = new Ray();

		pointConversionPosition = position;

		pointToRayMatrix = targetMatrix;
		
		pointConversionPosition.x = (pointConversionPosition.x - 0.5f) * 2f;
		pointConversionPosition.y = (pointConversionPosition.y - 0.5f) * 2f;
		
		pointConversionPosition.z = -1f;

		//---
		
		pointToRayMatrix = thisCamera.cameraToWorldMatrix * pointToRayMatrix.inverse;

		newRay.origin = pointToRayMatrix.MultiplyPoint(pointConversionPosition);

		pointConversionPosition.z = 1f;
		newRay.direction = pointToRayMatrix.MultiplyPoint(pointConversionPosition) - newRay.origin;
		
		return newRay;
	}
	
	//-----------------------
	
	public Vector3 WorldToScreenPoint (Vector3 position)
	{
		pointConversionPosition = WorldToViewportPoint(position);

		pointConversionPosition.x = pointConversionPosition.x * thisCamera.pixelWidth;
		pointConversionPosition.y = pointConversionPosition.y * thisCamera.pixelHeight;
		
		return pointConversionPosition;
	}

	public Vector3 WorldToViewportPoint (Vector3 position)
	{
		pointConversionPosition = position;

		pointToRayMatrix = targetMatrix;

		//---
		
		pointToRayMatrix = thisCamera.cameraToWorldMatrix * pointToRayMatrix.inverse;

		pointConversionPosition = pointToRayMatrix.inverse.MultiplyPoint(pointConversionPosition);
		
		pointConversionPosition.x = (pointConversionPosition.x + 1f) * 0.5f;
		pointConversionPosition.y = (pointConversionPosition.y + 1f) * 0.5f;
		pointConversionPosition.z = 0f;
		
		return pointConversionPosition;
	}
	
	//-----------------------
	
	public void OnEnable ()
	{
		targetMatrix = thisCamera.projectionMatrix;

		baseAspectScale.x = targetMatrix.m00;
		baseAspectScale.y = targetMatrix.m11;
	}

	public void OnDisable ()
	{
		thisCamera.ResetProjectionMatrix();
	}

	//-----------------------
	//-----------------------
	//-----------------------
	//Handle Camera Changes

	private bool cameraHasChanged = false;
	
	//-----------------------
	
	bool CheckForCameraUpdate ()
	{
		cameraHasChanged = false;

		//Resolution Changes
		currentScreenHeight = thisCamera.pixelHeight;
		currentScreenWidth = thisCamera.pixelWidth;
		
		//Field of View Changes
		currentFOV = thisCamera.fieldOfView;
		
		//Orthographic Size Changes
		currentOrthographicSize = thisCamera.orthographicSize;
		
		//Handle Clipping Plane Changes
		currentNearClipPlane = thisCamera.nearClipPlane;
		currentFarClipPlane = thisCamera.farClipPlane;
		
		//Projection Mode Changes
		currentProjectionMode = thisCamera.orthographic;

		return cameraHasChanged;
	}

	//-----------------------
	//-----------------------
	//-----------------------
	//Handle Resolution Changes

	private float _currentScreenHeight = 0f;
	
	private float currentScreenHeight {
		set {
			if (value != _currentScreenHeight)
			{
				_currentScreenHeight = value;
				
				cameraHasChanged = true;
			}
		}
	}

	private float _currentScreenWidth = 0f;
	
	private float currentScreenWidth {
		set {
			if (value != _currentScreenWidth)
			{
				_currentScreenWidth = value;
				
				cameraHasChanged = true;
			}
		}
	}

	//-----------------------
	//-----------------------
	//-----------------------
	//Handle Field of View Changes

	private float _currentFOV;

	private float currentFOV {
		set {
			if (value != _currentFOV)
			{
				_currentFOV = value;

				cameraHasChanged = true;
			}
		}
	}
	
	//-----------------------
	//-----------------------
	//-----------------------
	//Handle Orthographic Size Changes
	
	private float _currentOrthographicSize;
	
	private float currentOrthographicSize {
		set {
			if (value != _currentOrthographicSize)
			{
				_currentOrthographicSize = value;

				cameraHasChanged = true;
			}
		}
	}

	//-----------------------
	//-----------------------
	//-----------------------
	//Handle Clipping Plane Changes
	
	private float _currentNearClipPlane;
	
	private float currentNearClipPlane {
		set {
			if (value != _currentNearClipPlane)
			{
				_currentNearClipPlane = value;
				
				cameraHasChanged = true;
			}
		}
		get {
			return _currentNearClipPlane;
		}
	}

	private float _currentFarClipPlane;
	
	private float currentFarClipPlane {
		set {
			if (value != _currentFarClipPlane)
			{
				_currentFarClipPlane = value;
				
				cameraHasChanged = true;
			}
		}
		get {
			return _currentFarClipPlane;
		}
	}

	//-----------------------
	//-----------------------
	//-----------------------
	//Handle Projection Mode Changes
	
	private bool _currentProjectionMode;
	
	private bool currentProjectionMode {
		set {
			if (value != _currentProjectionMode)
			{
				_currentProjectionMode = value;

				cameraHasChanged = true;
			}
		}
	}
}