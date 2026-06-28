
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class SkillDebugModelUtil
{
    private static float _MouseOffsetPosX = -1;
    private static readonly Dictionary<EnAtkLinkScheculeType, Color> _TrackColors = new()
    {
        { EnAtkLinkScheculeType.Physics, new Color(1, 0, 0, 0.2f) },
        { EnAtkLinkScheculeType.Buff, new Color(0, 1, 0, 0.2f) },
        { EnAtkLinkScheculeType.Behaviour, new Color(0, 1, 1, 0.2f) },
        { EnAtkLinkScheculeType.Effect, new Color(1, 1, 0, 0.2f) },
        { EnAtkLinkScheculeType.FlyItem, new Color(1f, 0.5f, 0.5f, 0.2f) },
    };
    public static float GetTrackStartPosY(in Rect mainRect, in EnAtkLinkScheculeType scheduleType)
    {
        var padding = new float2(10, 10);
        var maxHeight = mainRect.height - padding.x - padding.y;
        var height = maxHeight / ((int)EnAtkLinkScheculeType.EnumCount - 1);
        var localPosY = height * ((int)scheduleType - 0.5f);
        return localPosY + padding.x;
    }

    public static Color GetTrackColor(in EnAtkLinkScheculeType scheduleType)
    {
        return _TrackColors[scheduleType];
    }
    public static float OnGuiStageBoxSliderController(
        in Rect mainRect
        , in Rect boxRect
        , in int key
        , in float schedule
        , ref int ctrlId)
    {
        var result = schedule;
        if (EditorUtil.HasMouseInRect(in boxRect)
            && Event.current.type == EventType.MouseDown
            && EditorUtil.MouseControllerIsNone()
            && EditorUtil.ControllerIsActive(EnEditorGlobalController.Move))
        {
            var mousePos = EditorUtil.GetEditorMousePos(Vector2.zero);
            ctrlId = key;
            EditorUtil.AddMouseController(EnMouseControllerType.StageScheduleBoxSlider);

            _MouseOffsetPosX = (mainRect.xMin + mainRect.width * schedule) - mousePos.x;
        }

        if (!EditorUtil.MouseControllerIs(EnMouseControllerType.StageScheduleBoxSlider)) return result;
        if (ctrlId != key) return result;

        var sliderValue = EditorUtil.GetMouseLocalSchedule(in mainRect, Vector2.right * _MouseOffsetPosX);
        result = sliderValue.x;
        if (Event.current.type == EventType.MouseUp)
        {
            EditorUtil.ClearMouseController(EnMouseControllerType.StageScheduleBoxSlider);
            ctrlId = -1;
        }

        return result;
    }
    public static void OnDebugModelStageBoxClick(in Rect boxRect, in ISkillEditor skillEditor)
    {
        if (!EditorUtil.HasMouseInRect(in boxRect)) return;
        if (Event.current.type != EventType.MouseDown) return;
        if (!EditorUtil.ControllerIsActive(EnEditorGlobalController.Click)) return;
        SkillPlayableAdapterDrawStageDebugModelEditor.SetDebugModelDrawSkillEditor(skillEditor);
    }
    public static Rect GetScheduleRect(
        in Rect mainRect
        , in EnAtkLinkScheculeType scheduleType
        , [InspectorRange(0, 1)] in float value)
    {
        var centerY = SkillDebugModelUtil.GetTrackStartPosY(in mainRect, in scheduleType) + mainRect.y;
        var boxSize = new Vector2(30, 15);
        var pointPosX = mainRect.width * value;
        var boxCenterOffsetX = (value - 0.5f) * -boxSize.x;
        var boxRect = new Rect()
        {
            size = boxSize,
            center = new Vector2(mainRect.x + pointPosX + boxCenterOffsetX, centerY)
        };
        return boxRect;
    }
    public static Rect DrawSliderBox(
        in Rect mainRect
        , in EnAtkLinkScheculeType scheduleType
        , [InspectorRange(0, 1)] in float value
        , in int key
        , in int ctrlId)
    {
        var boxRect = GetScheduleRect(in mainRect, in scheduleType, in value);
        var pointRect = new Rect()
        {
            size = new Vector2(3, boxRect.size.y),
            center = new Vector2(mainRect.x + mainRect.width * value, 0),
            y = boxRect.y
        };

        if (!EditorUtil.MouseControllerIs(EnMouseControllerType.StageScheduleBoxSlider))
            EditorGUI.DrawRect(boxRect, SkillEditorDefine.BoxDefaultColor);
        EditorGUI.DrawRect(pointRect, SkillDebugModelUtil.GetTrackColor(scheduleType));

        if (EditorUtil.HasMouseInRect(in mainRect)
            || ctrlId == key)
        {
            var labelRect = new Rect(boxRect)
            {
                y = boxRect.y + (EditorUtil.MouseControllerIs(EnMouseControllerType.StageScheduleBoxSlider)
                    ? -pointRect.height
                    : 0),
            };
            EditorGUI.LabelField(labelRect, $"{value * 100:0}%", SkillEditorUtil.LabelCenterStyle);
        }

        return boxRect;
    }
    public static (float value, Rect boxRect) DrawStageScheduleActionBox<T>(in T scheduleEditor, in Rect mainRect, float value, in int key, ref int ctrlId)
        where T : ISkillScheduleAction, ISkillEditor
    {
        var scheduleType = scheduleEditor.GetScheduleType();
        var boxRect = SkillDebugModelUtil.GetScheduleRect(in mainRect, scheduleType, value);
        var result = SkillDebugModelUtil.OnGuiStageBoxSliderController(in mainRect, in boxRect, in key, in value, ref ctrlId);

        boxRect = SkillDebugModelUtil.DrawSliderBox(in mainRect, in scheduleType, in result, in key, in ctrlId);
        if (EditorUtil.HasMouseInRect(in boxRect) || ctrlId == key)
            SkillEditorUtil.DrawStageSelectBox(in boxRect);

        SkillDebugModelUtil.OnDebugModelStageBoxClick(in boxRect, scheduleEditor);
        SkillDebugModelUtil.OnDebugModelSelectBoxHitArea(in boxRect, scheduleEditor);
        return (result, boxRect);
    }
    private static void OnDebugModelSelectBoxHitArea(in Rect boxRect, in ISkillEditor skillEditor)
    {
        if (!SkillPlayableAdapterDrawStageDebugModelEditor.IsDebugModelDrawSkillEditor(skillEditor))
            return;
        var mainColor = SkillEditorUtil.GetDrawSkillColor(skillEditor);
        var icon = EditorGUIUtility.IconContent("d_orangeLight");
        var iconRect = new Rect()
        {
            size = new Vector2(10, 10),
            center = new Vector2(boxRect.center.x, boxRect.yMax),
        };
        var isMouseContains = EditorUtil.HasMouseInRect(in boxRect);

        var hitInfo = SkillPlayableAdapterDrawStageDebugModelEditor.GetDrawSkillScheduleInfo(skillEditor);
        var forceColor = new Color(mainColor.r, mainColor.g, mainColor.b, mainColor.a * (isMouseContains ? 0.2f : 0.05f));
        var outlineColor = new Color(mainColor.r, mainColor.g, mainColor.b, mainColor.a * (isMouseContains ? 0.4f : 0.1f));
        Handles.DrawSolidRectangleWithOutline(new Vector3[]
        {
            hitInfo.rect.center + hitInfo.rect.size * 0.5f * new Vector2(-1, -1)
            , hitInfo.rect.center + hitInfo.rect.size * 0.5f * new Vector2(-1, 1)
            , hitInfo.rect.center + hitInfo.rect.size * 0.5f * new Vector2(1, 1)
            , hitInfo.rect.center + hitInfo.rect.size * 0.5f * new Vector2(1, -1)
        }, forceColor, outlineColor);
        Handles.DrawSolidRectangleWithOutline(new Vector3[]
        {
            iconRect.center
            , hitInfo.rect.center + hitInfo.rect.size * 0.5f * new Vector2(-1, -1)
            , hitInfo.rect.center + hitInfo.rect.size * 0.5f * new Vector2(1, -1)
            , hitInfo.rect.center + hitInfo.rect.size * 0.5f * new Vector2(1, -1)
        }, forceColor, outlineColor);

        EditorGUI.LabelField(iconRect, icon);
    }
}