using UnityEngine;

namespace Framework.Common
{
    public class IGameSystem
    {
        public virtual void OnInitialize(IResourceLoader loader, params object[] pars) { }

        public virtual void OnUninitialize() { }

        public virtual void OnUpdate() { }

        public virtual void OnInputMsg(Message.InputMessage msg) { }
    }

    public abstract class IResourceLoader
    {
        public abstract Object LoadAsset(string path, string name);

        public abstract GameObject LoadAndInstantiateAsset(string path, string name);

        public abstract Object LoadUIAsset(string assetName);
    }
}
