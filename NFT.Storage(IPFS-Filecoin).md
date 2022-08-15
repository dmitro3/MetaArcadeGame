## IPFS/Filecoin is used with NFT.Storage to store NFT data


https://https://github.com/MetaArcade/MetaArcadeGame/blob/main/MetaArcadeGameSourceCode/Assets/SCripts/BlockChainManager.cs

### Store NFT data on NFT.Storage IPFS
[Official site API](https://nft.storage/api-docs/)

``` C#
   IEnumerator UploadNFTMetadata(string _metadata, int _id, bool _skin)
    {
        if(MessaeBox.insta) MessaeBox.insta.showMsg("NFT purchase process started\nThis can up to minute", false);
        Debug.Log("Creating and saving metadata to IPFS..." + _metadata);
        WWWForm form = new WWWForm();
        form.AddField("meta", _metadata);

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/store", form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + ConstantManager.nftStorage_key);
            www.timeout = 40;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log("UploadNFTMetadata upload error " + www.downloadHandler.text);
                
                if (_skin)
                {
                    if (MessaeBox.insta)
                    {
                        MessaeBox.insta.ShowRetryPopup(_id);
                    }
                }
                else
                {
                    if (MessaeBox.insta) MessaeBox.insta.showMsg("Server error\nPlease try again", true);
                }
                www.Abort();
                www.Dispose();
            }
            else
            {
                Debug.Log("UploadNFTMetadata upload complete! " + www.downloadHandler.text);

                JSONObject j = new JSONObject(www.downloadHandler.text);
                if (j.HasField("value"))
                {
                    //Debug.Log("Predata " + j.GetField("value").GetField("ipnft").stringValue);
                    SingletonDataManager.nftmetaCDI = j.GetField("value").GetField("url").stringValue; //ipnft
                    //SingletonDataManager.tokenID = j.GetField("value").GetField("ipnft").stringValue; //ipnft
                    Debug.Log("Metadata saved successfully");
                    //PurchaseItem(cost, _id);
                    if(_skin) BurnableNFTBuyContract(_id, j.GetField("value").GetField("url").stringValue);
                    else NonBurnNFTBuyContract(_id, j.GetField("value").GetField("url").stringValue);
                }
            }
        }
    }
```
### IPFS/Filecoin use with NFT.Storage 
![NFT.Storage use](/Images/IPFS-Filecoin.jpg)
