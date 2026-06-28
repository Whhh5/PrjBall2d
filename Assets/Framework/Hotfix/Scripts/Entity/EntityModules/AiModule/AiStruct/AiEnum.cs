

public enum EnAiExecutionType
{
    None = 0,
    Cmd,
    Move,
    LookAt,
}

public enum EnAiPerceptionId
{
    None = 0,
    RandomEntity,
}

public enum EnAiDecisionId
{
    None = 0,
    Default,
}

public enum EnDecisionLevel
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
}
public enum EnAiDecisionSubGeneratorType
{
    None,
    Transform,
    LookAt,
}