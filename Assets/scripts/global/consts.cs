using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public static class consts{
    public static int 
        //tempering
        min_temper_lvl = 15,
        
        //tap
        st_tap_lvl = 1, 

        //upgrs
        st_upgrs_lvl = 0,
        upgrs_max_lvl = 25,

        //arts
        st_arts_lvl = 1,

        //tutors
        open_skins_win_min_lvl = 10,
        open_tmprng_win_min_lvl = 15,
        open_prof_win_min_lvl = 20,
        open_arts_panel_min_lvl = 15;

    public static float 
        //common
        bttn_min_alpha = 0.8f, //for upgrs, arts buttons only

        //tap number
        x1_nmbr_tap_crd,
        x2_nmbr_tap_crd,
        y1_nmbr_tap_crd,
        y2_nmbr_tap_crd,
        duration_tap_number = 0.7f,
        speed_tap_number = 100f,

        //cf
        x1_cf_crd,
        x2_cf_crd,
        y_cf_crd,
        cf_speed = 1900f,
        cf_min_size = 0.8f,
        cf_max_size = 1f,
        cf_sample_lifetime,
        cf_spawn_interval = 0.09f,
        cf_min_alpha = 0.7f,

        //discover
        discover_price_base = 2f,
        discover_price_coef = 1f,

        //xp
        xp_base = 2.3f, //xp_base^lvl * xp_coef
        xp_coef = 175f,
        xp_gps_fraction = 0.2f,
        lvlup_reward_tap_mult = 15f,
        lvlup_reward_gps_mult = 10f,
        xp_purchase_fraction = 0.1f,

        //tap
        st_tap_price = 80f,
        st_b_tap = 1f,
        st_b_tap_gain = 1f,
        tap_price_coef = 1.8f,
        tap_gain_coef = 1.45f,
        tap_hold_clicks_per_second = 7f,
        b_crit_ch = 0.01f,
        b_crit_m = 2f,
        b_diamond_ch = 0.001f,
        tap_indicator_bonus = 2f,
        tap_indicator_decay_speed = 0.25f,
        tap_indicator_click_gain = 0.07f,
        tap_indicator_bonus_threshold = 0.78f,
        tap_indicator_bonus_shake_strength = 6f,
        tap_indicator_bonus_shake_angle = 5f,
        tap_indicator_bonus_shake_speed = 18f,

        //upgrs
        upgrs_price_coef = 1.8f,
        upgrs_gain_coef = 1.3f,

        //arts
        st_arts_price = 1f,
        arts_price_coef = 1.5f,
        arts_str_coef = 1.1f,
        o_art_damping_power = 3f,

        //tempering
        tempering_base_reward = 1.35f, //cb_base^(lvl-min_temper_lvl)
        shw_rwrd_tmprng_number_change_period = 1f,
        shw_rwrd_tmprng_delay_after = 3f,
        shw_rwrd_tmprng_shrink_period = 0.7f,

        //ad
        ad_m = 5f,
        ad_bonus_duration = 60f, 
        ad_cd = 100f, 
        ad_max_show_interval = 10f, 
        ad_first_delay = 85f, 

        //tutors
        bttn_tutor_min_alpha = 0.5f,
        hand_tutor_speed = 20f,
        hand_tutor_max_dist = 10f,
        diamonds_tutor_interval = 4f,
        typewriter_delay = 0.04f,
        music_val_dec_while_tutor = 0.2f,

        //ach
        ach_noft_shrink_period = 1f,
        ach_noft_stay_period = 1f,

        //music, sound
        dec_music_vol_coef = 0.5f,

        //offline reward
        offline_reward_m = 2f,
        offline_gps_dec_c = 0.0625f,
        min_offline_period_to_get_rwrd = 900f,
        ad_fail_timeout = 1f,
        min_offline_reward = 1000f,

        //gen
        gen_impulse_force = 380f,
        gen_angular_velocity = 100f,
        gen_lifetime = 3f,

        //golden chocolate
        golden_spawn_mean_delay = 180f,
        golden_spawn_pity = 300f,
        golden_spawn_wave_min_delay = 0.1f, //minimum delay between chocolates within a wave
        golden_spawn_wave_max_delay = 0.5f, //maximum delay between chocolates within a wave
        golden_chocolate_move_speed = 600f,
        golden_chocolate_rotation_speed = 100f,
        golden_reward_gps_fraction = 30f, //seconds worth of current chocolate per second
        golden_reward_minimum = 1f,
        golden_text_move_speed = 90f,
        golden_text_lifetime = 1.2f,
        golden_text_font_size = 72f,
        golden_particle_speed = 260f,
        golden_particle_horizontal_speed = 110f,
        golden_particle_lifetime = 1.1f,
        golden_particle_min_scale = 0.25f,
        golden_particle_max_scale = 0.55f,
        golden_particle_size = 80f,

        //quests
        reward_period = 4f,
        //anticlicker
        max_taps_per_sec = 45f;

    public static int
        golden_spawn_wave_count = 3,
        golden_particle_count = 12,

        //audio priorities (lower value means higher priority)
        music_priority = 0,
        sound_priority = 128,
        sound_loop_priority = 140;

    public static Color32
        golden_particle_color = new Color32(255, 222, 128, 255),
        golden_text_color = new Color32(255, 232, 128, 255);

    public static TMP_FontAsset
        kanit_semibold_font;

    public static string
        //prof
        start_prof_nm = "Novice",

        //save-restore
        arts_order_postfix = "_order",
        achs_postfix = "_ach",
        achs_rewarded_cnt_postfix = achs_postfix + "_rwrd_cnt",
        achs_noft_idx_postfix = achs_postfix + "_noft_idx",

        //arts
        first_art = "Bar of Wealth";

    public static List<string> tutor_label_list =
        new(){"prof_lbl_tutor", "tempering_lbl_tutor", "skins_lbl_tutor", "arts_lbl_tutor", "click_lbl_tutor"};

    public static List<(TextMeshProUGUI unlock_txt, GameObject unlock_go, Image bttn_im)> 
        bttn_tutor_list = new();

    public static Dictionary<int, (int lvl, int reset_price)> prof_grade_lvl_mapping = new(){
        {0, (0, 0)}, 
        {1, (20, 30)}, 
        {2, (40, 350)}
    };

    public static List<string> suffixes = new();

    public static Dictionary<string, (int val, string desc, string reward, int idx)> quests_data = new(){
        {"quest_tap_1", (10, "Tap 10 times!", "Your clicks are pure cocoa magic!", 0)},
        {"quest_collect_1", (40, "Collect 40 coins!", "Sweet success!", 1)},
        {"quest_lvl_1", (10, "Reach 10 level!", "Delicious work, keep it up!", 2)},
        {"quest_lvl_2", (15 ,"Reach 15 level!", "Choco-wow! You’re on fire!", 3)},
        {"quest_lvl_3", (20, "Reach 20 level!", "You’re melting the competition!", 4)},
    };

    public static SortedDictionary<int, string> idx_quests_mapping = new(
        quests_data.ToDictionary(kvp => kvp.Value.idx, kvp => kvp.Key)
    );

    public static Dictionary<string, (
        string title,
        string ps_desc,
        (string l, string r) childs,
        List<(string format, float val)> val_ps,
        int grade
    )> profs_data = new(){
        //f - fraction/доля
        //p - probability  
        //a1 - as is, но с домножением 
        //a0 - as is без домножения
        //d - duration
        //o - other
        {"Novice", (
            "Produce with every chocolate click!",
            "No passive skill, you're a Novice!",
            ("Chocolate Industrialist", "Chocolate Enthusiast"),
            new(),
            0
        )},
        {"Chocolate Industrialist", (
            "Increases base chocolate production",
            "Chocolate per Second increases by {0}%",
            ("Manufacturer", "Economist"),
            new(){("f", 0.2f)},
            1
        )},
        {"Chocolate Enthusiast", (
            "Enhances click efficiency and critical hits",
            "+{0}% chance for a critical click",
            ("Combo Master", "Chocolate Crusher"),
            new(){("p", 0.05f)},
            1
        )},
        {"Manufacturer", (
            "Increases production on a permanent basis",
            "+{0}% Chocolate per Second for each opened upgrade",
            (null, null),
            new(){("f", 0.2f)},
            2
        )},
        {"Economist", (
            "Improves purchase efficiency",
            "{0}% chance to immediately upgrade an item by {1} levels upon purchase",
            (null, null),
            new(){("p", 0.2f), ("a0", 2f)},
            2
        )},
        {"Combo Master", (
            "Boosts bonuses for click combos",
            "Every {0}th click yields x{1} - x{2} the usual tap chocolate",
            (null, null),
            new(){("a0", 10f), ("a1", 10f), ("a1", 30f)},
            2
        )},
        {"Chocolate Crusher", (
            "Strengthens critical clicks and increases their activation chances",
            "+{0}% Critical Chance. Critical clicks yield x{1} chocolate",
            (null, null),
            new(){("p", 0.25f), ("a1", 3f)},
            2
        )}
    };

    public static Dictionary<string, (float start_str, string type, int max_lvl, float default_val, string desc)> 
    arts_data = new(){   
        //(upgr_nm, upgr_base_str, max_lvl, default_val, upgr_desc)
        {"Bar of Wealth", (1.3f, "m", -1, 1f, "x{0} All Chocolate")},
        {"Sweetie Hand", (1.5f, "m", -1, 1f, "x{0} Tap Chocolate")},
        {"Time Accelerator", (1.3f, "m", -1, 1f, "x{0} Passive Chocolate")},
        {"Cacao Multiplier", (1.15f, "m", -1, 1f, "x{0} Cacao Beans")},
        {"Critical Chocoarrow", (0.05f, "p", 10, 0f, "+{0}% Critical Chance")},
        {"Smooth Gear", (1.3f, "m", -1, 1f, "x{0} Passive Skill Efficiency")},
        {"Fortune Crystal", (0.001f, "p", 10, 0f, "+{0}% Diamond Chance on Tap")},
        {"Caramel Essence", (0.01f, "p", 15, 0f, "+{0}% Tap Chocolate From Upgrades")},
    };

    //nm: price, min_lvl, desc
    public static Dictionary<string, (int price, int min_lvl, string desc)> 
    skins_data = new(){
        {"Just chocolate bar", (-1, -1, 
            "pure chocolate perfection in every tap! For those who appreciate classic style with every click!")},
        {"Cocoa Whirl", (10, 10,
            "smooth, rich chocolate candy with a signature swirl, offering a perfect blend of deep cocoa flavor and sweetness.")},
        {"Caramel Drizzle", (300, 20, 
            "A smooth chocolate candy with delicate caramel streaks, offering a rich cocoa flavor paired with a gentle caramel sweetness.")},
        {"Nutty Crunch", (450, 30, 
            "A rich chocolate shell with crunchy nuts and a hint of caramel for the perfect bite.")},
        {"Cherry Delight", (700, 40, 
            "Decadent chocolate cake with rich layers and a cherry on top, balancing sweetness and depth.")},
        {"Choco Pop", (1, 99, 
            "Classic chocolate-coated ice cream bar with a creamy, rich center, perfect for a cool treat.")},
    };

    public static List<string> skins_order = new(){
        "Just chocolate bar",
        "Cocoa Whirl",
        "Caramel Drizzle",
        "Nutty Crunch",
        "Cherry Delight",
        "Choco Pop"
    };

    public static Dictionary<string, (int open_lvl, float bs, float bp)> 
    upgrs_data = new(){   
        //lvl: ('upgr_nm', 'upgr_base_str', 'upgr_base_price', 'upgr_info')
        {"Chocolate Chip", (2, 15f, 180f)},
        {"Cacao Magic Tree", (4, 35f, 600f)},
        {"Chocolate Fountain", (6, 65f, 1900f)},
        {"Chocolate Factory", (8, 300f, 12000f)},
        {"Chocolate River", (10, 650f, 40000f)},
        {"Rain of Chocolate", (12, 1500f, 128000f)},
        {"Wave of Chocolate", (14, 3700f, 420000f)},
        {"Chocolate Volcano", (16, 8500f, 1350000f)},
        {"Cacao Cyclone", (18, 20000f, 4371000f)},
        {"Chocolate Singularity", (20, 46000f, 14150000f)},
        {"Chocolate Universe", (22, 100000f, 46000000f)},
        {"Chocolate Divinity", (24, 250000f, 150000000f)},
    };

    public static SortedDictionary<int, string> lvl_upgr_mapping = new(
        upgrs_data.ToDictionary(kvp => kvp.Value.open_lvl, kvp => kvp.Key)
    );

    public static List<string> upgrs_sorted_list = upgrs_data
        .OrderBy(kvp => kvp.Value.open_lvl).Select(kvp => kvp.Key).ToList();

    public static Dictionary<string, (string desc, List<float> val_list, string mode)> achs_data = new(){
        {"tap", ("Tap {0} times", new List<float> {150f, 600f, 2.5e3f, 1e5f, 4e5f}, "int")}, 
        {"artifacts", ("Discover {0} artifact(s)", new List<float> {1f, 6f, 10f}, "int")},
        {"lvl", ("Achieve {0} level", new List<float> {20f, 30f, 40f, 50f, 99f}, "int")},
        {"skins", ("Purchase {0} skin(s)", new List<float> {1f, 2f, 5f}, "int")},
        {"hours", ("Play {0} hour(s)", new List<float> {1f, 2f, 3f, 4f, 5f}, "time")}, 
        {"upgrs", ("Unlock {0} upgrade(s)", new List<float> {6f, 9f, 12f}, "int")},
        {"gps", ("Reach {0} chocolate/sec", new List<float> {1e2f, 1e7f, 1e12f, 1e20f, 1e30f}, "int")}, 
        {"cacao beans", ("Collect {0} cacao bean(s)", new List<float> {1f, 100f, 1e3f, 1e7f, 1e20f}, "int")}
    };

    public static Dictionary<int, int> achs_reward = new(){
        {1, 10},
        {2, 30},
        {3, 100},
        {4, 300},
        {5, 200}
    };

    public static List<Sprite> gen_on_click_sprites;

    public static GameObject
        upgr_slot_pf,
        art_slot_pf,
        click_num_pf,
        tap_popup_bonus_ind_pf,
        chocofall_pf,
        ach_slot_pf,
        skin_slot_pf,
        ach_star_pf,
        gen_choco_pf,
        golden_chocolate_pf;

    public static AudioClip 
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
        change_ac,
        click_ac,
        open_win_ac,
        change_shop_ac,
        diamond_on_tap_ac,
        reveal_bttn_from_lock_ac,
        quest_completion_ac,
        typewrite_ac,
        gold_chocolate_ac,
        gain_gps_ac;

    public static Color32
        active_skin_bttn_clr,
        not_active_skin_bttn_clr,
        active_shop_bt_clr,
        not_active_shop_bt_clr,
        prof_entered_clr,
        prof_exited_clr,
        default_tap_clr,
        crit_tap_clr,
        combo_tap_clr,
        tap_indicator_multiplier_clr,
        active_upgr_clr,
        not_active_upgr_clr;
}