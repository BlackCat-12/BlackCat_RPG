using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;
    private Transform _playerTransform;
    
    public Transform PlayerTransform => _playerTransform;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
