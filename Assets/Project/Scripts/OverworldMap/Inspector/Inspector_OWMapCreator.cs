using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(OverworldMapCreator))]
public class Inspector_OWMapCreator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OverworldMapCreator overworldMapCreator = (OverworldMapCreator)target;

        
        // This creates infinite loop somehow :)
        /*
        GUILayout.Space(10);      
        if (GUILayout.Button("Regenerate Map"))
        {            
            overworldMapCreator.RegenerateMap();
        }
        */
    }

}
