using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AiEditorWindow : EditorWindow
{
    private AiDataEditor _CurAiEditor = null;

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        EditorUtil.ClearData();
    }

    private void OnEditorUpdate()
    {
        Repaint();
    }

    public void OnGUI()
    {
        EditorUtil.EditorUpdate(Event.current);

        var mainRect = new Rect(0, 0, position.width, position.height);
        var leftMenuSize = new Vector2(200, mainRect.height);
        var leftRect = new Rect(mainRect.position + _CurLeftMenuListScrollPos, leftMenuSize);

        var leftSize = leftMenuSize + _CurLeftMenuListScrollPos;

        var nodeAreaRect = new Rect(leftRect.xMax, mainRect.yMin, mainRect.width - leftSize.x, mainRect.height);
        GUI.BeginClip(nodeAreaRect);
        _CurAiEditor?.OnGUI(new Rect(0, 0, nodeAreaRect.width, nodeAreaRect.height));
        GUI.EndClip();

        GUI.BeginClip(leftRect);
        DrawLeftMenuList(new Rect(0, 0, leftRect.width, leftRect.height));
        GUI.EndClip();

        UpdateFps(position);
    }

    #region data

    private readonly Dictionary<int, AiDataEditor> _AiId2Editor = new();

    public void SetAiViewId(in int aiId)
    {
        if (_AiId2Editor.TryGetValue(aiId, out var editor))
        {
            _CurAiEditor = editor;
        }
        else
        {
            var aiData = new AiDataEditor();
            aiData.Initialization(aiId);
            _AiId2Editor.Add(aiId, aiData);
            _CurAiEditor = aiData;
        }
    }

    public void SetDecisionData(int[] aiDataArrDatas)
    {
        var aiData = new AiDataEditor();
        var aiComUserData = new AiCommonUserData()
        {
            arrParams = aiDataArrDatas,
            startIndex = 0,
            count = aiDataArrDatas.Length,
        };
        aiData.DeserializeFromIntArray(aiComUserData.arrParams, aiComUserData.startIndex, aiComUserData.count,
            aiComUserData);
        _CurAiEditor = aiData;
        var aiId = aiData.GetAiId();
        _AiId2Editor[aiId] = aiData;
    }

    private void Save2Excel()
    {
        foreach (var (aiId, editor) in _AiId2Editor)
        {
            editor.AppendData();
        }
        ExcelEditorUtil.SaveExcel<AICfg>();
    }

    #endregion

    #region left Menu list

    private bool _IsShow = true;
    private Vector2 _LeftMenuListScrollPos;
    private Vector2 _CurLeftMenuListScrollPos;
    
    private Vector2 _CurRightMenuSize;

    private void DrawLeftMenuList(in Rect rect)
    {
        var rightRect = new Rect(0, 0, 20, rect.height);
        rightRect.position = new Vector2()
        {
            x = rect.xMax - rightRect.width,
            y = rect.yMin,
        };
        var leftRect = new Rect(rect)
        {
            width = rect.width - rightRect.width,
        };
        EditorGUI.DrawRect(leftRect, new Color(0.1f, 0.1f, 0.1f, 1f));

        var topRect = new Rect(rect.position, new Vector2(rect.width - rightRect.width, rect.height * 0.2f));
        var bottomRect = new Rect(new Vector2(rect.xMin, topRect.yMax + 10),
            new Vector2(topRect.width, rect.height - topRect.height));

        GUI.BeginClip(topRect);
        DrawLeftMenuTop(new Rect(0, 0, topRect.width, topRect.height));
        GUI.EndClip();

        GUI.BeginClip(bottomRect);
        DrawLeftMenuBottom(new Rect(0, 0, bottomRect.width, bottomRect.height));
        GUI.EndClip();


        var rightMenuSize = EditorUtil.HasMouseInRect(rightRect) ? new Vector2() : new Vector2(-rightRect.width, 0);
        _CurRightMenuSize = Vector2.Lerp(_CurRightMenuSize, rightMenuSize, 0.03f);

        if (GUI.Button(new Rect(rightRect.position, rightRect.size + _CurRightMenuSize), ""))
        {
            _IsShow = !_IsShow;
            _LeftMenuListScrollPos = _IsShow ? Vector2.zero : -new Vector2(rect.width - rightRect.width, 0);
        }

        _CurLeftMenuListScrollPos = Vector2.Lerp(_CurLeftMenuListScrollPos, _LeftMenuListScrollPos, 0.03f);
    }

    private void DrawLeftMenuTop(in Rect rect)
    {
        var btnSize = new Vector2(rect.width - 10, 20);
        var btnRect = new Rect(rect.position + new Vector2((rect.width - btnSize.x) / 2, 10), btnSize);
        DrawDo(btnRect);

        btnRect.position += new Vector2(0, btnRect.height + 5);
        if (GUI.Button(btnRect, "Cache"))
        {
            SetDecisionDirty();
        }

        btnRect.position += new Vector2(0, btnRect.height + 5);
        if (GUI.Button(btnRect, "Save Excel"))
        {
            Save2Excel();
        }
    }

    private void DrawLeftMenuBottom(in Rect rect)
    {
        var aiCount = ExcelEditorUtil.GetCfgCount<AICfg>();
        var btnSize = new Vector2(rect.width - 10, 20);
        var offsetPos = rect.position + new Vector2((rect.width - btnSize.x) / 2, 10);
        for (var i = 0; i < aiCount; i++)
        {
            var aiCfg = ExcelEditorUtil.GetCfgByIndex<AICfg>(i);
            var btnRect = new Rect(offsetPos + new Vector2(0, i * (btnSize.y + 2)), btnSize);
            if (GUI.Button(btnRect, $"{aiCfg.nAIID}"))
            {
                SetAiViewId(aiCfg.nAIID);
            }
        }
    }

    #endregion

    #region do

    public void DrawDo(in Rect rect)
    {
        var undoRect = new Rect(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.width / 2 - 1, rect.height));
        var redoRect = new Rect(undoRect)
        {
            x = undoRect.xMax + 2,
        };

        var enable = GUI.enabled;
        GUI.enabled &= _CurHistoryIndex > 0;
        if (GUI.Button(undoRect, "Undo"))
        {
            ExecuteUndo();
        }
        GUI.enabled = enable;

        GUI.enabled &= _CurHistoryIndex < _HistoryLineList.Count;
        if (GUI.Button(redoRect, "Redo"))
        {
            ExecuteRedo();
        }
        GUI.enabled = enable;
    }

    #endregion

    #region history

    private List<List<int>> _HistoryLineList = new();
    private int _CurHistoryIndex = 0;

    public void SetDecisionDirty()
    {
        if (_CurAiEditor == null)
            return;
        var data = _CurAiEditor.SerializeToIntArray();

        if (_CurHistoryIndex == _HistoryLineList.Count)
            _HistoryLineList.Add(null);

        _HistoryLineList[_CurHistoryIndex++] = data;
        if (_CurHistoryIndex < _HistoryLineList.Count)
        {
            var newList = new List<int>[_CurHistoryIndex];
            _HistoryLineList.CopyTo(0, newList, 0, _CurHistoryIndex);
            _HistoryLineList = new List<List<int>>(newList);
        }
    }

    public void ExecuteUndo()
    {
        if (_CurHistoryIndex < 0)
            return;
        var data = _HistoryLineList[--_CurHistoryIndex];
        SetDecisionData(data.ToArray());
    }

    public void ExecuteRedo()
    {
        if (_CurHistoryIndex == _HistoryLineList.Count)
            return;
        var data = _HistoryLineList[_CurHistoryIndex++];
        SetDecisionData(data.ToArray());
    }

    #endregion
    
    #region fps
    private int _Fps = 0;
    private int _LastFps = 0;
    private float _LastFpsTime = 0;
    private void UpdateFps(in Rect parentRect)
    {
        _Fps++;
        var lastTime = Time.realtimeSinceStartup;
        if (_LastFpsTime + 1 < lastTime)
        {
            _LastFpsTime = lastTime;
            _LastFps = _Fps;
            _Fps = 0;
        }

        var fpsRect = new Rect(parentRect.width - 110, 10, 100, 15);
        var fpsStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperRight,
        };
        GUI.Label(fpsRect, $"{_LastFps}", fpsStyle);
    }
    #endregion
}