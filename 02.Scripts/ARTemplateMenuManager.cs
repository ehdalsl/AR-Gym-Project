using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// UI �޴��� AR ����� ���� ����� �����ϴ� ��ũ��Ʈ
/// </summary>
public class ARTemplateMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_ModalMenu;

    [SerializeField]
    XRScreenSpaceController m_ScreenSpaceController;

    [SerializeField]
    DebugSlider m_DebugPlaneSlider;

    [SerializeField]
    ARPlaneManager m_PlaneManager;

    [SerializeField]
    ARDebugMenu m_DebugMenu;

    [SerializeField]
    DebugSlider m_DebugMenuSlider;

    bool m_IsPointerOverUI;
    bool m_ShowObjectMenu;
    bool m_ShowOptionsModal;
    bool m_InitializingDebugMenu;
    Vector2 m_ObjectButtonOffset = Vector2.zero;
    Vector2 m_ObjectMenuOffset = Vector2.zero;
    readonly List<ARFeatheredPlaneMeshVisualizerCompanion> featheredPlaneMeshVisualizerCompanions = new List<ARFeatheredPlaneMeshVisualizerCompanion>();

    /// <summary>
    /// Ȱ��ȭ�� �� ȣ��Ǹ�, �̺�Ʈ �����ʸ� �߰���
    /// </summary>
    void OnEnable()
    {
        m_ScreenSpaceController.dragCurrentPositionAction.action.started += HideTapOutsideUI;
        m_ScreenSpaceController.tapStartPositionAction.action.started += HideTapOutsideUI;
        m_PlaneManager.planesChanged += OnPlaneChanged;
    }

    /// <summary>
    /// ��Ȱ��ȭ�� �� ȣ��Ǹ�, �̺�Ʈ �����ʸ� ������
    /// </summary>
    void OnDisable()
    {
        m_ScreenSpaceController.dragCurrentPositionAction.action.started -= HideTapOutsideUI;
        m_ScreenSpaceController.tapStartPositionAction.action.started -= HideTapOutsideUI;
        m_PlaneManager.planesChanged -= OnPlaneChanged;
    }

    /// <summary>
    /// �ʱ�ȭ �Լ���, ����� �޴��� �ʱ� ������ �� ����
    /// </summary>
    void Start()
    {
        m_DebugMenu.gameObject.SetActive(true);
        m_InitializingDebugMenu = true;

        InitializeDebugMenuOffsets();
        HideMenu();
    }

    /// <summary>
    /// �� �����Ӹ��� ȣ��Ǹ�, UI ���� �� ����� �޴� ���¸� ������
    /// </summary>
    void Update()
    {
        if (m_InitializingDebugMenu)
        {
            m_DebugMenu.gameObject.SetActive(false);
            m_InitializingDebugMenu = false;
        }

        m_IsPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
    }

    /// <summary>
    /// ��� �޴��� �����ְų� ����� �Լ�
    /// </summary>
    public void ShowHideModal()
    {
        if (m_ModalMenu.activeSelf)
        {
            m_ShowOptionsModal = false;
            m_ModalMenu.SetActive(false);
        }
        else
        {
            m_ShowOptionsModal = true;
            m_ModalMenu.SetActive(true);
        }
    }

    /// <summary>
    /// ��� ����� �ð�ȭ�� �����ְų� ����� �Լ�
    /// </summary>
    public void ShowHideDebugPlane()
    {
        if (m_DebugPlaneSlider.value == 1)
        {
            m_DebugPlaneSlider.value = 0;
            ChangePlaneVisibility(false);
        }
        else
        {
            m_DebugPlaneSlider.value = 1;
            ChangePlaneVisibility(true);
        }
    }

    /// <summary>
    /// AR ����� �޴��� �����ְų� ����� �Լ�
    /// </summary>
    public void ShowHideDebugMenu()
    {
        if (m_DebugMenu.gameObject.activeSelf)
        {
            m_DebugMenuSlider.value = 0;
            m_DebugMenu.gameObject.SetActive(false);
        }
        else
        {
            m_DebugMenuSlider.value = 1;
            m_DebugMenu.gameObject.SetActive(true);
            AdjustARDebugMenuPosition();
        }
    }

    /// <summary>
    /// �޴��� ����� �Լ�
    /// </summary>
    public void HideMenu()
    {
        m_ShowObjectMenu = false;
        AdjustARDebugMenuPosition();
    }

    /// <summary>
    /// ��� �ð�ȭ�� �����ϴ� �Լ�
    /// </summary>
    void ChangePlaneVisibility(bool setVisible)
    {
        var count = featheredPlaneMeshVisualizerCompanions.Count;
        for (int i = 0; i < count; ++i)
        {
            featheredPlaneMeshVisualizerCompanions[i].visualizeSurfaces = setVisible;
        }
    }

    /// <summary>
    /// UI �ܺθ� ��ġ���� �� �޴��� ����� �Լ�
    /// </summary>
    void HideTapOutsideUI(InputAction.CallbackContext context)
    {
        if (!m_IsPointerOverUI)
        {
            if (m_ShowObjectMenu)
                HideMenu();
            if (m_ShowOptionsModal)
                m_ModalMenu.SetActive(false);
        }
    }

    /// <summary>
    /// ����� �޴��� ��ġ�� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    void InitializeDebugMenuOffsets()
    {
        if (m_DebugMenu.TryGetComponent<RectTransform>(out var rect))
        {
            m_ObjectButtonOffset = new Vector2(0f, rect.anchoredPosition.y + rect.rect.height + 10f);
        }
    }

    /// <summary>
    /// AR ����� �޴��� ��ġ�� �����ϴ� �Լ�
    /// </summary>
    void AdjustARDebugMenuPosition()
    {
        float screenWidthInInches = Screen.width / Screen.dpi;

        if (screenWidthInInches < 5)
        {
            Vector2 menuOffset = m_ShowObjectMenu ? m_ObjectMenuOffset : m_ObjectButtonOffset;

            if (m_DebugMenu.toolbar.TryGetComponent<RectTransform>(out var rect))
            {
                rect.anchoredPosition = new Vector2(0, 20) + menuOffset;
            }
        }
    }

    /// <summary>
    /// ����� ����� �� ȣ��Ǹ�, ��� �ð�ȭ ������Ʈ�� �����ϴ� �Լ�
    /// </summary>
    void OnPlaneChanged(ARPlanesChangedEventArgs eventArgs)
    {
        if (eventArgs.added.Count > 0)
        {
            foreach (var plane in eventArgs.added)
            {
                if (plane.TryGetComponent<ARFeatheredPlaneMeshVisualizerCompanion>(out var visualizer))
                {
                    featheredPlaneMeshVisualizerCompanions.Add(visualizer);
                    visualizer.visualizeSurfaces = (m_DebugPlaneSlider.value != 0);
                }
            }
        }

        if (eventArgs.removed.Count > 0)
        {
            foreach (var plane in eventArgs.removed)
            {
                if (plane.TryGetComponent<ARFeatheredPlaneMeshVisualizerCompanion>(out var visualizer))
                    featheredPlaneMeshVisualizerCompanions.Remove(visualizer);
            }
        }

        if (m_PlaneManager.trackables.count != featheredPlaneMeshVisualizerCompanions.Count)
        {
            featheredPlaneMeshVisualizerCompanions.Clear();
            foreach (var trackable in m_PlaneManager.trackables)
            {
                if (trackable.TryGetComponent<ARFeatheredPlaneMeshVisualizerCompanion>(out var visualizer))
                {
                    featheredPlaneMeshVisualizerCompanions.Add(visualizer);
                    visualizer.visualizeSurfaces = (m_DebugPlaneSlider.value != 0);
                }
            }
        }
    }
}
