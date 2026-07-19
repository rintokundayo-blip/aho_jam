using UnityEngine;

public class ResultSEPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip resultSound;

    [Tooltip("音量（0.0〜1.0）。0.5で半減になります")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f; // ← デフォルトで音量を半減（0.5）に設定

    private void Awake()
    {
        // InspectorでAudioSourceが未設定の場合、自身から自動取得を試みる
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        PlayResultSE();
    }

    /// <summary>
    /// リザルトSEを1度だけ再生する
    /// </summary>
    public void PlayResultSE()
    {
        if (audioSource != null && resultSound != null)
        {
            // 第2引数に volume (0.5f) を渡すことで音量を半減させて再生します
            audioSource.PlayOneShot(resultSound, volume);
        }
        else
        {
            if (audioSource == null)
                Debug.LogWarning($"[{gameObject.name}] AudioSource コンポーネントが設定されていません。");
            if (resultSound == null)
                Debug.LogWarning($"[{gameObject.name}] ResultSound (AudioClip) が設定されていません。");
        }
    }
}