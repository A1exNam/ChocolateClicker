using UnityEngine;

public class statics_init: MonoBehaviour{
    void Awake(){
        common_utils.InitStaticDataClassFromMono(typeof(statics), this);
    }
    
    public logic_module logic_module; //для запуска корутин
}