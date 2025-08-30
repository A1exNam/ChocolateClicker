using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skin{
    public skin_refs 
        skin_references;
    public int 
        state; //состояние скина: -1 - closed, 0 - open, 1 - bought, 2 - active 
    public string 
        nm;

    public skin(string nm_){
        nm = nm_;
        state = -1;
        skin_references = 
            UnityEngine.Object.Instantiate(consts.skin_slot_pf, urefs.skins_content_panel_tr)
            .GetComponent<skin_refs>();
        skin_references.nm_text_txt.text = nm;
        skin_references.image_im.sprite = Resources.Load<Sprite>("images/" + nm);
        skin_references.desc_text_txt.text = consts.skins_data[nm].desc;
        skin_references.price_text_txt.text = consts.skins_data[nm].price.ToString();
        skin_references.stub_unlock_lvl_text_txt.text = 
            "unlocks at " 
            + consts.skins_data[nm].min_lvl 
            + " lvl";
        skin_references.stub_image_im.sprite = skin_references.image_im.sprite;
        skin_references.buy_button_bcc.on_click.AddListener(on_button_click);
        act_ui(new(){"all_except_alpha", "bttn_alpha"});
    }

    public void act_ui(HashSet<string> modes){
        bool 
            act_all_except_alpha = modes.Contains("all_except_alpha"),
            act_bttn_alpha = modes.Contains("bttn_alpha");

        if (act_all_except_alpha){
            switch (state){
                case -1: //closed
                    skin_references.stub_go.SetActive(true);
                    break;
                case 0: //open
                    skin_references.stub_go.SetActive(false);
                    skin_references.buy_button_im.color = consts.not_active_skin_bttn_clr;
                    skin_references.price_text_txt.color = consts.active_skin_bttn_clr;
                    statics.logic_module.StartCoroutine(common_utils.rebuild_layout(skin_references.button_rt));
                    break;
                case 1: //bought but not active
                    skin_references.stub_go.SetActive(false);
                    skin_references.buy_button_diam_image_go.SetActive(false);
                    skin_references.price_text_txt.text = "Choose";
                    skin_references.buy_button_im.color = consts.not_active_skin_bttn_clr;
                    skin_references.price_text_txt.color = consts.active_skin_bttn_clr;
                    statics.logic_module.StartCoroutine(common_utils.rebuild_layout(skin_references.button_rt));
                    break;
                case 2: //active
                    skin_references.stub_go.SetActive(false);
                    skin_references.buy_button_diam_image_go.SetActive(false);
                    skin_references.price_text_txt.text = "Current";
                    skin_references.buy_button_im.color = consts.active_skin_bttn_clr;
                    skin_references.price_text_txt.color = consts.not_active_skin_bttn_clr;
                    statics.logic_module.StartCoroutine(common_utils.rebuild_layout(skin_references.button_rt));

                    urefs.chocolate_im.sprite = skin_references.image_im.sprite;
                    break;
            }
        }
        if (act_bttn_alpha){
            if (state == 0 && statics.mngr_diamonds.amount < consts.skins_data[nm].price){
                skin_references.buy_button_cg.alpha = consts.bttn_min_alpha;
            } else {
                skin_references.buy_button_cg.alpha = 1f;
            }
        }
    }

    public void on_button_click(){
        int temp1 = consts.skins_data[nm].price;
        if ((state == 0) && (statics.mngr_diamonds.amount >= temp1)){

            state = 1;
            act_ui(new(){"all_except_alpha"});

            statics.mngr_diamonds.amount -= temp1;
            statics.mngr_diamonds.on_val_change();
        
            statics.mngr_achs.achs_dict["skins"].val += 1;
            statics.mngr_achs.achs_dict["skins"].on_val_change();

            urefs.sound_asrc_as.PlayOneShot(consts.currency);
            save_module.save_skin(this);
        } else if (state == 1){

            statics.mngr_skins.cur_skin.state = 1;
            statics.mngr_skins.cur_skin.act_ui(new(){"all_except_alpha"});
            save_module.save_skin(statics.mngr_skins.cur_skin);

            state = 2;
            act_ui(new(){"all_except_alpha"});
            statics.mngr_skins.cur_skin = this;

            urefs.sound_asrc_as.PlayOneShot(consts.change_ac);

            save_module.save_skin(this);
        } else {
            urefs.sound_asrc_as.PlayOneShot(consts.empty_click_ac);
        }
    }
}

public class upgr{
    public string 
        nm;

    public float 
        b_price, f_price,
        b_gps, f_gps,
        b_gain, f_gain; 

