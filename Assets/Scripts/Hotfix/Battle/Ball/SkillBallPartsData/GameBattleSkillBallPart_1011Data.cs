using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1011StarData : GameBattleSkillBallPartStarData
{
    public IEntityBuffData buffData;
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out buffData, in userData);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, buffData);
        return list;
    }
}
public class GameBattleSkillBallPart_1011Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1011EntityData, GameBattleSkillBallPart_1011StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new()
        {
            new GameBattleSkillBallPart_1011StarData()
            {
                buffData = new EntityElectricBuffData()
                {
                    duration = 3,
                    maxCount = 3,
                    reduceAttributeList = new()
                    {
                        new()
                        {
                            attributeId = EnEntityAttributeId.PhysicsArmor,
                            reducePercentage01 = 0.05f,
                        },
                        new()
                        {
                            attributeId = EnEntityAttributeId.MagicArmor,
                            reducePercentage01 = 0.05f,
                        },
                    }
                },
            },
            new GameBattleSkillBallPart_1011StarData()
            {
                buffData = new EntityElectricBuffData()
                {
                    duration = 4,
                    maxCount = 4,
                    reduceAttributeList = new()
                    {
                        new()
                        {
                            attributeId = EnEntityAttributeId.PhysicsArmor,
                            reducePercentage01 = 0.08f,
                        },
                        new()
                        {
                            attributeId = EnEntityAttributeId.MagicArmor,
                            reducePercentage01 = 0.08f,
                        },
                    }
                },
            },
            new GameBattleSkillBallPart_1011StarData()
            {
                buffData = new EntityElectricBuffData()
                {
                    duration = 5,
                    maxCount = 5,
                    reduceAttributeList = new()
                    {
                        new()
                        {
                            attributeId = EnEntityAttributeId.PhysicsArmor,
                            reducePercentage01 = 0.12f,
                        },
                        new()
                        {
                            attributeId = EnEntityAttributeId.MagicArmor,
                            reducePercentage01 = 0.12f,
                        },
                    }
                },
            },
        };
    }
}
