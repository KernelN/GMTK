using UnityEditor;
using UnityEngine;

namespace PlanetMover.Objects
{
    [CustomEditor(typeof(ObjectScaler))]
    public class ObjectScalerEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            ObjectScaler myTarget = (ObjectScaler)target;

            DrawDefaultInspector();
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Scale", myTarget.Scale.ToString());
            
            EditorGUILayout.Separator();
            if(GUILayout.Button("Get Scale"))
            {
                myTarget.GetScale();
            }
        }
    }
}
