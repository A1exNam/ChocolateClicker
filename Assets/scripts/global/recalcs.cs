using System.Collections.Generic;
using System;


public static class recalcs{
    public static void init(){
    }

    public static void recalc_diamonds_ch(){
        statics.mngr_tap.diamond_ch = consts.b_diamond_ch;
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Fortune Crystal", out var art1)) {
            statics.mngr_tap.diamond_ch += art1.str;
        }
    }

    public static void recalc_tempering_reward_m(){
        statics.mngr_tempering.cb_reward_m = 1f;
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Cacao Multiplier", out var art1)) {
            statics.mngr_tempering.cb_reward_m *= art1.str;
        }
        recalc_tempering_reward_amount();
    }

    public static void recalc_tempering_reward_amount(){
        if (statics.mngr_xp.lvl < consts.min_temper_lvl){
            statics.mngr_tempering.cb_reward_amount_f = 0f;
        } else {
            statics.mngr_tempering.cb_reward_amount_f = (float)Math.Truncate(
                (float)Math.Pow(consts.tempering_base_reward, 
                statics.mngr_xp.lvl - consts.min_temper_lvl + 1) 
                * statics.mngr_tempering.cb_reward_m
            );            
        }

        statics.mngr_tempering.act_ui(new(){"amount"});
    }

    public static void recalc_discover_price(){
        statics.mngr_discover.discover_price = (float)Math.Truncate(
            (float)Math.Pow(consts.discover_price_base, statics.mngr_arts.opened_arts_dict.Count)
            * consts.discover_price_coef
        ); 
        statics.mngr_discover.act_ui(new(){"info_slot"});
    }

    public static void recalc_gps(){
        statics.mngr_upgrs.gps = 0;
        foreach(upgr u in statics.mngr_upgrs.upgrs_list){
            statics.mngr_upgrs.gps += (float)Math.Truncate(u.f_gps);
        }
        statics.mngr_upgrs.act_ui(new(){"stats"});

        if (statics.mngr_arts.opened_arts_dict.ContainsKey("Caramel Essence")){
            recalcs.recalc_tap_f_tap();
        }

        var ach_upgrs_temp = statics.mngr_achs.achs_dict["gps"];
        if (statics.mngr_upgrs.gps > ach_upgrs_temp.val){
            ach_upgrs_temp.val = statics.mngr_upgrs.gps;
            ach_upgrs_temp.on_val_change();
        }
    }

    public static void recalc_gps_m(){
        statics.mngr_upgrs.gps_m = 1f; 
        switch (statics.mngr_prof.cur_prof_nm){
            case "Chocolate Industrialist":
                statics.mngr_upgrs.gps_m *= 1 + statics.mngr_prof.get_fval_ps()[0];
                if (statics.mngr_prof.as_activated) {
                    statics.mngr_upgrs.gps_m *= 1 + statics.mngr_prof.get_fval_as()[0]; 
                }
                break;
            case "Manufacturer":
                int opened_u = 0;
                foreach(var u in statics.mngr_upgrs.upgrs_list){
                    if (statics.mngr_xp.lvl >= consts.upgrs_data[u.nm].open_lvl){
                        opened_u++;
                    }
                }
                statics.mngr_upgrs.gps_m *= 1 + statics.mngr_prof.get_fval_ps()[0] 
                    * opened_u;
                if (statics.mngr_prof.as_activated) {
                    statics.mngr_upgrs.gps_m *= statics.mngr_prof.get_fval_as()[0]; 
                }
                break;
            case "Combo Master":
                if (statics.mngr_prof.as_activated) {
                    statics.mngr_upgrs.gps_m *= (float)Math.Pow(
                        statics.mngr_prof.get_fval_as()[0], 
                        statics.mngr_tap.click_cnt_while_as
                    ); 
                }
                break;
        }
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Bar of Wealth", out var art1)){
            statics.mngr_upgrs.gps_m *= art1.str;
        }
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Time Accelerator", out var art2)){
            statics.mngr_upgrs.gps_m *= art2.str;
        }
        if (statics.mngr_ad_bttn.is_bonus_active){
            statics.mngr_upgrs.gps_m *= consts.ad_m;
        }
        recalc_upgrs_f_gps(statics.mngr_upgrs.upgrs_list);
        recalc_upgrs_f_gain(statics.mngr_upgrs.upgrs_list);
    }

    public static void recalc_crit_ch(){
        statics.mngr_tap.crit_ch = consts.b_crit_ch;
        var temp1 = statics.mngr_prof.get_fval_ps();
        switch (statics.mngr_prof.cur_prof_nm){
            case "Chocolate Enthusiast":
                statics.mngr_tap.crit_ch += temp1[0];
                break;
            case "Chocolate Crusher":
                statics.mngr_tap.crit_ch += temp1[0];
                if (statics.mngr_prof.as_activated){
                    statics.mngr_tap.crit_ch += statics.mngr_prof.get_fval_as()[0];
                }
                break;
        }
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Critical Chocoarrow", out var art1)){
            statics.mngr_tap.crit_ch += art1.str;
        }
    }

    public static void recalc_crit_m(){
        statics.mngr_tap.crit_m = consts.b_crit_m;
        if (statics.mngr_prof.cur_prof_nm == "Chocolate Crusher"){
            statics.mngr_tap.crit_m = statics.mngr_prof.get_fval_ps()[1];
        }
    }

    public static void recalc_tap_f_gain(){
        statics.mngr_tap.f_gain = 
            (float)Math.Truncate(statics.mngr_tap.b_gain * statics.mngr_tap.tap_m);
        statics.mngr_tap.act_ui(new(){"gain"});
    }

    public static void recalc_tap_f_tap(){
        statics.mngr_tap.f_tap = 
            (float)Math.Truncate(statics.mngr_tap.b_tap * statics.mngr_tap.tap_m);
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Caramel Essence", out var art1)){
            statics.mngr_tap.f_tap += (float)Math.Truncate(art1.str * statics.mngr_upgrs.gps);
        }
        statics.mngr_tap.act_ui(new(){"tap"});
    }

    public static void recalc_tap_m(){
        statics.mngr_tap.tap_m = 1f;
        if (statics.mngr_prof.cur_prof_nm == "Chocolate Enthusiast" && statics.mngr_prof.as_activated){
            statics.mngr_tap.tap_m *= 1 + statics.mngr_prof.get_fval_as()[0];
        }
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Sweetie Hand", out var art1)){
            statics.mngr_tap.tap_m *= art1.str;
        }
        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Bar of Wealth", out var art2)){
            statics.mngr_tap.tap_m *= art2.str;
        }
        if (statics.mngr_ad_bttn.is_bonus_active){
            statics.mngr_tap.tap_m *= consts.ad_m;
        }
        recalc_tap_f_tap();
        recalc_tap_f_gain();
    }

    public static void recalc_cost_m(){
        statics.mngr_upgrs.cost_m = 1f; 
        if (statics.mngr_prof.cur_prof_nm == "Economist"){
            if (statics.mngr_prof.as_activated){
                statics.mngr_upgrs.cost_m *= (float)Math.Pow(
                    statics.mngr_prof.get_fval_as()[0],
                    statics.mngr_tap.click_cnt_while_as
                );
            }
        }
        
        recalc_upgrs_f_price(statics.mngr_upgrs.upgrs_list);
    }

    public static void recalc_skins_state(List<skin> skins_list){
        foreach(var s in skins_list){
            if ((s.state == -1) && (statics.mngr_xp.max_lvl >= consts.skins_data[s.nm].min_lvl)){
                s.state = 0;
                s.act_ui(new(){"all_except_alpha", "bttn_alpha"});
            }
        }
    }

    public static void recalc_upgrs_f_price(List<upgr> upgrs_list){
        foreach (var u in upgrs_list){
            u.f_price = (float)Math.Truncate(u.b_price * statics.mngr_upgrs.cost_m);
            u.act_ui(new(){"price", "alpha"});
        }
    }

    public static void recalc_upgrs_f_gps(List<upgr> upgrs_list){
        foreach (var u in upgrs_list){
            u.f_gps = (float)Math.Truncate(u.b_gps * statics.mngr_upgrs.gps_m);
        }
        recalc_gps();
    }

    public static void recalc_upgrs_f_gain(List<upgr> upgrs_list){
        foreach (var u in upgrs_list){
            u.f_gain = (float)Math.Truncate(u.b_gain * statics.mngr_upgrs.gps_m);
            u.act_ui(new(){"gain"});   
        }
    }
}
