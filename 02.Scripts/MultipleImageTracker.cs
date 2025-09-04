using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultipleImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [Tooltip("�̹����� ����� ������ ����Ʈ")]
    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            HandleTrackedImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImage(trackedImage);
        }
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;

        if (spawnedObjects.ContainsKey(referenceImageName) && spawnedObjects[referenceImageName] != null)
        {
            Debug.Log($"�̹� ������ ������Ʈ: {referenceImageName}");
            return;
        }

        GameObject prefabToSpawn = GetPrefabByName(referenceImageName);
        if (prefabToSpawn != null)
        {
            // �̹��� ���� �ణ ��ġ
            Vector3 adjustedPosition = trackedImage.transform.position + Vector3.up * 0.1f;
            GameObject newObject = Instantiate(prefabToSpawn, adjustedPosition, Quaternion.identity);

            // ȸ���� ����
            newObject.transform.rotation = Quaternion.Euler(0, trackedImage.transform.rotation.eulerAngles.y, 0);

            // ũ�� ����: ȭ�� ũ���� 50%
            newObject.transform.localScale = Vector3.one * 0.5f;

            // ��ųʸ��� �߰�
            spawnedObjects[referenceImageName] = newObject;
            Debug.Log($"������Ʈ ����: {referenceImageName}");
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;

        if (spawnedObjects.ContainsKey(referenceImageName) && spawnedObjects[referenceImageName] != null)
        {
            GameObject spawnedObject = spawnedObjects[referenceImageName];
            spawnedObject.transform.SetPositionAndRotation(
                trackedImage.transform.position + Vector3.up * 0.1f, // ��ġ ������Ʈ (�̹��� ���� �ణ �̵�)
                Quaternion.Euler(0, trackedImage.transform.rotation.eulerAngles.y, 0) // ȸ���� ����
            );
            Debug.Log($"������Ʈ ������Ʈ: {referenceImageName}");
        }
    }

    private GameObject GetPrefabByName(string name)
    {
        foreach (var prefab in placeablePrefabs)
        {
            if (prefab.name == name)
            {
                return prefab;
            }
        }
        Debug.LogWarning($"�������� ã�� �� �����ϴ�: {name}");
        return null;
    }
}
