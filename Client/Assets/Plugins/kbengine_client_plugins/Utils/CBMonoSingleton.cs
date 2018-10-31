/// <summary>
/// Generic Mono singleton.
/// </summary>
using UnityEngine;

namespace CBFrame.Utils
{
    public abstract class CBMonoSingleton<T> : MonoBehaviour where T : CBMonoSingleton<T>
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    if (_instance == null)
                    {
                        _instance = 
                            new GameObject("Singleton of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        _instance.OnInit();
                    }

                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        private void Awake()
        {

            if (_instance == null)
            {
                _instance = this as T;
            }
        }

        protected virtual void OnInit() { }

        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}