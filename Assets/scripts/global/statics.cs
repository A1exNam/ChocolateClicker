using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.Events;

public static class statics{
    public static class mngr_quests{
        public static string cur_quest_nm;
        public static bool is_reward_shown = false;

        public static void init(){
            cur_quest_nm = consts.idx_quests_mapping[0];
            act_ui();
        }

        //lvlup
        //on_tap
        //есть защита cur_quest_nm = null
        public static void recalc(string quest_nm){
            if (cur_quest_nm != quest_nm || is_reward_shown) return;
            act_ui();
            if (is_quest_completed()){
                mngr_settings.play_sound(consts.quest_completion_ac);
                is_reward_shown = true;
                logic_module.StartCoroutine(turn_off_and_go_next());
            }
        }

        public static IEnumerator turn_off_and_go_next(){
            yield return new WaitForSeconds(consts.reward_period);
            int next_quest_idx = consts.quests_data[cur_quest_nm].idx + 1;
            if (next_quest_idx < consts.quests_data.Count){
                cur_quest_nm = consts.idx_quests_mapping[next_quest_idx];
            } else {
                cur_quest_nm = "questline_finished";
            }
            save_module.save_quest();
            is_reward_shown = false;
            act_ui();
        }

        public static bool is_quest_completed(){
            var vals = get_val();
            return vals.Item1 >= vals.Item2;
        }

        public static Tuple<int, int> get_val(){  
            float val = -1f;
            switch (cur_quest_nm){
                case "quest_collect_1":
                case "quest_collect_2":
                    val = mngr_balance.max_amount;
                    break; 
                case "quest_lvl_1":
                case "quest_lvl_2":
                case "quest_lvl_3":
                    val = mngr_xp.max_lvl;
                    break;
            }  
            return new((int)val, consts.quests_data[cur_quest_nm].val);
        }

        //есть защита cur_quest_nm = null
        public static void act_ui(){
            if (cur_quest_nm == "questline_finished"){
                urefs.quest_go.SetActive(false);
                return;
            }
            urefs.quest_go.SetActive(true);
            if (!is_quest_completed()){
                urefs.quest_reward_go.SetActive(false);
                urefs.quest_progress_desc_txt.text = consts.quests_data[cur_quest_nm].desc;

                var vals = get_val();
                urefs.quest_progress_val_txt.text = vals.Item1.ToString() + "/" + vals.Item2.ToString();
                change_progress_bar((float)vals.Item1/vals.Item2);
 
                urefs.quest_progress_go.SetActive(true);
            } else {
                urefs.quest_progress_go.SetActive(false);
                urefs.quest_reward_txt.text = consts.quests_data[cur_quest_nm].reward;
                urefs.quest_reward_go.SetActive(true);
            }
        }

        public static void change_progress_bar(float val){
            Vector2 temp = urefs.quest_progress_bar_rt.anchoredPosition;
            temp.x = -urefs.quest_progress_bar_rt.sizeDelta.x * 0.85f * (1f - val);
            urefs.quest_progress_bar_rt.anchoredPosition = temp;
        }

        public static void init_after_restore(){  
            if (cur_quest_nm != "questline_finished" && is_quest_completed()){
                is_reward_shown = true;
                logic_module.StartCoroutine(turn_off_and_go_next());
            }
        }
    }
    public static class mngr_gameover{
        public static bool 
            is_gameover = false,
            is_gameover_anticlicker = false;

        public static void init(){
            urefs.restart_game_bcc.on_click.AddListener(restart);
        }

        public static void gameover(){
            is_gameover = true;
            act_ui();
            save_module.save_gameover_status();
        }

        public static void gameover_anticlicker(){
            is_gameover_anticlicker = true;
            act_ui();
            save_module.save_gameover_ac_status();
        }

        public static void act_ui(){
            if (is_gameover)
                urefs.gameover_screen_go.SetActive(true);
            if (is_gameover_anticlicker)
                urefs.gameover_ac_screen_go.SetActive(true);
        }

        public static void restart(){
            save_module.delete_all();
        }
    }


    public static class mngr_offline_reward{
        public static float 
            reward;
        
        public static bool
            can_save_last_played_dttm = true;

        public static void init(){
            urefs.offln_rwrd_bttn_claim_bcc.on_click.AddListener(
                () => get_reward()
            );
            urefs.offln_rwrd_bttn_claim_xtra_bcc.on_click.AddListener(
                () => logic_module.StartCoroutine(try_show_ad())
            );
            act_ui();
        }

        public static void act_ui(){
            urefs.offln_rwrd_text_txt.text = 
                common_utils.f2s(reward);
            statics.logic_module.StartCoroutine(
                common_utils.rebuild_layout(urefs.offln_rwrd_rt)
            );
            urefs.offln_rwrd_xtra_rwrd_bttn_txt.text = 
                common_utils.f2s(consts.offline_reward_m) 
                + "x Extra \nRewards!";
            statics.logic_module.StartCoroutine(
                common_utils.rebuild_layout(urefs.offln_rwrd_bttn_xtra_rt)
            );
        }

        public static IEnumerator try_show_ad(){
            sdk_common.start_ad();
            float elapsed_time = 0f;
            while (!sdk_common.is_ad_watched && elapsed_time < consts.ad_fail_timeout 
            && !sdk_common.is_ad_error){
                yield return null;
            }

            if (sdk_common.is_ad_watched){
                get_reward(consts.offline_reward_m);
                sdk_common.is_ad_watched = false;
            } else if (sdk_common.is_ad_error){
                urefs.offln_rwrd_bttn_xtra_go.SetActive(false);
                yield return logic_module.StartCoroutine(sdk_common.start_ad_error_lbl());
                sdk_common.is_ad_error = false;
            }
        }

        public static void get_reward(float m = 1f){
            mngr_balance.amount += (float)Math.Truncate(reward * m); 
            mngr_balance.on_val_change();

            mngr_settings.play_sound(consts.currency);
            urefs.offln_rwrd_w_go.SetActive(false);
            urefs.coin_get_rwrd_anmtr.Play("start");

            can_save_last_played_dttm = true;
        }
    }

    public static class mngr_tutor{
        public static Dictionary<string, (TextMeshProUGUI min_lvl_txt, GameObject min_lvl_go, 
        CanvasGroup bttn_cg, int min_lvl, Animator bttn_anmtr)> bttn_tutor_dict_collection;

        public static Dictionary<string, (GameObject go, button_custom_class bcc, Animator anmtr, TextMeshProUGUI txt)> tutor_go_bcc_dict;
        public static Dictionary<string, int> tutor_state_dict = new();

        public static Dictionary<string, bool>
            is_opened_dict = new(); //bttns

        public static void init(){
            bttn_tutor_dict_collection = new(){
                {"prof_bttn_tutor", (urefs.open_prof_win_bttn_min_lvl_txt, urefs.open_prof_win_bttn_min_lvl_go, 
                urefs.open_prof_win_bttn_cg, consts.open_prof_win_min_lvl, urefs.open_prof_win_bttn_anmtr)},
                {"skins_bttn_tutor", (urefs.open_skins_win_bttn_min_lvl_txt, urefs.open_skins_win_bttn_min_lvl_go, 
                urefs.open_skins_win_bttn_cg, consts.open_skins_win_min_lvl, urefs.open_skins_win_bttn_anmtr)},
                {"tempering_bttn_tutor", (urefs.open_tmprng_win_bttn_min_lvl_txt, urefs.open_tmprng_win_bttn_min_lvl_go, 
                urefs.open_tmprng_win_bttn_cg, consts.open_tmprng_win_min_lvl, urefs.open_tmprng_win_bttn_anmtr)},
                {"arts_bttn_tutor", (urefs.open_arts_panel_bttn_min_lvl_txt, urefs.open_arts_panel_bttn_min_lvl_go, 
                urefs.open_arts_panel_bttn_cg, consts.open_arts_panel_min_lvl, urefs.open_arts_panel_bttn_anmtr)},
            };
            
            foreach (var bttn_nm in bttn_tutor_dict_collection.Keys){
                is_opened_dict[bttn_nm] = false;
            }

            act_ui(new(bttn_tutor_dict_collection.Keys));

            tutor_go_bcc_dict = new(){
                {"tutor_click", (urefs.tutor_click_go, urefs.tutor_click_bcc, urefs.tutor_click_anmtr, urefs.tutor_click_txt)},
                {"tutor_tap", (urefs.tutor_tap_go, urefs.tutor_tap_bcc, urefs.tutor_tap_anmtr, urefs.tutor_tap_txt)},                
                {"tutor_upgr", (urefs.tutor_upgr_go, urefs.tutor_upgr_bcc, urefs.tutor_upgr_anmtr, urefs.tutor_upgr_txt)},
                {"tutor_arts", (urefs.tutor_arts_panel_bttn_go, urefs.tutor_arts_panel_bttn_bcc, urefs.tutor_arts_panel_bttn_anmtr, urefs.tutor_arts_panel_bttn_txt)},
                {"tutor_arts_discover", (urefs.tutor_arts_discover_go, urefs.tutor_arts_discover_bcc, urefs.tutor_arts_discover_anmtr, urefs.tutor_arts_discover_txt)},
                {"tutor_tempering_win_bttn", (urefs.tutor_tempering_win_bttn_go, urefs.tutor_tempering_win_bttn_bcc, urefs.tutor_tempering_win_bttn_anmtr, urefs.tutor_tempering_win_bttn_txt)},
                {"tutor_tempering", (urefs.tutor_tempering_go, urefs.tutor_tempering_bcc, urefs.tutor_tempering_anmtr, urefs.tutor_tempering_txt)},
                {"tutor_prof_win_bttn", (urefs.tutor_prof_win_bttn_go, urefs.tutor_prof_win_bttn_bcc, urefs.tutor_prof_win_bttn_anmtr, urefs.tutor_prof_win_bttn_txt)},
                {"tutor_prof", (urefs.tutor_prof_go, null, urefs.tutor_prof_anmtr, urefs.tutor_prof_txt)},
                {"tutor_skin_win_bttn", (urefs.tutor_skin_win_bttn_go, urefs.tutor_skin_win_bttn_bcc, urefs.tutor_skin_win_bttn_anmtr, urefs.tutor_skin_win_bttn_txt)},
                {"tutor_ach_win_bttn", (urefs.tutor_ach_win_bttn_go, urefs.tutor_ach_win_bttn_bcc, urefs.tutor_ach_win_bttn_anmtr, urefs.tutor_ach_win_bttn_txt)},
                {"tutor_diamonds", (urefs.tutor_diamonds_go, urefs.tutor_diamonds_bcc, urefs.tutor_diamonds_anmtr, urefs.tutor_diamonds_txt)}
            };

            foreach (string tutor_nm in tutor_go_bcc_dict.Keys){
                tutor_state_dict[tutor_nm] = 0;
            }

            tutor_go_bcc_dict["tutor_click"].bcc.on_click.AddListener(mngr_tap.on_tap);
            tutor_go_bcc_dict["tutor_tap"].bcc.on_click.AddListener(mngr_tap.try_buy);
            tutor_go_bcc_dict["tutor_upgr"].bcc.on_click.AddListener(mngr_upgrs.upgrs_dict[consts.upgrs_sorted_list[0]].try_buy);
            tutor_go_bcc_dict["tutor_arts"].bcc.on_click.AddListener(urefs.shop_arts_bt_bcc.on_click.Invoke);
            tutor_go_bcc_dict["tutor_arts_discover"].bcc.on_click.AddListener(mngr_discover.try_discover_art);
            tutor_go_bcc_dict["tutor_tempering_win_bttn"].bcc.on_click.AddListener(mngr_tempering.open_win);
            tutor_go_bcc_dict["tutor_tempering"].bcc.on_click.AddListener(
                () => {
                    statics.mngr_settings.play_sound(consts.reveal_bttn_from_lock_ac);
                    mngr_tutor.try_close_tutor("tutor_tempering");
                }
            );
            tutor_go_bcc_dict["tutor_prof_win_bttn"].bcc.on_click.AddListener(mngr_prof.try_open_win);
            tutor_go_bcc_dict["tutor_skin_win_bttn"].bcc.on_click.AddListener(mngr_skins.open_win);
            tutor_go_bcc_dict["tutor_ach_win_bttn"].bcc.on_click.AddListener(mngr_achs.open_win);
            tutor_go_bcc_dict["tutor_diamonds"].bcc.on_click.AddListener(
                () => {
                    statics.mngr_settings.play_sound(consts.reveal_bttn_from_lock_ac);
                    mngr_tutor.try_close_tutor("tutor_diamonds");
                }
            );
        }

