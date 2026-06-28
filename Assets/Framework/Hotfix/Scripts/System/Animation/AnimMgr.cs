using UnityEngine;
using System.Collections.Generic;

public enum EnAnimClip
{
    None,
    Battle_idle,

}
public class AnimMgr : Singleton<AnimMgr>
{
    public override EnManagerFuncType FuncType => base.FuncType;
    public AnimationClip GetClip(int clipID)
	{
        var clipCfg = GameSchedule.Instance.GetClipCfg0(clipID);
		var clip = ABBLoadMgr.Instance.Load<AnimationClip>(clipCfg.nAssetID);
		return clip;
	}
	public void RecycleClip(int clipID)
	{

	}
    public AvatarMask GetLayerAvatar(EnAnimLayer layerAvatarID)
    {
        var avatarCfg = GameSchedule.Instance.GetAnimLayerCfg0((int)layerAvatarID);
        var mask = ABBLoadMgr.Instance.Load<AvatarMask>(avatarCfg.nAssetID);
        return mask;
    }

	public EnAnimLayer GetAnimLayer(int clipID)
	{
        var clipCfg = GameSchedule.Instance.GetClipCfg0(clipID);
		return (EnAnimLayer)clipCfg.nLayer;
	}
	public bool GetAnimIsLoop(int clipID)
    {
        var clipCfg = GameSchedule.Instance.GetClipCfg0(clipID);
        return clipCfg.bIsLoop > 0;
    }
}

