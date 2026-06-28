using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public struct AnimJob : IAnimationJob
{
    public NativeArray<TransformStreamHandle> handles;
    public float weight;
    public void ProcessAnimation(AnimationStream stream)
    {
        var hum = stream.AsHuman();
        var stream1 = stream.GetInputStream(0);
        var stream2 = stream.GetInputStream(1);

        foreach (var handle in handles)
        {
            var pos1 = handle.GetLocalPosition(stream1);
            var pos2 = handle.GetLocalPosition(stream2);
            var pos = Vector3.Lerp(pos1, pos2, weight);
            handle.SetLocalPosition(stream, pos);

            var angle1 = handle.GetLocalRotation(stream1);
            var angle2 = handle.GetLocalRotation(stream2);
            var angle = Quaternion.Slerp(angle1, angle2, weight);
            handle.SetLocalRotation(stream, angle);
        }

        //hum.SolveIK();
    }

    public void ProcessRootMotion(AnimationStream stream)
    {
        var stream1 = stream.GetInputStream(0);
        var stream2 = stream.GetInputStream(1);

        stream.velocity = Vector3.Lerp(stream1.rootMotionPosition, stream2.rootMotionPosition, weight);
        stream.angularVelocity = Vector3.Lerp(stream1.angularVelocity, stream2.angularVelocity, weight);
    }
}

public interface IPlayableAdapter : IClassPool
{
    public static T Create<T>(PlayableGraphAdapter graph)
        where T : IPlayableAdapter, new()
    {
        var userData = ClassPoolMgr.Instance.Pull<PlayableAdapterUserData>();
        var adapter = Create<T>(graph, userData);
        ClassPoolMgr.Instance.Push(userData);
        return adapter;
    }
    public static T Create<T>(PlayableGraphAdapter graph, PlayableAdapterUserData customData)
        where T : IPlayableAdapter, new()
    {
        var rootPlayable = ScriptPlayable<BridgePlayableAdapter>.Create(graph.GetGraph(), GlobalConfig.Int0);
        customData.rootPlayable = rootPlayable;
        customData.graph = graph;
        var adapter = ClassPoolMgr.Instance.Pull<T>(customData);
        rootPlayable.GetBehaviour().Initialization(adapter);
        return adapter;
    }
    public static IPlayableAdapter Create(in int typeId, PlayableGraphAdapter graph, PlayableAdapterUserData customData)
    {
        var rootPlayable = ScriptPlayable<BridgePlayableAdapter>.Create(graph.GetGraph(), GlobalConfig.Int0);
        customData.rootPlayable = rootPlayable;
        customData.graph = graph;
        var adapter = TypeDefine.CreateInstance<IPlayableAdapter>(in typeId, customData);
        rootPlayable.GetBehaviour().Initialization(adapter);
        return adapter;
    }
    public static IPlayableAdapter Create(in EnTypeId typeId, PlayableGraphAdapter graph, PlayableAdapterUserData customData)
    {
        var adapter = Create((int)typeId, graph, customData);
        return adapter;
    }
    public static void Destroy(IPlayableAdapter adapter)
    {
        ClassPoolMgr.Instance.Push(adapter);
    }
    public PlayableGraphAdapter GetGraph();
    public void DisconnectRootAdapter(int inputPort = GlobalConfig.Int0);
    public void ConnectRootAdapter(IPlayableAdapter playable);
    public void ConnectRootAdapter(int inputPort, IPlayableAdapter playable, int sourceOutput = GlobalConfig.Int0, float weight = GlobalConfig.Float1);
    public int AddConnectRootAdapter(IPlayableAdapter playable, int sourceOutput = GlobalConfig.Int0, float weight = GlobalConfig.Float1);
    public float GetPlaySchedule01();
    public float GetPlaySchedule();
    public ref readonly Playable GetPlayable();
    public bool GetIsValid();
    public float GetUnitTime();
    public IPlayableAdapter GetMainPlayableAdapter();
    public float GetPlayTime();
    public EnAnimLayer GetOutputLayer();
    public int GetSkillId();
    public void SetSkillId(int skillId);
    public void Complete();
    public void SetSpeed(float speed);
    public bool IsPlayEnd();
    public bool IsLoop();

    // life
    public void OnPlayableCreate();
    public void OnGraphStart(Playable playable);
    public void OnGraphStop(Playable playable);
    public void OnPlayableDestroy(Playable playable);
    public void OnBehaviourPlay(Playable playable, FrameData info);
    public void OnBehaviourPause(Playable playable, FrameData info);
    public void PrepareData(Playable playable, FrameData info);
    public bool OnPrepareFrame(Playable playable, FrameData info);
    public void ProcessFrame(Playable playable, FrameData info, object playerData);

}

