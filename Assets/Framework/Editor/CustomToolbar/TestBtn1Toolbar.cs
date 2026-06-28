


using Newtonsoft.Json;
using UnityEngine;

[CustomUnityToolbarAttribute("TestBtn1", EnCustomUnityToolbarType.Right, EnCustomUnityToolbarOrder.TestBtn1)]

public class TestBtn1Toolbar : ICustomUnityToolbar
{
    public void OnClick()
    {
        var info = new AiDecisionInfo();
        var datas = info.SerializeToIntArray();
        var jsonStr = JsonConvert.SerializeObject(datas);
        Debug.Log(jsonStr);

        var userData = new AiCommonUserData()
        {
            aiId = 2,
            startIndex = 0,
            arrParams = datas.ToArray(),
            count = datas.Count,
        };
        var info2 = new AiDecisionInfo();
        info2.DeserializeFromIntArray(userData.arrParams, userData.startIndex, userData.count, userData);
        Debug.Log(info2);


        var perInfo = new AiPerceptionInfos();
        var perDatas = perInfo.SerializeToIntArray();
        var perJsonStr = JsonConvert.SerializeObject(perDatas);
        Debug.Log(perJsonStr);
        var perUserData = new AiCommonUserData()
        {
            aiId = 3,
            startIndex = 0,
            arrParams = perDatas.ToArray(),
            count = perDatas.Count,
        };
        perInfo.DeserializeFromIntArray(perUserData.arrParams, perUserData.startIndex, perUserData.count, perUserData);
        Debug.Log(perInfo);
    }
}