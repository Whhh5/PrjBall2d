using System;
using UnityEngine;
using UnityEngine.UI;

public class UIStartGameWindowData : UIWindowData<UIStartGameWindow>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIStartGameWindow;
}
public class UIStartGameWindow : UIWindow<UIStartGameWindowData>
{
    [SerializeField] private Button _EnterBtn = null;

    protected override void Awake()
    {
        base.Awake();
    }
    public override void OnUnload()
    {
        base.OnUnload();
        UIMgr.Instance.RemoveBtnListener(_EnterBtn, EnterGame);
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        UIMgr.Instance.AddBtnListener(_EnterBtn, EnterGame);
    }

    private void EnterGame()
    {

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
