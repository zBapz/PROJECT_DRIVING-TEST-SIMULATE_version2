using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    // Main Cameras
    private Camera driverSeatCamera;
    private Camera rearViewCamera;
    private Camera topViewCamera;

    // Additional Cameras
    private Camera leftMirrorCamera;
    private Camera rightMirrorCamera;
    private Camera reversingCamera;

    // RenderTextures for additional cameras
    private RenderTexture leftMirrorTexture;
    private RenderTexture rightMirrorTexture;
    private RenderTexture reversingTexture;

    // UI Elements
    private Canvas miniCanvas;
    private RawImage leftMirrorImage;
    private RawImage rightMirrorImage;
    private RawImage reversingImage;

    private Camera currentCamera; // Declare currentCamera here

    void Start()
    {
        // Initialize Cameras
        InitializeCameras();

        // Initialize RenderTextures
        InitializeRenderTextures();

        // Initialize UI
        InitializeUI();

        // Set main camera
        SwitchMainCamera(driverSeatCamera);
    }

    void InitializeCameras()
    {
        driverSeatCamera = FindChildCamera("DriverSeatCamera");
        if (driverSeatCamera == null)
        {
            driverSeatCamera = CreateCamera("DriverSeatCamera", new Vector3(0f, 1f, -2f), Vector3.zero);
            driverSeatCamera.tag = "MainCamera"; // Set as main camera
            driverSeatCamera.enabled = true;
        }
        else
        {
            driverSeatCamera.enabled = true;
        }

        rearViewCamera = FindChildCamera("RearViewCamera") ?? CreateCamera("RearViewCamera", new Vector3(0f, 1f, 2f), new Vector3(0f, 180f, 0f));
        rearViewCamera.enabled = false;

        topViewCamera = FindChildCamera("TopViewCamera") ?? CreateCamera("TopViewCamera", new Vector3(0f, 5f, 0f), new Vector3(90f, 0f, 0f));
        topViewCamera.enabled = false;

        leftMirrorCamera = FindChildCamera("LeftMirrorCamera") ?? CreateCamera("LeftMirrorCamera", new Vector3(-1.5f, 1.5f, -0.5f), new Vector3(0f, 30f, 0f));
        leftMirrorCamera.enabled = false;

        rightMirrorCamera = FindChildCamera("RightMirrorCamera") ?? CreateCamera("RightMirrorCamera", new Vector3(1.5f, 1.5f, -0.5f), new Vector3(0f, -30f, 0f));
        rightMirrorCamera.enabled = false;

        reversingCamera = FindChildCamera("ReversingCamera") ?? CreateCamera("ReversingCamera", new Vector3(0f, 1f, 3f), new Vector3(90f, 0f, 0f));
        reversingCamera.enabled = false;
    }

    Camera FindChildCamera(string name)
    {
        Transform camTransform = transform.Find(name);
        return camTransform != null ? camTransform.GetComponent<Camera>() : null;
    }

    Camera CreateCamera(string name, Vector3 position, Vector3 rotation)
    {
        GameObject camObj = new GameObject(name);
        camObj.transform.parent = this.transform;
        camObj.transform.localPosition = position;
        camObj.transform.localEulerAngles = rotation;
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.fieldOfView = 60f;
        cam.enabled = false;
        return cam;
    }

    void InitializeRenderTextures()
    {
        // Create RenderTextures for additional cameras
        leftMirrorTexture = new RenderTexture(256, 192, 16);
        rightMirrorTexture = new RenderTexture(256, 192, 16);
        reversingTexture = new RenderTexture(256, 192, 16);

        // Assign RenderTextures to additional cameras
        leftMirrorCamera.targetTexture = leftMirrorTexture;
        rightMirrorCamera.targetTexture = rightMirrorTexture;
        reversingCamera.targetTexture = reversingTexture;
    }

    void InitializeUI()
    {
        // Create Canvas for mini windows
        GameObject canvasObj = new GameObject("MiniCameraCanvas");
        miniCanvas = canvasObj.AddComponent<Canvas>();
        miniCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create a panel to hold all mini cameras
        GameObject panelContainer = new GameObject("MiniCameraPanel");
        panelContainer.transform.parent = canvasObj.transform;
        RectTransform containerRect = panelContainer.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(600, 150); // Adjust size as needed
        containerRect.anchorMin = new Vector2(0.5f, 0);
        containerRect.anchorMax = new Vector2(0.5f, 0);
        containerRect.pivot = new Vector2(0.5f, 0);
        containerRect.anchoredPosition = new Vector2(0, 10);

        // Create Left Mirror Panel
        GameObject leftPanelObj = CreatePanel(panelContainer.transform, "LeftMirrorPanel", new Vector2(200, 150), new Vector2(0, 0.5f), new Vector2(220, 0));
        leftMirrorImage = CreateRawImage(leftPanelObj, leftMirrorTexture);

        // Create Right Mirror Panel
        GameObject rightPanelObj = CreatePanel(panelContainer.transform, "RightMirrorPanel", new Vector2(200, 150), new Vector2(0, 0.5f), new Vector2(0, 0));
        rightMirrorImage = CreateRawImage(rightPanelObj, rightMirrorTexture);

        // Create Reversing Panel
        GameObject reversingPanelObj = CreatePanel(panelContainer.transform, "ReversingPanel", new Vector2(200, 150), new Vector2(0, 0.5f), new Vector2(440, 0));
        reversingImage = CreateRawImage(reversingPanelObj, reversingTexture);

        // Initially disable additional camera panels
        leftPanelObj.SetActive(false);
        rightPanelObj.SetActive(false);
        reversingPanelObj.SetActive(false);
    }

    GameObject CreatePanel(Transform parent, string name, Vector2 sizeDelta, Vector2 anchorMin, Vector2 anchoredPosition)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.parent = parent;
        RectTransform rectTransform = panelObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMin; // Set anchorMax to anchorMin for static positioning
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.5f); // semi-transparent background
        return panelObj;
    }

    RawImage CreateRawImage(GameObject parent, RenderTexture texture)
    {
        GameObject rawImageObj = new GameObject("RawImage");
        rawImageObj.transform.parent = parent.transform;
        RectTransform rawRect = rawImageObj.AddComponent<RectTransform>();
        rawRect.anchorMin = Vector2.zero;
        rawRect.anchorMax = Vector2.one;
        rawRect.offsetMin = Vector2.zero;
        rawRect.offsetMax = Vector2.zero;
        RawImage rawImage = rawImageObj.AddComponent<RawImage>();
        rawImage.texture = texture;
        return rawImage;
    }

    void Update()
    {
        // Handle main camera switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchMainCamera(driverSeatCamera);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchMainCamera(rearViewCamera);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchMainCamera(topViewCamera);
        }

        // Handle additional cameras toggling
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ToggleAdditionalCamera(leftMirrorCamera, leftMirrorImage.transform.parent.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ToggleAdditionalCamera(rightMirrorCamera, rightMirrorImage.transform.parent.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ToggleAdditionalCamera(reversingCamera, reversingImage.transform.parent.gameObject);
        }
    }

    void SwitchMainCamera(Camera newCamera)
    {
        if (newCamera == null)
        {
            Debug.LogWarning("Main Camera is not assigned.");
            return;
        }

        if (driverSeatCamera != newCamera && newCamera != rearViewCamera && newCamera != topViewCamera)
        {
            Debug.LogWarning("Selected camera is not a main camera.");
            return;
        }

        if (currentCamera != null)
        {
            currentCamera.enabled = false;
        }

        newCamera.enabled = true;
        currentCamera = newCamera; // Set currentCamera to newCamera
    }

    void ToggleAdditionalCamera(Camera cam, GameObject panel)
    {
        if (cam == null || panel == null)
        {
            Debug.LogWarning("Camera or Panel is not assigned.");
            return;
        }

        bool isEnabled = cam.enabled;
        cam.enabled = !isEnabled;
        panel.SetActive(!isEnabled);
    }
}
