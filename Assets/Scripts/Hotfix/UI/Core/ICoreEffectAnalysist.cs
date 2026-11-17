
public interface ICoreEffectAnalysist
{
    public string GetCoreName(int coreCfgId);
    public int GetIconAssetId(int coreCfgId);
    public float CalculateRandomEffectValue(int coreCfgId, EnCoreQuality quality);
    public string GetDescriptsByCoreId(int coreId, EnCoreQuality quality);
    public string GetDescriptsByCoreCfgId(int coreCfgId, EnCoreQuality quality);
    public void ApplyEntity(int entityId, int coreId);
}