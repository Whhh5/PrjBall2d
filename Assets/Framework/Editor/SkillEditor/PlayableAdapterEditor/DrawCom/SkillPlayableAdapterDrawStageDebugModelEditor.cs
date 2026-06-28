using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class SkillPlayableAdapterDrawStageDebugModelEditor : ISkillDebugModelEditor, ISkillDebugModelUpdateEditor
{
    private PlayableGraph _GraphEditor = default;
    private Playable _MainPlayable = default;
    private float _SliderValue = 0f;
    private readonly float4 _Padding = new(10, 10, 10, 10);
    private readonly float _SliderHeight = 20f;
    private static readonly List<ISkillEditor> _DrawSkillEditorList = new(10);
    private static readonly List<DrawSkillScheduleInfo> _DrawSkillEditorInfoList = new(10);
    private float _MaxTime = 0f;
    private readonly List<float> _StageTimeList = new();
    private float _LastGroupUpdateTime = 0f;
    private static readonly HashSet<KeyCode> _KeyCode = new(10);

    private long _LastTime = -1;
    private float _DeltaTime = -1;
    private SkillStageInfoEditor[] _DataList = null;
    public void SetDrawDataList(ISkillPlayableAdapterGetStageList drawEditor)
    {
        _DataList = drawEditor.GetSkillStageInfoEditors();
    }
    public void OnDebugModelExitEditor()
    {
        for (var i = 0; i < _DataList?.Length; i++)
            (_DataList[i] as ISkillDebugModelEditor).OnDebugModelExitEditor();

        if (_GraphEditor.IsValid())
            _GraphEditor.Destroy();
        _GraphEditor = default;
        _MainPlayable = default;
        _SliderValue
            = _MaxTime
            = _LastGroupUpdateTime
            = 0f;
        _DeltaTime
            = _LastTime
            = -1;
        _DrawSkillEditorList.Clear();
        _DrawSkillEditorInfoList.Clear();
        _KeyCode.Clear();
        _DataList = null;
    }
    public void OnDebugModelEnterEditor(GameObject go)
    {
        var anim = go.GetComponent<Animator>();
        _GraphEditor = PlayableGraph.Create($"custom-{anim.name}");
        _GraphEditor.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        var sourcePort = AnimationPlayableOutput.Create(_GraphEditor, $"{anim.name}-output", anim);
        var mixerPlayable = AnimationMixerPlayable.Create(_GraphEditor);
        sourcePort.SetSourcePlayable(mixerPlayable);
        _MainPlayable = mixerPlayable;
        var time = 0f;
        if (_DataList != null)
        {
            for (var i = 0; i < _DataList.Length; i++)
            {
                var stageInfo = _DataList[i];
                var clipId = stageInfo.GetClipID();
                var clipCfg = ExcelEditorUtil.GetCfg<ClipCfg>(clipId);
                _MaxTime += clipCfg.fLength;
                _StageTimeList.Add(clipCfg.fLength);

                var assCfg = ExcelEditorUtil.GetCfg<AssetCfg>(clipCfg.nAssetID);
                var clipAsset = AssetDatabase.LoadAssetAtPath<AnimationClip>(assCfg.strPath);
                var clipPlayable = AnimationClipPlayable.Create(_GraphEditor, clipAsset);
                mixerPlayable.AddInput(clipPlayable, 0);
                clipPlayable.SetTime(-time);
                time += _StageTimeList[i];
            }
        }

        for (int i = 0; i < _DataList?.Length; i++)
            (_DataList[i] as ISkillDebugModelEditor).OnDebugModelEnterEditor(go);

        _GraphEditor.Evaluate(0);
    }

    private Vector2 _ShowStageInfoScrollPos = Vector2.zero;
    public void OnGuiDebugModelUpdateEditor()
    {
        UpdateKeyCodeState();
        _DeltaTime = _LastTime < 0 ? 0 : (DateTime.Now.Ticks - _LastTime) / 10000f / 1000f;
        _LastTime = DateTime.Now.Ticks;
        UpdateKeyCodeControllerPlay(in _DeltaTime);

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Space(5);
            GUILayout.Button("", GUILayout.ExpandWidth(true), GUILayout.Height(SkillEditorDefine.LineSize));
            GUILayout.Space(5);
            DrawDebugModelControllerBar();
            GUILayout.Space(20);
            var sliderMainRect = GUILayoutUtility.GetRect(1000, 200, GUILayout.ExpandWidth(false));
            DrawSkillStagesArea(in sliderMainRect);

            DrawSliderArea(in sliderMainRect);

            GUILayout.Space(20);
            GUILayout.Button("", GUILayout.ExpandWidth(true), GUILayout.Height(SkillEditorDefine.LineSize));
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {
                for (int i = 0; i < _DrawSkillEditorList.Count; i++)
                {
                    _DrawSkillEditorList[i].OnSkillEditorGUI();
                    var info = _DrawSkillEditorInfoList[i];

                    if (Event.current.type != EventType.Layout)
                    {
                        info.rect = GUILayoutUtility.GetLastRect();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        UpdateSceneView();
    }
    private static void UpdateKeyCodeState()
    {
        if (!Event.current.isKey)
            return;
        if (Event.current.type == EventType.KeyDown)
        {
            _KeyCode.Add(Event.current.keyCode);
        }
        if (Event.current.type == EventType.KeyUp)
        {
            _KeyCode.Remove(Event.current.keyCode);
        }
    }
    private static bool IsKeyCode(KeyCode key)
    {
        return _KeyCode.Contains(key);
    }
    private void UpdateKeyCodeControllerPlay(in float deltaTime)
    {
        var incrementTime = (IsKeyCode(KeyCode.LeftShift) ? -1 : 1) * deltaTime;
        var increment = incrementTime / _MaxTime;

        if (EditorUtil.ControllerIsActive(EnEditorGlobalController.Play))
        {
            if (Mathf.Approximately(_SliderValue, 1))
                _SliderValue = 0;
            _SliderValue = Mathf.Clamp01(_SliderValue + increment);
        }
        else if (EditorUtil.ControllerIsActive(EnEditorGlobalController.PlayNext))
        {
            EditorUtil.SetControllerState(EnEditorGlobalController.PlayNext);
            _SliderValue = Mathf.Clamp01(_SliderValue + increment);
        }
    }
    private void UpdateSceneView()
    {
        if (!_GraphEditor.IsValid())
            return;
        var time = Mathf.Lerp(0, _MaxTime, _SliderValue);
        var deltaTime = time - _LastGroupUpdateTime;
        _LastGroupUpdateTime = time;
        _GraphEditor.Evaluate(deltaTime);

        var tempStageTime = 0f;
        var curStage = -1;
        for (var i = 0; i < _DataList?.Length; i++)
        {
            var stageTime = _StageTimeList[i];
            var localTime = Mathf.Clamp(time - tempStageTime, 0, stageTime);
            var localSlider = Mathf.Clamp01(localTime / stageTime);
            var timeInfo = new SkillDebugModelTimelineInfo()
            {
                skillTime = time,
                stageStartTime = tempStageTime,
                skillSlider = _SliderValue,
                skillMaxTime = _MaxTime,
                stageTime = localTime,
                stageMaxTime = stageTime,
                stageLocalSlider = localSlider,
            };
            _DataList[i].OnSceneDebugModelSliderValueChange(in timeInfo);
            tempStageTime += stageTime;
            if (localSlider > 0)
                curStage = i;
        }
        if (curStage >= 0)
        {
            for (var i = 0; i < _MainPlayable.GetInputCount(); i++)
            {
                _MainPlayable.SetInputWeight(i, i == curStage ? 1 : 0);
            }
        }

        SceneView.RepaintAll();
    }

    public static void SetDebugModelDrawSkillEditor(ISkillEditor skillEditor)
    {
        if (IsDebugModelDrawSkillEditor(skillEditor))
        {
            var index = _DrawSkillEditorList.IndexOf(skillEditor);
            _DrawSkillEditorList.RemoveAt(index);
            _DrawSkillEditorInfoList.RemoveAt(index);
        }
        else
        {
            _DrawSkillEditorList.Add(skillEditor);
            _DrawSkillEditorInfoList.Add(new());
        }
    }
    public static bool IsDebugModelDrawSkillEditor(ISkillEditor skillEditor)
    {
        return _DrawSkillEditorList.Contains(skillEditor);
    }
    public static DrawSkillScheduleInfo GetDrawSkillScheduleInfo(ISkillEditor skillEditor)
    {
        var index = _DrawSkillEditorList.IndexOf(skillEditor);
        var result = _DrawSkillEditorInfoList[index];
        return result;
    }

    private void DrawDebugModelControllerBar()
    {
        EditorGUILayout.BeginHorizontal();
        {
            for (var i = (int)EnEditorGlobalController.None;
                 i < Mathf.Abs((int)EnEditorGlobalController.EnumCount);
                 i++)
            {
                var controller = (EnEditorGlobalController)(1 << i);
                if (!Enum.IsDefined(typeof(EnEditorGlobalController), controller))
                    break;
                var style = SkillEditorUtil.GetButtonStyle(EditorUtil.ControllerIsActive(controller));
                var keyAtt = EditorUtil.GetEnumAttribute<KeyboardAttribute>(controller);
                if (GUILayout.Button(EditorUtil.GetEnumName(controller), style, GUILayout.Width(50))
                    || (Event.current.type == EventType.KeyDown && Event.current.keyCode == keyAtt.keyCode))
                {
                    EditorUtil.SetControllerState(controller);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSkillStagesArea(in Rect sliderMainRect)
    {
        var dataList = _DataList;
        if (dataList == null || dataList.Length == 0 || Event.current.type == EventType.Layout)
            return;

        var maxWidth = sliderMainRect.width - _Padding.x - _Padding.z;
        var curValue = 0f;
        for (var i = 0; i < dataList.Length; i++)
        {
            var stageInfo = dataList[i];
            var clipId = stageInfo.GetClipID();
            var clipCfg = ExcelEditorUtil.GetCfg<ClipCfg>(clipId);
            var length = clipCfg.fLength;
            var widthProgress = length / _MaxTime;
            var width = widthProgress * maxWidth;
            var stageRect = new Rect()
            {
                size = new Vector2(width, sliderMainRect.height - _SliderHeight - _Padding.y - _Padding.w),
                position = new Vector2(sliderMainRect.xMin + curValue + _Padding.x,
                    sliderMainRect.yMin + _Padding.y + _SliderHeight),
            };
            stageInfo.OnGuiDebugModelUpdateEditor(in stageRect, in i);
            curValue += width;
        }
    }

    private float _MouseOffsetX = 0;
    private void DrawSliderArea(in Rect sliderMainRect)
    {
        EditorGUI.DrawRect(sliderMainRect, new Color(0.8f, 0.8f, 0.8f, 0.1f));

        var sliderTrackRect = new Rect(sliderMainRect)
        {
            yMin = sliderMainRect.y + _Padding.y,
            xMin = sliderMainRect.x + _Padding.x,
            xMax = sliderMainRect.x + sliderMainRect.width - _Padding.z,
            height = 5,
        };

        var boxSize = new Vector2(50, _SliderHeight);
        var sliderTrackEventRect = new Rect(sliderTrackRect)
        {
            size = new Vector2(sliderTrackRect.width + boxSize.x, _SliderHeight),
            center = sliderTrackRect.center,
        };

        EditorGUI.DrawRect(sliderTrackRect, new Color(1, 1, 1, 0.1f));

        if (EditorUtil.HasMouseInRect(in sliderTrackEventRect) ||
            EditorUtil.MouseControllerIs(EnMouseControllerType.SliderBox))
        {
            var mousePos = EditorUtil.GetEditorMousePos(Vector2.zero);
            var lightBoxRect = new Rect()
            {
                size = boxSize * 1.2f,
                center = new Vector2(mousePos.x, sliderTrackRect.center.y),
            };
            EditorGUI.DrawRect(lightBoxRect, new Color(0, 1, 1, 0.1f));

            var lightSliderValue = Mathf.InverseLerp(sliderTrackRect.xMin, sliderTrackRect.xMax, lightBoxRect.center.x);
            EditorGUI.LabelField(lightBoxRect, $"{lightSliderValue * 100:0}%", SkillEditorUtil.LabelCenterStyle);


            if (Event.current.button == 0
                && Event.current.type == EventType.MouseDown
                && EditorUtil.MouseControllerIsNone())
            {
                var curSchedulePosX = Mathf.Lerp(sliderTrackRect.xMin, sliderTrackRect.xMax, _SliderValue);
                _MouseOffsetX = curSchedulePosX - mousePos.x;
                EditorUtil.AddMouseController(EnMouseControllerType.SliderBox);
            }
        }

        if (EditorUtil.MouseControllerIs(EnMouseControllerType.SliderBox) && Event.current.type != EventType.Layout)
        {
            var mousePos = EditorUtil.GetEditorMousePos(new Vector2(_MouseOffsetX, 0));
            var sliderValue = Mathf.InverseLerp(sliderTrackRect.xMin, sliderTrackRect.xMax, mousePos.x);
            _SliderValue = sliderValue;
        }

        if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
        {
            if (EditorUtil.MouseControllerIs(EnMouseControllerType.SliderBox))
            {
                EditorUtil.ClearMouseController(EnMouseControllerType.SliderBox);
            }
        }

        var localPosX = Mathf.Lerp(sliderTrackRect.xMin, sliderTrackRect.xMax, _SliderValue);
        //var localPosXOffset = Mathf.Lerp(boxSize.x, -boxSize.x, _SliderValue) * 0.5f;
        var sliderBoxRect = new Rect(sliderMainRect)
        {
            size = boxSize,
            center = new Vector2(localPosX/* + localPosXOffset*/, sliderTrackRect.center.y),
        };

        var sliderLineRect = new Rect(sliderBoxRect)
        {
            size = new Vector2(1, sliderMainRect.yMax - sliderBoxRect.yMax),
            center = new Vector2(localPosX, 0),
            y = sliderBoxRect.yMax,
        };

        EditorGUI.DrawRect(sliderLineRect, new Color(1, 0, 0, 0.8f));
        EditorGUI.DrawRect(sliderBoxRect, new Color(0, 0, 1, 1));
        EditorGUI.LabelField(sliderBoxRect, $"{_SliderValue * 100:0}%", SkillEditorUtil.LabelCenterStyle);
    }
}