

using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public partial class AiDecisionInfosEditor : AiDecisionInfo, IAiDecisionInfoEditor
{

    private readonly int _AiId = 0;

    public AiDecisionInfosEditor(int aiId)
    {
        _AiId = aiId;
    }

    [EditorField(nameof(_Decision))]
    private IAiDecisionEditor _Decision
    {
        get => this.GetFieldValue<IAiDecisionEditor>(nameof(_Decision));
        set => this.SetFieldValue<IAiDecisionEditor>(nameof(_Decision), value);
    }

    private Vector2 _RectPosOffset = new();
    private Vector2 _CurRectPosOffset = new();

    public void DrawEditor(in Rect parentRect)
    {
        var aiInfoRect = DrawTitle(in parentRect, in _CurRectPosOffset);

        UpdateViewMove();
        DrawBottomArea(in parentRect);
        _CurRectPosOffset = Vector3.Lerp(_CurRectPosOffset, _RectPosOffset, 0.03f);


        if (_Decision == null)
        {
            var decisionRect = new Rect(0, 0, 100, 50);
            decisionRect.center = aiInfoRect.center + new Vector2(0, aiInfoRect.height + 50 + decisionRect.height / 2);
            EditorGuiUtil.DrawEnum<EnAiDecisionId>(decisionRect, "Create", selectId =>
            {
                _Decision = SkillEditorDefine.CreateAiDecisionEditor(selectId);
            });
            EditorGuiUtil.LinkRectA2b(aiInfoRect, decisionRect, 0);
            return;
        }

        var rect = _Decision.CalculateLocalNodeArea();
        rect.position += aiInfoRect.position + EditorConfig.LayerInterval + new Vector2(aiInfoRect.width / 2 - rect.width / 2, aiInfoRect.height);
        _Decision.DrawNodeArea(rect);

        EditorGuiUtil.LinkRectA2b(aiInfoRect, rect, 0);
    }


    #region title
    private Rect DrawTitle(in Rect parentRect, in Vector2 offset)
    {
        var aiInfoRect = new Rect(0, 0, 200, 30);
        aiInfoRect.x = parentRect.x + parentRect.width / 2 - aiInfoRect.width / 2;
        aiInfoRect.y = parentRect.y + 100 + aiInfoRect.height / 2;
        aiInfoRect.position += offset;

        EditorGUI.DrawRect(aiInfoRect, EditorConfig.AreaColor);
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
        };
        GUI.Label(aiInfoRect, $"AiId: {_AiId}", style);
        return aiInfoRect;
    }
    #endregion

    #region mouse move
    private Vector2 _LastDownMousePos;
    private Vector2 _LastRectPosOffset;
    private void UpdateViewMove()
    {
        if (Event.current.button == 1)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                _LastDownMousePos = EditorUtil.GetEditorMousePos(Vector2.zero);
                _LastRectPosOffset = _RectPosOffset;
                EditorUtil.AddMouseController(EnMouseControllerType.Ai_ViewMove);
            }
            if (Event.current.type == EventType.MouseDrag)
            {
                var currentMousePos = EditorUtil.GetEditorMousePos(Vector2.zero);
                _RectPosOffset = _LastRectPosOffset + currentMousePos - _LastDownMousePos;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                EditorUtil.ClearMouseController(EnMouseControllerType.Ai_ViewMove);
            }
        }
    }
    #endregion

    #region bottom
    private void DrawBottomArea(in Rect parentRect)
    {
        var serIntArr = _Decision?.SerializeToIntArray() ?? new();
        var json = JsonConvert.SerializeObject(serIntArr);
        var height = GUI.skin.label.CalcHeight(new GUIContent(json), parentRect.width);
        var jsonRect = new Rect(parentRect.xMin, parentRect.yMax - height - 5, parentRect.width, height);
        EditorGUI.TextArea(jsonRect, json);


        if (_RectPosOffset != Vector2.zero)
        {
            var resetOffset = new Rect(parentRect.xMax - 110, parentRect.yMax - (parentRect.yMax - jsonRect.yMin) - 20, 100, 15);
            if (GUI.Button(resetOffset, "Reset View"))
            {
                _RectPosOffset = Vector2.zero;
            }
        }
    }

    #endregion

}