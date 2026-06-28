

using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public partial class SkillFlyItemScheduleActionEditor : ISkillScheduleActionDebugModelEditor
{
    private Transform _TargetTran;
    private GameObject _FlyItemGo;
    private GameObject _FlyItemGo2;
    private Rect _BoxRect;
    private Rect _StageRect;
    public void OnDebugModelEnterEditor(GameObject go)
    {
        _TargetTran = go.transform;
        var projectileCfg = ExcelEditorUtil.GetCfg<ProjectileCfg>(_ProjectileCfgId);
        var assetCfg = ExcelEditorUtil.GetCfg<AssetCfg>(projectileCfg.nAssetID);
        var goAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetCfg.strPath);
        _FlyItemGo = Object.Instantiate(goAsset);
        _FlyItemGo2 = Object.Instantiate(goAsset);
        ResetFlyItemTran();
    }

    public void OnDebugModelExitEditor()
    {
        Object.DestroyImmediate(_FlyItemGo.gameObject);
        Object.DestroyImmediate(_FlyItemGo2.gameObject);
        _TargetTran = null;
        _FlyItemGo = null;
        _FlyItemGo2 = null;
    }

    public void OnGuiDebugModelUpdateEditor(in Rect stageRect, in int key, ref int ctrlId)
    {
        var (value, rect) = SkillDebugModelUtil.DrawStageScheduleActionBox(this, in stageRect, _ScheduleInvoke, key, ref ctrlId);
        _BoxRect = rect;
        _StageRect = stageRect;
        if (!Mathf.Approximately(value, _ScheduleInvoke))
        {
            _ScheduleInvoke = value;
        }
    }

    public void OnSceneDebugModelUpdateEditor(in SkillDebugModelTimelineInfo timeInfo)
    {
        var effectTime = _MaxDistance / _Speed;
        var skillTime = timeInfo.StageLocalSliderToSkillTime(_ScheduleInvoke);
        var localTime = timeInfo.skillTime - skillTime;
        var state = localTime >= 0 && localTime <= effectTime;
        if (state != _FlyItemGo.activeSelf)
        {
            _FlyItemGo.SetActive(state);
            _FlyItemGo2.SetActive(state);
            ResetFlyItemTran();
        }
        if (!state)
        {
            return;
        }

        var slider = localTime / effectTime;
        _PosOffset = CalculateUtility.GetPosOffset(_TargetTran, _FlyItemGo.transform.position);
        _RotOffset = CalculateUtility.GetRotation(_TargetTran.rotation, _FlyItemGo.transform.rotation).eulerAngles;

        var pos = GetFlyItemPosition(slider, _FlyItemGo.transform.position, _FlyItemGo.transform.forward * _MaxDistance);
        _FlyItemGo2.transform.position = pos;
        
        var bgRect = new Rect()
        {
            width = effectTime * _StageRect.width / timeInfo.stageMaxTime,
            height = 2,
            x = _StageRect.x + _ScheduleInvoke * _StageRect.width,
            y = _BoxRect.yMax,
        };
        EditorGuiUtil.DrawSlider(bgRect, slider);
    }

    private void ResetFlyItemTran()
    {
        _FlyItemGo.transform.position 
            = _FlyItemGo2.transform.position
            = CalculateUtility.GetPos(_TargetTran, _PosOffset);
        _FlyItemGo.transform.rotation 
            = _FlyItemGo2.transform.rotation
            = CalculateUtility.GetRotation(_TargetTran.rotation, quaternion.Euler(_RotOffset));
    }
}
