using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;

public class PlayerData
{
    // Start is called before the first frame update       
    public LocalData localdata;
    
    public PlayerData()
    {
        //localdata = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalData>(PlayerPrefs.GetString("data"));
        //GetData();
        if (DatabaseManager.Instance != null)
        {
            localdata = DatabaseManager.Instance.GetLocalData();
        }

        if (localdata == null)
        {
            localdata = new LocalData();
        }
    }

    public  void UpdateData()
    {
        //PlayerPrefs.SetString("data", Newtonsoft.Json.JsonConvert.SerializeObject(localdata));
        DatabaseManager.Instance.UpdateData(localdata);
    }
    public void GetData()
    {        
        DatabaseManager.Instance.GetData();        
    }
}
