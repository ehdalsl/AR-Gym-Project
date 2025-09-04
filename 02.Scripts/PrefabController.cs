using UnityEngine;

public class PrefabController : MonoBehaviour
{
    public GameObject currentPrefab;
    public GameObject newPrefab; // 새로 나타날 프리팹

    public void SpawnNewPrefab(Vector3 position)
    {
        // 기존 프리팹이 있을 경우, UI를 숨기고 비활성화
        if (currentPrefab != null)
        {
            currentPrefab.GetComponent<PrefabUIManager>().HideUI();
            Destroy(currentPrefab); // 필요시 파괴
        }

        // 새 프리팹 생성 및 활성화
        currentPrefab = Instantiate(newPrefab, position, Quaternion.identity);

        // **프리팹의 UI를 활성화**
        PrefabUIManager uiManager = currentPrefab.GetComponent<PrefabUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowUI();
            Debug.Log("ShowUI called for new prefab.");
        }
        else
        {
            Debug.LogError("No PrefabUIManager found on the new prefab.");
        }
    }
} 