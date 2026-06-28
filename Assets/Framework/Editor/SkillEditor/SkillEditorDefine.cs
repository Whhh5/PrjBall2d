using System;
using System.Collections.Generic;
using UnityEngine;

public static class SkillEditorDefine
{
    public const float LineSize = 3;
    public const float TabWidth = 20;
    public const float DefineSpace = 10;
    public const int FieldWidth = 200;
    public const float MenuWidth = 100;
    public static Color BoxDefaultColor = new Color(1, 1, 1, 0.1f);
    public const float SelectLineLength = 20f;


    private static readonly Dictionary<Type, Type> _EditorTypeDict = new()
    {
        #region Skill

        { typeof(SkillLinkPlayableAdapter), typeof(SkillLinkPlayableAdapterEditor) },
        { typeof(SkillLoopPlayableAdapter), typeof(SkillLoopPlayableAdapterEditor) },
        { typeof(SkillRandomPlayableAdapter), typeof(SkillRandomPlayableAdapterEditor) },
        { typeof(SkillStageInfo), typeof(SkillStageInfoEditor) },

        { typeof(EntityNoMovementBuffData), typeof(EntityNoMovementBuffDataEditor) },

        { typeof(SkillPhysicsScheduleAction), typeof(SkillPhysicsScheduleActionEditor) },
        { typeof(SkillBuffScheduleAction), typeof(SkillBuffScheduleActionEditor) },
        { typeof(SkillFlyItemScheduleAction), typeof(SkillFlyItemScheduleActionEditor) },
        { typeof(SkillEffectScheduleAction), typeof(SkillEffectScheduleActionEditor) },
        { typeof(PhysicsResolveBox), typeof(PhysicsResolveBoxEditor) },
        { typeof(PhysicsResolveSphere), typeof(PhysicsResolveSphereEditor) },
        { typeof(PhysicsResolveCylinder), typeof(PhysicsResolveCylinderEditor) },

        { typeof(IEntityBuffParams), typeof(IEntityBuffParamsEditor) },
        { typeof(ISkillScheduleAction), typeof(ISkillScheduleActionEditor) },
        { typeof(IPhysicsResolve), typeof(IPhysicsResolveEditor) },

        #endregion

        #region Ai

        { typeof(AiCmdDecisionGenerator), typeof(AiCmdDecisionGeneratorEditor) },
        { typeof(AiTransformDecisionSubGenerator), typeof(AiTransformDecisionSubGeneratorEditor) },
        { typeof(AiLookAtDecisionSubGenerator), typeof(AiLookAtDecisionSubGeneratorEditor) },
        { typeof(AiDecisionGeneratorInfo), typeof(AiDecisionGeneratorInfoEditor) },
        { typeof(AiPerceptionMatchInfo), typeof(AiPerceptionMatchInfoEditor) },
        { typeof(AiDecisionInfo), typeof(AiDecisionInfosEditor) },
        { typeof(AiDefaultDecision), typeof(AiDefaultDecisionEditor) },
        { typeof(AiDistanceCondition), typeof(AiDistanceConditionEditor) },
        { typeof(AiRotationCondition), typeof(AiRotationConditionEditor) },
        { typeof(AiRandomEntityPerception), typeof(AiRandomEntityPerceptionEditor) },
        { typeof(AiPerceptionDistanceTest), typeof(AiPerceptionDistanceTestEditor) },
        { typeof(AiPerceptionLayerTest), typeof(AiPerceptionLayerTestEditor) },
        { typeof(AiIdleDecisionGenerator), typeof(AiIdleDecisionGeneratorEditor) },

        { typeof(IAiCondition), typeof(IAiConditionEditor) },
        { typeof(IAiPerceptionTest), typeof(IAiPerceptionTestEditor) },
        { typeof(IAiDecisionSubGenerator), typeof(IAiDecisionSubGeneratorEditor) },
        { typeof(IAiDecisionGenerator), typeof(IAiDecisionGeneratorEditor) },
        { typeof(IAiDecision), typeof(IAiDecisionEditor) },
        { typeof(IAiPerceptionMatchInfo), typeof(IAiPerceptionMatchInfoEditor) },
        { typeof(IAiPerception), typeof(IAiPerceptionEditor) },

        #endregion
    };


    public static Type GetEditorType(Type type)
    {
        return _EditorTypeDict[type];
    }

    #region Ai

    public static IAiDecisionEditor CreateAiDecisionEditor(EnAiDecisionId decisionId)
    {
        var editorType = decisionId switch
        {
            EnAiDecisionId.Default => GetEditorType(typeof(AiDefaultDecision)),
            _ => null,
        };
        var ins = Activator.CreateInstance(editorType);
        var editor = ins as IAiDecisionEditor;
        return editor;
    }

    public static IAiDecisionGeneratorEditor CreateAiDecisionGeneratorEditor(
        EnAiDecisionGeneratorType decisionGeneratorType)
    {
        var editorType = decisionGeneratorType switch
        {
            EnAiDecisionGeneratorType.Cmd => GetEditorType(typeof(AiCmdDecisionGenerator)),
            EnAiDecisionGeneratorType.Idle => GetEditorType(typeof(AiIdleDecisionGenerator)),
            _ => null,
        };
        var ins = Activator.CreateInstance(editorType);
        var editor = ins as IAiDecisionGeneratorEditor;
        return editor;
    }

