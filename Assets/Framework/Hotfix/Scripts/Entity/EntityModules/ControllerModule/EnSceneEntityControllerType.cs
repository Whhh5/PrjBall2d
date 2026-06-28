

using System;

[Flags]
public enum EnSceneEntityControllerType
{
    None = 0,
    Player = 1 << 0,
    Ai = 1 << 1,
}