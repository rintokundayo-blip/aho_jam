using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    // カウントダウン表示用の TextMeshPro
    [SerializeField] private TMP_Text countdownText;

    // ゲームの制限時間（秒）
    private float gameTime = 7f;

    [Header("End Game Settings")]
    // ゲーム終了時に出すエフェクト（光の柱など）
    [SerializeField] private GameObject finishEffect;

    // 終了時に鳴らすホイッスル音
    [SerializeField] private AudioClip endWhistle;

    // リザルト画面のシーン名
    [SerializeField] private string resultSceneName = "ResultScene";

    // SE 再生用の AudioSource
    private AudioSource audioSource;

    void Awake()
    {
        // countdownText が未設定なら自動で探す
        if (countdownText == null) countdownText = GetComponent<TMP_Text>();
        if (countdownText == null) countdownText = GetComponentInChildren<TMP_Text>();

        // 名前で探す（Counter / countdownText）
        if (countdownText == null)
        {
            TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (var t in allTexts)
            {
                if (t.gameObject.scene.IsValid() &&
                    (t.name == "Counter" || t.name == "countdownText"))
                {
                    countdownText = t;
                    break;
                }
            }
        }

        // 起動時は非表示にしておく
        if (countdownText != null)
        {
            countdownText.alpha = 0f;
        }

        // AudioSource が無ければ自動追加
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // SE 用の設定（2D）
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        // FinishEffect が未設定ならシーン内から探す
        if (finishEffect == null)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in allObjects)
            {
                if (go.scene.IsValid() && go.name.Trim() == "FinishEffect")
                {
                    finishEffect = go;
                    break;
                }
            }
        }
    }

    void Start()
    {
        // Text が見つからなければエラー
        if (countdownText == null)
        {
            Debug.LogError("CountdownTimer: TMP_Text object not found!");
            return;
        }
    }

    // GameManager から呼び出す開始関数
    public void StartCountdownFromManager()
    {
        StartCoroutine(StartCountdown());
    }

    // カウントダウン本体
    IEnumerator StartCountdown()
    {
        // 表示開始
        countdownText.alpha = 1f;

        // 3 → 2 → 1 のカウントダウン
        countdownText.fontSize = 120f;
        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        // ゲーム開始
        Time.timeScale = 1f;

        countdownText.text = "go!";
        countdownText.alpha = 1f;

        // GO! をフェードアウト
        StartCoroutine(FadeOutGO());
        yield return new WaitForSeconds(1f);

        // 制限時間カウントダウン
        float timeLeft = gameTime;

        while (timeLeft > 0)
        {
            countdownText.fontSize = 120f;
            countdownText.text = timeLeft.ToString("F0");
            countdownText.alpha = 1f;

            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        // --- ゲーム終了処理 ---
        countdownText.fontSize = 60f;
        countdownText.text = "time up!";

        // ゲーム停止
        Time.timeScale = 0f;

        // ホイッスル音
        if (audioSource != null && endWhistle != null)
        {
            audioSource.PlayOneShot(endWhistle, 0.5f);

        }

        // 終了エフェクトの生成 or 有効化
        GameObject effectInstance = null;

        if (finishEffect != null)
        {
            // Prefab なら Instantiate、シーン内なら SetActive
            if (finishEffect.scene.name == null)
            {
                effectInstance = Instantiate(finishEffect, Vector3.zero, Quaternion.identity);
            }
            else
            {
                effectInstance = finishEffect;
                effectInstance.SetActive(true);
            }

            // 水晶の位置に移動
            GameObject crystal = GameObject.Find("Crystal_0");
            if (crystal != null)
            {
                effectInstance.transform.position = crystal.transform.position;
            }

            // TimeScale = 0 でも動くように設定
            ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem ps in particleSystems)
            {
                var emission = ps.emission;
                emission.enabled = true;

                var main = ps.main;
                main.useUnscaledTime = true;

                if (!ps.isPlaying)
                {
                    ps.Play();
                }
            }

            // レンダー順を最前面に
            Renderer[] renderers = effectInstance.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                r.sortingOrder = 30;
            }

            // Canvas があれば最前面に
            Canvas[] canvases = effectInstance.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas c in canvases)
            {
                c.overrideSorting = true;
                c.sortingOrder = 30;
            }
        }

        // 点滅演出
        float elapsed = 0f;
        float flashInterval = 0.08f;
        bool isVisible = true;

        while (elapsed < 2.5f)
        {
            yield return new WaitForSecondsRealtime(flashInterval);
            elapsed += flashInterval;

            isVisible = !isVisible;

            // エフェクト点滅
            if (effectInstance != null)
            {
                effectInstance.SetActive(isVisible);
            }

            // テキスト点滅
            countdownText.alpha = isVisible ? 1f : 0.2f;
        }

        // 最終状態
        if (effectInstance != null) effectInstance.SetActive(true);
        countdownText.alpha = 1f;

        // リザルトシーンへ移動
        SceneManager.LoadScene(resultSceneName);
    }

    // GO!のフェードアウト処理
    IEnumerator FadeOutGO()
    {
        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            countdownText.alpha = 1f - (t / duration);
            yield return null;
        }

        countdownText.alpha = 0f;
    }
}
