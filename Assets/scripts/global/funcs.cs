using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Globalization;
using System;
using UnityEngine.UI;
using TMPro;
using CrazyGames;

public static class common_utils{
    public static void InitStaticDataClassFromMono(Type targetType, object source) {
        var sourceFields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var targetFields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        
        foreach (var sField in sourceFields) {
            var tField = targetFields.FirstOrDefault(f => f.Name == sField.Name && f.FieldType == sField.FieldType);
            if (tField != null) {
                var value = sField.GetValue(source);
                tField.SetValue(null, value);
            }
        }
    }

    public static string f2s(float num) {
        if (float.IsNaN(num) || float.IsInfinity(num)){
            statics.mngr_gameover.gameover();
            return "0";
        }
        string format = "0.##";
        if (Math.Abs(num) < 1000f) return num.ToString(format); 
        int magnitude = (int)Math.Floor(Math.Log10(Math.Abs(num)) / 3);
        magnitude = Math.Min(magnitude, consts.suffixes.Count - 1);
        float scaled = (float)(num / Math.Pow(1000f, magnitude));
        return scaled.ToString(format, CultureInfo.InvariantCulture) + consts.suffixes[magnitude];
    }   

    public static List<string> f2s(List<float> nums) {
        List<string> res = new();
        foreach (var n in nums){
            res.Add(f2s(n));
        }
        return res;
    }   

    public static IEnumerator rebuild_layout(RectTransform rt){
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    public static IEnumerator wait_until_state_end(Animator anmtr, string state_nm, int layer = 0){
        while (!anmtr.GetCurrentAnimatorStateInfo(layer).IsName(state_nm)){
            yield return null;
        }
        AnimatorStateInfo state_info_temp = anmtr.GetCurrentAnimatorStateInfo(layer);
        while (state_info_temp.IsName(state_nm) && state_info_temp.normalizedTime < 1.0f){
            yield return null;
            state_info_temp = anmtr.GetCurrentAnimatorStateInfo(layer);
        }
    }

    public static IEnumerator change_number_smoothly(
        TextMeshProUGUI txt, 
        float period, 
        float a,
        float b,
        RectTransform rt
    ){
        float 
            elapsed = 0f,
            progress;

        while (elapsed < period){
            elapsed += Time.deltaTime;
            progress = Mathf.Clamp01(elapsed / period);
            txt.text = common_utils.f2s(
                (float)Math.Truncate(Mathf.Lerp(a, b, progress))
            );
            yield return rebuild_layout(rt);
            yield return null;
        }
    }

    public static IEnumerator width_shrink(
        RectTransform rt,
        float period
    ){
        float 
            elapsed = 0f,
            start_width = rt.sizeDelta.x,
            progress;
            
        while (elapsed < period){
            elapsed += Time.deltaTime;
            progress = Mathf.Clamp01(elapsed / period);
            rt.sizeDelta = new Vector2(Mathf.Lerp(start_width, 0, progress), rt.sizeDelta.y);
            yield return null;
        }
    }

    public static void set_active_all_child(GameObject go, bool mode){
        foreach (Transform tr_ch in go.transform){
            tr_ch.gameObject.SetActive(mode);
        }
    }

    public static IEnumerator do_every_second(){
        int tap_cnt_last_sec = -1;
        while (true){
            statics.mngr_achs.achs_dict["hours"].val += 1f/3600f;
            statics.mngr_achs.achs_dict["hours"].on_val_change();

            if (statics.mngr_offline_reward.can_save_last_played_dttm){
                save_module.save_last_played_dttm();
            }
            save_module.save_balance();

            if (save_module.is_saves_restored || save_module.is_saves_timeout){
                int cnt_overall = (int)statics.mngr_achs.achs_dict["tap"].val;
                if (tap_cnt_last_sec != -1 
                && cnt_overall - tap_cnt_last_sec >= consts.max_taps_per_sec){
                    statics.mngr_gameover.gameover_anticlicker();
                }
                tap_cnt_last_sec = cnt_overall;
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    public static void fill_suffix_num_list(){
        consts.suffixes.Add("");

        // Первые стандартные
        consts.suffixes.Add("K"); // 10^3
        consts.suffixes.Add("M"); // 10^6
        consts.suffixes.Add("B"); // 10^9
        consts.suffixes.Add("T"); // 10^12

        // Генерация двухбуквенных (aa, ab, ..., zz)
        for (char first = 'a'; first <= 'z'; first++){
            for (char second = 'a'; second <= 'z'; second++){
                consts.suffixes.Add($"{first}{second}");
            }
        }
    }

    public static List<float> truncate_all(List<float> l){
        List<float> res = new();
        foreach(var v in l){
            res.Add((float)Math.Truncate(v));
        }
        return res;
    }

    public static IEnumerator retypewrite(TextMeshProUGUI txt, Func<bool> stop_cond = null){
        string tmp_txt = txt.text;
        txt.text = "";
        foreach (char c in tmp_txt){
            if (stop_cond != null && stop_cond()){
                yield break; 
            }
            txt.text += c;
            yield return new WaitForSeconds(consts.typewriter_delay);
            urefs.sound_asrc_as.PlayOneShot(consts.typewrite_ac);
        }
    }

    public static bool is_valid(object value){
        if (value == null)
            return false;
        switch (value){
            case float f:
                return !float.IsNaN(f) && !float.IsInfinity(f);
            case int i:
                return true;
            case string s:
                return !string.IsNullOrEmpty(s);
            default:
                return false;
        }
    }

}


//UnityEditor.EditorApplication.isPlaying = false;