using UnityEngine;
using UnityEngine.UI;

public class StoreUIManager : MonoBehaviour
{
    public GameObject[] storeButtons; // Upgrade, Menu
    private int selectedIndex = 0;

    void Start()
    {
        UpdateButtonVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = storeButtons.Length - 1;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= storeButtons.Length) selectedIndex = 0;
            UpdateButtonVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("ENTER Pressed! Store selection: " + selectedIndex);
            if (selectedIndex == 0)
            {
                FindObjectOfType<StoreManager>().UpgradeSpeed();
            }
            else if (selectedIndex == 1)
            {
                FindObjectOfType<GameUIManager>().ShowMainMenu();
                gameObject.SetActive(false); // 隐藏StorePanel
            }
        }
    }

    void UpdateButtonVisuals()
    {
        for (int i = 0; i < storeButtons.Length; i++)
        {
            var image = storeButtons[i].GetComponent<Image>();
            if (image != null)
            {
                if (i == selectedIndex)
                    image.color = Color.yellow;
                else
                    image.color = Color.white;
            }
        }
    }
}
