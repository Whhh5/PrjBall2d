using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtil
{
    public static Vector2 WorldToUIPos(Vector3 worldPos)
    {
        var canvasRect = UIMgr.Instance.GetCanvasRect();
        var uiCamera = CameraMgr.Instance.GetUICamera();
        var mainCamera = CameraMgr.Instance.GetMainCamera();
        var screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, uiCamera, out var uiPos))
            return Vector2.zero;
        return uiPos;
    }
    public static Vector2 ViewToUIPos(in Vector2 screenPoint)
    {
        var canvasRect = UIMgr.Instance.GetCanvasRect();
        var uiCamera = CameraMgr.Instance.GetUICamera();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, uiCamera, out var uiPos))
            return Vector2.zero;
        return uiPos;
    }


    private readonly static Dictionary<int, int> _Image2Sprite = new();
    public static async void LoadSprite(this Image img, int assetID)
    {
        var id = img.GetInstanceID();
        if (_Image2Sprite.TryGetValue(id, out var curAssetID))
        {
            if (curAssetID == assetID)
                return;
            img.UnloadSprite();
        }

        _Image2Sprite.Add(id, assetID);
        var sprite = await ABBLoadMgr.Instance.LoadAsync<Sprite>(assetID);
        if (_Image2Sprite.TryGetValue(id, out curAssetID))
            if (assetID != curAssetID)
                return;

        img.sprite = sprite;
    }
    public static async UniTask LoadSpriteAsync(this Image img, int assetID)
    {
        var id = img.GetInstanceID();
        if (_Image2Sprite.TryGetValue(id, out var curAssetID))
        {
            if (curAssetID == assetID)
                return;
            img.UnloadSprite();
        }

        _Image2Sprite.Add(id, assetID);
        var sprite = await ABBLoadMgr.Instance.LoadAsync<Sprite>(assetID);
        if (_Image2Sprite.TryGetValue(id, out curAssetID))
            if (assetID != curAssetID)
                return;

        img.sprite = sprite;
    }
    public static void UnloadSprite(this Image img)
    {
        var id = img.GetInstanceID();
        if (!_Image2Sprite.TryGetValue(id, out var assetID))
            return;
        _Image2Sprite.Remove(id);
        img.sprite = null;
        ABBLoadMgr.Instance.Unload(assetID);
    }
}
