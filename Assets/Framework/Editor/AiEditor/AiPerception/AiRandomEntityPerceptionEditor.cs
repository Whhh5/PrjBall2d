using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AiRandomEntityPerceptionEditor : AiRandomEntityPerception, IAiPerceptionEditor
{
    [EditorField(nameof(_Layer))]
    private EnGameLayerInt _Layer
    {
        get => this.GetFieldValue<EnGameLayerInt>(nameof(_Layer));
        set => this.SetFieldValue<EnGameLayerInt>(nameof(_Layer), value);
    }
    [EditorField(nameof(_Radius))]
    private float _Radius
    {
        get => this.GetFieldValue<float>(nameof(_Radius));
        set => this.SetFieldValue<float>(nameof(_Radius), value);
    }

    private AiNodeRectInfo[] _NodeRects;
    public Rect CalculateLocalNodeArea()
    {
        var rectInfos = new List<AiNodeRectInfo>(5);

        var layerRect = new Rect(0, 0, 100, 30);
        rectInfos.Add(new(layerRect, Vector2.zero));
        var radiusRect = new Rect(0, 0, 100, 30);
        rectInfos.Add(new(radiusRect, new Vector2(0, layerRect.height + 20)));

        _NodeRects = IAiNodeEditor.NormalizeRect(rectInfos, EditorConfig.BoxPadding);
        var thisRect = IAiNodeEditor.CalculateRect(_NodeRects, EditorConfig.BoxPadding);
        return thisRect;
    }

    public void DrawNodeArea(in Rect rect)
    {
        EditorGuiUtil.DrawBox(rect, $"{GetType()}");

        var rectInfoIndex = 0;
        var layerRectInfo = _NodeRects[rectInfoIndex++];
        var layerRect = layerRectInfo.GetRect(rect.position);
        EditorGuiUtil.DrawEnum(layerRect, _Layer, layer => _Layer = layer);
        var radius = EditorGUI.FloatField(_NodeRects[rectInfoIndex++].GetRect(rect.position), _Radius);
        if (radius != _Radius)
            _Radius = radius;
    }
}
