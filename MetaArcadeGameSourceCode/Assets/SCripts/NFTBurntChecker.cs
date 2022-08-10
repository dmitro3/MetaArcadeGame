using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTBurntChecker : MonoBehaviour
{
    [SerializeField] float checkTimer=5;
    float currentTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.unscaledDeltaTime;
        if (currentTime >= checkTimer)
        {
            currentTime = 0;
            CheckBurntNFT();

        }
    }
    async public void CheckBurntNFT()
    {
        string result = await BlockChainManager.Instance.ChecBurnableNFTStatus();
        if (!string.IsNullOrEmpty(result))
        {
            if(MetaManager.insta.myPlayer!=null)
            MetaManager.insta.myPlayer.GetComponent<PlayerController>().SetBurntNFTStatus(result);
           
        }
    }
}