    public int 
        lvl;

    public upgr_refs 
        upgr_references;

    public upgr(string nm_){
        nm = nm_;
        upgr_references = UnityEngine.Object.Instantiate(consts.upgr_slot_pf, urefs.upgrs_shop_tr)
                            .GetComponent<upgr_refs>();
        upgr_references.nm_txt.text = nm;
        upgr_references.icon_im.sprite = Resources.Load<Sprite>("images/" + nm);
        upgr_references.buy_button_bcc.on_click.AddListener(try_buy);
        upgr_references.buy_button_bcc.on_enter.AddListener(
            () => {
                if (lvl < consts.upgrs_max_lvl 
                && statics.mngr_balance.amount >= (float)Math.Truncate(f_price))
                    upgr_references.backgr_im.color = consts.active_upgr_clr;
            }
        );
        upgr_references.buy_button_bcc.on_exit.AddListener(
            () => {
                if (lvl < consts.upgrs_max_lvl 
                && statics.mngr_balance.amount >= (float)Math.Truncate(f_price))
                    upgr_references.backgr_im.color = consts.not_active_upgr_clr;
            }
        );

        lvl = consts.st_upgrs_lvl;
        act_ui(new(){"lvl"});

        b_gps = 0f;
        recalcs.recalc_upgrs_f_gps(new(){this});

        b_price = consts.upgrs_data[nm].bp;
        recalcs.recalc_upgrs_f_price(new(){this});

        b_gain = consts.upgrs_data[nm].bs;
        recalcs.recalc_upgrs_f_gain(new(){this});

        act_ui(new(){"alpha", "state", "new"});
    }

    public void act_ui(HashSet<string> modes){
        bool 
            act_price = modes.Contains("price"),
            act_gain = modes.Contains("gain"),
            act_lvl = modes.Contains("lvl"),
            act_alpha = modes.Contains("alpha"),
            act_state = modes.Contains("state"),
            act_new = modes.Contains("new");

        if (act_price){
            if (lvl < consts.upgrs_max_lvl){
                upgr_references.price_txt.text = 
                    common_utils.f2s((float)Math.Truncate(f_price));
                statics.logic_module.StartCoroutine(common_utils.rebuild_layout(upgr_references.price_rt));
            } else {
                upgr_references.max_lvl_text_go.SetActive(true);
                upgr_references.price_go.SetActive(false);
            }
        }
        if (act_gain){
            if (lvl < consts.upgrs_max_lvl){
                upgr_references.gain_txt.text = 
                    "+" + common_utils.f2s((float)Math.Truncate(f_gain)) + " coins/sec"; 
            }
        }
        if (act_lvl){
            upgr_references.lvl_txt.text =  lvl.ToString();
        }
        if (act_alpha){
            if (lvl < consts.upgrs_max_lvl 
            && statics.mngr_balance.amount >= (float)Math.Truncate(f_price)){
                upgr_references.dark_go.SetActive(false);
            } else {
                upgr_references.backgr_im.color = consts.not_active_upgr_clr;
                upgr_references.dark_go.SetActive(true);
            }
        }
        if (act_state){
            if (statics.mngr_xp.lvl >= consts.upgrs_data[nm].open_lvl){
                upgr_references.close_go.SetActive(false);
            } else {
                upgr_references.close_go.SetActive(true);
                upgr_references.cond_to_open_txt.text = 
                    "Unlocks at " + consts.upgrs_data[nm].open_lvl + " lvl";
            }
        }
        if (act_new){
            if (lvl == consts.st_upgrs_lvl && statics.mngr_balance.amount >= (float)Math.Truncate(f_price)
            && statics.mngr_xp.lvl >= consts.upgrs_data[nm].open_lvl){
                upgr_references.new_go.SetActive(true);
            } else {
                upgr_references.new_go.SetActive(false);
            }
        }
    }

