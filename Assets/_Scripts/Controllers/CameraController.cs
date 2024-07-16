using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeedMouse = 1;
    [Space(5)]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15;

    [HideInInspector]
    public bool CanMoveCamera = true;
    
    private Vector3 m_StartMousePos;
    
    private void Update()
    {
        HandleZoom();

#if UNITY_EDITOR

        // Check if some touch exist
        if (!Input.GetMouseButton(0))
            return;

        // On touch start
        if (Input.GetMouseButtonDown(0))
            OnTouchStart();

        // Updating camera position
        UpdateCameraPosition();

#else

        // Check if some touch exist
        if (Input.touchCount <= 0)
            return;

        // On touch start
        if (Input.GetTouch(0).phase == TouchPhase.Began)
            OnTouchStart();

        // Updating camera position
        UpdateCameraPosition();

#endif
    }

    private void OnTouchStart()
    {
        // Setting start touch position
#if UNITY_EDITOR

        m_StartMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

#else

        m_StartMousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

#endif
    }

    private void UpdateCameraPosition()
    {
        var camera = Camera.main;

#if UNITY_EDITOR
        
        var touchPos = Input.mousePosition;

#else

        var touchPos = Input.GetTouch(0).position;

#endif

        var diff = m_StartMousePos - camera.ScreenToWorldPoint(touchPos);

        camera.transform.position += diff;
    }

    private void HandleZoom()
    {
#if UNITY_EDITOR

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0.0f)
            return;
        
        Camera.main.orthographicSize -= scroll * zoomSpeedMouse;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);

#else

        if (Input.touchCount < 2)
            return;

        var touchZero = Input.GetTouch(0);
        var touchOne = Input.GetTouch(1);

        var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        Camera.main.orthographicSize -= difference * zoomSpeedTouch;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);

#endif
    }
}
