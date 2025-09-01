using System.Collections.Generic;
using CrazyGames;
using System;
using UnityEngine;
using System.Collections;

public static class save_module{
    public static bool 
        is_saves_restored_or_timeout = false,
        is_all_keys_deleted = false;
    public static IEnumerator init(){
        if (CrazySDK.IsAvailable){
            CrazySDK.Init(() => {});
            float elapsed_t = 0f;
            while (!CrazySDK.IsInitialized && elapsed_t < sdk_common.sdk_timeout_init){
                yield return null;
                elapsed_t += Time.deltaTime;
            }
            if (CrazySDK.IsInitialized){
                //CrazySDK.Data.DeleteAll();
                save_module.call_restores();

                statics.mngr_tempering.init_after_save_restore();
                statics.mngr_ad_bttn.init_after_save_restore();
                statics.mngr_tutor.init_after_restore();
                statics.mngr_quests.init_after_restore();

                sdk_common.gp_start();
            }
            urefs.load_scr_go.SetActive(false);
            urefs.music_asrc_as.enabled = true;
        }
    }

    public static void call_restores(){
        restore_gameover_status();
        if (!statics.mngr_gameover.is_gameover){
            restore_achs();
            restore_lvlxp();
            restore_tutor_bttns();
            restore_balance();
            restore_upgrs();
            restore_arts();
            restore_tap();
            restore_diamonds();
            restore_skins();
            restore_cb();
            restore_music_and_sound_vols();
            restore_prof();
            restore_offline_reward();
            restore_tutors();
            restore_quests();
            is_saves_restored_or_timeout = true;
        }
    }

    //call strictly after restore_lvlxp
    public static void restore_tutor_bttns(){
        foreach (string tutor_nm in statics.mngr_tutor.bttn_tutor_dict_collection.Keys){
            if (CrazySDK.Data.HasKey(tutor_nm)){
                statics.mngr_tutor.is_opened_dict[tutor_nm] = 
                    CrazySDK.Data.GetInt(tutor_nm) != 0;
                statics.mngr_tutor.act_ui(new(){tutor_nm});;
            }
        }

        if (CrazySDK.Data.HasKey("as_tutor")){
            statics.mngr_tutor.is_shown_tutor_as = 
                CrazySDK.Data.GetInt("as_tutor") != 0;
        }
    }

    //call strictly after restore_lvlxp
    public static void restore_upgrs(){
        foreach (var kv in consts.lvl_upgr_mapping){
            if (CrazySDK.Data.HasKey(kv.Value)){
                upgr u = statics.mngr_upgrs.upgrs_dict[kv.Value];

                u.lvl = CrazySDK.Data.GetInt(kv.Value);
                for (int i=0; i<u.lvl; i++){
                    u.b_gps += u.b_gain;
                    u.b_gain *= consts.upgrs_gain_coef;
                    u.b_price *= consts.upgrs_price_coef;
                }
                if (u.lvl == consts.upgrs_max_lvl){
                    u.b_gain = -1f;
                    u.b_price = -1f;
                    u.act_ui(new(){"price"});
                } else {
                    recalcs.recalc_upgrs_f_price(new(){u});
                    recalcs.recalc_upgrs_f_gain(new(){u});
                }

                u.act_ui(new(){"lvl"});
                recalcs.recalc_upgrs_f_gps(new(){u});
            }
        }
        statics.mngr_upgrs.act_ui(new(){"stats", "state", "new"});
    }

    public static void restore_arts(){
        SortedDictionary<int, string> ordered_arts_to_restore = new();
        foreach (string art_nm in consts.arts_data.Keys){
            string k = art_nm + consts.arts_order_postfix;
            if (CrazySDK.Data.HasKey(k)){
                ordered_arts_to_restore[CrazySDK.Data.GetInt(k)] = art_nm; 
            }
        }
        foreach (string v in ordered_arts_to_restore.Values){
            art a = new(v);
            statics.mngr_arts.arts_list.Add(a);
            statics.mngr_arts.opened_arts_dict[v] = a;
            statics.mngr_arts.closed_arts_set.Remove(v);

            a.lvl = CrazySDK.Data.GetInt(v);
            for (int i=consts.st_arts_lvl; i<a.lvl; i++){
                if (a.lvl == consts.arts_data[v].max_lvl){
                    a.price = -1f;
                } else {
                    a.price *= consts.arts_price_coef;
                }
                a.str *= consts.arts_str_coef;
            }

            a.act_ui(new(){"bb", "str", "lvl"});
            statics.mngr_arts.refresh_params(v);
        }
        statics.mngr_arts.act_ui(new(){"stats", "bttn_alpha"});
        recalcs.recalc_discover_price();
    }

    public static void restore_diamonds(){
        if (CrazySDK.Data.HasKey("diamonds")){
            statics.mngr_diamonds.amount = CrazySDK.Data.GetInt("diamonds");
            statics.mngr_diamonds.act_ui();
        }
    }
    
