using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class logic_module : MonoBehaviour{
    void Start(){        
        statics.mngr_achs.init();
        statics.mngr_skins.init();
        statics.mngr_cb.init();
        statics.mngr_diamonds.init();
        statics.mngr_balance.init();
        statics.mngr_xp.init();
        statics.mngr_prof.init();
        statics.mngr_tap.init();
        statics.mngr_indicator.init();
        statics.mngr_menu.init();
        statics.mngr_settings.init(); 
        statics.mngr_upgrs.init(); 
        statics.mngr_arts.init(); 
        statics.mngr_discover.init();
        statics.mngr_tempering.init();
        statics.mngr_ad_bttn.init();
        statics.mngr_tutor.init();
        statics.mngr_quests.init();
        statics.mngr_golden_chocolate.init();

        recalcs.init();

        statics.mngr_gameover.init();

        statics.logic_module.StartCoroutine(save_module.init());

        StartCoroutine(common_utils.do_every_second());
        statics.mngr_balance.ensure_gps_income_loop();
    }

    void Update(){
        statics.mngr_indicator.update(Time.deltaTime);
        statics.mngr_settings.tick();

        urefs.sun_tr.Rotate(0,0, 6f * Time.deltaTime);
    }
}
