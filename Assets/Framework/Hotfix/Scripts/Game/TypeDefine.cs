
public enum EnTypeId
{
    None = 0,

    ____________________________________________________________VirtualTimelineStart = 1000,
    VirtualSkillTimeline,
    _______________________________VirtualTimelineEventStart = 1100,
    VirtualTimelineDefaultEvent,

    ____________________________________________________________SkillTypeStart = 2000,
    SkillVirtualTimelineNodePlayableInfo,
    SkillVirtualTimelineEventPlayableInfo,
    _______________________________SkillVirtualTimelineEventStart = 2100,
    SkillTimelinePhysicsSphereEvent,
    SkillTimelinePhysicsCylinderEvent,
    SkillTimelineAttackCylinderEvent,
    SkillTypeSingletonPlayableAdapter,
    LayerMixerCmdPlayableAdapter,
    SkillLinkPlayableAdapter,
    SkillLoopPlayableAdapter,
    SkillRandomPlayableAdapter,
    SkillTypeSelectPlayableAdapter,
    _______________________________SkillPlayableNodeStart = 2200,
    SkillVirtualTimelineAnimationPlayableNode,
    _______________________________SkillPlayableItemDataStart = 2300,
    SkillItemInfo,
    SkillTypeLoopData,
    SkillTypeSelectData,
    SkillTypeSelectItemInfo,
    PhysicsResolveBox,
    PhysicsResolveSphere,
    SkillHeightBehaviourData,
    CompareLessInfo,
    CompareGreaterInfo,
    CompareEqualInfo,
    EntityBuffTimeDefaultInfo,
    EntityMoveDownBuffUserData,
    PhysicsResolveCylinder,
    EntityNoMovementBuffData,
    EntityNoRotationBuffData,
    _______________________________SkillScheduleStart = 2400,
    SkillBehaviourScheduleAction,
    SkillBuffScheduleAction,
    SkillEffectScheduleAction,
    SkillPhysicsScheduleAction,
    SkillFlyItemScheduleAction,


    ____________________________________________________________EntityTypeIdStart = 3000,
    PlayerEntityData,
    DefaultSceneProjectileEntityData,
    _______________________________Attachment = 3100,
    MapChunkAttachment_TowerEntityData,
    _______________________________Monster = 3200,
    MonsterEntityData,
    MonsterAnnaEntityData,
    MonsterAshuEntityData,




