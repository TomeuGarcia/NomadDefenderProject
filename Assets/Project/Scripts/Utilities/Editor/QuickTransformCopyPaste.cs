using UnityEngine;
using UnityEditor;

public class QuickTransformCopyPaste
{
    private static Vector3 copiedPosition;
    private static bool hasCopied = false;

    [MenuItem("Tools/Copy Position _9")]
    private static void CopyPosition()
    {
        if (Selection.activeTransform)
        {
            copiedPosition = Selection.activeTransform.position;
            hasCopied = true;
        }
    }

    [MenuItem("Tools/Paste Position _0")]
    private static void PastePosition()
    {
        if (Selection.activeTransform && hasCopied)
        {
            Undo.RecordObject(Selection.activeTransform, "Paste Position");

            Selection.activeTransform.position = copiedPosition;

            EditorUtility.SetDirty(Selection.activeTransform);
        }
    }
}
