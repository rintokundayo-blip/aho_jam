using UnityEngine;
using UnityEngine.UI;

public class HandCursorUI : MonoBehaviour
{
    public RectTransform handImage;   // 手のUIオブジェクト

    private Canvas canvas;            // 親Canvas
    private Camera canvasCamera;      // Canvasが使うカメラ

    void Start()
    {
        // 親からCanvasを取得
        canvas = GetComponentInParent<Canvas>();

        // CanvasからUICameraを取得
        if (canvas != null)
        {
            canvasCamera = canvas.worldCamera;  
        }
    }

    void Update()
    {
        // マウスのスクリーン座標を取得
        Vector2 mousePos = Input.mousePosition;

        // マウス位置が画面外に行かないようにする
        mousePos.x = Mathf.Clamp(mousePos.x, 0f, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0f, Screen.height);

        // 手の親RectTransform
        RectTransform parentRect = handImage.parent as RectTransform;

        if (parentRect != null)
        {
            Vector2 localPos;

            // スクリーン座標 を親RectTransform内のローカル座標へ変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                mousePos,
                canvasCamera,
                out localPos
            );

            // 手の中心と画像の中心がズレている場合の補正
            Vector3 visualOffset = Vector3.zero;
            if (handImage.childCount > 0)
            {
                Transform child = handImage.GetChild(0);
                // 子オブジェクトのローカル位置 × 親のスケールで補正量を計算
                visualOffset = Vector3.Scale(child.localPosition, handImage.localScale);
            }

            // 見た目の手の位置を計算
            Vector2 visualPos = localPos + (Vector2)visualOffset;

            // 親RectTransformの半分の幅・高さ
            float halfWidth = parentRect.rect.width * 0.5f;
            float halfHeight = parentRect.rect.height * 0.5f;

            // 見た目の手が左右上下で対称に動けるようにするための最大値
            float clampX = halfWidth - Mathf.Abs(visualOffset.x);
            float clampY = halfHeight - Mathf.Abs(visualOffset.y);

            // 見た目の手の位置を範囲内に制限
            visualPos.x = Mathf.Clamp(visualPos.x, -clampX, clampX);
            visualPos.y = Mathf.Clamp(visualPos.y, -clampY, clampY);

            // 見た目の中心からオフセットを引いて親の位置に戻す
            handImage.localPosition = visualPos - (Vector2)visualOffset;
        }
    }
}
