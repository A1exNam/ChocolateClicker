using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CrazyGames;

public class logic_module : MonoBehaviour{
    void Start(){
        save_module.init();
        
        statics.mngr_achs.init();
        statics.mngr_skins.init();
        statics.mngr_cb.init();
        statics.mngr_diamonds.init();
        statics.mngr_balance.init();
        statics.mngr_xp.init(); 
        statics.mngr_prof.init(); 
        statics.mngr_tap.init();     
        statics.mngr_menu.init();
        statics.mngr_settings.init(); 
        statics.mngr_upgrs.init(); 
        statics.mngr_arts.init(); 
        statics.mngr_discover.init();
        statics.mngr_tempering.init();
        statics.mngr_ad_bttn.init();
        statics.mngr_tutor.init();
        statics.mngr_quests.init();

        recalcs.init(); 

        statics.mngr_gameover.init();

        statics.logic_module.StartCoroutine(save_module.init());

        StartCoroutine(common_utils.do_every_second());
    }

    void Update(){ 
        statics.mngr_balance.amount += statics.mngr_upgrs.gps * Time.deltaTime;
        statics.mngr_balance.on_val_change();

        urefs.sun_tr.Rotate(0,0, 6f * Time.deltaTime);
    }
}
