using UnityEngine;

public class MuseumRoomBuilder : MonoBehaviour
{
    [Header("Environment Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject windowPrefab;
    public GameObject ceilingPrefab;

    [Header("Exhibit Prefabs")]
    public GameObject[] statuePrefabs;
    public GameObject tablePrefab;
    public GameObject[] framePrefabs;
    public GameObject bookshelfPrefab;

    [Header("Fire Area Prefabs")]
    public GameObject firePrefab;
    public GameObject chairPrefab;

    [Header("Room Settings")]
    public Vector3 roomSize = new Vector3(20, 5, 20);  // Width, Height, Depth

    void Start()
    {
        BuildRoom();
        PlaceStatues();
        PlaceFrames();
        PlaceBookshelf();
        PlaceFireAndChairs();
    }

    void BuildRoom()
    {
        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.localScale = new Vector3(roomSize.x / 10, 1, roomSize.z / 10);
        floor.transform.position = Vector3.zero;
        floor.name = "Floor";

        // Ceiling
        GameObject ceiling = Instantiate(ceilingPrefab, new Vector3(0, roomSize.y, 0), Quaternion.Euler(180, 0, 0));
        ceiling.transform.localScale = new Vector3(roomSize.x / 10, 1, roomSize.z / 10);
        ceiling.name = "Ceiling";

        // 4 Walls with Windows
        Vector3[] wallPositions = {
            new Vector3(0, roomSize.y / 2, -roomSize.z / 2), // back
            new Vector3(0, roomSize.y / 2, roomSize.z / 2),  // front
            new Vector3(-roomSize.x / 2, roomSize.y / 2, 0), // left
            new Vector3(roomSize.x / 2, roomSize.y / 2, 0)   // right
        };

        Vector3[] wallRotations = {
            Vector3.zero,
            new Vector3(0, 180, 0),
            new Vector3(0, 90, 0),
            new Vector3(0, -90, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = Instantiate(wallPrefab, wallPositions[i], Quaternion.Euler(wallRotations[i]));
            wall.transform.localScale = new Vector3(roomSize.x / 10, roomSize.y / 5, 1);
            wall.name = $"Wall_{i + 1}";

            // Window centered on the wall
            GameObject window = Instantiate(windowPrefab, wallPositions[i], Quaternion.Euler(wallRotations[i]));
            window.transform.position += new Vector3(0, 1.5f, 0);
            window.name = $"Window_{i + 1}";
        }
    }

    void PlaceStatues()
    {
        float spacing = 5f;
        Vector3 start = new Vector3(-spacing * 1.5f, 0, -roomSize.z / 3);

        for (int i = 0; i < 6; i++)
        {
            Vector3 tablePos = start + new Vector3(spacing * i, 0, 0);
            GameObject table = Instantiate(tablePrefab, tablePos, Quaternion.identity);
            table.name = $"Table_{i + 1}";

            GameObject statue = Instantiate(statuePrefabs[i % statuePrefabs.Length], tablePos + new Vector3(0, 1.1f, 0), Quaternion.identity);
            statue.name = $"Statue_{i + 1}";
        }
    }

    void PlaceFrames()
    {
        for (int i = 0; i < framePrefabs.Length; i++)
        {
            GameObject frame = Instantiate(framePrefabs[i], new Vector3(-roomSize.x / 2 + 0.1f, 2f + i * 1.5f, 0), Quaternion.Euler(0, 90, 0));
            frame.name = $"Frame_{i + 1}";
        }
    }

    void PlaceBookshelf()
    {
        Vector3 corner = new Vector3(roomSize.x / 2 - 1, 0, roomSize.z / 2 - 1);
        GameObject shelf = Instantiate(bookshelfPrefab, corner, Quaternion.Euler(0, -45, 0));
        shelf.name = "Bookshelf";
    }

    void PlaceFireAndChairs()
    {
        Vector3 center = new Vector3(0, 0, 0);
        GameObject fire = Instantiate(firePrefab, center + Vector3.up * 0.1f, Quaternion.identity);
        fire.name = "FirePit";

        float radius = 2f;
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            Vector3 chairPos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Quaternion rot = Quaternion.LookRotation(center - chairPos);
            GameObject chair = Instantiate(chairPrefab, chairPos, rot);
            chair.name = $"Chair_{i + 1}";
        }
    }
}
