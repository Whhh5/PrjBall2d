using UnityEngine;


// 用于在主线程执行操作的帮助类
public static class UnityMainThreadDispatcher
{
    private static System.Collections.Generic.Queue<System.Action> executionQueue = new System.Collections.Generic.Queue<System.Action>();

    public static void Enqueue(System.Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    public static void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }
}
