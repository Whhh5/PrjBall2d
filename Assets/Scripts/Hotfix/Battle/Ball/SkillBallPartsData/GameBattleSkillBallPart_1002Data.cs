using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1002StarData : GameBattleSkillBallPartStarData
{
    public int atkDamage;
    public float atkPercentage;
    public EnDamageType atkDamageType;
    public float radius;
    public int fireProbability;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out fireProbability);
    }


    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in fireProbability);

        return list;
    }
}
public class GameBattleSkillBallPart_1002Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1002EntityData, GameBattleSkillBallPart_1002StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart_1002StarData()
            {
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkPercentage = 0.5f,
                radius = 1.5f,
                fireProbability = 20,
            },
            new GameBattleSkillBallPart_1002StarData()
            {
                atkDamage = 20,
                atkDamageType = EnDamageType.Magic,
                atkPercentage = 0.5f,
                radius = 1.5f,
                fireProbability = 40,
            },
            new GameBattleSkillBallPart_1002StarData()
            {
                atkDamage = 30,
                atkDamageType = EnDamageType.Magic,
                atkPercentage = 0.5f,
                radius = 1.5f,
                fireProbability = 80,
            },
        };
    }
}
