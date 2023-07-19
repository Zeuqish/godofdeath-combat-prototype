using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatManager))]
public class CombatDataEditor : Editor
{
    private Editor editorInstance;

    private void OnEnable()
    {
        editorInstance = null;
    }

    public override void OnInspectorGUI()
    {
        CombatManager combatManager = (CombatManager)target;
        
        if (editorInstance == null)
        {
            editorInstance = Editor.CreateEditor(combatManager);
        }
        base.OnInspectorGUI();
        editorInstance.DrawDefaultInspector();
    }
}
