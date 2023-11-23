using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using System;


public class PlayerController : NetworkBehaviour
{
    private Dictionary<int, Color> colorsDict;
    public GameObject loadingOverlay;
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
    public float jumpHigh = 12f;
    public float normalJump = 13f;
    [Networked]
    private Angle _pitch { get; set; }
    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterController = null;
    [SerializeField]
    private Bullet bulletPrefab;
    //FPS關卡(專屬控制)的編號指定
    private int FPS_Level=3;
    
    private Vector3 startPoint;
    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public int score;
    }

    [System.Serializable]
    public class PlayerDataArray
    {
        public PlayerData[] players;
    }
    [System.Serializable]
    public class PlayerData2
    {
        public string name;
        public int score;
    }

    [System.Serializable]
    public class PlayerDataArray2
    {
        public PlayerData2[] players;
    }
    [SerializeField]
    private float fir = -1, sec = -1, thi = -1, fou = -1, cha;                                    // 排名用
    private int [] arr = {0,0,0,0};       
    public string [] sarr = { "-1", "-1", "-1", "-1" } ;                                                      // 分數用
    private string firN, secN, thiN, fouN, chaN;
    private int xCount = 0, yCount = 0 ;
    [SerializeField]
    private GameObject scoreObject;
    private GameObject timeObject;
    public GameObject wallObject;
    public GameObject rankingObject;
    public GameObject countdownTimerObject;
    public GameObject bulletCountObject;
    public GameObject AudioManagerObject;//music
    private float totalDistance;
    private Collider finishCollider;
    private GameObject finishObject;
    private GameObject finishObjectTemp;
    private Vector3 spawnPosition = Vector3.zero;
    [SerializeField]
    private float moveSpeed = 10f;
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
    [SerializeField]
    private int isFinish = 0;
    [SerializeField]
    private int frozen = 0;   
    [SerializeField]
    private int runfast = 0;
    [SerializeField]
    private int highhigh = 0;
    //AudioSource
    public GameObject AudioManagerPrefab;

    public AudioClip bgmBackground; // 背景音樂
    public AudioClip shootbgmFPS;//level3 backgroundmusic
    public AudioClip seShoot;// 碰撞音效槍
    public AudioClip seCollision;// 碰撞音效
    public AudioClip seDamage;// 碰撞音效被打到
    public AudioClip seCactus;// 碰撞音效被打到

    private AudioSource shootMusicSource;
    private AudioSource backgroundMusicSource;
    private AudioSource collisionSoundSource;

    private Dictionary<string, List<AudioClip>> audioClips = new Dictionary<string, List<AudioClip>>();
    //本地計時器
    private float timer = 0;
    public GameObject timerPrefab;
    public GameObject bulletCountPrefab;
    public GameObject countDownPrefab;
    public GameObject FinishPlanePrefab;
    //Effects
    public GameObject runfirePrefab;
    public GameObject frozenPrefab;
    public GameObject bloodPrefab;
    public GameObject jumpPrefab;
    public int blood = 0;
    public int door = 0;

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

    /*[Networked]
    [Capacity(4)] // Sets the fixed capacity of the collection
    NetworkArray<NetworkString<_32>> ScoreLeaderboard { get; } =
    MakeInitializer(new NetworkString<_32>[] { "-1", "-1", "-1", "-1" });   // 排名*/
    [Networked()]
    public int finish3PositionIndex { get; set; }
    private Dictionary<int,Vector3> finish3SpawnPositions = new Dictionary<int,Vector3>()
    {
        {1, new Vector3(27.40012f,58.84f,-350.8f)},
        {2, new Vector3(-5.9f, 58.84f, -269f)}, 
        {3, new Vector3(-2, 2, 0)}
    };

    [SerializeField]
    private MeshRenderer meshRenderer = null;
    private static readonly float FinishThreshold = 1.0f; // 這個值代表角色距離終點多近時算是已經抵達。
    private GameObject timerInstance;
    public bool isMainCamera=true;
    public bool isSideCamera = false;
    public bool gotonext = false;
    public int currentCameraMode = 0;
    public InputActionAsset myActions;
    public int ColorNum=0;
    private int isTriggedRedCubeTips=0;
    private int istrapTips = 0;

    IEnumerator StartAsHost(string pname,string sname,int score)
    {
        yield return DeleteDataInSession(sname);
        yield return new WaitForSeconds(0.2f);
        yield return SetDataInSession(pname,sname,score);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator Set_1(string name,string sname)
    {
        yield return AddDataInSession(sname,name,10);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession(sname);
        yield return new WaitForSeconds(0.2f);

    }
    IEnumerator Set_2(string name_1,string name_2,string sname)
    {
        yield return AddDataInSession(sname,name_1,10);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_2,8);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession(sname);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator Set_3(string name_1,string name_2,string name_3,string sname)
    {
        yield return AddDataInSession(sname,name_1,10);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_2,8);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_3,6);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession(sname);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator Set_4(string name_1,string name_2,string name_3,string name_4,string sname)
    {
        yield return AddDataInSession(sname,name_1,10);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_2,8);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_3,6);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_4,4);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession(sname);
        yield return new WaitForSeconds(0.2f);
    }
    
    IEnumerator Set3_1(string name,string sname)
    {
        yield return AddDataInSession(sname,name,5);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession_2(sname);
        yield return new WaitForSeconds(0.2f);

    }
    IEnumerator Set3_2(string name_1,string name_2,string sname)
    {
        yield return AddDataInSession(sname,name_1,5);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_2,2);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession_2(sname);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator Set3_3(string name_1,string name_2,string name_3,string sname)
    {
        yield return AddDataInSession(sname,name_1,5);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_2,2);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_3,2);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession_2(sname);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator Set3_4(string name_1,string name_2,string name_3,string name_4,string sname)
    {
        yield return AddDataInSession(sname,name_1,5);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_2,2);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_3,2);
        yield return new WaitForSeconds(0.2f);
        yield return AddDataInSession(sname,name_4,2);
        yield return new WaitForSeconds(0.2f);
        yield return GetDataInSession_2(sname);
        yield return new WaitForSeconds(0.2f);
    }
    IEnumerator DeleteDataInSession(string sname)                         // SQL清除Session 
    {
        string URL="http://140.136.151.71:5000/players?mode=clearsession&sname="+WWW.EscapeURL(sname);
        Debug.Log(URL);
        var request = UnityWebRequest.Get(URL);
        
        yield return request.Send();
        
        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            yield break;
        }
        
        //var html = request.downloadHandler.text;
        //Debug.Log(html);
    }

    IEnumerator GetDataInSession(string sname)                           // SQL取得Session
    {
        string URL="http://140.136.151.71:5000/players?mode=getorderplayers&sname="+WWW.EscapeURL(sname);
        Debug.Log(URL);
        var request = UnityWebRequest.Get(URL);
        
        yield return request.Send();
        
        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            yield break;
        }
        var html = request.downloadHandler.text;
        Debug.Log(html);
        // 解析 JSON 數據
        PlayerDataArray playerDataArray = JsonUtility.FromJson<PlayerDataArray>("{\"players\":" + request.downloadHandler.text + "}");

        // 將資料轉換成兩個獨立的陣列
        List<string> namesList = new List<string>();
        List<int> scoresList = new List<int>();

        foreach (PlayerData player in playerDataArray.players)
        {
            namesList.Add(player.name);
            scoresList.Add(player.score);
        }
        // 存取資料
        string[] names = namesList.ToArray();
        int[] scores = scoresList.ToArray();
        for (int i = 0; i < names.Length; i++)
        {
            Debug.Log(names[i] + " : " + scores[i]);
        }
        SetPlane_RPC();
        FinalPlaneDisplay_RPC(names);
        FinalScoreDisplay_RPC(names,scores);
    }
    IEnumerator GetDataInSession_2(string sname)                           // SQL取得Session
    {
        string URL="http://140.136.151.71:5000/players?mode=getorderplayers&sname="+WWW.EscapeURL(sname);
        Debug.Log(URL);
        var request = UnityWebRequest.Get(URL);
        
        yield return request.Send();
        
        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            yield break;
        }
        // 解析 JSON 數據
        PlayerDataArray2 playerDataArray2 = JsonUtility.FromJson<PlayerDataArray2>("{\"players\":" + request.downloadHandler.text + "}");

        // 將資料轉換成兩個獨立的陣列
        List<string> namesList2 = new List<string>();
        List<int> scoresList2 = new List<int>();

        foreach (PlayerData2 player in playerDataArray2.players)
        {
            namesList2.Add(player.name);
            scoresList2.Add(player.score);
        }
        // 存取資料
        string[] names2 = namesList2.ToArray();
        int[] scores2 = scoresList2.ToArray();
        for (int i = 0; i < names2.Length; i++)
        {
            Debug.Log(names2[i] + " : " + scores2[i]);
        }
        SetPlane2_RPC();
        FinalPlaneDisplay_RPC(names2);
        FinalScoreDisplay_RPC(names2,scores2);
        StartCoroutine(FetchDataInSession(names2[0],scores2[0]));
    }
    IEnumerator SetDataInSession(string pname,string sname,int score)                  // SQL設定玩家初始值
    {
        string URL="http://140.136.151.71:5000/players?mode=setplayerdata&pname="+WWW.EscapeURL(pname)+"&sname="+WWW.EscapeURL(sname)+"&score="+score;
        Debug.Log(URL);
        var request = UnityWebRequest.Get(URL);
        
        yield return request.Send();
        
        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            yield break;
        }
        
        var html = request.downloadHandler.text;
        Debug.Log(html);
    }

    IEnumerator AddDataInSession(string sname,string pname,int score)                  // SQL增加玩家分數
    {
        string URL="http://140.136.151.71:5000/players?mode=addplayerscore&sname=" + WWW.EscapeURL(sname) + "&pname=" + WWW.EscapeURL(pname) + "&score= " + score;
        Debug.Log(URL);
        var request = UnityWebRequest.Get(URL);
        
        yield return request.Send();
        
        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            yield break;
        }
        
        var html = request.downloadHandler.text;
        yield return new WaitForSeconds(0.1f);
        Debug.Log(html);
    }
    IEnumerator FetchDataInSession(string pname,int score)                  // SQL更新排行榜
    {
        string URL="http://140.136.151.71:5000/leaderboard?mode=updateleaderscore&pname=" + WWW.EscapeURL(pname) + "&score= " + score;
        Debug.Log(URL);
        var request = UnityWebRequest.Get(URL);
        
        yield return request.Send();
        
        if (request.isNetworkError)
        {
            Debug.Log(request.error);
            yield break;
        }
        
        var html = request.downloadHandler.text;
        yield return new WaitForSeconds(0.1f);
        Debug.Log(html);
    }

    private void Awake()
    {
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
    public ToolUI toolui;
    public FinishPlane finishPlane;
    private FirstCamera firstCamera;
    private BasicSpawner basicSpawner;  //引用

    void Start()
    {
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource.clip = bgmBackground;
        StartBackgroundMusic();

        collisionSoundSource = gameObject.AddComponent<AudioSource>();
        shootMusicSource = gameObject.AddComponent<AudioSource>();
        shootMusicSource.clip = shootbgmFPS;

        audioClips.Add("shoot", new List<AudioClip> { seShoot });
        audioClips.Add("collision", new List<AudioClip> { seShoot,seCollision });
        audioClips.Add("cactus", new List<AudioClip> { seShoot, seCollision,seCactus });

        networkCharacterController.SetJumpImpulse(normalJump);
    }

    public override void Spawned()
    {
        loadingOverlay=GameObject.FindGameObjectWithTag("LoadingOverlay");
        Destroy(loadingOverlay);
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
            else if(basicSpawner.levelIndex==3)
            {
                finishObject = GameObject.FindGameObjectWithTag("Finish3");
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
        //ChangeColor_RPC(ColorNum);
        if (Object.HasStateAuthority)
        {
            StartCoroutine(StartAsHost(PlayerPrefs.GetString("PlayerName"),PlayerPrefs.GetString("SessionName"),0));
            playerCount=0;
            bulletCount=maxBullet;
            Hp = maxHp;
        }
        else
        {
            StartCoroutine(SetDataInSession(PlayerPrefs.GetString("PlayerName"),PlayerPrefs.GetString("SessionName"),0));
        }


    }
    private float CalculateDistancePercentage()
    {
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
        if (finishObject != null)
        {
            //Vector3 closestPointOnBounds = finishCollider.boun  ds.ClosestPoint(transform.position);
            float currentDistance = Vector3.Distance(transform.position, finishObject.transform.position);
            // 在這裡訪問 finishObject 的屬性和方法
            //Debug.Log($"Absolute distance to the finish edge: {currentDistance}");
            return currentDistance;
        }
        else
        {
            // finishObject 是 null，可能已經被銷毀
            // 做相應的處理，例如返回一個特殊值或顯示錯誤訊息
            return -1f; // 返回一個特殊值表示錯誤情況
        }
    }

    public void StartBackgroundMusic()
    {
        RpcStartBackgroundMusic();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcStartBackgroundMusic()
    {
        backgroundMusicSource.Play();
        backgroundMusicSource.loop = true;
    }

    void StopBackgroundMusic()
    {
        RpcStopBackgroundMusic();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcStopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
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

            if (isMainCamera)
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
                    GetComponent<AudioSource>().clip = audioClips[selectedSound][0];
                    GetComponent<AudioSource>().Play(); 
                    GetComponent<AudioSource>().loop = false;
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
            Debug.Log("middleRespawn Respawn!");
            x = 0;
        }
        if (frozen == 1)
        {
            StartCoroutine(FreezePlayerForSeconds(5.0f));           
            frozen = 0;
        }       
        if (runfast == 1)
        {                    
            StartCoroutine(runPlayerForSeconds(8.0f));                     
            runfast = 0;
        }
        if (door == 1)
        {
            gateway();
            door = 0;
        }
        if (highhigh == 1)
        {
            StartCoroutine(jumpPlayerForSecondsCoroutine());
            highhigh = 0;
        }
        //if (HasStateAuthority)
        //{
            distance = CalculateDistancePercentage();
        //}
        transform.rotation = Quaternion.Euler(0, (float)_yaw,(float)_pitch);

        //var cameraEulerAngle = firstCamera.transform.rotation.eulerAngles;
        //firstCamera.transform.rotation = Quaternion.Euler((float)_pitch, cameraEulerAngle.y, cameraEulerAngle.z);
 
    }
    private bool followlevel()
    {
        if (gotonext == true)
            return true;
        else
            return false;
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
        else
            return false;
    }

    int x = 0;
    private bool MiddleRespawn()
    {
        if (x == 1)
        {
            return true;
        }
        else
            return false;
    }
    private IEnumerator runPlayerForSeconds(float seconds)
    {
        if (runfast == 1)
        {
            runfirePrefab.SetActive(true);
            var particleSystem = runfirePrefab.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            _speed = 28f;
            yield return new WaitForSeconds(seconds);
            _speed = 5f;
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
        }  
    }

    private IEnumerator FreezePlayerForSeconds(float seconds)
    {
        if (frozen == 1)
        {
            // Instantiate frozen effect
            GameObject frozenEffect = Instantiate(frozenPrefab, transform.position, transform.rotation);

            _speed = 0f;
            yield return new WaitForSeconds(seconds);
            _speed = 5f;

            // Stop the frozen effect
            var particleSystem = frozenEffect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
        }
    }

    private IEnumerator jumpPlayerForSecondsCoroutine()
    {
        Debug.Log("highhighYes!");
        yield return null;
        networkCharacterController.SetJumpImpulse(jumpHigh);
        networkCharacterController.Jump();
        Debug.Log("jump highhigh!");

        jumpPrefab.SetActive(true);
        var particleSystem = jumpPrefab.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        yield return new WaitForSeconds(12.0f);
        networkCharacterController.SetJumpImpulse(normalJump);
        networkCharacterController.Jump();
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 檢查是否已經抵達終點
        if (other.CompareTag("Finish3"))
        {
            if(HasInputAuthority){
                print("Touched finish");
                OnReachedFinish_3();
            }
            gotonext = true;
            Destroy(other.gameObject);
        }
        if ( other.CompareTag("Finish2") || other.CompareTag("Finish1") )
        {
            if(HasInputAuthority){
                print("Touched finish");
                OnReachedFinish();
            }
            gotonext = true;
            Vector3 p = other.transform.position;
            p.y = 50f;
            other.transform.position = p;         
        }

        if (other.gameObject.CompareTag("Frozen"))
        {
            if(HasInputAuthority)
            {
                Debug.Log("Trapdead frozen!");

                timeObject = GameObject.FindGameObjectWithTag("timerText");
                TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
                timerText.text = "冰凍中...";
                Invoke("C0", 5);
                frozen = 1;
            }
        }
        if (other.gameObject.CompareTag("cake"))
        {
            //Instantiate(runfirePrefab, transform.position, transform.rotation);
            if(HasInputAuthority)
            {
                timeObject = GameObject.FindGameObjectWithTag("timerText");
                TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
                timerText.text = "獲得加速道具 !";
                Invoke("C0", 5);
                Debug.Log("Get Cake!");
            }
            Destroy(other.gameObject);
            runfast = 1;
        }
        if (other.gameObject.CompareTag("gateway"))
        {
            if(HasInputAuthority)
            {
                timeObject = GameObject.FindGameObjectWithTag("timerText");
                TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
                timerText.text = "傳送門 Go!";
                Invoke("C0", 5);
            }
            Destroy(other.gameObject);
            door = 1;
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            //Instantiate(runfirePrefab, transform.position, transform.rotation);
            if(HasInputAuthority)
            {
                CoinPoint(this.PlayerName.ToString());
                timeObject = GameObject.FindGameObjectWithTag("timerText");
                TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
                timerText.text="獲得金幣 !";
                Invoke("C0", 5 );
                Debug.Log("Get Coin!");
                Debug.Log(basicSpawner.levelIndex);
                
            }
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Clip"))
        {
            if(HasInputAuthority)
            {
                Debug.Log("Get Clip!");
                timeObject = GameObject.FindGameObjectWithTag("timerText");
                TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
                timerText.text = "子彈 +5 !";
                bulletCount +=5;
                print("bulletCount:"+bulletCount);
            }
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Treasure")
        {
            if(HasInputAuthority)
            {
                timeObject = GameObject.FindGameObjectWithTag("timerText");
                TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
                timerText.text = "Jump High!";
                Invoke("C0", 5);
            }
            Destroy(other.gameObject);
            highhigh = 1;
        }
        if (other.gameObject.CompareTag("Soundtest"))
        {
            if(HasInputAuthority)
            {
            Debug.Log("Soundtest object collision!");
            timeObject = GameObject.FindGameObjectWithTag("timerText");
            TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
            timerText.text = "You died!";
            string selectedSound = "collision";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound][1];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = false;
            }

            // Respawn();
            a = 1;
            }
            
        }

        if (other.gameObject.CompareTag("Soundcactus"))
        {
            Debug.Log("Soundcactus object collision!");
            timeObject = GameObject.FindGameObjectWithTag("timerText");
            TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
            timerText.text = "You died!";
            string selectedSound = "cactus";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound][2];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = false;
            }

            // Respawn();
            a = 1;

            
        }

        if (other.gameObject.CompareTag("middleSpawn"))
        {
            Debug.Log("middleSpawn object collision!");
            timeObject = GameObject.FindGameObjectWithTag("timerText");
            TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
            timerText.text = "You died!";
            string selectedSound = "collision";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound][1];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = false;
            }

            // Respawn();
            x = 1;
        }

        if (other.gameObject.CompareTag("middleSpawn2"))
        {
            Debug.Log("middleSpawn2 object collision!");
            timeObject = GameObject.FindGameObjectWithTag("timerText");
            TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
            timerText.text = "You died!";
            string selectedSound = "cactus";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound][2];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔
                GetComponent<AudioSource>().loop = false;
            }

            // Respawn();
            x = 1;
        }
    }

    private void OnReachedFinish()                                //    第一二關的finish
    {
        FinishPlane finishPlane = FindObjectOfType<FinishPlane>();
        if (finishPlane != null)
        {
            if(playerCount == 0){
                GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in allPlayers)
                {
                    playerCount++;
                }
                SetPlayer_RPC(playerCount);
            }
            DistRutern();                                     
            SetPoint();
            Finish_RPC(this.PlayerName.ToString());
        }
        // 在這裡添加您想要在角色抵達終點時執行的程式碼
        timeObject = GameObject.FindGameObjectWithTag("Timer");
        timerUI timerScript = timeObject.GetComponent<timerUI>();
        timerScript.StopTimer();
    }
    private void OnReachedFinish_3()                             //     第三關的finish
    {
        FinishPlane finishPlane = FindObjectOfType<FinishPlane>();
        if (finishPlane != null)
        {
            if(playerCount == 0){
                GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in allPlayers)
                {
                    playerCount++;
                }
                SetPlayer_RPC(playerCount);
            }
            DistRutern();                                      
            SetPoint_3();
            Finish_RPC(this.PlayerName.ToString());
        }
        // 在這裡添加您想要在角色抵達終點時執行的程式碼
        timeObject = GameObject.FindGameObjectWithTag("Timer");
        timerUI timerScript = timeObject.GetComponent<timerUI>();
        timerScript.StopTimer();
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void gotoFPS()
    {
        print(basicSpawner.levelIndex);
        Hp=0;
        //networkCharacterController.transform.position = spawnPosition;
        /*networkCharacterController.transform.position = new Vector3(0, 61, -200);
        currentCameraMode = 1;
        basicSpawner.SideInput = false;*/
        //a = 1;
        //currentInputMode = InputMode.ModeFPS;
        //isFinish=0;
    }
    private void gateway()
    {
        if (basicSpawner.levelIndex == 1)
        {
            networkCharacterController.transform.position = new Vector3(215, 6, -5);
        }
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
            else if(basicSpawner.levelIndex == 1 && x == 1)
            {
                networkCharacterController.transform.position = new Vector3(145, 17, 0);
            }
            else if (basicSpawner.levelIndex == 2 && networkCharacterController.transform.position.x >= 105 && networkCharacterController.transform.position.y <= -5f)
            {
                networkCharacterController.transform.position = new Vector3(117, 17, 195);
            }
            else if (basicSpawner.levelIndex == 2 && x == 1)
            {
                networkCharacterController.transform.position = new Vector3(117, 17, 195);
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

        if (Hp > 0)
        {
            blood = 1;
            var particleSystem = bloodPrefab.GetComponent<ParticleSystem>();
            if (blood == 1)
            {
                bloodPrefab.SetActive(true);
                particleSystem.Play();
            }        
        }
        blood = 0;
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
        if(transform.position.x>165&&basicSpawner.levelIndex==1&&isTriggedRedCubeTips==0)//提示碰到紅方塊會死人
        {
            timeObject = GameObject.FindGameObjectWithTag("timerText");
            TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
            timerText.text="避開紅方塊而行 !";
            Invoke("C0", 5 );
            isTriggedRedCubeTips=1;
        }
        if (transform.position.x > 90 && basicSpawner.levelIndex == 1 && istrapTips == 0)//提示碰到紅方塊會死人
        {
            timeObject = GameObject.FindGameObjectWithTag("timerText");
            TextMeshProUGUI timerText = timeObject.GetComponent<TMPro.TextMeshProUGUI>();
            timerText.text = "下方有驚喜通道 !";
            Invoke("C0", 3);
            istrapTips = 1;
        }
        //FirstCamera.enabled = currentCameraMode == 2;

        if (basicSpawner.levelIndex==1||basicSpawner.levelIndex==2)//在確認為第一關或第二關要做的事情
        {
            
        }
        else if(basicSpawner.levelIndex==FPS_Level)//為了同個.unity檔案的FPS模式(目前level為3)做準備，預設會是正朝向模式
        {
            currentCameraMode = 1;
            isMainCamera=false;
            isSideCamera = true;
        }
        InputAction View = myActions.FindAction("View");
        View.started += ctx=>switchView();
        InputAction Start = myActions.FindAction("Start");
        Start.started += ctx=>StartButtonOnClick();
        InputAction Reset = myActions.FindAction("Reset");
        Reset.started += ctx=>ResetButtonOnClick();
        void ResetButtonOnClick()
        {
            SetId_RPC();
            gotoFPS();
            // 停止播放背景音樂
            //backgroundMusicSource.Stop();
            StopBackgroundMusic();
            Debug.Log("shootmusic!");
            // 播放特殊條件音樂
            shootMusicSource.Play();
            shootMusicSource.loop = true;
        }
        void StartButtonOnClick()
        {
            if (basicSpawner.levelIndex == 3)
            {
                wallObject = GameObject.FindGameObjectWithTag("StartWall3");
                StartWall startWallScript = wallObject.GetComponent<StartWall>();
                if (startWallScript != null)
                {
                    // 呼叫RPC來消除物件
                    startWallScript.RequestDespawnWall_RPC();
                    Debug.Log("wall3!");
                }
                StartGame();
            }
            else
            {
                StartGame();
            }
        }
        InputAction SwitchColor = myActions.FindAction("SwitchColor");
        SwitchColor.started += ctx=> SwitchColorButtonPressed();
        void SwitchColorButtonPressed()
        {
            ChangeColor_RPC(ColorNum);
            ColorNum++;
            if(ColorNum>=9)
            {
                ColorNum=0;
            }
        }
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
            if(basicSpawner.levelIndex==3)
            {
                wallObject = GameObject.FindGameObjectWithTag("StartWall1");
            }
            print(wallObject.transform.position.y);
            if(wallObject.transform.position.y<=20||basicSpawner.levelIndex==3)//當牆還在原位
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
            else
            {
                if(basicSpawner.levelIndex==3)
                {
                    StartM_RPC();  
                }
            }
        }
        //切換鏡頭模式
        if (Input.GetKeyDown(KeyCode.C))
            switchView();
        if (Input.GetKeyDown(KeyCode.K)){
            ResetButtonOnClick();
        }
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
            SwitchColorButtonPressed();
            //Reload_RPC();//測試:按R來Reload
        }
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.U))
        {
            print(distance);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartButtonOnClick();
        }
        
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            //CalculateDistancePercentage();
            timer = 0;
        }
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]                                               // 顏色更換_RPC
    private void ChangeColor_RPC(int ColorNumber)
    {
        colorsDict =new Dictionary<int, Color>() {
        {0, Color.red},
        {1, Color.blue},
        {2, Color.green},
        {3, Color.white},
        {4, Color.black},
        {5, Color.yellow},
        {6, Color.cyan} ,
        {7, Color.magenta},
        {8, Color.gray}
        };
        meshRenderer.material.color = colorsDict[ColorNumber%9];
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
        
        xCount = 0;
        yCount += 1;
        gotonext = true;
    }   
    [Rpc(RpcSources.All, RpcTargets.All)]                                                                     
    public void SetPlane_RPC()
    {
        FinishPlane finishPlane = FindObjectOfType<FinishPlane>();
        finishPlane.FinishClick();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]                                                                     
    public void SetPlane2_RPC()
    {
        FinishPlane finishPlane = FindObjectOfType<FinishPlane>();
        finishPlane.Finish3Click();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]                                                                      // 設置ID＿RPC
    public void SetId_RPC()
    {
        basicSpawner.levelIndex = 3;
    }

    private void SetPoint()     // 第一，二關分數設置
    {

        if (playerCount == 4 )
        {
            StartCoroutine(Set_4(sarr[0],sarr[1],sarr[2],sarr[3],PlayerPrefs.GetString("SessionName")));
        }
        else if (playerCount == 3)
        {
            StartCoroutine(Set_3(sarr[0],sarr[1],sarr[2],PlayerPrefs.GetString("SessionName")));
        }
        else if (playerCount == 2)
        {
            StartCoroutine(Set_2(sarr[0],sarr[1],PlayerPrefs.GetString("SessionName")));
        }
        else if (playerCount == 1)
        {
            StartCoroutine(Set_1(sarr[0],PlayerPrefs.GetString("SessionName")));
        }
    }
    private void SetPoint_3()     // 第三關分數設置
    {
        Debug.Log(playerCount);
        if (playerCount == 4 )
        {
            StartCoroutine(Set3_4(sarr[0],sarr[1],sarr[2],sarr[3],PlayerPrefs.GetString("SessionName")));
        }
        else if (playerCount == 3)
        {
            StartCoroutine(Set3_3(sarr[0],sarr[1],sarr[2],PlayerPrefs.GetString("SessionName")));
        }
        else if (playerCount == 2)
        {
            StartCoroutine(Set3_2(sarr[0],sarr[1],PlayerPrefs.GetString("SessionName")));
        }
        else if (playerCount == 1)
        {
            StartCoroutine(Set3_1(sarr[0],PlayerPrefs.GetString("SessionName")));
        }
    }
    private void CoinPoint(string a)           // 金幣分數設置
    {
         StartCoroutine(AddDataInSession(PlayerPrefs.GetString("SessionName"),a,1));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]                                                                      // 排名顯示FP＿RPC
    public void FinalPlaneDisplay_RPC(string [] names)
    {
        rankingObject = GameObject.FindGameObjectWithTag("RankingText");
        TextMeshProUGUI rankingText = rankingObject.GetComponent<TMPro.TextMeshProUGUI>();
        
        rankingText.text="";
        for (int i = 0; i < names.Length; ++i)
        {
            rankingText.text=rankingText.text+"\n"+(i+1)+" : "+names[i] ;
        }     
    }

    [Rpc(RpcSources.All, RpcTargets.All)]                                                                      // 分數顯示＿RPC
    public void FinalScoreDisplay_RPC(string [] names, int [] scores)
    {
        scoreObject = GameObject.FindGameObjectWithTag("scoreText");
        TextMeshProUGUI scoreText = scoreObject.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.text="";
        for (int i = 0; i < names.Length; i++)
        {
            if(i == 0 ){scoreText.text = "Score";}
            scoreText.text=scoreText.text+"\n"+names[i]+" : "+scores[i] ;
        }        
    }                                                                

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void StartM_RPC()
    {
        if(Object.HasStateAuthority)
        {
            finish3PositionIndex = UnityEngine.Random.Range(1, 3); //Host隨機選擇Index值
        }
        if(basicSpawner.levelIndex==3)
        {
            finishObject = GameObject.FindGameObjectWithTag("Finish3");
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
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetPlayer_RPC(int a)
    {
        playerCount = a ;
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
    //[Rpc(RpcSources.All, RpcTargets.All)]
    public void DistRutern()
    {
        Debug.Log(basicSpawner.levelIndex);
        xCount = 0;
        //if (HasInputAuthority)
        //{
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in allPlayers)
            {
                // 先檢查遊戲物件是否有你想要的 Component（比如你的 PlayerController 或其他）
                PlayerController playerController = player.GetComponent<PlayerController>();
                if(playerController != null)
                {
                    if(xCount == 0){
                        fir=playerController.distance;
                        firN=playerController.PlayerName.ToString();
                        Debug.Log(playerController.distance);
                        Debug.Log(playerController.PlayerName.ToString());
                        xCount++;
                    }
                    else if(xCount == 1 && playerController.PlayerName.ToString() != firN){
                        sec=playerController.distance;
                        secN=playerController.PlayerName.ToString();
                        Debug.Log(playerController.distance);
                        Debug.Log(playerController.PlayerName.ToString());
                        xCount++;
                        if(sec<=fir){
                            cha=sec;    //2->1
                            chaN=secN;
                            sec=fir;
                            secN=firN;
                            fir=cha;
                            firN=chaN;
                        }
                    }
                    else if(xCount == 2 && playerController.PlayerName.ToString() != secN && playerController.PlayerName.ToString() != firN){
                        thi=playerController.distance;
                        thiN=playerController.PlayerName.ToString();
                        Debug.Log(playerController.distance);
                        Debug.Log(playerController.PlayerName.ToString());
                        xCount++;
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
                    else if(xCount == 3 && playerController.PlayerName.ToString() != thiN && playerController.PlayerName.ToString() != secN && playerController.PlayerName.ToString() != firN){
                        fou=playerController.distance;
                        fouN=playerController.PlayerName.ToString();
                        Debug.Log(playerController.distance);
                        Debug.Log(playerController.PlayerName.ToString());
                        xCount++;
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
                        //ScoreLeaderboard.Set(0, firN);
                        sarr[0] = firN;
                        print(firN+" Is The First Place !! ");
                        if(sec != -1){
                            //ScoreLeaderboard.Set(1, secN);
                            sarr[1] = secN;
                            print(secN+" Is The Second Place !! ");
                            
                        }
                        if(thi != -1){
                            //ScoreLeaderboard.Set(2, thiN);
                            sarr[2] = thiN;
                            print(thiN+" Is The Third Place !! ");
                            
                        }
                        if(fou != -1){
                            //ScoreLeaderboard.Set(3, fouN);
                            sarr[3] = fouN;
                            print(fouN+" Is The Fourth Place !! ");
                           
                        }
                }
            }
        //}
    }
}
