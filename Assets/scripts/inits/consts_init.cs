using UnityEngine;
using System.Collections.Generic;

public class consts_init: MonoBehaviour{
    void Awake(){
        common_utils.InitStaticDataClassFromMono(typeof(consts), this);
        common_utils.fill_suffix_num_list();
    }

    void Start(){
        float 
            cf_pf_width_temp = (consts.chocofall_pf.transform as RectTransform).rect.width,
            cf_pf_height_temp = (consts.chocofall_pf.transform as RectTransform).rect.height;
        
        consts.x1_cf_crd = 
            - urefs.cfrd_zone_rt.rect.width/2 + cf_pf_width_temp/2;
        consts.x2_cf_crd =
            + urefs.cfrd_zone_rt.rect.width/2 - cf_pf_width_temp/2;
        consts.y_cf_crd = 
            + urefs.cfrd_zone_rt.rect.height/2 + cf_pf_height_temp/2;

        consts.cf_sample_lifetime = 
            Mathf.Sqrt(2f * (urefs.cfrd_zone_rt.rect.height + cf_pf_height_temp) / consts.cf_acceleration);

        float 
            rd_pf_width_temp = (consts.raindrop_pf.transform as RectTransform).rect.width,
            rd_pf_height_temp = (consts.raindrop_pf.transform as RectTransform).rect.height;
        
        consts.x1_rd_crd = 
            - urefs.cfrd_zone_rt.rect.width/2 + rd_pf_width_temp/2;
        consts.x2_rd_crd =
            + urefs.cfrd_zone_rt.rect.width/2 - rd_pf_width_temp/2;
        consts.y_rd_crd = 
            + urefs.cfrd_zone_rt.rect.height/2 + rd_pf_height_temp/2;

        consts.rd_sample_lifetime = 
            Mathf.Sqrt(2f * (urefs.cfrd_zone_rt.rect.height + rd_pf_height_temp) / consts.cf_acceleration);
        
        consts.x1_nmbr_tap_crd = - urefs.chocolate_rt.rect.width/2;
        consts.x2_nmbr_tap_crd = + urefs.chocolate_rt.rect.width/2;
        consts.y1_nmbr_tap_crd = - urefs.chocolate_rt.rect.height/2;
        consts.y2_nmbr_tap_crd = + urefs.chocolate_rt.rect.height/2;
    }

    public List<Sprite> gen_on_click_sprites;

    public GameObject
        upgr_slot_pf, 
        art_slot_pf, 
        click_num_pf, 
        chocofall_pf,
        ach_slot_pf,
        skin_slot_pf,
        ach_star_pf,
        raindrop_pf,
        gen_choco_pf;

    public AudioClip 
        ach_noft_ac, 
        currency, 
        empty_click_ac,  
        discover_final_ac, 
        discover_waiting_ac,
        lvlup_ac,
        open_u_ac, 
        ach_reward_ac, 
        tempering_ac, 
        close_win_ac, 
        active_skill_ac, 
        change_ac,
        click_ac,
        open_win_ac,
        change_shop_ac,
        diamond_on_tap_ac,
        reveal_bttn_from_lock_ac,
        quest_completion_ac;

    public Color32 
        active_skin_bttn_clr,
        not_active_skin_bttn_clr,
        active_shop_bt_clr,
        not_active_shop_bt_clr,
        prof_entered_clr,
        prof_exited_clr,
        default_tap_clr,
        crit_tap_clr,
        combo_tap_clr,
        active_upgr_clr,
        not_active_upgr_clr;
}