using UnityEngine;

public class bttn_w_shine : MonoBehaviour{
    public Animator 
        anmtr_shine;
    void Awake(){
        var bcc = GetComponent<button_custom_class>();

        bcc.on_enter.AddListener(
            () => {
                anmtr_shine.SetFloat("speed", 1);
                anmtr_shine.Play("shine", 0, 
                    Mathf.Clamp01(anmtr_shine.GetCurrentAnimatorStateInfo(0).normalizedTime)
                );
            }
        );
        bcc.on_exit.AddListener(
            () => {
                anmtr_shine.SetFloat("speed", -1);
                anmtr_shine.Play("shine", 0, 
                    Mathf.Clamp01(anmtr_shine.GetCurrentAnimatorStateInfo(0).normalizedTime)
                );
            }
        );
    }
}
