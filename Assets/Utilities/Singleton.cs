using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
{
    public static T instance;

    protected virtual void Initialize () { }

    protected void Awake ()
    {
        if (FindObjectsOfType(typeof(T)).Length > 1)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = gameObject.GetComponent<T>();
        Initialize();
    }
}
