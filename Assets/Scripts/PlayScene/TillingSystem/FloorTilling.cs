using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FloorTilling : MonoBehaviour
{
    public Material floorMaterial;

    [Tooltip("텍스처 1장이 표현하는 실제 크기 (미터)")]
    public float textureSize = 1f;

    void Start()
    {
        Apply();
    }

    public void Apply()
    {
        Renderer rend = GetComponent<Renderer>();

        if (floorMaterial != null)
            rend.sharedMaterial = floorMaterial;

        // 바닥(Quad)은 localScale = (가로, 세로, 1) 구조
        float width = transform.localScale.x;
        float depth = transform.localScale.y;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetVector("_BaseMap_ST", new Vector4(
            width / textureSize,
            depth / textureSize,
            0f, 0f
        ));
        rend.SetPropertyBlock(block);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
            Apply();
    }
#endif
}
