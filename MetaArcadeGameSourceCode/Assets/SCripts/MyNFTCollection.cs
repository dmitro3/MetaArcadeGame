using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyNFTCollection : MonoBehaviour
{
    public static MyNFTCollection insta;
    [SerializeField] GameObject itemPanelUI;
    [SerializeField] GameObject itemPurchaseUI;

    [SerializeField] Transform itemParent;
    //item panel stuff
    [SerializeField] GameObject itemButtonPrefab;


    //purcahse panel stuff
    [SerializeField] RawImage purchaseItemImg;
    [SerializeField] TMP_Text purchaseItemText;    
    [SerializeField] GameObject okBTN;
    

    int currentSelectedItem = -1;

    private void Awake()
    {
        insta = this;
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ClosePurchasePanel();

/*        if (CovalentManager.loadingData)
        {
            MessaeBox.insta.showMsg("Loading Data", true);
            CloseItemPanel();
            return;
        }

        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < SingletonDataManager.myNFTData.Count; i++)
        {
            var temp = Instantiate(itemButtonPrefab, itemParent);
            temp.GetComponent<RawImage>().texture = SingletonDataManager.metanftlocalData[SingletonDataManager.myNFTData[i].itemid].imageTexture;
            var tempNo = i;
            var tempTexture = temp.GetComponent<RawImage>().texture;
            temp.GetComponent<Button>().onClick.AddListener(() => SelectItem(tempNo, tempTexture));
        }
*/
       
    }

   

    public void GenerateItem(int tokenId)
    {

        Debug.Log(tokenId);
        var temp = Instantiate(itemButtonPrefab, itemParent);
        temp.transform.GetChild(1).GetComponent<RawImage>().texture = DatabaseManager.Instance.GetNFTTexture(tokenId);//SingletonDataManager.metanftlocalData[i].imageTexture;
        temp.transform.GetChild(2).GetComponent<TMP_Text>().text = DatabaseManager.Instance.GetNFTName(tokenId); // SingletonDataManager.metanftlocalData[i].cost.ToString();
        temp.GetComponent<Button>().onClick.AddListener(() => { SelectItem(tokenId, DatabaseManager.Instance.GetNFTTexture(tokenId)); });
        //var tempNo = i;
        //var tempTexture = SingletonDataManager.metanftlocalData[i].imageTexture;
        if (tokenId == 400 || tokenId == 401)
        {
            //temp.transform.GetChild(3).gameObject.SetActive(true);
            temp.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => BurnNFT(tokenId));
        }
    }

    public void SelectItem(int _no, Texture _texture)
    {
        okBTN.SetActive(true);
        if (_no == 400 || _no == 401)
        {
            okBTN.transform.GetChild(0).GetComponent<TMP_Text>().text = "Claim";
            okBTN.GetComponent<Button>().onClick.RemoveAllListeners();
            okBTN.GetComponent<Button>().onClick.AddListener(()=> {
                BurnNFT(_no);
                okBTN.SetActive(false);
            });
        }
        else
        {
            okBTN.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ok";
            okBTN.GetComponent<Button>().onClick.RemoveAllListeners();
            okBTN.GetComponent<Button>().onClick.AddListener(() => {
                ClosePurchasePanel();
            });
        }

        Debug.Log("Selected item " + _no);
        currentSelectedItem = _no;
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(true);
        purchaseItemImg.texture = _texture;// itemButtons[_no].GetComponent<RawImage>().texture;
        if (DatabaseManager.Instance.GetNFTMetaData(_no) != null)
        {
            purchaseItemText.text = DatabaseManager.Instance.GetNFTMetaData(_no).description;
        }
    }

    public void BurnNFT(int i)
    {
        BlockChainManager.Instance.BurnOnSendContract(i);
    }

    public void ClosePurchasePanel()
    {
        itemPanelUI.SetActive(true);
        itemPurchaseUI.SetActive(false);      
    }

    public void CloseItemPanel()
    {
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(false);
       // if (!CovalentManager.loadingData) CovalentManager.insta.GetNFTUserBalance();
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(false);

        MetaManager.insta.myPlayer.GetComponent<PlayerController>().ResumeMovement();
    }

    public void DestroyItems()
    {
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }
    }
}
