//----- Singleton.cs START -----

using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected bool IsSingletonInstance =>
        instance == this;

    protected virtual void Awake()
    {
        T currentInstance = (T)this;

        if (instance != null &&
            instance != currentInstance)
        {
            Debug.LogWarning(
                $"[SINGLETON] Duplicate {typeof(T).Name} found on '{gameObject.name}'. " +
                "Destroying the duplicate.");

            Destroy(gameObject);
            return;
        }

        instance = currentInstance;
        if (transform.parent != null)
        {
            Debug.LogWarning(
                $"[SINGLETON] {typeof(T).Name} is attached to  '{gameObject.name}', which has a parent." +
                $"DontDestroyOnLoad only works reliably on root objects.");


        }
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}

//----- Singleton.cs END -----