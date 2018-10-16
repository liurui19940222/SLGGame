using Framework.Common;
using Game.Component;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class ResourceLoader : IResourceLoader
    {
        public const string UI_PATH = "Prefabs/UI/";

        public const string MAP_PATH = "Prefabs/Map/";

        public const string CHAR_PATH = "Prefabs/Character/";

        public const string COMP_PATH = "Prefabs/Comp/";

        public const string CHAR_CFG_PATH = "Config/Character/";

        private Dictionary<string, Object> m_LoadedAssets;

        public ResourceLoader()
        {
            m_LoadedAssets = new Dictionary<string, Object>();
        }

        public override Object LoadAsset(string path, string name)
        {
            string key = path + name;
            if (m_LoadedAssets.ContainsKey(key))
            {
                return m_LoadedAssets[key];
            }
            Object obj = Resources.Load(key);
            m_LoadedAssets.Add(key, obj);
            return obj;
        }

        public override GameObject LoadAndInstantiateAsset(string path, string name)
        {
            Object obj = LoadAsset(path, name);
            return GameObject.Instantiate(obj) as GameObject;
        }

        public override Object LoadUIAsset(string assetName)
        {
            return LoadAsset(UI_PATH, assetName);
        }

        public MapRes LoadMapAsset(string assetName)
        {
            GameObject go = LoadAndInstantiateAsset(MAP_PATH, assetName);
            Transform trans = go.transform;
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            return go.GetComponent<MapRes>();
        }

        public T LoadToolComponent<T>(string name) where T : UnityEngine.Component
        {
            GameObject go = LoadAndInstantiateAsset(COMP_PATH, name);
            return go.GetComponent<T>();
        }
    }
}
