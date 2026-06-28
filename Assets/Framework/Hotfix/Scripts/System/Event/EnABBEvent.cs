public enum EnABBEvent
{
    NONE,
    EVENTDEFAULT,
    ENTITY_ONTRIGGER_ENTER,
    ENTITY_ONTRIGGER_EXIT,

    EVENT_SELECT_AREA_START,
    EVENT_SELECT_AREA_END,

    EVENT_BATTLE_INFO, 


    EVENT_GAME_START, // 游戏开始
    EVENT_GAME_OVER, // 游戏结束


    EVENT_RECREATE_SCENE, // 重新创建场景
    EVENT_CREATE_SCENE, // 创建场景


    EVENT_PLAYER_INGROUND, // 玩家触碰到地面
    EVENT_RESURGENCE, // 玩家复活 


    EVENT_MOUSE_START, // 鼠标有效按下
    EVENT_MOUSE_END, // 鼠标有效抬起
    EVENT_MOUSE_DRAW, // 鼠标有效持续滑动


    EVENT_RETURN_LOBBY, // 返回大厅界面


    OnTimelineEnd,

    EVENT_SKILL_TIMELINE_PHYSICS, // 技能时间线物理事件

    /// <see cref="EventSkillAttackData"/>
    EVENT_SKILL_ATTACK, // 技能攻击事件


    // ui
    EVENT_LEVELWINDOW_LEVEL,
}

