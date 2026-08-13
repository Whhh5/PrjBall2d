using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleUplevelWindowItemData : IClassPoolDestroy
{
    public EnUIBattleUplevelWindowItemType itemType;
    public IUIBattleUplevelWindow_ItemUserData userData;
    public void OnPoolDestroy()
    {
        if (userData != null)
            ClassPoolMgr.Instance.Push(userData);
        itemType = EnUIBattleUplevelWindowItemType.None;
        userData = null;
    }
}

public enum EnUIBattleUplevelWindowItemType
{
    None,
    SkillBall,
    SkillPassivity,
    SkillBallFussion,
    SkillBallPart,
    Attribute,
    EnumCount,
}
public interface IUIBattleUplevelWindowItem
{
    public List<DescriptionInfo> GetDescriptionList(EnDescriptionLevel startLevel, EnDescriptionLevel endLevel);
    public void Select();
    public Vector3 GetWorldPos();
    public float GetSizeDeltaY();
}
public class UIBattleUplevelWindowUserData : UIWindowUserData
{
    public List<UIBattleUplevelWindowItemData> itemDataList = new();
    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        itemDataList.Clear();
    }
}
public class UIBattleUplevelWindowData : UIWindowData<UIBattleUplevelWindow, UIBattleUplevelWindowUserData>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIBattleUplevelWindow;

    private readonly List<UIBattleUplevelWindowItemData> _ItemDataList = new();
    private CancellationTokenSource _TokenSource = null;

    protected override void OnPoolInit(UIBattleUplevelWindowUserData userData)
    {
        _ItemDataList.AddRange(userData.itemDataList);
    }
    public override void OnPoolDestroy()
    {
        foreach (var item in _ItemDataList)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        base.OnPoolDestroy();
        _ItemDataList.Clear();
    }
    public override void OnGODestroy()
    {
        _TokenSource.Cancel();
        base.OnGODestroy();
        _TokenSource = null;
    }
    public override void OnGOCreate(GameEntity entity)
    {
        base.OnGOCreate(entity);

        _TokenSource = new();

        _UIWindow.Setting(_ItemDataList, _TokenSource.Token);
    }
    public List<UIBattleUplevelWindowItemData> CopyItemData()
    {
        var list = new List<UIBattleUplevelWindowItemData>(_ItemDataList.Count);
        for (int i = 0; i < _ItemDataList.Count; i++)
        {
            var item = _ItemDataList[i];
            var itemData = ClassPoolMgr.Instance.Pull<UIBattleUplevelWindowItemData>();
            itemData.itemType = item.itemType;
            itemData.userData = item.userData.Copy();
            list.Add(itemData);
        }
        return list;
    }
}
public class UIBattleUplevelWindow : UIWindow
{
    [SerializeField]
    private Button _CloseBtn = null;
    [SerializeField]
    private TextMeshProUGUI _DescLevel2Txt = null;
    [SerializeField]
    private TextMeshProUGUI _DescLevel3Txt = null;
    [SerializeField]
    private RectTransform _SelectScrollRootRect = null;
    [SerializeField]
    private RectTransform _SelectFrameRect = null;
    [SerializeField]
    private GameObject _LineGo = null;
    [SerializeField]
    private CanvasGroup _DescRootGroup = null;

    private readonly List<TextMeshProUGUI> _DescLevelTxts = new();
    private readonly List<GameObject> _LineGos = new();

