using UnityEngine;

public class MuseumRoomBuilder : MonoBehaviour
{
    [Header("Environment Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject windowPrefab;
    public GameObject ceilingPrefab;

    [Header("Exhibit Prefabs")]
    public GameObject[] statuePrefabs; // index 1 uses a table, index 2 floats
    public GameObject tablePrefab;

    [Header("Fire Area Prefabs")]
    public GameObject firePrefab;
    public GameObject chairPrefab;

    [Header("Room Settings")]
    public Vector3 roomSize = new Vector3(40, 10, 40); // width, height, length

    public Vector3 FloorCenter => Vector3.zero;

    void Start()
    {
        BuildRoom();
        PlaceFireAndChairs();
        PlaceStatues();
    }

    void BuildRoom()
    {
        // === Floor ===
        Vector3 floorScale = new Vector3(40, 1, 40); // Unity Plane base is 10x10 → 400x400 world units
        GameObject floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
        floor.transform.localScale = floorScale;
        floor.name = "Floor";

        if (floor.GetComponent<Collider>() == null)
            floor.AddComponent<MeshCollider>();

        float unitSize = 10f; // Plane base size
        float actualWidth = floorScale.x * unitSize;
        float actualLength = floorScale.z * unitSize;
        float wallHeight = roomSize.y;
        float wallThickness = 0.5f;

        // === Walls ===
        Vector3[] wallPositions = {
            new Vector3(0, wallHeight / 2f, -actualLength / 2f), // back
            new Vector3(0, wallHeight / 2f, actualLength / 2f),  // front
            new Vector3(-actualWidth / 2f, wallHeight / 2f, 0),  // left
            new Vector3(actualWidth / 2f, wallHeight / 2f, 0)    // right
        };

        Vector3[] wallRotations = {
            Vector3.zero,
            new Vector3(0, 180, 0),
            new Vector3(0, 90, 0),
            new Vector3(0, -90, 0)
        };

        Vector3[] wallScales = {
            new Vector3(actualWidth, wallHeight, wallThickness),
            new Vector3(actualWidth, wallHeight, wallThickness),
            new Vector3(actualLength, wallHeight, wallThickness),
            new Vector3(actualLength, wallHeight, wallThickness)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(wallPrefab, wallPositions[i], Quaternion.Euler(wallRotations[i]));
            wall.transform.localScale = wallScales[i];
            wall.name = $"Wall_{i + 1}";

            GameObject window = Instantiate(windowPrefab, wallPositions[i] + new Vector3(0, 1.5f, 0), Quaternion.Euler(wallRotations[i]));
            window.name = $"Window_{i + 1}";
        }

        // === Ceiling ===
        float ceilingY = wallHeight;
        GameObject ceiling = Instantiate(ceilingPrefab, new Vector3(0, ceilingY, 0), Quaternion.identity);
        ceiling.transform.localScale = floorScale;
        ceiling.name = "Ceiling";
    }



    void PlaceFireAndChairs()
    {
        Vector3 center = Vector3.zero;

        // Place fire pit
        GameObject fire = Instantiate(firePrefab, center + Vector3.up * 0.1f, Quaternion.identity);
        fire.transform.localScale *= 1.5f;
        fire.name = "FirePit";

        // Place chairs around fire
        float chairRadius = 8f;
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * chairRadius;
            Quaternion rot = Quaternion.LookRotation(center - pos);

            GameObject chair = Instantiate(chairPrefab, pos + Vector3.up * 0.05f, rot);
            chair.name = $"Chair_{i + 1}";
            chair.transform.localScale *= 2f; // Enlarge chair
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
                GameObject table = Instantiate(tablePrefab, pos, Quaternion.identity);
                table.name = $"StatueTable_{i}";
                pos.y += 1.1f;
            }
            else if (index == 2)
            {
                pos.y += 3f; // floating
            }
            else
            {
                pos.y += 0.05f;
            }

            GameObject statue = Instantiate(statuePrefabs[index], pos, rot);
            statue.name = $"Statue_{i}";

            statue.AddComponent<Rotator>();
        }
    }
}
