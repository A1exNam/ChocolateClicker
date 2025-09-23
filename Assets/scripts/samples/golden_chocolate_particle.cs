using UnityEngine;
using UnityEngine.UI;

public class golden_chocolate_particle : MonoBehaviour{
    RectTransform rect_transform;
    Image image;
    Vector2 velocity;
    float rotation_speed;
    float elapsed_time = 0f;

    void Awake(){
        rect_transform = transform as RectTransform;
        image = GetComponent<Image>();
    }

    public void Initialize(Vector2 velocityValue){
        velocity = velocityValue;
        if (rect_transform == null)
            rect_transform = transform as RectTransform;
        if (image == null)
            image = GetComponent<Image>();
        rotation_speed = UnityEngine.Random.Range(-consts.golden_chocolate_rotation_speed, consts.golden_chocolate_rotation_speed);
    }

    void Update(){
        if (rect_transform == null || image == null)
            return;

        elapsed_time += Time.deltaTime;
        rect_transform.anchoredPosition += velocity * Time.deltaTime;
        rect_transform.Rotate(0f, 0f, rotation_speed * Time.deltaTime);

        float t = Mathf.Clamp01(elapsed_time / consts.golden_particle_lifetime);
        Color clr = image.color;
        clr.a = Mathf.Lerp(1f, 0f, t);
        image.color = clr;

        if (elapsed_time >= consts.golden_particle_lifetime)
            UnityEngine.Object.Destroy(gameObject);
    }
}
