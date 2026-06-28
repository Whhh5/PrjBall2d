using DG.DemiEditor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SkillEditorWindow : EditorWindow
{
    public static bool IsInteract = true;
    private static bool _IsDebugModel = false;
    public static bool IsDebugModel => _IsDebugModel;
    private bool _DebugSerializeData = false;
    private bool _IsHistoryPanelShow = false;

    private ISkillEditor _CurSkillEditor = null;
    private Vector2 _ContentScrollPos = Vector2.zero;
    private int _CurMonsterId = 0;
    private int _CurSkillId = 0;
    private readonly Dictionary<int, ISkillEditor> _SkillEditorCache = new();
    private Vector2 _OnGuiLeftMonsterMenuScrollPos;
    private readonly bool[] _IsShowMonsterMenu = new bool[10];
    private static GameObject _DebugModelGameObject = null;

    private readonly List<bool> _MonsterIsShowSkillList = new List<bool>();
    private static int _UpdateFrame = 0;
    public static int FrameCount => _UpdateFrame;

    private string GetHistorySaveDirectory()
    {
        return ABBUtil.GetUnityCustomCachePath("Skill_Editor");
    }


    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }
    private void OnDisable()
    {
        ExitDebugModel();
        EditorApplication.update -= OnEditorUpdate;
        _IsDebugModel
            = _DebugSerializeData
            = _IsHistoryPanelShow
            = false;
        _CurSkillId
            = _CurMonsterId
            = _UpdateFrame
            = 0;
        _OnGuiLeftMonsterMenuScrollPos
            = _ContentScrollPos
            = Vector2.zero;
        _CurSkillEditor = null;

        _MonsterIsShowSkillList.Clear();
        _SkillEditorCache.Clear();
    }
    private void OnEditorUpdate()
    {
        // 每帧刷新窗口
        Repaint();
    }
    private void OnGUI()
    {
        // 左侧
        _UpdateFrame++;
        GUILayout.Space(3);
        GUILayout.Button("", GUILayout.Height(3), GUILayout.ExpandWidth(true));
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        {
            // 左
            OnGuiLeftMonsterMenu();

            GUILayout.Button("", GUILayout.Width(3), GUILayout.ExpandHeight(true));
            GUILayout.Space(10);

            if (_IsHistoryPanelShow)
            {
                OnGuiLeftHistoryPanel();
                GUILayout.Button("", GUILayout.Width(3), GUILayout.ExpandHeight(true));
                GUILayout.Space(10);
            }

            // 中
            EditorGUILayout.BeginVertical();
            {
                OnGuiCenterMonsterSkillSetting();
                //  中下
                OnGuiCenterBottom();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }


    private void OnGuiLeftMonsterMenu()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(SkillEditorDefine.FieldWidth));
        {
            DrawControllerList();
            DrawMonsterList();
        }
        EditorGUILayout.EndVertical();
    }//'E:\My\mars-editor2\Misc\Cache\Skill_Editor\cTo_SkillCfg.xlsx'.
    private void OnGuiLeftHistoryPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(SkillEditorDefine.FieldWidth * 2));
        {
            GUILayout.Label("历史记录面板");
            GUILayout.Button("", GUILayout.ExpandWidth(true), GUILayout.Height(SkillEditorDefine.LineSize));
            GUILayout.Space(10);

            var dirs = Directory.GetDirectories(GetHistorySaveDirectory());
            for (var i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var saveKey = dir.FileOrDirectoryName();
                var saveTimeDate = ABBUtil.StringToDateTime(saveKey);
                var interval = DateTime.Now - saveTimeDate;
                var btnName = interval.TotalHours >= 24
                    ? saveTimeDate.ToString(CultureInfo.InvariantCulture) // $"{interval.TotalHours / 24:F1}天前"
                    : interval.TotalHours >= 1
                        ? $"{interval.TotalHours:F1}小时前"
                        : interval.TotalMinutes >= 1
                            ? $"{interval.TotalMinutes:F1}分钟前"
                            : $"{interval.TotalSeconds:F1}秒前";

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("回到之前", GUILayout.Width(SkillEditorDefine.FieldWidth / 2)))
                    {
                        RevertHistory(saveKey, GetFromHistoryFileName());
                        ExcelEditorUtil.ClearCache<SkillCfg>();
                        ReloadSkillData();
                    }
                    if (GUILayout.Button(new GUIContent(btnName, saveKey)))
                    {
                        EditorUtil.OpenFolder(dir);
                    }
                    if (GUILayout.Button("回到之后", GUILayout.Width(SkillEditorDefine.FieldWidth / 2)))
                    {
                        RevertHistory(saveKey, GetToHistoryFileName());
                        ExcelEditorUtil.ClearCache<SkillCfg>();
                        ReloadSkillData();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawControllerList()
    {
        _IsShowMonsterMenu[1] = EditorGUILayout.BeginFoldoutHeaderGroup(_IsShowMonsterMenu[1], "功能列表");
        if (_IsShowMonsterMenu[1])
        {
            var style = _IsDebugModel
                ? new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter }
                : new GUIStyle(GUI.skin.button);
            if (GUILayout.Button("调试模式", style, GUILayout.Height(50)))
            {
                if (_IsDebugModel)
                    ExitDebugModel();
                else
                    EnterDebugModel();
            }

            var style2 = _DebugSerializeData
                ? new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter }
                : new GUIStyle(GUI.skin.button);
            if (GUILayout.Button("序列化调试", style2, GUILayout.Height(30)))
            {
                _DebugSerializeData = !_DebugSerializeData;
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("保存", style2, GUILayout.Height(30)))
                {

                }
                if (GUILayout.Button("保存全部", style2, GUILayout.Height(30)))
                {
                    Save();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("打开保存历史面板", style2, GUILayout.Height(30)))
                {
                    _IsHistoryPanelShow = !_IsHistoryPanelShow;
                }
                if (GUILayout.Button("打开历史存文件夹", style2, GUILayout.Height(30)))
                {
                    EditorUtil.OpenFolder(GetHistorySaveDirectory());
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawMonsterList()
    {
        _IsShowMonsterMenu[0] = EditorGUILayout.BeginFoldoutHeaderGroup(_IsShowMonsterMenu[0], "怪物列表");
        if (_IsShowMonsterMenu[0])
        {
            _OnGuiLeftMonsterMenuScrollPos = EditorGUILayout.BeginScrollView(_OnGuiLeftMonsterMenuScrollPos);
            {
                var monsterCfgCount = ExcelEditorUtil.GetCfgCount<MonsterCfg>();
                for (var i = 0; i < monsterCfgCount; i++)
                {
                    if (_MonsterIsShowSkillList.Count <= i)
                        _MonsterIsShowSkillList.Add(false);
                    var monsterCfg = ExcelEditorUtil.GetCfgByIndex<MonsterCfg>(i);

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (_CurMonsterId == monsterCfg.nMonsterID)
                        {
                            GUILayout.Toggle(true, "", GUILayout.Width(20));
                        }

                        if (GUILayout.Button($"{monsterCfg.nMonsterID}:{monsterCfg.strName}"))
                        {
                            _MonsterIsShowSkillList[i] = !_MonsterIsShowSkillList[i];
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (!_MonsterIsShowSkillList[i])
                        continue;
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Button("", GUILayout.Width(SkillEditorDefine.LineSize),
                            GUILayout.ExpandHeight(true));
                        GUILayout.Space(SkillEditorDefine.DefineSpace);

                        DrawMonsterSkillList(monsterCfg.nMonsterID);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawMonsterSkillList(int monsterId)
    {
        var monsterCfg = ExcelEditorUtil.GetCfg<MonsterCfg>(monsterId);
        EditorGUILayout.BeginVertical();
        {
            var skillGroup = monsterCfg.arrSkillGroup;
            for (var j = 0; j < skillGroup.Length; j++)
            {
                var skillId = skillGroup[j];
                var skillCfg = ExcelEditorUtil.GetCfg<SkillCfg>(skillId);
                var isToggle = _CurMonsterId == monsterCfg.nMonsterID && skillId == _CurSkillId;
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Toggle(isToggle, "", GUILayout.Width(20));
                    if (GUILayout.Button($"{skillId}:{skillCfg.strName}", GUILayout.ExpandWidth(true)))
                    {
                        SetSkillEditorContent(monsterCfg.nMonsterID, skillCfg.nSkillID);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Skill"))
            {
                AddNewSkill(in monsterId);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void AddNewSkill(in int monsterId)
    {
        var monsterCfg = ExcelEditorUtil.GetCfg<MonsterCfg>(monsterId);
        var skillGroup = monsterCfg.arrSkillGroup;
        var menuContents = new List<GUIContent>();
        for (var i = EnSkillType.None + 1; i < EnSkillType.EnumCount; i++)
        {
            menuContents.Add(new GUIContent($"{i}"));
        }

        var rect = new Rect()
        {
            position = Event.current.mousePosition,
        };
        EditorUtility.DisplayCustomMenu(rect, menuContents.ToArray(), -1, (_, dataList, selectIndex) =>
        {
            var curSkillCount = ExcelEditorUtil.GetCfgCount<SkillCfg>();
            var skillId = -1;
            for (var i = 1; i <= curSkillCount + 1; i++)
            {
                var skillCfg = ExcelEditorUtil.GetCfg<SkillCfg>(i);
                if (skillCfg != null)
                    continue;
                skillId = i;
                break;
            }
            if (skillId <= 0)
                return;
            var skillType = Enum.Parse<EnSkillType>(dataList[selectIndex]);
            var skillTypeId = skillType switch
            {
                EnSkillType.Link => EnTypeId.SkillLinkPlayableAdapter,
                EnSkillType.Random => EnTypeId.SkillRandomPlayableAdapter,
                EnSkillType.Loop => EnTypeId.SkillLoopPlayableAdapter,
                _ => EnTypeId.None,
            };
            if(skillTypeId == EnTypeId.None)
                return;
            
            var newSkillCfg = ExcelEditorUtil.CreateTypeInstance<SkillCfg>();
            ExcelEditorUtil.SetCfgValue(newSkillCfg, nameof(newSkillCfg.nSkillID), skillId);
            ExcelEditorUtil.SetCfgValue(newSkillCfg, nameof(newSkillCfg.nTypeId), (int)skillTypeId);
            ExcelEditorUtil.SetCfgValue(newSkillCfg, nameof(newSkillCfg.arrParams), Array.Empty<int>());
            ExcelEditorUtil.AddCfg(newSkillCfg);
            
            var list = skillGroup.ToList();
            list.Add(skillId);
            skillGroup = list.ToArray();
            ExcelEditorUtil.SetCfgValue(monsterCfg, nameof(monsterCfg.arrSkillGroup), skillGroup);
        }, null);
    }

    private void SetSkillEditorContent(int monsterId, int skillId)
    {
        if (_IsDebugModel && (monsterId != _CurMonsterId || skillId != _CurSkillId))
            ExitDebugModel();

        _CurMonsterId = monsterId;
        _CurSkillId = skillId;
        if (monsterId <= 0 || skillId <= 0)
        {
            _CurSkillEditor = null;
            return;
        }

        if (_SkillEditorCache.TryGetValue(skillId, out _CurSkillEditor))
            return;

        var skillCfg = ExcelEditorUtil.GetCfg<SkillCfg>(skillId);
        var playableTypeId = skillCfg.nTypeId;
        var skillEditor = SkillEditorDefine.GetPlayableAdapterEditor((EnTypeId)playableTypeId);
        var userData = new SkillPlayableAdapterUserData()
        {
            startIndex = 0,
            count = skillCfg.arrParams.Length,
            arrParams = skillCfg.arrParams,
        };
        skillEditor.DeserializeFromIntArray(
            userData.arrParams
            , userData.startIndex
            , userData.count
            , userData);
        EditorUtil.ChangeEditorStruct(skillEditor);
        _CurSkillEditor = skillEditor;
        _SkillEditorCache.Add(skillId, skillEditor);
        if (_IsDebugModel)
            EnterDebugModel();
    }

    private void OnGuiCenterMonsterSkillSetting()
    {
        if (_IsDebugModel && _CurSkillEditor is ISkillDebugModelUpdateEditor updateEditor)
        {
            updateEditor.OnGuiDebugModelUpdateEditor();
        }
        else
        {
            _ContentScrollPos = EditorGUILayout.BeginScrollView(_ContentScrollPos);
            {
                _CurSkillEditor?.OnSkillEditorGUI();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void OnGuiCenterBottom()
    {
        if (!_DebugSerializeData)
            return;
        if (_CurSkillEditor == null)
            return;
        GUILayout.FlexibleSpace();
        GUILayout.Space(SkillEditorDefine.DefineSpace);
        GUILayout.Button("", GUILayout.Height(SkillEditorDefine.LineSize), GUILayout.ExpandWidth(true));
        GUILayout.Space(SkillEditorDefine.DefineSpace);
        var data = _CurSkillEditor.SerializeToIntArray();
        var jsonStr = JsonConvert.SerializeObject(data);
        GUILayout.TextArea(jsonStr, GUILayout.ExpandWidth(true));
    }

    private void EnterDebugModel()
    {
        if (_CurSkillEditor is not ISkillDebugModelEditor debugModelEditor)
            return;
        if (_IsDebugModel)
            return;
        _IsDebugModel = true;

        var monsterCfg = ExcelEditorUtil.GetCfg<MonsterCfg>(_CurMonsterId);
        var assetCfg = ExcelEditorUtil.GetCfg<AssetCfg>(monsterCfg.nAssetCfgID);
        var goAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetCfg.strPath);
        _DebugModelGameObject = GameObject.Instantiate(goAsset);
        debugModelEditor.OnDebugModelEnterEditor(_DebugModelGameObject);
    }
    private void ExitDebugModel()
    {
        if (_CurSkillEditor is not ISkillDebugModelEditor debugModelEditor)
            return;
        if (_IsDebugModel == false)
            return;
        _IsDebugModel = false;

        debugModelEditor.OnDebugModelExitEditor();

        GameObject.DestroyImmediate(_DebugModelGameObject);
        _DebugModelGameObject = null;
    }

    private void Save()
    {
        var now = DateTime.Now;
        var changeLog = new StringBuilder(1000);
        foreach (var (skillId, skillEditor) in _SkillEditorCache)
        {
            var data = skillEditor.SerializeToIntArray();
            var skillCfg = ExcelEditorUtil.GetCfg<SkillCfg>(skillId);
            if (ABBUtil.ArrayElementEquals(skillCfg.arrParams, data?.ToArray()))
                continue;
            changeLog.AppendLine("{");
            changeLog.AppendLine($"\t SkillId:   {skillId}");
            changeLog.AppendLine($"\t\t arrParams: {JsonConvert.SerializeObject(skillCfg.arrParams)}");
            changeLog.AppendLine($"\t\t ---------->: {JsonConvert.SerializeObject(data)}");
            changeLog.AppendLine("}");
            ExcelEditorUtil.SetCfgValue(skillCfg, nameof(SkillCfg.arrParams), data.ToArray());
        }

        if (changeLog.Length == 0)
        {
            Debug.Log("SkillEditor Save No Change");
            return;
        }

        var saveKey = ABBUtil.DateTimeToString(now);
        SaveHistory(saveKey, GetFromHistoryFileName());
        SaveHistoryLog(saveKey, changeLog.ToString());
        ExcelEditorUtil.SaveExcel<SkillCfg>();
        SaveHistory(saveKey, GetToHistoryFileName());
        ExcelEditorUtil.SaveExcel<MonsterCfg>();
    }
    private void SaveHistoryLog(string saveKey, string logStr)
    {
        var saveRoot = GetHistorySaveDirectory();
        var savePath = Path.Combine(saveRoot, saveKey);
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        var logPath = Path.Combine(savePath, $"changeLog.txt");
        File.WriteAllText(logPath, logStr, Encoding.UTF8);
        Debug.Log($"SkillEditor Save Change Log Path: {logPath}");
    }
    private void SaveHistory(string saveKey, string fileName)
    {
        var saveRoot = GetHistorySaveDirectory();
        var savePath = Path.Combine(saveRoot, saveKey);
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        var skillExcelPath = ExcelEditorUtil.GetExcelPath<SkillCfg>();
        File.Copy(skillExcelPath, Path.Combine(savePath, fileName), true);
    }
    private void RevertHistory(string saveKey, string fileName)
    {
        var saveRoot = GetHistorySaveDirectory();
        var savePath = Path.Combine(saveRoot, saveKey);
        var skillExcelPath = ExcelEditorUtil.GetExcelPath<SkillCfg>();
        File.Copy(Path.Combine(savePath, fileName), skillExcelPath, true);
    }
    private string GetFromHistoryFileName()
    {
        var skillExcelPath = ExcelEditorUtil.GetExcelPath<SkillCfg>();
        var fileName = $"cFrom_{Path.GetFileName(skillExcelPath)}";
        return fileName;
    }
    private string GetToHistoryFileName()
    {
        var skillExcelPath = ExcelEditorUtil.GetExcelPath<SkillCfg>();
        var fileName = $"cTo_{Path.GetFileName(skillExcelPath)}";
        return fileName;
    }
    private void ReloadSkillData()
    {
        foreach (var (skillId, skillEditor) in _SkillEditorCache)
        {
            var skillCfg = ExcelEditorUtil.GetCfg<SkillCfg>(skillId);
            var userData = new SkillPlayableAdapterUserData()
            {
                startIndex = 0,
                count = skillCfg.arrParams.Length,
                arrParams = skillCfg.arrParams,
            };
            skillEditor.DeserializeFromIntArray(
                userData.arrParams
                , userData.startIndex
                , userData.count
                , userData);
            EditorUtil.ChangeEditorStruct(skillEditor);
        }
    }
    public static void EditorInstanceInitialize(ISkillEditor skill)
    {
        if (_IsDebugModel && skill is ISkillDebugModelEditor debugModel)
        {
            debugModel.OnDebugModelEnterEditor(_DebugModelGameObject);
        }
    }
    public static void EditorInstanceDestroy(ISkillEditor skill)
    {
        if (_IsDebugModel && skill is ISkillDebugModelEditor debugModel)
        {
            debugModel.OnDebugModelExitEditor();
        }
    }
}