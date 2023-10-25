using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
public class PlayerController : NetworkBehaviour
{
    public GameObject MainCameraObject;
    public GameObject SideCameraObject;
    public GameObject FirstCameraObject;
    
    public Camera MainCamera;
    public Camera SideCamera;
    public Camera FirstCamera;
    [Networked]
    private Angle _yaw { get; set; }
    [SerializeField]
    private float _speed = 5f;
    [Networked]
    private Angle _pitch { get; set; }
    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterController = null;
    [SerializeField]
    private Bullet bulletPrefab;
    //FPS關卡(專屬控制)的編號指定
    private int FPS_Level=3;
    
    private Vector3 startPoint;
    [SerializeField]
    private float fir = -1, sec = -1, thi = -1, fou = -1, cha;                                    // 排名用
    private int   firS = 0, secS = 0, thiS = 0, fouS = 0;                                         // 分數用
    private int [] arr = {0,0,0,0};                                                               // 分數用
    private string firN, secN, thiN, fouN, chaN;
    private string firN_2, secN_2, thiN_2, fouN_2, chaN_2;
    private int xxx = 0, yyy = 0 , pp = 0 ,cc = 0 , ppp = 0;
    [SerializeField]
    private GameObject scoreObject;
    private GameObject timeObject;
    public GameObject wallObject;
    public GameObject rankingObject;//
    public GameObject countdownTimerObject;
    public GameObject bulletCountObject;
    public GameObject AudioManagerObject;//music
    private float totalDistance;
    private Collider finishCollider;
    private GameObject finishObject;
    private GameObject finishObjectTemp;
    private Vector3 spawnPosition = Vector3.zero;
    [SerializeField]
    private float moveSpeed = 15f;
    [SerializeField]
    private Text UIname = null;
    [SerializeField]
    private Image hpBar = null;
    [SerializeField]
    private int maxHp = 100;
    [SerializeField]
    private int maxBullet = 5;
    [SerializeField]
    private float maxDist = 100;
    //AudioSource
    public GameObject AudioManagerPrefab;

    public AudioClip bgmBackground; // 背景音樂
    public AudioClip bgmBackgroundFPS; // 背景音樂
    public AudioClip seShoot;// 碰撞音效槍
    public AudioClip seCollision;// 碰撞音效
    public AudioClip seDamage;// 碰撞音效被打到
    public AudioClip seCactus;// 碰撞音效被打到

