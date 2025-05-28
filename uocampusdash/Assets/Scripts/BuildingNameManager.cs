using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Manages displaying building names as UI labels when the player is near buildings.
/// </summary>

public class BuildingNameManager : MonoBehaviour
{
    public Camera mainCamera; // Assign the main camera
    public GameObject nameUIPrefab; // Assign the UI prefab for building names

    public float maxVisibleDistance = 100f; // Maximum distance visibility

    // Stores the buildings and their corresponding UI name objects
    private List<(Transform building, GameObject nameUI)> trackedBuildings = new(); 

    void Start()
    {
        // Find all objects in the scene with a BuildingName component
        BuildingName[] buildings = FindObjectsOfType<BuildingName>();
        foreach (var building in buildings)
        {
            // Instantiate the UI name label and set the text
            GameObject nameUI = Instantiate(nameUIPrefab, transform); 
            nameUI.GetComponent<TextMeshProUGUI>().text = building.buildingName;

            trackedBuildings.Add((building.transform, nameUI)); // Track this building and its label
        }
    }

void Update()
{
    foreach (var (building, nameUI) in trackedBuildings)
    {
        if (building == null) continue;

        // Convert building world position to screen space
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(building.position + Vector3.up * 5f);

        // Set UI position on screen
        nameUI.transform.position = screenPosition;

        // Show or hide based on distance and visibility
        float distance = Vector3.Distance(mainCamera.transform.position, building.position);

        // Optionally: hide if behind the camera
        if (screenPosition.z < 0)
        {
            nameUI.SetActive(false);
        }
        else
        {
            nameUI.SetActive(distance <= maxVisibleDistance);
        }
    }
}
}
