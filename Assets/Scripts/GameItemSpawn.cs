using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemSpawn : MonoBehaviour
{
    [SerializeField] private Transform _PlayerTransform;
    [SerializeField] private GameObject _gameItemPrefab;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        UnityEventCenter.Instance.AddListener<ItemStack>("DropGameItem", SpawnGameItem);
    }

    private void OnDisable()
    {
        UnityEventCenter.Instance.RemoveListener<ItemStack>("DropGameItem", SpawnGameItem);
    }

    public void SpawnGameItem(ItemStack state)
    {
        if (state == null) return;
        var Item = Instantiate(_gameItemPrefab);
        Item.transform.position = transform.position;
        if (!Item.TryGetComponent<GameItem>(out var gameItemScript)) return;
        gameItemScript.Init(state, _PlayerTransform);
        gameItemScript.Throw();
        Debug.Log("Spawning Game Item");
    }
}
