using UnityEditor;
using UnityEngine;

namespace MiniRacing
{
    [CustomEditor(typeof(ThirdPersonCamera))]
    public class ThirdPersonCameraEditor : Editor
    {
        SerializedProperty maxDistFromTargetProp;
        SerializedProperty lookAngleProp;
        SerializedProperty offsetFromTargetProp;

        void OnEnable()
        {
            maxDistFromTargetProp = serializedObject.FindProperty("distanceFromTarget");
            lookAngleProp = serializedObject.FindProperty("lookAngle");
            offsetFromTargetProp = serializedObject.FindProperty("offsetFromTarget");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            ThirdPersonCamera controller = (ThirdPersonCamera)target;
            if(controller.followMode == ThirdPersonCamera.FollowMode.FixedHard ||
                controller.followMode == ThirdPersonCamera.FollowMode.FixedSmooth)
            {
                EditorGUILayout.PropertyField(offsetFromTargetProp, new GUIContent("Offset from target"));
            }
            else
            {
                EditorGUILayout.Slider(maxDistFromTargetProp, 1, 30, new GUIContent("Max distance from target"));
                EditorGUILayout.Slider(lookAngleProp, 0, 90, new GUIContent("Camera looking angle"));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}