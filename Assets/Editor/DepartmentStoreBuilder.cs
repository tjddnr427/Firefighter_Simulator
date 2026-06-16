using UnityEngine;
using UnityEditor;

public class DepartmentStoreBuilder : Editor
{
    static readonly Color32 VOID = new Color32(255, 255, 255, 255);
    static readonly Color32 WALL = new Color32(30,  30,  30,  255);

    const float TILE          = 1f;
    const float WALL_H        = 3f;
    const int   COLOR_TOLERANCE = 30;

    [MenuItem("Firefighter/맵 생성")]
    static void BuildMap()
    {
        // 텍스처 임포트 설정 강제 적용 (Read/Write, NoCompression, Point)
        string path = "Assets/Maps/dept_floorplan_sectors.png";
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.isReadable        = true;
            importer.filterMode        = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.sRGBTexture       = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        Texture2D map = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (map == null)
        {
            Debug.LogError("Assets/Maps/dept_floorplan_sectors.png 를 찾을 수 없습니다.");
            return;
        }

        // 기존 맵 삭제
        GameObject old = GameObject.Find("GeneratedLevel");
        if (old != null) DestroyImmediate(old);

        GameObject root = new GameObject("GeneratedLevel");

        int w = map.width, h = map.height;
        Color32[] pixels = map.GetPixels32();

        int wallCount = 0, floorCount = 0, voidCount = 0;

        // 샘플 픽셀 로그
        Debug.Log($"맵 크기: {w}x{h} / 픽셀[0,0]:{pixels[0]} / [2,4]:{pixels[4*w+2]} / [50,22]:{pixels[22*w+50]}");

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Color32 c = pixels[y * w + x];

                // 흰색(바깥) 스킵: R,G,B 모두 200 이상
                if (c.r > 200 && c.g > 200 && c.b > 200) { voidCount++; continue; }

                Vector3 pos = new Vector3(x * TILE, 0f, (h - 1 - y) * TILE);

                // 어두운 픽셀(벽): R,G,B 모두 80 이하
                if (c.r < 80 && c.g < 80 && c.b < 80)
                {
                    SpawnWall(pos, root.transform);
                    wallCount++;
                }
                else
                {
                    SpawnFloor(pos, root.transform);
                    floorCount++;
                }
            }
        }

        Debug.Log($"✅ 맵 생성 완료: 벽={wallCount} / 바닥={floorCount} / 빈칸={voidCount}");
    }

    static void SpawnWall(Vector3 pos, Transform parent)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall";
        wall.transform.SetParent(parent);
        wall.transform.position = pos + Vector3.up * (WALL_H * 0.5f);
        wall.transform.localScale = new Vector3(TILE, WALL_H, TILE);
    }

    static void SpawnFloor(Vector3 pos, Transform parent)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.name = "Floor";
        floor.transform.SetParent(parent);
        floor.transform.position = pos;
        floor.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        floor.transform.localScale = new Vector3(TILE, TILE, TILE);
    }

    static bool Match(Color32 a, Color32 b) =>
        Mathf.Abs(a.r - b.r) <= COLOR_TOLERANCE &&
        Mathf.Abs(a.g - b.g) <= COLOR_TOLERANCE &&
        Mathf.Abs(a.b - b.b) <= COLOR_TOLERANCE;
}
