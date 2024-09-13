using System.IO;
using UnityEngine;
namespace ComboSystem
{
    public class ResMgr
    {
        public static ResMgr Instance = null;
        public void Init()
        {
            ResMgr.Instance = this;
        }

        public T LoadAssetSync<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            path = Path.Combine("Assets/Resources/AssetsPackage", path);
            T ret = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
            return ret;

#else
            path = Path.Combine("AssetsPackage", path);
            T ret = Resources.Load<T>(path);
            return ret;
#endif
        }
    }
}
