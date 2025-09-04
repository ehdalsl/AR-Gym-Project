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
            uiManager.ShowUI(3f); // UI�� 3�� �ڿ� ��������� ����
            Debug.Log("UI shown on touch, will hide after 3 seconds.");
        }
    }
}
