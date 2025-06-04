using UnityEngine;

public class MuseumRoomBuilder : MonoBehaviour
{
    [Header("Environment Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject windowPrefab;
    public GameObject ceilingPrefab;

    [Header("Exhibit Prefabs")]
    public GameObject[] statuePrefabs; // index 1 uses a table
    public GameObject tablePrefab;

    [Header("Fire Area Prefabs")]
    public GameObject firePrefab;
    public GameObject chairPrefab;

    [Header("Room Settings")]
    public Vector3 roomSize = new Vector3(40, 8, 40); // affects ceiling/wall size

    public Vector3 FloorCenter => Vector3.zero;
    public float FloorY => 0.1f;

    void Start()
    {
        BuildRoom();
        PlaceFireAndChairs();
        PlaceStatues();
    }

    void BuildRoom()
    {
        // === Floor ===
        GameObject floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
        Vector3 floorScale = new Vector3(20, 1, 20);
        floor.transform.localScale = floorScale;
        floor.name = "Floor";

        if (floor.GetComponent<Collider>() == null)
            floor.AddComponent<MeshCollider>();

        float floorWidth = floorScale.x * 10f; // Unity plane unit = 10
        float floorLength = floorScale.z * 10f;
        float wallHeight = roomSize.y;

        // === Ceiling ===
        float ceilingHeight = wallHeight + 10f;
        GameObject ceiling = Instantiate(ceilingPrefab, new Vector3(0, ceilingHeight, 0), Quaternion.Euler(180, 0, 0));
        ceiling.transform.localScale = new Vector3(floorScale.x, 1, floorScale.z);
        ceiling.name = "Ceiling";

        // === Walls ===
        Vector3[] wallPositions = {
            new Vector3(0, wallHeight / 2f, -floorLength / 2f), // back
            new Vector3(0, wallHeight / 2f, floorLength / 2f),  // front
            new Vector3(-floorWidth / 2f, wallHeight / 2f, 0),  // left
            new Vector3(floorWidth / 2f, wallHeight / 2f, 0)    // right
        };

        Vector3[] wallRotations = {
            Vector3.zero,
            new Vector3(0, 180, 0),
            new Vector3(0, 90, 0),
            new Vector3(0, -90, 0)
        };

        Vector3[] wallScales = {
            new Vector3(floorScale.x, wallHeight / 5f, 1),  // back
            new Vector3(floorScale.x, wallHeight / 5f, 1),  // front
            new Vector3(floorScale.z, wallHeight / 5f, 1),  // left
            new Vector3(floorScale.z, wallHeight / 5f, 1)   // right
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(wallPrefab, wallPositions[i], Quaternion.Euler(wallRotations[i]));
            wall.transform.localScale = wallScales[i];
            wall.name = $"Wall_{i + 1}";

            GameObject window = Instantiate(windowPrefab, wallPositions[i] + new Vector3(0, 1.5f, 0), Quaternion.Euler(wallRotations[i]));
            window.name = $"Window_{i + 1}";
        }
    }


    void PlaceFireAndChairs()
    {
        Vector3 center = Vector3.zero;

        // Fire pit in the center
        GameObject fire = Instantiate(firePrefab, center + Vector3.up * 0.1f, Quaternion.identity);
        fire.transform.localScale *= 1.5f;
        fire.name = "FirePit";

        // Four chairs around fire
        float chairRadius = 6f;
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * chairRadius;
            Quaternion rot = Quaternion.LookRotation(center - pos);
            GameObject chair = Instantiate(chairPrefab, pos + Vector3.up * 0.05f, rot);
            chair.name = $"Chair_{i + 1}";
        }
    }

    void PlaceStatues()
    {
        Vector3 center = Vector3.zero;
        float statueRadius = 12f;
        int totalStatues = 16;

        for (int i = 0; i < totalStatues; i++)
        {
            float angle = i * (360f / totalStatues) * Mathf.Deg2Rad;
            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * statueRadius;
            Quaternion rot = Quaternion.LookRotation(center - pos);

            int index = i % statuePrefabs.Length;

            if (index == 1)
            {
                // Only index 1 gets a table
                GameObject table = Instantiate(tablePrefab, pos, Quaternion.identity);
                table.name = $"StatueTable_{i}";
                pos.y += 1.1f; // raise statue to sit on table
            }
            else
            {
                // Index 0 and 2 placed directly on floor
                pos.y += 0.05f;
            }

            GameObject statue = Instantiate(statuePrefabs[index], pos, rot);
            statue.name = $"Statue_{i}";
        }
    }
}
