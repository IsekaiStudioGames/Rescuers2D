//----- Singleton.cs START -----

using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    public static T Instance { get; private set; }

    protected bool IsSingletonInstance =>
        Instance == this;

    protected virtual void Awake()
    {
        T currentInstance = (T)this;

        if (Instance != null &&
            Instance != currentInstance)
        {
            Debug.LogWarning(
                $"[SINGLETON] Duplicate {typeof(T).Name} found on '{gameObject.name}'. " +
                "Destroying the duplicate.");

            Destroy(gameObject);
            return;
        }
        
        if (transform.parent != null)
        {
            Debug.LogWarning(
                $"[SINGLETON] {typeof(T).Name} is attached to  '{gameObject.name}', which has a parent." +
                $"DontDestroyOnLoad only works reliably on root objects.");


        }
        Instance = currentInstance;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

//----- Singleton.cs END -----