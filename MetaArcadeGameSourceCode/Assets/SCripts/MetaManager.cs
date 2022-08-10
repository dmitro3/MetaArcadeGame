using Cinemachine;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MetaManager : MonoBehaviour
{

    public static MetaManager insta;

    public Transform[] playerPoz;
    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera fpsCam;
    public CinemachineVirtualCamera hardFollowCam;   
    public GameObject myCam;
    public GameObject myPlayer;


    public Transform[] racePositions;

    //public static GameObject fightPlayer;
    //public static Photon.Pun.PhotonView fighterView;    
    public static bool isPlayingMinigame = false;
    public static Photon.Realtime.Player fightReqPlayer;
    public static Photon.Realtime.Player inChallengePlayer;

    public static string _fighterid;

    public static bool inVirtualWorld = false;

    [SerializeField] GameObject fortuneWheelUI;


    [Header("Common References")]
    public UnityEngine.UI.Button enterMiniGameBTN;
    public UnityEngine.UI.Button actionBTN;
    public GameObject ShootArea;
    public TMPro.TMP_Text Text_miniGameTimer;
    public TMPro.TMP_Text Text_miniGameCounter;
    public TMPro.TMP_Text Text_Counter;
    public GameObject crossHair;
    [Header("References")]
    public Camera playerCamera;
    public CinemachineInputProvider camera_rotate_input;
    public CinemachineFreeLook freeLook_camera;
    public GameObject Emotes_UI;
    public WheelButtonController emote_wheel_controller;

    [Header("Player Level References")]
    public Image level_progressFillBar;
    public TMPro.TMP_Text text_level;
    public GameObject powerUp_2x;
    public GameObject powerUp_free;
    public GameObject soloRaceBTN;
    private void Awake()
    {
        insta = this;
    }

    private void Update()
    {
        if (inChallengePlayer != null)
        {
            Debug.Log(inChallengePlayer.NickName);
        }
    }
    public void StopMovement()
    {
        myPlayer.GetComponent<PlayerController>().StopMovement();
    }
    public void UpdatePlayerWorldProperties()
    {
        if (SingletonDataManager.myNFTData.Count > 0)
        {
            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["virtualworld"] = JsonConvert.SerializeObject(SingletonDataManager.myNFTData);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            Debug.Log("Updated UpdatePlayerWorldProperties");
        }
    }


    
    #region DAILY SPIN SYSYTEM
    public void ToggleSpinUI(bool activate)
    {
        fortuneWheelUI.SetActive(activate);
    }
    #endregion
}


