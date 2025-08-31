using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

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
        cf_acceleration = 380f,
        cf_min_size = 0.6f,
        cf_max_size = 0.9f,
        cf_sample_lifetime,
        cf_spawn_interval = 0.3f,
        cf_min_alpha = 0.75f,

        //rd
        x1_rd_crd,
        x2_rd_crd,
        y_rd_crd,
        rd_sample_lifetime,
        rd_spawn_interval = 0.03f,
        rd_acceleration = 1300f,

        //discover
        discover_price_base = 2f,
        discover_price_coef = 1f,

        //xp
        xp_base = 1.5f, //xp_base^lvl * xp_coef
        xp_coef = 10f,

        //tap
        st_tap_price = 30f,
        st_b_tap = 1f,
        st_b_tap_gain = 1f, 
        tap_price_coef = 1.6f,
        tap_gain_coef = 1.3f,
        b_crit_ch = 0.01f,
        b_crit_m = 2f,
        b_diamond_ch = 0.001f,

        //active skill
        as_cd = 30f,

        //upgrs
        upgrs_price_coef = 1.8f,
        upgrs_gain_coef = 1.3f,

        //arts
        st_arts_price = 1f,
        arts_price_coef = 1.5f,
        arts_str_coef = 1.3f,

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
        typewriter_delay = 0.05f,

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

        //quests
        reward_period = 4f;

    public static string
        //prof
        start_prof_nm = "Novice",

        //save-restore
        arts_order_postfix = "_order",
        achs_postfix = "_ach",
        achs_rewarded_cnt_postfix = achs_postfix + "_rwrd_cnt",
        achs_noft_idx_postfix = achs_postfix + "_noft_idx";

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
        string as_desc, 
        (string l, string r) childs, 
        List<(string format, float val)> val_ps, 
        List<(string format, float val)> val_as,
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
            "Level up to get better grade profession!", 
            ("Chocolate Industrialist", "Chocolate Enthusiast"),
            new(), 
            new(), 
            0
        )},
        {"Chocolate Industrialist", (
            "Increases base chocolate production",
            "Chocolate per Second increases by {0}%",
            "For {1} seconds Chocolate per Second increases by {0}%",
            ("Manufacturer", "Economist"),
            new(){("f", 0.2f)},
            new(){("f", 0.35f), ("d", 10f)},
            1
        )},
        {"Chocolate Enthusiast", (
            "Enhances click efficiency and critical hits",
            "+{0}% chance for a critical click",
            "For {1} seconds taps get {0}% more chocolate",
            ("Combo Master", "Chocolate Crusher"),
            new(){("p", 0.05f)},
            new(){("f", 0.2f), ("d", 10f)},
            1
        )},
        {"Manufacturer", (
            "Increases production on a permanent basis",
            "+{0}% Chocolate per Second for each opened upgrade",
            "x{0} Chocolate per Second for {1} seconds",
            (null, null),
            new(){("f", 0.2f)},
            new(){("a1", 3f), ("d", 15f)},
            2
        )},
        {"Economist", (
            "Improves purchase efficiency",
            "{0}% chance to immediately upgrade an item by {1} levels upon purchase",
            "For {1} seconds each tap reduces the cost of all upgrades by x{0}",
            (null, null),
            new(){("p", 0.2f), ("a0", 2f)},
            new(){("o", 0.99f), ("d", 15f)},
            2
        )},
        {"Combo Master", (
            "Boosts bonuses for click combos",
            "Every {0}th click yields x{1} - x{2} the usual tap chocolate",
            "For {1} seconds each click increases total chocolate production by x{0}",
            (null, null),
            new(){("a0", 10f), ("a1", 10f), ("a1", 30f)},
            new(){("o", 1.025f), ("d", 15f)},
            2
        )},
        {"Chocolate Crusher", (
            "Strengthens critical clicks and increases their activation chances",
            "+{0}% Critical Chance. Critical clicks yield x{1} chocolate",
            "For {1} seconds critical click chance increases by {0}%",
            (null, null),
            new(){("p", 0.25f), ("a1", 3f)},
            new(){("f", 0.5f), ("d", 15f)},
            2
        )}
    };

    public static Dictionary<string, (float start_str, string type, int max_lvl, float default_val, string desc)> 
    arts_data = new(){   
        //(upgr_nm, upgr_base_str, max_lvl, default_val, upgr_desc)
        {"Bar of Wealth", (1.5f, "m", -1, 1f, "x{0} All Chocolate")},
        {"Sweetie Hand", (2f, "m", -1, 1f, "x{0} Tap Chocolate")},
        {"Time Accelerator", (2f, "m", -1, 1f, "x{0} Passive Chocolate")},
        {"Cacao Multiplier", (1.5f, "m", -1, 1f, "x{0} Cacao Beans")},
        {"Critical Chocoarrow", (0.02f, "p", 10, 0f, "+{0}% Critical Chance")},
        {"Smooth Gear", (1.5f, "m", -1, 1f, "x{0} Passive Skill Efficiency")},
        {"Chocoextender", (1.5f, "m", -1, 1f, "x{0} Active Skill Duration")},
        {"Chococharger", (1.5f, "m", -1, 1f, "x{0} Active Skill Efficiency")},
        {"Fortune Crystal", (0.001f, "p", 10, 0f, "+{0}% Diamond Chance on Tap")},
        {"Caramel Essence", (0.01f, "p", 15, 0f, "+{0}% Tap Gain From Passive")},
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
        {"Chocolate Chip", (4, 1f, 40f)},
        {"Cacao Magic Tree", (10, 7f, 150f)},
        {"Chocolate Fountain", (15, 50f, 1e3f)},
        {"Chocolate Factory", (20, 300f, 7e3f)},
        {"Chocolate River", (25, 1.4e4f, 3.5e5f)},
        {"Rain of Chocolate", (30, 1e5f, 2.5e6f)},
        {"Wave of Chocolate", (35, 5e5f, 1.5e7f)},
        {"Chocolate Volcano", (40, 3.5e6f, 1e8f)},
        {"Cacao Cyclone", (45, 2.5e7f, 7e8f)},
        {"Chocolate Singularity", (50, 1.5e8f, 4.9e9f)},
        {"Chocolate Universe", (55, 1e9f, 3.5e10f)},
        {"Chocolate Divinity", (99, 1e10f, 2e11f)},
    };

    public static SortedDictionary<int, string> lvl_upgr_mapping = new(
        upgrs_data.ToDictionary(kvp => kvp.Value.open_lvl, kvp => kvp.Key)
    );

    public static List<string> upgrs_sorted_list = upgrs_data
        .OrderBy(kvp => kvp.Value.open_lvl).Select(kvp => kvp.Key).ToList();

    public static Dictionary<string, (string desc, List<float> val_list, string mode)> achs_data = new(){
        {"tap", ("Tap {0} times", new List<float> {150f, 500f, 1e3f, 1e4f, 1e5f}, "int")}, 
        {"artifacts", ("Discover {0} artifact(s)", new List<float> {1f, 5f, 10f}, "int")},
        {"lvl", ("Achieve {0} level", new List<float> {15f, 25f, 40f, 60f, 100f}, "int")},
        {"skins", ("Purchase {0} skin(s)", new List<float> {1f, 2f, 5f}, "int")},
        {"hours", ("Play {0} hour(s)", new List<float> {1f, 2f, 4f, 16f, 32f}, "time")}, 
        {"upgrs", ("Unlock {0} upgrade(s)", new List<float> {3f, 9f, 15f}, "int")},
        {"gps", ("Reach {0} chocolate/sec", new List<float> {1e2f, 1e4f, 1e8f, 1e13f, 1e20f}, "int")}, 
        {"cacao beans", ("Collect {0} cacao bean(s)", new List<float> {1f, 20f, 300f, 1.5e3f, 1e7f}, "int")}
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
        chocofall_pf,
        ach_slot_pf,
        skin_slot_pf,
        ach_star_pf,
        raindrop_pf,
        gen_choco_pf;

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
        active_skill_ac, 
        change_ac,
        click_ac,
        open_win_ac,
        change_shop_ac,
        diamond_on_tap_ac,
        reveal_bttn_from_lock_ac,
        quest_completion_ac,
        typewrite_ac;

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
        active_upgr_clr,
        not_active_upgr_clr;
}