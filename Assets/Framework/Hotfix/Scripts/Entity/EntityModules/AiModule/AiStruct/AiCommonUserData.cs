

public class AiCommonUserData : ISerializeToIntArrayUserData
{
    public int aiId;
    public int startIndex { get; set; }
    public int count { get; set; }
    public int[] arrParams { get; set; }

    public void OnPoolDestroy()
    {
        startIndex
            = count
            = aiId
            = -1;
        arrParams = null;
    }
}