using UnityEngine;

public class PrefabTouchHandler : MonoBehaviour
{
    private PrefabUIManager uiManager;

    void Start()
    {
        uiManager = GetComponent<PrefabUIManager>();
        if (uiManager == null)
        {
            Debug.LogError("PrefabUIManager component is missing from the prefab.");
        }
    }

    void OnMouseDown()
    {
        if (uiManager != null)
        {
            uiManager.ShowUI(3f); // UI가 3초 뒤에 사라지도록 설정
            Debug.Log("UI shown on touch, will hide after 3 seconds.");
        }
    }
}
