using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIExtend
{
    public static void AddListener(this Button btn, UnityAction callback)
    {
        btn.onClick.AddListener(callback);
    }
    public static void RemoveAllListeners(this Button btn)
    {
        btn.onClick.RemoveAllListeners();
    }
    public static void RemoveListener(this Button btn, UnityAction callback)
    {
        btn.onClick.RemoveListener(callback);
    }
}
