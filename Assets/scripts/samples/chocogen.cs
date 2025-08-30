using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class chocogen : MonoBehaviour{
    public Image im;
    public Rigidbody2D rb;

    void Start(){
        im.sprite = consts.gen_on_click_sprites[
            UnityEngine.Random.Range(0, consts.gen_on_click_sprites.Count)
        ];
        float x = UnityEngine.Random.Range(-0.8f, 0.8f);
        float y = (float)Math.Sqrt(1f - (float)Math.Pow(x, 2));
        rb.AddForce(new Vector2(x, y) * consts.gen_impulse_force, ForceMode2D.Impulse);
        rb.angularVelocity = consts.gen_angular_velocity;
        UnityEngine.Object.Destroy(gameObject, consts.gen_lifetime);
    }
    public static void gen(){
        GameObject go = UnityEngine.Object.Instantiate(consts.gen_choco_pf, urefs.genchocozone_tr);
        go.transform.SetAsLastSibling();
    }
}
