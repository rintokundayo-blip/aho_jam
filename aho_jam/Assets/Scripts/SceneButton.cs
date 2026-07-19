using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneButton : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("遷移先のシーン名（Inspectorで直接入力も可能です）")]
    [SerializeField] private string targetSceneName;

#if UNITY_EDITOR
    [Tooltip("ドラッグ＆ドロップ用（設定すると自動で上の Scene Name に反映されます）")]
    [SerializeField] private SceneAsset targetScene;

    // Inspectorで SceneAsset を変更した瞬間に自動で targetSceneName を更新・保存する
    private void OnValidate()
    {
        if (targetScene != null)
        {
            targetSceneName = targetScene.name;
        }
    }
#endif

    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    // シーン移動しないボタン（SE再生のみ）
    public void PlaySEOnly()
    {
        PlaySound();
    }

    // シーン移動するボタン
    public void LoadTargetScene()
    {
        PlaySound();
        StartCoroutine(LoadSceneCoroutine());
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private IEnumerator LoadSceneCoroutine()
    {
        // SEがある場合は再生終了まで待つ（Time.timeScale=0 でも止まらないよう Realtime を使用）
        if (clickSound != null)
        {
            yield return new WaitForSecondsRealtime(clickSound.length);
        }

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] シーン名が空です。Inspectorで Scene または Target Scene Name を設定してください。");
        }
    }

    // EXITボタン
    public void ExitGame()
    {
        PlaySound();
        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator QuitGameCoroutine()
    {
        if (clickSound != null)
        {
            yield return new WaitForSecondsRealtime(clickSound.length);
        }

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}