    private readonly List<IUIBattleUplevelWindowItem> _SelectItemEntitys = new();
    private int _CurSelectIndex = -1;
    private readonly EnDotweenId _MoveId = EnDotweenId.UIBattleUplevelWindow;
    public override void OnUnload()
    {
        DOTween.Kill(_MoveId);
        ClearItemEntitys();

        for (int i = 0; i < _DescLevelTxts.Count; i++)
        {
            var txt = _DescLevelTxts[i].gameObject;
            UnregisterUIDissolve(txt);
            GameObject.Destroy(txt);
        }
        for (int i = 0; i < _LineGos.Count; i++)
        {
            var line = _LineGos[i];
            UnregisterUIDissolve(line);
            GameObject.Destroy(line);
        }
        _CloseBtn.RemoveListener(OnClick_CloseBtn);
        base.OnUnload();
        _DescLevelTxts.Clear();
        _CurSelectIndex = -1;
        _LineGos.Clear();
    }
    private void ClearItemEntitys()
    {
        for (int i = 0; i < _SelectItemEntitys.Count; i++)
        {
            var item = _SelectItemEntitys[i];
            UnregisterUIDissolve(item);
            UIMgr.Instance.UnloadWindowItem(item);
        }
        _SelectItemEntitys.Clear();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();

        _DescLevel2Txt.gameObject.SetActive(false);
        _DescLevel3Txt.gameObject.SetActive(false);
        _LineGo.SetActive(false);
        _DescRootGroup.gameObject.SetActive(false);
        _DescRootGroup.alpha = 0;
        _SelectFrameRect.gameObject.SetActive(false);

        _CloseBtn.AddListener(OnClick_CloseBtn);
    }

    public async void Setting(List<UIBattleUplevelWindowItemData> itemDatas, CancellationToken token)
    {
        for (int i = 0; i < itemDatas.Count; i++)
        {
            var index = i;
            var itemData = itemDatas[index];
            var (isCancel, item) = await CreateItem(i, itemData.itemType, itemData.userData, token)
                .SuppressCancellationThrow();
            if (isCancel)
                break;
            _SelectItemEntitys.Add(item);
            RegisterUIDissolve(item);
        }
    }

    private void OnClick_CloseBtn()
    {
        UIMgr.Instance.HideWindow(_WindowId);
    }

    private void OnClick_Item(int index)
    {
        var itemEnttiy = _SelectItemEntitys[index];
        if (_CurSelectIndex == index)
        {
            // 选择
            itemEnttiy.Select();
            UIMgr.Instance.HideWindow(_WindowId, EnWindowHideType.SelectItem);
        }
        else
        {
            _CurSelectIndex = index;
            PlayAnim(index);
            // 显示详情
            var level2DescInfos = itemEnttiy.GetDescriptionList(EnDescriptionLevel.Level2, EnDescriptionLevel.Level2);
            var descStr = new StringBuilder();
            if (level2DescInfos.Count > 0)
            {
                for (int i = 0; i < level2DescInfos.Count; i++)
                {
                    var info = level2DescInfos[i];
                    var content = string.IsNullOrWhiteSpace(info.title)
                        ? $"{info.content}"
                        : $"{info.title}:{info.content}";
                    if (i < level2DescInfos.Count - 1)
                        descStr.AppendLine(content);
                    else
                        descStr.Append(content);
                }
                _DescLevel2Txt.text = descStr.ToString();
            }
            if (_DescLevel2Txt.gameObject.activeSelf != level2DescInfos.Count > 0)
                _DescLevel2Txt.gameObject.SetActive(level2DescInfos.Count > 0);

            for (int i = 0; i < _DescLevelTxts.Count; i++)
                _DescLevelTxts[i].gameObject.SetActive(false);
            for (int i = 0; i < _LineGos.Count; i++)
                _LineGos[i].SetActive(false);

            for (var j = EnDescriptionLevel.Level3; j < EnDescriptionLevel.Count; j++)
            {
                var levelInfo = itemEnttiy.GetDescriptionList(j, j);
                if (levelInfo.Count == 0)
                    break;
                descStr.Clear();
                for (int i = 0; i < levelInfo.Count; i++)
                {
                    var info = levelInfo[i];
                    var content = string.IsNullOrWhiteSpace(info.title)
                        ? $"{info.content}"
                        : $"{info.title}:{info.content}";
                    if (i < levelInfo.Count - 1)
                        descStr.AppendLine(content);
                    else
                        descStr.Append(content);
                }
                var itemIndex = j - EnDescriptionLevel.Level3;
                TextMeshProUGUI txt;
                GameObject line;
                if (itemIndex < _DescLevelTxts.Count)
                {
                    txt = _DescLevelTxts[itemIndex];
                    line = _LineGos[itemIndex];
                }
                else
                {
                    line = GameObject.Instantiate(_LineGo, _LineGo.transform.parent);
                    RegisterUIDissolve(line);
                    _LineGos.Add(line);
                    txt = GameObject.Instantiate(_DescLevel3Txt, _DescLevel3Txt.transform.parent);
                    RegisterUIDissolve(txt);
                    _DescLevelTxts.Add(txt);
                }
                txt.text = descStr.ToString();
                txt.gameObject.SetActive(true);
                line.SetActive(true);
            }
        }
    }

