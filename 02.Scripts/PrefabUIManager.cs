using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabUIManager : MonoBehaviour
{
    public GameObject uiCanvas;

    private static List<PrefabUIManager> activeUIs = new List<PrefabUIManager>();

    protected virtual void Start()
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
            Debug.Log("UI Canvas is set to inactive at start.");
        }
    }

    public virtual void ShowUI(float hideDelay = 0f)
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true);
            AddToActiveUIs();
            Debug.Log("UI Canvas is now active.");

            if (hideDelay > 0f)
            {
                StartCoroutine(HideAfterDelay(hideDelay)); // Áö¿¬ ÈÄ UI ¼û±è
            }
        }
        else
        {
            Debug.LogError("ShowUI: UI Canvas is null.");
        }
    }

    public virtual void HideUI()
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
            RemoveFromActiveUIs();
            Debug.Log("UI Canvas is now inactive.");
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideUI();
        Debug.Log("UI automatically hidden after delay.");
    }

    private void AddToActiveUIs()
    {
        if (!activeUIs.Contains(this))
        {
            activeUIs.Add(this);
        }
    }

    private void RemoveFromActiveUIs()
    {
        if (activeUIs.Contains(this))
        {
            activeUIs.Remove(this);
        }
    }
}
