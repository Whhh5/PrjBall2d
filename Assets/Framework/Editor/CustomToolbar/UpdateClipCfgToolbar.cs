using UnityEngine;


[CustomUnityToolbarAttribute("Update ClipCfg", EnCustomUnityToolbarType.Left,
    EnCustomUnityToolbarOrder.ExportExcel)]
public class UpdateClipCfgToolbar : ICustomUnityToolbar
{
    public void OnClick()
    {
        var clipCfgCount = ExcelEditorUtil.GetCfgCount<ClipCfg>();
        for (var i = 0; i < clipCfgCount; i++)
        {
            var clipCfg = ExcelEditorUtil.GetCfgByIndex<ClipCfg>(i);
            var assetCfg = ExcelEditorUtil.GetCfg<AssetCfg>(clipCfg.nAssetID);
            var clipAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>(assetCfg.strPath);
            if (clipAsset == null)
            {
                Debug.LogError($"AnimationClip 资源不存在 path：{assetCfg.strPath}");
                continue;
            }
            ExcelEditorUtil.SetCfgValue(clipCfg, nameof(ClipCfg.bIsLoop), clipAsset.isLooping ? 1 : 0);
            ExcelEditorUtil.SetCfgValue(clipCfg, nameof(ClipCfg.fLength), clipAsset.length);
        }

        ExcelEditorUtil.SaveExcel<ClipCfg>();
    }
}
