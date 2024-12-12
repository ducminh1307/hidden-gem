using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    [SerializeField] private bool dontDestroyOnLoad = false;
    private static T _instance;

    public static T Instance => _instance ? _instance : null;

    protected virtual void Awake()
    {
        if ( _instance == null)
        {
            _instance = FindFirstObjectByType<T>();
            if (_instance == null)
            {
                var singleton = new GameObject(typeof(T).Name);
                _instance = singleton.AddComponent<T>();
            }

            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}