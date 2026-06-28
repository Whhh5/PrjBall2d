
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;


public struct FootIKInfo
{
    public TransformStreamHandle lowerLegHandle;
    public float weight;

    public Vector3 worldPos;
    public Quaternion quaternion;
    public Vector3 direction;
    public Vector3 lastWorldPos;
    public Quaternion lastQuaternion;


    public float curWeight;
}
public struct PlayableGraphAnimJob : IAnimationJob
{
    public bool isActivity;
    public bool applyRootMotion;
    public float lastOffsetY;
    public TransformStreamHandle rootHandle;

    public FootIKInfo leftFootIKInfo;
    public FootIKInfo rightFootIKInfo;
    public void ProcessAnimation(AnimationStream stream)
    {
        isActivity = true;
        var human = stream.AsHuman();

        UpdateFootData(ref stream, ref human, ref leftFootIKInfo, AvatarIKGoal.LeftFoot);
        UpdateFootData(ref stream, ref human, ref rightFootIKInfo, AvatarIKGoal.RightFoot);

        var curBodyPos = human.bodyPosition;

        var temp1 = human.GetGoalPosition(AvatarIKGoal.LeftFoot);
        var temp2 = human.GetGoalPosition(AvatarIKGoal.RightFoot);


        var dis1 = Mathf.Abs(leftFootIKInfo.worldPos.y - leftFootIKInfo.lastWorldPos.y) * leftFootIKInfo.curWeight;
        var dis2 = Mathf.Abs(rightFootIKInfo.worldPos.y - rightFootIKInfo.lastWorldPos.y) * rightFootIKInfo.curWeight;

        var maxDis = Mathf.Max(dis1, dis2);
        var minY = Mathf.Min(temp1.y, temp2.y);


        var bodyWeight = Mathf.Abs(curBodyPos.y - minY) > 1f ? 1 : 0;

        var offsetY = Mathf.Max(maxDis, 0);
        lastOffsetY = Mathf.Lerp(lastOffsetY, offsetY * bodyWeight, 0.01f);

        var bodyPos = curBodyPos - Vector3.up * lastOffsetY;
        human.bodyPosition = bodyPos;
        human.SolveIK();
    }

    private void UpdateFootData(ref AnimationStream stream, ref AnimationHumanStream human, ref FootIKInfo info, AvatarIKGoal iKGoal)
    {
        var footPos = human.GetGoalPosition(iKGoal);
        info.direction = Vector3.Normalize(footPos - info.lowerLegHandle.GetPosition(stream));

        info.lastWorldPos = human.GetGoalPosition(iKGoal);
        info.lastQuaternion = human.GetGoalRotation(iKGoal);



        var lerpSpeed = 0f;
        var line = 0.3f;
        if (info.weight > line && isActivity)
        {
            lerpSpeed = Mathf.Lerp(0, 0.01f, (info.weight - line) / (1 - line));
        }
        else
        {
            lerpSpeed = Mathf.Lerp(0, 0.2f, (line - info.weight) / line);
        }

        var toWeight = isActivity ? Mathf.Clamp01(Mathf.Ceil(info.weight - line)) : 0;
        //var t = (!isActivity || toWeight == 0) ? 0.1f : 0.01f;
        var weight = Mathf.Lerp(info.curWeight, toWeight, lerpSpeed);
        human.SetGoalPosition(iKGoal, info.worldPos);
        human.SetGoalRotation(iKGoal, info.quaternion);
        human.SetGoalWeightPosition(iKGoal, weight);
        human.SetGoalWeightRotation(iKGoal, weight);

        info.curWeight = weight;
    }

    public void ProcessRootMotion(AnimationStream stream)
    {
        //stream.velocity = Vector3.one;
        //return;
        //if (!applyRootMotion)
        //{
        //    stream.velocity = Vector3.one;
        //    return;
        //}
        //// 获取默认的根运动位移和旋转
        //Vector3 defaultDeltaPosition = stream.rootMotionPosition;
        //Quaternion defaultDeltaRotation = stream.rootMotionRotation;
        //// 修改位移，根据自定义速度乘数调整
        //Vector3 customDeltaPosition = defaultDeltaPosition;
        //// 应用修改后的根运动
        //stream.velocity = stream.velocity;

    }
}