    ____________________________________________________________Ai = 4000,
    _______________________________AiDecision = 4100,
    AiDefaultDecision,
    AiTransformDecisionSubGenerate,
    AiDirectionDecisionSubGenerator,
    _______________________________AiDecisionGenerator = 4200,
    AiComAtkDecisionGenerator,
    AiIdleDecisionGenerator,
    AiCmdDecisionGenerator,
    _______________________________AiCondition = 4300,
    AiDistanceCondition,
    AiRotationCondition,
    _______________________________AiMisc = 4400,
    AiPerceptionMatchInfo,
    AiDecisionGeneratorInfo,
    _______________________________AiPerception = 4500,
    AiRandomEntityPerception,
    AiPerceptionInfos,
    AiDecisionInfos,
    _______________________________AiPerceptionTest = 4600,
    AiPerceptionLayerTest,
    AiPerceptionDistanceTest,
}
public static class TypeDefine
{
    public static IClassPool CreateInstance(in int typeId, in IClassPoolUserData userData)
    {
        return (EnTypeId)typeId switch
        {
            EnTypeId.None => null,


            EnTypeId.SkillVirtualTimelineNodePlayableInfo => ClassPoolMgr.Instance.Pull<SkillVirtualTimelineNodePlayableInfo>(userData),

            EnTypeId.SkillVirtualTimelineEventPlayableInfo => ClassPoolMgr.Instance.Pull<SkillVirtualTimelineEventInfo>(userData),


            EnTypeId.VirtualSkillTimeline => ClassPoolMgr.Instance.Pull<SkillVirtualTimeline>(userData),
            EnTypeId.VirtualTimelineDefaultEvent => ClassPoolMgr.Instance.Pull<VirtualTimelineDefaultEvent>(userData),

            EnTypeId.SkillTimelinePhysicsSphereEvent => ClassPoolMgr.Instance.Pull<SkillTimelinePhysicsSphereEvent>(userData),
            EnTypeId.SkillTimelinePhysicsCylinderEvent => ClassPoolMgr.Instance.Pull<SkillTimelinePhysicsCylinderEvent>(userData),
            EnTypeId.SkillTimelineAttackCylinderEvent => ClassPoolMgr.Instance.Pull<SkillTimelineAttackCylinderEvent>(userData),
            EnTypeId.SkillTypeSingletonPlayableAdapter => ClassPoolMgr.Instance.Pull<SkillSingletonPlayableAdapter>(userData),
            EnTypeId.LayerMixerCmdPlayableAdapter => ClassPoolMgr.Instance.Pull<LayerMixerCmdPlayableAdapter>(userData),
            EnTypeId.SkillLinkPlayableAdapter => ClassPoolMgr.Instance.Pull<SkillLinkPlayableAdapter>(userData),
            EnTypeId.SkillLoopPlayableAdapter => ClassPoolMgr.Instance.Pull<SkillLoopPlayableAdapter>(userData),
            EnTypeId.SkillRandomPlayableAdapter => ClassPoolMgr.Instance.Pull<SkillRandomPlayableAdapter>(userData),
            EnTypeId.SkillTypeSelectPlayableAdapter => ClassPoolMgr.Instance.Pull<SkillTypeSelectPlayableAdapter>(userData),

            EnTypeId.SkillVirtualTimelineAnimationPlayableNode => ClassPoolMgr.Instance.Pull<SkillVirtualTimelineAnimationPlayableNode>(userData),

            EnTypeId.SkillItemInfo => ClassPoolMgr.Instance.Pull<SkillStageInfo>(userData),
            EnTypeId.SkillTypeLoopData => ClassPoolMgr.Instance.Pull<SkillTypeLoopData>(userData),
            EnTypeId.SkillTypeSelectData => ClassPoolMgr.Instance.Pull<SkillTypeSelectData>(userData),
            EnTypeId.SkillTypeSelectItemInfo => ClassPoolMgr.Instance.Pull<SkillTypeSelectItemInfo>(userData),
            EnTypeId.PhysicsResolveBox => ClassPoolMgr.Instance.Pull<PhysicsResolveBox>(userData),
            EnTypeId.PhysicsResolveSphere => ClassPoolMgr.Instance.Pull<PhysicsResolveSphere>(userData),
            EnTypeId.CompareLessInfo => ClassPoolMgr.Instance.Pull<CompareLessInfo>(userData),
            EnTypeId.CompareGreaterInfo => ClassPoolMgr.Instance.Pull<CompareGreaterInfo>(userData),
            EnTypeId.CompareEqualInfo => ClassPoolMgr.Instance.Pull<CompareEqualInfo>(userData),
            EnTypeId.PhysicsResolveCylinder => ClassPoolMgr.Instance.Pull<PhysicsResolveCylinder>(userData),
            EnTypeId.EntityNoMovementBuffData => ClassPoolMgr.Instance.Pull<EntityNoMovementBuffData>(userData),

            EnTypeId.SkillBehaviourScheduleAction => ClassPoolMgr.Instance.Pull<SkillBehaviourScheduleAction>(userData),
            EnTypeId.SkillBuffScheduleAction => ClassPoolMgr.Instance.Pull<SkillBuffScheduleAction>(userData),
            EnTypeId.SkillEffectScheduleAction => ClassPoolMgr.Instance.Pull<SkillEffectScheduleAction>(userData),
            EnTypeId.SkillPhysicsScheduleAction => ClassPoolMgr.Instance.Pull<SkillPhysicsScheduleAction>(userData),
            EnTypeId.SkillFlyItemScheduleAction => ClassPoolMgr.Instance.Pull<SkillFlyItemScheduleAction>(userData),


            EnTypeId.AiDefaultDecision => ClassPoolMgr.Instance.Pull<AiDefaultDecision>(userData),
            EnTypeId.AiTransformDecisionSubGenerate => ClassPoolMgr.Instance.Pull<AiTransformDecisionSubGenerator>(userData),
            EnTypeId.AiDirectionDecisionSubGenerator => ClassPoolMgr.Instance.Pull<AiLookAtDecisionSubGenerator>(userData),


            EnTypeId.AiComAtkDecisionGenerator => ClassPoolMgr.Instance.Pull<AiCmdDecisionGenerator>(userData),
            EnTypeId.AiIdleDecisionGenerator => ClassPoolMgr.Instance.Pull<AiIdleDecisionGenerator>(userData),

            EnTypeId.AiDistanceCondition => ClassPoolMgr.Instance.Pull<AiDistanceCondition>(userData),
            EnTypeId.AiRotationCondition => ClassPoolMgr.Instance.Pull<AiRotationCondition>(userData),

            EnTypeId.AiPerceptionMatchInfo => ClassPoolMgr.Instance.Pull<AiPerceptionMatchInfo>(userData),
            EnTypeId.AiDecisionGeneratorInfo => ClassPoolMgr.Instance.Pull<AiDecisionGeneratorInfo>(userData),

            EnTypeId.AiRandomEntityPerception => ClassPoolMgr.Instance.Pull<AiRandomEntityPerception>(userData),
            EnTypeId.AiPerceptionInfos => ClassPoolMgr.Instance.Pull<AiPerceptionInfos>(userData),
            EnTypeId.AiDecisionInfos => ClassPoolMgr.Instance.Pull<AiDecisionInfo>(userData),

            EnTypeId.AiPerceptionLayerTest => ClassPoolMgr.Instance.Pull<AiPerceptionLayerTest>(userData),
            EnTypeId.AiPerceptionDistanceTest => ClassPoolMgr.Instance.Pull<AiPerceptionDistanceTest>(userData),
            _ => null,
        };
    }
    public static T CreateInstance<T>(in int typeId, in IClassPoolUserData userData)
        where T : IClassPool
    {
        var result = CreateInstance(in typeId, in userData);
        return (T)result;
    }
    public static void DestroyInstance(in IClassPool instance)
    {
        ClassPoolMgr.Instance.Push(instance);
    }
}
