using DG.Tweening;
using UnityEngine;


public class GameEffectFireLineEntityData : GameEntityData<GameEffectFireLineEntity>, IUpdate
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_GameEffectFireLineEntity;

    private GamePhysicsHit[] _Hits = new GamePhysicsHit[1];
    private Vector3 _Point1;
    private Vector3 _Point2;
    private Vector3 _Point3;
    private bool _Point3Active;

    public override void SetPosition(in Vector3 worldPos)
    {
        var posZ = GameMapUtility.GetCurMapDepth() + (int)EnGameEntityOrderLayer.LineUp;
        base.SetPosition(new Vector3(worldPos.x, worldPos.y, posZ));
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();

        _Entity.SetPoint3Active(_Point3Active);
        _Entity.SetPoint1(_Point1);
        _Entity.SetPoint2(_Point2);
        if (_Point3Active)
            _Entity.SetPoint3(_Point3);
        UpdateMgr.Instance.Registener(this);
    }
    public override void OnGODestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.OnGODestroy();
    }
    public void Update()
    {
        var firePos = GameBattleInputMgr.Instance.GetFirePos();
        var dir = GameBattleInputMgr.Instance.GetFireDirection();
        var mapId = GameMapMgr.Instance.GetCurMapId();
        SetPoint1(firePos);

        var eagePointCount = GameMapUtility.GetMapEagePoint(mapId, in firePos, in dir, EnDirection.All & ~EnDirection.Bottom, ref _Hits);
        if (eagePointCount == 0)
            return;
        var mapPoint1 = _Hits[0].point;
        var normal = _Hits[0].normal;
        var mapDir = _Hits[0].dir;

        var btmPointCount = GameMapUtility.GetMapEagePoint(mapId, in firePos, in dir, EnDirection.Bottom, ref _Hits);
        if (btmPointCount == 0)
            return;
        var startPoint = _Hits[0].point;
        var count = GamePhysicsUtility.TryLineCastSort(in startPoint, in mapPoint1, 0, ref _Hits);
        if (count > 0)
        {
            var hit = _Hits[0];
            if (TryGetEntityEageCastPoint(hit.entityId, in startPoint, in dir, out var entityPoint))
            {
                SetPoint2(in entityPoint);
                SetPoint3Active(false);
            }
            else
            {

            }
        }
        else
        {
            SetPoint2(mapPoint1);
            var dir2 = Vector2.Reflect(dir, -normal);
            var validEdge = EnDirection.All & ~mapDir;
            if ((mapDir & EnDirection.Top) == 0)
            {
                validEdge &= ~EnDirection.Bottom;
            }
            var eagePointCount2 = GameMapUtility.GetMapEagePoint(mapId, in mapPoint1, in dir2, in validEdge, ref _Hits);
            if (eagePointCount2 > 0)
            {
                var mapPoint2 = _Hits[0].point;

                var entityCount2 = GamePhysicsUtility.TryLineCastSort(in mapPoint1, in mapPoint2, 0, ref _Hits);
                if (entityCount2 > 0)
                {
                    var entityId = _Hits[0].entityId;
                    if (TryGetEntityEageCastPoint(entityId, in mapPoint1, in dir2, out var entityPoint))
                    {
                        SetPoint3Active(true);
                        SetPoint3(in entityPoint);
                    }
                    else
                    {

                    }
                }
                else
                {
                    SetPoint3Active(true);
                    SetPoint3(in mapPoint2);
                }
            }
            else
            {

            }
        }
    }
    private bool TryGetEntityEageCastPoint(int entityId, in Vector2 startPoint, in Vector2 dir, out Vector2 point)
    {
        var mapId = GameMapMgr.Instance.GetCurMapId();
        var gridSize = GameMapUtility.GetMapGridSize(mapId);
        var pointInfo = EntityMgr.Instance.GetEntityData<IEntityGridPointInfo>(entityId);
        var entityPos = pointInfo.GetMainGridPos();
        ref readonly var arrPos = ref pointInfo.GetGridIndexs();
        ref readonly var arrValidEages = ref pointInfo.GetGridValidEdges();

        point = default;
        var minDis = float.MaxValue;
        for (int i = 0; i < arrPos.Length; i += 2)
        {
            ref readonly var row = ref arrPos[i];
            ref readonly var col = ref arrPos[i + 1];

            var testLbPos = entityPos + new Vector2(col - 0.5f, row - 0.5f) * gridSize;
            var testRuPos = entityPos + new Vector2(col + 0.5f, row + 0.5f) * gridSize;
            var validEage = (EnDirection)arrValidEages[i / 2];
            var posCount = GamePhysicsUtility.CastBoxEage(in testLbPos, in testRuPos, in startPoint, in dir, in validEage, ref _Hits, 1);
            if (posCount > 0)
            {
                var point2 = _Hits[0].point;
                var testDis = Vector2.SqrMagnitude(point2 - startPoint);
                if (testDis < minDis)
                {
                    minDis = testDis;
                    point = point2;
                }
            }
        }
        if (minDis != float.MaxValue)
            return true;
        return false;
    }

    public void SetPoint1(in Vector2 pos)
    {
        _Point1 = pos;
        if (IsLoadSuccess)
            _Entity.SetPoint1(pos);
    }
    public void SetPoint2(in Vector2 pos)
    {
        _Point2 = pos;
        if (IsLoadSuccess)
            _Entity.SetPoint2(pos);
    }
    public void SetPoint3(in Vector2 pos)
    {
        _Point3 = pos;
        if (IsLoadSuccess)
            _Entity.SetPoint3(pos);
    }
    public void SetPoint3Active(in bool active)
    {
        _Point3Active = active;
        if (IsLoadSuccess)
            _Entity.SetPoint3Active(active);
    }
}
public class GameEffectFireLineEntity : GameEntity
{
    [SerializeField]
    private Transform _Point1;
    [SerializeField]
    private Transform _Point2;
    [SerializeField]
    private Transform _Point3;
    [SerializeField]
    private LineRenderer _Line1;
    [SerializeField]
    private LineRenderer _Line2;

    private readonly EnGameEntityOrderLayer _LineLayer = EnGameEntityOrderLayer.LineDown;
    public void SetPoint1(in Vector2 pos)
    {
        _Point1.position = pos;
        _Line1.SetPosition(0, new Vector3(pos.x, pos.y, GameMapUtility.GetCurMapDepth() + (int)_LineLayer));
    }
    public void SetPoint2(in Vector2 pos)
    {
        _Point2.position = pos;
        _Line1.SetPosition(1, new Vector3(pos.x, pos.y, GameMapUtility.GetCurMapDepth() + (int)_LineLayer));
        _Line2.SetPosition(0, new Vector3(pos.x, pos.y, GameMapUtility.GetCurMapDepth() + (int)_LineLayer));
    }
    public void SetPoint3(in Vector2 pos)
    {
        _Point3.position = pos;
        _Line2.SetPosition(1, new Vector3(pos.x, pos.y, GameMapUtility.GetCurMapDepth() + (int)_LineLayer));
    }
    public void SetPoint3Active(in bool active)
    {
        if (_Point3.gameObject.activeSelf == active)
            return;
        _Point3.gameObject.SetActive(active);
        _Line2.gameObject.SetActive(active);
    }

    protected override void Update()
    {
        base.Update();

        var add = 180 * GameUtility.GetGameTimeDelta() * Vector3.forward;
        _Point1.Rotate(add);
        _Point2.Rotate(-add * 2);
        if (_Point3.gameObject.activeSelf)
            _Point3.Rotate(add * 3);
    }
}
