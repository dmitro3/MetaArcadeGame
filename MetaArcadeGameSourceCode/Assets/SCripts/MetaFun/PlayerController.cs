using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Header("Input Actions")]
    private TPSActionAsset playerActionAsset;
    private InputAction _actionMove;
    private InputAction _actionSprint;


    [Header("Player Componentes")]
    [SerializeField] GameObject vCamTarget;
    [SerializeField] Animator animator;
    [SerializeField] CharacterController cc;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cam;
    [SerializeField] CinemachineInputProvider camera_rotate_input;


    [SerializeField] GameObject power_2x;
    [SerializeField] GameObject power_free;
    [SerializeField] GameObject soloRaceBTN;
 

    [SerializeField] CinemachineFreeLook freeLook_camera;
    [SerializeField] CinemachineVirtualCamera fpsCam;
    [SerializeField] CinemachineVirtualCamera hardFollowCam;
    [SerializeField] GameObject[] characterModels;
    [SerializeField] TMP_Text Text_Counter;


    [Header("Player Properties")]
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;  
    [Tooltip("How far in degrees can you move the camera up in Race Mode")]
    public float TopClamp_Race = 70.0f;
    [Tooltip("How far in degrees can you move the camera down in Race Mode")]
    public float BottomClamp_Race = 23f;

    
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    public float sensitivity=0.3f;
    [SerializeField] TMP_Text usernameText;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float sprintSpeed = 1f;
    [SerializeField] Vector3 lastWorldPos;


    [SerializeField] float race_timer = 0;
    [SerializeField] int player_selected_road;
    [SerializeField] int player_selected_side_road;


    [SerializeField] float jumpForce = 5f;
    [SerializeField] float maxSpeed = 1f;
    [SerializeField] float maxSpeedSprint = 3f;
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] Vector3 verticalDirection = Vector3.zero;
    [SerializeField] bool isSprinting = false;
    [SerializeField] bool isEmoteUIOpened = false;
    [SerializeField] bool isDoingEmote = false;
    [SerializeField] bool can_rotate = true;
    [SerializeField] bool can_move = true;
    [SerializeField] bool isDragging=false;
    [SerializeField] bool lock_x_movement=false;
    [SerializeField] bool lock_z_movement=false;
    int playerNo;

   /* [Header("Weapons")]
    [SerializeField] GameObject weaponObj;
    [SerializeField] GameObject[] weaponParent;
    [SerializeField] GameObject[] weapons;
    [SerializeField] Vector3[] weaponStartPosz;
    [SerializeField] Quaternion[] weaponStartRotz;
    [SerializeField] Vector3 throwableStartPos;
    [SerializeField] Quaternion throwableStartRot;
    [SerializeField] Vector3 throwableGrenadeStartPos;
    [SerializeField] Quaternion throwableGrenadeStartRot;*/

    Vector3 weaponLastPoz;
    Quaternion weaponLastRot;
    [SerializeField] GameObject WeaponCollider;

    [Header("Throwables")]
    [SerializeField] GameObject throwableObject;
    [SerializeField] GameObject throwableGranade;
    [SerializeField] float throwForce = 40f;
    [SerializeField] float throwForceGranade = 50f;

    [Header("Health")]
    [SerializeField] GameObject healthUI;
    [SerializeField] Image healthbarImg;

    [Header("Emotes")]
    [SerializeField] int selected_emote = -1;
    [SerializeField] GameObject Emotes_UI;
    [SerializeField] WheelButtonController emote_wheel_controller;

    [Header("Common Properties")]
    private const float _threshold = 0.01f;

   

    [SerializeField] float gravity = 9.81f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject meetObj;
    [SerializeField] float lerpSpeed = 100;
    [SerializeField] InputActionReference look_action_reference;
    

    [Header("Shooting Properties")]
    [SerializeField] Button enterMiniGameBTN;
    [SerializeField] Button actionBTN;
    [SerializeField] TMP_Text Text_miniGameTimer;
    [SerializeField] TMP_Text Text_miniGameCounter;
    [SerializeField] GameObject bullet_prefab;
    [SerializeField] float bullet_speed;
    [SerializeField] Transform bullet_start_pos;
    [SerializeField] GameObject crossHair;
    [SerializeField] LayerMask aimColliderMask;    
    [SerializeField] Vector3 aimWorldPos;
    [SerializeField] GameObject ParticleEffect;

    [Header("Level Properties")]
    [SerializeField] Image level_progressBar;
    [SerializeField] TMP_Text txt_level_no;


    public bool isPlayingMiniGame;
    [SerializeField] bool isRaceBegan=false;

    [Header("UI Panels")]
    [SerializeField] GameObject virtualWorldUI;
    [SerializeField] GameObject meetUI;

    [Header("Photon")]
    [SerializeField] PhotonView pv;


    [Header("NFT information")]
    [SerializeField] bool isFreeGames=false;
    [SerializeField] bool isDoubleCoins = false;

    
    PlayerData data;


    [Header("Mini Game Tutorials")]
    [SerializeField] TMP_Text txt_tutorialtext;
    [SerializeField] Image tutorial_image;
    

    #region Mouse Click Action
    private void Attack_canceled(InputAction.CallbackContext obj)
    {
        if (!MetaManager.isPlayingMinigame)
        {
            isDragging = false;
            camera_rotate_input.XYAxis = null;
        }
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        
        if (obj.canceled) return;
        
        isDragging = true;
        camera_rotate_input.XYAxis = look_action_reference;
    }
    #endregion
    void Awake()
    {
        cam = Camera.main;

        /*weaponStartPosz = new Vector3[weapons.Length];
        weaponStartRotz = new Quaternion[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            weaponStartPosz[i] = weapons[i].transform.localPosition;
            weaponStartRotz[i] = weapons[i].transform.localRotation;
        }
        throwableStartPos = throwableObject.transform.localPosition;
        throwableStartRot = throwableObject.transform.localRotation;

        throwableGrenadeStartPos = throwableGranade.transform.localPosition;
        throwableGrenadeStartRot = throwableGranade.transform.localRotation;*/

        if (pv.IsMine)
        {

            //TEST
            
            data = new PlayerData();


         

            _cinemachineTargetYaw = vCamTarget.transform.rotation.eulerAngles.y;

            playerActionAsset = new TPSActionAsset();

            MetaManager.insta.myPlayer = gameObject;

            playerCamera = MetaManager.insta.playerCamera;
            camera_rotate_input = MetaManager.insta.camera_rotate_input;

            freeLook_camera = MetaManager.insta.freeLook_camera;
            freeLook_camera.LookAt = this.transform;
            freeLook_camera.Follow = this.transform;

            fpsCam = MetaManager.insta.fpsCam;
            hardFollowCam = MetaManager.insta.hardFollowCam;
            fpsCam.Follow = vCamTarget.transform;
            hardFollowCam.Follow = vCamTarget.transform;

            freeLook_camera.Priority = 10;
            fpsCam.Priority = 0;
            hardFollowCam.Priority = 0;

            meetObj.SetActive(true);


            soloRaceBTN = MetaManager.insta.soloRaceBTN;
            Emotes_UI = MetaManager.insta.Emotes_UI;
            emote_wheel_controller = MetaManager.insta.emote_wheel_controller;

            Text_miniGameTimer = MetaManager.insta.Text_miniGameTimer;
            Text_miniGameCounter = MetaManager.insta.Text_miniGameCounter;
            Text_Counter = MetaManager.insta.Text_Counter;

            Text_miniGameTimer.gameObject.SetActive(false);
            Text_miniGameCounter.gameObject.SetActive(false);

            enterMiniGameBTN = MetaManager.insta.enterMiniGameBTN;
            actionBTN = MetaManager.insta.actionBTN;
            crossHair = MetaManager.insta.crossHair;

            power_2x = MetaManager.insta.powerUp_2x;
            power_free = MetaManager.insta.powerUp_free;
            
           

            txt_level_no = MetaManager.insta.text_level;
            level_progressBar= MetaManager.insta.level_progressFillBar;


            soloRaceBTN.GetComponent<Button>().onClick.AddListener(()=> {
                StartSoloRace();
            });


            usernameText.gameObject.SetActive(false);

            //Set XP Data
            txt_level_no.text = data.localdata.level.ToString();
            Debug.Log((float)data.localdata.xp / ((float)(data.localdata.level + 1) * 100));

            level_progressBar.fillAmount = (float)data.localdata.xp / ((float)(data.localdata.level + 1) * 100);

            player_selected_road = data.localdata.selected_road;
            SetMaterials();
        }

        usernameText.text = pv.Owner.NickName;
        playerNo = int.Parse(pv.Owner.CustomProperties["char_no"].ToString());
        Debug.Log("Player Number " + playerNo);
        characterModels[playerNo].SetActive(true);
        animator = characterModels[playerNo].GetComponent<Animator>();

        meetUI.SetActive(false);
        virtualWorldUI.SetActive(false);
        showHealthBar(false);
    }

    async void Start()
    {
        if (pv.IsMine)
        {
            UIManager.insta.ToggleLoadingPanel(true);

            long lastTimeSpin = long.Parse(data.localdata.last_spin_time);
            long currentTime = await DatabaseManager.Instance.GetCurrentTime();


            Debug.Log("lastTimeSpin : " + lastTimeSpin);
            Debug.Log("currentTime : " + currentTime);

            long diff = currentTime - lastTimeSpin;

            Debug.Log("difference : " + diff);
            //if (lastTimeSpin != null && currentTime != null)
            {
                //TimeSpan ts = currentTime - lastTimeSpin;
               if (diff > 86400)
                {
                    StopMovement();
                    MetaManager.insta.ToggleSpinUI(true);
                }
            }

            UIManager.insta.ToggleLoadingPanel(false);
        }
    }
   
    
    void showHealthBar(bool _show)
    {
        if (_show)
        {
            healthUI.SetActive(true);
        }
        else
        {
            healthUI.SetActive(false);
            healthbarImg.fillAmount = 1;
        }
    }

    private void OnEnable()
    {
        if (pv.IsMine)
        {
            playerActionAsset.Player.Attack.performed += Attack_performed;
            playerActionAsset.Player.Attack.canceled += Attack_canceled;

            _actionMove = playerActionAsset.Player.Move;
            _actionSprint = playerActionAsset.Player.Sprint;
            playerActionAsset.Player.Emote.started += DoOpenEmotePanel;
            playerActionAsset.Player.Emote.canceled += DoCloseEmotePanel;
            playerActionAsset.Player.Enable();
        }
        PhotonNetwork.AddCallbackTarget(this);
    }



    private void OnDisable()
    {
        if (pv.IsMine)
        {
            playerActionAsset.Player.Attack.performed -= Attack_performed;
            playerActionAsset.Player.Attack.canceled -= Attack_canceled;

            playerActionAsset.Player.Emote.started -= DoOpenEmotePanel;
            playerActionAsset.Player.Emote.canceled -= DoCloseEmotePanel;
            playerActionAsset.Player.Disable();
        }
        PhotonNetwork.RemoveCallbackTarget(this);
    }

     
   

    [SerializeField]bool isOnGround=false;
    private void Update()
    {
        if (pv.IsMine)
        {
            Vector2 moveValue = _actionMove.ReadValue<Vector2>();
            isOnGround = IsGrounded();
            if (isDoingEmote && moveValue.magnitude > 0)
            {
                isDoingEmote = false;
                selected_emote = -1;
                animator.Play("Base Layer.Movement", 0, 0);

            }

            isPlayingMiniGame = MetaManager.isPlayingMinigame;

            if (!lock_x_movement && !lock_z_movement)
            {
                moveDirection += moveValue.x * GetCameraRight(playerCamera) * (isSprinting ? sprintSpeed : movementSpeed);
                moveDirection += moveValue.y * GetCameraForward(playerCamera) * (isSprinting ? sprintSpeed : movementSpeed);
            }            
            else
            {
                moveDirection += 1 * GetCameraForward(playerCamera) * sprintSpeed;
            }

            LookAt();

            if (isOnGround)
            {
                gravity = 9.81f;
                if (playerActionAsset.Player.Jump.triggered)
                {
                    verticalDirection.y = jumpForce;
                    if (playerNo == 0)
                    {
                        AudioManager.insta.playSound(17);
                    }
                    else
                    {
                        AudioManager.insta.playSound(19);
                    }
                    animator.SetBool("jump", true);
                    StartCoroutine(ResetJumpBool());
                }
                else
                    verticalDirection.y = 0;

                
            }
            else
            {
                verticalDirection.y += -1 * gravity * Time.deltaTime;
                gravity = gravity + gravity * Time.deltaTime;
            }

            
            if (can_move)
            {
                cc.Move(moveDirection * Time.deltaTime);
                if (isSprinting && (moveDirection.x !=0 || moveDirection.z!=0))
                {
                    animator.SetFloat("speed", 1);
                }
                else
                {
                    if (moveDirection.x != 0 || moveDirection.z != 0)
                    {
                        animator.SetFloat("speed", 0.5f);
                    }
                    else
                    {
                        animator.SetFloat("speed", 0);
                    }
                }
            }
            else
            {
                if (MetaManager.isPlayingMinigame)
                {
                    if (isRaceBegan)
                    {
                        animator.SetFloat("speed", 1);
                        isSprinting = true;
                    }
                    else
                    {
                        animator.SetFloat("speed", 0);
                        isSprinting = false;
                    }
                }
            }
           


            if (can_rotate)
            {
                camera_rotate_input.enabled = true;
            }
            else
            {
                camera_rotate_input.enabled = false;
            }

            if (!MetaManager.isPlayingMinigame)
            {
                isSprinting = _actionSprint.IsPressed();
            }
            
            if (isEmoteUIOpened)
            {
                selected_emote = emote_wheel_controller.ID;
            }


            if (RaceObjectPool.isRaceOn)
            {
                race_timer += Time.deltaTime;
            }

/*
            if (playerActionAsset.Player.Attack.triggered && MetaManager.isFighting && !animator.GetBool("attack"))
            {
                animator.SetBool("attack", true);
                StartCoroutine(waitForEnd(animator.GetCurrentAnimatorStateInfo(0).length));
                AudioManager.insta.playSound(8);
                WeaponCollider.SetActive(true);
                Debug.Log("Attack Collider");
            }
            else
            {
                WeaponCollider.SetActive(false);
            }*/

            

            moveDirection = Vector3.zero;

            cc.Move(verticalDirection * Time.deltaTime);

            

                
        }
    }

    

    private void LateUpdate()
    {


        usernameText.transform.LookAt(MetaManager.insta.myCam.transform);
        usernameText.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);

        healthbarImg.transform.LookAt(MetaManager.insta.myCam.transform);
        healthbarImg.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);

        meetUI.transform.LookAt(MetaManager.insta.myCam.transform);
        meetUI.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);

        virtualWorldUI.transform.LookAt(MetaManager.insta.myCam.transform);
        virtualWorldUI.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);


        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        CameraRotation();


        
    }

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private void CameraRotation()
    {


        if (isDragging)
        {
            Vector2 look = playerActionAsset.Player.Look.ReadValue<Vector2>();
            // if there is an input and camera position is not fixed
            if (look.sqrMagnitude >= _threshold )
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;// : Time.deltaTime;

                _cinemachineTargetYaw += look.x * deltaTimeMultiplier * sensitivity;
                _cinemachineTargetPitch -= look.y * deltaTimeMultiplier * sensitivity;
            }

            if (isPlayingMiniGame)
            {
                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, -50, 50);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp_Race, TopClamp_Race);
            }
            else
            {
                // clamp our rotations so our values are limited 360 degrees
                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
            }
           

            // Cinemachine will follow this target

        }

        
        
        if (!MetaManager.isPlayingMinigame)
        {
            _cinemachineTargetYaw = cam.transform.rotation.eulerAngles.y;
            _cinemachineTargetPitch = cam.transform.rotation.eulerAngles.x;

            vCamTarget.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, transform.eulerAngles.y,0f);
           
        }
        else if(isDragging)
        {            
            vCamTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
    }
    IEnumerator waitForEnd(float _time, int _action = 0)
    {

        yield return new WaitForSeconds(0.85f);
        animator.SetBool("attack", false);
    }

    private void LookAt()
    {
        Vector3 direction = moveDirection;
        direction.y = 0f;
        if (direction.magnitude > 0.05f && !lock_z_movement )
        {
            if (!can_move)
            {
                return;
            }
            this.transform.rotation = Quaternion.Slerp( transform.rotation,Quaternion.LookRotation(direction, Vector3.up),lerpSpeed * Time.fixedDeltaTime);
        }
        
    }
    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    
   
    
    
    IEnumerator ResetJumpBool()
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool("jump", false);
    }
    public void OnEmoteAnimationComplete()
    {
        isDoingEmote = false;
    }
    private void DoCloseEmotePanel(InputAction.CallbackContext obj)
    {
        
        Emotes_UI.SetActive(false);
        isEmoteUIOpened = false;


        if (!MetaManager.isPlayingMinigame)
        {
            if (selected_emote != -1 && IsGrounded() && !MetaManager.isPlayingMinigame)
            {
                isDoingEmote = true;
                animator.SetFloat("emote_id", selected_emote);
                animator.SetBool("emote", true);
                animator.Play("Base Layer.Emotes", 0, 0);
                StartCoroutine(changeEmoteBool());

            }

            can_move = true;
            can_rotate = true;

        }

    }
    IEnumerator changeEmoteBool()
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool("emote", false);
    }

    private void DoOpenEmotePanel(InputAction.CallbackContext obj)
    {
        if (!MetaManager.isPlayingMinigame)
        {
            Emotes_UI.SetActive(true);
            isEmoteUIOpened = true;
            can_move = false;
            can_rotate = false;
        }
    }

    Ray ray;
    


    #region RPC Actions
    public void RequestFight()
    {
        if (MetaManager.isPlayingMinigame) { return; }

        Debug.Log("RequestFight" + pv.Owner.NickName);
        Debug.Log("RequestFightID" + pv.Owner.UserId);
        MetaManager._fighterid = pv.Owner.UserId;
        AudioManager.insta.playSound(2);
        // MetaManager.insta.myPlayer.GetComponent<MyCharacter>().pview.RPC("RequestFightRPC", RpcTarget.All, pview.Owner.UserId);
        pv.RPC("RequestFightRPC", RpcTarget.All, pv.Owner.UserId);

        Debug.Log("RequestFight My " + MetaManager.insta.myPlayer.GetComponent<PhotonView>().Owner.UserId + " | figher " + MetaManager._fighterid);


        UIManager.insta.UpdateStatus("Fight request sent to\n" + pv.Owner.NickName);
        MetaManager.inChallengePlayer = pv.Owner;
    }
    [PunRPC]
    void RequestFightRPC(string _uid, PhotonMessageInfo info)
    {
        Debug.Log("uidPre " + _uid);
        if (pv.IsMine)
        {

            Debug.Log("uid " + _uid);
            if (pv.Owner.UserId.Equals(_uid))
            {
                if (MetaManager.fightReqPlayer != null || MetaManager.isPlayingMinigame ) return;

                Debug.LogFormat("Info: {0} {1}", info.Sender, info.photonView.IsMine);

                MetaManager._fighterid = info.Sender.UserId;
                MetaManager.fightReqPlayer = info.Sender;
                MetaManager.inChallengePlayer = info.Sender;
                //MetaManager.fighterView = info.photonView;
                //MetaManager.fightPlayer = info.photonView.gameObject;
                UIManager.insta.FightReq(info.Sender.ToString());
                AudioManager.insta.playSound(3);

                Debug.Log("RequestFightRPC My " + MetaManager.insta.myPlayer.GetComponent<PhotonView>().Owner.UserId + " | figher " + MetaManager._fighterid);

            }
        }
    }

    [PunRPC]
    void UpdateHealth(string _uid)
    {
        if (pv.Owner.UserId.Equals(_uid))
        {
            if (healthbarImg.fillAmount > 0.1)
            {
                healthbarImg.fillAmount -= 0.1f;
                if (pv.IsMine)
                {
                    AudioManager.insta.playSound(playerNo);
                    var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                    hash["health"] = healthbarImg.fillAmount;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                    UIManager.insta.UpdatePlayerUIData(true, data.localdata);
                }
            }
            else
            {
                showHealthBar(false);

                //ResetWeapon();
                ResetFight();

                if (pv.IsMine)
                {
                    Debug.Log("UserLost");
                    AudioManager.insta.playSound(7);
                    if (SingletonDataManager.insta)
                    {
                        SingletonDataManager.userData.fightLose++;
                        SingletonDataManager.insta.UpdateUserDatabase();
                    }
                    UIManager.insta.ShowResult(1);
                }
            }


        }
    }

    public void VisitVirtualWorld()
    {
        if (SingletonDataManager.insta)
        {
            if (SingletonDataManager.insta.otherPlayerNFTData != null) SingletonDataManager.insta.otherPlayerNFTData.Clear();
            SingletonDataManager.insta.otherPlayerNFTData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MyMetadataNFT>>(pv.Owner.CustomProperties["virtualworld"].ToString());

            if (SingletonDataManager.insta.otherPlayerNFTData != null) UIManager.insta.VisitOtherPlayerVirtualWorld();
            else MessaeBox.insta.showMsg("No virtual world item", true);
        }

        Debug.Log("data" + pv.Owner.CustomProperties["virtualworld"].ToString());





    }
    #endregion

    /* #region Weapons
     int currentWeapon = 0;
     public void SelectWeapon()
     {
         Debug.Log("SelectWeapon");
         currentWeapon = UnityEngine.Random.Range(0, 2);

         meetUI.SetActive(false);
         virtualWorldUI.SetActive(false);

         if (pv.IsMine) MetaManager.isFighting = true;
         weaponObj.SetActive(true);
         showHealthBar(true);
         *//* weaponLastPoz = weapons[currentWeapon].transform.localPosition;
          weaponLastRot = weapons[currentWeapon].transform.localRotation;*//*
         weapons[currentWeapon].transform.localPosition = weaponStartPosz[currentWeapon];
         weapons[currentWeapon].transform.localRotation = weaponStartRotz[currentWeapon];

         weapons[currentWeapon].SetActive(true);
         weapons[currentWeapon].transform.parent = weaponParent[playerNo].transform;
     }

     void ResetWeapon()
     {
         if (pv.IsMine) MetaManager.isFighting = false;
         weapons[currentWeapon].transform.parent = weaponObj.transform;
         // weapons[currentWeapon].transform.localPosition = weaponLastPoz;
         //weapons[currentWeapon].transform.localRotation = weaponLastRot;
         weapons[currentWeapon].transform.localPosition = weaponStartPosz[currentWeapon];
         weapons[currentWeapon].transform.localRotation = weaponStartRotz[currentWeapon];

         weapons[currentWeapon].SetActive(false);
         weaponObj.SetActive(false);


     }


     #endregion
 */

    #region  Race Management
    IEnumerator GoToRacePos(int i,float yPos)
    {
        lastWorldPos = this.transform.position;

        MetaManager.isPlayingMinigame = true;

        race_timer = 0;
        isRaceBegan = false;

        Vector3 pos = RaceObjectPool.Instance.gameObject.transform.position;
        pos.y = yPos;


        RaceObjectPool.Instance.gameObject.transform.position = pos;

        yield return new WaitForSeconds(0.1f);
        Debug.Log("POSITION CHANGED");

        fpsCam.Priority = 0;
        hardFollowCam.Priority = 10;

        Debug.Log("TEST 1 HERE" + transform.position);

        cc.enabled = false;
        this.transform.position = MetaManager.insta.racePositions[i].position;
        this.transform.rotation = MetaManager.insta.racePositions[i].rotation;
        cc.enabled = true;
        

        Debug.Log("TEST 2 HERE" + transform.position);

        vCamTarget.transform.localRotation = Quaternion.identity;
        _cinemachineTargetYaw= vCamTarget.transform.rotation.eulerAngles.y;
        _cinemachineTargetPitch = BottomClamp_Race;

        StartRace(i);
    }

    public void StartSoloRace()
    {
        MetaManager.fightReqPlayer = null;
        MetaManager.inChallengePlayer = null;

        float randomPos = UnityEngine.Random.Range(0, 1000);

        MetaManager.isPlayingMinigame = true;
        can_move = false;
        StartCoroutine(GoToRacePos(0, randomPos));

        float[] offsets = new float[UnityEngine.Random.Range(8, 16)];
        for (int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = UnityEngine.Random.Range(-20, 30f);
        }
        RaceObjectPool.Instance.offsetsOfGeneration = offsets;

        RaceObjectPool.Instance.ResetRace();

        cc.enabled = false;
        this.transform.position = MetaManager.insta.racePositions[0].position;
        this.transform.rotation = MetaManager.insta.racePositions[0].rotation;
        cc.enabled = true;
    }

    public void StartRace(int i)
    {
        RaceObjectPool.Instance.ResetRace();

        StartCoroutine(Countdown(3,i));


        cc.enabled = false;
        this.transform.position = MetaManager.insta.racePositions[i].position;
        this.transform.rotation = MetaManager.insta.racePositions[i].rotation;
        cc.enabled = true;

    
    }
    IEnumerator Countdown(int j,int pos)
    {
        UIManager.insta.ToggleExtraButtons(false);
    Debug.Log("TEST 3 HERE" + transform.position);
        int t = j;
        Text_Counter.text = t.ToString();
        Text_Counter.gameObject.SetActive(true);

        while (t > 0)
        {
            

            t -= 1;
            yield return new WaitForSeconds(1);
            Text_Counter.text = t.ToString();
            Debug.Log("TEST 4 HERE" + transform.position);
        }


        yield return new WaitForSeconds(1);
        Text_Counter.text = "Go!";
        yield return new WaitForSeconds(1);

        Text_Counter.gameObject.SetActive(false);
        isRaceBegan = true;
        RaceObjectPool.Instance.StartRace();       
    }
    #endregion

    #region Event Handling

    private const byte FightEventCode = 1;

    public void RequestFightAction(bool _action)
    {
        Debug.Log("RequestFightAction" + pv.Owner.NickName + " | " + pv.IsMine);

        float randomPos = UnityEngine.Random.Range(0, 1000);

        if (_action)
        {
            Debug.Log("TESTING PART 1");
            MetaManager.isPlayingMinigame = true;
            can_move = false;
            StartCoroutine(GoToRacePos(0,randomPos));

            
        }
        float[] offsets = new float[UnityEngine.Random.Range(8, 16)];
        for (int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = UnityEngine.Random.Range(-20,30f);
        }
        RaceObjectPool.Instance.offsetsOfGeneration = offsets;
        SendFightAction(_action, randomPos,Newtonsoft.Json.JsonConvert.SerializeObject(offsets), MetaManager.fightReqPlayer.UserId, PhotonNetwork.LocalPlayer.UserId);

        MetaManager.inChallengePlayer = MetaManager.fightReqPlayer;

        MetaManager.fightReqPlayer = null;
    }

    private void SendFightAction(bool _action,float randomYPos, string offsetJson, string _p1uid, string _p2uid)
    {
        Debug.Log("SendFightAction OnEvent");
        object[] content = new object[] { _action,randomYPos, offsetJson, _p1uid, _p2uid }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(FightEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {

        byte eventCode = photonEvent.Code;
        if (eventCode == FightEventCode)
        {
            if ((bool)pv.Owner.CustomProperties["isfighting"]) return;

            
            object[] data = (object[])photonEvent.CustomData;
            bool _action = (bool)data[0];
            float yPos = (float)data[1];

            RaceObjectPool.Instance.offsetsOfGeneration =Newtonsoft.Json.JsonConvert.DeserializeObject<float[]>((string)data[2]);

            for (int i = 3; i < data.Length; i++)
            {
                if (pv.Owner.UserId.Equals((string)data[i]))
                {
                    if (_action)
                    {                     
                        Debug.Log("TESTING PART 2");
                        UIManager.insta.UpdateStatus(PhotonNetwork.CurrentRoom.Players[photonEvent.Sender].NickName + " is ready to fight");
                        if (pv.IsMine)
                        {
                            can_move = false;
                            MetaManager.isPlayingMinigame = true;
                         /*   var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                            hash["isfighting"] = true;
                            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);*/
                            StartCoroutine(GoToRacePos(1,yPos));
                            Debug.Log("TESTING PART 3");      

                            
                            
                        }
                        else
                        {
                            MetaManager.inChallengePlayer = pv.Owner;
                        }
                       
                        //SelectWeapon();
                        AudioManager.insta.playSound(4);
                    }
                    else
                    {
                        //Debug.Log(info.Sender + " rejected fight");
                        AudioManager.insta.playSound(5);
                        UIManager.insta.UpdateStatus(PhotonNetwork.CurrentRoom.Players[photonEvent.Sender].NickName + " rejected fight");
                    }
                }
            }
        }       
    }
    #endregion

    #region Trigger Player
    bool _waitToReattack = false;
    private void OnTriggerEnter(Collider other)
    {
        if ( !MetaManager.isPlayingMinigame)
        {
            if (!pv.IsMine && (bool)pv.Owner.CustomProperties["isfighting"] == false && !MetaManager.inVirtualWorld)
            {
                if (other.CompareTag("Meet") && !other.GetComponentInParent<PlayerController>().isPlayingMiniGame && !MetaManager.isPlayingMinigame)
                {
                    Debug.Log("Meet him");
                    meetUI.SetActive(true);
                    //virtualWorldUI.SetActive(true);
                }
            }
        }
       

        if (MetaManager.isPlayingMinigame)           
        {
            if (other.transform.CompareTag("finish"))
            {   
                
            }
            else if (other.transform.CompareTag("Coin"))
            {

                    Destroy(other.gameObject);
             
            }                          
            
        }
    }
    IEnumerator waitToReattack()
    {
        _waitToReattack = true;
        Debug.Log("Attacked " + pv.Owner);
        // UpdateHealth();
        AudioManager.insta.playSound(playerNo);
        pv.RPC("UpdateHealth", RpcTarget.All, pv.Owner.UserId);
        yield return new WaitForSeconds(0.85f);
        _waitToReattack = false;
    }
    private void OnTriggerExit(Collider other)
    {
        //if (MetaManager.isPlayingMinigame) return;
        if (!pv.IsMine)
        {
            if (other.CompareTag("Meet"))
            {
                Debug.Log("Meet bye");
                meetUI.SetActive(false);
               // virtualWorldUI.SetActive(false);
            }
        }
    }

    #endregion
    public void StopMovement()
    {
        can_move = false;
        can_rotate = false;
    }
    public void ResumeMovement()
    {
        can_move = true;
        can_rotate = true;
    }
    public void SetBurntNFTStatus(string result)
    {
        //Debug.Log(result); 
        switch (result)
        {
            case "400":
                {
                    isDoubleCoins = true;                    
                    if (!power_2x.activeInHierarchy)
                    {
                        power_2x.SetActive(true);
                    }
                    break;
                }
            case "401":
                {
                    isFreeGames = true;
                    if (!power_free.activeInHierarchy)
                    {
                        power_free.SetActive(true);
                    }                  
                    break;
                }
            case "-1":
                {
                    isFreeGames = false;
                    isDoubleCoins = false;

                    if (power_free.activeInHierarchy)
                    {
                        power_free.SetActive(false);
                    }
                    if (power_2x.activeInHierarchy)
                    {
                        power_2x.SetActive(false);
                    }
                    break;
                }
        }
    }
    public void ClaimPrize(int prize_index)
    {
        switch (prize_index)
        {
            case 0:
                {
                    //1 Coin
                    data.localdata.score++;
                    data.UpdateData();
                    UIManager.insta.UpdatePlayerUIData(true, data.localdata);
                    break;
                }
            case 1:
                {
                    //2 Coin
                    data.localdata.score+=2;
                    data.UpdateData();
                    UIManager.insta.UpdatePlayerUIData(true, data.localdata);
                    break;
                }
            case 2:
                {   
                    //BETTER LUCK NEEXT TIME
                    break;
                }
            case 3:
                {
                    //5 Coin
                    data.localdata.score += 5;
                    data.UpdateData();
                    UIManager.insta.UpdatePlayerUIData(true, data.localdata);
                    break;
                }
            case 4:
                {
                    //isFreeGames = true;

                    // BlockChainManager.Instance.purchaseItem(0, true);
                    data.localdata.score += 10;
                    data.UpdateData();
                    UIManager.insta.UpdatePlayerUIData(true, data.localdata);
                    break;
                }
            case 5:
                {
                    BlockChainManager.Instance.purchaseItem(0, false);
                    //isDoubleCoins = true;
                    break;
                }
        }
        ResumeMovement();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {      

        if (pv.IsMine)
        {
            Debug.Log("HIT"+ hit.transform.name);
            if (hit.transform.CompareTag("barrier"))
            {
                if (MetaManager.isPlayingMinigame)
                {
                    if (MetaManager.inChallengePlayer != null)
                    {
                        pv.RPC("GameOver", MetaManager.inChallengePlayer);
                    }
                    RaceOver(false);
                }
            }         
        }
    }


    public void SetMaterials()
    {
        RaceObjectPool.Instance.SetMaterials(player_selected_road);
    }
    public void SelectMaterial(int index)
    {
        player_selected_road = index - 399;
        data.localdata.selected_road = index - 399;

        Debug.Log(player_selected_road);
        data.UpdateData();
        SetMaterials();
    }
    public void RaceOver(bool _won)
    {
        if (pv.IsMine)
        {
          //  this.transform.position = lastWorldPos;
            isDragging = false;
            animator.SetFloat("speed", 0);
            Debug.Log(" I won the game? " + _won + " ,Race Timer is "+ race_timer);
            isRaceBegan = false;
            MetaManager.isPlayingMinigame = false;
            isSprinting = false;
            can_move = true;
            RaceObjectPool.isRaceOn = false;
            RaceObjectPool.Instance.ResetRace();

            cc.enabled = false;
            this.transform.position = lastWorldPos;
            cc.enabled = true;

            int stars_got = (int)race_timer / 2;
            UIManager.insta.ShowGameCompleteUI(_won, stars_got);
            data.localdata.score += stars_got;
            data.UpdateData();
            UIManager.insta.UpdatePlayerUIData(true, data.localdata);
           

            StartCoroutine(Resetplayer());
        }
    }
    public void IncreaseXP(bool won)
    {
        //if (won)
        {
            data.localdata.xp += 100;

            if (data.localdata.xp >= (data.localdata.level * 100) + 100)
            {
                //if (data.localdata.level < 5)
                {
                    data.localdata.level++;
                    data.localdata.xp -= data.localdata.level * 100;
                    data.UpdateData();
                }
            }
           
        }

        LeanTween.value(level_progressBar.gameObject, (float)level_progressBar.fillAmount, (float)data.localdata.xp / ((float)(data.localdata.level + 1) * 100), 0.5f).setOnUpdate((float v) => {
            level_progressBar.fillAmount = v;
        });

        UIManager.insta.ShowNFTPopup(data.localdata.level);
        txt_level_no.text = data.localdata.level.ToString();
        
        UIManager.insta.UpdatePlayerUIData(true, data.localdata);
    }
    IEnumerator Resetplayer()
    {
        yield return new WaitForSeconds(0.5f);
        fpsCam.Priority = 10;
        hardFollowCam.Priority = 0;
        UIManager.insta.ToggleExtraButtons(true);
    }



    [PunRPC]
    void GameOver()
    {
        // if (pv.Owner.UserId.Equals(_uid))
        {
            Debug.Log("RPC GOT",this.gameObject);
            MetaManager.insta.myPlayer.GetComponent<PlayerController>().RaceOver(true);

        }
       /* else if (pv.Owner.UserId.Equals(_p2uid))
        {
            ResetFight();

            if (pv.IsMine)
            {
                Debug.Log("UserWon");
                AudioManager.insta.playSound(7);
               // SingletonDataManager.userData.fightLose++;
                //SingletonDataManager.insta.UpdateUserDatabase();
                UIManager.insta.ShowResult(1);
            }
        }*/
    }



    void ResetFight()
    {
        if (pv.IsMine)
        {
            //var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            MetaManager.isPlayingMinigame = false;
            // PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            cc.enabled = false;
            this.transform.position = lastWorldPos;
            cc.enabled = true;
            
        }
    }

    private bool IsGrounded()
    {
        ray = new Ray(this.transform.position + Vector3.up * 0.01f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.05f, groundLayer))
        {           
            return true;
        }
        else
        {         
            return false;
        }
    }

    #region Photon Callbacks

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (targetPlayer.UserId.Equals(MetaManager._fighterid))
        {
            if (!(bool)targetPlayer.CustomProperties["isfighting"] && healthUI.activeSelf)
            {
                if ((bool)pv.Owner.CustomProperties["isfighting"] && pv.IsMine)
                {
                    Debug.Log("User Winner");
                    AudioManager.insta.playSound(6);
                    if (SingletonDataManager.insta)
                    {
                        SingletonDataManager.userData.fightWon++;
                        SingletonDataManager.userData.score++;
                        SingletonDataManager.insta.UpdateUserDatabase();
                    }
                    UIManager.insta.ShowResult(0);
                }

                showHealthBar(false);
                ResetFight();
              //  ResetWeapon();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);

        if (pv.IsMine)
        {
            if (otherPlayer.UserId.Equals(MetaManager._fighterid))
            {
                if ((bool)otherPlayer.CustomProperties["isfighting"] && healthUI.activeSelf)
                {
                    AudioManager.insta.playSound(9);

                    Debug.Log("Player left");
                    showHealthBar(false);
                    ResetFight();
                    //ResetWeapon();
                    UIManager.insta.ShowResult(2);
                }

            }
        }
    }
    #endregion
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}
