using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GOData : IClassPoolDestroy
{
    public EnLoadTarget loadTarget;
    public GameObject go;
    public AsyncInstantiateOperation<GameObject> insOperation = null;
    public void OnPoolDestroy()
    {
        loadTarget = EnLoadTarget.None;
        go = null;
        insOperation = null;
    }
}
public class ABBGOMgr : Singleton<ABBGOMgr>
{
    private Dictionary<int, GOData> m_GOMap = new();
    public GameObject GetGo(int goID)
    {
        if (!m_GOMap.TryGetValue(goID, out var goData))
            return null;
        return goData.go;
    }
    public T GetGoCom<T>(in int goID)
    {
        var go = GetGo(goID);
        var com = go.GetComponent<T>();
        return com;
    }
    public async UniTask<int> CreateGOAsync(EnLoadTarget target, Transform parent = null)
    {
        var obj = await ABBLoadMgr.Instance.LoadAsync<GameObject>(target);
        if (obj == null)
            return -1;
        var goID = ABBUtil.GetTempKey();



        var goData = ClassPoolMgr.Instance.Pull<GOData>();
        m_GOMap.Add(goID, goData);

        goData.loadTarget = target;
        goData.insOperation = GameObject.InstantiateAsync(obj, parent);
        var ins = await goData.insOperation.ToUniTask();
        goData.go = ins[0];
        goData.insOperation = null;
        return goID;
    }
    public void DestroyGO(int goID)
    {
        if (!m_GOMap.TryGetValue(goID, out var goData))
            return;
        if (goData.insOperation != null)
        {
            goData.insOperation.Cancel();
            goData.insOperation = null;
        }
        else
        {
            GameObject.Destroy(goData.go);
        }
        var loadTarget = goData.loadTarget;
        m_GOMap.Remove(goID);
        ABBLoadMgr.Instance.Unload(loadTarget);

        ClassPoolMgr.Instance.Push(goData);
    }
}

