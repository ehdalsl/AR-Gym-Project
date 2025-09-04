using UnityEngine;

public class PrefabController : MonoBehaviour
{
    public GameObject currentPrefab;
    public GameObject newPrefab; // ���� ��Ÿ�� ������

    public void SpawnNewPrefab(Vector3 position)
    {
        // ���� �������� ���� ���, UI�� ����� ��Ȱ��ȭ
        if (currentPrefab != null)
        {
            currentPrefab.GetComponent<PrefabUIManager>().HideUI();
            Destroy(currentPrefab); // �ʿ�� �ı�
        }

        // �� ������ ���� �� Ȱ��ȭ
        currentPrefab = Instantiate(newPrefab, position, Quaternion.identity);

        // **�������� UI�� Ȱ��ȭ**
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