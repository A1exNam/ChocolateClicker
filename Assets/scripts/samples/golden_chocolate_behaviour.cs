using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class golden_chocolate_behaviour : MonoBehaviour, IPointerClickHandler{
    RectTransform rect_transform;
    RectTransform parent_rect;
    Image image;
    float destroy_x;
    float move_speed;
    float rotation_speed;
    bool collected = false;

    void Awake(){
        rect_transform = transform as RectTransform;
        image = GetComponent<Image>();
        parent_rect = transform.parent as RectTransform;
    }

    public void Initialize(RectTransform parentRect, float destroyX, float moveSpeed, float rotationSpeed){
        if (rect_transform == null)
            rect_transform = transform as RectTransform;
        parent_rect = parentRect;
        destroy_x = destroyX;
        move_speed = moveSpeed;
        rotation_speed = rotationSpeed;
    }

    void Update(){
        if (rect_transform == null || collected)
            return;

        rect_transform.anchoredPosition += new Vector2(move_speed * Time.deltaTime, 0f);
        rect_transform.Rotate(0f, 0f, rotation_speed * Time.deltaTime);

        if (rect_transform.anchoredPosition.x >= destroy_x)
            UnityEngine.Object.Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData){
        collect();
    }

    void collect(){
        if (collected)
            return;
        collected = true;

        float reward = statics.mngr_upgrs.gps * consts.golden_reward_gps_fraction;
        reward = Mathf.Max(reward, consts.golden_reward_minimum);
        reward = (float)Math.Truncate(reward);
        if (reward < 1f)
            reward = 1f;

        statics.mngr_balance.amount += reward;
        statics.mngr_balance.on_val_change();
        statics.mngr_quests.recalc("quest_collect_1");

        spawn_effects(reward);
        urefs.sound_asrc_as.PlayOneShot(consts.gold_chocolate_ac);
        UnityEngine.Object.Destroy(gameObject);
    }

    void spawn_effects(float reward){
        if (rect_transform == null)
            return;

        if (parent_rect == null)
            parent_rect = transform.parent as RectTransform;
        if (parent_rect == null)
            return;

        Vector2 position = rect_transform.anchoredPosition;
        Sprite sprite = image != null ? image.sprite : null;

        for (int i = 0; i < consts.golden_particle_count; i++){
            GameObject particle_go = new GameObject("golden_chocolate_particle", typeof(RectTransform), typeof(Image));
            RectTransform particle_rect = particle_go.GetComponent<RectTransform>();
            particle_rect.SetParent(parent_rect, false);
            particle_rect.anchoredPosition = position;
            particle_rect.SetAsLastSibling();
            particle_rect.sizeDelta = Vector2.one * consts.golden_particle_size;
            particle_rect.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));

            Image particle_image = particle_go.GetComponent<Image>();
            particle_image.raycastTarget = false;
            particle_image.preserveAspect = true;
            if (sprite != null)
                particle_image.sprite = sprite;
            particle_image.color = consts.golden_particle_color;

            float scale = UnityEngine.Random.Range(consts.golden_particle_min_scale, consts.golden_particle_max_scale);
            particle_rect.localScale = new Vector3(scale, scale, 1f);

            float horizontal = UnityEngine.Random.Range(-consts.golden_particle_horizontal_speed, consts.golden_particle_horizontal_speed);
            Vector2 velocity = new Vector2(horizontal, -consts.golden_particle_speed);

            golden_chocolate_particle particle = particle_go.AddComponent<golden_chocolate_particle>();
            particle.Initialize(velocity);
        }

        GameObject text_go = new GameObject("golden_chocolate_text", typeof(RectTransform));
        RectTransform text_rect = text_go.GetComponent<RectTransform>();
        text_rect.SetParent(parent_rect, false);
        text_rect.anchoredPosition = position;
        text_rect.SetAsLastSibling();

        TextMeshProUGUI tmp = text_go.AddComponent<TextMeshProUGUI>();
        tmp.text = "+" + common_utils.f2s((float)Math.Truncate(reward));
        tmp.color = consts.golden_text_color;
        tmp.fontSize = consts.golden_text_font_size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.raycastTarget = false;
        if (consts.kanit_semibold_font != null)
            tmp.font = consts.kanit_semibold_font;

        ContentSizeFitter content_size_fitter = text_go.AddComponent<ContentSizeFitter>();
        content_size_fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        content_size_fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        golden_reward_text reward_text = text_go.AddComponent<golden_reward_text>();
        reward_text.Initialize();
    }
}