        public static void init_after_restore(){
            try_show_tutor("tutor_click");
        }

		public static void try_show_tutor(string tutor_nm){
			if (tutor_state_dict[tutor_nm] == 0){
                mngr_settings.apply_tutor_music_duck();
				tutor_go_bcc_dict[tutor_nm].go.SetActive(true);
                if (tutor_go_bcc_dict[tutor_nm].txt){
                    UnityAction action_temp = null;
                    if (tutor_nm == "tutor_tempering") 
                        action_temp = () => urefs.tutor_tempering_bttn_go.SetActive(true);
                    else if (tutor_nm == "tutor_diamonds")
                        action_temp = () => urefs.tutor_diamonds_bttn_go.SetActive(true);
                    logic_module.StartCoroutine(
                        common_utils.retypewrite_with_postaction(
                            tutor_go_bcc_dict[tutor_nm].txt,
                            () => !tutor_go_bcc_dict[tutor_nm].go.activeSelf,
                            action_temp
                        )
                    );
                }
				tutor_state_dict[tutor_nm] = 1;
			}
		}

        public static void try_close_tutor(string tutor_nm){
            List<string> tutor_list = new();
            if (tutor_nm == "all"){
                foreach (string key in tutor_go_bcc_dict.Keys){
                    tutor_list.Add(key);
                }
            } else {
                tutor_list.Add(tutor_nm);
            }

            bool at_least_one_closed = false;
            
            foreach (string nm in tutor_list){
                if (tutor_state_dict[nm] == 1){
                    tutor_state_dict[nm] = 2;
                    logic_module.StartCoroutine(deact_tutor_after_delay(nm));
                    save_module.save_tutor(nm);
                    at_least_one_closed = true;
                }
            }

            if (at_least_one_closed)
                mngr_settings.release_tutor_music_duck();
        }

        public static IEnumerator deact_tutor_after_delay(string tutor_nm){
            tutor_go_bcc_dict[tutor_nm].anmtr.Play("disappear");
            yield return common_utils.wait_until_state_end(tutor_go_bcc_dict[tutor_nm].anmtr, "disappear");
            tutor_go_bcc_dict[tutor_nm].go.SetActive(false);
        }
        
        public static void act_ui(HashSet<string> modes){
            foreach (var mode in modes){
                var bttn_tutor = bttn_tutor_dict_collection[mode];
                if (mngr_xp.max_lvl < bttn_tutor.min_lvl){
                    bttn_tutor.bttn_cg.alpha = consts.bttn_tutor_min_alpha;
                    bttn_tutor.min_lvl_txt.text = "Require " + bttn_tutor.min_lvl + " lvl";
                    bttn_tutor.min_lvl_go.SetActive(true);
                } else {
                    bttn_tutor.bttn_cg.alpha = 1f;
                    bttn_tutor.min_lvl_go.SetActive(false);
                }
            }
        }

        public static void try_show_opening_bttns(){
            foreach (var bttn_tutor_keyval in bttn_tutor_dict_collection){
                if (mngr_xp.max_lvl >= bttn_tutor_keyval.Value.min_lvl 
                && bttn_tutor_keyval.Value.min_lvl_go.activeSelf){
                    act_ui(new(){bttn_tutor_keyval.Key});
                    bttn_tutor_keyval.Value.bttn_anmtr.Play("start_show");
                    is_opened_dict[bttn_tutor_keyval.Key] = true; 
                    save_module.save_bttn_tutor(bttn_tutor_keyval.Key);
                    mngr_settings.play_sound(consts.reveal_bttn_from_lock_ac);
                }
            }
        }
    }

    public static class mngr_ad_bttn{
        public static bool 
            is_bonus_active = false,
            is_cd = false;

        public static void init(){
            urefs.ad_button_bcc.on_click.AddListener(try_show_ad);
        }

        public static void init_after_save_restore(){
            //logic_module.StartCoroutine(main_loop()); //basic_implementation
        }

        public static void try_show_ad(){
            if (!is_bonus_active && !is_cd){
                sdk_common.start_ad();
            }
        }

        public static IEnumerator start_ad_timer(){
            urefs.ad_button_anmtr.enabled = false;
            urefs.ad_button_rt.localRotation = Quaternion.identity;
            is_bonus_active = true;
            urefs.ad_timer_go.SetActive(true); 
            urefs.ad_bttn_lifetime_all_bar_go.SetActive(false);

            recalcs.recalc_gps_m();
            recalcs.recalc_tap_m();

            float 
                duration_temp = consts.ad_bonus_duration,
                elapsedTime_temp = 0f;

                while (elapsedTime_temp < duration_temp){
                    elapsedTime_temp += Time.deltaTime;
                    urefs.ad_timer_txt.text = "00:" + ((int)(duration_temp - elapsedTime_temp)).ToString("D2");
                    yield return null;
                }
            
            is_bonus_active = false;

            recalcs.recalc_gps_m();
            recalcs.recalc_tap_m();

            urefs.ad_button_anmtr.enabled = true;
            urefs.ad_timer_go.SetActive(false); 
            urefs.ad_bttn_lifetime_all_bar_go.SetActive(true);
            sdk_common.is_ad_watched = false;
        }

        public static IEnumerator ad_cd(){
            is_cd = true;

            float 
                elapsedTime_temp = 0f;

            while (elapsedTime_temp < consts.ad_cd){
                elapsedTime_temp += Time.deltaTime;
                yield return null;
            }

            is_cd = false;
        }

        public static IEnumerator main_loop(){
            float elapsed;
            urefs.ad_button_go.SetActive(false);
            yield return new WaitForSeconds(consts.ad_first_delay);
            while (true){
                urefs.ad_button_go.SetActive(true);
                urefs.ad_button_anmtr.Play("expand", 1);
                yield return common_utils.wait_until_state_end(
                    urefs.ad_button_anmtr,
                    "expand",
                    1
                );

                elapsed = 0f;
                while (elapsed < consts.ad_max_show_interval 
                && !sdk_common.is_ad_watched && !sdk_common.is_ad_error){
                    urefs.ad_bttn_lifetime_bar_im.fillAmount = 
                        1 - elapsed/consts.ad_max_show_interval;
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (sdk_common.is_ad_watched){
                    yield return start_ad_timer();
                } else {
                    urefs.ad_button_anmtr.Play("shrink", 1);
                    yield return common_utils.wait_until_state_end(
                        urefs.ad_button_anmtr,
                        "shrink",
                        1
                    );
                    if (sdk_common.is_ad_error){
                        logic_module.StartCoroutine(sdk_common.start_ad_error_lbl());
                        sdk_common.is_ad_error = false;
                    }
                }
                urefs.ad_button_go.SetActive(false);
                urefs.ad_bttn_lifetime_bar_im.fillAmount = 1f;

                yield return ad_cd();
            }
        }
    }

    public static class mngr_tempering{
        public static float
            cb_reward_amount_f,
            cb_reward_m;

        static float calc_reward_for_lvl(int lvl){
            if (lvl < consts.min_temper_lvl){
                return 0f;
            }
            return (float)Math.Truncate(
                (float)Math.Pow(
                    consts.tempering_base_reward,
                    lvl - consts.min_temper_lvl + 1
                )
                * cb_reward_m
            );
        }

        public static void init(){
            urefs.open_temper_win_bcc.on_click.AddListener(open_win);
            urefs.tempering_reward_bttn_bcc.on_click.AddListener(
                () => logic_module.StartCoroutine(try_tempering())
            );
            recalcs.recalc_tempering_reward_m();
            recalcs.recalc_tempering_reward_amount();
            act_ui(new(){"bttn_alpha"});
        }

