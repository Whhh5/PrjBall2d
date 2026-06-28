using UnityEngine;

public abstract class UIEntityData : GameEntityData
{
    private UIEntity m_UIEntity = null;
    private Vector2 m_AnchoredPos2D;
    public Vector2 AnchoredPos2D => m_AnchoredPos2D;
    public override void OnGODestroy()
    {
        m_AnchoredPos2D = Vector2.zero;
        m_UIEntity = null;
        base.OnGODestroy();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        m_UIEntity = _GameEntity as UIEntity;
    }
    public void SetAnchoredPos2D(Vector2 anchorPos)
    {
        m_AnchoredPos2D = anchorPos;
        if (_IsLoadSuccess)
            m_UIEntity.SetAnchoredPos2D();
    }
}

public abstract class UIEntity: GameEntity
{
    private RectTransform m_Rect = null;
    private UIEntityData m_UIEntityData = null;
    private void OnDestroy()
    {
        m_Rect = null;
    }
    protected override void Awake()
    {
        base.Awake();
        m_Rect = GetComponent<RectTransform>();
    }
    public override void OnUnload()
    {
        m_UIEntityData = null;
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        m_UIEntityData = _EntityData as UIEntityData;
        SetAnchoredPos2D();
    }

    public void SetAnchoredPos2D()
    {
        m_Rect.anchoredPosition3D = m_UIEntityData.AnchoredPos2D;
    }
}
