using TMPro;
using UnityEngine;

public class golden_reward_text : MonoBehaviour{
    TextMeshProUGUI tmp;
    RectTransform rect_transform;
    float elapsed_time = 0f;
    Color start_color;
    bool initialized = false;

    void Awake(){
        tmp = GetComponent<TextMeshProUGUI>();
        rect_transform = transform as RectTransform;
        if (tmp != null)
            start_color = tmp.color;
    }

    public void Initialize(){
        if (tmp == null)
            tmp = GetComponent<TextMeshProUGUI>();
        if (rect_transform == null)
            rect_transform = transform as RectTransform;
        if (tmp != null)
            start_color = tmp.color;
        initialized = true;
    }

    void Update(){
        if (!initialized)
            Initialize();
        if (tmp == null || rect_transform == null)
            return;

        elapsed_time += Time.deltaTime;
        rect_transform.anchoredPosition += Vector2.up * consts.golden_text_move_speed * Time.deltaTime;

        float t = Mathf.Clamp01(elapsed_time / consts.golden_text_lifetime);
        Color clr = start_color;
        clr.a = Mathf.Lerp(start_color.a, 0f, t);
        tmp.color = clr;

        if (elapsed_time >= consts.golden_text_lifetime)
            UnityEngine.Object.Destroy(gameObject);
    }
}
