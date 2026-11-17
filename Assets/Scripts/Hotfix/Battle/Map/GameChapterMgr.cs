using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameChapterMgr : Singleton<GameChapterMgr>
{
    public bool IsActive(int chapterId)
    {
        var chapterData = PersistentMgr.Instance.GetData<PersistentChapterData>(EnPersistentId.Chapter);
        var maxSuccessChapterId = chapterData.GetMaxSuccessChapterId();
        return chapterId <= maxSuccessChapterId + 2;
    }
    public bool IsLock(int chapterId)
    {
        var chapterData = PersistentMgr.Instance.GetData<PersistentChapterData>(EnPersistentId.Chapter);
        var maxSuccessChapterId = chapterData.GetMaxSuccessChapterId();
        return chapterId > maxSuccessChapterId + 1;
    }
    public int GetMaxSuccessChapterId()
    {
        var chapterData = PersistentMgr.Instance.GetData<PersistentChapterData>(EnPersistentId.Chapter);
        var maxSuccessChapterId = chapterData.GetMaxSuccessChapterId();
        return maxSuccessChapterId;
    }
    public void SetMaxSuccessChapterId(int chapterId)
    {
        var chapterData = PersistentMgr.Instance.GetData<PersistentChapterData>(EnPersistentId.Chapter);
        chapterData.SetMaxSuccessChapterId(chapterId);
    }
    public int GetCurSelectChapterId()
    {
        var chapterData = PersistentMgr.Instance.GetData<PersistentChapterData>(EnPersistentId.Chapter);
        var chapterId = chapterData.GetCurSelectChapterId();

        var count = GameSchedule.Instance.GetChapterCfgCount();
        var min = GameSchedule.Instance.GetChapterCfg(0);
        var max = GameSchedule.Instance.GetChapterCfg(count - 1);
        var result = Mathf.Clamp(chapterId, min.nChapterId, max.nChapterId);
        return result;
    }
    public void SetCurSelectChapterId(int chapterId)
    {
        var chapterData = PersistentMgr.Instance.GetData<PersistentChapterData>(EnPersistentId.Chapter);
        chapterData.SetCurSelectChapter(chapterId);
    }
}
