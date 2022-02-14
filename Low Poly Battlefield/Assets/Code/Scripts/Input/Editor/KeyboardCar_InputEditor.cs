using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Car
{
    [CustomEditor(typeof(KeyboardCar_Input))]
    public class KeyboardCar_InputEditor : Editor
    {
        #region Variables
        KeyboardCar_Input targetInput;
        #endregion

        #region Builtin Methods
        private void OnEnable()
        {
            targetInput = (KeyboardCar_Input)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDebugUI();

            Repaint();
        }
        #endregion

        #region Custom Methods
        void DrawDebugUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Accelerator: " + targetInput.AcceleratorInput.ToString("0.00"), EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Direction: " + targetInput.DirectionInput.ToString("0.00"), EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Break: " + targetInput.Breaking.ToString(), EditorStyles.boldLabel);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}
