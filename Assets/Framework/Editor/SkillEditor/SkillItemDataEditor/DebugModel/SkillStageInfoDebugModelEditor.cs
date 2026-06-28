using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillStageInfoEditor : ISkillDebugModelEditor
{


    private const float _TrackLingHeight = 2f;
    private const float _HitInfoRadius = 5;
    private int _StageScheduleBoxControllerId = -1;
    private static int _CurrentStageIndex = -1;

    private static float _MouseOffsetPosX = -1;

    public void OnDebugModelExitEditor()
    {
        for (int i = 0; i < _ArrAtkLinkSchedule?.Length; i++)
        {
            if (_ArrAtkLinkSchedule[i] is not ISkillDebugModelEditor debugEditor)
                continue;
            debugEditor.OnDebugModelExitEditor();
        }
        _MouseOffsetPosX
            = _StageScheduleBoxControllerId
            = -1;
    }
    public void OnDebugModelEnterEditor(GameObject go)
    {
        for (int i = 0; i < _ArrAtkLinkSchedule?.Length; i++)
        {
            if (_ArrAtkLinkSchedule[i] is not ISkillDebugModelEditor debugEditor)
                continue;
            debugEditor.OnDebugModelEnterEditor(go);
        }
    }

    #region gui
    public void OnGuiDebugModelUpdateEditor(in Rect stageRect, in int index)
    {
        EditorGUI.DrawRect(stageRect, new Color(0.8f, 0.8f, 0.8f, 0.1f));
        //EditorGUI.LabelField(stageRect, _ClipId.ToString(), SkillEditorUtil.LabelCenterStyle);

        OnGuiDebugModelBackgroundEditor(in stageRect);
        if (_ArrAtkLinkSchedule is { Length: > 0 })
            DrawStageBox(in stageRect);
        if (EditorUtil.HasMouseInRect(stageRect)
            || (EditorUtil.MouseControllerIs(EnMouseControllerType.StageScheduleBoxSlider) && _StageScheduleBoxControllerId >= 0))
            SkillEditorUtil.DrawStageSelectBox(in stageRect);

        OnGuiDebugModelInfoEditor(in stageRect, in index);
    }

    private void DrawStageBox(in Rect stageRect)
    {
        for (var i = 0; i < _ArrAtkLinkSchedule.Length; i++)
        {
            var scheduleEditor = _ArrAtkLinkSchedule[i];
            var scheduleDebugModelEditor = scheduleEditor as ISkillScheduleActionDebugModelEditor;

            scheduleDebugModelEditor.OnGuiDebugModelUpdateEditor(in stageRect, in i, ref _StageScheduleBoxControllerId);
        }
    }

    private void OnGuiDebugModelInfoEditor(in Rect mainRect, in int index)
    {
        _AtkEndTime = OnDebugModelStagePointInfo(mainRect, _AtkEndTime, Color.red, EnMouseControllerType.StageInfoSlider_AtkEndTime, index, "_AtkEndTime");
        _CanNextTime = OnDebugModelStagePointInfo(mainRect, _CanNextTime, Color.yellow, EnMouseControllerType.StageInfoSlider_CanNextTime, index, "_CanNextTime");
    }

    private static float OnDebugModelStagePointInfo(in Rect mainRect, float value, Color color, EnMouseControllerType ctlType, int ctrlKey, string hitMsg)
    {
        var center = new Vector2(mainRect.x + mainRect.width * value, mainRect.y);
        var hitRect = new Rect()
        {
            size = _HitInfoRadius * 2 * Vector2.one,
            center = center
        };

        if (EditorUtil.HasMouseInRect(in hitRect)
            && EditorUtil.MouseControllerIsNone()
            && Event.current.type == EventType.MouseDown
            && Event.current.button == 0
            )
        {
            var mousePos = EditorUtil.GetEditorMousePos(Vector2.zero);
            EditorUtil.AddMouseController(ctlType);
            _CurrentStageIndex = ctrlKey;
            _MouseOffsetPosX = (mainRect.width * value + mainRect.xMin) - mousePos.x;
        }

        if (EditorUtil.MouseControllerIs(ctlType) && _CurrentStageIndex == ctrlKey)
        {
            var sliderValue = EditorUtil.GetMouseLocalSchedule(in mainRect, Vector2.right * _MouseOffsetPosX);
            value = sliderValue.x;

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                EditorUtil.ClearMouseController(ctlType);
            }
        }

        hitRect.center = new Vector2(mainRect.x + mainRect.width * value, hitRect.center.y);
        EditorGuiUtil.DrawTriangleHit(hitRect, _HitInfoRadius, color, hitMsg);
        return value;
    }



    // draw bg
    private void OnGuiDebugModelBackgroundEditor(in Rect mainRect)
    {
        for (var i = EnAtkLinkScheculeType.None + 1; i < EnAtkLinkScheculeType.EnumCount; i++)
        {
            var localPosY = SkillDebugModelUtil.GetTrackStartPosY(in mainRect, in i);
            var trackRect = new Rect()
            {
                size = new Vector2(mainRect.width, _TrackLingHeight),
                center = new Vector2(0, mainRect.y + localPosY),
                x = mainRect.xMin,
            };
            EditorGUI.DrawRect(trackRect, SkillDebugModelUtil.GetTrackColor(i));
        }
    }



    #endregion

    #region slider change
    public void OnSceneDebugModelSliderValueChange(in SkillDebugModelTimelineInfo timeInfo)
    {
        for (int i = 0; i < _ArrAtkLinkSchedule?.Length; i++)
        {
            if (_ArrAtkLinkSchedule[i] is not ISkillScheduleActionDebugModelEditor debugEditor)
                continue;
            debugEditor.OnSceneDebugModelUpdateEditor(in timeInfo);
        }
    }
    #endregion
}