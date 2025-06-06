using UnityEngine;

public class RestaurantLayout : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public GameObject wallPrefab;
    public GameObject tablePrefab;
    public GameObject chairPrefab;
    public GameObject burgerPrefab;

    [Header("Spawn Point")]
    public Transform playerSpawnPoint;

    private Vector3 baseWorldPosition = new Vector3(0, 10, 0); // Floor's global position

    void Start()
    {
        transform.position = Vector3.zero;

        CreateFloor();
        CreateCeiling();
        CreateWalls();
        CreateLighting();
        CreatePlayerSpawnPoint();
        CreateTablesChairsBurgers();
    }

    void CreateFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.parent = transform;
        floor.transform.position = baseWorldPosition;
        floor.transform.localScale = new Vector3(10, 0.1f, 10);

        Renderer renderer = floor.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material floorMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Texture2D tex = Resources.Load<Texture2D>("Textures/restaurant_floor");

            if (tex != null)
            {
                floorMat.SetTexture("_BaseMap", tex);
                floorMat.SetTextureScale("_BaseMap", new Vector2(5, 5));
                renderer.material = floorMat;
            }
            else
            {
                Debug.LogWarning("❌ Texture not found for floor.");
            }
        }
    }

    void CreateCeiling()
    {
        GameObject ceiling = Instantiate(ceilingPrefab, transform);
        ceiling.name = "Ceiling";
        ceiling.transform.localPosition = new Vector3(0, 3, 0);
        ceiling.transform.localScale = new Vector3(1, 0.01f, 1);
        ApplyTransparentMaterial(ceiling);
    }

    void CreateWalls()
    {
        float wallHeight = 3f;
        float wallThickness = 0.2f;
        float roomSize = 10f;
        float baseY = baseWorldPosition.y;

        float wallY = baseY + wallHeight / 2f;

        CreateWall("Wall_Back", new Vector3(0, wallY, -roomSize / 2f), new Vector3(roomSize, wallHeight, wallThickness));
        CreateWall("Wall_Front", new Vector3(0, wallY, roomSize / 2f), new Vector3(roomSize, wallHeight, wallThickness));
        CreateWall("Wall_Left", new Vector3(-roomSize / 2f, wallY, 0), new Vector3(wallThickness, wallHeight, roomSize));
        CreateWall("Wall_Right", new Vector3(roomSize / 2f, wallY, 0), new Vector3(wallThickness, wallHeight, roomSize));
    }

    void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.parent = transform;
        wall.transform.position = position;
        wall.transform.localScale = scale;
        ApplyGlassTexture(wall);
    }

    void CreateLighting()
    {
        GameObject lightObj = new GameObject("CeilingLight");
        lightObj.transform.parent = transform;
        lightObj.transform.position = baseWorldPosition + new Vector3(0, 2.8f, 0);

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 10f;
        light.intensity = 1.2f;
    }

    void CreatePlayerSpawnPoint()
    {
        GameObject spawn = new GameObject("RoomEntryPoint");
        spawn.transform.parent = transform;
        spawn.transform.position = baseWorldPosition + new Vector3(0, 0.1f, 0);
        playerSpawnPoint = spawn.transform;
    }

    void CreateTablesChairsBurgers()
    {
        if (tablePrefab == null || chairPrefab == null) return;

        Vector3[] positions = new Vector3[]
        {
            new Vector3(-3, 0.05f, -3),
            new Vector3(3, 0.05f, -3),
            new Vector3(-3, 0.05f, 3),
            new Vector3(3, 0.05f, 3)
        };

        foreach (Vector3 localOffset in positions)
        {
            Vector3 tablePos = baseWorldPosition + localOffset;

            GameObject table = Instantiate(tablePrefab, transform);
            table.name = "Table";
            table.transform.position = tablePos;
            ApplyMetalTexture(table);

            float offset = 1.2f;
            CreateChair(tablePos + new Vector3(offset, 0, 0), tablePos, table.transform);
            CreateChair(tablePos + new Vector3(-offset, 0, 0), tablePos, table.transform);
            CreateChair(tablePos + new Vector3(0, 0, offset), tablePos, table.transform);
            CreateChair(tablePos + new Vector3(0, 0, -offset), tablePos, table.transform);

            if (burgerPrefab != null)
            {
                Vector3[] burgerOffsets = new Vector3[]
                {
                    new Vector3(0.3f, 1.1f, 0.3f),
                    new Vector3(-0.3f, 1.1f, 0.3f),
                    new Vector3(0.3f, 1.1f, -0.3f),
                    new Vector3(-0.3f, 1.1f, -0.3f)
                };

                foreach (Vector3 offsetPos in burgerOffsets)
                {
                    GameObject burger = Instantiate(burgerPrefab, table.transform);
                    burger.name = "Burger";
                    burger.transform.position = tablePos + offsetPos;
                    burger.transform.localScale = new Vector3(1.2f, 0.7f, 1.2f);
                    ApplyBurgerTexture(burger);
                }
            }
        }
    }

    void CreateChair(Vector3 chairPosition, Vector3 lookAtPosition, Transform parent)
    {
        GameObject chair = Instantiate(chairPrefab, parent);
        chair.name = "Chair";
        chair.transform.position = chairPosition;
        chair.transform.localScale *= 1.5f;

        Vector3 lookDir = lookAtPosition - chairPosition;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            chair.transform.rotation = Quaternion.LookRotation(lookDir);
        }

        ApplyMetalTexture(chair);
    }

    void ApplyTransparentMaterial(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material transparentMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            transparentMat.SetFloat("_Surface", 1);
            transparentMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            transparentMat.renderQueue = 3000;
            transparentMat.color = new Color(1f, 1f, 1f, 0.2f);
            renderer.material = transparentMat;
        }
    }

    void ApplyGlassTexture(GameObject wall)
    {
        Renderer renderer = wall.GetComponent<Renderer>();
        if (renderer != null)
        {
            Texture2D tex = Resources.Load<Texture2D>("Textures/restaurant_walls");
            Material glassMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            if (tex != null)
            {
                glassMat.SetTexture("_BaseMap", tex);
            }

            glassMat.SetFloat("_Surface", 1);
            glassMat.SetFloat("_Blend", 0);
            glassMat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            glassMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            glassMat.SetFloat("_ZWrite", 0);
            glassMat.renderQueue = 3000;
            glassMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            glassMat.color = new Color(1f, 1f, 1f, 0.5f);

            renderer.material = glassMat;
        }
    }

    void ApplyMetalTexture(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Texture2D tex = Resources.Load<Texture2D>("Textures/Bench");

            if (tex != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.SetTexture("_BaseMap", tex);
                mat.SetTextureScale("_BaseMap", new Vector2(2f, 2f));
                mat.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                mat.SetFloat("_Smoothness", 0.7f);
                mat.SetFloat("_Metallic", 0.9f);
                renderer.material = mat;
            }
            else
            {
                Debug.LogWarning("❌ Metal texture not found.");
            }
        }
    }

    void ApplyBurgerTexture(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Texture2D tex = Resources.Load<Texture2D>("Textures/Burger");

            if (tex != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.SetTexture("_BaseMap", tex);
                mat.SetTextureScale("_BaseMap", new Vector2(1, 1));
                mat.color = Color.white;
                renderer.material = mat;
            }
            else
            {
                Debug.LogWarning("❌ burger_texture.png not found in Resources/Textures");
            }
        }
    }
}