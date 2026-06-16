using UnityEngine;
using UnityEditor;

public class WallBuilder : Editor
{
    const float MAP_SCALE      = 1.0f;
    const float WALL_HEIGHT    = 5.0f;
    const float WALL_THICKNESS = 1.0f;

    static readonly float[,] WALLS = new float[,] {
        { 2f, 2f, 46f, 2f },
        { 52f, 2f, 98f, 2f },
        { 98f, 2f, 98f, 38f },
        { 74f, 38f, 98f, 38f },
        { 74f, 38f, 74f, 42f },
        { 60f, 42f, 74f, 42f },
        { 60f, 38f, 60f, 42f },
        { 28f, 38f, 60f, 38f },
        { 28f, 38f, 28f, 42f },
        { 14f, 42f, 28f, 42f },
        { 14f, 38f, 14f, 42f },
        { 2f, 38f, 14f, 38f },
        { 2f, 2f, 2f, 22f },
        { 2f, 26f, 2f, 38f },
        { 26f, 4f, 26f, 16f },
        { 26f, 20f, 26f, 38f },
        { 4f, 21f, 14f, 21f },
        { 16f, 21f, 26f, 21f },
        { 10f, 30f, 10f, 37f },
        { 16f, 30f, 16f, 37f },
        { 22f, 30f, 22f, 37f },
        { 28f, 24f, 31f, 24f },
        { 33f, 24f, 39f, 24f },
        { 41f, 24f, 47f, 24f },
        { 49f, 24f, 55f, 24f },
        { 57f, 24f, 60f, 24f },
        { 36f, 24f, 36f, 38f },
        { 44f, 24f, 44f, 38f },
        { 52f, 24f, 52f, 38f },
        { 60f, 24f, 60f, 30f },
        { 60f, 33f, 60f, 38f },
        { 28f, 22f, 34f, 22f },
        { 36f, 22f, 42f, 22f },
        { 42f, 6f, 42f, 22f },
        { 74f, 18f, 74f, 26f },
        { 74f, 30f, 74f, 38f },
        { 74f, 18f, 88f, 18f },
        { 88f, 6f, 88f, 20f },
        { 88f, 24f, 88f, 38f },
        { 89f, 14f, 97f, 14f },
        { 89f, 22f, 97f, 22f },
        { 89f, 30f, 97f, 30f },
        { 70f, 18f, 78f, 18f },
        { 80f, 18f, 88f, 18f },
        { 70f, 6f, 70f, 18f }
    };

    [MenuItem("Firefighter/벽 생성")]
    static void BuildWalls()
    {
        GameObject old = GameObject.Find("Walls");
        if (old != null) DestroyImmediate(old);

        GameObject root = new GameObject("Walls");
        int n = WALLS.GetLength(0);

        for (int i = 0; i < n; i++)
        {
            Vector3 a = new Vector3(WALLS[i, 0] * MAP_SCALE, 0f, WALLS[i, 1] * MAP_SCALE);
            Vector3 b = new Vector3(WALLS[i, 2] * MAP_SCALE, 0f, WALLS[i, 3] * MAP_SCALE);

            Vector3 dir = b - a;
            float len = dir.magnitude;
            if (len < 0.001f) continue;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"Wall_{i}";
            cube.transform.SetParent(root.transform);
            cube.transform.position = (a + b) * 0.5f + Vector3.up * (WALL_HEIGHT * 0.5f);
            cube.transform.rotation = Quaternion.LookRotation(dir / len, Vector3.up);
            cube.transform.localScale = new Vector3(WALL_THICKNESS, WALL_HEIGHT, len);
        }

        Debug.Log($"✅ 벽 {n}개 생성 완료");
    }
}
