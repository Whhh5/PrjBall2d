

using UnityEditor;
using UnityEngine;

public partial class SkillEffectScheduleActionEditor : ISkillScheduleActionDebugModelEditor
{
    private GameObject _TargetGo = null;
    private GameObject _EffectGo = null;
    private Rect _EffectRect = default;
    private Rect _StageRect = default;
    public void OnDebugModelEnterEditor(GameObject go)
    {
        _TargetGo = go;
        var effectCfg = ExcelEditorUtil.GetCfg<EffectCfg>(_EffectID);
        var assetCfg = ExcelEditorUtil.GetCfg<AssetCfg>(effectCfg.nAssetID);
        var assetGo = AssetDatabase.LoadAssetAtPath<GameObject>(assetCfg.strPath);
        _EffectGo = Object.Instantiate(assetGo);
        _EffectGo.SetActive(false);
        ResetEffectTran();
    }

    public void OnDebugModelExitEditor()
    {
        Object.DestroyImmediate(_EffectGo);

        _TargetGo = null;
        _EffectGo = null;
        _EffectRect = default;
    }

    public void OnGuiDebugModelUpdateEditor(in Rect stageRect, in int key, ref int ctrlId)
    {
        _StageRect = stageRect;
        var (value, effectRect) = SkillDebugModelUtil.DrawStageScheduleActionBox(this, stageRect, _Schedule, key, ref ctrlId);
        _EffectRect = effectRect;
        if (!Mathf.Approximately(value, _Schedule))
        {
            _Schedule = value;
        }
    }

    public void OnSceneDebugModelUpdateEditor(in SkillDebugModelTimelineInfo timeInfo)
    {
        var effectCfg = ExcelEditorUtil.GetCfg<EffectCfg>(_EffectID);
        var time = effectCfg.fDelayDestroyTime;
        var curShowTime = timeInfo.skillTime - timeInfo.StageLocalSliderToSkillTime(_Schedule);
        var state = curShowTime >= 0 && timeInfo.stageLocalSlider >= _Schedule && curShowTime <= time;
        if (_EffectGo.activeSelf != state)
        {
            _EffectGo.SetActive(state);
            ResetEffectTran();
        }
        if (!state)
            return;

        UpdateEffectTime(curShowTime);
        SetEffectPosOffset(_EffectGo.transform.position);
        SetEffectRotOffset(_EffectGo.transform.rotation);
        SetEffectScaleOffset(_EffectGo.transform.lossyScale);

        var sliderBgRect = new Rect()
        {
            x = _StageRect.x + _StageRect.width * _Schedule,
            y = _EffectRect.yMax,
            width = time / timeInfo.stageMaxTime * _StageRect.width,
            height = 2,
        };
        EditorGuiUtil.DrawSlider(sliderBgRect, curShowTime / time);
    }

    private void UpdateEffectTime(float time)
    {
        var effectComs = _EffectGo.GetComponent<ParticleSystem>();

        effectComs.Simulate(time, true, true, true);
        //for (int i = 0; i < effectComs.Length; i++)
        //{
        //    var effectCom = effectComs[i];
            
        //}
    }

    private void ResetEffectTran()
    {
        _EffectGo.transform.position = GetEffectPosition();
        _EffectGo.transform.rotation = GetEffectRotation();
        _EffectGo.transform.localScale = GetEffectScale();
    }
    private Vector3 GetEffectPosition()
    {
        return CalculateUtility.GetPos(_TargetGo.transform.position, _TargetGo.transform.forward, _TargetGo.transform.up, _TargetGo.transform.right, _OffsetPos);
    }
    private Quaternion GetEffectRotation()
    {
        return CalculateUtility.GetRotation(_TargetGo.transform.rotation, Quaternion.Euler(_OffsetRot));
    }
    private Vector3 GetEffectScale()
    {
        return CalculateUtility.GetScale(_TargetGo.transform.lossyScale, _OffsetScale);
    }
    private void SetEffectPosOffset(Vector3 worldPos)
    {
        var pos = CalculateUtility.GetPosOffset(_TargetGo.transform.position, _TargetGo.transform.forward, _TargetGo.transform.up, _TargetGo.transform.right, worldPos);
        if (Vector3.Distance(pos, _OffsetPos) < 0.01f)
            return;
        _OffsetPos = pos;
    }
    private void SetEffectRotOffset(Quaternion worldRot)
    {
        var rot = CalculateUtility.GetRotationOffset(_TargetGo.transform.rotation, worldRot);
        if (Quaternion.Angle(rot, Quaternion.Euler(_OffsetRot)) < 0.1f)
            return;
        _OffsetRot = rot.eulerAngles;
    }
    private void SetEffectScaleOffset(Vector3 worldScale)
    {
        var scale = CalculateUtility.GetScaleOffset(_TargetGo.transform.lossyScale, worldScale);
        if (Vector3.Distance(scale, _OffsetScale) < 0.01f)
            return;
        _OffsetScale = scale;
    }
}