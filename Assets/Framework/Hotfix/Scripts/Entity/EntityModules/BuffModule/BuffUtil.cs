public partial class BuffUtil
{
	public static ISkillEntityBuff CreateBuffData(EnBuff buff, IClassPoolUserData data)
	{
		return buff switch
		{
			EnBuff.NoMovement => ClassPoolMgr.Instance.Pull<EntityNoMovementBuff>(data),
			_ => default,
		};
	}
}
