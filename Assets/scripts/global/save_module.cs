using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public static class save_module{
    public static bool 
        is_saves_restored = false,
        is_saves_timeout = false,
        is_all_keys_deleted = false;
    public static IEnumerator init(){
        urefs.load_scr_go.SetActive(true);
        is_saves_timeout = false;

        save_module.call_restores();

        statics.mngr_tempering.init_after_save_restore();
        statics.mngr_ad_bttn.init_after_save_restore();
        statics.mngr_tutor.init_after_restore();
        statics.mngr_quests.init_after_restore();

        sdk_common.gp_start();
        urefs.load_scr_go.SetActive(false);
        urefs.music_asrc_as.enabled = true;

        yield return null;
    }

    public static void call_restores(){
        //PlayerPrefs.DeleteAll();
        restore_gameover_ac_status();
        restore_gameover_status();
        if (!statics.mngr_gameover.is_gameover && !statics.mngr_gameover.is_gameover_anticlicker){
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
            is_saves_restored = true;
        }
    }

    //call strictly after restore_lvlxp
    public static void restore_tutor_bttns(){
        foreach (string tutor_nm in statics.mngr_tutor.bttn_tutor_dict_collection.Keys){
            if (PlayerPrefs.HasKey(tutor_nm)){
                statics.mngr_tutor.is_opened_dict[tutor_nm] =
                    PlayerPrefs.GetInt(tutor_nm) != 0;
                statics.mngr_tutor.act_ui(new(){tutor_nm});;
            }
        }

    }

    //call strictly after restore_lvlxp
    public static void restore_upgrs(){
        foreach (var kv in consts.lvl_upgr_mapping){
            if (PlayerPrefs.HasKey(kv.Value)){
                upgr u = statics.mngr_upgrs.upgrs_dict[kv.Value];

                u.lvl = PlayerPrefs.GetInt(kv.Value);
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
            if (PlayerPrefs.HasKey(k)){
                ordered_arts_to_restore[PlayerPrefs.GetInt(k)] = art_nm;
            }
        }
        foreach (string v in ordered_arts_to_restore.Values){
            art a = new(v);
            statics.mngr_arts.arts_list.Add(a);
            statics.mngr_arts.opened_arts_dict[v] = a;
            statics.mngr_arts.closed_arts_set.Remove(v);

            a.lvl = PlayerPrefs.GetInt(v);
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
        if (PlayerPrefs.HasKey("diamonds")){
            statics.mngr_diamonds.amount = PlayerPrefs.GetInt("diamonds");
            statics.mngr_diamonds.act_ui();
        }
    }
    
    public static void restore_lvlxp(){
        if (PlayerPrefs.HasKey("level")){
            statics.mngr_xp.lvl = PlayerPrefs.GetInt("level");
        }
        if (PlayerPrefs.HasKey("xp")){
            statics.mngr_xp.xp = PlayerPrefs.GetFloat("xp");
        }
        if (PlayerPrefs.HasKey("max_lvl")){
            statics.mngr_xp.max_lvl = PlayerPrefs.GetInt("max_lvl");
        }
        statics.mngr_xp.act_ui();
    }
    
    //call strictly after restore_lvlxp
    public static void restore_skins(){
        recalcs.recalc_skins_state(statics.mngr_skins.skins_list);
        foreach (string nm in consts.skins_data.Keys){
            if (PlayerPrefs.HasKey(nm)){
                statics.mngr_skins.skins_dict[nm].state = PlayerPrefs.GetInt(nm);
                if (statics.mngr_skins.skins_dict[nm].state == 2){
                    statics.mngr_skins.cur_skin = statics.mngr_skins.skins_dict[nm];
                }
                statics.mngr_skins.skins_dict[nm].act_ui(new(){"all_except_alpha"});  
            }
        }
        statics.mngr_skins.act_ui(new(){"bttn_alpha"});
    }
    
    public static void restore_cb(){
        if (PlayerPrefs.HasKey("cb")){
            statics.mngr_cb.amount = PlayerPrefs.GetFloat("cb");
            statics.mngr_cb.on_val_change();
        }
    }
    
    public static void restore_music_and_sound_vols(){
        if (PlayerPrefs.HasKey("sound_vol")){
            urefs.sound_slider_sl.value = PlayerPrefs.GetFloat("sound_vol");
            urefs.sound_text_txt.text = ((int)(urefs.sound_slider_sl.value * 100)).ToString();
            statics.mngr_settings.refresh_sound_volume();
        }
        if (PlayerPrefs.HasKey("music_vol")){
            urefs.music_slider_sl.value = PlayerPrefs.GetFloat("music_vol");
            urefs.music_text_txt.text = ((int)(urefs.music_slider_sl.value * 100)).ToString();
            statics.mngr_settings.refresh_music_volume();
        }
    }
    
    //call strictly after restore_xplvl
    public static void restore_prof(){
        statics.mngr_prof.act_ui(new(){"prof_w"});
        if (PlayerPrefs.HasKey("cur_prof")){
            string old_prof_nm = statics.mngr_prof.cur_prof_nm;
            statics.mngr_prof.cur_prof_nm = PlayerPrefs.GetString("cur_prof");
            statics.mngr_prof.change_params_for_ps(old_prof_nm);
            statics.mngr_prof.change_params_for_ps(statics.mngr_prof.cur_prof_nm);
            statics.mngr_prof.act_ui(new(){"win_bttn", "prof_w", "reset", "bttn_reset_alpha"});
        }
    }
    
    //in the end
    public static void restore_achs(){
        foreach (string ach_nm in consts.achs_data.Keys){
            if (PlayerPrefs.HasKey(ach_nm + consts.achs_postfix)){
                statics.mngr_achs.achs_dict[ach_nm].val =
                    PlayerPrefs.GetFloat(ach_nm + consts.achs_postfix);
                statics.mngr_achs.achs_dict[ach_nm].last_nofted_idx =
                    PlayerPrefs.GetInt(ach_nm + consts.achs_noft_idx_postfix);
                statics.mngr_achs.achs_dict[ach_nm].rewarded_cnt =
                    PlayerPrefs.GetInt(ach_nm + consts.achs_rewarded_cnt_postfix);
                statics.mngr_achs.achs_dict[ach_nm]
                .act_ui(new(){"desc", "bttn_txt", "bar", "stars", "bttn_alpha"});
            }    
        }
    }

    public static void restore_tap(){
        if (PlayerPrefs.HasKey("tap")){
            statics.mngr_tap.lvl = PlayerPrefs.GetInt("tap");
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
        if (PlayerPrefs.HasKey("last_played_dttm")){
            string dttm_to_parse = PlayerPrefs.GetString("last_played_dttm");
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
        if (PlayerPrefs.HasKey("balance")){
            statics.mngr_balance.amount = PlayerPrefs.GetFloat("balance");
            statics.mngr_balance.max_amount = PlayerPrefs.GetFloat("max_balance");
            statics.mngr_balance.on_val_change();
        }
    }

    public static void restore_gameover_status(){
        if (PlayerPrefs.HasKey("is_gameover")){
            statics.mngr_gameover.is_gameover =
                PlayerPrefs.GetInt("is_gameover") != 0;
            statics.mngr_gameover.act_ui();
        }
    }

    public static void restore_gameover_ac_status(){
        if (PlayerPrefs.HasKey("is_gameover_ac")){
            statics.mngr_gameover.is_gameover_anticlicker =
                PlayerPrefs.GetInt("is_gameover_ac") != 0;
            statics.mngr_gameover.act_ui();
        }
    }

    public static void restore_tutors(){
        var keys = new List<string>(statics.mngr_tutor.tutor_state_dict.Keys);

        foreach(string tutor_nm in keys){
            if (PlayerPrefs.HasKey(tutor_nm)){
                statics.mngr_tutor.tutor_state_dict[tutor_nm] = PlayerPrefs.GetInt(tutor_nm);
            }
        }
    }

    public static void restore_quests(){
        if (PlayerPrefs.HasKey("quest_nm")){
            statics.mngr_quests.cur_quest_nm = PlayerPrefs.GetString("quest_nm");
        } else {
            statics.mngr_quests.cur_quest_nm = consts.idx_quests_mapping.Last().Value;
            //stub
            if (statics.mngr_quests.is_quest_completed()){
                statics.mngr_quests.cur_quest_nm = "questline_finished";
            } else {
                statics.mngr_quests.cur_quest_nm = consts.idx_quests_mapping[0];
            }
        }
        statics.mngr_quests.act_ui();
    }
        
    public static void save_upgr(upgr u){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(u.lvl)){
                PlayerPrefs.SetInt(u.nm, u.lvl);
                PlayerPrefs.Save();
            }
        }
    }

    public static void clean_upgrs(){
        if (!is_all_keys_deleted && is_saves_restored){
            foreach (string nm in consts.upgrs_data.Keys){
                PlayerPrefs.DeleteKey(nm);
            }
            PlayerPrefs.Save();
        }
    }

    public static void save_lvlxp(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(statics.mngr_xp.lvl)
            && common_utils.is_valid(statics.mngr_xp.max_lvl)
            && common_utils.is_valid(statics.mngr_xp.xp)){
                PlayerPrefs.SetInt("level", statics.mngr_xp.lvl);
                PlayerPrefs.SetInt("max_lvl", statics.mngr_xp.max_lvl);
                PlayerPrefs.SetFloat("xp", statics.mngr_xp.xp);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_diamonds(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(statics.mngr_diamonds.amount)){
                PlayerPrefs.SetInt("diamonds", statics.mngr_diamonds.amount);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_cb(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(statics.mngr_cb.amount)){
                PlayerPrefs.SetFloat("cb", statics.mngr_cb.amount);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_bttn_tutor(string tutor_nm){
        if (!is_all_keys_deleted && is_saves_restored){
            int res = statics.mngr_tutor.is_opened_dict[tutor_nm] ? 1 : 0;
            if (common_utils.is_valid(res)){
                PlayerPrefs.SetInt(tutor_nm, res);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_art(art a){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(a.lvl)){
                PlayerPrefs.SetInt(a.nm, a.lvl);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_art_order(art a, int order){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(order)){
                PlayerPrefs.SetInt(a.nm + "_order", order);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_skin(skin s){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(s.state)){
                PlayerPrefs.SetInt(s.nm, s.state);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_sound_vol(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(urefs.sound_slider_sl.value)){
                PlayerPrefs.SetFloat("sound_vol", urefs.sound_slider_sl.value);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_music_vol(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(urefs.music_slider_sl.value)){
                PlayerPrefs.SetFloat("music_vol", urefs.music_slider_sl.value);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_prof(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(statics.mngr_prof.cur_prof_nm)){
                PlayerPrefs.SetString("cur_prof", statics.mngr_prof.cur_prof_nm);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_ach(ach a){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(a.val)
            && common_utils.is_valid(a.rewarded_cnt)
            && common_utils.is_valid(a.last_nofted_idx)){
                PlayerPrefs.SetFloat(a.nm + consts.achs_postfix, a.val);
                PlayerPrefs.SetInt(a.nm + consts.achs_rewarded_cnt_postfix, a.rewarded_cnt);
                PlayerPrefs.SetInt(a.nm + consts.achs_noft_idx_postfix, a.last_nofted_idx);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_tap(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(statics.mngr_tap.lvl)){
                PlayerPrefs.SetInt("tap", statics.mngr_tap.lvl);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_last_played_dttm(){
        if (!is_all_keys_deleted && is_saves_restored){
            string res = DateTime.Now.ToString("o");
            if (common_utils.is_valid(res)){
                PlayerPrefs.SetString("last_played_dttm", res);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_balance(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(statics.mngr_balance.amount)
            && common_utils.is_valid(statics.mngr_balance.max_amount)){
                PlayerPrefs.SetFloat("balance", statics.mngr_balance.amount);
                PlayerPrefs.SetFloat("max_balance", statics.mngr_balance.max_amount);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_gameover_status(){
        if (!is_all_keys_deleted && is_saves_restored){
            int res = statics.mngr_gameover.is_gameover ? 1 : 0;
            if (common_utils.is_valid(res)){
                PlayerPrefs.SetInt("is_gameover", res);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_gameover_ac_status(){
        if (!is_all_keys_deleted && is_saves_restored){
            int res = statics.mngr_gameover.is_gameover_anticlicker ? 1 : 0;
            if (common_utils.is_valid(res)){
                PlayerPrefs.SetInt("is_gameover_ac", res);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_tutor(string tutor_nm){
        if (!is_all_keys_deleted && is_saves_restored){
            int res = statics.mngr_tutor.tutor_state_dict[tutor_nm];
            if (common_utils.is_valid(res)){
                PlayerPrefs.SetInt(tutor_nm, res);
                PlayerPrefs.Save();
            }
        }
    }

    public static void save_quest(){
        if (!is_all_keys_deleted && is_saves_restored){
            if (common_utils.is_valid(statics.mngr_quests.cur_quest_nm)){
                PlayerPrefs.SetString("quest_nm", statics.mngr_quests.cur_quest_nm);
                PlayerPrefs.Save();
            }
        }
    }

    public static void delete_all(){
        if (!is_all_keys_deleted && is_saves_restored){
            is_all_keys_deleted = true;
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
