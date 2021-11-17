using UnityEngine;

public class Window : MonoBehaviour
{
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void SetSize(float width, float height)
    {
        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
