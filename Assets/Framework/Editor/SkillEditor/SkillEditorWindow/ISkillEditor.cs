using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillEditor : IEditor, ISerializeToIntArray
{
    public void OnSkillEditorGUI();
}

public interface ISkillDebugModelEditor
{
    public void OnDebugModelEnterEditor(GameObject go);
    public void OnDebugModelExitEditor();
}

public interface ISkillDebugModelUpdateEditor
{
    public void OnGuiDebugModelUpdateEditor();
}