    public static void restore_lvlxp(){
        if (CrazySDK.Data.HasKey("level")){
            statics.mngr_xp.lvl = CrazySDK.Data.GetInt("level");
        }
        if (CrazySDK.Data.HasKey("xp")){
            statics.mngr_xp.xp = CrazySDK.Data.GetFloat("xp");
        }
        if (CrazySDK.Data.HasKey("max_lvl")){
            statics.mngr_xp.max_lvl = CrazySDK.Data.GetInt("max_lvl");
        }
        statics.mngr_xp.act_ui();
    }
    
    //call strictly after restore_lvlxp
    public static void restore_skins(){
        recalcs.recalc_skins_state(statics.mngr_skins.skins_list);
        foreach (string nm in consts.skins_data.Keys){
            if (CrazySDK.Data.HasKey(nm)){
                statics.mngr_skins.skins_dict[nm].state = CrazySDK.Data.GetInt(nm);
                if (statics.mngr_skins.skins_dict[nm].state == 2){
                    statics.mngr_skins.cur_skin = statics.mngr_skins.skins_dict[nm];
                }
                statics.mngr_skins.skins_dict[nm].act_ui(new(){"all_except_alpha"});  
            }
        }
        statics.mngr_skins.act_ui(new(){"bttn_alpha"});
    }
    
    public static void restore_cb(){
        if (CrazySDK.Data.HasKey("cb")){
            statics.mngr_cb.amount = CrazySDK.Data.GetFloat("cb");
            statics.mngr_cb.on_val_change();
        }
    }
    
    public static void restore_music_and_sound_vols(){
        if (CrazySDK.Data.HasKey("sound_vol")){
            urefs.sound_slider_sl.value = CrazySDK.Data.GetFloat("sound_vol");
            urefs.sound_text_txt.text = ((int)(urefs.sound_slider_sl.value * 100)).ToString();
            urefs.music_asrc_as.volume = urefs.sound_slider_sl.value;
        }
        if (CrazySDK.Data.HasKey("music_vol")){
            urefs.music_slider_sl.value = CrazySDK.Data.GetFloat("music_vol");
            urefs.music_text_txt.text = ((int)(urefs.music_slider_sl.value * 100)).ToString();
            urefs.music_asrc_as.volume = urefs.music_slider_sl.value * consts.dec_music_vol_coef;
        }
    }
    
    //call strictly after restore_xplvl
    public static void restore_prof(){
        statics.mngr_prof.act_ui(new(){"prof_w"});
        if (CrazySDK.Data.HasKey("cur_prof")){
            string old_prof_nm = statics.mngr_prof.cur_prof_nm;
            statics.mngr_prof.cur_prof_nm = CrazySDK.Data.GetString("cur_prof");
            statics.mngr_prof.change_params_for_ps(old_prof_nm);
            statics.mngr_prof.change_params_for_ps(statics.mngr_prof.cur_prof_nm);
            statics.mngr_prof.act_ui(new(){"win_bttn", "prof_w", "reset", "bttn_reset_alpha"});
        }
        if (CrazySDK.Data.HasKey("as_activated_or_cd")){
            if (CrazySDK.Data.GetInt("as_activated_or_cd") == 1){
                statics.logic_module.StartCoroutine(statics.mngr_prof.start_cd());
            }
        }
    }
    
    //in the end
    public static void restore_achs(){
        foreach (string ach_nm in consts.achs_data.Keys){
            if (CrazySDK.Data.HasKey(ach_nm + consts.achs_postfix)){
                statics.mngr_achs.achs_dict[ach_nm].val = 
                    CrazySDK.Data.GetFloat(ach_nm + consts.achs_postfix);
                statics.mngr_achs.achs_dict[ach_nm].last_nofted_idx = 
                    CrazySDK.Data.GetInt(ach_nm + consts.achs_noft_idx_postfix);
                statics.mngr_achs.achs_dict[ach_nm].rewarded_cnt = 
                    CrazySDK.Data.GetInt(ach_nm + consts.achs_rewarded_cnt_postfix);
                statics.mngr_achs.achs_dict[ach_nm]
                .act_ui(new(){"desc", "bttn_txt", "bar", "stars", "bttn_alpha"});
            }    
        }
    }

    public static void restore_tap(){
        if (CrazySDK.Data.HasKey("tap")){
            statics.mngr_tap.lvl = CrazySDK.Data.GetInt("tap");
            for (int i=0; i<statics.mngr_tap.lvl - 1; i++){
                statics.mngr_tap.b_tap += statics.mngr_tap.b_gain;

                statics.mngr_tap.b_gain *= consts.tap_gain_coef;

                statics.mngr_tap.price *= consts.tap_price_coef;
            }
            recalcs.recalc_tap_f_tap();
            recalcs.recalc_tap_f_gain();
            statics.mngr_tap.act_ui(new(){"lvl", "price"});
        }
    }

