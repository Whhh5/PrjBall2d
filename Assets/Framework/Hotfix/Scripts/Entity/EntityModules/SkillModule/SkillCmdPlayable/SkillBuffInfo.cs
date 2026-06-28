using System.Collections.Generic;

//public class SkillBuffInfo : IClassPoolInit<ISerializeToIntArrayUserData>, ISerializeToIntArray
//{
//    public EnBuff buff;
//    public int[] arrParams;

//    public EnTypeId GetTypeDefineId() => EnTypeId.SkillBuffInfo;
//    public void DiserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
//    {
//        var localIndex = 0;
//        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out buff);
//        ISerializeToIntArray.ParseToValue(in datas, in start, in count, ref localIndex, out arrParams);
//    }
//    public List<int> SerializeToIntArray()
//    {
//        var result = new List<int>();
//        ISerializeToIntArray.SerializeToInt(ref result, buff);
//        ISerializeToIntArray.SerializeToInt(ref result, arrParams);
//        return result;
//    }
//    public void OnPoolDestroy()
//    {
//        buff = EnBuff.None;
//        arrParams = null;
//    }
//    public void OnPoolInit(ISerializeToIntArrayUserData userData)
//    {
//        DiserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
//    }
//}