using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraPerspectiveEditor))]
[CanEditMultipleObjects]
public class CameraPerspectiveEditorEditor : Editor
{
	private CameraPerspectiveEditor cameraOffsetController;
	private Camera thisCamera;
	private Matrix4x4 viewMatrix;
	private float orthographicSize;
	private Vector3 bl;
	private Vector3 tl;
	private Vector3 br;
	private Vector3 tr;
	private Vector3 bl1;
	private Vector3 tl1;
	private Vector3 br1;
	private Vector3 tr1;
	private Vector3 bl2;
	private Vector3 tl2;
	private Vector3 br2;
	private Vector3 tr2;

	//-----------------------

	public SerializedProperty lensShift;
	public SerializedProperty lensShiftProportionalToAspect;
	public SerializedProperty lensTilt;
	public SerializedProperty positionShift;
	public SerializedProperty skew;
	public SerializedProperty aspectScale;
	public SerializedProperty clippingPlaneSkew;
	public SerializedProperty dollyZoom;
	public SerializedProperty dollyZoomFocalDistance;
	public SerializedProperty dollyZoomFocalTarget;
	private Vector2 utilityVector2;
	
	//-----------------------
	
	private static bool showLensShift = true;
	private static bool showLensTilt = false;
	private static bool showPositionShift = false;
	private static bool showSkew = false;
	private static bool showAspectScale = false;
	private static bool showClippingPlaneSkew = false;
	private static bool showDollyZoom = false;
	
	//-----------------------
	
