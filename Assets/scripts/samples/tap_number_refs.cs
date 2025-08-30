using UnityEngine;
using TMPro;

public class tap_number_refs: MonoBehaviour{
    public TextMeshProUGUI 
        txt;
    public RectTransform 
        rt;

    public float 
        speed,
        elapsedTime = 0f;

    public void Start(){
        speed = consts.speed_tap_number;
        rt.anchoredPosition = new Vector2(
            UnityEngine.Random.Range(consts.x1_nmbr_tap_crd, consts.x2_nmbr_tap_crd), 
            UnityEngine.Random.Range(consts.y1_nmbr_tap_crd, consts.y2_nmbr_tap_crd)
        );
    }

    public void Update(){
        transform.position += new Vector3(0, speed) * Time.deltaTime;
        speed -= speed/consts.duration_tap_number * Time.deltaTime;
        if (elapsedTime < consts.duration_tap_number){
            elapsedTime += Time.deltaTime;
            Color temp1 = txt.color;
            temp1.a = 1 - elapsedTime/consts.duration_tap_number;
            txt.color = temp1;
        } else {
            Destroy(gameObject);
        }
    }
}
