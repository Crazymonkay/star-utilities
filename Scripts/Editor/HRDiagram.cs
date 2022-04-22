using UnityEditor;
using UnityEngine;

namespace RoadTurtleGames.StarUtilities.EditorScripts
{
    public static class HRDiagram
    {
        public static StarProperties DrawHertzsprungRussellDiagram(StarProperties p, Texture2D hertzsprungRussellDiagram, Texture2D hertzsprungRussellDiagramPoint)
        {
            //Draw diagram
            const float topMargin = 20;
            const float leftMargin = 30;
            const float rightBottomMargin = 20;

            Rect graphRect = GUILayoutUtility.GetRect(Screen.width, Screen.width);
            GUI.Box(graphRect, new GUIContent("Hertzsprung-Russell Diagram"));

            Rect imageRect = new Rect(graphRect.x + leftMargin, graphRect.y + topMargin, graphRect.width - leftMargin - rightBottomMargin, graphRect.height - leftMargin - rightBottomMargin);
            GUI.DrawTexture(imageRect, hertzsprungRussellDiagram);

            //Luminosity on Y axis
            float labelInterval = imageRect.height / (StarUtilities.LuminosityExponentMax - StarUtilities.LuminosityExponentMin);
            int c = 0;

            for (int i = StarUtilities.LuminosityExponentMax; i >= StarUtilities.LuminosityExponentMin; i--)
            {
                Rect r = new Rect(graphRect.x, imageRect.y + labelInterval * c - (labelInterval / 2f), leftMargin, labelInterval);

                if (i != 0)
                    EditorGUI.LabelField(r, "10^" + i, EditorStyles.miniLabel);
                else
                    EditorGUI.LabelField(r, "1", EditorStyles.miniLabel);

                EditorGUI.DrawRect(new Rect(r.xMax, r.y + r.height / 2f, 10, 1), Color.white);
                c++;
            }

            //Temperature on X axis
            int steps = 5;
            labelInterval = imageRect.width / (steps - 1);
            float temp = StarUtilities.TemperatureMin;

            for (int i = 0; i < steps; i++)
            {
                float x = imageRect.xMax - (labelInterval * i);
                Rect r = new Rect(x, imageRect.yMax, labelInterval, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(r, temp + "K", EditorStyles.miniLabel);
                EditorGUI.DrawRect(new Rect(r.x, r.yMin - 10, 1, 10), Color.white);

                temp *= 2;
            }

            //Allow for clicking directly on graph
            Event currentEvent = Event.current;
            bool isEventValid = currentEvent.type == EventType.MouseDown && imageRect.Contains(currentEvent.mousePosition);
            if (isEventValid)
            {
                Vector2 relativeMousePos = currentEvent.mousePosition - imageRect.position;
                Vector2 normalizedMousePos = new Vector2(1 - (relativeMousePos.x / imageRect.width), 1 - (relativeMousePos.y / imageRect.height));

                p.Temperature = (int)StarUtilities.GetTemperature(normalizedMousePos.x);
                p.Radius = StarUtilities.GetEstimatedRadius(p.Temperature, StarUtilities.GetLuminosity(normalizedMousePos.y));
            }

            float normalizedTemperature = StarUtilities.GetNormalizedTemperature(p.Temperature);
            float normalizedLuminosityExponent = StarUtilities.GetNormalizedEstimatedSolarLuminosityExponent(p.Temperature, p.Radius);

            Rect pointRect = new Rect(imageRect.xMax - (imageRect.width * normalizedTemperature) - 8, imageRect.yMax - (imageRect.height * normalizedLuminosityExponent) - 8, 16, 16);
            GUI.DrawTexture(pointRect, hertzsprungRussellDiagramPoint);

            string spectralClass = StarUtilities.GetSpectralClass(p.Temperature);
            float solarRadii = p.Radius / StarUtilities.SunRadius;
            float solarTemp = (float)p.Temperature / StarUtilities.SunTemperature;
            float solarLuminosity = StarUtilities.GetEstimatedSolarLuminosity(p.Temperature, p.Radius);

            //Spectral class label
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Estimated Spectral Class", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField(spectralClass, GUILayout.Width(80));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Luminosity
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Estimated Luminosity", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField(solarLuminosity.ToString(), GUILayout.Width(80));
            EditorGUILayout.LabelField(" L☉");

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Solar radii
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Radius", EditorStyles.boldLabel, GUILayout.Width(200));

            EditorGUI.BeginChangeCheck();
            float modifiedSolarRadius = EditorGUILayout.FloatField(solarRadii, GUILayout.Width(80));

            if (EditorGUI.EndChangeCheck())
            {
                p.Radius = modifiedSolarRadius * StarUtilities.SunRadius;
            }

            EditorGUILayout.LabelField(" R☉", GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Solar Temperature
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Temperature", EditorStyles.boldLabel, GUILayout.Width(200));

            EditorGUI.BeginChangeCheck();
            float modifiedSolarTemp = EditorGUILayout.FloatField(solarTemp, GUILayout.Width(80));

            if (EditorGUI.EndChangeCheck())
            {
                p.Temperature = (int)(modifiedSolarTemp * StarUtilities.SunTemperature);
            }

            EditorGUILayout.LabelField(" T☉", GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            return p;
        }
    }
}