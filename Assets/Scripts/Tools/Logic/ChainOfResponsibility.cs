using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tools.Logic
{

    public interface IHandler
    {
        IHandler SetNext(IHandler handler);
        void Handle(CORContext request);
    }

    public abstract class AbstractHandler : IHandler
    {
        private IHandler _nextHandler;
        public IHandler SetNext(IHandler handler) => this._nextHandler = handler;
        public virtual void Handle(CORContext request) => this._nextHandler?.Handle(request);
        
    }


    public class CORContext
    {
        private Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();


        public void AddData(string key, object value)
        {
            Data[key] = value;
        }

        public void AddSingletonData<T>(T value) where T : class
        {
            var key = typeof(T).FullName;

            if (!Data.ContainsKey(key))
            {
                Data[key] = value;
            }
            else
            {
                Debug.LogWarning($"Data with key '{key}' already exists. Singleton data cannot be overwritten.");
            }
        }

        public T GetData<T>(string key)
        {
            if (Data.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            else
            {
                Debug.LogError($"Data with key '{key}' not found or of incorrect type.");
                return default;
            }
        }

        public T GetSingletonData<T>() where T : class
        {

            var key = typeof(T).FullName;
            if (Data.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            else
            {
                Debug.LogError($"Singleton data of type '{typeof(T).FullName}' not found or of incorrect type.");
                return default;
            }
        }


    }

}
