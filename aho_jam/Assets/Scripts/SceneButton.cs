using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneButton : MonoBehaviour
{
    // インスペクターでシーン指定
    public SceneAsset targetScene;

    public AudioSource audioSource; // SEを鳴らすAudioSource
    public AudioClip clickSound;    // 再生する音

    // シーン移動しないボタン
    public void PlaySEOnly()
    {
        audioSource.PlayOneShot(clickSound);
    }

    // シーン移動するボタン
    public void LoadTargetScene()
    {
        audioSource.PlayOneShot(clickSound);
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(clickSound.length);
        string sceneName = targetScene.name;

        // シーン遷移
        SceneManager.LoadScene(sceneName);
    }

    //EXITボタン
    public void ExitGame()
    {
        audioSource.PlayOneShot(clickSound);
        StartCoroutine(QuitGameCoroutine());
    }

    IEnumerator QuitGameCoroutine()
    {
        yield return new WaitForSeconds(clickSound.length);
#if UNITY_EDITOR
        //Unityならプレイを止める
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}