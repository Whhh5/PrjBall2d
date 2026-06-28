


public interface IPhysicsColliderCallbackCustomData
{

}


public interface IPhysicsResolve : IClassPoolInit<ISerializeToIntArrayUserData>, ISerializeToIntArray
{
    public EnSkillPhysicsType GetSkillPhysicsType();
    public void Execute(in int entityID, in EnGameLayerInt layer, in PhysicsColliderCallback callback, in IPhysicsColliderCallbackCustomData cusomData);
}