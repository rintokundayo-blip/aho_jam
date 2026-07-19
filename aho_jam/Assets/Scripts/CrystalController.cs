using UnityEngine;

public class CrystalController : MonoBehaviour
{
    // 光の粒子
    public ParticleSystem crystalGlow;

    // 擦った瞬間に飛び散る粒子
    public ParticleSystem rubEffect;

    // 光の強さを変えるためのマテリアル
    public Material glowMat;

    // --- 音関連 ---
    [Header("Audio Settings")]
    [SerializeField] private AudioClip rubSound; // 擦り音
    private AudioSource audioSource;             // 再生用
    private float rubSeTimer = 0f;               // 擦り音の連続再生防止
    private const float RubSeInterval = 0.15f;   // 擦り音の間隔

    // 擦った回数（光の強さに使う）
    int rubCount = 0;

    // マウス位置の記録（移動量判定用）
    private Vector2 lastMousePosition;

    // クリスタルの当たり判定（SpriteRenderer の範囲）
    private SpriteRenderer sr;

    // Canvas 上にある場合、正しいカメラを使うため
    private Camera canvasCamera;

    // 擦り続けている判定のバッファ（0.5秒）
    private float rubTimer = 0f;
    private const float RubBufferDuration = 0.5f;

    void Start()
    {
        // SpriteRenderer を取得（クリスタルの当たり判定に使う）
        sr = GetComponent<SpriteRenderer>();

        // マウス位置初期化
        lastMousePosition = Input.mousePosition;

        // AudioSource が無ければ自動追加
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D音

        // Canvas のカメラを取得（UI上にある場合）
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasCamera = canvas.worldCamera;
        }

        // シーン内の粒子を名前で自動取得
        ParticleSystem[] allPS = Resources.FindObjectsOfTypeAll<ParticleSystem>();
        foreach (var ps in allPS)
        {
            if (!ps.gameObject.scene.IsValid()) continue;

            if (ps.name == "CrystalGlow") crystalGlow = ps;
            if (ps.name == "RubEffect") rubEffect = ps;
        }

        // 粒子の初期設定
        if (crystalGlow != null)
        {
            var main = crystalGlow.main;
            main.useUnscaledTime = true; // 時間停止中でも動く

            var emission = crystalGlow.emission;
            emission.enabled = false; // 初期は非表示

            var psr = crystalGlow.GetComponent<ParticleSystemRenderer>();
            if (psr != null)
            {
                psr.sortingLayerName = "Default";
                psr.sortingOrder = 10; // 前面に表示
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
        // マウス移動量（こすっているか判定）
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");

        // Canvas 上なら Canvas のカメラ、なければ MainCamera
        Camera activeCam = canvasCamera != null ? canvasCamera : Camera.main;

        // マウスのワールド座標を取得
        Vector3 worldMousePos = Vector3.zero;
        if (activeCam != null)
        {
            worldMousePos = activeCam.ScreenToWorldPoint(Input.mousePosition);
            worldMousePos.z = transform.position.z;
        }

        // マウスがクリスタルの上にあるか判定
        bool isOverCrystal = false;
        if (sr != null)
        {
            isOverCrystal = sr.bounds.Contains(worldMousePos);
        }

        // マウスが動いているか
        bool isMouseMoving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f;

        // 擦り音のクールタイム
        if (rubSeTimer > 0f)
        {
            rubSeTimer -= Time.unscaledDeltaTime;
        }

        // --- 擦り判定 ---
        if (Time.timeScale > 0f && isOverCrystal && isMouseMoving)
        {
            rubTimer = RubBufferDuration; // 擦り継続バッファ
            rubCount++;                   // 擦り回数加算
            Rub();                        // 光の強さ更新

            // 擦り音再生（連続しすぎないように）
            if (rubSeTimer <= 0f && audioSource != null && rubSound != null)
            {
                audioSource.PlayOneShot(rubSound);
                rubSeTimer = RubSeInterval;
            }
        }

        // --- 擦り中の粒子制御 ---
        if (rubTimer > 0f)
        {
            rubTimer -= Time.unscaledDeltaTime;

            // 擦り粒子
            if (rubEffect != null)
            {
                rubEffect.transform.position = transform.position;

                if (!rubEffect.gameObject.activeSelf)
                    rubEffect.gameObject.SetActive(true);

                var emission = rubEffect.emission;
                emission.enabled = true;

                if (!rubEffect.isPlaying)
                    rubEffect.Play();
            }

            // 光粒子
            if (crystalGlow != null)
            {
                crystalGlow.transform.position = transform.position;

                if (!crystalGlow.gameObject.activeSelf)
                    crystalGlow.gameObject.SetActive(true);

                var emission = crystalGlow.emission;
                emission.enabled = true;

                if (!crystalGlow.isPlaying)
                    crystalGlow.Play();
            }
        }
        else
        {
            // 擦っていない時は粒子を止める
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

        // --- ゲーム停止中でも粒子を動かす ---
        if (Time.timeScale == 0)
        {
            float dt = Time.unscaledDeltaTime;

            if (crystalGlow != null && crystalGlow.gameObject.activeSelf)
                crystalGlow.Simulate(dt, true, false, false);

            if (rubEffect != null && rubEffect.gameObject.activeSelf)
                rubEffect.Simulate(dt, true, false, false);
        }
    }

    // 擦った時の光の強さ・色変化処理
    void Rub()
    {
        // 擦り回数に応じて色を白→金色へ変化
        float t = Mathf.Clamp01(rubCount / 1000f);
        Color targetColor = Color.Lerp(Color.white, new Color(1f, 0.9f, 0.2f), t);

        // クリスタル本体の色変更
        if (sr != null)
        {
            sr.color = targetColor;
        }

        // 光の強さを徐々に強くする
        float intensity = Mathf.Min(1f + rubCount * 0.01f, 2.5f);
        Color baseColor = targetColor * intensity;

        // マテリアルの発光色を更新
        if (glowMat != null)
        {
            glowMat.SetColor("_Color", baseColor);
            glowMat.SetColor("_BaseColor", baseColor);
        }
    }
}
