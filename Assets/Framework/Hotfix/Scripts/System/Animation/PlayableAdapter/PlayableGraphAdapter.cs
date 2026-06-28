using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public enum EnAnimLayer
{
    None = -1,
    Base = 0,
    LowerBottom,
    LowerTop,
    Body,
    Bottom,
    Top,
    EnumCount,
}
public class PlayableGraphAdapter : IClassPool<PlayableGraphAdapterUserData>, IUpdate
{

    public static PlayableGraphAdapter Create(int entityID, Animator animator)
    {
        animator.applyRootMotion = false;
        var data = ClassPoolMgr.Instance.Pull<PlayableGraphAdapterUserData>();
        data.entityID = entityID;
        data.anim = animator;
        var playable = ClassPoolMgr.Instance.Pull<PlayableGraphAdapter>(data);
        ClassPoolMgr.Instance.Push(data);
        return playable;
    }
    public static void OnDestroy(PlayableGraphAdapter graph)
    {
        ClassPoolMgr.Instance.Push(graph);
    }
    public static implicit operator int(PlayableGraphAdapter adapter)
    {
        return adapter.GetEntityID();
    }

    private PlayableGraph _Graph = default;
    private Animator _Anim = null;
    private int _EntityID = -1;
    private AnimationLayerMixerPlayable _LayerMixerPlayable;
    private Dictionary<EnAnimLayer, LayerMixerInfo> _Layer2unusePortDic = new();
    private List<EnAnimLayer> _EnterLayerList = new();
    private List<EnAnimLayer> _ExistLayerList = new();
    //private AnimationScriptPlayable _PlayableJob;

    public void PoolConstructor()
    {
    }

    public void OnPoolInit(PlayableGraphAdapterUserData userData)
    {
        _EntityID = userData.entityID;
        _Anim = userData.anim;
        InitAnimatorGroup(userData.anim);
        UpdateMgr.Instance.Registener(this);
    }

    private void InitAnimatorGroup(Animator anim)
    {
        _Graph = PlayableGraph.Create($"custom-{anim.name}");
        _Graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        var output = AnimationPlayableOutput.Create(_Graph, $"{anim.name}-output", anim);
        //var jobData = new PlayableGraphAnimJob();
        //jobData.leftFootIKInfo.lowerLegHandle = anim.BindStreamTransform(anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
        //jobData.rightFootIKInfo.lowerLegHandle = anim.BindStreamTransform(anim.GetBoneTransform(HumanBodyBones.RightLowerLeg));
        //_PlayableJob = AnimationScriptPlayable.Create<PlayableGraphAnimJob>(_Graph, jobData);
        _LayerMixerPlayable = AnimationLayerMixerPlayable.Create(_Graph, (int)EnAnimLayer.EnumCount);
        //_PlayableJob.AddInput(_LayerMixerPlayable, 0, 1);
        //output.SetSourcePlayable(_PlayableJob);
        output.SetSourcePlayable(_LayerMixerPlayable);
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        _LayerMixerPlayable.Destroy();
        _Graph.Destroy();
        _EntityID = -1;
        _EnterLayerList.Clear();
        _ExistLayerList.Clear();
        _Layer2unusePortDic.Clear();
        _Anim = null;
        _Graph = default;
    }

    public void PoolRelease()
    {
    }
    private float GetLayerWeight(EnAnimLayer layer)
    {
        return _LayerMixerPlayable.GetInputWeight((int)layer);
    }
    private void SetLayerWeight(EnAnimLayer layer, float weight)
    {
        _LayerMixerPlayable.SetInputWeight((int)layer, weight);
    }
    private void SetLayerAdditive(EnAnimLayer layer, bool isAdditive)
    {
        _LayerMixerPlayable.SetLayerAdditive((uint)layer, isAdditive);
    }
    public PlayableGraph GetGraph()
    {
        return _Graph;
    }
    public int GetEntityID()
    {
        return _EntityID;
    }
    public void UpdtaeGraphEvaluate()
    {
        _Graph.Evaluate(ABBUtil.GetTimeDelta());
    }
    public bool IsPlaying()
    {
        return _Graph.IsPlaying();
    }
    public void PlayGraph()
    {
        _Graph.Play();
    }
    public void StopGraph()
    {
        _Graph.Stop();
    }
    public void PauseGraph()
    {

    }

