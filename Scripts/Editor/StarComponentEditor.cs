using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RoadTurtleGames.StarUtilities.EditorScripts
{
    [CustomEditor(typeof(StarComponent))]
    public class StarComponentEditor : Editor
    {
        private Texture2D hertzsprungRussellDiagram;
        private Texture2D hertzsprungRussellDiagramPoint;

        private SerializedProperty starProperty;
        private SerializedProperty radiusProperty;
        private SerializedProperty temperatureProperty;

        private void OnEnable()
        {
            hertzsprungRussellDiagram = Resources.Load<Texture2D>("Hertzsprung–Russell_Diagram");
            hertzsprungRussellDiagramPoint = Resources.Load<Texture2D>("Hertzsprung–Russell_Diagram_Point");

            starProperty = serializedObject.FindProperty("StarProperties");
            temperatureProperty = starProperty.FindPropertyRelative("Temperature");
            radiusProperty = starProperty.FindPropertyRelative("Radius");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            EditorGUILayout.Separator();

            StarProperties p = new StarProperties()
            {
                Temperature = temperatureProperty.intValue,
                Radius = radiusProperty.floatValue
            };

            p = HRDiagram.DrawHertzsprungRussellDiagram(p, hertzsprungRussellDiagram, hertzsprungRussellDiagramPoint);

            radiusProperty.floatValue = p.Radius;
            temperatureProperty.intValue = p.Temperature;

            serializedObject.ApplyModifiedProperties();
        }
    }
}


