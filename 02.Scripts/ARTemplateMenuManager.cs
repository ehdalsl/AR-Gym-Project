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
/// UI 메뉴와 AR 디버그 관련 기능을 관리하는 스크립트
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
    /// 활성화될 때 호출되며, 이벤트 리스너를 추가함
    /// </summary>
    void OnEnable()
    {
        m_ScreenSpaceController.dragCurrentPositionAction.action.started += HideTapOutsideUI;
        m_ScreenSpaceController.tapStartPositionAction.action.started += HideTapOutsideUI;
        m_PlaneManager.planesChanged += OnPlaneChanged;
    }

    /// <summary>
    /// 비활성화될 때 호출되며, 이벤트 리스너를 제거함
    /// </summary>
    void OnDisable()
    {
        m_ScreenSpaceController.dragCurrentPositionAction.action.started -= HideTapOutsideUI;
        m_ScreenSpaceController.tapStartPositionAction.action.started -= HideTapOutsideUI;
        m_PlaneManager.planesChanged -= OnPlaneChanged;
    }

    /// <summary>
    /// 초기화 함수로, 디버그 메뉴를 초기 설정한 후 숨김
    /// </summary>
    void Start()
    {
        m_DebugMenu.gameObject.SetActive(true);
        m_InitializingDebugMenu = true;

        InitializeDebugMenuOffsets();
        HideMenu();
    }

    /// <summary>
    /// 매 프레임마다 호출되며, UI 상태 및 디버그 메뉴 상태를 관리함
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
    /// 모달 메뉴를 보여주거나 숨기는 함수
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
    /// 평면 디버그 시각화를 보여주거나 숨기는 함수
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
    /// AR 디버그 메뉴를 보여주거나 숨기는 함수
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
    /// 메뉴를 숨기는 함수
    /// </summary>
    public void HideMenu()
    {
        m_ShowObjectMenu = false;
        AdjustARDebugMenuPosition();
    }

    /// <summary>
    /// 평면 시각화를 변경하는 함수
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
    /// UI 외부를 터치했을 때 메뉴를 숨기는 함수
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
    /// 디버그 메뉴의 위치를 초기화하는 함수
    /// </summary>
    void InitializeDebugMenuOffsets()
    {
        if (m_DebugMenu.TryGetComponent<RectTransform>(out var rect))
        {
            m_ObjectButtonOffset = new Vector2(0f, rect.anchoredPosition.y + rect.rect.height + 10f);
        }
    }

    /// <summary>
    /// AR 디버그 메뉴의 위치를 조정하는 함수
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
    /// 평면이 변경될 때 호출되며, 평면 시각화 컴포넌트를 관리하는 함수
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