    private async UniTask<IUIBattleUplevelWindowItem> CreateItem(int index, EnUIBattleUplevelWindowItemType type, IClassPool userData, CancellationToken token)
    {
        switch (type)
        {
            case EnUIBattleUplevelWindowItemType.SkillBall:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillBallItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillBallItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillBallItemUserData;
                    entityItem.Setting(data.ballId, data.star, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.SkillBallPart:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillBallPartItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillBallPartItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillBallPartItemUserData;
                    entityItem.Setting(data.partId, data.star, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.Attribute:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_AttributeItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_AttributeItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_AttributeItemUserData;
                    entityItem.Setting(data.attId, data.star, data.addValue, data.valueType, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.SkillBallFussion:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillBallFussionItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillBallFussionItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillBallFussionItemUserData;
                    entityItem.Setting(data.fussionId, () => OnClick_Item(index));
                    return entityItem;
                }
            case EnUIBattleUplevelWindowItemType.SkillPassivity:
                {
                    var (isCencel, entityItem) = await UIMgr.Instance.LoadWindowItemAsync<UIBattleUplevelWindow_SkillPassivityItem>(EnLoadTarget.Pre_UIBattleUplevelWindow_SkillPassivityItem, _SelectScrollRootRect, token)
                        .SuppressCancellationThrow();
                    if (isCencel)
                        throw new OperationCanceledException();
                    var data = userData as UIBattleUplevelWindow_SkillPassivityItemUserData;
                    entityItem.Setting(data.skillPassivityId, data.star, () => OnClick_Item(index));
                    return entityItem;
                }
            default:
                throw new Exception($"{type} create error");
        }
    }

    private void PlayAnim(int index)
    {
        var entityItem = _SelectItemEntitys[index];
        var toPos = entityItem.GetWorldPos();
        var toSizeDeltaY = entityItem.GetSizeDeltaY() + 30;
        DOTween.Kill(_MoveId);
        if (_SelectFrameRect.gameObject.activeSelf)
        {
            var fromPos = _SelectFrameRect.transform.position;
            var time = Vector3.Distance(fromPos, toPos) / 10;

            var fromSizeDelayY = _SelectFrameRect.rect.height;
            DOTween.To(() => 0f, slider =>
            {
                _SelectFrameRect.transform.position = Vector3.Lerp(fromPos, toPos, slider);

                var sizeDelayY = Mathf.Lerp(fromSizeDelayY, toSizeDeltaY, slider);
                _SelectFrameRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelayY);

            }, 1f, time)
                .SetId(_MoveId)
                .SetUpdate(true);
        }
        else
        {
            _SelectFrameRect.gameObject.SetActive(true);
            _SelectFrameRect.transform.position = toPos;

            _SelectFrameRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, toSizeDeltaY);
        }

        var descCount = entityItem.GetDescriptionList(EnDescriptionLevel.Level2, EnDescriptionLevel.Count).Count;
        if (descCount > 0)
        {
            if (!_DescRootGroup.gameObject.activeSelf)
            {
                _DescRootGroup.gameObject.SetActive(true);
                var from = 0f;
                var to = 1f;
                DOTween.To(() => 0f, slider =>
                {
                    _DescRootGroup.alpha = Mathf.Lerp(from, to, slider);

                }, 1f, 0.5f)
                    .SetId(_MoveId)
                    .SetUpdate(true);
            }
        }
        else
        {
            if (_DescRootGroup.gameObject.activeSelf)
            {
                _DescRootGroup.gameObject.SetActive(false);
            }
        }
    }
}
