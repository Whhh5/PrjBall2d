using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleMainWindow_SkillBallIconItem : MonoBehaviour
{
    [SerializeField]
    private Image _IconImg;
    [SerializeField]
    private Button _Btn;

    private Action _BtnCallback;
    private RectTransform _Rect = null;

    private void Awake()
    {
        _Btn.AddListener(OnClick_Btn);
    }
    private void OnDestroy()
    {
        _Btn.RemoveListener(OnClick_Btn);
    }

    public void Uninit()
    {
        _IconImg.UnloadSprite();
        _BtnCallback = null;
        _Rect = null;
    }
    public void Init(EnLoadTarget icon, Action callback)
    {
        _Rect = GetComponent<RectTransform>();
        _IconImg.LoadSprirte(icon);
        _BtnCallback = callback;
    }

    private void OnClick_Btn()
    {
        _BtnCallback?.Invoke();
    }
    public Vector2 GetScreenPosition()
    {
        var screenPos = UIUtil.WorldToView(_Rect.position);
        return screenPos;
    }
    public Vector3 GetWorldPos()
    {
        return _Rect.position;
    }
    public void SetLocalPos(Vector2 localPos)
    {
        _Rect.anchoredPosition = localPos;
    }
    public Vector2 GetSize()
    {
        return _Rect.sizeDelta;
    }
}
