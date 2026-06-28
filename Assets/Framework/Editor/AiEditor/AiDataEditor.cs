using System.Collections.Generic;
using UnityEngine;

public class AiDataEditor : AiData, IAiEditor
{
    public EnTypeId GetTypeDefineId()
    {
        throw new System.NotImplementedException();
    }

    public List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in _AiCfgId);
        ISerializeToIntArray.SerializeToInt(ref list, in _PerceptionInfos);
        ISerializeToIntArray.SerializeToInt(ref list, in _DecisionInfos);
        return list;
    }

    public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var localIndex = 0;
        ISerializeToIntArray.ParseToValue(datas, start, count, ref localIndex, out _AiCfgId);
        ISerializeToIntArray.ParseToValue(datas, start, count, ref localIndex, out _PerceptionInfos, userData);
        ISerializeToIntArray.ParseToValue(datas, start, count, ref localIndex, out _DecisionInfos, userData);
    }

    private int _AiCfgId;
    private AiPerceptionInfosEditor _PerceptionInfos = null;
    private AiDecisionInfosEditor _DecisionInfos = null;

    public void AppendData()
    {
        var aiCfg = ExcelEditorUtil.GetCfg<AICfg>(_AiCfgId);
        {
            var datas1 = _PerceptionInfos.SerializeToIntArray();
            ExcelEditorUtil.SetCfgValue(aiCfg, nameof(AICfg.arrPerception), datas1.ToArray());
        }

        {
            var datas2 = _DecisionInfos.SerializeToIntArray();
            ExcelEditorUtil.SetCfgValue(aiCfg, nameof(AICfg.arrDecision), datas2.ToArray());
        }
    }
    public void Initialization(int aiCfgId)
    {
        _AiCfgId = aiCfgId;

        var aiCfg = ExcelEditorUtil.GetCfg<AICfg>(aiCfgId);
        {
            var aiComUserData = new AiCommonUserData()
            {
                aiId = aiCfgId,
                arrParams = aiCfg.arrDecision,
                count = aiCfg.arrDecision.Length,
                startIndex = 0,
            };
            _DecisionInfos = new AiDecisionInfosEditor(aiCfgId);
            _DecisionInfos.DeserializeFromIntArray(aiComUserData.arrParams,
                aiComUserData.startIndex, aiComUserData.count, aiComUserData);
            EditorUtil.ChangeEditorStruct(_DecisionInfos);
        }
        {
            var aiComUserData = new AiCommonUserData()
            {
                aiId = aiCfgId,
                arrParams = aiCfg.arrPerception,
                count = aiCfg.arrPerception.Length,
                startIndex = 0,
            };
            _PerceptionInfos = new AiPerceptionInfosEditor(aiCfgId);
            _PerceptionInfos.DeserializeFromIntArray(aiComUserData.arrParams,
                aiComUserData.startIndex, aiComUserData.count, aiComUserData);
            EditorUtil.ChangeEditorStruct(_PerceptionInfos);
        }
    }

    public int GetAiId()
    {
        return _AiCfgId;
    }

    public void OnGUI(in Rect rect)
    {
        var perceptionInfosRect =
            new Rect(rect.position + _CurPerInfoViewOffset, new Vector2(rect.width, rect.height / 2));
        GUI.BeginClip(perceptionInfosRect);
        DrawPerceptionInfosEditor(new Rect(Vector2.zero, perceptionInfosRect.size));
        GUI.EndClip();


        var decisionInfosRect = new Rect(
            perceptionInfosRect.x
            , perceptionInfosRect.yMax
            , perceptionInfosRect.width
            , rect.height - (perceptionInfosRect.height + _CurPerInfoViewOffset.y)
            );
        GUI.BeginClip(decisionInfosRect);
        DrawAiEditor(new Rect(Vector2.zero, decisionInfosRect.size));
        GUI.EndClip();
    }

    private void DrawAiEditor(in Rect parentRect)
    {
        if (_DecisionInfos == null)
            return;
        _DecisionInfos.DrawEditor(in parentRect);
    }

    private bool _PerInfoViewState = true;
    private Vector2 _PerInfoViewOffset;
    private Vector2 _CurPerInfoViewOffset;

    private Vector2 _CurPerInfoBtnSize;

    private void DrawPerceptionInfosEditor(in Rect rect)
    {
        var viewStateBtnRect = new Rect(0, 0, rect.width, 20);
        viewStateBtnRect.position = new Vector2()
        {
            x = rect.x,
            y = rect.yMax - viewStateBtnRect.height,
        };
        var perInfoContentRect = new Rect(rect)
        {
            height = rect.height - viewStateBtnRect.height,
        };
        if (_PerceptionInfos != null)
        {
            _PerceptionInfos.CalculateLocalNodeArea();
            _PerceptionInfos.DrawNodeArea(perInfoContentRect);
        }

        var preInfoBtnSize = EditorUtil.HasMouseInRect(viewStateBtnRect)
            ? Vector2.zero
            : new Vector2(0, -viewStateBtnRect.height);
        _CurPerInfoBtnSize = Vector2.Lerp(_CurPerInfoBtnSize, preInfoBtnSize, 0.03f);
        if (GUI.Button(new Rect(viewStateBtnRect.position, viewStateBtnRect.size + _CurPerInfoBtnSize), ""))
        {
            _PerInfoViewState = !_PerInfoViewState;
            _PerInfoViewOffset =
                _PerInfoViewState ? Vector2.zero : -new Vector2(0, rect.height - viewStateBtnRect.height);
        }

        _CurPerInfoViewOffset = Vector2.Lerp(_CurPerInfoViewOffset, _PerInfoViewOffset, 0.03f);
    }
}