    #region layermixerinfo
    public bool TryGetLayerMixerInfo(EnAnimLayer layer, out LayerMixerInfo info)
    {
        if (!_Layer2unusePortDic.TryGetValue(layer, out info))
            return false;
        return true;
    }
    #endregion

    #region layermixer

    public void AddMixerLayer(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return;
        _EnterLayerList.Add(layer);
        if (info.IsStatus(EnAnimLayerStatus.Exiting))
        {
            _ExistLayerList.Remove(layer);
        }
        info.SetStatus(EnAnimLayerStatus.Entering);
    }
    public void RemoveMixerLayer(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return;
        _ExistLayerList.Add(layer);
        if (info.IsStatus(EnAnimLayerStatus.Entering))
        {
            _EnterLayerList.Remove(layer);
        }
        info.SetStatus(EnAnimLayerStatus.Exiting);
    }

    private void DestroyLayerMixerInfo(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
            return;
        _LayerMixerPlayable.DisconnectInput((int)layer);
        _Layer2unusePortDic.Remove(layer);
        ClassPoolMgr.Instance.Push(layerInfo);
    }

    public LayerMixerInfo CreateLayerMixerInfo(EnAnimLayer layer)
    {
        var layerAdapter = ScriptPlayable<BridgePlayableAdapter>.Create(_Graph);
        var infoUserData = ClassPoolMgr.Instance.Pull<LayerMixerInfoUserData>();
        infoUserData.layer = layer;
        infoUserData.layerAdapter = layerAdapter;
        var info = ClassPoolMgr.Instance.Pull<LayerMixerInfo>(infoUserData);
        ClassPoolMgr.Instance.Push(infoUserData);
        _Layer2unusePortDic.Add(layer, info);
        _LayerMixerPlayable.ConnectInput((int)layer, layerAdapter, GlobalConfig.Int0, GlobalConfig.Int0);
        var avatar = AnimMgr.Instance.GetLayerAvatar(layer);
        if (avatar != null)
            _LayerMixerPlayable.SetLayerMaskFromAvatarMask((uint)layer, avatar);
        SetLayerAdditive(layer, false);
        return info;
    }
    public void DisconnectLayerInput(EnAnimLayer layer, int port)
    {
        if (!TryGetLayerMixerInfo(layer, out var layerInfo))
            return;
        if (!layerInfo.ContainsPortID(port))
            return;
        var playableAdapter = layerInfo.GetAdapter(port);
        playableAdapter.Complete();
        if (layerInfo.GetConnectCount() == GlobalConfig.Int1)
        {
            RemoveMixerLayer(layer);
            return;
        }
        layerInfo.Disconnect(port);
    }
    #endregion