    public void try_buy(){
        float price_real = (float)Math.Truncate(f_price);
        if (!upgr_references.close_go.activeSelf && lvl < consts.upgrs_max_lvl && statics.mngr_balance.amount >= price_real){
            float price_old_temp = price_real;

            lvl++;
            b_gps += (float)Math.Truncate(b_gain);
            if (lvl < consts.upgrs_max_lvl){
                b_gain *= consts.upgrs_gain_coef;
                b_price *= consts.upgrs_price_coef;

                if (statics.mngr_prof.cur_prof_nm == "Economist"){
                    if (UnityEngine.Random.value <= statics.mngr_prof.get_fval_ps()[0]){
                        lvl++;
                        b_gps += (float)Math.Truncate(b_gain);

                        if (lvl < consts.upgrs_max_lvl){
                            b_gain *= consts.upgrs_gain_coef;
                            b_price *= consts.upgrs_price_coef;
                        }
                    }
                }
            }

            act_ui(new(){"lvl"});
            recalcs.recalc_upgrs_f_gps(new(){this});

            if (lvl == consts.upgrs_max_lvl){
                b_gain = -1f;
                b_price = -1f;
                act_ui(new(){"price"});
            } else {
                recalcs.recalc_upgrs_f_price(new(){this});
                recalcs.recalc_upgrs_f_gain(new(){this});
            }
            
            act_ui(new(){"new"});

            statics.mngr_balance.amount -= price_real;
            statics.mngr_balance.on_val_change();

            urefs.sound_asrc_as.PlayOneShot(consts.currency);
            statics.mngr_tutor.try_close_tutor("tutor_upgr");
            save_module.save_upgr(this);
        } else {
            urefs.sound_asrc_as.PlayOneShot(consts.empty_click_ac);
        }
    } 
}

public class art{
    public string
        nm;

    public art_refs 
        art_references;

    public float 
        price, 
        str;

    public int 
        lvl;

    public void act_ui(HashSet<string> modes){
        bool 
            act_bb = modes.Contains("bb"),
            act_str = modes.Contains("str"),
            act_lvl = modes.Contains("lvl"),
            act_bttn_alpha = modes.Contains("bttn_alpha");

        if (act_bb){
            if ((consts.arts_data[nm].max_lvl == -1) || (lvl < consts.arts_data[nm].max_lvl)){
                art_references.buy_button_up_text_txt.text = 
                    common_utils.f2s((float)Math.Truncate(price));
                art_references.buy_button_down_text_txt.text = "Upgrade";
            } else {
                art_references.cb_image_go.SetActive(false);
                art_references.buy_button_up_text_txt.text = "MAX";
                art_references.buy_button_down_text_txt.text = "LVL";
            }
            statics.logic_module.StartCoroutine(common_utils.rebuild_layout(art_references.buy_button_rt));
        }
        if (act_str){
            float temp2 = str;
            if (consts.arts_data[nm].type == "p"){
                temp2 *= 100;
            }
            art_references.str_text_txt.text = string.Format(
                consts.arts_data[nm].desc,
                common_utils.f2s(temp2)
            );
        }
        if (act_lvl){
            art_references.lvl_text_txt.text =  "Lv. " + lvl.ToString();
        }
        float real_price = (float)Math.Truncate(price);
        if (act_bttn_alpha){
            if (statics.mngr_cb.amount >= real_price && real_price != -1){
                art_references.buy_button_cg.alpha = 1f;
            } else {
                art_references.buy_button_cg.alpha = consts.bttn_min_alpha;
            }
        }
    }

    public art(string nm_){
        nm = nm_;
        art_references = 
            UnityEngine.Object.Instantiate(consts.art_slot_pf, urefs.arts_shop_tr)
            .GetComponent<art_refs>();
        art_references.nm_text_txt.text = nm;
        art_references.art_image_im.sprite = Resources.Load<Sprite>("images/" + nm);
        art_references.buy_button_bcc.on_click.AddListener(try_buy);

        price = consts.st_arts_price;
        lvl = consts.st_arts_lvl;
        str = consts.arts_data[nm].start_str;

        act_ui(new(){"bb", "str", "lvl", "bttn_alpha"});
    }

    public void try_buy(){
        float price_real = (float)Math.Truncate(price);
        if ((statics.mngr_cb.amount >= price_real) &&
                ((lvl < consts.arts_data[nm].max_lvl) || (consts.arts_data[nm].max_lvl == -1))){
            float price_old_temp = price_real;
            lvl++;
            if (lvl == consts.arts_data[nm].max_lvl){
                price = -1f;
            } else {
                price *= consts.arts_price_coef;
            }
            str *= consts.arts_str_coef;
            act_ui(new(){"bb", "str", "lvl"});

            statics.mngr_arts.refresh_params(nm);

            statics.mngr_cb.amount -= price_old_temp;
            statics.mngr_cb.on_val_change();

            urefs.sound_asrc_as.PlayOneShot(consts.currency);

            save_module.save_art(this);
        } else {
            urefs.sound_asrc_as.PlayOneShot(consts.empty_click_ac);
        }
    } 
}

public class ach{
    public string 
        nm;

    public int 
        last_nofted_idx = -1,
        rewarded_cnt = 0;

    public float 
        val = 0f;

