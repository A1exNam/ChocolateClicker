using System.Collections;
using UnityEngine;

public static class sdk_common{
    public static bool 
        is_ad_error = false,
        is_ad_watched = false;

    public static float 
        sdk_timeout_init = 30f;

    public static IEnumerator start_ad_error_lbl(){
        urefs.ad_error_lbl_go.SetActive(true);
        yield return common_utils.wait_until_state_end(urefs.ad_error_lbl_anmtr, "start");
        urefs.ad_error_lbl_go.SetActive(false);
    }

    public static void gp_start(){}

    public static void gp_stop(){}

    public static void start_ad(){
        urefs.ad_watching_backgr_go.SetActive(true);
        is_ad_error = true;
        urefs.ad_watching_backgr_go.SetActive(false);
        urefs.music_asrc_as.mute = false;
        urefs.sound_asrc_as.mute = false;
    }
}
