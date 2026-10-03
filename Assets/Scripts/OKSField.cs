using UnityEngine;

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

    private float phase;

    void Start()
    {
        SpawnDots();
    }

    [ContextMenu("Respawn Dots")]
    void SpawnDots()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        Random.InitState(seed);
        float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));

        for (int i = 0; i < dotCount; i++)
        {
            Vector3 dir;
            if (randomScatter)
            {
                dir = Random.onUnitSphere;
            }
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

        phase += 2f * Mathf.PI * frequencyHz * Time.deltaTime;
        float angle = amplitudeDeg * Mathf.Sin(phase);
        float sign = reverseDirection ? -1f : 1f;
        transform.localRotation = Quaternion.AngleAxis(sign * angle, Vector3.forward);
    }

    void HandleKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) frequencyHz = 0.02f;   // OKS mean corner
        if (Input.GetKeyDown(KeyCode.Alpha2)) frequencyHz = 0.04f;   // GVS mean corner
        if (Input.GetKeyDown(KeyCode.Alpha3)) frequencyHz = 0.1f;
        if (Input.GetKeyDown(KeyCode.Alpha4)) frequencyHz = 0.25f;
        if (Input.GetKeyDown(KeyCode.Alpha5)) frequencyHz = 0.5f;
        if (Input.GetKeyDown(KeyCode.Tab)) showPanel = !showPanel;
    }

    string Note()
    {
        if (frequencyHz <= 0.02f) return "At or below OKS mean corner.";
        if (frequencyHz <= 0.04f) return "Between OKS and GVS mean corners.";
        return "Above both mean corners, in the roll-off region.";
    }

    void OnGUI()
    {
        if (showCrosshair) DrawCrosshair();

        Rect panel = new Rect(10, 10, 380, 230);
        Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        PointerOverPanel = showPanel && (panel.Contains(mouse) || GUIUtility.hotControl != 0);
        if (!showPanel) return;

        GUI.skin.label.wordWrap = true;
        GUILayout.BeginArea(panel, GUI.skin.box);

        GUILayout.Label($"Frequency  {frequencyHz:0.000} Hz");
        float logF = Mathf.Log10(frequencyHz);
        float newLog = GUILayout.HorizontalSlider(logF, -2f, Mathf.Log10(0.5f));
        if (!Mathf.Approximately(newLog, logF)) frequencyHz = Mathf.Pow(10f, newLog);

        GUILayout.Label($"Amplitude  {amplitudeDeg:0}°");
        amplitudeDeg = GUILayout.HorizontalSlider(amplitudeDeg, 2f, 50f);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OKS corner")) frequencyHz = 0.02f;
        if (GUILayout.Button("GVS corner")) frequencyHz = 0.04f;
        if (GUILayout.Button("0.1 Hz")) frequencyHz = 0.1f;
        if (GUILayout.Button("0.25 Hz")) frequencyHz = 0.25f;
        if (GUILayout.Button("0.5 Hz")) frequencyHz = 0.5f;
        GUILayout.EndHorizontal();

        GUILayout.Label($"At {frequencyHz:0.000} Hz, one cycle takes {1f / frequencyHz:0.00} s. {Note()}");
        GUILayout.Label("Keys: 1-5 presets, Tab hides panel, R resets view");

        GUILayout.EndArea();
    }

    void DrawCrosshair()
    {
        float cx = Screen.width * 0.5f;
        float cy = Screen.height * 0.5f;
        float len = Screen.height * 0.075f;           // matches the simulator's proportions
        float th = Mathf.Max(1f, Screen.height / 540f);

        Color prev = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.12f);
        GUI.DrawTexture(new Rect(cx - len, cy - th * 0.5f, 2f * len, th), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(cx - th * 0.5f, cy - len, th, 2f * len), Texture2D.whiteTexture);
        GUI.color = prev;
    }
}