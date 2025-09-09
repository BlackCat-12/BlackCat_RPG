using System.IO;
using UnityEngine;

public class ResourceService : MonoBehaviour
{
    private static ResourceService _instance;
    
    public static ResourceService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ResourceService>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ResourceService");
                    _instance = go.AddComponent<ResourceService>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    // 异步加载Sprite
    public void LoadSpriteAsync(string path, System.Action<Sprite> onComplete)
    {
        if (string.IsNullOrEmpty(path))
        {
            onComplete?.Invoke(null);
            return;
        }
        
        StartCoroutine(LoadSpriteCoroutine(path, onComplete));
    }
    
    private System.Collections.IEnumerator LoadSpriteCoroutine(string path, System.Action<Sprite> onComplete)
    {
        ResourceRequest request = Resources.LoadAsync<Sprite>(path);
        yield return request;
        
        onComplete?.Invoke(request.asset as Sprite);
    }
    
    // 同步加载Sprite（用于立即需要的情况）
    public Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        return Resources.Load<Sprite>(path);
    }
    
    // 加载JSON数据
    public string LoadJsonData(string filePath)
    {
        string json =  File.ReadAllText(filePath);
        return json;
    }
    
    // 预加载常用资源
    public void PreloadResources(string[] paths)
    {
        foreach (var path in paths)
        {
            Resources.LoadAsync<Sprite>(path);
        }
    }
}