    public static IAiDecisionSubGeneratorEditor CreateAiDecisionSubGeneratorEditor(EnAiDecisionSubGeneratorType type)
    {
        var editorType = type switch
        {
            EnAiDecisionSubGeneratorType.Transform => GetEditorType(typeof(AiTransformDecisionSubGenerator)),
            EnAiDecisionSubGeneratorType.LookAt => GetEditorType(typeof(AiLookAtDecisionSubGenerator)),
            _ => null,
        };
        var ins = Activator.CreateInstance(editorType);
        var editor = ins as IAiDecisionSubGeneratorEditor;
        return editor;
    }

    public static IAiConditionEditor CreateAiConditionEditor(EnAiConditionType type)
    {
        var editorType = type switch
        {
            EnAiConditionType.Distance => GetEditorType(typeof(AiDistanceCondition)),
            EnAiConditionType.Rotation => GetEditorType(typeof(AiRotationCondition)),
            _ => null,
        };
        var ins = Activator.CreateInstance(editorType);
        var editor = ins as IAiConditionEditor;
        return editor;
    }

    public static IAiPerceptionMatchInfoEditor CreateAiPerceptionMatchInfoEditor(EnAiPerceptionMatchInfoType type)
    {
        var editorType = type switch
        {
            EnAiPerceptionMatchInfoType.AiPerceptionMatchInfo => GetEditorType(typeof(AiPerceptionMatchInfo)),
            _ => null,
        };
        var ins = Activator.CreateInstance(editorType);
        var editor = ins as IAiPerceptionMatchInfoEditor;
        return editor;
    }

    public static IAiPerceptionTestEditor CreateAiPerceptionTestEditor(EnAiPerceptionTestType type)
    {
        var editorType = type switch
        {
            EnAiPerceptionTestType.Distance => GetEditorType(typeof(AiPerceptionDistanceTest)),
            EnAiPerceptionTestType.Layer => GetEditorType(typeof(AiPerceptionLayerTest)),
            _ => null,
        };
        var ins = Activator.CreateInstance(editorType);
        var editor = ins as IAiPerceptionTestEditor;
        return editor;
    }

    public static IAiPerceptionEditor CreateAiPerceptionEditor(EnAiPerceptionId type)
    {
        var editorType = type switch
        {
            EnAiPerceptionId.RandomEntity => GetEditorType(typeof(AiRandomEntityPerception)),
            _ => null,
        };
        var ins = Activator.CreateInstance(editorType);
        var editor = ins as IAiPerceptionEditor;
        return editor;
    }

    #endregion

    #region Skill

    public static ISkillEditor GetPlayableAdapterEditor(EnTypeId typeId)
    {
        var editorType = typeId switch
        {
            EnTypeId.SkillLinkPlayableAdapter => GetEditorType(typeof(SkillLinkPlayableAdapter)),
            EnTypeId.SkillLoopPlayableAdapter => GetEditorType(typeof(SkillLoopPlayableAdapter)),
            EnTypeId.SkillRandomPlayableAdapter => GetEditorType(typeof(SkillRandomPlayableAdapter)),
            _ => null
        };
        var ins = Activator.CreateInstance(editorType);
        var result = ins as ISkillEditor;
        SkillEditorWindow.EditorInstanceInitialize(result);
        return result;
    }

    public static IEntityBuffParamsEditor GetEntityBuffParamsEditor(EnBuff buff)
    {
        var buffType = buff switch
        {
            EnBuff.NoMovement => GetEditorType(typeof(EntityNoMovementBuffData)),
            _ => null,
        };
        if (buffType == null)
            return null;
        var ins = Activator.CreateInstance(buffType);
        var result = ins as IEntityBuffParamsEditor;
        SkillEditorWindow.EditorInstanceInitialize(result);
        return result;
    }

    public static ISkillScheduleActionEditor GetSkillScheduleActionEditor(EnAtkLinkScheculeType scheduleType)
    {
        var scheduleTypeType = scheduleType switch
        {
            EnAtkLinkScheculeType.Physics => GetEditorType(typeof(SkillPhysicsScheduleAction)),
            EnAtkLinkScheculeType.Buff => GetEditorType(typeof(SkillBuffScheduleAction)),
            EnAtkLinkScheculeType.Effect => GetEditorType(typeof(SkillEffectScheduleAction)),
            EnAtkLinkScheculeType.FlyItem => GetEditorType(typeof(SkillFlyItemScheduleAction)),
            _ => null,
        };
        var ins = Activator.CreateInstance(scheduleTypeType);
        var result = ins as ISkillScheduleActionEditor;
        SkillEditorWindow.EditorInstanceInitialize(result);
        return result;
    }

    public static IPhysicsResolveEditor GetPhysicsResolveEditor(EnSkillPhysicsType physicsType)
    {
        var physicsTypeType = physicsType switch
        {
            EnSkillPhysicsType.Box => GetEditorType(typeof(PhysicsResolveBox)),
            EnSkillPhysicsType.Sphere => GetEditorType(typeof(PhysicsResolveSphere)),
            EnSkillPhysicsType.Cylinder => GetEditorType(typeof(PhysicsResolveCylinder)),
            _ => null,
        };
        var ins = Activator.CreateInstance(physicsTypeType);
        var result = ins as IPhysicsResolveEditor;
        SkillEditorWindow.EditorInstanceInitialize(result);
        return result;
    }

    public static void RecycleEditorInstance(IClassPool obj)
    {
        if (obj is ISkillEditor skillEditor)
            SkillEditorWindow.EditorInstanceDestroy(skillEditor);
    }

    #endregion
}