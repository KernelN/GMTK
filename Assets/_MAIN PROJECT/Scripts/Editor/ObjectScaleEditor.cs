using UnityEditor;
using UnityEngine;

namespace PlanetMover.Gameplay.Objects
{
    [CustomEditor(typeof(Scalable))]
    public class ObjectScaleEditor : Editor 
    {
        float scaleDebugger;
        public override void OnInspectorGUI()
        {
            Scalable myTarget = (Scalable)target;

            DrawDefaultInspector();
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Runtime Values", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Scale", myTarget.CurrentScale.ToString());
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Functions", EditorStyles.boldLabel);
            if(GUILayout.Button("Get Scale"))
            {
                myTarget.GetScale();
            }
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Set Scale"))
            {
                myTarget.SetScale(scaleDebugger);
            }
            scaleDebugger = EditorGUILayout.Slider("", scaleDebugger, myTarget.MinScale, myTarget.MaxScale);
            GUILayout.EndHorizontal();
        }
    }
}
