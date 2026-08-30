using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHallMainBattleWindow_ChapterItem : UIWindowItem
{
    [SerializeField, AutoReference(SearchScope.Self)]
    private RectTransform _Rect;
    [SerializeField, AutoReference]
    private Image _IconImg;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _ChapterNameTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _MapNameTxt;
    [SerializeField, AutoReference]
    private TextMeshProUGUI _ChapterTxt;
    [SerializeField, AutoReference]
    private CanvasGroup _TextGroup;
    [SerializeField, AutoReference]
    private GameObject _LockGo;

    public override void Unload()
    {
        _IconImg.UnloadSprite();
        base.Unload();
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
    }
    public void Init(int chapterId, bool isLock)
    {
        var chapterCfg = GameSchedule.Instance.GetChapterCfg0(chapterId);
        var mapCfg = GameSchedule.Instance.GetMapCfg0(chapterCfg.nMapId);
        _ChapterNameTxt.SetLanguageText(chapterCfg.nNameLangId);
        _MapNameTxt.SetLanguageText(mapCfg.nNameLangId);
        _ChapterTxt.SetText($"{chapterId}");

        _IconImg.LoadSprirte((EnLoadTarget)mapCfg.nIconAssetId);
        SetIsLock(isLock);
    }
    public void SetSelectStatus(bool isSelect)
    {
        _TextGroup.alpha = isSelect ? 1 : 0;
    }
    public Vector2 GetSize()
    {
        return _Rect.sizeDelta;
    }
    public float GetLocalPosX()
    {
        return _Rect.anchoredPosition.x;
    }
    public void SetLocalPos(float posX)
    {
        var pos = _Rect.anchoredPosition;
        pos.x = posX;
        _Rect.anchoredPosition = pos;
    }
    public float GetLocalScale()
    {
        return _Rect.localScale.x;
    }
    public void SetLocalScale(float sclae)
    {
        _Rect.localScale = Vector2.one * sclae;
    }
    public void SetIsLock(bool isLock)
    {
        _LockGo.SetActive(isLock);
    }
}
