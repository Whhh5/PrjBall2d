public class AiExecutionUserData : IClassPoolUserData
{
    public int aiId;
    public void OnPoolDestroy()
    {
        aiId = -1;
    }
}