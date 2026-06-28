using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class SkillPhysicsScheduleActionEditor : SkillPhysicsScheduleAction, ISkillScheduleActionEditor
{
    public void OnSkillEditorGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            DrawBaseField();

            DrawBuffInfo();
            DrawPhysics();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawBaseField()
    {
        _AtkSchedule = EditorGUILayout.Slider("触发进度", _AtkSchedule, 0f, 1f, GUILayout.Width(200));
        _AtkValue = EditorGUILayout.IntField("攻击参考值", _AtkValue, GUILayout.Width(200));
        EditorGuiUtil.DrawCfgField<EffectCfg>(_EffectID, effectId => _EffectID = effectId, 200);
    }

    private int _DrawBuffInfoIndex = -1;

    private void DrawBuffInfo()
    {
        EditorGuiUtil.DrawEnum<EnBuff>(buff =>
        {
            var buffInfoEditor = SkillEditorDefine.GetEntityBuffParamsEditor(buff);
            if (buffInfoEditor == null)
                return;
            var tempList = _BuffInfoList?.ToList() ?? new();
            tempList.Add(buffInfoEditor);
            _BuffInfoList = tempList.ToArray();
        }, "添加buff", null, GUILayout.Width(SkillEditorDefine.FieldWidth));

        if ((_BuffInfoList?.Length ?? 0) == 0)
            EditorGUILayout.HelpBox("选择一个buff类型", MessageType.Warning);
        else
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Button("", GUILayout.Width(SkillEditorDefine.LineSize));
                GUILayout.Space(SkillEditorDefine.DefineSpace);

                SkillEditorUtil.SkillShowBuffMenu(_BuffInfoList, _DrawBuffInfoIndex,
                    selectIndex => _DrawBuffInfoIndex = selectIndex, buffInfos => _BuffInfoList = buffInfos);
                GUILayout.Space(SkillEditorDefine.DefineSpace);

                if (_DrawBuffInfoIndex >= 0 && _DrawBuffInfoIndex < _BuffInfoList?.Length)
                    _BuffInfoList[_DrawBuffInfoIndex].OnSkillEditorGUI();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawPhysics()
    {
        EditorGuiUtil.DrawEnum<EnSkillPhysicsType>(physicsType =>
        {
            if (physicsType == _PhysicsResolve?.GetSkillPhysicsType())
                return;

            var physicsResolve = SkillEditorDefine.GetPhysicsResolveEditor(physicsType);
            if (physicsResolve == null && _PhysicsResolve != null)
                if (!EditorUtility.DisplayDialog("提示", "移除物理检测", "ok", "cancel"))
                    return;

            if (physicsResolve != null && _PhysicsResolve != null)
                if (!EditorUtility.DisplayDialog("提示", "更换物理检测", "ok", "cancel"))
                    return;

            if (_PhysicsResolve != null)
                SkillEditorDefine.RecycleEditorInstance(_PhysicsResolve);

            _PhysicsResolve = physicsResolve;
        }, "添加物理检测", null, GUILayout.Width(SkillEditorDefine.FieldWidth));
        if (_PhysicsResolve == null)
            EditorGUILayout.HelpBox("选择一个物理检测类型", MessageType.Error);
        else
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Button("", GUILayout.Width(SkillEditorDefine.LineSize), GUILayout.ExpandHeight(true));
                GUILayout.Space(SkillEditorDefine.DefineSpace);
                _PhysicsResolve.OnSkillEditorGUI();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    [EditorField(nameof(_AtkSchedule))]
    private float _AtkSchedule
    {
        get => this.GetFieldValue(nameof(_AtkSchedule), 0f);
        set => this.SetFieldValue(nameof(_AtkSchedule), value);
    }

    [EditorField(nameof(_AtkValue))]
    private int _AtkValue
    {
        get => this.GetFieldValue(nameof(_AtkValue), 0);
        set => this.SetFieldValue(nameof(_AtkValue), value);
    }

    [EditorField(nameof(_EffectID))]
    private int _EffectID
    {
        get => this.GetFieldValue(nameof(_EffectID), 0);
        set => this.SetFieldValue(nameof(_EffectID), value);
    }

    [EditorField(nameof(_BuffInfoList))]
    private IEntityBuffParamsEditor[] _BuffInfoList
    {
        get => this.GetFieldValue(nameof(_BuffInfoList), default(IEntityBuffParamsEditor[]));
        set => this.SetFieldValue(nameof(_BuffInfoList), value);
    }

    [EditorField(nameof(_PhysicsResolve))]
    private IPhysicsResolveEditor _PhysicsResolve
    {
        get => this.GetFieldValue(nameof(_PhysicsResolve), default(IPhysicsResolveEditor));
        set => this.SetFieldValue(nameof(_PhysicsResolve), value);
    }
}