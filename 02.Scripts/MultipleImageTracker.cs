using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultipleImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [Tooltip("이미지와 연결될 프리팹 리스트")]
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
            Debug.Log($"이미 생성된 오브젝트: {referenceImageName}");
            return;
        }

        GameObject prefabToSpawn = GetPrefabByName(referenceImageName);
        if (prefabToSpawn != null)
        {
            // 이미지 위로 약간 배치
            Vector3 adjustedPosition = trackedImage.transform.position + Vector3.up * 0.1f;
            GameObject newObject = Instantiate(prefabToSpawn, adjustedPosition, Quaternion.identity);

            // 회전값 조정
            newObject.transform.rotation = Quaternion.Euler(0, trackedImage.transform.rotation.eulerAngles.y, 0);

            // 크기 설정: 화면 크기의 50%
            newObject.transform.localScale = Vector3.one * 0.5f;

            // 딕셔너리에 추가
            spawnedObjects[referenceImageName] = newObject;
            Debug.Log($"오브젝트 생성: {referenceImageName}");
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        string referenceImageName = trackedImage.referenceImage.name;

        if (spawnedObjects.ContainsKey(referenceImageName) && spawnedObjects[referenceImageName] != null)
        {
            GameObject spawnedObject = spawnedObjects[referenceImageName];
            spawnedObject.transform.SetPositionAndRotation(
                trackedImage.transform.position + Vector3.up * 0.1f, // 위치 업데이트 (이미지 위로 약간 이동)
                Quaternion.Euler(0, trackedImage.transform.rotation.eulerAngles.y, 0) // 회전값 조정
            );
            Debug.Log($"오브젝트 업데이트: {referenceImageName}");
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
        Debug.LogWarning($"프리팹을 찾을 수 없습니다: {name}");
        return null;
    }
}
