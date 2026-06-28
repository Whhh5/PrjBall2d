

using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using UnityEditor;
using UnityEngine;

public partial class SkillBuffScheduleActionEditor : ISkillScheduleActionDebugModelEditor
{
    public void OnDebugModelEnterEditor(GameObject go)
    {
    }

    public void OnDebugModelExitEditor()
    {
    }

    public void OnGuiDebugModelUpdateEditor(in Rect stageRect, in int key, ref int ctrlId)
    {
        var (startSchedule, startRect) = SkillDebugModelUtil.DrawStageScheduleActionBox(this, in stageRect, _StartSchedule, key + 1001, ref ctrlId);
        if (!Mathf.Approximately(startSchedule, _StartSchedule))
        {
            _StartSchedule = startSchedule;
            if (startSchedule > _EndSchedule)
            {
                _EndSchedule = startSchedule;
            }
        }

        if (_IsSectionBuff)
        {
            var (endSchedule, endRect) = SkillDebugModelUtil.DrawStageScheduleActionBox(this, in stageRect, _EndSchedule, key + 1002, ref ctrlId);
            if (!Mathf.Approximately(endSchedule, _EndSchedule))
            {
                _EndSchedule = endSchedule;
                if (endSchedule < _StartSchedule)
                {
                    _StartSchedule = endSchedule;
                }
            }

            var startPos = new Vector2(stageRect.x + stageRect.width * _StartSchedule, startRect.yMin);
            var endPos = new Vector2(stageRect.x + stageRect.width * _EndSchedule, startRect.yMin);

            var startPos2 = new Vector2(startPos.x + (endPos.x - startPos.x) * 0.2f, startPos.y - 10f);
            var endPos2 = new Vector2(endPos.x - (startPos2.x - startPos.x), startPos2.y);

            Handles.DrawDottedLines(
                new Vector3[] { startPos, startPos2, endPos2, endPos }
                , new int[] { 0, 1, 1, 2, 2, 3 }
                , 0.5f);
        }

    }

    public void OnSceneDebugModelUpdateEditor(in SkillDebugModelTimelineInfo timeInfo)
    {
    }
}