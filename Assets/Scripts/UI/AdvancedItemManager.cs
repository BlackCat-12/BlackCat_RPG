using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.Linq;

public class AdvancedItemManager : MonoBehaviour
{
    private static AdvancedItemManager _instance;
    private Dictionary<int, ItemDefinitionSO> _loadedItems = new Dictionary<int, ItemDefinitionSO>();
    private CancellationTokenSource _cts;

    public static AdvancedItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AdvancedItemManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ItemManager");
                    _instance = go.AddComponent<AdvancedItemManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        ReleaseAllItems();
    }

    // 异步加载单个物品 (UniTask版本)
    public async UniTask<ItemDefinitionSO> LoadItemDefinitionAsync(int itemId, CancellationToken token = default)
    {
        // 检查取消请求
        token.ThrowIfCancellationRequested();

        // 内存缓存检查
        if (_loadedItems.TryGetValue(itemId, out var cachedItem))
        {
            return cachedItem;
        }

        string address = $"Item_{itemId:000}";
        AsyncOperationHandle<ItemDefinitionSO> operation = Addressables.LoadAssetAsync<ItemDefinitionSO>(address);

        try
        {
            // 使用UniTask的扩展方法等待Addressables
            await operation.WithCancellation(token);

            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedItems[itemId] = operation.Result;
                return operation.Result;
            }
            else
            {
                Debug.LogError($"加载失败 ItemID: {itemId}, Address: {address}");
                return null;
            }
        }
        catch (OperationCanceledException)
        {
            Addressables.Release(operation);
            throw;
        }
    }

    // 批量加载 (UniTask并行优化)
    public async UniTask<List<ItemDefinitionSO>> LoadMultipleItemsAsync(IEnumerable<int> itemIds, CancellationToken token = default)
    {
        var tasks = new List<UniTask<ItemDefinitionSO>>();
        foreach (int id in itemIds)
        {
            tasks.Add(LoadItemDefinitionAsync(id, token));
        }

        var results = await UniTask.WhenAll(tasks);
        return new List<ItemDefinitionSO>(results);
    }

    // 预加载常用物品 (带进度回调)
    public async UniTask PreloadCommonItems(
        IEnumerable<int> itemIds,
        IProgress<float> progress = null,
        CancellationToken token = default)
    {
        int total = 0;
        int completed = 0;
        var ids = itemIds as ICollection<int> ?? itemIds.ToList();
        total = ids.Count;

        await foreach (var id in ids.ToUniTaskAsyncEnumerable().WithCancellation(token))
        {
            await LoadItemDefinitionAsync(id, token);
            completed++;
            progress?.Report((float)completed / total);
        }
    }

    // 释放单个资源
    public void ReleaseItem(int itemId)
    {
        if (_loadedItems.TryGetValue(itemId, out var item))
        {
            Addressables.Release(item);
            _loadedItems.Remove(itemId);
        }
    }

    // 释放所有资源
    public void ReleaseAllItems()
    {
        foreach (var kvp in _loadedItems)
        {
            Addressables.Release(kvp.Value);
        }
        _loadedItems.Clear();
    }
}