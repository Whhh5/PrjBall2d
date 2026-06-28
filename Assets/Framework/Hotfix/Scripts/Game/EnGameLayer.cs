
using System;

public enum EnGameLayer
{
    None = 0,

    Ground = 6,
    Player = 7,
    Monster = 8,
    Terrain = 9,
    FlyEntity = 9,
}
[Flags]
public enum EnGameLayerInt
{
    None = 0,

    Ground = 1 << EnGameLayer.Ground,
    Player = 1 << EnGameLayer.Player,
    Monster = 1 << EnGameLayer.Monster,
    Terrain = 1 << EnGameLayer.Terrain,
    FlyEntity = 1 << EnGameLayer.FlyEntity,
}
