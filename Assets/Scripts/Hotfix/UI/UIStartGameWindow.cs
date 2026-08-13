using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.AudioSettings;
using Object = UnityEngine.Object;

public class UIStartGameWindowData : UIWindowData<UIStartGameWindow>
{
    public override EnUIWindowType WindowType => EnUIWindowType.Window;

    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_UIStartGameWindow;
}
public class UIStartGameWindow : UIWindow
{
    [SerializeField] private Button _EnterBtn = null;
    [SerializeField] private Button _RestartBtn = null;
    [SerializeField] private Button _ExitBtn = null;


    private int _BattleMainWindowId;

    public override void OnUnload()
    {
        base.OnUnload();
        UIMgr.Instance.RemoveBtnListener(_EnterBtn, EnterGame);
        UIMgr.Instance.RemoveBtnListener(_RestartBtn, RestartGame);
        UIMgr.Instance.RemoveBtnListener(_ExitBtn, ExitGame);
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        UIMgr.Instance.AddBtnListener(_EnterBtn, EnterGame);
        UIMgr.Instance.AddBtnListener(_RestartBtn, RestartGame);
        UIMgr.Instance.AddBtnListener(_ExitBtn, ExitGame);

        UpdateButtonStatus();
    }

    private void EnterGame()
    {
        //var content = await ServerMgr.Instance.REQ_DOWNLOAD_FILE("path/path");
        //Debug.LogWarning(content);

        //return;
        var mapId = 1;
        GameMapMgr.Instance.EnterMap(mapId);

        _BattleMainWindowId = UIMgr.Instance.ShowWindow<UIBattleMainWindowData>();

        UpdateButtonStatus();
    }
    private void RestartGame()
    {
        UIMgr.Instance.HideWindow(_BattleMainWindowId);
        GameMapMgr.Instance.ReenterMap();
        _BattleMainWindowId = UIMgr.Instance.ShowWindow<UIBattleMainWindowData>();

        UpdateButtonStatus();
    }
    private void ExitGame()
    {
        UIMgr.Instance.HideWindow(_BattleMainWindowId);
        GameMapMgr.Instance.ExitMap();

        UpdateButtonStatus();
        _BattleMainWindowId = -1;
    }

    private void UpdateButtonStatus()
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        _EnterBtn.interactable = mapId <= 0;
        _RestartBtn.interactable
            = _ExitBtn.interactable
            = mapId > 0;
    }

    private float _LastFpsUpdateTime = 0;
    private int _Fps = 0;
    void OnGUI()
    {
        if (Time.time - _LastFpsUpdateTime > 1f)
        {
            if (Time.deltaTime > 0)
            {
                _Fps = Mathf.RoundToInt(1 / Time.deltaTime);
                _LastFpsUpdateTime = Time.time;
            }
        }
        GUI.Label(
            new Rect(10, 10, 300, 30),
            $"{_Fps:F0}",
            new GUIStyle
            {
                fontSize = 70,
                normal = new GUIStyleState { textColor = Color.yellow }
            }
        );
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
