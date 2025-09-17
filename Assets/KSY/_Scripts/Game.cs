using System;
using System.Xml;
using BackEnd;
using BackEnd.Tcp;
using Google.FlatBuffers;
using InputData.Map;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneType
{
    None = 0,
    Account,
    InGame
}
public class Game : SingletonBehaviour<Game>
{
    [SerializeField] private GameDataSO _gameData;
    [SerializeField] private string[] _mapNames;

    //씬이 다 로드된 후에 호출됨 
    public bool isEndedGame = false;

    public event Action LoadedAccountMenu;
    public event Action LoadedMainMenu;
    public event Action LoadedInGame;

    public bool IsAllReady { get; private set; }
    private bool _InGameLoaded;
    public Map MapCompo { get; private set; }

    static public byte P1LIFE = 3;
    static public byte P2LIFE = 3;
    private byte _otherHitCount;

    Player p1;
    Player p2;
    public byte OtherHitCount
    {
        get
        {
            return _otherHitCount;
        }
        set
        {
            _otherHitCount = (byte)Mathf.Clamp(value,0,10);
        }
    }

    public SceneType currnetScene = SceneType.Account;
    byte[] receiveBff = new byte[64];

    #region Unity Event Function
    private void Awake()
    {
        BackendReturnObject Initialize = Backend.Initialize();

        if (!Initialize.IsSuccess())
        {
            //초기화 실패 처리
        }
    }
    private void Start()
    {
        P1LIFE = (byte)UnityEngine.Random.Range(3, 7);
        P2LIFE = (byte)UnityEngine.Random.Range(3, 7);

        LoadedInGame += () => _InGameLoaded = true;

        //씬이 완료되었을 떄 호출되는 이벤트 등록.
        SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
        {
            //로딩을 완료한 씬의 이름.
            var name = s.name;

            //씬의 이름을 통해 어떤 씬의 로드를 완료했는지 판단함.
            switch (name)
            {
                case "AccountMenu":
                    {
                        LoadedAccountMenu?.Invoke();
                        break;
                    }
                //In Game Loaded
                default:
                    {
                        MapCompo = GameObject.Find("Map").GetComponent<Map>();
                        LoadedInGame?.Invoke();
                        break;
                    }
            }
        };

        //모든 유저가 준비되었을 때 호출되는 이벤트
        Backend.Match.OnMatchInGameStart = () =>
        {
            IsAllReady = true;
            EnterScene(SceneType.InGame);

            //인 게임 씬이 다 로드된 다음에 플레이어 초기화.
            if (!_InGameLoaded) LoadedInGame += InitPlayer;
            else InitPlayer();
        };

        Backend.Match.OnMatchRelay += ReceiveData;
    }

