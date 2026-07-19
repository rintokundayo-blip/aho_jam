using UnityEngine;

public class CrystalController : MonoBehaviour
{
    public ParticleSystem crystalGlow;
    public ParticleSystem rubEffect;
    public Material glowMat;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip rubSound;
    private AudioSource audioSource;
    private float rubSeTimer = 0f;
    private const float RubSeInterval = 0.15f;

    int rubCount = 0;
    private Vector2 lastMousePosition;
    private SpriteRenderer sr;
    private Camera canvasCamera;

    private float rubTimer = 0f;
    private const float RubBufferDuration = 0.5f;

    void Awake()
    {
        // VSync（垂直同期）を無効化（targetFrameRate を有効にするため）
        QualitySettings.vSyncCount = 0;

        // ターゲットフレームレートを 60 FPS に固定
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        rubCount = 0;
        PlayerPrefs.SetInt("RubCount", 0);

        sr = GetComponent<SpriteRenderer>();
        lastMousePosition = Input.mousePosition;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasCamera = canvas.worldCamera;
        }

        ParticleSystem[] allPS = Resources.FindObjectsOfTypeAll<ParticleSystem>();
        foreach (var ps in allPS)
        {
            if (!ps.gameObject.scene.IsValid()) continue;

            if (ps.name == "CrystalGlow") crystalGlow = ps;
            if (ps.name == "RubEffect") rubEffect = ps;
        }

        if (crystalGlow != null)
        {
            var main = crystalGlow.main;
            main.useUnscaledTime = true;

            var emission = crystalGlow.emission;
            emission.enabled = false;

            var psr = crystalGlow.GetComponent<ParticleSystemRenderer>();
            if (psr != null)
            {
                psr.sortingLayerName = "Default";
                psr.sortingOrder = 10;
            }
        }
        if (rubEffect != null)
        {
            var main = rubEffect.main;
            main.useUnscaledTime = true;

            var emission = rubEffect.emission;
            emission.enabled = false;

            var psr = rubEffect.GetComponent<ParticleSystemRenderer>();
            if (psr != null)
            {
                psr.sortingLayerName = "Default";
                psr.sortingOrder = 10;
            }
        }
    }

    void Update()
    {
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");

        Camera activeCam = canvasCamera != null ? canvasCamera : Camera.main;

        Vector3 worldMousePos = Vector3.zero;
        if (activeCam != null)
        {
            worldMousePos = activeCam.ScreenToWorldPoint(Input.mousePosition);
            worldMousePos.z = transform.position.z;
        }

        bool isOverCrystal = false;
        if (sr != null)
        {
            isOverCrystal = sr.bounds.Contains(worldMousePos);
        }

        bool isMouseMoving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f;

        if (rubSeTimer > 0f)
        {
            rubSeTimer -= Time.unscaledDeltaTime;
        }

        if (Time.timeScale > 0f && isOverCrystal && isMouseMoving)
        {
            rubTimer = RubBufferDuration;
            rubCount++;
            Rub();

            if (rubSeTimer <= 0f && audioSource != null && rubSound != null)
            {
                audioSource.PlayOneShot(rubSound);
                rubSeTimer = RubSeInterval;
            }
        }

        if (rubTimer > 0f)
        {
            rubTimer -= Time.unscaledDeltaTime;

            if (rubEffect != null)
            {
                rubEffect.transform.position = transform.position;
                if (!rubEffect.gameObject.activeSelf)
                {
                    rubEffect.gameObject.SetActive(true);
                }
                var emission = rubEffect.emission;
                emission.enabled = true;
                if (!rubEffect.isPlaying)
                {
                    rubEffect.Play();
                }
            }

            if (crystalGlow != null)
            {
                crystalGlow.transform.position = transform.position;
                if (!crystalGlow.gameObject.activeSelf)
                {
                    crystalGlow.gameObject.SetActive(true);
                }
                var emission = crystalGlow.emission;
                emission.enabled = true;
                if (!crystalGlow.isPlaying)
                {
                    crystalGlow.Play();
                }
            }
        }
        else
        {
            if (rubEffect != null)
            {
                var emission = rubEffect.emission;
                emission.enabled = false;
            }
            if (crystalGlow != null)
            {
                var emission = crystalGlow.emission;
                emission.enabled = false;
            }
        }

        if (Time.timeScale == 0)
        {
            float dt = Time.unscaledDeltaTime;
            if (crystalGlow != null && crystalGlow.gameObject.activeSelf)
            {
                crystalGlow.Simulate(dt, true, false, false);
            }
            if (rubEffect != null && rubEffect.gameObject.activeSelf)
            {
                rubEffect.Simulate(dt, true, false, false);
            }
        }
    }

    void Rub()
    {
        float t = Mathf.Clamp01(rubCount / 1000f);
        Color targetColor = Color.Lerp(Color.white, new Color(1f, 0.9f, 0.2f), t);

        if (sr != null)
        {
            sr.color = targetColor;
        }

        float intensity = Mathf.Min(1f + rubCount * 0.01f, 2.5f);
        Color baseColor = targetColor * intensity;
        if (glowMat != null)
        {
            glowMat.SetColor("_Color", baseColor);
            glowMat.SetColor("_BaseColor", baseColor);
        }

        PlayerPrefs.SetInt("RubCount", rubCount);
    }
}