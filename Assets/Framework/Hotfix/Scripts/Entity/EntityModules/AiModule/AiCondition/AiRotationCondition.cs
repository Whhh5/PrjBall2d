
    using System.Collections.Generic;
    using UnityEngine.PlayerLoop;

    public class AiRotationCondition : IAiCondition
    {

        public List<int> SerializeToIntArray()
        {
            var result = new List<int>();
            ISerializeToIntArray.SerializeToInt(ref result, in _Angle);
            ISerializeToIntArray.SerializeToInt(ref result, _ConditionType);
            return result;
        }

        public void DeserializeFromIntArray(int[] datas, int start, int count, ISerializeToIntArrayUserData userData)
        {
            var localIndex = 0;
            ISerializeToIntArray.ParseToValue(datas, start, count, ref localIndex, out _Angle);
            ISerializeToIntArray.ParseToValue(datas, start, count, ref localIndex, out _ConditionType);
        }

        private float _Angle = -1;
        private EnAiThanConditionType _ConditionType = EnAiThanConditionType.None;
        public EnTypeId GetTypeDefineId() => EnTypeId.AiRotationCondition;
        public EnAiConditionType GetConditionType() => EnAiConditionType.Rotation;

        private int _AiId;
        public void OnPoolDestroy()
        {
            ISerializeToIntArray.Release(ref _Angle);
            ISerializeToIntArray.Release(ref _ConditionType);
            _AiId = -1;
        }
        public void OnPoolInit(AiCommonUserData userData)
        {
            _AiId = userData.aiId;
        }


        public bool IsPass(in int inputId)
        {
            var entityId = AiMgr.Instance.GetAiEntityId(_AiId);
            var angle = EntityUtility.GetLocalForwardAngle(entityId, inputId);

            var result = _ConditionType switch
            {
                EnAiThanConditionType.GreaterThan => angle > _Angle,
                EnAiThanConditionType.LessThan => angle < _Angle,
                _ => false,
            };
            return result;
        }
    }
