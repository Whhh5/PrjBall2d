using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindowData<T>: UIWindowData
    where T: UIWindow
{
    protected T _UIWindow = null;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _UIWindow = null;
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _UIWindow = _GameEntity as T;
    }
}
public abstract class UIWindowData : UIEntityData
{
    public abstract EnUIWindowType WindowType { get; }

    public virtual void OnShow(IClassPool userData)
    {

    }
    public virtual void OnHide()
    {

    }
}
public abstract class UIWindow : UIEntity
{

}
public abstract class UIWindow<T> : UIWindow
    where T: UIWindowData
{
    protected T _UIWindowData = null;
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        _UIWindowData = _EntityData as T;
    }
    public override void OnUnload()
    {
        base.OnUnload();
        _UIWindowData = null;
    }
}