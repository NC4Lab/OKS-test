using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OKSField : MonoBehaviour
{
    // True while the mouse is over the control panel, so MouseLook can ignore drags there
    public static bool PointerOverPanel;

    [Header("Dots")]
    public GameObject dotPrefab;
    public int dotCount = 2000;           // roughly 230 end up in view at 60° vertical FOV
    public float domeRadius = 10f;
    public float dotSizeMin = 0.1f;       // ~0.55° at radius 10
    public float dotSizeMax = 0.2f;       // ~1.1°
    public bool randomScatter = true;     // false = even Fibonacci spacing
    public int seed = 12345;              // same seed = same dot field every run

    [Header("Motion")]
    [Range(0.01f, 0.5f)] public float frequencyHz = 0.02f;
    [Range(2f, 50f)] public float amplitudeDeg = 30f;
    public bool reverseDirection = false;

    [Header("Display")]
    public bool showPanel = true;
    public bool showCrosshair = true;

    [Header("UI References")]
    public Slider frequencySlider;
    public TextMeshProUGUI frequencyLabel;
    public Slider amplitudeSlider;
    public TextMeshProUGUI amplitudeLabel;
    public TextMeshProUGUI cycleInfo;
    public TextMeshProUGUI legend;
    public Button oksButton;
    public Button gvsButton;
    public Button button01;
    public Button button02;
    public Button button05;
    public Image controlPanel;
    public GameObject crosshair;

    private float phase;
    private CanvasGroup panelCanvasGroup;

    void Start()
    {
        SpawnDots();
        InitializeUI();
        UpdateUILabels();
    }

    void InitializeUI()
    {
        // Set up slider ranges for log-scale frequency
        frequencySlider.minValue = Mathf.Log10(0.01f);
        frequencySlider.maxValue = Mathf.Log10(0.5f);
        frequencySlider.value = Mathf.Log10(frequencyHz);
        frequencySlider.onValueChanged.AddListener(OnFrequencySliderChanged);

        // Set up amplitude slider
        amplitudeSlider.minValue = 2f;
        amplitudeSlider.maxValue = 50f;
        amplitudeSlider.value = amplitudeDeg;
        amplitudeSlider.onValueChanged.AddListener(OnAmplitudeSliderChanged);

        // Connect preset buttons
        oksButton.onClick.AddListener(() => SetPreset(0.02f));
        gvsButton.onClick.AddListener(() => SetPreset(0.04f));
        button01.onClick.AddListener(() => SetPreset(0.1f));
        button02.onClick.AddListener(() => SetPreset(0.20f));
        button05.onClick.AddListener(() => SetPreset(0.5f));

        // Get CanvasGroup for hide/show effect
        panelCanvasGroup = controlPanel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null) panelCanvasGroup = controlPanel.gameObject.AddComponent<CanvasGroup>();

        // Initialize crosshair visibility
        if (crosshair != null)
            crosshair.SetActive(showCrosshair);

        // Set initial legend text
        legend.text = "Keys: 1-5 presets, Tab hides panel, R resets view";
    }

    void OnFrequencySliderChanged(float logValue)
    {
        frequencyHz = Mathf.Pow(10f, logValue);
        UpdateUILabels();
    }

    void OnAmplitudeSliderChanged(float value)
    {
        amplitudeDeg = value;
        UpdateUILabels();
    }

    void UpdateUILabels()
    {
        frequencyLabel.text = $"Frequency  {frequencyHz:0.000} Hz";
        amplitudeLabel.text = $"Amplitude  {amplitudeDeg:0}°";
        cycleInfo.text = $"At {frequencyHz:0.000} Hz, one cycle takes {1f / frequencyHz:0.00} s. {Note()}";
    }

    public void SetPreset(float hz)
    {
        frequencyHz = hz;
        frequencySlider.value = Mathf.Log10(hz);
        UpdateUILabels();
    }

    [ContextMenu("Respawn Dots")]
    void SpawnDots()
    {
        // Clear all existing dots
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        Random.InitState(seed);
        // Distribute points on a sphere around the player using the golden angle (spherical Fibonacci lattice).
        float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));

        for (int i = 0; i < dotCount; i++)
        {
            Vector3 dir;
            // Default: Randomly scattered points on a sphere as in other OKS simulators.
            if (randomScatter)
            {
                dir = Random.onUnitSphere;
            }
            // Option for evenly spaced points on a sphere using the golden angle
            else
            {
                float y = 1f - 2f * i / (dotCount - 1f);
                float r = Mathf.Sqrt(1f - y * y);
                float theta = i * goldenAngle;
                dir = new Vector3(r * Mathf.Cos(theta), y, r * Mathf.Sin(theta));
            }

            GameObject dot = Instantiate(dotPrefab, transform);
            dot.transform.localPosition = dir * domeRadius;
            dot.transform.localScale = Vector3.one * Random.Range(dotSizeMin, dotSizeMax);
        }
    }

    void Update()
    {
        HandleKeys();

        // Detect if pointer is over UI panel
        PointerOverPanel = showPanel && (EventSystem.current.IsPointerOverGameObject() || GUIUtility.hotControl != 0);

        phase += 2f * Mathf.PI * frequencyHz * Time.deltaTime;
        float angle = amplitudeDeg * Mathf.Sin(phase);
        float sign = reverseDirection ? -1f : 1f;
        transform.localRotation = Quaternion.AngleAxis(sign * angle, Vector3.forward);
    }

    void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetPreset(0.02f);   // OKS mean corner
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetPreset(0.04f);   // GVS mean corner
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetPreset(0.1f);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetPreset(0.20f);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetPreset(0.5f);
        if (Input.GetKeyDown(KeyCode.Tab)) TogglePanel();
    }

    void TogglePanel()
    {
        showPanel = !showPanel;
        panelCanvasGroup.alpha = showPanel ? 1f : 0f;
        panelCanvasGroup.blocksRaycasts = showPanel;
    }

    string Note()
    {
        if (frequencyHz <= 0.02f) return "At or below OKS mean corner.";
        if (frequencyHz <= 0.04f) return "Between OKS and GVS mean corners.";
        return "Above both mean corners, in the roll-off region.";
    }
}