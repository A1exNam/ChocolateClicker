using UnityEngine;

public class raindrop : MonoBehaviour{
    public float 
        velocity = 0f,
        cur_lifetime = 0f;
    
    public RectTransform
        rt;

    void Awake(){
        rt.anchoredPosition = 
            new(UnityEngine.Random.Range(consts.x1_rd_crd, consts.x2_rd_crd), consts.y_rd_crd);
    }

    void Update(){
        cur_lifetime += Time.deltaTime;
        rt.anchoredPosition -= new Vector2(0, velocity * Time.deltaTime);
        velocity += consts.rd_acceleration * Time.deltaTime;
    }
}
