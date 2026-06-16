using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateMapPrefabs : Editor
{
    [MenuItem("Firefighter/맵 Prefab 생성")]
    static void CreatePrefabs()
    {
        string prefabFolder = "Assets/Prefabs";
        if (!Directory.Exists(prefabFolder))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        // ── 벽 Prefab ──────────────────────────────────────
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "WallPrefab";
        wall.transform.localScale = new Vector3(1f, 3f, 1f);

        Material wallMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        wallMat.color = new Color(0.55f, 0.50f, 0.45f);
        AssetDatabase.CreateAsset(wallMat, prefabFolder + "/WallMaterial.mat");
        wall.GetComponent<Renderer>().sharedMaterial = wallMat;

        PrefabUtility.SaveAsPrefabAsset(wall, prefabFolder + "/WallPrefab.prefab");
        DestroyImmediate(wall);

        // ── 바닥 Prefab ────────────────────────────────────
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.name = "FloorPrefab";
        floor.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        floor.transform.localScale = new Vector3(1f, 1f, 1f);

        Material floorMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floorMat.color = new Color(0.75f, 0.73f, 0.70f);
        AssetDatabase.CreateAsset(floorMat, prefabFolder + "/FloorMaterial.mat");
        floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        PrefabUtility.SaveAsPrefabAsset(floor, prefabFolder + "/FloorPrefab.prefab");
        DestroyImmediate(floor);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ WallPrefab, FloorPrefab 생성 완료! Assets/Prefabs 폴더 확인하세요.");
    }
}
