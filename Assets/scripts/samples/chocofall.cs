using UnityEngine;
using UnityEngine.UI;

public class chocofall : MonoBehaviour{
    public Image
        im;

    public float
        cur_lifetime = 0f;
    
    public RectTransform
        rt;

    void Awake(){
        (transform as RectTransform).anchoredPosition = 
            new(UnityEngine.Random.Range(consts.x1_cf_crd, consts.x2_cf_crd), consts.y_cf_crd);
        (transform as RectTransform).localScale *= 
            (float)UnityEngine.Random.Range(consts.cf_min_size, consts.cf_max_size);
        rt = transform as RectTransform;
    }

    void Update(){
        cur_lifetime += Time.deltaTime;
        rt.anchoredPosition -= new Vector2(0, consts.cf_speed * Time.deltaTime);
        var temp = im.color;
        temp.a = Mathf.Lerp(1f, consts.cf_min_alpha, cur_lifetime/consts.cf_sample_lifetime);
        im.color = temp;
    }
}
