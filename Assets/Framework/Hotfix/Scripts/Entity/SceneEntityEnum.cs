using System;

[Flags]
public enum EnSceneEntityStatus
{
    None = 0,
    Normalize = 1 << 0, // 正常
    Fly = 1 << 1, // 在天上
    Die = 1 << 2, // 死亡
}

