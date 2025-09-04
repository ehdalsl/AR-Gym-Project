using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// GoalManager�� �Ϻη� �޼��ؾ� �ϴ� �º��� ��ǥ�� ��Ÿ���� ����ü
/// </summary>
public struct Goal
{
    public GoalManager.OnboardingGoals CurrentGoal;
    public bool Completed;

    public Goal(GoalManager.OnboardingGoals goal)
    {
        CurrentGoal = goal;
        Completed = false;
    }
}

/// <summary>
/// GoalManager�� ����ڰ� �Ϸ��ؾ� �� �Ϸ��� �º��� ��ǥ�� ��ȯ ó��
/// </summary>
public class GoalManager : MonoBehaviour
{
    public enum OnboardingGoals
    {
        Empty,
        FindSurfaces,
        TapSurface,
        Hints,
        Scale
    }

    [Tooltip("�º��� ���� �� ǥ�õǴ� �λ縻 ������Ʈ GameObject")]
    [SerializeField]
    GameObject m_GreetingPrompt;

    public GameObject greetingPrompt
    {
        get => m_GreetingPrompt;
        set => m_GreetingPrompt = value;
    }

    [Tooltip("�λ縻 ������Ʈ�� �����Ǹ� Ȱ��ȭ�Ǵ� �ɼ� ��ư")]
    [SerializeField]
    GameObject m_OptionsButton;

    public GameObject optionsButton
    {
        get => m_OptionsButton;
        set => m_OptionsButton = value;
    }

    [Tooltip("�λ縻 ������Ʈ�� �����Ǹ� Ȱ��ȭ�Ǵ� AR ���ø� �޴� ������ ��ü")]
    [SerializeField]
    ARTemplateMenuManager m_MenuManager;

    public ARTemplateMenuManager menuManager
    {
        get => m_MenuManager;
        set => m_MenuManager = value;
    }

    // **�߰��� �κ� ����**

    [Tooltip("������Ʈ ��ü ���� ��ư")]
    [SerializeField]
    GameObject m_DeleteAllButton;

    /// <summary>
    /// ������ ������Ʈ���� �����ϴ� ����Ʈ
    /// </summary>
    List<GameObject> m_SpawnedObjects = new List<GameObject>();

    // **�߰��� �κ� ��**

    const int k_NumberOfSurfacesTappedToCompleteGoal = 1;

    Queue<Goal> m_OnboardingGoals;
    Coroutine m_CurrentCoroutine;
    Goal m_CurrentGoal;
    bool m_AllGoalsFinished;
    int m_SurfacesTapped;

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame && !m_AllGoalsFinished &&
            (m_CurrentGoal.CurrentGoal == OnboardingGoals.FindSurfaces ||
             m_CurrentGoal.CurrentGoal == OnboardingGoals.Hints ||
             m_CurrentGoal.CurrentGoal == OnboardingGoals.Scale))
        {
            if (m_CurrentCoroutine != null)
            {
                StopCoroutine(m_CurrentCoroutine);
            }
            CompleteGoal();
        }
    }

    void CompleteGoal()
    {
        m_CurrentGoal.Completed = true;
        if (m_OnboardingGoals.Count > 0)
        {
            m_CurrentGoal = m_OnboardingGoals.Dequeue();
        }
        else
        {
            m_AllGoalsFinished = true;
        }

        PreprocessGoal();
    }

    void PreprocessGoal()
    {
        if (m_CurrentGoal.CurrentGoal == OnboardingGoals.FindSurfaces)
        {
            m_CurrentCoroutine = StartCoroutine(WaitUntilNextCard(5f));
        }
        else if (m_CurrentGoal.CurrentGoal == OnboardingGoals.Hints)
        {
            m_CurrentCoroutine = StartCoroutine(WaitUntilNextCard(6f));
        }
        else if (m_CurrentGoal.CurrentGoal == OnboardingGoals.Scale)
        {
            m_CurrentCoroutine = StartCoroutine(WaitUntilNextCard(8f));
        }
        else if (m_CurrentGoal.CurrentGoal == OnboardingGoals.TapSurface)
        {
            m_SurfacesTapped = 0;
            CreateObjectAtPosition(Vector3.zero); // ���÷� (0,0,0)�� ����
        }
    }

    public IEnumerator WaitUntilNextCard(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (!Pointer.current.press.wasPressedThisFrame)
        {
            m_CurrentCoroutine = null;
            CompleteGoal();
        }
    }

    public void ForceCompleteGoal()
    {
        CompleteGoal();
    }

    void CreateObjectAtPosition(Vector3 position)
    {
        GameObject prefab = null; // ���⿡ �������� �Ҵ��ϰų� �ν����Ϳ��� ����
        if (prefab != null)
        {
            GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
            m_SpawnedObjects.Add(newObject);
            m_SurfacesTapped++;
            if (m_SurfacesTapped >= k_NumberOfSurfacesTappedToCompleteGoal)
            {
                CompleteGoal();
            }
        }
    }

    /// <summary>
    /// ��� ������ ������Ʈ�� �����ϴ� �Լ�
    /// </summary>
    public void DeleteAllObjects()
    {
        foreach (var obj in m_SpawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
                Debug.Log("Deleted object: " + obj.name); // ����� �α� �߰�
            }
        }
        m_SpawnedObjects.Clear();
    }

    public void StartCoaching()
    {
        if (m_OnboardingGoals != null)
        {
            m_OnboardingGoals.Clear();
        }

        m_OnboardingGoals = new Queue<Goal>();

        if (!m_AllGoalsFinished)
        {
            var findSurfaceGoal = new Goal(OnboardingGoals.FindSurfaces);
            m_OnboardingGoals.Enqueue(findSurfaceGoal);
        }

        var tapSurfaceGoal = new Goal(OnboardingGoals.TapSurface);
        var translateHintsGoal = new Goal(OnboardingGoals.Hints);
        var scaleHintsGoal = new Goal(OnboardingGoals.Scale);
        var rotateHintsGoal = new Goal(OnboardingGoals.Hints);

        m_OnboardingGoals.Enqueue(tapSurfaceGoal);
        m_OnboardingGoals.Enqueue(translateHintsGoal);
        m_OnboardingGoals.Enqueue(scaleHintsGoal);
        m_OnboardingGoals.Enqueue(rotateHintsGoal);

        m_CurrentGoal = m_OnboardingGoals.Dequeue();
        m_AllGoalsFinished = false;

        if (m_GreetingPrompt != null)
            m_GreetingPrompt.SetActive(false);
        else
            Debug.LogError("m_GreetingPrompt is not assigned.");

        if (m_OptionsButton != null)
            m_OptionsButton.SetActive(true);
        else
            Debug.LogError("m_OptionsButton is not assigned.");

        if (m_MenuManager != null)
            m_MenuManager.enabled = true;
        else
            Debug.LogError("m_MenuManager is not assigned.");

        PreprocessGoal();
    }
}
