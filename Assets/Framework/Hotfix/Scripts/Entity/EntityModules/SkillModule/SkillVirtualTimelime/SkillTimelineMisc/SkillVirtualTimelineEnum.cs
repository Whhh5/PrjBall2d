
public enum EnSkillPhysicsType
{
    None = 0,
    Sphere,
    Sphere2D,
    Box,
    Box2D,
    Cylinder,
    Cylinder2D,
}

public enum EnSkillPhysicsRayType
{
    None = 0,
    Near = 1 << 0,
    One = 1 << 1,
    All = 1 << 2,
    Num = 1 << 3,
    Random = 1 << 4,
}