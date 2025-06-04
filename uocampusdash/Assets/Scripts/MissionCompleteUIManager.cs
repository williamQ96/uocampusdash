using UnityEngine;

public class MissionCompleteUIManager : MonoBehaviour
{public GameObject successMenu;
public GameObject failureMenu;

    public static MissionCompleteUIManager Instance;

    [Tooltip("Assign the unified reward panel here")]
    public GameObject rewardPanel;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowRewardMenu()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(true);
    }

    public void HideRewardMenu()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(false);
    }
    public void HideAllMenus()
{
    if (successMenu != null) successMenu.SetActive(false);
    if (failureMenu != null) failureMenu.SetActive(false);
}

}
