

public readonly ref struct AiTimeInfo
{
    public readonly float deltaTime;
    public readonly float elapsedTime;
    public AiTimeInfo(float deltaTime, float elapsedTime)
    {
        this.deltaTime = deltaTime;
        this.elapsedTime = elapsedTime;
    }
}