    private AudioSource backgroundMusicSource;
    private AudioSource collisionSoundSource;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> backAudios = new Dictionary<string, AudioClip>();
    //本地計時器
    private float timer = 0;
    public GameObject timerPrefab;
    public GameObject bulletCountPrefab;
    public GameObject countDownPrefab;
    public GameObject FinishPlanePrefab; 
    [Networked] public float Dist { get; set; }
    //玩家血量
    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }
    //玩家子彈數量
    [Networked]
    public int bulletCount { get; set; }
    [Networked]
    public int playerCount { get; set; }
    //玩家名稱
    [Networked(OnChanged = nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }
    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }

    //不用網路字典了，直接在每個玩家裡面存一個網路變數，Server端可以直接呼叫所有玩家來回傳(用function)
    [Networked]
    [field: SerializeField]
    public float distance { get; private set; }

    [Networked]
    [Capacity(4)] // Sets the fixed capacity of the collection
    NetworkArray<NetworkString<_32>> ScoreLeaderboard { get; } =
    MakeInitializer(new NetworkString<_32>[] { "-1", "-1", "-1", "-1" });   // 排名
    [Networked()]
    public int finish3PositionIndex { get; set; }
    private Dictionary<int,Vector3> finish3SpawnPositions = new Dictionary<int,Vector3>()
    {
        {1, new Vector3(27.40012f,58.84f,-350.8f)},
        {2, new Vector3(-5.9f, 58.84f, -269f)}, 
        {3, new Vector3(-2, 2, 0)}
    };
    [Networked]
    [Capacity(4)] // Sets the fixed capacity of the collection
    NetworkArray<NetworkString<_32>> FinalScoreBoard { get; } =
    MakeInitializer(new NetworkString<_32>[] { "-2", "-2", "-2", "-2" });       // 分數 

    [Networked]
    [Capacity(4)] // Sets the fixed capacity of the collection
    NetworkArray<int> ScoreBoard { get; } =
    MakeInitializer(new int[] { 0,0,0,0 }); 

    [SerializeField]
    private MeshRenderer meshRenderer = null;
    private static readonly float FinishThreshold = 1.0f; // 這個值代表角色距離終點多近時算是已經抵達。
    private GameObject timerInstance;
    public bool isMainCamera=true;
    public bool isSideCamera = false;
    public bool gotonext = false;
    public int currentCameraMode = 0;
    public InputActionAsset myActions;

    private void Awake()
    {
        //backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        //backgroundMusicSource.clip = bgmBackground;
        
        collisionSoundSource = gameObject.AddComponent<AudioSource>();

        //originalBackgroundMusicVolume = backgroundMusicSource.volume;

        basicSpawner = FindObjectOfType<BasicSpawner>(); // 取得 BasicSpawner 的實例
        firstCamera = FindObjectOfType<FirstCamera>();
        myActions.Enable();
    }


    private void InstantiateHUD_UI()
    {
        if (Object.HasInputAuthority)
        {
            //Runner.Spawn(HUD_UI_Prefab);
            GameObject timerInstance = Instantiate(timerPrefab);
            GameObject bulletCountInstance = Instantiate(bulletCountPrefab);
            GameObject countDownInstance = Instantiate(countDownPrefab); //
            GameObject FinishPlaneInstance = Instantiate(FinishPlanePrefab); //
        }
    }
    public CountdownTimer countdownTimer;
    public FinishPlane finishPlane;
    private FirstCamera firstCamera;
    private BasicSpawner basicSpawner;  //引用
    

    void Start()
    {
        backAudios.Add("background", bgmBackground);
        backAudios.Add("shootbackg", bgmBackgroundFPS);
        audioClips.Add("shoot", seShoot);
        audioClips.Add("collision", seCollision);
        audioClips.Add("damage", seDamage);
        audioClips.Add("cactus", seCactus);
    }

    public void SomeMethod(Dictionary<string, AudioClip> spawnPositions)
    {
        // 確保你有有效的索引來訪問陣列中的值
        int levelIndex = basicSpawner.levelIndex;

        if (levelIndex == 1 || levelIndex == 2)
        {
            // 如果是前兩關
            string selectedSound = "background";
            if (backAudios.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = backAudios[selectedSound];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = true;
            }
        }
        else if (levelIndex == 3)
        {
            // 如果是最後一關
            string selectedSound = "shootbackg";
            if (backAudios.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = backAudios[selectedSound];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = true;
            }
        }
    }


    public override void Spawned()
    {
        
        //ReloadLevel();
        //根據不同關卡去制定不同的終點
        if(basicSpawner.levelIndex==1)
        {
            finishObject = GameObject.FindGameObjectWithTag("Finish1");
        }
        else if(basicSpawner.levelIndex==2)
        {
            finishObject = GameObject.FindGameObjectWithTag("Finish2");
        }
        else if(basicSpawner.levelIndex==3)
        {
            finishObject = GameObject.FindGameObjectWithTag("Finish3");
        }
        if (Object.HasInputAuthority)
        {
            if (SceneManager.GetActiveScene().name == "FPS")
            {
                MainCamera.enabled = false;
                SideCamera.enabled = false;
                FirstCamera.enabled = true;
                firstCamera.GetComponent<AudioListener>().enabled = true;

            }
            else
            {
                firstCamera.enabled = false;
                firstCamera.GetComponent<AudioListener>().enabled = false;
            }
            
            SetPlayerName_RPC(PlayerPrefs.GetString("PlayerName"));
            //根據不同關卡去制定不同的終點
            if(basicSpawner.levelIndex==1)
            {
                finishObject = GameObject.FindGameObjectWithTag("Finish1");
            }
            else if(basicSpawner.levelIndex==2)
            {
                finishObject = GameObject.FindGameObjectWithTag("Finish2");
            }
            finishCollider = finishObject.GetComponent<Collider>(); 
            
            totalDistance = Vector3.Distance(startPoint, finishObject.transform.position);
            InstantiateHUD_UI();
            maxDist = CalculateDistancePercentage();
            //EnablePlayerControl_RPC();
        }
        else
        {
            firstCamera.enabled = false;
            firstCamera.GetComponent<AudioListener>().enabled = false;
        }
        if (Object.HasStateAuthority)
        {
            playerCount=pp;
            bulletCount=maxBullet;
            Hp = maxHp;
        }
        


    }
    private float CalculateDistancePercentage()
    {
        //Vector3 closestPointOnBounds = finishCollider.boun  ds.ClosestPoint(transform.position);
        float currentDistance = Vector3.Distance(transform.position, finishObject.transform.position);
        //Debug.Log($"Absolute distance to the finish edge: {currentDistance}");
        return currentDistance;
    }

    public void ReloadLevel()//用來載入下一關，此已將兩個StartWall恢復原位
    {
        a=1;
        wallObject = GameObject.FindGameObjectWithTag("StartWall1");
        Vector3 p = wallObject.transform.position;
        p.y = 0f;
        wallObject.transform.position = p;
        print(wallObject.transform.position);
        wallObject = GameObject.FindGameObjectWithTag("StartWall2");
        p = wallObject.transform.position;
        p.y = 0f;
        wallObject.transform.position = p;
        print(wallObject.transform.position);
    }
    
    public void ReloadFinish()//用來載入下一關，將終點位置恢復原位
    {
        finishObjectTemp = GameObject.FindGameObjectWithTag("Finish1");
        //確保離得夠遠
        while( Mathf.Abs(networkCharacterController.transform.position.x-finishObjectTemp.transform.position.x)<=2)
        {
            
        }
        Vector3 p = wallObject.transform.position;
        finishObjectTemp = GameObject.FindGameObjectWithTag("Finish1");
        p = finishObjectTemp.transform.position;
        p.y = 1.17f;
        finishObjectTemp.transform.position = p;
        finishObjectTemp = GameObject.FindGameObjectWithTag("Finish2");
        p = finishObjectTemp.transform.position;
        p.y = 4.05f;
        finishObjectTemp.transform.position = p;
        
    }
    
    public override void FixedUpdateNetwork()
    {
        bulletCountObject = GameObject.FindGameObjectWithTag("BulletCount");
        timeObject = GameObject.FindGameObjectWithTag("Timer");
        scoreObject = GameObject.FindGameObjectWithTag("Score");
        rankingObject = GameObject.FindGameObjectWithTag("FinPlane");
        
        //定期更新子彈數量
        Text bulletCountText = bulletCountObject.GetComponent<Text>();
        bulletCountText.text=bulletCount.ToString();
        if (GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);
            ButtonsPrevious = buttons;

            var moveInput = new Vector3(data.MoveInput.y, 0, data.MoveInput.x);
            networkCharacterController.Move(transform.rotation * moveInput * _speed * Runner.DeltaTime);
            if(isMainCamera)
            {
                _yaw=0;
                _pitch=0;
            }
            else
            {
                HandlePitchYaw(data);
            }
            
            if (pressed.IsSet(InputButtons.JUMP))
            {
                networkCharacterController.Jump();
            }
            if (pressed.IsSet(InputButtons.FIRE))
            {
                string selectedSound = "shoot";
                if (audioClips.ContainsKey(selectedSound))
                {                
                    GetComponent<AudioSource>().clip = audioClips[selectedSound];
                    GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                    GetComponent<AudioSource>().loop = false;

                    //backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.7f;

                }

                //發射子彈(子彈數量的檢測)
                if (bulletCount > 0)
                {
                    // 創建一個向前方旋轉 90 度的 Quaternion
                    Quaternion rotation = Quaternion.Euler(0, 90, 0);
                    // 使用轉向旋轉子彈方向
                    Quaternion bulletRotation = Quaternion.LookRotation(rotation * transform.TransformDirection(Vector3.forward));

                    Runner.Spawn(
                        bulletPrefab,
                        transform.position + bulletRotation * Vector3.forward, // 使用旋轉後的方向
                        bulletRotation,
                        Object.InputAuthority);

                    bulletCount--;
                    print("bulletCount:" + bulletCount);
                }

            }

            
        }
        

        if (ShouldRespawn()==true)
        {
            Respawn();
            a = 0;
        }
        if (MiddleRespawn() == true)
        {
            Respawn();
            a = 2;
        }
        if (frozen == 1)
        {
            StartCoroutine(FreezePlayerForSeconds(5.0f));
            frozen = 0;
        }

        if (HasStateAuthority)
        {
            distance = CalculateDistancePercentage();
        }
        transform.rotation = Quaternion.Euler(0, (float)_yaw,(float)_pitch);

        //var cameraEulerAngle = firstCamera.transform.rotation.eulerAngles;
        //firstCamera.transform.rotation = Quaternion.Euler((float)_pitch, cameraEulerAngle.y, cameraEulerAngle.z);

       
        if(HasInputAuthority)
        if (gotonext == true)
        {

            gotoFPS_RPC();
            gotonext = false;
        }

    }
    
    
    int a = 0;
    private bool ShouldRespawn()
    {
       if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f)
            return true;
       if (a==1)
       {
            return true;
       }
       if (a == 3)
       {
           return true;
       }
        else
            return false;
    }

    private bool MiddleRespawn()
    {
        int a = 2;
        if (Hp <= 145 && networkCharacterController.transform.position.y <= -5f)
            return true;
        //上述if再加一個條件是超過中間的
        if (a == 3)
        {
            return true;
        }
        else
            return false;
    }

    int frozen = 0; 
    private IEnumerator FreezePlayerForSeconds(float seconds)
    {
        if (frozen == 1)
        {
            moveSpeed = 0f;
            yield return new WaitForSeconds(seconds); // 等待指定的秒數
            moveSpeed = 13f; // 恢復移動速度
        }
        frozen = 0;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 檢查是否已經抵達終點
        if (other.gameObject == finishObject)
        {
            OnReachedFinish();
            gotonext = true;
            //Destroy(other.gameObject);
            Vector3 p = other.transform.position;
            p.y = 50f;
            other.transform.position = p;
        }
        if (other.gameObject.CompareTag("trapdead"))
        {
            Debug.Log("Trapdead object collision!");
            // Respawn();
            a = 1;
        }
        if (other.gameObject.CompareTag("Frozen"))
        {
            Debug.Log("Trapdead frozen!");
            frozen = 1;
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            CoinPoint_RPC(this.PlayerName.ToString());
            cc=0;
            Debug.Log("Get Coin!");
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Clip"))
        {
            Debug.Log("Clip Get!");
            bulletCount+=5;
            print("bulletCount:"+bulletCount);
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Treasure")
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Soundtest"))
        {
            Debug.Log("Soundtest object collision!");

            string selectedSound = "collision";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = false;

                //backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.7f;
            }

            // Respawn();
            a = 1;

            
        }

        if (other.gameObject.CompareTag("Soundcactus"))
        {
            Debug.Log("Soundcactus object collision!");

            string selectedSound = "cactus";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = false;

                //backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.7f;
            }

            // Respawn();
            a = 1;

            
        }

        if (other.gameObject.CompareTag("middleSpawn"))
        {
            Debug.Log("middleSpawn object collision!");

            string selectedSound = "collision";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = false;

                //backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.7f;
            }

            // Respawn();
            a = 3;
        }
    }

    private void OnReachedFinish()
    {
        FinishPlane finishPlane = FindObjectOfType<FinishPlane>();
        if (finishPlane != null)
        {
            DistRutern_RPC();                                        // 限制次數
            Finish_RPC(this.PlayerName.ToString());
            
        }
        // 在這裡添加您想要在角色抵達終點時執行的程式碼
        timeObject = GameObject.FindGameObjectWithTag("Timer");
        timerUI timerScript = timeObject.GetComponent<timerUI>();
        timerScript.StopTimer();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void gotoFPS_RPC()
    {
        //basicSpawner.levelIndex = 3;
        //networkCharacterController.transform.position = spawnPosition;
        networkCharacterController.transform.position = new Vector3(0, 61, -200);
        currentCameraMode = 1;
        basicSpawner.SideInput = false;

        //currentInputMode = InputMode.ModeFPS;
    }

    private void Respawn()
    {
        Vector3 spawnPosition = basicSpawner.GetSpawnPosition(basicSpawner.levelIndex, basicSpawner.playerNumber);
        print(spawnPosition);
        if (spawnPosition != Vector3.zero) // 檢查是否成功獲取重生位置
        {
            if (basicSpawner.levelIndex == 1&& networkCharacterController.transform.position.x >= 145 && networkCharacterController.transform.position.y <= -5f)
            {
                networkCharacterController.transform.position = new Vector3(145, 17, 0);
            }
            else if(basicSpawner.levelIndex == 1 && a == 3)
            {
                networkCharacterController.transform.position = new Vector3(145, 17, 0);
            }
            else if (basicSpawner.levelIndex == 2 && networkCharacterController.transform.position.x >= 105 && networkCharacterController.transform.position.y <= -5f)
            {
                networkCharacterController.transform.position = new Vector3(105, 17, 200);
            }
            else if (basicSpawner.levelIndex == 2 && a == 3)
            {
                networkCharacterController.transform.position = new Vector3(105, 17, 200);
            }
            else if (basicSpawner.levelIndex == 3)
            {
                networkCharacterController.transform.position = spawnPosition;
            }
            else
            {
                networkCharacterController.transform.position = spawnPosition; // 使用原來的重生位置
            }
        }
        else
        {
            // 如果無法獲取重生位置，可以設定一個默認的重生位置
            networkCharacterController.transform.position = Vector3.up * 2;
        }
        Hp = maxHp; 
    }


    static void OnNameChanged(Changed<PlayerController> changed)
    {
        Debug.Log($"Name changed for player to {changed.Behaviour.PlayerName}");
        changed.Behaviour.UIname.text = changed.Behaviour.PlayerName.ToString();
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void SetPlayerName_RPC(string name,RpcInfo info=default)
    {
        Debug.Log($"[RPC] SetName {name}");
        this.PlayerName= name;
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            Hp -= damage;
        }
    }
    private static void OnHpChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
    }
    

    private void Update()
    {
        MainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        SideCameraObject = GameObject.FindGameObjectWithTag("SideCamera");
        FirstCameraObject = GameObject.FindGameObjectWithTag("FirstCamera");
        MainCamera = MainCameraObject.GetComponent<Camera>();
        FirstCamera = FirstCameraObject.GetComponent<Camera>();
        SideCamera = SideCameraObject.GetComponent<Camera>();
        MainCamera.enabled = currentCameraMode == 0;
        isMainCamera = currentCameraMode == 0;
        basicSpawner.SideInputToggle(currentCameraMode == 0);

        SideCamera.enabled = currentCameraMode == 1;
        //FirstCamera.enabled = currentCameraMode == 2;

        if (basicSpawner.levelIndex==1||basicSpawner.levelIndex==2)//在確認為第一關或第二關要做的事情
        {
            
        }
        else if(basicSpawner.levelIndex==FPS_Level)//為了同個.unity檔案的FPS模式(具體為哪個level要再討論，目前為3)做準備，預設會是正朝向模式
        {
            currentCameraMode = 1;
            isMainCamera=false;
            isSideCamera = true;
        }
        InputAction View = myActions.FindAction("View");
        View.started += ctx=>switchView();
        InputAction Start = myActions.FindAction("Start");
        Start.started += ctx=>StartGame();
        void switchView()
        {
            if(basicSpawner.levelIndex==1||basicSpawner.levelIndex==2)//在第一關或第二關，按鍵切換模式
            {
                currentCameraMode = (currentCameraMode + 1) % 2;
                
            }
            else if(basicSpawner.levelIndex==FPS_Level)//為了同個.unity檔案的FPS模式做準備，預設會是正朝向模式，按鍵不會切換
            {
                currentCameraMode = 1;
                
            }
            else
            {

            }
        }
        void StartGame()
        {
            if(basicSpawner.levelIndex==1)
            {
                wallObject = GameObject.FindGameObjectWithTag("StartWall1");
            }
            if(basicSpawner.levelIndex==2)
            {
                wallObject = GameObject.FindGameObjectWithTag("StartWall2");
            }
            print(wallObject.transform.position.y);
            if(wallObject.transform.position.y<=20)//當牆還在原位
            {
                timeObject = GameObject.FindGameObjectWithTag("Timer");
                StartWall startWallScript = wallObject.GetComponent<StartWall>(); 
                if (startWallScript != null)
                {
                    StartM_RPC();     
                }
                timerUI timerScript = timeObject.GetComponent<timerUI>();
                if (timerScript != null)
                {
                    //timerScript.StartTimer();
                    // 在這裡訪問 timerScript 或執行相關操作
                    //之後要實作timer相關的rpc呼叫
                }
                else
                {
                    print("錯誤");
                }
            }
        }
        //切換鏡頭模式
        if (Input.GetKeyDown(KeyCode.C))
            switchView();
        MainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        SideCameraObject = GameObject.FindGameObjectWithTag("SideCamera");
        MainCamera = MainCameraObject.GetComponent<Camera>();
        SideCamera = SideCameraObject.GetComponent<Camera>();
        MainCamera.enabled = currentCameraMode == 0;
        isMainCamera = currentCameraMode == 0;
        basicSpawner.SideInputToggle(currentCameraMode == 0);
        SideCamera.enabled = currentCameraMode == 1;
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            ChangeColor_RPC(Color.red);
            Reload_RPC();//測試:按R來Reload
        }
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.U))
        {
            print(distance);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeColor_RPC(Color.green);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeColor_RPC(Color.blue);
        }
        if (Input.GetKeyDown(KeyCode.Return))
            StartGame();
        
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            //CalculateDistancePercentage();
            timer = 0;
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]                                               // 顏色更換_RPC
    private void ChangeColor_RPC(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]                                               // 顏色更換_RPC
    private void Reload_RPC()//所有關於重載的事情在此進行(包含牆壁重置和終點重置)
    {
        ReloadLevel();
        ReloadFinish();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]                                                                // 終點＿RPC
    public void Finish_RPC(string a)
    {
        print("Player:"+a+" Is the First Place.");
        DistRutern_RPC();
        FinishPlane finishPlane = FindObjectOfType<FinishPlane>();
        ppp=playerCount;
        if (finishPlane != null)
        {
            finishPlane.FinishClick();
            if( basicSpawner.levelIndex == 1 || basicSpawner.levelIndex == 2 )          //在第一關或第二關
            {
                FinalPlaneDisplay_RPC();
                CalculateAndSyncScores();
            }
            else if( basicSpawner.levelIndex == 3 )                              //在第三關
            {
                CalculateAndSyncScores();
                TotalScoreDisplay_RPC();
            }
        }

        
        cc = 0;
        xxx = 0;
        yyy += 1;
        gotonext = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]                                                                      // 排名顯示FP＿RPC
    public void FinalPlaneDisplay_RPC()
    {
        rankingObject = GameObject.FindGameObjectWithTag("RankingText");
        TextMeshProUGUI rankingText = rankingObject.GetComponent<TMPro.TextMeshProUGUI>();
        
        rankingText.text="";
        for (int i = 0; i < ScoreLeaderboard.Length; ++i)
        {
            rankingText.text=rankingText.text+"\n"+(i+1)+" : "+ScoreLeaderboard[i] ;
        }      
    }



    [Rpc(RpcSources.All, RpcTargets.All)]                                                                      // 最終排名顯示＿RPC
    public void TotalScoreDisplay_RPC()
    {
        rankingObject = GameObject.FindGameObjectWithTag("RankingText");
        TextMeshProUGUI rankingText = rankingObject.GetComponent<TMPro.TextMeshProUGUI>();
        rankingText.text="";
        for (int i = 0; i < playerCount; i++)
        {
            if(i == 0)
            {
                rankingText.text=rankingText.text+"\n"+FinalScoreBoard[0]+" WIN ! : " + arr[0] + " Points" ;
            }
            else if(i == 1)
            {
                rankingText.text=rankingText.text+"\n"+FinalScoreBoard[1]+" 2nd : "+arr[1] + " Points";
            }
            else if(i == 2)
            {
                rankingText.text=rankingText.text+"\n"+FinalScoreBoard[2]+" 3rd : "+arr[2] + " Points";
            }
            else if(i == 3)
            {
                rankingText.text=rankingText.text+"\n"+FinalScoreBoard[3]+" 4th : "+arr[3] + " Points";
            }
        }        
    }

    [Rpc(RpcSources.All, RpcTargets.All)]                                                                      // 分數顯示＿RPC
    public void FinalScoreDisplay_RPC()
    {
        scoreObject = GameObject.FindGameObjectWithTag("scoreText");
        TextMeshProUGUI scoreText = scoreObject.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.text="";
        for (int i = 0; i < playerCount; i++)
        {
            scoreText.text=scoreText.text+"\n"+FinalScoreBoard[i]+" : "+arr[i] ;
        }        
    }

    public void CalculateAndSyncScores()
    {
        for (int i = 0; i < playerCount && cc==0 && ppp > 0; i++)
        {
            if (FinalScoreBoard[i] == ScoreLeaderboard[3] && playerCount >= 4 ){
                arr[i] = arr[i]+4;
                ScoreBoard.Set(i, arr[i]);
                ppp--;
            }
            else if (FinalScoreBoard[i] == ScoreLeaderboard[2] && playerCount >= 3){
                arr[i] = arr[i]+6;
                ScoreBoard.Set(i, arr[i]);
                ppp--;
            }
            else if (FinalScoreBoard[i] == ScoreLeaderboard[1] && playerCount >= 2){
                arr[i] = arr[i]+8;
                ScoreBoard.Set(i, arr[i]);
                ppp--;
            }
            else if (FinalScoreBoard[i] == ScoreLeaderboard[0] && playerCount >= 1){
                arr[i] = arr[i]+10;
                ScoreBoard.Set(i, arr[i]);
                ppp--;
            }
        }
        cc=cc+1;
        if( basicSpawner.levelIndex == 1 || basicSpawner.levelIndex == 2 )        // 第一關第二關時，調用 FinalScoreDisplay_RPC
        {
            FinalScoreDisplay_RPC();
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]                                                                   // 金幣分數＿RPC試作
    public void CoinPoint_RPC(string a)
    {
        if(yyy == 0)
        {
            DistRutern_RPC();
        }
        if (FinalScoreBoard[0] == a && cc == 0){
            arr[0]++;
            cc++;
        }
        else if (FinalScoreBoard[1] == a && cc == 0){
            arr[1]++;
            cc++;
        }
        else if (FinalScoreBoard[2] == a && cc == 0){
            arr[2]++;
            cc++;
        }
        else if (FinalScoreBoard[3] == a && cc == 0){
            arr[3]++;
            cc++;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void StartM_RPC()
    {
        if(Object.HasStateAuthority)
        {
            finish3PositionIndex = Random.Range(1, 3); //Host隨機選擇Index值
        }
        if(basicSpawner.levelIndex==3)
        {
            finishObject.transform.position=finish3SpawnPositions[finish3PositionIndex];
        }
        else
        {
            if(basicSpawner.levelIndex==1)
            {
                wallObject = GameObject.FindGameObjectWithTag("StartWall1");
                Vector3 p = wallObject.transform.position;
                p.y = 50f;
                
                wallObject.transform.position = p;
                print(wallObject.transform.position);
            }
            if(basicSpawner.levelIndex==2)
            {
                wallObject = GameObject.FindGameObjectWithTag("StartWall2");
                Vector3 p = wallObject.transform.position;
                p.y = 50f;
                
                wallObject.transform.position = p;
                print(wallObject.transform.position);
            }
        }
        timeObject = GameObject.FindGameObjectWithTag("timerText");
        CountdownTimer countdownTimer = FindObjectOfType<CountdownTimer>();
        if (countdownTimer != null)
        {
            countdownTimer.SetCountdownTime();
        }
        else
        {
            Debug.LogError("countdownTimer is null! Make sure it's properly initialized.");
        }
        TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
        timerText.text=" Start ! ";
        Invoke("C0", 5 );
    }
    private void C0(){                                                                                         // 倒數用
        timeObject = GameObject.FindGameObjectWithTag("timerText");
        TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
        timerText.text=" ";
    }

    private void HandlePitchYaw(NetworkInputData data)
    {
        _yaw   += data.Yaw;
        _pitch += data.Pitch;
        

        if (_pitch >= 180 && _pitch <= 329)
        {
            _pitch = 330;
        }
        else if (_pitch <= 180 && _pitch >= 29)
        {
            _pitch = 28;
        }
        //print(_pitch);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DistRutern_RPC()
    {
        if (HasStateAuthority)
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in allPlayers)
            {
                // 先檢查遊戲物件是否有你想要的 Component（比如你的 PlayerController 或其他）
                PlayerController playerController = player.GetComponent<PlayerController>();
                if(playerController != null)
                {
                    if(xxx == 0){
                        fir=playerController.distance;
                        firN=playerController.PlayerName.ToString();
                        if(yyy == 0){
                            playerCount ++;
                        }
                        xxx++;
                    }
                    else if(xxx == 1 && playerController.PlayerName.ToString() != firN){
                        sec=playerController.distance;
                        secN=playerController.PlayerName.ToString();
                        xxx++;
                        if(yyy == 0){
                           playerCount ++;
                        }
                        if(sec<=fir){
                            cha=sec;    //2->1
                            chaN=secN;
                            sec=fir;
                            secN=firN;
                            fir=cha;
                            firN=chaN;
                        }
                    }
                    else if(xxx == 2 && playerController.PlayerName.ToString() != secN && playerController.PlayerName.ToString() != firN){
                        thi=playerController.distance;
                        thiN=playerController.PlayerName.ToString();
                        xxx++;
                        if(yyy == 0){
                           playerCount ++;
                        }
                        if(thi<=fir){
                            cha=thi;  // 3->1
                            chaN=thiN;
                            thi=fir;
                            thiN=firN;
                            fir=cha;
                            firN=chaN;

                            cha=sec;   // 2->3
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                        else if(thi <= sec){
                            cha=sec;
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                    }
                    else if(xxx == 3 && playerController.PlayerName.ToString() != thiN && playerController.PlayerName.ToString() != secN && playerController.PlayerName.ToString() != firN){
                        fou=playerController.distance;
                        fouN=playerController.PlayerName.ToString();
                        xxx++;
                        if(yyy == 0){
                            playerCount ++;
                        }
                        if(fou<=fir){
                            cha=fou;
                            chaN=fouN;
                            fou=fir;
                            fouN=firN;
                            fir=cha;
                            firN=chaN;

                            cha=fou;
                            chaN=fouN;
                            fou=thi;
                            fouN=thiN;
                            thi=cha;
                            thiN=chaN;

                            cha=sec;
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;

                        }
                        else if(fou<=sec){
                            cha=fou;     // 4->3
                            chaN=fouN;
                            fou=thi;
                            fouN=thiN;
                            thi=cha;
                            thiN=chaN;

                            cha=sec;   // 3->2
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                        else if(fou<=thi){
                            cha=fou;     // 4->3
                            chaN=fouN;
                            fou=thi;
                            fouN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                    }
                        if(yyy == 0 ){
                            FinalScoreBoard.Set(0, firN);
                        }
                        ScoreLeaderboard.Set(0, firN);
                        print(firN+" Is The First Place !! ");
                        if(sec != -1){
                            ScoreLeaderboard.Set(1, secN);
                            if(yyy == 0 ){
                                FinalScoreBoard.Set(1, secN);
                            }
                            print(secN+" Is The Second Place !! ");
                            
                        }
                        if(thi != -1){
                            ScoreLeaderboard.Set(2, thiN);
                            if(yyy == 0 ){
                                FinalScoreBoard.Set(2, thiN);
                            }
                            print(thiN+" Is The Third Place !! ");
                            
                        }
                        if(fou != -1){
                            ScoreLeaderboard.Set(3, fouN);
                            if(yyy == 0 ){
                                FinalScoreBoard.Set(3, fouN);
                            }
                            print(fouN+" Is The Fourth Place !! ");
                           
                        }
                }
            }
        }
    }
}
