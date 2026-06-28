using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;

    private int _CurControllerEntityId = -1;
    private Vector3 _MoveDir = Vector3.zero;

    public override async UniTask OnEnableAsync()
    {
        await base.OnEnableAsync();

        ABBInputMgr.Instance.AddListaner(KeyCode.W, InputClick_W);
        ABBInputMgr.Instance.AddListaner(KeyCode.S, InputClick_S);
        ABBInputMgr.Instance.AddListaner(KeyCode.A, InputClick_A);
        ABBInputMgr.Instance.AddListaner(KeyCode.D, InputClick_D);
    }

    public override void OnDisable()
    {
        ABBInputMgr.Instance.RemoveListaner(KeyCode.W, InputClick_W);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.S, InputClick_S);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.A, InputClick_A);
        ABBInputMgr.Instance.RemoveListaner(KeyCode.D, InputClick_D);

        base.OnDisable();
    }

    public void SetControllerEntity(in int entityId)
    {
        if (entityId > 0)
            SceneEntityMgr.Instance.RemoveControllerType(in entityId, EnSceneEntityControllerType.Player);
        _CurControllerEntityId = entityId;
        CameraMgr.Instance.SetCameraTarget(in entityId);
        SceneEntityMgr.Instance.AddControllerType(in entityId, EnSceneEntityControllerType.Player);
    }


    private void InputClick_W()
    {
        _MoveDir += Vector3.forward;
    }

    private void InputClick_S()
    {
        _MoveDir += -Vector3.forward;
    }

    private void InputClick_A()
    {
        _MoveDir += -Vector3.right;
    }

    private void InputClick_D()
    {
        _MoveDir += Vector3.right;
    }


    public override void Update()
    {
        base.Update();

        if (!SceneEntityMgr.Instance.EntityIsValid(in _CurControllerEntityId))
            return;

        if(!SceneEntityMgr.Instance.IsCanController(in _CurControllerEntityId, EnSceneEntityControllerType.Player))
            return;


        if (Vector3.SqrMagnitude(_MoveDir) > 0.1f)
        {
            var cameraForward = CameraMgr.Instance.GetCameraForward();
            var moveDir = EntityUtility.CalculateMoveDir(in _CurControllerEntityId, in _MoveDir, in cameraForward);

            EntityUtility.RotationEntity(in _CurControllerEntityId, in moveDir);

            var isShift = ABBInputMgr.Instance.GetKey(KeyCode.LeftShift);
            EntityUtility.MoveEntity(in _CurControllerEntityId, in moveDir, isShift ? 1f : 0.5f);
        }
        else
        {
            var monsterCfg = MonsterUtility.GetMonsterCfg(in _CurControllerEntityId);
            if (SceneEntityMgr.Instance.HasCmd(in _CurControllerEntityId, in monsterCfg.nRunCmdId))
                SceneEntityMgr.Instance.RemoveCmd(in _CurControllerEntityId, in monsterCfg.nRunCmdId);
            if (SceneEntityMgr.Instance.HasCmd(in _CurControllerEntityId, in monsterCfg.nWalkCmdId))
                SceneEntityMgr.Instance.RemoveCmd(in _CurControllerEntityId, in monsterCfg.nWalkCmdId);
        }


        _MoveDir = Vector3.zero;
    }
}