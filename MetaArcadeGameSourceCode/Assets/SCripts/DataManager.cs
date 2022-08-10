using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager
{
  
}

[System.Serializable]
public class localuserData
{
    public int fightWon = 0;
    public int fightLose = 0;
    public int score = 0;
    public int characterNo = 0;
}


[System.Serializable]
public class MetaJungleNFTLocal 
{
    public int itemid;
    public string name;
    public string description;
    public string imageurl;
    public int cost;
    public Texture imageTexture;
}

[System.Serializable]
public class MetaFunNFTLocal
{
    public int itemid;
    public string name;
    public string description;
    public string imageurl;
    public Texture imageTexture;
}



[System.Serializable]
public class MetadataNFT
{
    public int itemid;
    public string name;
    public string description;
    public string image;
    //public properties properties =  new properties();
}

[System.Serializable]
public class MyMetadataNFT
{
    public int itemid;
    public string name;
    public string description;
    public string image;
    public string tokenId;
    //public properties properties =  new properties();
}

[System.Serializable]
public class properties {
    public string videoClip = null;
}


