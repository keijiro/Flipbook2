using UnityEngine;

namespace Flipbook {

readonly struct Page
{
    #region Allocation/deallocation

    public static Page
      Allocate(Mesh mesh, Material material, Vector2Int resolution, int layer)
    {
        var go = new GameObject("Page");
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        var rt = new RenderTexture(resolution.x, resolution.y, 0);

        go.hideFlags = HideFlags.HideInHierarchy;
        go.layer = layer;
        mf.sharedMesh = mesh;
        mr.sharedMaterial = material;

        _block.SetFloat("StartTime", 1e+3f);
        mr.SetPropertyBlock(_block);

        return new Page(go, mr, rt);
    }

    public static void Deallocate(Page page)
      => Object.Destroy(page._gameObject);

    #endregion

    #region Public method

    public Page StartFlipping(RenderTexture source, float speed)
    {
        _block.SetFloat("Speed", speed);
        _block.SetFloat("StartTime", Time.time);
        _block.SetTexture("ColorMap", _rt);
        _renderer.SetPropertyBlock(_block);
        Graphics.CopyTexture(source, _rt);
        return this;
    }

    #endregion

    #region Private members

    GameObject _gameObject { get; }
    MeshRenderer _renderer { get; }
    RenderTexture _rt { get; }

    static MaterialPropertyBlock _block = new MaterialPropertyBlock();

    Page(GameObject go, MeshRenderer mr, RenderTexture rt)
      => (_gameObject, _renderer, _rt) = (go, mr, rt);

    #endregion
}

} // namespace Flipbook
