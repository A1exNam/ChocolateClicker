using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class button_custom_class : MonoBehaviour, 
    IPointerClickHandler, 
    IPointerDownHandler, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerUpHandler{

    //mode = 0 - no anim
    //mode = 1 - default, size anim
    //mode = 2 - x
    public int mode = 1; 

    public UnityEvent on_enter, on_exit, on_down, on_up, on_click, on_disable;

    public Sprite x_entered, x_exited;
    public Image im;

    public Animator anmtr;

    void Start(){
        anmtr = GetComponent<Animator>();
        if (mode == 1 || mode == 2){
            if (anmtr == null){
                anmtr = gameObject.AddComponent<Animator>();
                anmtr.runtimeAnimatorController = 
                    Resources.Load<RuntimeAnimatorController>("anim_ctrl/" + "bcc_size");
            }
            on_down.AddListener(
                () => anmtr.Play("make_smaller")
            );
            on_up.AddListener(
                () => anmtr.Play("make_normal_from_smaller")
            );
        }
        if (mode == 2){
            Image im = GetComponent<Image>();
            x_entered = Resources.Load<Sprite>("images/" + "x_entered");
            x_exited = Resources.Load<Sprite>("images/" + "x_exited");
            if (im == null){
                im = gameObject.AddComponent<Image>();
            }

            on_enter.AddListener(
                () => im.sprite = x_entered
            );

            on_exit.AddListener(
                () => im.sprite = x_exited
            );

            on_disable.AddListener(
                () => im.sprite = x_exited
            );

            on_click.AddListener(
                () => {
                    urefs.sound_asrc_as.PlayOneShot(consts.close_win_ac);
                    sdk_common.gp_start();
                }
            );
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData){
        on_enter?.Invoke();
    }

    public void OnPointerExit(PointerEventData pointerEventData){
        on_exit?.Invoke();
    }

    public void OnPointerDown(PointerEventData pointerEventData){
        on_down?.Invoke();
    }

    public void OnPointerUp(PointerEventData pointerEventData){
        on_up?.Invoke();
    }

    public void OnPointerClick(PointerEventData pointerEventData){
        on_click?.Invoke();
    }

    void OnDisable() {
        on_disable?.Invoke();
    }
}
