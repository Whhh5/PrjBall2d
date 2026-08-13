using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart1012StarData : GameBattleSkillBallPartStarData
{
    public int collisionEnergy;
    public int energyMaxCount;
    public float restorePercentage01;
    public EnEntityAttributeId testAttId;
    public EnEntityAttributeId addAttId;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out collisionEnergy);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out energyMaxCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out restorePercentage01);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out testAttId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out addAttId);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in collisionEnergy);
        ISerializeToIntArray.SerializeToInt(ref list, in energyMaxCount);
        ISerializeToIntArray.SerializeToInt(ref list, in restorePercentage01);
        ISerializeToIntArray.SerializeToInt(ref list, testAttId);
        ISerializeToIntArray.SerializeToInt(ref list, addAttId);
        return list;
    }
    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var testAttName = LanguageUtility.GetLanguage_AttiributeName(testAttId);
        var addAttName = LanguageUtility.GetLanguage_AttiributeName(addAttId);

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            collisionEnergy, energyMaxCount, restorePercentage01, testAttName, addAttName);
        descInfos.Add(info);
    }
}
public class GameBattleSkillBallPart1012Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1012EntityData, GameBattleSkillBallPart1012StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart1012StarData()
            {
                collisionEnergy = 1,
                energyMaxCount = 10,
                restorePercentage01 = 0.1f,
                testAttId = EnEntityAttributeId.LimitHp,
                addAttId = EnEntityAttributeId.Hp,
            },
            new GameBattleSkillBallPart1012StarData()
            {
                collisionEnergy = 1,
                energyMaxCount = 8,
                restorePercentage01 = 0.1f,
                testAttId = EnEntityAttributeId.LimitHp,
                addAttId = EnEntityAttributeId.Hp,
            },
            new GameBattleSkillBallPart1012StarData()
            {
                collisionEnergy = 1,
                energyMaxCount = 6,
                restorePercentage01 = 0.1f,
                testAttId = EnEntityAttributeId.LimitHp,
                addAttId = EnEntityAttributeId.Hp,
            },
        };
    }
}