    public ach_refs 
        ach_references;

    public List<Image> stars = new();

    public ach(string ach_nm){
        nm = ach_nm;
        ach_references = UnityEngine.Object.Instantiate(consts.ach_slot_pf, urefs.ach_panel_tr)
                        .GetComponent<ach_refs>();
        ach_references.ach_image_im.sprite = Resources.Load<Sprite>("images/" + nm);
        for (int i=0; i<consts.achs_data[nm].val_list.Count; i++){
            stars.Add(
                UnityEngine.Object.Instantiate(consts.ach_star_pf, ach_references.stars_panel_tr)
                .transform.GetComponent<Image>()
            );
        }
        ach_references.reward_bttn_bcc.on_click.AddListener(try_get_reward);
        act_ui(new(){"desc", "bttn_txt", "bar", "bttn_alpha"});
    }

    public void act_ui(HashSet<string> modes){
        bool
            act_desc = modes.Contains("desc"),
            act_bttn_txt = modes.Contains("bttn_txt"),
            act_bar = modes.Contains("bar"),
            act_stars = modes.Contains("stars"),
            act_bttn_alpha = modes.Contains("bttn_alpha");

        var temp1 = consts.achs_data[nm];
        
        if (act_desc){
            ach_references.desc_text_txt.text = 
                string.Format(
                    temp1.desc, 
                    common_utils.f2s(
                        temp1.val_list[Math.Min(rewarded_cnt, temp1.val_list.Count - 1)]
                    )
                );
        }
        if (act_bttn_txt){
            if (rewarded_cnt < temp1.val_list.Count){
                ach_references.reward_text_txt.text = consts.achs_reward[rewarded_cnt + 1].ToString();
            } else {
                ach_references.collect_gr_go.SetActive(false);
                ach_references.completed_text_go.SetActive(true);
            }
            statics.logic_module.StartCoroutine(common_utils.rebuild_layout(ach_references.collect_gr_rt));
        }
        if (act_bar){
            float temp_val_raw = temp1.val_list[Math.Min(rewarded_cnt, temp1.val_list.Count - 1)];

            switch (temp1.mode){
                case "int":
                    ach_references.bar_text1_txt.text = 
                        common_utils.f2s((float)Math.Truncate(val)) + "/" 
                        + common_utils.f2s((float)Math.Truncate(temp_val_raw));
                    break;
                case "time":
                    ach_references.bar_text1_txt.text = 
                        $"{(int)val}h {(int)((val % 1) * 60f)}m";
                    break;
            }

            ach_references.bar_text2_txt.text = ach_references.bar_text1_txt.text;

            ach_references.bar_im.fillAmount = (float)Math.Truncate(val)/temp1.val_list[
                Math.Min(rewarded_cnt, temp1.val_list.Count - 1)
            ];
        }  
        if (act_stars){
            for (int i=0; i<rewarded_cnt; i++){
                stars[i].sprite = Resources.Load<Sprite>("images/" + "star_full");
            }
        }
        if (act_bttn_alpha){
            if (rewarded_cnt == temp1.val_list.Count 
            || (float)Math.Truncate(val) < temp1.val_list[rewarded_cnt]){
                ach_references.reward_bttn_cg.alpha = consts.bttn_min_alpha;
            } else {
                ach_references.reward_bttn_cg.alpha = 1f;
            }
        }
    }

    public void on_val_change(){
        act_ui(new(){"bar", "bttn_alpha"}); 
        while (last_nofted_idx < consts.achs_data[nm].val_list.Count - 1 
        && (float)Math.Truncate(val) >= consts.achs_data[nm].val_list[last_nofted_idx + 1]){
            statics.logic_module.StartCoroutine(statics.mngr_achs.ach_noft(nm));
            last_nofted_idx++;
        }
        save_module.save_ach(this);
    }

    public void try_get_reward(){
        var temp1 = consts.achs_data[nm];
        if (rewarded_cnt < temp1.val_list.Count
        && (float)Math.Truncate(val) >= temp1.val_list[rewarded_cnt]){
            rewarded_cnt++;
            act_ui(new(){"desc", "bttn_txt", "bar", "stars", "bttn_alpha"});

            statics.mngr_diamonds.amount += consts.achs_reward[rewarded_cnt];
            statics.mngr_diamonds.on_val_change();

            urefs.sound_asrc_as.PlayOneShot(consts.ach_reward_ac);

            save_module.save_ach(this);
        } else {
            urefs.sound_asrc_as.PlayOneShot(consts.empty_click_ac);
        }
    }
}