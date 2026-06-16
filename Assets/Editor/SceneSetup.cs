using UnityEngine;
using UnityEditor;

public class SceneSetup : EditorWindow
{
    [MenuItem("Firefighter/씬 자동 세팅")]
    static void SetupScene()
    {
        // ── 바닥 ──────────────────────────────────────
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.position = new Vector3(0, 0, 0);
        floor.transform.localScale = new Vector3(2, 1, 2);
        SetColor(floor, new Color(0.4f, 0.6f, 0.3f)); // 초록빛 바닥

        // ── 벽 4개 ────────────────────────────────────
        CreateWall("Wall_N", new Vector3(0,    1.5f,  10.5f), new Vector3(21, 3, 1));
        CreateWall("Wall_S", new Vector3(0,    1.5f, -10.5f), new Vector3(21, 3, 1));
        CreateWall("Wall_E", new Vector3(10.5f, 1.5f, 0),     new Vector3(1,  3, 21));
        CreateWall("Wall_W", new Vector3(-10.5f,1.5f, 0),     new Vector3(1,  3, 21));

        // ── 플레이어 ──────────────────────────────────
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 1, 0);
        player.tag = "Player";
        SetColor(player, new Color(0.2f, 0.5f, 1f)); // 파란색

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY
                       | RigidbodyConstraints.FreezeRotationZ;

        // FirePoint (물 발사 위치)
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(0, 0, 0.8f);

        // ── 카메라 ────────────────────────────────────
        Camera.main.transform.position = new Vector3(0, 10, -7);
        Camera.main.transform.rotation = Quaternion.Euler(50, 0, 0);

        // ── SpawnPoints (불 생성 위치) ─────────────────
        GameObject spawnRoot = new GameObject("SpawnPoints");
        Vector3[] spawnPos = {
            new Vector3(-4, 0.3f,  4),
            new Vector3( 4, 0.3f,  4),
            new Vector3( 0, 0.3f, -4)
        };
        for (int i = 0; i < spawnPos.Length; i++)
        {
            GameObject sp = new GameObject("SpawnPoint_" + (i + 1));
            sp.transform.SetParent(spawnRoot.transform);
            sp.transform.position = spawnPos[i];
        }

        // ── 매니저 오브젝트 ────────────────────────────
        new GameObject("FireManager");
        new GameObject("GameManager");

        Debug.Log("✅ 씬 세팅 완료! Player, Floor, Wall 4개, SpawnPoint 3개 생성됨.");
    }

    static void CreateWall(string name, Vector3 pos, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        SetColor(wall, new Color(0.6f, 0.4f, 0.2f)); // 갈색 벽
    }

    static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        renderer.material = mat;
    }
}
