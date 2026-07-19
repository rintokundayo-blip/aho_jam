using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; 
#endif

public class SceneButton : MonoBehaviour
{
    
    //  シーン設定（SceneAsset → シーン名）
    [Header("Scene Settings")]
    [Tooltip("遷移先のシーン名（SceneAssetを設定すると自動で更新されます）")]
    [SerializeField] private string targetSceneName; // 実際にロードするシーン名

#if UNITY_EDITOR
    [Tooltip("ドラッグ＆ドロップ用のシーンファイル（Editor専用）")]
    [SerializeField] private SceneAsset targetScene;

    /// <summary>
    /// Inspector 上で値が変更された時に呼ばれる。
    /// SceneAsset が設定されたら、その名前を targetSceneName に自動反映する。
    /// </summary>
    private void OnValidate()
    {
        if (targetScene != null)
        {
            // SceneAsset のファイル名（拡張子なし）をシーン名として保存
            targetSceneName = targetScene.name;
        }
    }
#endif

    //  サウンド設定
    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource; // SE 再生用 AudioSource
    [SerializeField] private AudioClip clickSound;    // ボタン押下時の SE

    // SE 再生のみ行うボタン
    public void PlaySEOnly()
    {
        PlaySound();
    }

    // SE 再生後にシーン遷移するボタン
    public void LoadTargetScene()
    {
        PlaySound();
        StartCoroutine(LoadSceneCoroutine());
    }

    // SE 再生処理（AudioSource と AudioClip が揃っている場合のみ）
    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    /// <summary>
    /// SE 再生終了を待ってからシーン遷移するコルーチン
    /// Time.timeScale = 0 の状態でも動くように WaitForSecondsRealtime を使用
    /// </summary>
    private IEnumerator LoadSceneCoroutine()
    {
        // SE が設定されていれば、その再生時間だけ待つ
        if (clickSound != null)
        {
            yield return new WaitForSecondsRealtime(clickSound.length);
        }

        // シーン名が設定されていれば遷移
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] シーン名が空です。SceneAsset または Scene Name を設定してください。");
        }
    }

    //  EXIT ボタン
    public void ExitGame()
    {
        PlaySound();
        StartCoroutine(QuitGameCoroutine());
    }

    // SE 再生後にゲーム終了）
    private IEnumerator QuitGameCoroutine()
    {
        if (clickSound != null)
        {
            yield return new WaitForSecondsRealtime(clickSound.length);
        }

#if UNITY_EDITOR
        // Editor 上ではプレイモードを終了
        EditorApplication.isPlaying = false;
#else
        // ビルド後はアプリを終了
        Application.Quit();
#endif
    }
}
