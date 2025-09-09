using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventCenter : MonoBehaviour
{
    private static UnityEventCenter _instance;
    public static UnityEventCenter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UnityEventCenter>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("UnityEventCenter");
                    _instance = obj.AddComponent<UnityEventCenter>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }
    
    // 无参事件字典
    [SerializeField]
    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
    
    // 泛型单参数事件字典
    [SerializeField]
    private Dictionary<string, UnityEventBase> genericEventDictionary = new Dictionary<string, UnityEventBase>();

    #region 无参事件
    /// <summary>
    /// 注册无参事件
    /// </summary>
    public void AddListener(string eventName, UnityAction listener)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            UnityEvent newEvent = new UnityEvent();
            newEvent.AddListener(listener);
            eventDictionary.Add(eventName, newEvent);
        }
    }

    /// <summary>
    /// 移除无参事件监听
    /// </summary>
    public void RemoveListener(string eventName, UnityAction listener)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
            
            if (thisEvent.GetPersistentEventCount() == 0)
            {
                eventDictionary.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 触发无参事件
    /// </summary>
    public void TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.Invoke();
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found");
        }
    }
    #endregion

    #region 泛型单参数事件
    /// <summary>
    /// 注册泛型单参数事件
    /// </summary>
    public void AddListener<T>(string eventName, UnityAction<T> listener)
    {
        if (!genericEventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            var newEvent = new UnityEvent<T>();
            newEvent.AddListener(listener);
            genericEventDictionary.Add(eventName, newEvent);
        }
        else
        {
            if (existingEvent is UnityEvent<T> typedEvent)
            {
                typedEvent.AddListener(listener);
            }
            else
            {
                Debug.LogError($"Event {eventName} type mismatch");
            }
        }
    }

    /// <summary>
    /// 移除泛型单参数事件监听
    /// </summary>
    public void RemoveListener<T>(string eventName, UnityAction<T> listener)
    {
        if (genericEventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            if (existingEvent is UnityEvent<T> typedEvent)
            {
                typedEvent.RemoveListener(listener);
                
                if (typedEvent.GetPersistentEventCount() == 0)
                {
                    genericEventDictionary.Remove(eventName);
                }
            }
        }
    }

    /// <summary>
    /// 触发泛型单参数事件
    /// </summary>
    public void TriggerEvent<T>(string eventName, T eventParam)
    {
        if (genericEventDictionary.TryGetValue(eventName, out UnityEventBase thisEvent))
        {
            if (thisEvent is UnityEvent<T> castEvent)
            {
                castEvent.Invoke(eventParam);
                Debug.Log($"Event {eventName} triggered");
            }
            else
            {
                Debug.LogError($"Event {eventName} type mismatch");
            }
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found");
        }
    }
    #endregion

    /// <summary>
    /// 清除所有事件监听
    /// </summary>
    public void Clear()
    {
        foreach (var unityEvent in eventDictionary.Values)
        {
            unityEvent.RemoveAllListeners();
        }
        eventDictionary.Clear();

        foreach (var unityEvent in genericEventDictionary.Values)
        {
            // UnityEventBase 没有直接的 RemoveAllListeners 方法
            // 需要特殊处理不同类型的 UnityEvent
            if (unityEvent is UnityEvent simpleEvent)
            {
                simpleEvent.RemoveAllListeners();
            }
            // 可以添加其他类型的处理
        }
        genericEventDictionary.Clear();
    }

    private void OnDestroy()
    {
        Clear();
        if (_instance == this)
        {
            _instance = null;
        }
    }
}