    public static void restore_offline_reward(){
        if (CrazySDK.Data.HasKey("last_played_dttm")){
            string dttm_to_parse = CrazySDK.Data.GetString("last_played_dttm");
            DateTime.TryParse(
                dttm_to_parse, 
                null, 
                System.Globalization.DateTimeStyles.RoundtripKind, 
                out DateTime parsed_time
            );
            float total_seconds = (float)(DateTime.Now - parsed_time).TotalSeconds;
            if (total_seconds >= consts.min_offline_period_to_get_rwrd){
                float reward_temp = (float)Math.Truncate(total_seconds 
                * statics.mngr_upgrs.gps * consts.offline_gps_dec_c);

                if (reward_temp > consts.min_offline_reward){
                    statics.mngr_offline_reward.can_save_last_played_dttm = false;
                    statics.mngr_offline_reward.init();
                    statics.mngr_offline_reward.reward = reward_temp;
                    statics.mngr_offline_reward.act_ui();
                    urefs.offln_rwrd_w_go.SetActive(true);
                }
            }
        }
    }

    public static void restore_balance(){
        if (CrazySDK.Data.HasKey("balance")){
            statics.mngr_balance.amount = CrazySDK.Data.GetFloat("balance");
            statics.mngr_balance.max_amount = CrazySDK.Data.GetFloat("max_balance");
            statics.mngr_balance.on_val_change();
        }
    }

    public static void restore_gameover_status(){
        if (CrazySDK.Data.HasKey("is_gameover")){
            statics.mngr_gameover.is_gameover = 
                CrazySDK.Data.GetInt("is_gameover") != 0;
            statics.mngr_gameover.act_ui();
        }
    }

    public static void restore_tutors(){
        var keys = new List<string>(statics.mngr_tutor.tutor_state_dict.Keys);

        foreach(string tutor_nm in keys){
            if (CrazySDK.Data.HasKey(tutor_nm)){
                statics.mngr_tutor.tutor_state_dict[tutor_nm] = CrazySDK.Data.GetInt(tutor_nm);
            }
        }
    }

    public static void restore_quests(){
        if (CrazySDK.Data.HasKey("quest_nm")){
            statics.mngr_quests.cur_quest_nm = CrazySDK.Data.GetString("quest_nm");
            statics.mngr_quests.act_ui();
        }
    }
        
    public static void save_upgr(upgr u){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt(u.nm, u.lvl);
        }
    }

    public static void clean_upgrs(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            foreach (string nm in consts.upgrs_data.Keys){
                CrazySDK.Data.DeleteKey(nm);
            }
        }
    }

    public static void save_lvlxp(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt("level", statics.mngr_xp.lvl);
            CrazySDK.Data.SetInt("max_lvl", statics.mngr_xp.max_lvl);
            CrazySDK.Data.SetFloat("xp", statics.mngr_xp.xp);
        }
    }

    public static void save_diamonds(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt("diamonds", statics.mngr_diamonds.amount);
        }   
    }

    public static void save_cb(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetFloat("cb", statics.mngr_cb.amount);
        }
    }

    public static void save_bttn_tutor(string tutor_nm){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt(tutor_nm, statics.mngr_tutor.is_opened_dict[tutor_nm] ? 1 : 0);
        }
    }

    public static void save_as_tutor(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt("as_tutor", statics.mngr_tutor.is_shown_tutor_as ? 1 : 0);
        }
    }

    public static void save_art(art a){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt(a.nm, a.lvl);
        }
    }

    public static void save_art_order(art a, int order){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt(a.nm + "_order", order);
        }
    }

    public static void save_skin(skin s){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt(s.nm, s.state);
        }
    }

    public static void save_sound_vol(){  
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetFloat("sound_vol", urefs.sound_slider_sl.value);
        }
    }

    public static void save_music_vol(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetFloat("music_vol", urefs.music_slider_sl.value);
        }
    }

    public static void save_as_status(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt("as_activated_or_cd", 
                statics.mngr_prof.as_activated || statics.mngr_prof.is_cd ? 1 : 0
            );
        }
    }

    public static void save_prof(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetString("cur_prof", statics.mngr_prof.cur_prof_nm);
        }
    }

    public static void save_ach(ach a){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetFloat(a.nm + consts.achs_postfix, a.val);
            CrazySDK.Data.SetInt(a.nm + consts.achs_rewarded_cnt_postfix, a.rewarded_cnt);
            CrazySDK.Data.SetInt(a.nm + consts.achs_noft_idx_postfix, a.last_nofted_idx);
        }
    }

    public static void save_tap(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt("tap", statics.mngr_tap.lvl);
        }
    }

    public static void save_last_played_dttm(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetString("last_played_dttm", DateTime.Now.ToString("o"));
        }
    }

    public static void save_balance(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetFloat("balance", statics.mngr_balance.amount);
            CrazySDK.Data.SetFloat("max_balance", statics.mngr_balance.max_amount);
        }
    }

    public static void save_gameover_status(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt("is_gameover", statics.mngr_gameover.is_gameover ? 1 : 0);
        }
    }

    public static void save_tutor(string tutor_nm){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetInt(tutor_nm, statics.mngr_tutor.tutor_state_dict[tutor_nm]);
        }
    }

    public static void save_quest(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            CrazySDK.Data.SetString("quest_nm", statics.mngr_quests.cur_quest_nm);
        }
    }

    public static void delete_all(){
        if (!is_all_keys_deleted && is_saves_restored_or_timeout){
            is_all_keys_deleted = true;
            CrazySDK.Data.DeleteAll();
        }
    }
}
