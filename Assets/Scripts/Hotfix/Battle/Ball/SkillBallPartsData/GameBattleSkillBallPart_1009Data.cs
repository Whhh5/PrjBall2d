using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GameBattleSkillBallPart_1009StarData : GameBattleSkillBallPartStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public float width;
    public int chargeCount;
    public float atkInterval;
    public int maxChargeCount;

    public override List<int> SerializeToIntArray()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, in width);
        ISerializeToIntArray.SerializeToInt(ref list, in chargeCount);
        ISerializeToIntArray.SerializeToInt(ref list, in atkInterval);
        ISerializeToIntArray.SerializeToInt(ref list, in maxChargeCount);
        return list;
    }
    public override void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out width);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out chargeCount);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkInterval);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out maxChargeCount);
    }
}
public class GameBattleSkillBallPart_1009Data : GameBattleSkillBallPartData<GameBattleSkillBallPart_1009EntityData, GameBattleSkillBallPart_1009StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
        {
            new GameBattleSkillBallPart_1009StarData()
            {
                 damageValue = 10,
                 damageType = EnDamageType.Magic,
                 atkPercentage = 0.45f,
                 width = 0.4f,
                 atkInterval = 1f,
                 chargeCount = 1,
                 maxChargeCount = 3,
            },
            new GameBattleSkillBallPart_1009StarData()
            {
                 damageValue = 20,
                 damageType = EnDamageType.Magic,
                 atkPercentage = 0.6f,
                 width = 0.6f,
                 atkInterval = 0.6f,
                 chargeCount = 2,
                 maxChargeCount = 10,
            },
            new GameBattleSkillBallPart_1009StarData()
            {
                 damageValue = 30,
                 damageType = EnDamageType.Magic,
                 atkPercentage = 1f,
                 width = 1f,
                 atkInterval = 0.2f,
                 chargeCount = 3,
                 maxChargeCount = 20,
            },
        };
    }
}
