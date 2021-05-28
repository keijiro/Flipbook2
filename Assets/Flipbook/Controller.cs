using UnityEngine;
using System.Collections.Generic;

namespace Flipbook {

sealed class Controller : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] Camera _sourceCamera = null;
    [SerializeField] Vector2Int _resolution = new Vector2Int(1280, 720);
    [SerializeField] int _pageCount = 15;
    [SerializeField, Range(0.02f, 0.2f)] float _interval = 0.1f;
    [SerializeField, Range(0.1f, 8.0f)] float _speed = 1;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] Mesh _mesh = null;
    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private variables

    Material _material;
    RenderTexture _rt;
    Queue<Page> _pages = new Queue<Page>();
    float _timer;

    #endregion

    #region MonoBehaviour implementation

    void OnValidate()
    {
        _resolution = Vector2Int.Max(_resolution, Vector2Int.one * 32);
        _resolution = Vector2Int.Min(_resolution, Vector2Int.one * 2048);
        _interval = Mathf.Max(_interval, 1.0f / 60);
    }

    void Start()
    {
        _material = new Material(_shader);

        _rt = new RenderTexture(_resolution.x, _resolution.y, 0);
        _sourceCamera.targetTexture = _rt;

        var layer = gameObject.layer;
        for (var i = 0; i < _pageCount; i++)
            _pages.Enqueue(Page.Allocate(_mesh, _material, _resolution, layer));
    }

    void OnDestroy()
    {
        while (_pages.Count > 0) Page.Deallocate(_pages.Dequeue());
        Destroy(_material);
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > _interval)
        {
            _pages.Enqueue(_pages.Dequeue().StartFlipping(_rt, _speed));
            _timer %= _interval;
        }
    }

    #endregion
}

} // namespace Flipbook
