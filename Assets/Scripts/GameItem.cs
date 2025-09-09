using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer  _spriteRenderer;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rb;

    [Header("运动参数")]
    [SerializeField] private float _floatHeight;
    [SerializeField] private float _floatSpeed;
    
    [Header("丢弃相关")]
    [SerializeField] private float _hoverHeight = 0.5f; // 悬停高度
    [SerializeField] private LayerMask _groundLayer; // 在Inspector中设置地面层级
    [SerializeField]private float _minForce = 2;
    [SerializeField]private float _maxForce = 3;
    private float _afterTime = 1;
    private float _ForceY = 2;
    private bool _floatAnim = false;
    
    
    private ItemStack _itemStack;
    private Transform _playerTransform;

    private Vector3 _originalPosition;
    private float _timeCounter;
    
    public ItemStack ItemStack => _itemStack;
    
    private void Start()
    {
        if (_spriteRenderer == null) return;
        StartCoroutine(EnableCollider(_afterTime));
        SetUpSprite();
        SetUpName();
    }

    private void Update()
    {
        if (_floatAnim)
        {
            FloatAnimation();
        }
    }

    public void Init(ItemStack itemStack, Transform playerTransform)
    {
        _itemStack = itemStack;
        _playerTransform = playerTransform;
    }

    #region Update
    private void SetUpSprite()
    {
        _spriteRenderer.sprite = _itemStack.ItemDefinition.icon;
    }

    private void SetUpName()
    {
        string name = _itemStack.ItemDefinition.name;
        string number = _itemStack.NumberOfItems.ToString();
        
        gameObject.name = $"{name} ({number})";
    }

    private void FloatAnimation()
    {
        _timeCounter += Time.deltaTime * _floatSpeed;
        float newY = _originalPosition.y + Mathf.Sin(_timeCounter) * _floatHeight;  // 直接移动pos进行正弦运动
        
        transform.position = new Vector3(
            _originalPosition.x,
            newY,
            _originalPosition.z
        );
        
        // 缓慢旋转效果
        Vector3 direction = GameManager.Instance.PlayerTransform.position - transform.position;
        direction.y = 0; // 忽略 Y 轴高度差，只计算水平方向
        if (direction != Vector3.zero) // 避免零向量错误
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // 仅保留 Y 轴旋转
        }
    }
    

    #endregion
  
    public ItemStack Pick()
    {
        Destroy(gameObject);
        return _itemStack;
    }
    private IEnumerator EnableCollider(float afterTime)
    {
        yield return new WaitForSeconds(afterTime);
        _collider.enabled = true;
    }
    
    public void Throw()
    {
        _rb.useGravity = true; // 启用重力
        float ForceForward = Random.Range(_minForce, _maxForce);
        _rb.velocity = transform.forward * ForceForward + new Vector3(0, _ForceY, 0);  // 添加抛掷速度
        StartCoroutine(DisableGravityAtHeight(_hoverHeight));
        _originalPosition  = transform.position;
        _floatAnim = true;
    }
    
    private IEnumerator DisableGravityAtHeight(float targetHeight)
    {
        while (true)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                float distanceToGround = hit.distance; // 当前离地距离
                if (distanceToGround <= targetHeight)
                {
                    _rb.useGravity = false;
                    _rb.velocity = Vector3.zero; // 停止运动
                    yield break; // 结束协程
                }
            }
            yield return null; // 每帧检测
        }
    }
}
