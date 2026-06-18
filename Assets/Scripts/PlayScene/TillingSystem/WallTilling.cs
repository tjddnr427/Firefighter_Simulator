using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WallTilling : MonoBehaviour
{
    public Material wallMaterial;

    [Tooltip("텍스처 1장이 표현하는 실제 크기 (미터)")]
    public float textureSize = 1f;

    void Start()
    {
        Apply();
    }

    public void Apply()
    {
        Renderer rend = GetComponent<Renderer>();

        if (wallMaterial != null)
            rend.sharedMaterial = wallMaterial;

        // 벽은 localScale = (두께, 높이, 길이) 구조
        float width  = transform.localScale.z; // 벽 길이 (가로 타일링)
        float height = transform.localScale.y; // 벽 높이 (세로 타일링)

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetVector("_BaseMap_ST", new Vector4(
            width  / textureSize,
            height / textureSize,
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
