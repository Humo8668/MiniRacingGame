#if UNITY_EDITOR

using UnityEngine;

namespace MiniRacing
{
    [UnityEditor.CustomEditor(typeof(ThirdPersonCamera))]
    public class ThirdPersonCameraEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty maxDistFromTargetProp;
        UnityEditor.SerializedProperty lookAngleProp;
        UnityEditor.SerializedProperty offsetFromTargetProp;

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
                UnityEditor.EditorGUILayout.PropertyField(offsetFromTargetProp, new GUIContent("Offset from target"));
            }
            else
            {
                UnityEditor.EditorGUILayout.Slider(maxDistFromTargetProp, 1, 30, new GUIContent("Distance from target"));
                UnityEditor.EditorGUILayout.Slider(lookAngleProp, 0, 90, new GUIContent("Camera looking angle"));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif