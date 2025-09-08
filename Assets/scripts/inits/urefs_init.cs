using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class urefs_init : MonoBehaviour{
    void Awake(){
        common_utils.InitStaticDataClassFromMono(typeof(urefs), this);
    }
    
    public GameObject
        discover_slot_go, 
        discover_proc_go, 
        new_art_image_go, 
        new_art_flash_go,
        orange_cacao_bean_go,
        upgr_shop_go,
        arts_shop_go,
        stats_upgrs_go,
        stats_arts_go,
        prof_window_reset_bt_go,
        prof_window_go,
        temper_window_go, 
        achs_window_go, 
        as_circle_timer_go, 
        as_numbers_timer_go, 
        skins_window_go, 
        ad_timer_go, 
        prof_change_flash_go, 
        prof_window_l_go,
        prof_window_r_go,
        prof_window_cur_go,
        ad_button_go,
        tempering_flash_go,
        win_settings_go,
        open_prof_win_bttn_min_lvl_go,
        open_tmprng_win_bttn_min_lvl_go,
        open_skins_win_bttn_min_lvl_go,
        open_arts_panel_bttn_min_lvl_go,
        prof_reset_win_go,
        ad_bttn_lifetime_all_bar_go,
        tutor_as_go,
        tutor_label_as_go,
        shw_rwrd_tmprng_panel_go,
        offln_rwrd_w_go,
        changelog_w_go,
        gameover_screen_go,
        load_scr_go,
        ad_error_lbl_go,
        offln_rwrd_bttn_xtra_go,
        ad_watching_backgr_go,
        tutor_click_go,
        tutor_tap_go,
        tutor_upgr_go,
        tutor_tempering_win_bttn_go,
        tutor_tempering_go,
        tutor_prof_win_bttn_go,
        tutor_prof_go,
        tutor_skin_win_bttn_go,
        tutor_ach_win_bttn_go,
        tutor_diamonds_go,
        tutor_arts_panel_bttn_go,
        tutor_arts_discover_go,
        quest_reward_go,
        quest_progress_go,
        quest_go,
        as_label_to_trigger_go,
        gameover_ac_screen_go;

    public Transform 
        arts_shop_tr, 
        upgrs_shop_tr, 
        stats_upgrs_text_tr, 
        sun_tr, 
        skins_content_panel_tr,
        discover_slot_tr,
        ach_panel_tr,
        genchocozone_tr;

    public Image 
        new_art_image_im,
        shop_upgrs_bt_im,
        shop_arts_bt_im,
        xp_bar_image_im,
        prof_image_im,
        prof_window_l_im,
        prof_window_l_image_im,
        prof_window_r_im,
        prof_window_r_image_im,
        as_circle_timer_im,
        chocolate_im,
        prof_window_cur_image_im,
        ach_noft_im,
        ad_bttn_lifetime_bar_im;

    public TextMeshProUGUI
        new_art_nm_text_txt,
        discover_price_text_txt,
        sound_text_txt,
        music_text_txt,
        diamonds_text_txt,
        xp1_text_txt, 
        xp2_text_txt, 
        lvl_text_txt,
        tap_val_text_txt, 
        prof_nm_bt_txt, 
        prof_window_lvl_text_txt,
        prof_window_prof_nm_text_txt,
        prof_window_state_text_txt,
        prof_window_l_nm_txt,
        prof_window_l_desc_text_txt,
        prof_window_l_desc_as_text_txt,
        prof_window_l_desc_ps_text_txt,
        prof_window_r_nm_txt,
        prof_window_r_desc_text_txt,
        prof_window_r_desc_as_text_txt,
        prof_window_r_desc_ps_text_txt,
        prof_window_reset_price_text_txt,
        prof_window_reset_class_text_txt,
        balance_text_txt,
        as_numbers_timer_txt,
        stats_upgrs_text_txt,
        stats_arts_text_txt,
        cb_amount_text_txt,
        prof_window_cur_nm_txt,
        prof_window_cur_desc_text_txt,
        prof_window_cur_desc_as_text_txt,
        prof_window_cur_desc_ps_text_txt,
        tempering_cb_cnt_txt,
        ad_timer_txt,
        shop_bt_upgrs_txt,
        shop_bt_arts_txt,
        tempering_reward_bttn_txt,
        open_prof_win_bttn_min_lvl_txt,
        open_tmprng_win_bttn_min_lvl_txt,
        open_skins_win_bttn_min_lvl_txt,
        open_arts_panel_bttn_min_lvl_txt,
        shw_rwrd_tmprng_text_txt,
        offln_rwrd_text_txt,
        offln_rwrd_xtra_rwrd_bttn_txt,
        quest_progress_desc_txt,
        quest_progress_val_txt,
        quest_reward_txt,
        tutor_click_txt,
        tutor_tap_txt,
        tutor_upgr_txt,
        tutor_arts_panel_bttn_txt,
        tutor_arts_discover_txt,
        tutor_tempering_win_bttn_txt,
        tutor_prof_win_bttn_txt,
        tutor_prof_txt,
        tutor_skin_win_bttn_txt,
        tutor_ach_win_bttn_txt,
        tutor_diamonds_txt;

    public AudioSource
        sound_asrc_as,
        sound_loop_asrc_as,
        music_asrc_as;

    public Slider 
        sound_slider_sl,
        music_slider_sl;

    public RectTransform 
        chocolate_rt,
        shop_upgrs_rt,
        shop_arts_rt,
        ad_button_rt,
        cfrd_zone_rt,
        prof_nm_bt_rt,
        diamonds_panel_rt,
        shw_rwrd_tmprng_panel_rt,
        offln_rwrd_rt,
        offln_rwrd_bttn_xtra_rt,
        changelog_w_rt,
        menu_rt,
        quest_progress_bar_rt;

    public ScrollRect
        shop_scrrt_sr;

    public Scrollbar
        shop_scrollbar;

    public button_custom_class
        reset_prof_bt_bcc,
        prof_window_l_bcc,
        prof_window_r_bcc,
        chocolate_bcc,
        as_bt_bcc,
        discover_proc_bcc,
        discover_bt_bcc,
        shop_upgrs_bt_bcc,
        shop_arts_bt_bcc,
        tempering_reward_bttn_bcc,
        ad_button_bcc,
        open_ach_win_bcc,
        open_settings_win_bcc,
        open_temper_win_bcc,
        open_skin_win_bcc,
        open_prof_win_bcc,
        open_prof_reset_win_bcc,
        offln_rwrd_bttn_claim_bcc,
        offln_rwrd_bttn_claim_xtra_bcc,
        open_changelog_w_bcc,
        restart_game_bcc,
        close_ach_win_bcc,
        tutor_click_bcc,
        tutor_tap_bcc,
        tutor_upgr_bcc,
        tutor_tempering_win_bttn_bcc,
        tutor_tempering_bcc,
        tutor_prof_win_bttn_bcc,
        tutor_skin_win_bttn_bcc,
        tutor_ach_win_bttn_bcc,
        tutor_arts_panel_bttn_bcc,
        tutor_arts_discover_bcc;

    public Animator 
        ad_button_anmtr,
        ach_noft_anmtr,
        as_bt_anmtr,
        tempering_flash_anmtr,
        prof_change_flash_anmtr,
        open_prof_win_bttn_anmtr,
        open_skins_win_bttn_anmtr,
        open_tmprng_win_bttn_anmtr,
        open_arts_panel_bttn_anmtr,
        lvl_text_anmtr,
        ad_error_lbl_anmtr,
        coin_get_rwrd_anmtr,
        tutor_click_anmtr,
        tutor_tap_anmtr,
        tutor_upgr_anmtr,
        tutor_arts_panel_bttn_anmtr,
        tutor_arts_discover_anmtr,
        tutor_tempering_win_bttn_anmtr,
        tutor_tempering_anmtr,
        tutor_prof_win_bttn_anmtr,
        tutor_prof_anmtr,
        tutor_skin_win_bttn_anmtr,
        tutor_ach_win_bttn_anmtr,
        tutor_diamonds_anmtr,
        as_label_to_trigger_anmtr;

    public CanvasGroup
        prof_window_reset_bt_cg,
        tempering_reward_bttn_cg,
        open_prof_win_bttn_cg,
        open_tmprng_win_bttn_cg,
        open_skins_win_bttn_cg,
        open_arts_panel_bttn_cg;

    public ContentSizeFitter
        shw_rwrd_tmprng_panel_csf;

    public upgr_refs
        tap_references;
}
