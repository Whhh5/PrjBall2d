

using UnityEditor;

/// <summary>
/// & alt
/// % ctrl
/// # shift
/// _：无修饰（仅限主菜单）(仅 F1："Tools/MyMenu _F1")
/// 字母或数字：具体按键
/// </summary>
internal class EditorWindowManager
{
    [MenuItem("AbbFramework/Skill Editor %E")]
    public static void OpenSkillEditorWindow()
    {
        var window = EditorWindow.GetWindow<SkillEditorWindow>();
        window.titleContent = new UnityEngine.GUIContent("Skill Editor");
        window.Show();
    }
    [MenuItem("AbbFramework/Ai Editor &%A")]
    public static void OpenAiEditorWindow()
    {
        var window = EditorWindow.GetWindow<AiEditorWindow>();
        window.titleContent = new UnityEngine.GUIContent("Ai Editor");
        window.Show();
    }
}

