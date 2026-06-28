using UnityEngine;

public class SingletonMono<T> : MonoBehaviour
    where T: class
{
    public static T Instance = null;
    protected virtual void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
        Instance = this as T;
    }
    protected virtual void Start()
    {

    }
    protected virtual void OnDestroy()
    {

    }
    protected virtual void Update()
    {
        
    }
}

