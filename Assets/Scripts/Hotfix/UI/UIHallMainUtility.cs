using System.Collections.Generic;
using System.Linq;


public static class UIHallMainUtility
{
    private class ItemData
    {
        public bool isShow;
        public bool isLock;
        public EnLoadTarget icon;
        public EnLoadTarget windowId;
        public int nameLangId;

        public ItemData(bool isShow, bool isLock, EnLoadTarget icon, int nameLangId, EnLoadTarget windowId)
        {
            this.isShow = isShow;
            this.isLock = isLock;
            this.icon = icon;
            this.windowId = windowId;
            this.nameLangId = nameLangId;
        }
    }
    private static readonly Dictionary<EnUIHallMainMenuItemId, ItemData> _ItemDatas = new()
    {
        {EnUIHallMainMenuItemId.Battle, new(true, false, EnLoadTarget.Sp_zhujiemian_zhsndou1, 20011, EnLoadTarget.Pre_UIHallMainBattleWindow) },
        {EnUIHallMainMenuItemId.Shop, new(true, false, EnLoadTarget.Sp_zhujiemian_shangdian1, 20021, EnLoadTarget.Pre_UIHallMainShopWindow) },
        {EnUIHallMainMenuItemId.Inlay, new(true, false, EnLoadTarget.Sp_zhujiemian_tianfu1, 20071, EnLoadTarget.Pre_UIHallMainInlayWindow) },
        {EnUIHallMainMenuItemId.Core, new(true, false, EnLoadTarget.Sp_zhujiemian_zhuangbei1, 20041, EnLoadTarget.Pre_UIHallMainCoreWindow) },
        {EnUIHallMainMenuItemId.Character, new(true, false, EnLoadTarget.Sp_zhujiemian_kuaijiefangshi, 20051, EnLoadTarget.Pre_UIHallMainCharacterWindow) },
        {EnUIHallMainMenuItemId.Item2, new(true, false, EnLoadTarget.Sp_zhujiemian_kuaijiefangshi, 20061, EnLoadTarget.Pre_UIHallMainDebugWindow) },
    };

    public static List<EnUIHallMainMenuItemId> GetMenuItemList()
    {
        return _ItemDatas.Keys.ToList().FindAll(item => _ItemDatas[item].isShow);
    }
    public static bool IsLock(EnUIHallMainMenuItemId itemId)
    {
        var data = _ItemDatas[itemId];
        return data.isLock;
    }
    public static EnLoadTarget GetIconId(EnUIHallMainMenuItemId itemId)
    {
        var data = _ItemDatas[itemId];
        return data.icon;
    }
    public static EnLoadTarget GetWindowId(EnUIHallMainMenuItemId itemId)
    {
        var data = _ItemDatas[itemId];
        return data.windowId;
    }
    public static string GetName(EnUIHallMainMenuItemId itemId)
    {
        var data = _ItemDatas[itemId];
        var nameStr = LanguageUtility.GetLanguage(data.nameLangId);
        return nameStr;
    }
}