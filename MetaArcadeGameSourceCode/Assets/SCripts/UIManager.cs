using Defective.JSON;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager insta;

    [SerializeField] GameObject LoadingPanel;
    [Header("GameplayMenu")]
    public GameObject StartUI;
    public GameObject usernameUI;


    public TMP_Text usernameText;

    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button[] gender;

    [SerializeField] TMP_Text statusText;

    // fight manager
    [SerializeField] GameObject FightRequestUI;
    [SerializeField] TMP_Text fightRequestText;


    public static string username;
    public static int usergender;

    [Header("No Coins Info")]
    [SerializeField] GameObject NoCoinsUI;
    public TMP_Text txt_information;

    [Header("Achivements NFT")]
    [SerializeField] GameObject NFTPopup;
    [SerializeField] TMP_Text txt_nftDetails;
    [SerializeField] TMP_Text txt_nftName;
    [SerializeField] RawImage nft_image;
    [SerializeField] GameObject claimBTN;

    [Header("Mini Game Complete UI")]
    [SerializeField] GameObject MiniGameCompleteUI;
    [SerializeField] TMP_Text txt_result;
    [SerializeField] TMP_Text txt_gamename;
    [SerializeField] TMP_Text txt_score_change;


    [Header("GameplayMenu")]
    public GameObject GameplayUI;
    public GameObject extraBTNS;
    [SerializeField] TMP_Text scoreTxt;
    [SerializeField] TMP_Text winCountTxt;
    [SerializeField] TMP_Text lostCountTxt;
    [SerializeField] Slider healthSlider;

    [SerializeField] FrostweepGames.WebGLPUNVoice.Recorder recorder;
    [SerializeField] FrostweepGames.WebGLPUNVoice.Listener lister;

    [Header("GAme Complete UI")]
    [SerializeField] GameObject gameoverUI;
    [SerializeField] TMP_Text winnerText;
    [SerializeField] TMP_Text starsgotText;

    [Header("VoiceChat")]
    [SerializeField] Image recorderImg;
    [SerializeField] Image listenerImg;
    [SerializeField] Sprite[] recorderSprites; //0 on 1 off
    [SerializeField] Sprite[] listenerSprites; //0 on 1 off


    [Header("StoreAndCollection")]
    [SerializeField] GameObject myCollectionUI;
    [SerializeField] TMP_Text TxtHeaderCollection;


    [Header("Result")]
    [SerializeField] Image resultImg;
    [SerializeField] Sprite[] resultprites; //0 win 1 lose 2 tie

    [Header("Tutorial")]
    [SerializeField] GameObject TutorialUI;
    [SerializeField] int currentTutorial = 0;
    [SerializeField] GameObject[] tutorialObjects;

    [Header("Mini Game Tutorial")]
    [SerializeField] GameObject MiniTutorialUI;
    [SerializeField] TMP_Text minigametutorial_txt;
    [SerializeField] Image minigame_tutorial_image;

    public GameObject MyCollectionUIButton;

    public GameObject VirtualWorldObj;

    [Header("Spin Failed")]
    [SerializeField] GameObject FailedPopup;
    [SerializeField] GameObject RetryBTN;

    private void Awake()
    {
        insta = this;
        if (DatabaseManager.Instance)
        {
            DatabaseManager.Instance.GetData();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameplayUI.SetActive(false);
        resultImg.gameObject.SetActive(false);
        StartUI.SetActive(true);
        usernameUI.SetActive(false);
        gender[0].interactable = false;
        gender[1].interactable = true;
        statusText.gameObject.SetActive(true);
        statusText.text = "";
        healthSlider.value = 1;

        PlayerData data = new PlayerData();
        UpdatePlayerUIData(true, data.localdata, true);
        UpdateUserName(SingletonDataManager.username, SingletonDataManager.userethAdd);

        if (PlayerPrefs.GetInt("init", 0) == 0)
        {
            PlayerPrefs.SetInt("init", 1);
            EditUserProfile();
        }

        recorder.StopRecord();

        if (DatabaseManager.Instance)
        {
            usergender = DatabaseManager.Instance.GetUserGender();
        }
        
    }

    public void ToggleLoadingPanel(bool _show)
    {
        LoadingPanel.SetActive(_show);
    }

    async public void ShowMyCollection(bool _show)
    {
        if (_show)
        {

           

            MetaManager.insta.myPlayer.GetComponent<PlayerController>().StopMovement();
            LoadingPanel.SetActive(true);

            MyNFTCollection.insta.DestroyItems();

            string result = await BlockChainManager.Instance.CheckNFTBalance();            
            if (!string.IsNullOrEmpty(result) && result != "[]")
            {
                Debug.Log(result);
                JSONObject jsonObject = new JSONObject(result);
                for (int i = 0; i < jsonObject.count; i++)
                {
                    
                    Debug.Log(jsonObject[i].GetField("tokenId"));
                    if(jsonObject[i].GetField("tokenId").stringValue.StartsWith("40"))
                    {
                        continue;
                    }
                    MyNFTCollection.insta.GenerateItem(Int32.Parse(jsonObject[i].GetField("tokenId").stringValue));
                }
                TxtHeaderCollection.text = "My Collection";
                myCollectionUI.SetActive(true);
            }
            else MessaeBox.insta.showMsg("Nothing in collection", true);
            LoadingPanel.SetActive(false);
            
        }
        else
        {
            MetaManager.insta.myPlayer.GetComponent<PlayerController>().ResumeMovement();
            myCollectionUI.SetActive(false);
        }

    }
    async public void ShowMyPowers(bool _show)
    {
        if (_show)
        {
            
            MetaManager.insta.myPlayer.GetComponent<PlayerController>().StopMovement();
            LoadingPanel.SetActive(true);

            MyNFTCollection.insta.DestroyItems();

            string result = await BlockChainManager.Instance.CheckNFTBalance();
            if (!string.IsNullOrEmpty(result) && result != "[]")
            {
                Debug.Log(result);
                JSONObject jsonObject = new JSONObject(result);
                for (int i = 0; i < jsonObject.count; i++)
                {

                    if (jsonObject[i].GetField("tokenId").stringValue.StartsWith("50"))
                    {
                        continue;
                    }
                    MyNFTCollection.insta.GenerateItem(Int32.Parse(jsonObject[i].GetField("tokenId").stringValue));
                }
                TxtHeaderCollection.text = "Themes";
                myCollectionUI.SetActive(true);
            }
            else MessaeBox.insta.showMsg("Nothing in collection", true);
            LoadingPanel.SetActive(false);

        }
        else
        {
            MetaManager.insta.myPlayer.GetComponent<PlayerController>().ResumeMovement();
            myCollectionUI.SetActive(false);
        }

    }
    public void ShowResult(int _no)
    {

        LeanTween.scale(resultImg.gameObject, Vector2.one, 1.5f).setFrom(Vector2.zero).setEaseOutBounce();
        StartCoroutine(gameResult(_no));

    }

    IEnumerator gameResult(int _no)
    {
        resultImg.gameObject.SetActive(true);
        resultImg.sprite = resultprites[_no];
        yield return new WaitForSeconds(3);
        resultImg.gameObject.SetActive(false);

        if (_no == 0)
        {
            if (SingletonDataManager.myNFTData.Count == 0)
                MessaeBox.insta.showMsg("Get land for your virtual world from store", true);
            else MessaeBox.insta.showMsg("Earned 1 coin", true);
        }
    }

    public void VisitVirtualWorld(bool _show)
    {

       /* if (_show && !MetaManager.inVirtualWorld && !MetaManager.isFighting && !MetaManager.isShooting)
        {
            if (SingletonDataManager.myNFTData.Count > 0)
            {
                SingletonDataManager.isMyVirtualWorld = true;
                VirtualWorldObj.SetActive(true);
                MetaManager.inVirtualWorld = true;
            }
            else
            {
                Debug.Log("No enough nft");
                MessaeBox.insta.showMsg("No virtual world item\nFight to earn coins and buy item", true);
            }
        }
        else
        {
            MetaManager.inVirtualWorld = false;
            VirtualWorldObj.SetActive(false);
        }*/

    }


    public void VisitOtherPlayerVirtualWorld()
    {

        
        if (SingletonDataManager.insta && SingletonDataManager.insta.otherPlayerNFTData.Count > 0)
        {
            Debug.Log("Virtual world available");
            SingletonDataManager.isMyVirtualWorld = false;
            VirtualWorldObj.SetActive(true);
            MetaManager.inVirtualWorld = true;
        }
        else
        {
            MetaManager.inVirtualWorld = false;
            Debug.Log("Virtual world NOT available");
            MessaeBox.insta.showMsg("No virtual world item", true);
        }
    }

    public void StartGame()
    {
        //StartUI.SetActive(false);
        if (PlayerPrefs.GetInt("tutorial", 0) == 0)
        {
            ShowTutorial();
        }
        else
        {
            MPNetworkManager.insta.OnConnectedToServer();
        }
    }
    #region Tutorial
    public void ShowTutorial()
    {
        TutorialUI.SetActive(true);
        for (int i = 0; i < tutorialObjects.Length; i++)
        {
            tutorialObjects[i].SetActive(false);
        }
        tutorialObjects[currentTutorial].SetActive(true);
    }
    public void NextTutorial()
    {
        tutorialObjects[currentTutorial].SetActive(false);
        currentTutorial++;
        if (currentTutorial >= tutorialObjects.Length)
        {
            SkipTutorial();
            return;
        }
        tutorialObjects[currentTutorial].SetActive(true);
    }
    public void SkipTutorial()
    {
        PlayerPrefs.SetInt("tutorial", 1);
        TutorialUI.SetActive(false);
        MPNetworkManager.insta.OnConnectedToServer();
    }
    int minigame_index =0;
  
    #endregion

    public void UpdatePlayerUIData(bool _show,LocalData data,bool _init = false)
    {
        /*if (_show)
        {
            if (_init)
            {
                nameInput.text = SingletonDataManager.username;
                SelectGender(SingletonDataManager.userData.characterNo);
            }

            scoreTxt.text = SingletonDataManager.userData.score.ToString();
            winCountTxt.text = SingletonDataManager.userData.fightWon.ToString();
            lostCountTxt.text = SingletonDataManager.userData.fightLose.ToString();
            if (PhotonNetwork.LocalPlayer.CustomProperties["health"] != null) healthSlider.value = float.Parse(PhotonNetwork.LocalPlayer.CustomProperties["health"].ToString());
        }
        else
        {
            GameplayUI.SetActive(false);
        }*/

        //TEST
        if (_show)
        {           
            scoreTxt.text = data.score.ToString(); // SingletonDataManager.userData.score.ToString();
            winCountTxt.text = data.gameWon.ToString(); //SingletonDataManager.userData.fightWon.ToString();
            lostCountTxt.text = data.gameLoss.ToString();//SingletonDataManager.userData.fightLose.ToString();
            if (PhotonNetwork.LocalPlayer.CustomProperties["health"] != null) healthSlider.value = float.Parse(PhotonNetwork.LocalPlayer.CustomProperties["health"].ToString());
        }
        else
        {
            GameplayUI.SetActive(false);
        }
    }

    public void MuteUnmute()
    {
        if (recorder.recording)
        {
            recorder.recording = false;
            recorderImg.sprite = recorderSprites[1];
            recorder.StopRecord();
        }
        else
        {
            recorder.RefreshMicrophones();
            recorder.recording = true;
            recorder.StartRecord();
            recorderImg.sprite = recorderSprites[0];
        }


    }

    public void MuteUnmuteListner()
    {
        if (lister._listening)
        {
            lister._listening = false;
            listenerImg.sprite = listenerSprites[1];
        }
        else
        {
            lister._listening = true;
            listenerImg.sprite = listenerSprites[0];
        }
    }

    public void openMyWorld()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("VirtualWorld");
    }
    public void UpdateUserName(string _name, string _ethad = null)
    {
        if (_ethad != null)
        {
            usernameText.text = "Hi, " + _name + "\n Your crypto address is : " + _ethad;
            username = _name;
        }
        else usernameText.text = _name;
    }

    public void UpdateStatus(string _msg)
    {
        FightRequestUI.SetActive(false);
        statusText.text = _msg;
        StartCoroutine(ResetUpdateText());
    }

    IEnumerator ResetUpdateText()
    {
        yield return new WaitForSeconds(2);
        statusText.text = "";
    }


    public void EditUserProfile()
    {
        usernameUI.SetActive(true);
        StartUI.SetActive(false);
        if (DatabaseManager.Instance)
        {
           string name= DatabaseManager.Instance.GetUserName();
           nameInput.text = name;
        }
    }
    public void GetName()
    {
        if (nameInput.text.Length > 0 && !nameInput.text.Contains("Enter")) username = nameInput.text;
        else username = "Player_" + UnityEngine.Random.Range(11111, 99999);


        usernameUI.SetActive(false);

        if (SingletonDataManager.insta)
        {
            SingletonDataManager.insta.submitName(username);
        }
        if (DatabaseManager.Instance)
        {
            DatabaseManager.Instance.ChangeGenderAndNameData(usergender, username);
        }

        StartUI.SetActive(true);

    }

    bool won = false;
    public void ShowGameCompleteUI( bool wonGame, int coinsChange)
    {
        won = wonGame;
        if (wonGame)
        {
            winnerText.text = "Congrats! You won!";
             starsgotText.text = "stars gained: " + coinsChange.ToString();
        }
        else
        {
            winnerText.text = "Oops! You Got Hit!";
            starsgotText.text = "stars gained: " + coinsChange.ToString();
        }
        gameoverUI.SetActive(true);
        LeanTween.scale(gameoverUI.transform.GetChild(0).gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInQuad);
    }

    

    public void CloseGameCompleteUI()
    {
        MetaManager.insta.myPlayer.GetComponent<PlayerController>().IncreaseXP(won);
        LeanTween.scale(gameoverUI.transform.GetChild(0).gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(() => {
            gameoverUI.SetActive(false);
        });
    }

    int nft_coontract_id;

   
    async public void ShowNFTPopup(int level)
    {
        if (level == 0)
        {
            return;
        }
        NFTPopup.SetActive(true);
        nft_coontract_id =Mathf.Clamp( 499+ level,500,504);
        claimBTN.SetActive(true);

        string result = await BlockChainManager.Instance.CheckNFTBalance();
        if (!string.IsNullOrEmpty(result) && result != "[]")
        {
            bool isInList = true;
            int not_in_list_contracr_id = 500;
            Debug.Log(result);
            JSONObject jsonObject = new JSONObject(result);

            List<int> all_nft_ids=new List<int>();
            for (int i = 0; i < jsonObject.count; i++)
            {
                Debug.Log(jsonObject[i]);
                Debug.Log(jsonObject[i].GetField("tokenId"));
               

                all_nft_ids.Add(Int32.Parse(jsonObject[i].GetField("tokenId").stringValue));


               
           //     MyNFTCollection.insta.GenerateItem(Int32.Parse(jsonObject[i].GetField("tokenId").stringValue));
            }
            if(nft_coontract_id==504 && all_nft_ids.Contains(504))
            {
                NFTPopup.SetActive(false);
                return;
            }
            for (int j = 500; j < nft_coontract_id; j++)
            {
                if (!all_nft_ids.Contains(j))
                {
                    isInList = false;
                    not_in_list_contracr_id = j;
                    break;
                }
            }
            if (!isInList)
            {
                Debug.Log("not In List + " + not_in_list_contracr_id.ToString());
                nft_coontract_id = not_in_list_contracr_id;
            }
            else if (all_nft_ids.Contains(nft_coontract_id))
            {
                NFTPopup.SetActive(false);
                return;
            } 


            //   myCollectionUI.SetActive(true);
        }
        else
        {
            nft_coontract_id = 500;
        }

        MetaFunNFTLocal NftData = DatabaseManager.Instance.GetNFTMetaData(nft_coontract_id);
        if (NftData != null) {
            txt_nftDetails.text = NftData.description;
            txt_nftName.text = NftData.name;
            nft_image.texture = NftData.imageTexture;
        }
        LeanTween.scale(NFTPopup.transform.GetChild(0).gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInQuad);
    }
    public void ClaimNonBurnableNFT()
    {
        LeanTween.scale(NFTPopup.transform.GetChild(0).gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(() => {
            NFTPopup.SetActive(false);
        });
        BlockChainManager.Instance.purchaseItem(nft_coontract_id-496, false);      
    }

    public void BuyThemeFromShop(int index)
    {        
        BlockChainManager.Instance.purchaseItem(index, false);
    }

    public void ShowNoCoinsPopup()
    {
        MetaManager.insta.myPlayer.GetComponent<PlayerController>().StopMovement();
        NoCoinsUI.SetActive(true);
        LeanTween.scale(NoCoinsUI.transform.GetChild(0).gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInQuad);

    }
    public void CloseNoCoinsPopup()
    {

        MetaManager.insta.myPlayer.GetComponent<PlayerController>().ResumeMovement();
        LeanTween.scale(NoCoinsUI.transform.GetChild(0).gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(()=> {
            NoCoinsUI.SetActive(false);
        });

    }

    
    public void SelectGender(int _no)
    {
        if (_no == 0)
        {
            gender[0].interactable = false;
            gender[1].interactable = true;
        }
        else
        {
            gender[1].interactable = false;
            gender[0].interactable = true;
        }

        usergender = _no;
        if (SingletonDataManager.insta)
        {
            SingletonDataManager.userData.characterNo = _no;
        }
    }


    public void ShowBurnableNFTConfimation(int _id,string status)
    {
        txt_information.transform.parent.gameObject.SetActive(true);
        if (status.Equals("success"))
        {
            txt_information.text = "Coin Purchase of " + status + " successful";
        }
        else
        {
            txt_information.text = "Coin Purchase of " + status + " Failed";
        }

        StartCoroutine(disableTextInfo());
    }
    public void ShowCoinPurchaseStatus(TranscationInfo info)
    {
        txt_information.transform.parent.gameObject.SetActive(true);
        if (info.transactionStatus.Equals("success")) {
            txt_information.text = "Coin Purchase of " + info.coinAmount + " successful";
        }
        else
        {
            txt_information.text = "Coin Purchase of " + info.coinAmount + " Failed";
        }

        StartCoroutine(disableTextInfo());
    }
    public void ShowInfoMsg(string info)
    {
        txt_information.transform.parent.gameObject.SetActive(true);
        
        txt_information.text = info;       

        StartCoroutine(disableTextInfo());
    }
    IEnumerator disableTextInfo()
    {
        yield return new WaitForSeconds(3f);
        txt_information.transform.parent.gameObject.SetActive(false);
    }


    #region FightREquest
    public void FightReq(string _userdata)
    {
        FightRequestUI.SetActive(true);
        fightRequestText.text = _userdata + " want to fight with you !";
    }

   
    public void FightReqAcion(bool _accept)
    {
        if (_accept)
        {

        }
        else
        {

        }
        MetaManager.insta.myPlayer.GetComponent<PlayerController>().RequestFightAction(_accept);
        FightRequestUI.SetActive(false);
        Debug.Log("Fight Action " + _accept);
       // PhotonView photonView = PhotonView.Get(this);
       // photonView.RPC("UpdateHealthMe", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId);
    }
    [PunRPC]
    void UpdateHealthMe(string _uid)
    {
        Debug.Log("CheckID " + _uid);
    }





    #endregion
    public void ToggleExtraButtons(bool _active)
    {
        extraBTNS.SetActive(_active);
    }
}
