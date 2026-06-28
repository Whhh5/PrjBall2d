using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

[GameStartAttribute]
[Preserve]
public class GameManager : MonoBehaviour
{
    private int _GameManagerGoID = -1;
    #region 生命周期管理
    private EnManagerFuncType _CurInitSatge = EnManagerFuncType.None;
    private readonly Dictionary<EnManagerFuncType, List<Singleton>> _ManagerList = new();
    private void SubManager(Singleton f_Manager)
    {
        for (int i = 0; i < 4; i++)
        {
            var type = (EnManagerFuncType)(1 << i);
            if ((f_Manager.FuncType & type) != type)
                continue;
            if (!_ManagerList.TryGetValue(type, out var list))
            {
                list = new();
                _ManagerList.Add(type, list);
            }
            list.Add(f_Manager);
        }

        foreach (var item in _ManagerList)
        {
            item.Value.Sort((item1, item2) =>
            {
                if (item1.SingletonOrder == item2.SingletonOrder)
                    return 0;
                else if (item1.SingletonOrder < item2.SingletonOrder)
                    return -1;
                else
                    return 1;
            });
        }
    }
    private void Register()
    {
        Assembly[] hotUpdateAsss = AppDomain.CurrentDomain.GetAssemblies();
        var parentType = typeof(Singleton<>);
        var instanceStr = "Instance";
        foreach (var assembly in hotUpdateAsss)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (!typeof(Singleton).IsAssignableFrom(type))
                    continue;
                if (parentType == type)
                    continue;
                if (typeof(Singleton) == type)
                    continue;
                var specificParentType = parentType.MakeGenericType(type);
                var instanceField = specificParentType.GetField(instanceStr, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                var instance = instanceField.GetValue(null);
                var monoBehaviour = instance as Singleton;
                SubManager(monoBehaviour);
                Debug.Log($"Register ======> {type}");
            }
        }
    }

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
        Register();
    }
    private async void Start()
    {
        Debug.Log(" --------------------- start Awake -----------------------");
        // ���� awake
        if (_ManagerList.TryGetValue(EnManagerFuncType.AwakeAsync, out var list))
        {
            var count = list.Count;
            foreach (var item in list)
            {
                await item.AwakeAsync();
            }
        }
        _CurInitSatge |= EnManagerFuncType.AwakeAsync;
        Debug.Log(" --------------------- Awake end -----------------------");


        Debug.Log(" --------------------- start Start -----------------------");
        if (_ManagerList.TryGetValue(EnManagerFuncType.OnEnableAsync, out list))
        {
            var count = list.Count;
            foreach (var item in list)
            {
                await item.OnEnableAsync();
            }
        }
        _CurInitSatge |= EnManagerFuncType.OnEnableAsync;
        Debug.Log(" ---------------------  Start end -----------------------");


        _GameManagerGoID = await ABBGOMgr.Instance.CreateGOAsync(EnLoadTarget.Pre_GameManager, null);
    }
    private void OnDestroy()
    {

    }
    private void Update()
    {
        if ((_CurInitSatge & EnManagerFuncType.OnEnableAsync) == EnManagerFuncType.None)
        {
            return;
        }
        // update
        if (_ManagerList.TryGetValue(EnManagerFuncType.Update, out var list))
        {
            foreach (var item in list)
            {
                item.Update();
            }
        }
    }

    private void FixedUpdate()
    {
        if ((_CurInitSatge & EnManagerFuncType.OnEnableAsync) == EnManagerFuncType.None)
            return;

        // update
        if (_ManagerList.TryGetValue(EnManagerFuncType.FixedUpdate, out var list))
        {
            foreach (var item in list)
            {
                item.FixedUpdate();
            }
        }
    }

    private void OnApplicationQuit()
    {
        // start
        Debug.Log(" --------------------- OnDisable -----------------------");
        _CurInitSatge &= ~EnManagerFuncType.OnEnableAsync;
        if (_ManagerList.TryGetValue(EnManagerFuncType.OnEnableAsync, out var list))
        {
            for (int i = 1; i <= list.Count; i++)
            {
                list[^i].OnDisable();
            }
        }
        Debug.Log(" --------------------- OnDisable -----------------------");
        // awake
        Debug.Log(" --------------------- Destroy -----------------------");
        _CurInitSatge &= ~EnManagerFuncType.AwakeAsync;
        if (_ManagerList.TryGetValue(EnManagerFuncType.AwakeAsync, out list))
        {
            for (int i = 1; i <= list.Count; i++)
            {
                list[^i].Destroy();
            }
        }
        Debug.Log(" --------------------- ֹͣ Destroy end -----------------------");
        //ABBGOMgr.Instance.DestroyGO(m_GameManagerGoID);
        _GameManagerGoID = -1;
    }
    #endregion
}
