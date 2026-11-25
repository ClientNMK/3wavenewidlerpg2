using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    ///     <c>true</c> if this Singleton Awake() method has already been called by Unity; otherwise, <c>false</c>.
    /// </summary>
    public static bool IsAwakened { get; private set; }

    /// <summary>
    ///     <c>true</c> if this Singleton Start() method has already been called by Unity; otherwise, <c>false</c>.
    /// </summary>
    public static bool IsStarted { get; private set; }

    /// <summary>
    ///     <c>true</c> if this Singleton OnDestroy() method has already been called by Unity; otherwise, <c>false</c>.
    /// </summary>
    public static bool IsDestroyed { get; private set; }

    private static T _instance = null;
    private static object _syncobj = new object();
    private static bool appIsClosing = false;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if (IsDestroyed) return null;

                _instance = FindExistingInstance() ?? CreateNewInstance();
            }
            return _instance;
        }
    }
    private static T FindExistingInstance()
    {
        T[] existingInstances = FindObjectsOfType<T>();

        // No instance found
        if (existingInstances == null || existingInstances.Length == 0) return null;

        return existingInstances[0];
    }
    private static T CreateNewInstance()
    {
        var containerGO = new GameObject(typeof(T).Name);
        return containerGO.AddComponent<T>();
    }

    protected virtual void OnApplicationQuit()
    {
        appIsClosing = true;
    }

    protected virtual void NotifyInstanceRepeated()
    {
        Destroy(this.gameObject);
    }
    protected virtual void SingletonAwakened() { }
    protected virtual void SingletonStarted() { }
    protected virtual void SingletonDestroyed() { }

    protected virtual void Awake()
    {
        T thisInstance = this.GetComponent<T>();

        // Initialize the singleton if the script is already in the scene in a GameObject
        if (_instance == null)
        {
            _instance = thisInstance;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (thisInstance != _instance)
        {
            NotifyInstanceRepeated();
            return;
        }


        if (!IsAwakened)
        {
            SingletonAwakened();
            IsAwakened = true;
        }
    }
}