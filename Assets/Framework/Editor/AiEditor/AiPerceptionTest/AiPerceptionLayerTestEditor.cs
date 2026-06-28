


using System.Collections.Generic;
using UnityEngine;

public class AiPerceptionLayerTestEditor : AiPerceptionLayerTest, IAiPerceptionTestEditor
{
    [EditorField(nameof(_Layer))]
    private EnGameLayerInt _Layer
    {
        get => this.GetFieldValue<EnGameLayerInt>(nameof(_Layer));
        set => this.SetFieldValue<EnGameLayerInt>(nameof(_Layer), value);
    }

    private AiNodeRectInfo[] _NodeRects;
    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);
        var layerRect = new Rect(0, 0, 100, 30);
        rectInfos.Add(new(layerRect, Vector2.zero));
        _NodeRects = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_NodeRects, EditorConfig.BoxPadding);
        return thisRect;
    }

    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");

        var layerRectInfo = _NodeRects[0];
        var layerRect = layerRectInfo.GetRect(rect.position);
        EditorGuiUtil.DrawEnum(layerRect, _Layer, layer => _Layer = layer);
    }
}