        public static void open_win(){
            if (mngr_tutor.is_opened_dict["tempering_bttn_tutor"]){
                sdk_common.gp_stop();
                urefs.temper_window_go.SetActive(true);

                mngr_tutor.try_close_tutor("tutor_tempering_win_bttn");
                mngr_tutor.try_show_tutor("tutor_tempering");
                mngr_tutor.try_close_tutor("tutor_diamonds");
                mngr_settings.play_sound(consts.open_win_ac);
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static void act_ui(HashSet<string> modes){
            bool 
                act_amount = modes.Contains("amount"),
                act_bttn_alpha = modes.Contains("bttn_alpha");
            
            if (act_amount){
                urefs.tempering_cb_cnt_txt.text = "+"
                    + common_utils.f2s(cb_reward_amount_f);
                var next_lvl_reward = calc_reward_for_lvl(mngr_xp.lvl + 1);
                var diff_with_next_lvl = next_lvl_reward - cb_reward_amount_f;
                if (diff_with_next_lvl > 0){
                    urefs.forecast_cb_txt.text = "(+" + common_utils.f2s(diff_with_next_lvl)
                    + " cacao beans more next level!)";
                } else {
                    urefs.forecast_cb_txt.text = "";
                }
            }
            if (act_bttn_alpha){
                if (mngr_xp.lvl < consts.min_temper_lvl){
                    urefs.tempering_reward_bttn_cg.alpha = consts.bttn_min_alpha;
                    urefs.tempering_reward_bttn_txt.text = "Require " + consts.min_temper_lvl + " lvl";
                } else {
                    urefs.tempering_reward_bttn_cg.alpha = 1f;
                    urefs.tempering_reward_bttn_txt.text = "Temper";
                }
            }
        }

        public static IEnumerator try_tempering(){
            if (mngr_xp.lvl >= consts.min_temper_lvl){
                mngr_tutor.try_close_tutor("tutor_tempering");
                urefs.tempering_flash_go.SetActive(true);
                mngr_settings.play_sound(consts.tempering_ac);
                yield return common_utils.wait_until_state_end(
                    urefs.tempering_flash_anmtr,
                    "enabling"
                );
                float 
                    amount_temp = mngr_cb.amount,
                    reward_temp = cb_reward_amount_f;

                mngr_cb.amount += cb_reward_amount_f;
                mngr_cb.on_val_change();

                mngr_tap.clear_tap();
                mngr_xp.clear_xp();
                mngr_upgrs.clear_upgrs();
                mngr_balance.clear_balance();
                mngr_tempering.act_ui(new(){"bttn_alpha"});
                recalcs.recalc_tempering_reward_amount();

                urefs.temper_window_go.SetActive(false);

                logic_module.StartCoroutine(
                    show_reward_tempering_panel(amount_temp, amount_temp + reward_temp)
                );

                yield return common_utils.wait_until_state_end(
                    urefs.tempering_flash_anmtr,
                    "disabling"
                );
                urefs.tempering_flash_go.SetActive(false);
                sdk_common.gp_start();

                mngr_tutor.try_show_tutor("tutor_arts");
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static IEnumerator show_reward_tempering_panel(float a, float b){
            urefs.shw_rwrd_tmprng_panel_go.SetActive(true);
            common_utils.set_active_all_child(urefs.shw_rwrd_tmprng_panel_go, true);
            urefs.shw_rwrd_tmprng_panel_csf.enabled = true;
            yield return common_utils.change_number_smoothly(
                urefs.shw_rwrd_tmprng_text_txt, 
                consts.shw_rwrd_tmprng_number_change_period,
                a,
                b,
                urefs.shw_rwrd_tmprng_panel_rt
            );
            yield return new WaitForSeconds(consts.shw_rwrd_tmprng_delay_after);
            common_utils.set_active_all_child(urefs.shw_rwrd_tmprng_panel_go, false);
            urefs.shw_rwrd_tmprng_panel_csf.enabled = false;
            yield return common_utils.width_shrink(
                urefs.shw_rwrd_tmprng_panel_rt, 
                consts.shw_rwrd_tmprng_shrink_period
            );
            urefs.shw_rwrd_tmprng_panel_go.SetActive(false);
        }

        public static void init_after_save_restore(){
            recalcs.recalc_tempering_reward_amount();
            act_ui(new(){"bttn_alpha"});
        }

    }

    public static class mngr_skins{
        public static string active_skin_nm; //имя активного скина
        public static List<skin> skins_list = new();
        public static skin cur_skin = null;
        public static Dictionary<string, skin> skins_dict = new();

        public static void init(){
            foreach (var skin_nm in consts.skins_order){ 
                skin s = new(skin_nm);
                skins_list.Add(s);
                skins_dict[skin_nm] = s;
            }
            cur_skin = skins_list[0];
            cur_skin.state = 2;
            cur_skin.act_ui(new(){"all_except_alpha"});  

            recalcs.recalc_skins_state(skins_list); //for revealing start skins
            urefs.open_skin_win_bcc.on_click.AddListener(open_win);
        }

        public static void open_win(){
            if (mngr_tutor.is_opened_dict["skins_bttn_tutor"]){
                mngr_tutor.try_close_tutor("tutor_skin_win_bttn");
                mngr_tutor.try_close_tutor("tutor_diamonds");
                sdk_common.gp_stop();
                urefs.skins_window_go.SetActive(true);
                foreach (var s in skins_list){
                    if (s.state != -1){
                        logic_module.StartCoroutine(common_utils.rebuild_layout(s.skin_references.button_rt));
                    }
                }

                mngr_settings.play_sound(consts.open_win_ac);
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static void act_ui(HashSet<string> modes){
            bool 
                act_bttn_alpha = modes.Contains("bttn_alpha");
            
            if (act_bttn_alpha){
                foreach (var s in skins_list){
                    s.act_ui(new(){"bttn_alpha"});
                }
            }
        }
    }

    public static class mngr_cb{
        public static float 
            amount = 0f;

        public static void init(){
            act_ui();
        }

        public static void on_val_change(){
            act_ui();
            mngr_arts.act_ui(new(){"bttn_alpha"});

            var ach_cb_temp = mngr_achs.achs_dict["cacao beans"];
            if (amount > ach_cb_temp.val){
                ach_cb_temp.val = amount;
                ach_cb_temp.on_val_change();
            }

            save_module.save_cb();
        }

        public static void act_ui(){
            urefs.cb_amount_text_txt.text = common_utils.f2s(amount);
        }
    }

    public static class mngr_diamonds{
        public static int 
            amount = 0;

        public static void init(){
            act_ui();
        }

        public static void on_val_change(){
            mngr_skins.act_ui(new(){"bttn_alpha"});
            mngr_prof.act_ui(new(){"bttn_reset_alpha"});
            act_ui();
            save_module.save_diamonds();
        }

        public static void act_ui(){
            urefs.diamonds_text_txt.text = amount.ToString();
            statics.logic_module.StartCoroutine(common_utils.rebuild_layout(urefs.diamonds_panel_rt));
        }
    }

    public static class mngr_balance{
        public static float
            amount = 0f,
            max_amount = 0f;

        static Coroutine gps_income_coroutine;

        public static void init(){
            act_ui();
            start_gps_income_loop();
        }

        public static void on_val_change(){
            if (amount > max_amount) max_amount = amount;
            mngr_tap.act_ui(new(){"alpha"});
            mngr_upgrs.act_ui(new(){"alpha"});
            act_ui();

            if (gps_income_coroutine == null)
                start_gps_income_loop();
        }

        public static void ensure_gps_income_loop(){
            if (gps_income_coroutine == null)
                start_gps_income_loop();
        }

        static void start_gps_income_loop(){
            if (statics.logic_module == null)
                return;

            if (gps_income_coroutine != null)
                statics.logic_module.StopCoroutine(gps_income_coroutine);

            gps_income_coroutine = statics.logic_module.StartCoroutine(gps_income_loop());
        }

        static IEnumerator gps_income_loop(){
            WaitForSeconds wait = new WaitForSeconds(1f);

            while (true){
                yield return wait;

                float gps_value = statics.mngr_upgrs.gps;
                if (gps_value <= 0f){
                    if (urefs.balance_gain_txt_txt != null){
                        GameObject gain_go = urefs.balance_gain_txt_txt.gameObject;
                        if (gain_go.activeSelf)
                            gain_go.SetActive(false);
                    }
                    continue;
                }

                float gps_to_display = (float)Math.Truncate(gps_value);
                if (urefs.balance_gain_txt_txt != null){
                    urefs.balance_gain_txt_txt.text =
                        "+" + common_utils.f2s(gps_to_display);
                }

                GameObject gain_go_temp = urefs.balance_gain_txt_txt != null
                    ? urefs.balance_gain_txt_txt.gameObject
                    : null;

                if (gain_go_temp != null){
                    gain_go_temp.SetActive(true);
                }

                if (urefs.balance_gain_txt_anmtr != null){
                    yield return common_utils.wait_until_state_end(urefs.balance_gain_txt_anmtr, "appear");
                } else {
                    yield return null;
                }

                amount += gps_value;
                on_val_change();

                mngr_settings.play_sound(consts.gain_gps_ac);

                float gps_xp = gps_value * consts.xp_gps_fraction;
                if (gps_xp > 1f){
                    mngr_xp.add((float)Math.Truncate(gps_xp));
                    mngr_xp.act_ui();
                }

                yield return common_utils.wait_until_state_end(urefs.balance_gain_txt_anmtr, "disappear");
                gain_go_temp.SetActive(false);
            }
        }

        public static void clear_balance(){
            amount = 0f;
            on_val_change();
        }

        public static void act_ui(){
            urefs.balance_text_txt.text = common_utils.f2s((float)Math.Truncate(amount));
        }
    }

    public static class mngr_golden_chocolate{
        static Coroutine spawn_coroutine;
        static float time_since_last_spawn;
        static float next_spawn_delay;

        public static void init(){
            if (statics.logic_module == null)
                return;
            if (consts.golden_chocolate_pf == null || urefs.goldenchocozone_tr == null)
                return;

            time_since_last_spawn = 0f;
            next_spawn_delay = get_next_delay();

            if (spawn_coroutine != null)
                statics.logic_module.StopCoroutine(spawn_coroutine);
            spawn_coroutine = statics.logic_module.StartCoroutine(spawn_loop());
        }

        static IEnumerator spawn_loop(){
            while (true){
                time_since_last_spawn += Time.deltaTime;
                if ((next_spawn_delay > 0f && time_since_last_spawn >= next_spawn_delay)
                    || time_since_last_spawn >= consts.golden_spawn_pity){
                    yield return spawn_wave();
                    time_since_last_spawn = 0f;
                    next_spawn_delay = get_next_delay();
                }
                yield return null;
            }
        }

        static IEnumerator spawn_wave(){
            int wave_count = consts.golden_spawn_wave_count;
            if (wave_count <= 0)
                yield break;

            spawn_single();

            if (wave_count <= 1)
                yield break;

            float min_delay = consts.golden_spawn_wave_min_delay;
            float max_delay = consts.golden_spawn_wave_max_delay;

            if (max_delay < min_delay){
                float temp = min_delay;
                min_delay = max_delay;
                max_delay = temp;
            }

            for (int i = 1; i < wave_count; i++){
                float delay = UnityEngine.Random.Range(min_delay, max_delay);
                yield return new WaitForSeconds(delay);
                spawn_single();
            }
        }

        static void spawn_single(){
            if (consts.golden_chocolate_pf == null || urefs.goldenchocozone_tr == null)
                return;

            RectTransform parent_rect = urefs.goldenchocozone_tr as RectTransform;
            if (parent_rect == null)
                return;

            GameObject go = UnityEngine.Object.Instantiate(consts.golden_chocolate_pf, parent_rect);
            go.transform.SetAsLastSibling();

            RectTransform rect = go.transform as RectTransform;
            if (rect == null)
                return;

            golden_chocolate_behaviour behaviour = go.GetComponent<golden_chocolate_behaviour>();
            if (behaviour == null)
                behaviour = go.AddComponent<golden_chocolate_behaviour>();

            float half_width = rect.rect.width * rect.localScale.x * 0.5f;
            float half_height = rect.rect.height * rect.localScale.y * 0.5f;
            float half_diagonal = Mathf.Sqrt(half_width * half_width + half_height * half_height);

            float left_edge = -parent_rect.rect.width * parent_rect.pivot.x;
            float right_edge = parent_rect.rect.width * (1f - parent_rect.pivot.x);
            float bottom_edge = -parent_rect.rect.height * parent_rect.pivot.y;
            float top_edge = parent_rect.rect.height * (1f - parent_rect.pivot.y);

            float spawn_x = left_edge - half_width;
            float min_y = bottom_edge + half_diagonal;
            float max_y = top_edge - half_diagonal;
            float spawn_y = (max_y > min_y)
                ? UnityEngine.Random.Range(min_y, max_y)
                : (min_y + max_y) * 0.5f;
            rect.anchoredPosition = new(spawn_x, spawn_y);

            float destroy_x = right_edge + half_width;
            behaviour.Initialize(parent_rect, destroy_x, consts.golden_chocolate_move_speed, consts.golden_chocolate_rotation_speed);
        }

        static float get_next_delay(){
            float mean = Mathf.Max(consts.golden_spawn_mean_delay, 0.1f);
            float rand = Mathf.Clamp(UnityEngine.Random.value, 1e-3f, 0.999f);
            return -Mathf.Log(rand) * mean;
        }
    }

    public static class mngr_prof{
        public static string
            cur_prof_nm;
        public static void init(){
            //пересчет параметров не производится
            cur_prof_nm = consts.start_prof_nm;
            act_ui(new(){"win_bttn", "prof_w", "reset", "bttn_reset_alpha"});

            urefs.prof_window_l_bcc.on_click.AddListener(
                () => logic_module.StartCoroutine(try_up_prof(consts.profs_data[cur_prof_nm].childs.l))
            );
            urefs.prof_window_r_bcc.on_click.AddListener(
                () => logic_module.StartCoroutine(try_up_prof(consts.profs_data[cur_prof_nm].childs.r))
            );
            urefs.reset_prof_bt_bcc.on_click.AddListener(
                () => logic_module.StartCoroutine(try_reset_prof())
            );

            urefs.prof_window_l_bcc.on_enter.AddListener(
                () => {urefs.prof_window_l_im.color = consts.prof_entered_clr;}
            );
            urefs.prof_window_l_bcc.on_exit.AddListener(
                () => {urefs.prof_window_l_im.color = consts.prof_exited_clr;}
            );
            urefs.prof_window_r_bcc.on_enter.AddListener(
                () => {urefs.prof_window_r_im.color = consts.prof_entered_clr;}
            );
            urefs.prof_window_r_bcc.on_exit.AddListener(
                () => {urefs.prof_window_r_im.color = consts.prof_exited_clr;}
            );
            urefs.open_prof_win_bcc.on_click.AddListener(try_open_win);
            urefs.open_prof_reset_win_bcc.on_click.AddListener(try_open_reset_win);
        }

        public static void try_open_win(){
            if (mngr_tutor.is_opened_dict["prof_bttn_tutor"]){
                sdk_common.gp_stop();
                urefs.prof_window_go.SetActive(true);

                mngr_tutor.try_close_tutor("tutor_prof_win_bttn");
                mngr_tutor.try_close_tutor("tutor_diamonds");
                mngr_tutor.try_show_tutor("tutor_prof");
                mngr_settings.play_sound(consts.open_win_ac);
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static void try_open_reset_win(){
            urefs.prof_reset_win_go.SetActive(true);
            mngr_settings.play_sound(consts.change_shop_ac);
        }

        public static IEnumerator try_reset_prof(){
            int reset_price_temp = 
                consts.prof_grade_lvl_mapping[consts.profs_data[cur_prof_nm].grade].reset_price;
            if (statics.mngr_diamonds.amount >= reset_price_temp){
                statics.mngr_diamonds.amount -= reset_price_temp;
                statics.mngr_diamonds.on_val_change();

                urefs.prof_change_flash_go.SetActive(true);
                mngr_settings.play_sound(consts.change_ac);
                yield return common_utils.wait_until_state_end(
                    urefs.prof_change_flash_anmtr, 
                    "start_flash"
                );
                string old_prof_nm = cur_prof_nm;
                cur_prof_nm = consts.start_prof_nm;
                change_params_for_ps(old_prof_nm);
                change_params_for_ps(cur_prof_nm);
                act_ui(new(){"win_bttn", "prof_w", "reset", "bttn_reset_alpha"});

                urefs.prof_reset_win_go.SetActive(false);

                yield return common_utils.wait_until_state_end(
                    urefs.prof_change_flash_anmtr, 
                    "end_flash"
                );
                urefs.prof_change_flash_go.SetActive(false);

                save_module.save_prof();
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static IEnumerator try_up_prof(string target_prof_nm){
            var temp = consts.profs_data[cur_prof_nm];
            if (temp.grade < consts.prof_grade_lvl_mapping.Keys.Max()
            && mngr_xp.max_lvl >= consts.prof_grade_lvl_mapping[temp.grade + 1].lvl){
                mngr_tutor.try_close_tutor("tutor_prof");

                urefs.prof_change_flash_go.SetActive(true); //Start Anim Flash
                mngr_settings.play_sound(consts.change_ac);
                yield return common_utils.wait_until_state_end(
                    urefs.prof_change_flash_anmtr, 
                    "start_flash"
                );
                string old_prof_nm = cur_prof_nm;
                cur_prof_nm = target_prof_nm;            
                change_params_for_ps(old_prof_nm);
                change_params_for_ps(cur_prof_nm);
                act_ui(new(){"win_bttn", "prof_w", "reset", "bttn_reset_alpha"});
                yield return common_utils.wait_until_state_end(
                    urefs.prof_change_flash_anmtr,
                    "end_flash"
                );
                urefs.prof_change_flash_go.SetActive(false);

                save_module.save_prof();
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static List<float> get_fval_ps(string prof_nm = null){
            prof_nm ??= cur_prof_nm;
            List<(string, float)> temp = consts.profs_data[prof_nm].val_ps;
            List<float> res = new();
            foreach (var (format, value) in temp){
                float temp1 = value;
                switch (format){
                    case "f":
                    case "a1":
                    case "p":
                        if (statics.mngr_arts.opened_arts_dict.TryGetValue("Smooth Gear", out var art1)){
                            temp1 *= art1.str;
                        }
                        break;
                }
                res.Add(temp1);
            }
            return res;
        }

        public static List<float> get_fval_ps_ui(string prof_nm){
            List<float> res = new();
            List<float> temp1 = get_fval_ps(prof_nm);
            float temp2;
            string temp3;
            for (int i=0; i<temp1.Count; i++){
                temp3 = consts.profs_data[prof_nm].val_ps[i].format;
                temp2 = temp1[i];
                if (temp3 == "f" || temp3 == "p"){
                    temp2 *= 100;
                }
                res.Add(temp2);
            }
            return res;
        }

        public static void act_ui(HashSet<string> modes){
            bool
                act_win_bttn = modes.Contains("win_bttn"),
                act_prof_window = modes.Contains("prof_w"),
                act_reset = modes.Contains("reset"),
                act_reset_alpha = modes.Contains("bttn_reset_alpha");

            var temp = consts.profs_data[cur_prof_nm];

            if (act_win_bttn) {
                urefs.prof_nm_bt_txt.text = cur_prof_nm;
                statics.logic_module.StartCoroutine(common_utils.rebuild_layout(urefs.prof_nm_bt_rt));
                urefs.prof_image_im.sprite = Resources.Load<Sprite>("images/" + cur_prof_nm);
            }
            if (act_prof_window){
                //panel up
                urefs.prof_window_lvl_text_txt.text = "Max Level " + mngr_xp.max_lvl.ToString();
                urefs.prof_window_prof_nm_text_txt.text = cur_prof_nm;
                if (temp.grade == consts.prof_grade_lvl_mapping.Keys.Max()){
                    urefs.prof_window_state_text_txt.text = "The peak has been reached.";
                } else if (consts.prof_grade_lvl_mapping[temp.grade + 1].lvl <= mngr_xp.max_lvl) {
                    urefs.prof_window_state_text_txt.text = "Choose a profession";
                } else {
                    urefs.prof_window_state_text_txt.text = "Next profession at "
                        + consts.prof_grade_lvl_mapping[temp.grade + 1].lvl + " lvl";
                }

                //panel down 
                if (temp.grade < consts.prof_grade_lvl_mapping.Keys.Max() 
                && mngr_xp.max_lvl >= consts.prof_grade_lvl_mapping[temp.grade + 1].lvl){
                    // left/right
                    var temp1 = consts.profs_data[temp.childs.l];
                    var temp2 = consts.profs_data[temp.childs.r];

                    urefs.prof_window_l_nm_txt.text = temp.childs.l;
                    urefs.prof_window_r_nm_txt.text = temp.childs.r;

                    urefs.prof_window_l_desc_text_txt.text = temp1.title;
                    urefs.prof_window_r_desc_text_txt.text = temp2.title;                    

                    urefs.prof_window_l_desc_ps_text_txt.text =
                        string.Format(temp1.ps_desc,
                            common_utils.f2s(
                                get_fval_ps_ui(temp.childs.l)
                            ).Cast<object>().ToArray()
                        );
                    urefs.prof_window_r_desc_ps_text_txt.text =
                        string.Format(temp2.ps_desc, 
                            common_utils.f2s(
                                get_fval_ps_ui(temp.childs.r)
                            ).Cast<object>().ToArray()
                        );

                    urefs.prof_window_l_image_im.sprite = Resources.Load<Sprite>("images/" + temp.childs.l);
                    urefs.prof_window_r_image_im.sprite = Resources.Load<Sprite>("images/" + temp.childs.r);

                    urefs.prof_window_cur_go.SetActive(false);
                    urefs.prof_window_l_go.SetActive(true);
                    urefs.prof_window_r_go.SetActive(true);
                } else {
                    urefs.prof_window_cur_nm_txt.text = cur_prof_nm;
                    urefs.prof_window_cur_desc_text_txt.text = temp.title;
                    urefs.prof_window_cur_desc_ps_text_txt.text =
                        string.Format(temp.ps_desc,
                            common_utils.f2s(
                                get_fval_ps_ui(cur_prof_nm)
                            ).Cast<object>().ToArray()
                        );
                    urefs.prof_window_cur_image_im.sprite = Resources.Load<Sprite>("images/" + cur_prof_nm);

                    urefs.prof_window_cur_go.SetActive(true);
                    urefs.prof_window_l_go.SetActive(false);
                    urefs.prof_window_r_go.SetActive(false);
                }
            }
            if (act_reset){
                //reset panel
                if (temp.grade == 0){
                    urefs.prof_window_reset_bt_go.SetActive(false);
                } else {
                    urefs.prof_window_reset_bt_go.SetActive(true);
                }
                urefs.prof_window_reset_price_text_txt.text = 
                    consts.prof_grade_lvl_mapping[temp.grade].reset_price.ToString();
                urefs.prof_window_reset_class_text_txt.text = consts.start_prof_nm;
            }
            if (act_reset_alpha){
                if (temp.grade != 0){
                    if (statics.mngr_diamonds.amount < consts.prof_grade_lvl_mapping[temp.grade].reset_price){
                        urefs.prof_window_reset_bt_cg.alpha = consts.bttn_min_alpha;
                    } else {
                        urefs.prof_window_reset_bt_cg.alpha = 1f;
                    }
                }
            }
        }

        public static void change_params_for_ps(string prof_nm = null){
            prof_nm ??= cur_prof_nm;
            switch (prof_nm){
                case "Novice":
                    break;
                case "Chocolate Industrialist":
                    recalcs.recalc_gps_m();
                    break;
                case "Chocolate Enthusiast":
                    recalcs.recalc_crit_ch();
                    break;
                case "Manufacturer":
                    recalcs.recalc_gps_m();
                    break;
                case "Economist":
                    //
                    break;
                case "Combo Master":
                    mngr_tap.combo_click_cnt = 0;
                    break; 
                case "Chocolate Crusher":
                    recalcs.recalc_crit_ch();
                    recalcs.recalc_crit_m();
                    break;
            }
        }
    }

    public static class mngr_indicator{
        public static float val;

        static bool is_visible = false;
        static bool is_bonus_active = false;
        static Coroutine deactivate_coroutine;
        static Coroutine bonus_shake_coroutine;
        static Coroutine bonus_txt_deactivate_coroutine;
        static Coroutine glowing_choco_deactivate_coroutine;
        static Coroutine chocorain_coroutine;

        const int glowing_choco_animator_layer = 2;
        const string glowing_choco_appear_state = "appear";
        const string glowing_choco_disappear_state = "disappear";
        static Vector2 bonus_txt_initial_pos;
        static Vector3 bonus_txt_initial_euler;
        static bool bonus_txt_defaults_initialized = false;
        static Animator bonus_txt_animator;

        public static void init(){
            val = 0f;
            is_visible = false;
            is_bonus_active = false;
            stop_bonus_shake();
            stop_chocorain();
            if (glowing_choco_deactivate_coroutine != null && statics.logic_module != null){
                statics.logic_module.StopCoroutine(glowing_choco_deactivate_coroutine);
                glowing_choco_deactivate_coroutine = null;
            }
            bonus_txt_defaults_initialized = false;
            bonus_txt_deactivate_coroutine = null;
            if (deactivate_coroutine != null && statics.logic_module != null){
                statics.logic_module.StopCoroutine(deactivate_coroutine);
                deactivate_coroutine = null;
            }
            if (urefs.glowing_choco_go != null && urefs.glowing_choco_go.activeSelf)
                urefs.glowing_choco_go.SetActive(false);
            bonus_txt_animator = urefs.tap_indicator_bonus_txt_anmtr;
            if (urefs.tap_indicator_bonus_txt != null){
                RectTransform bonus_rt = urefs.tap_indicator_bonus_txt.rectTransform;
                if (bonus_rt != null){
                    bonus_txt_initial_pos = bonus_rt.anchoredPosition;
                    bonus_txt_initial_euler = bonus_rt.localEulerAngles;
                    bonus_txt_defaults_initialized = true;
                    reset_bonus_text_transform();
                }
                urefs.tap_indicator_bonus_txt.text =
                    common_utils.f2s(consts.tap_indicator_bonus) + "x";
                urefs.tap_indicator_bonus_txt.gameObject.SetActive(false);
            }
            if (urefs.tap_indicator_fill_im != null)
                urefs.tap_indicator_fill_im.fillAmount = 0f;
            if (urefs.tap_indicator_go != null && urefs.tap_indicator_go.activeSelf)
                urefs.tap_indicator_go.SetActive(false);
        }

        public static void on_click(){
            val = Mathf.Clamp01(val + consts.tap_indicator_click_gain);
            update_state();
        }

        public static void update(float delta_time){
            if (val > 0f){
                val = Mathf.Max(0f, val - consts.tap_indicator_decay_speed * delta_time);
                update_state();
            } else if (is_visible || is_bonus_active){
                update_state();
            }
        }

        public static float get_tap_multiplier(){
            return is_bonus_active ? consts.tap_indicator_bonus : 1f;
        }

        static void update_state(){
            if (urefs.tap_indicator_fill_im != null)
                urefs.tap_indicator_fill_im.fillAmount = val;

            bool should_be_visible = val > 0f;
            if (should_be_visible != is_visible){
                if (should_be_visible){
                    show();
                } else {
                    hide();
                }
            }

            bool should_show_bonus = val > consts.tap_indicator_bonus_threshold;
            if (should_show_bonus != is_bonus_active){
                is_bonus_active = should_show_bonus;
                handle_bonus_text_visibility(is_bonus_active);
                handle_glowing_choco_visibility(is_bonus_active);
                update_chocorain_state();
            }
        }

        static void show(){
            is_visible = true;
            if (deactivate_coroutine != null && statics.logic_module != null){
                statics.logic_module.StopCoroutine(deactivate_coroutine);
                deactivate_coroutine = null;
            }
            if (urefs.tap_indicator_go != null && !urefs.tap_indicator_go.activeSelf)
                urefs.tap_indicator_go.SetActive(true);
            if (urefs.tap_indicator_anmtr != null)
                urefs.tap_indicator_anmtr.Play("appear", 0, 0f);
        }

        static void hide(){
            if (!is_visible) return;
            is_visible = false;
            if (urefs.tap_indicator_anmtr != null)
                urefs.tap_indicator_anmtr.Play("disappear", 0, 0f);
            handle_bonus_text_visibility(false);
            handle_glowing_choco_visibility(false);
            if (deactivate_coroutine != null && statics.logic_module != null)
                statics.logic_module.StopCoroutine(deactivate_coroutine);
            if (statics.logic_module != null){
                deactivate_coroutine = statics.logic_module.StartCoroutine(disable_after());
            } else {
                deactivate_coroutine = null;
                if (urefs.tap_indicator_go != null)
                urefs.tap_indicator_go.SetActive(false);
            }
        }

        static void handle_bonus_text_visibility(bool visible){
            if (urefs.tap_indicator_bonus_txt == null)
                return;

            if (bonus_txt_deactivate_coroutine != null && statics.logic_module != null){
                statics.logic_module.StopCoroutine(bonus_txt_deactivate_coroutine);
                bonus_txt_deactivate_coroutine = null;
            }

            if (visible){
                GameObject bonus_go = urefs.tap_indicator_bonus_txt.gameObject;
                if (!bonus_go.activeSelf)
                    bonus_go.SetActive(true);
                if (bonus_txt_animator != null && bonus_txt_animator.isActiveAndEnabled)
                    bonus_txt_animator.Play("appear", 0, 0f);

                if (!bonus_txt_defaults_initialized)
                    return;
                stop_bonus_shake();
                if (statics.logic_module != null)
                    bonus_shake_coroutine = statics.logic_module.StartCoroutine(shake_bonus_text());
            } else {
                stop_bonus_shake();
                if (bonus_txt_animator != null && statics.logic_module != null
                    && bonus_txt_animator.isActiveAndEnabled
                    && urefs.tap_indicator_bonus_txt.gameObject.activeInHierarchy){
                    bonus_txt_deactivate_coroutine =
                        statics.logic_module.StartCoroutine(disable_bonus_text_after_animation());
                } else if (urefs.tap_indicator_bonus_txt.gameObject.activeSelf){
                    if (bonus_txt_animator != null && bonus_txt_animator.isActiveAndEnabled)
                        bonus_txt_animator.Play("disappear", 0, 0f);
                    urefs.tap_indicator_bonus_txt.gameObject.SetActive(false);
                }
            }
        }

        static void handle_glowing_choco_visibility(bool visible){
            if (urefs.glowing_choco_go == null)
                return;

            if (visible){
                if (glowing_choco_deactivate_coroutine != null && statics.logic_module != null){
                    statics.logic_module.StopCoroutine(glowing_choco_deactivate_coroutine);
                    glowing_choco_deactivate_coroutine = null;
                }
                if (!urefs.glowing_choco_go.activeSelf)
                    urefs.glowing_choco_go.SetActive(true);
                if (urefs.glowing_choco_anmtr != null && urefs.glowing_choco_anmtr.isActiveAndEnabled){
                    float start_time = get_glowing_choco_start_time(glowing_choco_appear_state);
                    play_glowing_choco_animation(glowing_choco_appear_state, start_time);
                }
            } else {
                if (!urefs.glowing_choco_go.activeSelf)
                    return;

                if (glowing_choco_deactivate_coroutine != null && statics.logic_module != null){
                    statics.logic_module.StopCoroutine(glowing_choco_deactivate_coroutine);
                    glowing_choco_deactivate_coroutine = null;
                }

                if (urefs.glowing_choco_anmtr != null && statics.logic_module != null
                    && urefs.glowing_choco_anmtr.gameObject.activeInHierarchy
                    && urefs.glowing_choco_anmtr.isActiveAndEnabled){
                    glowing_choco_deactivate_coroutine =
                        statics.logic_module.StartCoroutine(disable_glowing_choco_after_animation());
                } else {
                    if (urefs.glowing_choco_anmtr != null && urefs.glowing_choco_anmtr.isActiveAndEnabled){
                        float start_time = get_glowing_choco_start_time(glowing_choco_disappear_state);
                        play_glowing_choco_animation(glowing_choco_disappear_state, start_time);
                    }
                    urefs.glowing_choco_go.SetActive(false);
                    glowing_choco_deactivate_coroutine = null;
                }
            }
        }

        static IEnumerator disable_glowing_choco_after_animation(){
            if (urefs.glowing_choco_anmtr != null){
                float start_time = get_glowing_choco_start_time(glowing_choco_disappear_state);
                play_glowing_choco_animation(glowing_choco_disappear_state, start_time);
            }

            if (urefs.glowing_choco_anmtr != null
                && urefs.glowing_choco_anmtr.gameObject.activeInHierarchy
                && urefs.glowing_choco_anmtr.isActiveAndEnabled){
                yield return common_utils.wait_until_state_end(
                    urefs.glowing_choco_anmtr,
                    glowing_choco_disappear_state,
                    glowing_choco_animator_layer
                );
            }

            if (urefs.glowing_choco_go != null)
                urefs.glowing_choco_go.SetActive(false);

            glowing_choco_deactivate_coroutine = null;
        }

        static float get_glowing_choco_start_time(string state_name){
            if (urefs.glowing_choco_anmtr == null)
                return 0f;

            float target_start_time = 0f;
            if (state_name == glowing_choco_appear_state){
                float progress = get_glowing_choco_state_progress(glowing_choco_disappear_state);
                if (progress >= 0f)
                    target_start_time = 1f - progress;
            } else if (state_name == glowing_choco_disappear_state){
                float progress = get_glowing_choco_state_progress(glowing_choco_appear_state);
                if (progress >= 0f)
                    target_start_time = 1f - progress;
            }

            if (target_start_time <= 0f){
                float current_progress = get_glowing_choco_state_progress(state_name);
                if (current_progress > 0f)
                    target_start_time = current_progress;
            }

            return Mathf.Clamp(target_start_time, 0f, 0.999f);
        }

        static float get_glowing_choco_state_progress(string state_name){
            if (urefs.glowing_choco_anmtr == null)
                return -1f;

            Animator animator = urefs.glowing_choco_anmtr;
            AnimatorStateInfo current_state = animator.GetCurrentAnimatorStateInfo(glowing_choco_animator_layer);
            if (current_state.IsName(state_name)){
                if (current_state.normalizedTime < 1f)
                    return Mathf.Clamp01(current_state.normalizedTime);
                return -1f;
            }

            if (animator.IsInTransition(glowing_choco_animator_layer)){
                AnimatorStateInfo next_state = animator.GetNextAnimatorStateInfo(glowing_choco_animator_layer);
                if (next_state.IsName(state_name)){
                    if (next_state.normalizedTime < 1f)
                        return Mathf.Clamp01(next_state.normalizedTime);
                    return -1f;
                }
            }

            return -1f;
        }

        static void play_glowing_choco_animation(string state_name, float normalized_time){
            if (urefs.glowing_choco_anmtr == null
                || !urefs.glowing_choco_anmtr.isActiveAndEnabled)
                return;

            urefs.glowing_choco_anmtr.Play(
                state_name,
                glowing_choco_animator_layer,
                normalized_time
            );
        }

        static void stop_bonus_shake(){
            if (bonus_shake_coroutine != null && statics.logic_module != null)
                statics.logic_module.StopCoroutine(bonus_shake_coroutine);
            bonus_shake_coroutine = null;
            reset_bonus_text_transform();
        }

        static void reset_bonus_text_transform(){
            if (!bonus_txt_defaults_initialized || urefs.tap_indicator_bonus_txt == null)
                return;
            RectTransform bonus_rt = urefs.tap_indicator_bonus_txt.rectTransform;
            if (bonus_rt == null)
                return;
            bonus_rt.anchoredPosition = bonus_txt_initial_pos;
            bonus_rt.localEulerAngles = bonus_txt_initial_euler;
        }

        static IEnumerator shake_bonus_text(){
            if (!bonus_txt_defaults_initialized || urefs.tap_indicator_bonus_txt == null)
                yield break;
            RectTransform bonus_rt = urefs.tap_indicator_bonus_txt.rectTransform;
            if (bonus_rt == null)
                yield break;

            float time = UnityEngine.Random.value * 10f;
            while (is_bonus_active && bonus_rt.gameObject.activeInHierarchy){
                time += Time.unscaledDeltaTime;
                float speed = consts.tap_indicator_bonus_shake_speed;
                float strength = consts.tap_indicator_bonus_shake_strength;
                float noise_x = Mathf.PerlinNoise(time * speed, 0f) * 2f - 1f;
                float noise_y = Mathf.PerlinNoise(0f, time * speed) * 2f - 1f;
                Vector2 offset = new Vector2(noise_x * strength, noise_y * strength * 0.6f);
                bonus_rt.anchoredPosition = bonus_txt_initial_pos + offset;
                float angle = Mathf.Sin(time * speed * 1.5f) * consts.tap_indicator_bonus_shake_angle;
                bonus_rt.localEulerAngles = bonus_txt_initial_euler + new Vector3(0f, 0f, angle);
                yield return null;
            }

            reset_bonus_text_transform();
            bonus_shake_coroutine = null;
        }

        static IEnumerator disable_after(){
            if (urefs.tap_indicator_anmtr != null
                && urefs.tap_indicator_anmtr.gameObject.activeInHierarchy
                && urefs.tap_indicator_anmtr.isActiveAndEnabled){
                yield return common_utils.wait_until_state_end(urefs.tap_indicator_anmtr, "disappear");
            }
            if (urefs.tap_indicator_go != null)
                urefs.tap_indicator_go.SetActive(false);
            deactivate_coroutine = null;
        }

        static IEnumerator disable_bonus_text_after_animation(){
            if (bonus_txt_animator != null)
                bonus_txt_animator.Play("disappear", 0, 0f);

            if (bonus_txt_animator != null
                && bonus_txt_animator.gameObject.activeInHierarchy
                && bonus_txt_animator.isActiveAndEnabled){
                yield return common_utils.wait_until_state_end(bonus_txt_animator, "disappear");
            }

            if (urefs.tap_indicator_bonus_txt != null)
                urefs.tap_indicator_bonus_txt.gameObject.SetActive(false);

            bonus_txt_deactivate_coroutine = null;
        }

        static void update_chocorain_state(){
            if (statics.logic_module == null){
                stop_chocorain();
                return;
            }

            if (is_bonus_active){
                if (chocorain_coroutine == null)
                    chocorain_coroutine = statics.logic_module.StartCoroutine(run_chocorain());
            } else {
                stop_chocorain();
            }
        }

        static void stop_chocorain(){
            if (chocorain_coroutine != null && statics.logic_module != null)
                statics.logic_module.StopCoroutine(chocorain_coroutine);
            chocorain_coroutine = null;
        }

        static IEnumerator run_chocorain(){
            while (is_bonus_active){
                GameObject cf_instance_temp =
                    UnityEngine.Object.Instantiate(consts.chocofall_pf, urefs.cfrd_zone_rt);

                UnityEngine.Object.Destroy(cf_instance_temp, consts.cf_sample_lifetime);
                yield return new WaitForSeconds(consts.cf_spawn_interval);
            }

            chocorain_coroutine = null;
        }
    }

    public static class mngr_tap{
        public static int
            lvl,
            combo_click_cnt = 0; //для combo master

        public static float
            price,
            b_gain,
            f_gain,
            b_tap,
            f_tap,
            crit_ch,
            crit_m,
            tap_m,
            diamond_ch;

        static bool is_hold_pressed;
        static bool hold_generated_click;
        static bool skip_next_release_click;
        static Coroutine hold_clicks_coroutine;

        public static void init(){
            recalcs.recalc_tap_m();
            recalcs.recalc_crit_ch();
            recalcs.recalc_crit_m();
            recalcs.recalc_diamonds_ch();

            reset();

            urefs.tap_references.buy_button_bcc.on_click.AddListener(try_buy);
            urefs.chocolate_bcc.on_click.AddListener(handle_chocolate_click);

            urefs.chocolate_bcc.on_down.AddListener(on_chocolate_pressed);
            urefs.chocolate_bcc.on_up.AddListener(on_chocolate_released);
            urefs.chocolate_bcc.on_exit.AddListener(cancel_chocolate_hold);
            urefs.chocolate_bcc.on_disable.AddListener(cancel_chocolate_hold);

            urefs.tap_references.buy_button_bcc.on_enter.AddListener(
                () => {
                    if (statics.mngr_balance.amount >= (float)Math.Truncate(price))
                        urefs.tap_references.backgr_im.color = consts.active_upgr_clr;
                }
            );
            urefs.tap_references.buy_button_bcc.on_exit.AddListener(
                () => {
                    if (statics.mngr_balance.amount >= (float)Math.Truncate(price))
                        urefs.tap_references.backgr_im.color = consts.not_active_upgr_clr;
                }
            );
        }

        static void handle_chocolate_click(){
            if (skip_next_release_click){
                skip_next_release_click = false;
                return;
            }

            on_tap();
        }

        static void on_chocolate_pressed(){
            if (urefs.chocolate_bcc.anmtr != null)
                urefs.chocolate_bcc.anmtr.Play("make_smaller", 0, 0f);

            start_hold_clicks();
        }

        static void on_chocolate_released(){
            if (urefs.chocolate_bcc.anmtr != null)
                urefs.chocolate_bcc.anmtr.Play("make_normal_from_smaller", 0, 0f);

            stop_hold_clicks(true);
        }

        static void cancel_chocolate_hold(){
            if (is_hold_pressed && urefs.chocolate_bcc.anmtr != null)
                urefs.chocolate_bcc.anmtr.Play("make_normal_from_smaller", 0, 0f);

            stop_hold_clicks(false);
        }

        static void start_hold_clicks(){
            is_hold_pressed = true;
            hold_generated_click = false;
            skip_next_release_click = false;

            if (statics.logic_module == null)
                return;

            if (hold_clicks_coroutine != null)
                statics.logic_module.StopCoroutine(hold_clicks_coroutine);

            hold_clicks_coroutine = statics.logic_module.StartCoroutine(run_hold_clicks());
        }

        static void stop_hold_clicks(bool shouldSkipReleaseClick){
            is_hold_pressed = false;

            if (hold_clicks_coroutine != null && statics.logic_module != null){
                statics.logic_module.StopCoroutine(hold_clicks_coroutine);
                hold_clicks_coroutine = null;
            }

            if (shouldSkipReleaseClick && hold_generated_click)
                skip_next_release_click = true;

            hold_generated_click = false;
        }

        static IEnumerator run_hold_clicks(){
            float interval = consts.tap_hold_clicks_per_second > 0f
                ? 1f / consts.tap_hold_clicks_per_second
                : 0.1f;

            interval = Mathf.Max(interval, 0.01f);

            yield return new WaitForSeconds(interval);

            while (is_hold_pressed){
                perform_hold_click();
                yield return new WaitForSeconds(interval);
            }

            hold_clicks_coroutine = null;
        }

        static void perform_hold_click(){
            on_tap();
            hold_generated_click = true;

            if (statics.logic_module != null)
                statics.logic_module.StartCoroutine(play_hold_click_animation());
        }

        static IEnumerator play_hold_click_animation(){
            var animator = urefs.chocolate_bcc?.anmtr;
            if (animator == null)
                yield break;

            animator.Play("make_normal_from_smaller", 0, 0f);
            yield return null;

            if (is_hold_pressed)
                animator.Play("make_smaller", 0, 0f);
        }

        public static void clear_tap(){
            reset();
            save_module.save_tap();
        }

        public static void reset(){
            lvl = consts.st_tap_lvl;
            price = consts.st_tap_price;
            b_gain = consts.st_b_tap_gain;
            b_tap = consts.st_b_tap;
            recalcs.recalc_tap_f_tap();
            recalcs.recalc_tap_f_gain();
            act_ui(new(){"lvl", "price", "alpha"});
        }

        public static void act_ui(HashSet<string> modes){
            bool 
                act_lvl = modes.Contains("lvl"),
                act_price = modes.Contains("price"),
                act_gain = modes.Contains("gain"),
                act_tap = modes.Contains("tap"),
                act_alpha = modes.Contains("alpha");

            if (act_lvl){
                urefs.tap_references.lvl_txt.text = lvl.ToString();
            }  
            if (act_price){
                urefs.tap_references.price_txt.text = 
                    common_utils.f2s((float)Math.Truncate(price));
                statics.logic_module.StartCoroutine(common_utils.rebuild_layout(urefs.tap_references.price_rt));
            }
            if (act_gain){
                urefs.tap_references.gain_txt.text = 
                    "+" + common_utils.f2s(f_gain) + " coins/tap"; 
            }
            if (act_tap){
                urefs.tap_val_text_txt.text = "Per Tap: " + common_utils.f2s(f_tap);
            }
            if (act_alpha){
                if (statics.mngr_balance.amount < price){
                    urefs.tap_references.backgr_im.color = consts.not_active_upgr_clr;
                    urefs.tap_references.dark_go.SetActive(true);
                } else {
                    urefs.tap_references.dark_go.SetActive(false);
                }
            }
        }

        public static void try_buy(){
            float real_price = (float)Math.Truncate(price);
            if (statics.mngr_balance.amount >= real_price){
                float price_old_temp = real_price;

                b_tap += b_gain;
                recalcs.recalc_tap_f_tap();

                b_gain *= consts.tap_gain_coef;
                recalcs.recalc_tap_f_gain();

                price *= consts.tap_price_coef;
                lvl++;
                act_ui(new(){"lvl", "price"});

                statics.mngr_balance.amount -= price_old_temp;
                statics.mngr_balance.on_val_change();

                float xp_reward = price_old_temp * consts.xp_purchase_fraction;
                if (xp_reward > 0f){
                    mngr_xp.add((float)Math.Truncate(xp_reward));
                    mngr_xp.act_ui();
                }

                save_module.save_tap();

                mngr_tutor.try_close_tutor("tutor_tap");

                mngr_settings.play_sound(consts.currency);
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static void change_float_number_params(float number, Color32 clr, bool is_indicator_bonus_active){
            GameObject popup_prefab = consts.click_num_pf;
            if (is_indicator_bonus_active && consts.tap_popup_bonus_ind_pf != null){
                popup_prefab = consts.tap_popup_bonus_ind_pf;
            }

            var temp1 = UnityEngine.Object.Instantiate(popup_prefab, urefs.chocolate_rt)
                .GetComponent<tap_number_refs>();
            temp1.txt.text = "+" + common_utils.f2s((float)Math.Truncate(number));
            temp1.txt.color = clr;
            if (is_indicator_bonus_active && temp1.multiplier_txt != null){
                temp1.multiplier_txt.text = "(" + common_utils.f2s(consts.tap_indicator_bonus) + "x)";
                temp1.multiplier_txt.color = consts.tap_indicator_multiplier_clr;
            }
        }

        public static void on_tap(){
            mngr_tutor.try_close_tutor("tutor_click");
            mngr_indicator.on_click();
            float indicator_multiplier = mngr_indicator.get_tap_multiplier();
            bool is_indicator_bonus_active = indicator_multiplier > 1f;
            if (is_indicator_bonus_active)
                chocogen.gen();
            if (UnityEngine.Random.value < diamond_ch){
                mngr_diamonds.amount++;
                mngr_diamonds.on_val_change();

                mngr_settings.play_sound(consts.diamond_on_tap_ac);
            }
            float temp1 = f_tap;
            Color32 tap_clr = consts.default_tap_clr;
            if (UnityEngine.Random.value < crit_ch){
                tap_clr = consts.crit_tap_clr;
                temp1 = f_tap * crit_m;
            }
            if (statics.mngr_prof.cur_prof_nm == "Combo Master"){
                combo_click_cnt++;
                var temp2 = statics.mngr_prof.get_fval_ps();
                if (combo_click_cnt == temp2[0]){
                    temp1 = UnityEngine.Random.Range((int)temp2[1], (int)temp2[2] + 1) * f_tap;
                    combo_click_cnt = 0;
                    tap_clr = consts.combo_tap_clr;
                }
            }
            temp1 *= indicator_multiplier;
            temp1 = (float)Math.Truncate(temp1);
            change_float_number_params(temp1, tap_clr, is_indicator_bonus_active);

            mngr_balance.amount += temp1; 
            mngr_balance.on_val_change();

			if (mngr_tutor.tutor_state_dict["tutor_upgr"] == 0 
                && mngr_balance.amount >= consts.upgrs_data[consts.upgrs_sorted_list[0]].bp
                && mngr_xp.lvl >= consts.upgrs_data[consts.upgrs_sorted_list[0]].open_lvl
            ){
				mngr_tutor.try_show_tutor("tutor_upgr");
			}

			if (mngr_tutor.tutor_state_dict["tutor_tap"] == 0 
            && mngr_balance.amount >= consts.st_tap_price
            && urefs.shop_scrollbar.value >= 0.99){
				mngr_tutor.try_show_tutor("tutor_tap");
			}

            mngr_xp.add(temp1);
            mngr_xp.act_ui();

            mngr_achs.achs_dict["tap"].val += 1;
            mngr_achs.achs_dict["tap"].on_val_change();

            mngr_quests.recalc("quest_collect_1");
            mngr_quests.recalc("quest_collect_2");

            mngr_settings.play_sound(consts.click_ac);
        }
    }

    public static class mngr_xp{

        static Coroutine levelup_banner_coroutine;

        public static float 
            xp;

        public static int 
            lvl,
            max_lvl;

        public static void init(){
            reset();
            max_lvl = lvl;
            act_ui();

            var ach_lvl_temp = mngr_achs.achs_dict["lvl"];
            if (max_lvl > ach_lvl_temp.val){
                ach_lvl_temp.val = max_lvl;
                ach_lvl_temp.on_val_change();
            }
        }

        public static void clear_xp(){
            reset();
            act_ui();
            save_module.save_lvlxp();
        }

        public static void add(float val){
            float xp_need;
            bool is_lvl_up = false;
            while (val > 0){
                xp_need = get_xp_to_next_lvl() - xp;
                if (val >= xp_need){
                    val -= xp_need;
                    //lvlup
                    lvl++;
                    xp = 0;

                    mngr_upgrs.try_open_upgr();
                    is_lvl_up = true;       
                } else {
                    xp += val;
                    val = 0;
                }
            }
            if (is_lvl_up){
                if (lvl >=  consts.min_temper_lvl){
                    recalcs.recalc_tempering_reward_amount();
                }
                mngr_tempering.act_ui(new(){"bttn_alpha"});
                if (lvl > max_lvl){
                    max_lvl = lvl;
                    var ach_lvl_temp = mngr_achs.achs_dict["lvl"];
                    if (max_lvl > ach_lvl_temp.val){
                        ach_lvl_temp.val = max_lvl;
                        ach_lvl_temp.on_val_change();
                    }
                    mngr_prof.act_ui(new(){"prof_w"});
                    recalcs.recalc_skins_state(mngr_skins.skins_list);
                    mngr_tutor.try_show_opening_bttns();
                    if (mngr_xp.lvl >= consts.open_tmprng_win_min_lvl)
                        mngr_tutor.try_show_tutor("tutor_tempering_win_bttn");
                    if (mngr_xp.lvl >= consts.open_prof_win_min_lvl)
                        mngr_tutor.try_show_tutor("tutor_prof_win_bttn");
                    if (mngr_xp.lvl >= consts.open_skins_win_min_lvl)
                        mngr_tutor.try_show_tutor("tutor_skin_win_bttn");

                    mngr_quests.recalc("quest_lvl_1");
                    mngr_quests.recalc("quest_lvl_2");
                    mngr_quests.recalc("quest_lvl_3");
                }

                mngr_settings.play_sound(consts.lvlup_ac);
                urefs.lvl_text_anmtr.Play("start");
                try_show_levelup_banner();
            }
            save_module.save_lvlxp();
        }

        public static float get_xp_to_next_lvl(){
            return (float)Math.Truncate(
                (float)Math.Pow(consts.xp_base, lvl) * consts.xp_coef
            );
        }

        public static void reset(){
            xp = 0;
            lvl = 1;
        }

        public static void act_ui(){
            urefs.xp1_text_txt.text = 
                (float)Math.Truncate(xp/get_xp_to_next_lvl() * 100) + "%";
            urefs.xp2_text_txt.text = urefs.xp1_text_txt.text;
            urefs.xp_bar_image_im.fillAmount = xp/get_xp_to_next_lvl();
            urefs.lvl_text_txt.text = "Level " + lvl.ToString();
        }

        static float get_levelup_reward(){
            float tap_reward = consts.lvlup_reward_tap_mult * mngr_tap.f_tap;
            float gps_reward = consts.lvlup_reward_gps_mult * mngr_upgrs.gps;
            return (float)Math.Truncate(tap_reward + gps_reward);
        }

        static void try_show_levelup_banner(){
            float reward = get_levelup_reward();
            if (reward > 0f){
                mngr_balance.amount += reward;
                mngr_balance.on_val_change();
            }

            if (urefs.levelup_banner_reward_txt != null){
                urefs.levelup_banner_reward_txt.text = "+" + common_utils.f2s(reward);
            }

            if (urefs.levelup_banner_go == null
                || urefs.levelup_banner_anmtr == null
                || statics.logic_module == null)
                return;

            if (levelup_banner_coroutine != null){
                statics.logic_module.StopCoroutine(levelup_banner_coroutine);
                levelup_banner_coroutine = null;
            }

            if (urefs.levelup_banner_go.activeSelf)
                urefs.levelup_banner_go.SetActive(false);

            levelup_banner_coroutine =
                statics.logic_module.StartCoroutine(show_levelup_banner());
        }

        static IEnumerator show_levelup_banner(){
            urefs.levelup_banner_go.SetActive(true);
            urefs.levelup_banner_anmtr.Play("appear", 0, 0f);

            if (urefs.levelup_banner_anmtr.gameObject.activeInHierarchy
                && urefs.levelup_banner_anmtr.isActiveAndEnabled){
                yield return common_utils.wait_until_state_end(
                    urefs.levelup_banner_anmtr,
                    "appear"
                );
            }

            urefs.levelup_banner_anmtr.Play("disappear", 0);

            if (urefs.levelup_banner_anmtr.gameObject.activeInHierarchy
                && urefs.levelup_banner_anmtr.isActiveAndEnabled){
                yield return common_utils.wait_until_state_end(
                    urefs.levelup_banner_anmtr,
                    "disappear"
                );
            }

            urefs.levelup_banner_go.SetActive(false);
            urefs.levelup_banner_go.GetComponent<CanvasGroup>().alpha = 1f;
            levelup_banner_coroutine = null;
        }
    }

    public static class mngr_menu{
        public static void init(){
            urefs.shop_upgrs_bt_bcc.on_click.AddListener(
                () => try_change_shop(urefs.shop_upgrs_bt_bcc)
            );
            urefs.shop_arts_bt_bcc.on_click.AddListener(
                () => try_change_shop(urefs.shop_arts_bt_bcc)
            );

            logic_module.StartCoroutine(common_utils.rebuild_layout(urefs.menu_rt));
        }

        private static void try_change_shop(button_custom_class bcc){
            if ((bcc == urefs.shop_upgrs_bt_bcc) && (!urefs.upgr_shop_go.activeSelf)){
                shop_deact("arts");
                shop_activate("upgrs");
            } else if (bcc == urefs.shop_arts_bt_bcc && !urefs.arts_shop_go.activeSelf
            && mngr_tutor.is_opened_dict["arts_bttn_tutor"]){
                shop_deact("upgrs");
                shop_activate("arts");
                mngr_tutor.try_close_tutor("tutor_arts");
                if (mngr_tutor.tutor_state_dict["tutor_arts"] == 2)
                    mngr_tutor.try_show_tutor("tutor_arts_discover");
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        private static void shop_activate(string shop_nm){
            mngr_settings.play_sound(consts.change_shop_ac);
            switch (shop_nm){
                case "upgrs":
                    urefs.shop_scrrt_sr.content = urefs.shop_upgrs_rt;
                    urefs.shop_upgrs_bt_im.color = consts.active_shop_bt_clr;
                    urefs.shop_bt_upgrs_txt.color = consts.not_active_shop_bt_clr;
                    urefs.upgr_shop_go.SetActive(true);
                    statics.mngr_upgrs.on_shop_enable();
                    urefs.stats_upgrs_go.SetActive(true);
                    break;
                case "arts":
                    urefs.shop_scrrt_sr.content = urefs.shop_arts_rt;
                    urefs.shop_arts_bt_im.color = consts.active_shop_bt_clr;
                    urefs.shop_bt_arts_txt.color = consts.not_active_shop_bt_clr;
                    urefs.arts_shop_go.SetActive(true);
                    statics.mngr_arts.on_shop_enable();
                    urefs.stats_arts_go.SetActive(true);
                    break;
            }
            logic_module.StartCoroutine(common_utils.rebuild_layout(urefs.menu_rt));
        }

        private static void shop_deact(string shop_nm){
            urefs.shop_scrrt_sr.content = null;
            switch (shop_nm){
                case "upgrs":
                    urefs.shop_upgrs_bt_im.color = consts.not_active_shop_bt_clr;
                    urefs.shop_bt_upgrs_txt.color = consts.active_shop_bt_clr;
                    urefs.upgr_shop_go.SetActive(false);
                    urefs.stats_upgrs_go.SetActive(false);
                    break;
                case "arts":
                    urefs.shop_arts_bt_im.color = consts.not_active_shop_bt_clr;
                    urefs.shop_bt_arts_txt.color = consts.active_shop_bt_clr;
                    urefs.arts_shop_go.SetActive(false);
                    urefs.stats_arts_go.SetActive(false);
                    break;
            }
        }
    }

    public static class mngr_settings{
        private static float _musicBaseVolume;
        private static float _musicDuckMultiplier = 1f;
        private static int _tutorDuckRequests;
        private static bool _musicVolumeGuardReady;
        private static float _soundBaseVolume;
        private static int _activeSoundOneShots;

        private static float music_target_volume =>
            Mathf.Clamp01(_musicBaseVolume * _musicDuckMultiplier);

        public static void init(){
            urefs.sound_slider_sl.onValueChanged.AddListener(sound_sl_upd);
            urefs.music_slider_sl.onValueChanged.AddListener(music_sl_upd);

            urefs.open_settings_win_bcc.on_click.AddListener(open_win);

            urefs.open_changelog_w_bcc.on_click.AddListener(open_changelog_win);

            urefs.music_asrc_as.priority = consts.music_priority;
            urefs.sound_asrc_as.priority = consts.sound_priority;
            if (urefs.sound_loop_asrc_as != null){
                urefs.sound_loop_asrc_as.priority = consts.sound_loop_priority;
            }

            _musicDuckMultiplier = 1f;
            _tutorDuckRequests = 0;
            urefs.music_asrc_as.ignoreListenerVolume = true;
            urefs.music_asrc_as.bypassEffects = true;
            urefs.music_asrc_as.bypassListenerEffects = true;
            urefs.music_asrc_as.bypassReverbZones = true;

            _soundBaseVolume = Mathf.Clamp01(urefs.sound_slider_sl.value);
            apply_sound_volume();
            _activeSoundOneShots = 0;

            _musicBaseVolume = urefs.music_slider_sl.value * consts.dec_music_vol_coef;
            apply_music_volume();
            _musicVolumeGuardReady = true;
        }

        public static void open_win(){
            mngr_tutor.try_close_tutor("tutor_diamonds");
            sdk_common.gp_stop();
            urefs.win_settings_go.SetActive(true);
            statics.logic_module.StartCoroutine(common_utils.rebuild_layout(urefs.changelog_w_rt));
            mngr_settings.play_sound(consts.open_win_ac);
        }

        //for bind
        public static void sound_sl_upd(float val){
            urefs.sound_text_txt.text = ((int)(val*100)).ToString();
            _soundBaseVolume = Mathf.Clamp01(val);
            apply_sound_volume();
            save_module.save_sound_vol();
        }

        //for bind
        public static void music_sl_upd(float val){
            urefs.music_text_txt.text = ((int)(val*100)).ToString();
            _musicBaseVolume = val * consts.dec_music_vol_coef;
            apply_music_volume();
            save_module.save_music_vol();
        }

        public static void apply_tutor_music_duck(){
            if (_tutorDuckRequests == 0){
                _musicDuckMultiplier *= consts.music_val_dec_while_tutor;
                apply_music_volume();
            }

            _tutorDuckRequests++;
        }

        public static void release_tutor_music_duck(){
            if (_tutorDuckRequests == 0)
                return;

            _tutorDuckRequests--;
            if (_tutorDuckRequests == 0){
                _musicDuckMultiplier /= consts.music_val_dec_while_tutor;
                apply_music_volume();
            }
        }

        public static void refresh_music_volume(){
            apply_music_volume();
        }

        public static void refresh_sound_volume(){
            _soundBaseVolume = Mathf.Clamp01(urefs.sound_slider_sl.value);
            apply_sound_volume();
        }

        public static void tick(){
            if (!_musicVolumeGuardReady || urefs.music_asrc_as == null
                || !urefs.music_asrc_as.enabled)
                return;

            if (!Mathf.Approximately(urefs.music_asrc_as.volume, music_target_volume)){
                apply_music_volume();
            }
        }

        private static void apply_music_volume(){
            urefs.music_asrc_as.volume = music_target_volume;
        }

        private static void apply_sound_volume(){
            update_sound_source_volume();

            if (urefs.sound_loop_asrc_as != null)
                urefs.sound_loop_asrc_as.volume = _soundBaseVolume;
        }

        public static void play_sound(AudioClip clip){
            if (clip == null || urefs.sound_asrc_as == null)
                return;

            if (_soundBaseVolume <= 0f)
                return;

            _activeSoundOneShots++;
            update_sound_source_volume();

            urefs.sound_asrc_as.PlayOneShot(clip);

            if (clip.length <= 0f || statics.logic_module == null){
                release_sound_one_shot_immediate();
                return;
            }

            float duration = clip.length / Mathf.Max(Mathf.Abs(urefs.sound_asrc_as.pitch), 0.01f);
            statics.logic_module.StartCoroutine(release_sound_one_shot(duration));
        }

        private static IEnumerator release_sound_one_shot(float delay){
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            release_sound_one_shot_immediate();
        }

        private static void release_sound_one_shot_immediate(){
            if (_activeSoundOneShots > 0)
                _activeSoundOneShots--;

            update_sound_source_volume();
        }

        private static void update_sound_source_volume(){
            if (urefs.sound_asrc_as == null)
                return;

            if (_soundBaseVolume <= 0f){
                urefs.sound_asrc_as.volume = 0f;
                return;
            }

            int active = Mathf.Max(_activeSoundOneShots, 1);
            urefs.sound_asrc_as.volume = _soundBaseVolume / active;
        }

        public static void open_changelog_win(){
            urefs.changelog_w_go.SetActive(true);
            play_sound(consts.change_shop_ac);
        }
    }

    public static class mngr_discover{        
        public static string
            art_opening_state = "not_active";

        public static float
            discover_price;

        //call strictly after mngr_arts.init
        public static void init(){
            recalcs.recalc_discover_price();
            urefs.discover_bt_bcc.on_click.AddListener(try_discover_art);
            urefs.discover_proc_bcc.on_click.AddListener(show_new_art_or_close);
        }

        //for bind
        public static void try_discover_art(){
            if (statics.mngr_cb.amount >= discover_price){
                mngr_tutor.try_close_tutor("tutor_arts_discover");
            
                statics.mngr_cb.amount -= discover_price;
                statics.mngr_cb.on_val_change();

                art_opening_state = "waiting_to_reveal";
                urefs.discover_proc_go.SetActive(true);

                urefs.sound_loop_asrc_as.clip = consts.discover_waiting_ac;
                urefs.sound_loop_asrc_as.Play();
            } else {
                mngr_settings.play_sound(consts.empty_click_ac);
            }
        }

        public static void show_new_art_or_close(){
            if (art_opening_state == "waiting_to_reveal"){
                urefs.orange_cacao_bean_go.SetActive(false);
                mngr_arts.define_new_art();
                act_ui(new(){"new_art"});
                urefs.new_art_image_go.SetActive(true); 
                urefs.new_art_flash_go.SetActive(true);
                art_opening_state = "revealed";
                urefs.sound_loop_asrc_as.Stop();
                mngr_settings.play_sound(consts.discover_final_ac);
            } else if (art_opening_state == "revealed"){
                urefs.orange_cacao_bean_go.SetActive(true);
                urefs.new_art_flash_go.SetActive(false);
                urefs.new_art_image_go.SetActive(false);
                urefs.discover_proc_go.SetActive(false);
                mngr_arts.open_new_art();
                recalcs.recalc_discover_price();
                art_opening_state = "not_active";
            }
        }

        public static void act_ui(HashSet<string> modes){
            bool 
                act_new_art = modes.Contains("new_art"),
                act_discover_slot = modes.Contains("info_slot");

            if (act_new_art){
                urefs.new_art_image_im.sprite = Resources.Load<Sprite>("images/" + mngr_arts.new_art_to_open);
                urefs.new_art_nm_text_txt.text = mngr_arts.new_art_to_open;  
            }
            if (act_discover_slot){
                if (statics.mngr_arts.closed_arts_set.Count == 0){
                    urefs.discover_slot_go.SetActive(false);
                } else {
                    urefs.discover_price_text_txt.text = common_utils.f2s(discover_price);
                    urefs.discover_slot_tr.SetAsFirstSibling();
                } 
            }
        }
    }

    //для составных переменных отдельно написать рекалк
    //с дефолтными значениями из константы
    public static logic_module logic_module;

    public static class mngr_upgrs{
        public static List<upgr> upgrs_list = new();
        public static Dictionary<string, upgr> upgrs_dict = new();
        public static float 
            gps,
            gps_m,
            cost_m;
        
        //call strictly after mngr_xp.init()
        public static void init(){
            foreach (var u_nm in consts.upgrs_sorted_list){
                upgr u = new(u_nm);
                upgrs_list.Add(u);
                upgrs_dict[u_nm] = u;
            }

            recalcs.recalc_gps_m();
            recalcs.recalc_cost_m();
            act_ui(new(){"stats", "new"});
        }

        public static void on_shop_enable(){
            foreach (var u in upgrs_list){
                statics.logic_module.StartCoroutine(common_utils.rebuild_layout(u.upgr_references.price_rt));
            } 
        }

        public static void clear_upgrs(){
            foreach (var u in upgrs_list){
                UnityEngine.Object.Destroy(u.upgr_references.go);
            } 
            upgrs_list.Clear();
            save_module.clean_upgrs();

            foreach (var u_nm in consts.upgrs_sorted_list){
                upgr u = new(u_nm);
                upgrs_list.Add(u);
                upgrs_dict[u_nm] = u;
            }

            recalcs.recalc_gps_m(); //can depend on upgrs cnt
            act_ui(new(){"stats"});
        }

        public static void try_open_upgr(){
            if (mngr_prof.cur_prof_nm == "Manufacturer"){
                recalcs.recalc_gps_m();
            }
            act_ui(new(){"stats", "state", "new"});

            int opened_u = 0;
            foreach(var u in statics.mngr_upgrs.upgrs_list){
                if (statics.mngr_xp.lvl >= consts.upgrs_data[u.nm].open_lvl){
                    opened_u++;
                }
            }

            var ach_upgrs_temp = statics.mngr_achs.achs_dict["upgrs"];
            if (opened_u > ach_upgrs_temp.val){
                ach_upgrs_temp.val = opened_u;
                ach_upgrs_temp.on_val_change();
            }

            mngr_settings.play_sound(consts.open_u_ac);
        }

        public static void act_ui(HashSet<string> modes){
            bool 
                act_stats_panel = modes.Contains("stats"),
                act_alpha = modes.Contains("alpha"),
                act_state = modes.Contains("state"),
                act_new = modes.Contains("new");

            if (act_stats_panel){
                urefs.stats_upgrs_text_txt.text = 
                    "Per second: " + common_utils.f2s(statics.mngr_upgrs.gps);
            }
            if (act_alpha){
                foreach (var u in upgrs_list){
                    u.act_ui(new(){"alpha"});
                }
            }
            if (act_state){
                foreach (var u in upgrs_list){
                    u.act_ui(new(){"state"});
                }
            }
            if (act_new){
                foreach (var u in upgrs_list){
                    u.act_ui(new(){"new"});
                }
            }
        }
    }

    public static class mngr_arts{
        public static List<art> arts_list = new();
        public static HashSet<string> closed_arts_set = new();
        public static Dictionary<string, art> opened_arts_dict = new();
        public static string new_art_to_open;

        public static void init(){
            closed_arts_set = consts.arts_data.Keys.ToHashSet();
            act_ui(new(){"stats"});
        }

        public static void on_shop_enable(){
            foreach (var art in arts_list){
                statics.logic_module.StartCoroutine(common_utils.rebuild_layout(art.art_references.buy_button_rt));
            } 
        }

        public static void define_new_art(){
            new_art_to_open = closed_arts_set.ElementAt(
                UnityEngine.Random.Range(0, closed_arts_set.Count)
            );
            if (mngr_arts.arts_list.Count == 0){
                new_art_to_open = consts.first_art;
            }
        }

        public static void open_new_art(){
            art new_art = new(new_art_to_open);
            mngr_arts.arts_list.Add(new_art);
            opened_arts_dict[new_art_to_open] = new_art;
            closed_arts_set.Remove(new_art_to_open);
            refresh_params(new_art_to_open);

            act_ui(new(){"stats"});

            mngr_achs.achs_dict["artifacts"].val = arts_list.Count;
            mngr_achs.achs_dict["artifacts"].on_val_change();

            save_module.save_art(new_art);
            save_module.save_art_order(new_art, mngr_arts.arts_list.Count);
        }

        public static void act_ui(HashSet<string> modes){
            bool 
                act_bttn_alpha = modes.Contains("bttn_alpha"),
                act_stats_panel = modes.Contains("stats");
            
            if (act_bttn_alpha){
                foreach (var a in arts_list){
                    a.act_ui(new(){"bttn_alpha"});
                }
            }
            if (act_stats_panel){
                urefs.stats_arts_text_txt.text =  
                    arts_list.Count.ToString() + 
                    "/" +
                    consts.arts_data.Count.ToString();
            }
        }

        public static void refresh_params(string art_nm){
            switch (art_nm){
                case "Bar of Wealth":
                    recalcs.recalc_gps_m();
                    recalcs.recalc_tap_m();
                    break;
                case "Sweetie Hand":
                    recalcs.recalc_tap_m();
                    break;
                case "Time Accelerator":
                    recalcs.recalc_gps_m();
                    break;
                case "Cacao Multiplier":
                    recalcs.recalc_tempering_reward_m();
                    break;
                case "Critical Chocoarrow":
                    recalcs.recalc_crit_ch();
                    break;
                case "Smooth Gear":
                    mngr_prof.change_params_for_ps();
                    mngr_prof.act_ui(new(){"prof_w"});
                    break;
                case "Fortune Crystal":
                    recalcs.recalc_diamonds_ch();
                    break;
                case "Caramel Essence":
                    recalcs.recalc_tap_f_tap();
                    break;
            }
        }

    }

    public static class mngr_achs{
        public static Dictionary<string, ach> achs_dict = new();

        public static void init(){
            foreach (var ach_nm in consts.achs_data.Keys){
                achs_dict[ach_nm] = new(ach_nm);
            }

            urefs.open_ach_win_bcc.on_click.AddListener(open_win);
            urefs.ach_noft_bcc.on_click.AddListener(open_win);
            urefs.close_ach_win_bcc.on_click.AddListener(on_close_win);
        }

        public static void open_win(){
            mngr_tutor.try_close_tutor("tutor_ach_win_bttn");
            mngr_tutor.try_close_tutor("tutor_diamonds");
            sdk_common.gp_stop();
            urefs.achs_window_go.SetActive(true);
            foreach (var ach in achs_dict.Values){
                logic_module.StartCoroutine(common_utils.rebuild_layout(ach.ach_references.collect_gr_rt));
            }
            mngr_settings.play_sound(consts.open_win_ac);
        }

        public static void on_close_win(){
            if (mngr_tutor.tutor_state_dict["tutor_diamonds"] == 0){
                mngr_tutor.try_show_tutor("tutor_diamonds");
            }
        }

        public static void ach_noft(string nm){
            urefs.ach_noft_im.sprite = Resources.Load<Sprite>("images/" + nm);
            mngr_settings.play_sound(consts.ach_noft_ac);
            urefs.ach_noft_anmtr.Play("open", 0, 0f);
            // yield return common_utils.wait_until_state_end(
            //     urefs.ach_noft_anmtr, 
            //     "close"
            // );
            if (mngr_tutor.tutor_state_dict["tutor_ach_win_bttn"] == 0){
                mngr_tutor.try_close_tutor("all");
            };
            mngr_tutor.try_show_tutor("tutor_ach_win_bttn");
            if (urefs.achs_window_go.activeSelf || 
            urefs.win_settings_go.activeSelf ||
            urefs.temper_window_go.activeSelf ||
            urefs.prof_window_go.activeSelf || 
            urefs.skins_window_go.activeSelf) 
                mngr_tutor.try_close_tutor("tutor_ach_win_bttn");
        }
    }
}