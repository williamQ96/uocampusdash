using UnityEngine;

public class MergeWithUniqueNames : MonoBehaviour
{
    public Transform[] sources;         // Drag your 4 road pieces here in editor
    public Transform destinationParent; // Create an empty GameObject as your new container

    [ContextMenu("Merge With Unique Names")]
    void Merge()
    {
        foreach (Transform src in sources)
        {
            RenameRecursively(src, src.name + "_");
            src.SetParent(destinationParent, true);
        }

        Debug.Log("Merge complete.");
    }

    void RenameRecursively(Transform t, string prefix)
    {
        t.name = prefix + t.name;
        foreach (Transform child in t)
        {
            RenameRecursively(child, prefix);
        }
    }
}
