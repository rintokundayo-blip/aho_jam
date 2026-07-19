using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 説明
    public GameObject explanationPanel;

    // カウントダウンを管理するスクリプト
    [SerializeField] private CountdownTimer countdownTimer;

    [Header("Audio Settings")]
    // ボタンを押した時の音
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    // スタートボタン（Canvas 内の Button）
    private GameObject startButton;

    void Start()
    {
        // AudioSource が未設定なら自動で追加する
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // SE 用の設定（2D）
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        // Canvas 内の「Button」を探す
        startButton = GameObject.Find("Button");

        // 説明パネルがプレハブ参照の場合、シーン内の実体 POP_0 を探す
        if (explanationPanel == null || explanationPanel.scene.name == null)
        {
            explanationPanel = GameObject.Find("POP_0");
        }

        // ゲーム開始前は説明パネルとボタンを表示
        if (explanationPanel != null)
        {
            explanationPanel.SetActive(true);
        }
        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        // ゲームを停止しておく（カウントダウン開始まで動かさない）
        Time.timeScale = 0f;
    }

    // スタートボタンが押された時に呼ばれる
    public void StartGame()
    {
        // クリック音を再生
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // 説明パネルが見つからない場合は再検索
        if (explanationPanel == null || explanationPanel.scene.name == null)
        {
            explanationPanel = GameObject.Find("POP_0");
        }

        // 説明パネルを非表示にする
        if (explanationPanel != null)
        {
            explanationPanel.SetActive(false);
        }

        // スタートボタンも非表示にする
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // カウントダウン開始
        if (countdownTimer != null)
        {
            countdownTimer.StartCountdownFromManager();
        }
    }
}
