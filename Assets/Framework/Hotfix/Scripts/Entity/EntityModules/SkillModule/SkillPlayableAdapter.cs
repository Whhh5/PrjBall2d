using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum EnCmdStep
{
    None = -1,
    Step0,
    Step1,
    Step2,
    Step3,
    Step4,
    Step5,
    Step6,
    Step7,
}
public class SkillPlayableAdapterUserData : PlayableAdapterUserData, ISerializeToIntArrayUserData
{
    public int startIndex { get; set; }
    public int count { get; set; }
    public int[] arrParams { get; set; }

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        startIndex
            = count
            = -1;
        arrParams = null;
    }
}
public interface ISkillPlayableAdapter : IPlayableAdapter, ISerializeToIntArray
{
    public void ExecutePlayable();
    public void RemovePlayable();
    public void ReexecutePlayable();
    public void CancelPlayable();
    public bool NextAnimLevelComdition();
}

public abstract class SkillPlayableAdapter<T> : PlayableAdapter<T>, ISkillPlayableAdapter, ISerializeToIntArray
    where T : SkillPlayableAdapterUserData, new()
{
    private bool _IsValid = false;

    public abstract EnTypeId GetTypeDefineId();
    public abstract List<int> SerializeToIntArray();
    public abstract void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData);
    public override void OnPoolInit(T userData)
    {
        base.OnPoolInit(userData);
        DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
    }
    public virtual void ExecutePlayable()
    {
        _IsValid = true;
    }
    public virtual void RemovePlayable()
    {
        _IsValid = false;
    }
    public virtual void ReexecutePlayable()
    {

    }
    public virtual void CancelPlayable()
    {

    }
    public virtual bool NextAnimLevelComdition()
    {
        return true;
    }
    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        return _IsValid;
    }

}
