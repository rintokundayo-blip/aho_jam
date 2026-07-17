using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;  
#endif

public class SceneButton : MonoBehaviour
{
    // インスペクターでシーンファイルを直接ドラッグして指定できる
    public SceneAsset targetScene;

    public void LoadTargetScene()
    {
        // シーンが設定されていない時のエラー防止
        if (targetScene == null)
        {
            Debug.LogError("遷移先シーンが設定されていません");
            return;
        }

        string sceneName = targetScene.name;

        // シーン遷移
        SceneManager.LoadScene(sceneName);
    }

    //EXITボタン
    public void ExitGame()
    {

#if UNITY_EDITOR
        //Unityならプレイを止める
        EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

}