public abstract class PlayableAdapter<T> : IPlayableAdapter, IClassPool<T>
    where T : PlayableAdapterUserData
{
    public static implicit operator Playable(PlayableAdapter<T> playable)
    {
        return playable.GetPlayable();
    }
    private bool _IsValid = false;
    private Playable _RootPlayable;
    protected PlayableGraphAdapter _Graph = null;
    private int _SkillId = -1;

    public virtual void OnPoolDestroy()
    {
        _RootPlayable.Destroy();
        _IsValid = false;
        _Graph = null;
        _SkillId = -1;
    }
    public virtual void OnPoolInit(T userData)
    {
        _IsValid = true;
        _Graph = userData.graph;
        _RootPlayable = userData.rootPlayable;
    }
    public virtual void PoolConstructor()
    {

    }

    public void OnPoolEnable()
    {

    }


    public virtual void PoolRelease()
    {
    }
    public virtual IPlayableAdapter GetMainPlayableAdapter()
    {
        return this;
    }
    public PlayableGraphAdapter GetGraph()
    {
        return _Graph;
    }
    public void DisconnectRootAdapter(int inputPort = GlobalConfig.Int0)
    {
        ref readonly var rootPlayable = ref GetPlayable();
        rootPlayable.DisconnectInput(inputPort);
    }
    public void ConnectRootAdapter(IPlayableAdapter playable)
    {
        ConnectRootAdapter(GlobalConfig.Int0, playable);
    }
    public void ConnectRootAdapter(int inputPort, IPlayableAdapter playable, int sourceOutput = GlobalConfig.Int0, float weight = GlobalConfig.Float1)
    {
        ref readonly var rootPlayable = ref GetPlayable();
        rootPlayable.ConnectInput(inputPort, playable.GetPlayable(), sourceOutput, weight);
    }
    public int AddConnectRootAdapter(IPlayableAdapter playable, int sourceOutput = GlobalConfig.Int0, float weight = GlobalConfig.Float1)
    {
        ref readonly var mainPlayable = ref playable.GetPlayable();
        var port = AddConnectRootAdapter(in mainPlayable, sourceOutput, weight);
        return port;
    }
    public int AddConnectRootAdapter(in Playable playable, int sourceOutput = GlobalConfig.Int0, float weight = GlobalConfig.Float1)
    {
        ref readonly var rootPlayable = ref GetPlayable();
        var port = rootPlayable.AddInput(playable, sourceOutput, weight);
        return port;
    }
    public virtual bool GetIsValid()
    {
        return _IsValid;
    }
    public virtual ref readonly Playable GetPlayable()
    {
        return ref _RootPlayable;
    }
    public void SetSpeed(float speed)
    {
        ref readonly var playable = ref GetPlayable();
        playable.SetSpeed(speed);
    }
    public double GetSpeed()
    {
        ref readonly var playable = ref GetPlayable();
        var speed = playable.GetSpeed();
        return speed;
    }
    public virtual void Complete()
    {
        SetSpeed(0);
    }
    // 是否循环
    public virtual bool IsLoop()
    {
        return false;
    }
    // 循环一次需要的时间
    public abstract float GetUnitTime();
    public abstract float GetPlayTime();
    public float GetPlaySchedule()
    {
        var schedule = GetPlayTime() / GetUnitTime();
        return schedule;
    }
    public float GetPlaySchedule01()
    {
        var schedule = GetPlaySchedule();
        return Mathf.Clamp01(schedule);
    }
    public virtual int GetPlayCount()
    {
        var schedule = GetPlaySchedule();
        return Mathf.FloorToInt(schedule);
    }
    public abstract EnAnimLayer GetOutputLayer();
    // 是否播放结束
    public virtual bool IsPlayEnd()
    {
        var isEnd = GetPlaySchedule01() == 1;
        return isEnd;
    }
    public void SetSkillId(int skillId)
    {
        _SkillId = skillId;
    }
    public virtual int GetSkillId()
    {
        return _SkillId;
    }

    public virtual void OnPlayableCreate()
    {
    }

	public virtual void OnGraphStart(Playable playable)
    {
    }

    public virtual void OnGraphStop(Playable playable)
    {
    }

    public virtual void OnPlayableDestroy(Playable playable)
    {
    }

    public virtual void OnBehaviourPlay(Playable playable, FrameData info)
    {
    }

    public virtual void OnBehaviourPause(Playable playable, FrameData info)
    {
    }

    public virtual void PrepareData(Playable playable, FrameData info)
    {
    }

    public virtual bool OnPrepareFrame(Playable playable, FrameData info)
    {
        return true;
    }

    public virtual void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
    }


    // 显式转换从double到Fahrenheit    
    //public static explicit operator Playable(PlayableAdapter playable)
    //{
    //    Playable p = playable;
    //    return new Fahrenheit(d);
    //}
}