using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleSkillBallPart_1001StarData : GameBattleSkillBallPartStarData
{
    public int atkDamage;
    public EnDamageType atkDamageType;
    public float radius;

    public float atkVulnerableDamagePercentage;

    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkDamageType);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out radius);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkVulnerableDamagePercentage);
    }

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in atkDamage);
        ISerializeToIntArray.SerializeToInt(ref list, atkDamageType);
        ISerializeToIntArray.SerializeToInt(ref list, in radius);
        ISerializeToIntArray.SerializeToInt(ref list, in atkVulnerableDamagePercentage);

        return list;
    }
}
public class GameBattleSkillBallPart_1001Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1001EntityData, GameBattleSkillBallPart_1001StarData>
{
    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart_1001StarData()
            {
                radius = 1.5f,
                atkDamage = 10,
                atkDamageType = EnDamageType.Magic,
                atkVulnerableDamagePercentage = 1,
            },
            new GameBattleSkillBallPart_1001StarData()
            {
                radius = 1.5f,
                atkDamage = 20,
                atkDamageType = EnDamageType.Magic,
                atkVulnerableDamagePercentage = 2,
            },
            new GameBattleSkillBallPart_1001StarData()
            {
                radius = 1.5f,
                atkDamage = 30,
                atkDamageType = EnDamageType.Magic,
                atkVulnerableDamagePercentage = 3,
            },
        };
    }
}