	void OnEnable ()
	{
		lensShift = serializedObject.FindProperty("lensShift");
		lensShiftProportionalToAspect = serializedObject.FindProperty("lensShiftProportionalToAspect");
		lensTilt = serializedObject.FindProperty("lensTilt");
		positionShift = serializedObject.FindProperty("positionShift");
		skew = serializedObject.FindProperty("skew");
		aspectScale = serializedObject.FindProperty("aspectScale");
		clippingPlaneSkew = serializedObject.FindProperty("clippingPlaneSkew");
		dollyZoom = serializedObject.FindProperty("dollyZoom");
		dollyZoomFocalDistance = serializedObject.FindProperty("dollyZoomFocalDistance");
		dollyZoomFocalTarget = serializedObject.FindProperty("dollyZoomFocalTarget");

		cameraOffsetController = target as CameraPerspectiveEditor;
		thisCamera = cameraOffsetController.GetComponent<Camera>();
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		GUILayout.BeginVertical();
		{

			if (showLensShift = EditorGUILayout.Foldout(showLensShift, "Lens Shift"))
			{
				EditorGUI.indentLevel++;
				
				EditorGUILayout.HelpBox("Similar to \"Shift\" on a Tilt-Shift Camera lens.  Commonly used for oblique perspectives/projections, including 2.5D effects.", MessageType.Info);
				
				utilityVector2.x = EditorGUILayout.FloatField("Horizontal", lensShift.vector2Value.x);
				utilityVector2.y = EditorGUILayout.FloatField("Vertical", lensShift.vector2Value.y);
				lensShift.vector2Value = utilityVector2;
				
				GUI.enabled = !thisCamera.orthographic;
				//Perspective Mode
				if (!thisCamera.orthographic)
				{
					lensShiftProportionalToAspect.boolValue = EditorGUILayout.Toggle("Proportional To Aspect", lensShiftProportionalToAspect.boolValue);
				}
				//Orthographic Mode
				else
				{
					EditorGUILayout.Toggle("Proportional To Aspect", false);
					
					EditorGUILayout.HelpBox("Never Proportional To Aspect While Orthographic", MessageType.Warning);
				}
				GUI.enabled = true;
				
				GUILayout.Space(15);
				
				EditorGUI.indentLevel--;
			}

			if (showLensTilt = EditorGUILayout.Foldout(showLensTilt, "Lens Tilt"))
			{
				EditorGUI.indentLevel++;
				
				EditorGUILayout.HelpBox("Similar to \"Tilt\" on a Tilt-Shift Camera lens.", MessageType.Info);
				utilityVector2.x = EditorGUILayout.FloatField("Horizontal", lensTilt.vector2Value.x);
				utilityVector2.y = EditorGUILayout.FloatField("Vertical", lensTilt.vector2Value.y);
				lensTilt.vector2Value = utilityVector2;
				GUILayout.Space(15);
				
				EditorGUI.indentLevel--;
			}

			if (showPositionShift = EditorGUILayout.Foldout(showPositionShift, "Position Shift"))
			{
				EditorGUI.indentLevel++;
				
				EditorGUILayout.HelpBox("Similar to Truck(horizontal) and Pedestal(vertical) Camera motion, but without actually moving the Transform.", MessageType.Info);
				utilityVector2.x = EditorGUILayout.FloatField("Horizontal", positionShift.vector2Value.x);
				utilityVector2.y = EditorGUILayout.FloatField("Vertical", positionShift.vector2Value.y);
				positionShift.vector2Value = utilityVector2;

				if (positionShift.vector2Value.x != 0 || positionShift.vector2Value.y != 0)
				{
					EditorGUILayout.HelpBox("Unity's Camera gizmo does not properly draw changes to Position Shift; refer to the yellow Scene-view lines for an accurate representation.", MessageType.Warning);
				}

				GUILayout.Space(15);
				
				EditorGUI.indentLevel--;
			}

			if (showSkew = EditorGUILayout.Foldout(showSkew, "Skew"))
			{
				EditorGUI.indentLevel++;
				
				utilityVector2.x = EditorGUILayout.FloatField("Horizontal", skew.vector2Value.x);
				utilityVector2.y = EditorGUILayout.FloatField("Vertical", skew.vector2Value.y);
				skew.vector2Value = utilityVector2;
				GUILayout.Space(15);
				
				EditorGUI.indentLevel--;
			}

			if (showAspectScale = EditorGUILayout.Foldout(showAspectScale, "Aspect Scale"))
			{
				EditorGUI.indentLevel++;
				
				EditorGUILayout.HelpBox("Adjust per-axis foreshortening - like zooming only on the vertical or horizontal.", MessageType.Info);
				utilityVector2.x = EditorGUILayout.FloatField("Horizontal", aspectScale.vector2Value.x);
				utilityVector2.y = EditorGUILayout.FloatField("Vertical", aspectScale.vector2Value.y);
				aspectScale.vector2Value = utilityVector2;
				GUILayout.Space(15);
				
				EditorGUI.indentLevel--;
			}
			
			if (showClippingPlaneSkew = EditorGUILayout.Foldout(showClippingPlaneSkew, "Clipping Plane Skew"))
			{
				EditorGUI.indentLevel++;
				
				EditorGUILayout.HelpBox("Skews the side-planes of the view frustum in the depth(z) direction.", MessageType.Info);
				utilityVector2.x = EditorGUILayout.FloatField("Horizontal", clippingPlaneSkew.vector2Value.x);
				utilityVector2.y = EditorGUILayout.FloatField("Vertical", clippingPlaneSkew.vector2Value.y);
				clippingPlaneSkew.vector2Value = utilityVector2;
				GUILayout.Space(15);
				
				EditorGUI.indentLevel--;
			}
			
			EditorGUI.BeginDisabledGroup(thisCamera.orthographic);
			if (showDollyZoom = EditorGUILayout.Foldout(showDollyZoom, "Dolly Zoom"))
			{
				EditorGUI.indentLevel++;
				
				if (thisCamera.orthographic)
				{
					EditorGUILayout.HelpBox("Not Available With Orthographic Cameras", MessageType.Warning);
				}
				EditorGUILayout.HelpBox("Simulates a \"Dolly Zoom\", or \"Trombone\" effect without actually moving or zooming the camera.\n\"Focal Distance\" is the distance at which objects will maintain their screen-relative size.\nIf \"Focal Target\" is set, it will automatically define \"Focal Distance\" based on the target's distance.", MessageType.Info);
				EditorGUILayout.Slider(dollyZoom, 0f, 1f, "Dolly Zoom");
				EditorGUI.BeginDisabledGroup(dollyZoomFocalTarget.objectReferenceValue);
				dollyZoomFocalDistance.floatValue = EditorGUILayout.FloatField("Focal Distance", dollyZoomFocalDistance.floatValue);
				EditorGUI.EndDisabledGroup();
				dollyZoomFocalTarget.objectReferenceValue = EditorGUILayout.ObjectField("Focal Target", dollyZoomFocalTarget.objectReferenceValue, typeof(Transform), true);

				if (dollyZoom.floatValue != 0)
				{
					EditorGUILayout.HelpBox("Unity's Camera gizmo does not properly draw changes to Dolly Zoom; refer to the yellow Scene-view lines for an accurate representation.", MessageType.Warning);
				}
				
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndDisabledGroup();

		}
		GUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI ()
	{
		if (thisCamera.enabled && cameraOffsetController.enabled)
		{
			viewMatrix = thisCamera.projectionMatrix;
			thisCamera.ResetProjectionMatrix();
			Matrix4x4 originalMatrix = thisCamera.projectionMatrix;

			if (NativeFrustumIsInaccurate())
			{
				DrawFrustum(viewMatrix, new Color(1f, 1f, 0f, 0.65f));
			}

			DrawFrustum(originalMatrix, new Color(1f, 0f, 0f, 0.65f));

			thisCamera.projectionMatrix = viewMatrix;
		}
	}

	private void DrawFrustum(Matrix4x4 projectionMatrix, Color color)
	{
		if (thisCamera.orthographic)
		{
			projectionMatrix.m00 = 1f / thisCamera.aspect;
			projectionMatrix.m11 = 1f;
			
			//Begin - Fix for representing orthographic zoom correctly
			projectionMatrix.m02 = lensShift.vector2Value.x / thisCamera.aspect;
			projectionMatrix.m12 = lensShift.vector2Value.y;
			//End - Fix for representing orthographic zoom correctly
			
			orthographicSize = thisCamera.orthographicSize;
			
			tr.x = tr.y = br.x = tl.y = orthographicSize;
			bl.x = bl.y = tl.x = br.y = -orthographicSize;
		}
		else
		{
			tr.x = tr.y = br.x = tl.y = 1f;
			bl.x = bl.y = tl.x = br.y = -1f;
		}
		bl.z = tl.z = br.z = tr.z = 1f;
		
		projectionMatrix = thisCamera.cameraToWorldMatrix * projectionMatrix.inverse;
		
		bl1 = projectionMatrix.MultiplyPoint(-tr);
		tl1 = projectionMatrix.MultiplyPoint(-br);
		br1 = projectionMatrix.MultiplyPoint(-tl);
		tr1 = projectionMatrix.MultiplyPoint(-bl);
		bl2 = projectionMatrix.MultiplyPoint(bl);
		tl2 = projectionMatrix.MultiplyPoint(tl);
		br2 = projectionMatrix.MultiplyPoint(br);
		tr2 = projectionMatrix.MultiplyPoint(tr);
		
		Handles.color = color;

		Handles.DrawPolyLine(new Vector3[] {
			bl1,
			tl1,
			tr1,
			br1,
			bl1,
			bl2,
			tl2,
			tr2,
			br2,
			bl2
		});
		Handles.DrawLine(tl1, tl2);
		Handles.DrawLine(tr1, tr2);
		Handles.DrawLine(br1, br2);
	}

	private bool NativeFrustumIsInaccurate()
	{
		return positionShift.vector2Value.x != 0 || positionShift.vector2Value.y != 0 || dollyZoom.floatValue != 0;
	}
}