    private void SetLayerStatus(EnAnimLayer layer, EnAnimLayerStatus status)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return;
        info.SetStatus(status);
    }
    public int GetConnectCount(EnAnimLayer layer)
    {
        if (!TryGetLayerMixerInfo(layer, out var info))
            return -1;
        var count = info.GetConnectCount();
        return count;
    }
    public void Update()
    {
        UpdateFootIK();
        UpdtaeGraphEvaluate();
        for (int i = 0; i < _EnterLayerList.Count; i++)
        {
            var layer = _EnterLayerList[i];
            var curWeight = GetLayerWeight(layer);
            if (curWeight == 1)
            {
                _EnterLayerList[i] = _EnterLayerList[^1];
                _EnterLayerList.RemoveAt(_EnterLayerList.Count - 1);
                i--;
                SetLayerStatus(layer, EnAnimLayerStatus.Playing);
                continue;
            }
            var toWeight = curWeight + ABBUtil.GetTimeDelta() * 5;
            var weight = Mathf.Clamp(toWeight, 0, 1);
            SetLayerWeight(layer, weight);
        }
        for (int i = 0; i < _ExistLayerList.Count; i++)
        {
            var layer = _ExistLayerList[i];
            var curWeight = GetLayerWeight(layer);
            if (curWeight == 0)
            {
                _ExistLayerList[i] = _ExistLayerList[^1];
                _ExistLayerList.RemoveAt(_ExistLayerList.Count - 1);
                i--;
                SetLayerStatus(layer, EnAnimLayerStatus.Nothing);
                if (!TryGetLayerMixerInfo(layer, out var layerInfo))
                    continue;
                layerInfo.DisconnectAll();
                DestroyLayerMixerInfo(layer);
                continue;
            }
            var toWeight = curWeight - ABBUtil.GetTimeDelta() * 5;
            var weight = Mathf.Clamp(toWeight, 0, 1);
            SetLayerWeight(layer, weight);
        }
    }
    public void SetApplyRootMotion(bool applyRootMotion)
    {
        //var job = _PlayableJob.GetJobData<PlayableGraphAnimJob>();
        //job.applyRootMotion = applyRootMotion;
        //_PlayableJob.SetJobData(job);

        _Anim.applyRootMotion = applyRootMotion;
    }

    float rayLine = 1f;

    private void UpdateLeftFootIK(ref FootIKInfo info)
    {
        var curSkillId = SceneEntityMgr.Instance.GetEntityCurSkillId(_EntityID);
        var skillCfg = GameSchedule.Instance.GetSkillCfg0((int)curSkillId);
        if (skillCfg.bIsIK <= 0)
        {
            info.weight = 0;
            return;
        }

        var legDir = info.direction.normalized;
        var leftPos = info.lastWorldPos;
        var startPos = leftPos + -legDir * 1;
        var dis = 10 + rayLine;
        Debug.DrawLine(startPos, startPos + legDir * dis, Color.red);
        //var count = Physics.RaycastNonAlloc(startPos, legDir, m_ArrHit, dis, (int)Mathf.Pow(2, (int)EnGameLayer.Terrain));
        //if (count > 0)
        //{
        //    var hit = m_ArrHit[0];
        if (Physics.Raycast(startPos, legDir, out var hit, dis, (int)Mathf.Pow(2, (int)EnGameLayer.Terrain)))
        {
            var pos = hit.point + hit.normal * 0.128f;
            var ikDis = Vector3.Distance(pos, leftPos);

            //DebugDrawMgr.Instance.DrawSphere(pos, 0.1f, 0.01f);

            var weight2 = pos.y < leftPos.y
                ? Mathf.Lerp(0, 1, 1 - Mathf.Pow(Mathf.Clamp01(ikDis / rayLine), 1))
                : 1;
            info.worldPos = pos;
            info.weight = weight2;


            var leftForward = info.lastQuaternion * Vector3.forward;
            var forward = Vector3.ProjectOnPlane(leftForward, hit.normal);
            var dir = Quaternion.LookRotation(forward, hit.normal);
            info.quaternion = dir;
        }
        else
        {
            info.weight = 0;
        }
    }
    private void UpdateFootIK()
    {
        //var job = _PlayableJob.GetJobData<PlayableGraphAnimJob>();

        //UpdateLeftFootIK(ref job.leftFootIKInfo);
        //UpdateLeftFootIK(ref job.rightFootIKInfo);

        //_PlayableJob.SetJobData(job);
    }


    public PlayableClipAdapter CreateClipPlayableAdapter(int clipID)
    {
        var clipData = ClassPoolMgr.Instance.Pull<PlayableClipAdapterUserData>();
        clipData.clipID = clipID;
        var clipPlayable = Create<PlayableClipAdapter>(clipData);
        ClassPoolMgr.Instance.Push(clipData);
        return clipPlayable;
    }
    public void DestroyPlayableAdapter(IPlayableAdapter playableAdapter)
    {
        IPlayableAdapter.Destroy(playableAdapter);
    }
    public PlayableMixerAdapter CreateMixerPlayableAdapter(IPlayableAdapter from, IPlayableAdapter to, float time, PlayableMixerCompleteAction complete)
    {
        var clipData = ClassPoolMgr.Instance.Pull<PlayableMixerAdapterUserData>();
        clipData.from = from;
        clipData.to = to;
        clipData.time = time;
        clipData.complete = complete;
        var clipPlayable = Create<PlayableMixerAdapter>(clipData);
        ClassPoolMgr.Instance.Push(clipData);
        return clipPlayable;
    }
    public T Create<T>()
        where T : IPlayableAdapter, new()
    {
        var playable = IPlayableAdapter.Create<T>(this);
        return playable;
    }
    public T Create<T>(PlayableAdapterUserData customData)
        where T : IPlayableAdapter, new()
    {
        var playable = IPlayableAdapter.Create<T>(this, customData);
        return playable;
    }

    public IPlayableAdapter Create(in EnTypeId typeId, PlayableAdapterUserData customData)
    {
        var playable = Create((int)typeId, customData);
        return playable;
    }
    public IPlayableAdapter Create(in int typeId, PlayableAdapterUserData customData)
    {
        var playable = IPlayableAdapter.Create(in typeId, this, customData);
        return playable;
    }
}
