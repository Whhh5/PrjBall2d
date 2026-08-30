using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStartGameWindowData : UIWindowData<UIStartGameWindow>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIStartGameWindow;
}
public class UIStartGameWindow : UIWindow
{
    [SerializeField]
    private Button _EnterBtn = null;

    [SerializeField]
    private Button _SystemBtn = null;
    [SerializeField]
    private Button _SystemItem = null;
    private readonly Dictionary<string, Button> _SystemItems = new();

    public override void OnUnload()
    {
        base.OnUnload();
        UIMgr.Instance.RemoveBtnListener(_EnterBtn, EnterGame);
    }
    public override void LoadCompletion()
    {
        base.LoadCompletion();
        UIMgr.Instance.AddBtnListener(_EnterBtn, EnterGame);

        _SystemItem.gameObject.SetActive(false);
        _SystemBtn.AddListener(() => _SystemItem.transform.parent.gameObject.SetActive(!_SystemItem.transform.parent.gameObject.activeSelf));
    }

    private void EnterGame()
    {
        UIMgr.Instance.HideWindow(_WindowId);
        UIMgr.Instance.ShowLobby();
    }

    protected override void Update()
    {
        base.Update();

        if (_SystemItem.transform.parent.gameObject.activeSelf)
        {
            var systems = GameBattleMgr.Instance.GetBattleSystems();
            foreach (var (key, value) in systems)
            {
                if (!_SystemItems.TryGetValue(key, out var btn))
                {
                    var clickKey = key;
                    btn = GameObject.Instantiate(_SystemItem, _SystemItem.transform.parent);
                    btn.gameObject.SetActive(true);
                    _SystemItems.Add(clickKey, btn);
                    btn.onClick.AddListener(() => OnClick_SystemItem(clickKey));
                    var sys = GameBattleMgr.Instance.GetSystem(clickKey);
                    var txtCom = btn.transform.GetComponentInChildren<TextMeshProUGUI>();
                    txtCom.text = $"{sys.GetType().Name.Replace("Game", "").Replace("System", "").Replace("Battle", "")}";
                }
            }
        }
    }

    private void OnClick_SystemItem(string sysKey)
    {
        var btn = _SystemItems[sysKey];
        var isEnable = GameBattleMgr.Instance.IsEnableSystem(sysKey);
        if (isEnable)
        {
            GameBattleMgr.Instance.DisableSystem(sysKey);
        }
        else
        {
            GameBattleMgr.Instance.EnableSystem(sysKey);
        }
        var img = btn.GetComponent<Image>();
        img.color = isEnable ? Color.red : Color.green;
    }
    //private void EnterGame()
    //{
    //    _EnterBtn.gameObject.SetActive(false);
    //    _EntityId = MonsterUtility.CreateMonster(8);
    //    EntityMgr.Instance.LoadEntity(_EntityId);

    //    PlayerController.Instance.SetControllerEntity(_EntityId);
    //}

    //private int _EntityId = -1;
    //private int _AiId = -1;
    //protected override void Update()
    //{
    //    base.Update();

    //    if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        if (_EntityId > 0)
    //        {
    //            if (_AiId < 0)
    //            {
    //                _AiId = AiMgr.Instance.AddEntityAi(in _EntityId);
    //            }
    //            else
    //            {
    //                AiMgr.Instance.RemoveEntityAi(_EntityId);
    //                _AiId = -1;
    //            }
    //        }

    //    }
    //    if (Input.GetKey(KeyCode.Alpha1) && Input.GetMouseButtonDown(0))
    //    {
    //        var entityId = MonsterUtility.CreateMonster(9);

    //        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out var hit, (int)EnGameLayerInt.Ground))
    //        {
    //            EntityMgr.Instance.LoadEntity(entityId);
    //            EntityMgr.Instance.SetEntityWorldPos(in entityId, hit.point);
    //        }
    //    }
    //}
}
