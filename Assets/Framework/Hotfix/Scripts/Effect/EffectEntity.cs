using UnityEngine;


public abstract class EffectEntityData : GameEntityData<EffectEntity>
{
    public sealed override EnLoadTarget LoadTarget
    {
        get
        {
            var effectCfg = GameSchedule.Instance.GetEffectCfg0(_EffectID);
            return (EnLoadTarget)effectCfg.nAssetID;
        }
    }
    private int _EffectID = -1;
    public bool IsPlaying { get; private set; } = false;

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _EffectID
            = -1;
        IsPlaying = false;
    }
    public override void OnPoolInit(GameEntityDataUserData userData)
    {
        base.OnPoolInit(userData);

        var data = userData as EffectDataUserData;
        _EffectID = data.effectCfgID;
    }

    public void Play()
    {
        SetIsPlaying(true);
    }
    public void Stop()
    {
        SetIsPlaying(false);
    }
    private void SetIsPlaying(bool isPlaying)
    {
        IsPlaying = isPlaying;
        if (_IsLoadSuccess)
            _Entity.SetIsPlaying();
    }
    public float GetMaxTime()
    {
        var cfg = GameSchedule.Instance.GetEffectCfg0(_EffectID);
        return cfg.fDelayDestroyTime;
    }

}

public abstract class EffectEntity : GameEntity<EffectEntityData>
{
    [SerializeField, AutoReference]
    private ParticleSystem _MainParticleSystem = null;

    private float _SingletonMaxTime = 0;
    protected override void Awake()
    {
        base.Awake();
        UpdateMaxTime();

        //_MainParticleSystem.Simulate(0, true, false);
    }

    public override void LoadCompeletion()
    {
        base.LoadCompeletion();

        SetIsPlaying();
    }

    public void SetIsPlaying()
    {
        if (_GameEntityData.IsPlaying)
            Play();
        else
            Stop();
    }
    public void UpdateMaxTime()
    {
        var parComs = GetComponentsInChildren<ParticleSystem>(true);
        _SingletonMaxTime = 0;
        foreach (var item in parComs)
        {
            var parTime = GetParticleTime(item);
            var time = Mathf.Max(item.main.duration, parTime);
            _SingletonMaxTime = Mathf.Max(_SingletonMaxTime, time);
        }
    }
    public void Play()
    {
        _MainParticleSystem.Play();
    }
    public void Stop()
    {
        _MainParticleSystem.Stop();
    }
    public void SetSimulationTime(float time)
    {
        _MainParticleSystem.Simulate(time, true, true);
    }
    public float GetSingletonMaxLifeTime()
    {
        return _SingletonMaxTime;
    }
    private float GetParticleTime(ParticleSystem parSystem)
    {
        // 获取粒子系统的主模块
        var main = parSystem.main;

        // 获取粒子的起始生命周期属性
        ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;

        float maxLifetime;

        // 根据起始生命周期的模式确定最大生命周期
        switch (startLifetime.mode)
        {
            case ParticleSystemCurveMode.Constant:
                // 常量模式，最大生命周期就是该常量值
                maxLifetime = startLifetime.constant;
                break;
            case ParticleSystemCurveMode.TwoConstants:
                // 两个常量模式，最大生命周期是较大的那个常量值
                maxLifetime = Mathf.Max(startLifetime.constantMin, startLifetime.constantMax);
                break;
            case ParticleSystemCurveMode.Curve:
                // 曲线模式，在曲线的定义域内找到最大值
                maxLifetime = EvaluateMaxCurveValue(startLifetime.curve);
                break;
            case ParticleSystemCurveMode.TwoCurves:
                // 两条曲线模式，在两条曲线的定义域内找到最大值
                float maxCurve1 = EvaluateMaxCurveValue(startLifetime.curveMin);
                float maxCurve2 = EvaluateMaxCurveValue(startLifetime.curveMax);
                maxLifetime = Mathf.Max(maxCurve1, maxCurve2);
                break;
            default:
                maxLifetime = 0f;
                break;
        }

        return maxLifetime;
    }
    // 辅助方法：评估曲线在定义域内的最大值
    private float EvaluateMaxCurveValue(AnimationCurve curve)
    {
        float maxValue = float.MinValue;
        for (float t = 0f; t <= 1f; t += 0.01f)
        {
            float value = curve.Evaluate(t);
            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        return maxValue;
    }
}
