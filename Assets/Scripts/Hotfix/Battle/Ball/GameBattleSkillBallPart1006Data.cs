using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart1006StarData : GameBattleSkillBallPartStarData
{
    public EnBuff testBuffId;
    public int testBuffCount;
    public int testProbability0100;
    public EntityFreezeBuffData buffData;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out testBuffId);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out testBuffCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out testProbability0100);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, userData);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, testBuffId);
        ISerializeToIntArray.SerializeToInt(ref list, in testBuffCount);
        ISerializeToIntArray.SerializeToInt(ref list, in testProbability0100);
        ISerializeToIntArray.SerializeToInt(ref list, in buffData);
        return list;
    }

    public override void GetDescriptionList(ref List<DescriptionInfo> descInfos, EnDescriptionLevel level)
    {
        base.GetDescriptionList(ref descInfos, level);

        var buffStyleName = LanguageUtility.GetLanguage_BuffStyleName(testBuffId);
        var buffName = LanguageUtility.GetLanguage_BuffStyleName(buffData.GetBuff());

        var info = ClassPoolMgr.Instance.Pull<DescriptionInfo>();
        info.level = level;
        info.title = LanguageUtility.GetLanguage_BallPartStyleName(_PartId);
        info.content = string.Format(LanguageUtility.GetLanguage_BallPartDescription(_PartId),
            buffStyleName, testBuffCount, testProbability0100, buffName);
        descInfos.Add(info);

        buffData.GetDescriptionList(ref descInfos, level + 1);
    }
}
public class GameBattleSkillBallPart1006Data : GameBattleSkillBallPartData<GameBattleSkillBallPart1006EntityData, GameBattleSkillBallPart1006StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart1006StarData()
            {
                testBuffId = EnBuff.Crystal,
                testBuffCount = 3,
                testProbability0100 = 20,
                buffData = new EntityFreezeBuffData()
                {
                    duration = 1f,
                    speedReducePercentage01 = 1,
                    attId = EnEntityAttributeId.MoveSpeed,
                },
            },
            new GameBattleSkillBallPart1006StarData()
            {
                testBuffId = EnBuff.Crystal,
                testBuffCount = 3,
                testProbability0100 = 30,
                buffData = new EntityFreezeBuffData()
                {
                    duration = 1f,
                    speedReducePercentage01 = 1,
                    attId = EnEntityAttributeId.MoveSpeed,
                },
            },
            new GameBattleSkillBallPart1006StarData()
            {
                testBuffId = EnBuff.Crystal,
                testBuffCount = 3,
                testProbability0100 = 50,
                buffData = new EntityFreezeBuffData()
                {
                    duration = 1f,
                    speedReducePercentage01 = 1,
                    attId = EnEntityAttributeId.MoveSpeed,
                },
            },
        };
    }
}
