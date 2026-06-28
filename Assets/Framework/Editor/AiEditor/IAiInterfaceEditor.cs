using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public interface IAiEditor : IEditor, ISerializeToIntArray
{
}

public readonly struct AiNodeRectIndexInfo
{
    public readonly int start;
    public readonly int count;
    public AiNodeRectIndexInfo(in int start, in int count)
    {
        this.start = start;
        this.count = count;
    }
    public AiNodeRectIndexInfo(in int index)
    {
        this.start = index;
        this.count = 1;
    }
    public int GetIndex()
    {
        return GetIndex(0);
    }
    public int GetIndex(in int offset)
    {
        var result = start + offset;
        if (offset >= count || offset < 0)
            return -1;
        return result;
    }
}
public readonly struct AiNodeRectInfo
{
    public readonly Rect rect;
    public readonly Vector2 offset;
    public AiNodeRectInfo(in Rect rect, in Vector2 offset)
    {
        this.rect = rect;
        this.offset = offset;
    }
    public readonly Rect GetLocalRect()
    {
        return new Rect(rect.position + offset, rect.size);
    }
    public readonly Rect GetRect(in Vector2 offset)
    {
        var localRect = GetLocalRect();
        localRect.position += offset;
        return localRect;
    }
    public readonly Vector2 GetLocalMin()
    {
        return rect.min + offset;
    }
    public readonly Vector2 GetLocalMax()
    {
        return rect.max + offset;
    }
}
public interface IAiNodeEditor : IAiEditor
{
    public void DrawNodeArea(in Rect rect);
    public Rect CalculateLocalNodeArea();

    public static AiNodeRectInfo[] NormalizeRect(List<AiNodeRectInfo> rectInfos, Vector4 padding)
    {
        var ltPoint = new Vector2(float.MaxValue, float.MaxValue);
        var result = new AiNodeRectInfo[rectInfos.Count];
        for (var i = 0; i < rectInfos.Count; i++)
            ltPoint = Vector2.Min(ltPoint, rectInfos[i].GetLocalMin());
        for (var i = 0; i < rectInfos.Count; i++)
        {
            ref var item = ref result[i];
            result[i] = new AiNodeRectInfo(rectInfos[i].rect, rectInfos[i].offset - ltPoint + new Vector2(padding.x, padding.z));
        }
        return result;
    }
    public static Rect CalculateRect(AiNodeRectInfo[] rects, Vector4 padding)
    {
        if (rects.Length == 0)
            return new Rect();
        var thisRect = new Rect()
        {
            min = rects[0].GetLocalMin(),
            max = rects[0].GetLocalMax(),
        };
        for (int i = 1; i < rects.Length; i++)
        {
            ref readonly var rectInfo = ref rects[i];
            thisRect.min = Vector2.Min(thisRect.min, rectInfo.GetLocalMin());
            thisRect.max = Vector2.Max(thisRect.max, rectInfo.GetLocalMax());
        }
        thisRect.min -= new Vector2(padding.x, padding.z);
        thisRect.max += new Vector2(padding.y, padding.w);
        return thisRect;
    }

    public static Rect CalculateRect(AiNodeRectInfo[] rects, AiNodeRectIndexInfo indexInfo)
    {
        var selectRects = new AiNodeRectInfo[indexInfo.count];
        Array.Copy(rects, indexInfo.start, selectRects, 0, selectRects.Length);
        var rect = CalculateRect(selectRects, Vector4.zero);
        return rect;
    }
}
public interface IAiDecisionEditor : IAiDecision, IAiNodeEditor
{

}
public interface IAiDecisionGeneratorEditor : IAiDecisionGenerator, IAiNodeEditor
{

}
public interface IAiDecisionSubGeneratorEditor : IAiNodeEditor, IAiDecisionSubGenerator
{
    public string GetTitle();
    public IAiConditionEditor[] GetConditionsEditor();
    public void SetConditionsEditor(IAiConditionEditor[] conditionEditors);
}
public interface IAiPerceptionTestEditor : IAiPerceptionTest, IAiNodeEditor
{

}
public interface IAiConditionEditor : IAiCondition, IAiNodeEditor
{

}
public interface IAiPerceptionMatchInfoEditor : IAiPerceptionMatchInfo, IAiNodeEditor
{

}
public interface IAiPerceptionEditor : IAiPerception, IAiNodeEditor
{

}
public interface IAiDecisionInfoEditor : IAiDecisionInfo, IAiEditor
{

}