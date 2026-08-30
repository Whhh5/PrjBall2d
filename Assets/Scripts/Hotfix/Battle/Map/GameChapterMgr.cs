using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameChapterMgr : Singleton<GameChapterMgr>
{
    public bool IsActive(int chapterId)
    {
        return true;
    }
    public bool IsLock(int chapterId)
    {
        return false;
    }
}
