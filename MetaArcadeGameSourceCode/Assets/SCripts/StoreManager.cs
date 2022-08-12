using Defective.JSON;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager insta;
    [SerializeField] GameObject itemPanelUI;
    [SerializeField] GameObject itemPurchaseUI;
    [SerializeField] GameObject LoadingImage;
    [SerializeField] GameObject ShopUI;
    [SerializeField] GameObject[] ThemesItems;
    [SerializeField] GameObject ThemeSection;
    //item panel stuff
    //[SerializeField] Button[] itemButtons;


    //purcahse panel stuff
    [SerializeField] RawImage purchaseItemImg;
    [SerializeField] TMP_Text purchaseItemText;
    [SerializeField] TMP_Text purchaseItemCostText;

    [SerializeField] TMP_Text balanceText;

    int currentSelectedItem = -1;

    [SerializeField] Transform itemParent;
    //item panel stuff
    [SerializeField] GameObject itemButtonPrefab;

    private void Awake()
    {
        insta = this;
    }



    private void OnEnable()
    {
        balanceText.text = "Balance : " + BlockChainManager.userBalance.ToString();

        DisableOwnedItems();
    }

    async public void DisableOwnedItems()
    {
        LoadingImage.SetActive(true);
        ShopUI.SetActive(false);
        string result = await BlockChainManager.Instance.CheckNFTBalance();
        if (!string.IsNullOrEmpty(result) && result != "[]")
        {
            Debug.Log(result);
            JSONObject jsonObject = new JSONObject(result);
            for (int i = 0; i < jsonObject.count; i++)
            {

                Debug.Log(jsonObject[i].GetField("tokenId"));

                if (jsonObject[i].GetField("tokenId").stringValue.StartsWith("40"))
                {
                    ThemesItems[Int32.Parse(jsonObject[i].GetField("tokenId").stringValue) - 401].SetActive(false);
                }
            }

            bool all_purchased = true;
            for (int i = 0; i < ThemesItems.Length; i++)
            {
                if (ThemesItems[i].activeSelf)
                {
                    all_purchased = false;
                }
            }
            if (all_purchased)
            {
                ThemeSection.SetActive(false);
            }
        }


        LoadingImage.SetActive(false);
        ShopUI.SetActive(true);
    }
    public void SelectItem(int _no, Texture _texture)
    {
        Debug.Log("Selected item " + _no);
        currentSelectedItem = _no;
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(true);
        purchaseItemImg.texture = _texture;// itemButtons[_no].GetComponent<RawImage>().texture;
        purchaseItemText.text = SingletonDataManager.metanftlocalData[_no].description;
        purchaseItemCostText.text = SingletonDataManager.metanftlocalData[_no].cost.ToString();


    }

    public void purchaseItem(int index)
    {
        BlockChainManager.Instance.CoinBuyOnSendContract(index);
    }
   

    public void ClosePurchasePanel()
    {
        itemPanelUI.SetActive(true);
        itemPurchaseUI.SetActive(false);
        
    }

    public void CloseItemPanel()
    {
        //itemPanelUI.SetActive(false);
        //itemPurchaseUI.SetActive(false);
        //Debug.Log("close");
        /*if (!CovalentManager.loadingData) CovalentManager.insta.GetNFTUserBalance();
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }*/
        gameObject.SetActive(false);
        MetaManager.insta.myPlayer.GetComponent<PlayerController>().ResumeMovement();
    }
}
