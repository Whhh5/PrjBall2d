using System.Collections.Generic;

public class GameBattleSkillBall_101StarData : GameBattleSkillBallStarData
{
    public int damageValue;
    public float atkPercentage;
    public EnDamageType damageType;

    public int maxCollisionCount;


    protected override List<int> SerializeToIntArray2()
    {
        var list = new List<int>();
        ISerializeToIntArray.SerializeToInt(ref list, in damageValue);
        ISerializeToIntArray.SerializeToInt(ref list, in atkPercentage);
        ISerializeToIntArray.SerializeToInt(ref list, damageType);

        ISerializeToIntArray.SerializeToInt(ref list, maxCollisionCount);
        return list;
    }
    protected override void DeserializeFromIntArray2(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
    {
        var index = 0;
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageValue);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out atkPercentage);
        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out damageType);

        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref index, out maxCollisionCount);
    }
}
public class BattleSkillBall_101UserData : GameBattleSkillBallEntityUserData
{
    public int star;
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        star = -1;
    }
}
public class GameBattleSkillBall_101Data : GameBallData<GameBattleSkillBall_101EntityData, GameBattleSkillBall_101StarData>
{

    public static List<ISerializeToIntArray> GetStarSerializeArray()
    {
        return new List<ISerializeToIntArray>()
                {
                    new GameBattleSkillBall_101StarData(){
                        damageValue = 5,
                        atkPercentage = 0.25f,
                        damageType = EnDamageType.Physic,
                        maxCollisionCount = 1,
                    },
                    new GameBattleSkillBall_101StarData(){
                        damageValue = 10,
                        atkPercentage = 0.4f,
                        damageType = EnDamageType.Physic,
                        maxCollisionCount = 2,
                    },
                    new GameBattleSkillBall_101StarData(){
                        damageValue = 20,
                        atkPercentage = 0.8f,
                        damageType = EnDamageType.Physic,
                        maxCollisionCount = 3,
                    },
                };
    }
}