    private void ReceiveData(MatchRelayEventArgs args)
    {
        if (args.From.NickName != Server.MyName)
        {
            //수신 받은 데이터를 버퍼에 담기
            receiveBff = args.BinaryUserData;
            ByteBuffer _receiveBff = new ByteBuffer(receiveBff);

            //플랫폼 관련 데이터라면 넘겨주기;
            if (MapMessage.MapMessageBufferHasIdentifier(_receiveBff))
            {
                MapMessage message = MapMessage.GetRootAsMapMessage(_receiveBff);
                MapMessageType messageType = message.MapMessageTypeType;

                //찾은 플랫폼에 수신받은 데이터를 적용함.
                Server.Instance.ApplyData(message, null, messageType);
            }
        }
    }
    private void Update()
    {
        Backend.Match.Poll();
    }
    private void OnValidate()
    {
        _mapNames = _gameData.MapNames;
    }
    #endregion
    private void InitPlayer()
    {
        //********내 데이터 처리********

        //씬에서 플레이어 오브젝트 P1을 찾음
        GameObject.Find("P1").TryGetComponent(out p1);
        
        //서버로부터 불러왔던 나의 데이터를 가져옴
        UserData? myData = Server.Instance.GetMyData();

        //데이터가 제대로 불러와지지 않았다면 return;
        if (myData == null)
        {
            Debug.LogError("Error");
            return;
        }

        //플레이어 객체에 데이터 할당
        p1.GetUserData((UserData)myData);

        //나의 플레이어 객체 세팅
        p1.Init();

        //플레이어 위치 세팅
        MapCompo.SetPlayerStartPos(p1);

        //********상대방 데이터 처리********

        //씬에서 플레이어 오브젝트 P2를 찾음
        GameObject.Find("P2").TryGetComponent(out p2);

        //서버로부터 불러왔던 상대방 데이터를 가져옴
        UserData? otherData = Server.Instance.GetOtherData();

        //데이터가 제대로 불러와지지 않았다면 return;
        if (otherData == null)
        {
            Debug.LogError("Error");
            return;
        }

        //플레이어 객체에 데이터 할당
        p2.GetUserData((UserData)otherData);

        //상대 플레이어 객체 세팅
        p2.Init();

        //플레이어 위치 세팅
        MapCompo.SetPlayerStartPos(p2);
    }
    private void InitPlayer(Scene s, LoadSceneMode lsm)
    {
        Debug.Log("Start Init Player");
        //********내 데이터 처리********

        //씬에서 플레이어 오브젝트 P1을 찾음
        GameObject.Find("P1").TryGetComponent(out p1);

        //서버로부터 불러왔던 나의 데이터를 가져옴
        UserData? myData = Server.Instance.GetMyData();

        //데이터가 제대로 불러와지지 않았다면 return;
        if (myData == null)
        {
            Debug.LogError("Error");
            return;
        }

        //플레이어 객체에 데이터 할당
        p1.GetUserData((UserData)myData);

        //상대 플레이어 객체 세팅
        p1.Init();

        //플레이어 위치 세팅
        MapCompo.SetPlayerStartPos(p1);

        //********상대방 데이터 처리********
        Debug.Log("Start Init Other");

        //씬에서 플레이어 오브젝트 P2를 찾음
        GameObject.Find("P2").TryGetComponent(out p2);

        //서버로부터 불러왔던 상대방 데이터를 가져옴
        UserData? otherData = Server.Instance.GetOtherData();

        //데이터가 제대로 불러와지지 않았다면 return;
        if (otherData == null)
        {
            Debug.LogError("Error");
            return;
        }

        //플레이어 객체에 데이터 할당
        p2.GetUserData((UserData)otherData);

        //상대 플레이어 객체 세팅
        p2.Init();

        //플레이어 위치 세팅
        MapCompo.SetPlayerStartPos(p2);
    }
    public void EnterScene(SceneType t)
    {
        switch(t)
        {
            case SceneType.Account:
                {
                    currnetScene = SceneType.Account;
                    SceneManager.LoadScene("AccountMenu");
                    break;
                }
            case SceneType.InGame:
                {
                    currnetScene = SceneType.InGame;

                    if (Server.IsSuperGamer)
                    {
                        //랜덤한 맵을 선정함.
                        int mapIndex = UnityEngine.Random.Range(0, _mapNames.Length);

                        //선정한 맵의 이름을 가져옴
                        GameDataSend((byte)mapIndex);
                        SelectMap((byte)mapIndex);
                    }
                    break;
                }
        }   
    }
    public void SendPlayerHealth(string damagedPlayerName, sbyte playerHealth)
    {
        byte[] bff = Server.Instance.SerializationCurrentData(damagedPlayerName, playerHealth);
        Server.Instance.Send(bff);
        //플레이어 체력이 깎였을 때 호출 
    }
    public void ReceivePlayerHealth(string name, sbyte health)
    {
        if(name == "P1")
        {
            Debug.Log($"P1");
            p2.MyEntity.Health = (sbyte)health;
            MapCompo.P2Health.text = $"{Server.OtherName} health : {health}";
        }
        else if(name == "P2")
        {
            Debug.Log($"P1");
            p1.MyEntity.Health = (sbyte)health;
            MapCompo.P1Health.text = $"{Server.MyName} health : {health}";
        }
    }
    public void UpdateTime(byte Time)
    {
        Debug.Log(Time);
    }
    public void SelectMap(byte mapIndex)
    {
        string mapName = _mapNames[mapIndex];
        SceneManager.LoadScene(mapName);
    }
    private void GameDataSend(byte mapIndex)
    {
        byte[] bff = Server.Instance.SerializationStartEndData(mapIndex, P1LIFE, P2LIFE);
        Server.Instance.Send(bff);
    }
    public void EndDataReceive(byte hitCount)
    {
        string otherNick = Server.OtherName;
        string myNick = Server.MyName;
        GameObject gameOverUI = MapCompo.GameOverUI;

        if (hitCount > OtherHitCount)
        {
            gameOverUI.GetComponentInChildren<TMP_Text>().text = $"이긴 사람 : {otherNick}";
        }
        else if (hitCount < OtherHitCount)
        {
            gameOverUI.GetComponentInChildren<TMP_Text>().text = $"이긴 사람 : {myNick}";
        }
        else
        {
            gameOverUI.GetComponentInChildren<TMP_Text>().text = $"무승부";

        }

        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }
    public void EndDataSend(byte hitCount)
    {
        byte[] bff = Server.Instance.SerializationStartEndData(hitCount);
        Server.Instance.Send(bff);
    }
}
