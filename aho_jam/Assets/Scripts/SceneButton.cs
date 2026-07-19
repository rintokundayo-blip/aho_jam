using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private string targetSceneName; // シーン名を直接指定（ビルド対応）

    [SerializeField] private AudioSource audioSource; // SEを鳴らすAudioSource
    [SerializeField] private AudioClip clickSound;    // 再生する音

    // シーン移動しないボタン
    public void PlaySEOnly()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }

    // シーン移動するボタン
    public void LoadTargetScene()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);

        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        if (clickSound)
            yield return new WaitForSeconds(clickSound.length);

        // シーン遷移
        if (!string.IsNullOrEmpty(targetSceneName))
            SceneManager.LoadScene(targetSceneName);
        else
            Debug.LogWarning("Scene name is not set in SceneButton.");
    }

    // EXITボタン
    public void ExitGame()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);

        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator QuitGameCoroutine()
    {
        if (clickSound)
            yield return new WaitForSeconds(clickSound.length);

#if UNITY_EDITOR
        // Unityエディターならプレイを止める
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ビルド後はアプリを終了
        Application.Quit();
#endif
    }
}
