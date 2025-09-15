using System;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    None = 0,
    Account,
    MainMenu,
    InGame
}
public class Game : SingletonBehaviour<Game>
{
    [SerializeField] private GameDataSO _gameData;
    [SerializeField] private string[] _mapNames;

    //씬이 다 로드된 후에 호출됨 
    public event Action LoadedAccountMenu;
    public event Action LoadedMainMenu;
    public event Action LoadedInGame;

    public bool IsAllReady { get; private set; }
    private bool _InGameLoaded;
    public Map MapCompo { get; private set; }

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
        Backend.Match.OnMatchInGameStart = () => {
            IsAllReady = true;

            //인 게임 씬이 다 로드된 다음에 플레이어 초기화.
            if (!_InGameLoaded) LoadedInGame += InitPlayer;
            else InitPlayer();
        };
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
        Player p1;  GameObject.Find("P1").TryGetComponent(out p1);
        
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
        Player p2; GameObject.Find("P2").TryGetComponent(out p2);

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
        Player p1; GameObject.Find("P1").TryGetComponent(out p1);

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
        Player p2; GameObject.Find("P2").TryGetComponent(out p2);

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
                    SceneManager.LoadScene("AccountMenu");
                    break;
                }
            case SceneType.MainMenu:
                {
                    SceneManager.LoadScene("MainMenu");
                    break;
                }
            case SceneType.InGame:
                {
                    ////랜덤한 맵을 선정함.
                    //int mapIndex = UnityEngine.Random.Range(0, _mapNames.Length - 1);
                    ////선정한 맵의 이름을 가져옴
                    //string mapName = _mapNames[mapIndex];
                    ////가져온 이름의 씬(맵)을 로드함.
                    //SceneManager.LoadScene(mapName);

                    SceneManager.LoadScene("KSY_Map_1");
                    break;
                }
        }   
    }

    public void EndGame(string playerName)
    {
        Debug.Log($"<color=blue> End *게임 결과 처리 해야함* </color>");

        string myName = Server.Instance.GetMyData().Value.nickname;
        string otherName = Server.Instance.GetOtherData().Value.nickname;

        //만약 떨어져 죽은 사람이 나라면
        if (playerName == Server.Instance.GetMyData().Value.nickname)
        {
            //other winner 처리
            Debug.Log(otherName);
        }
        else
        {
            //my winner 처리
            Debug.Log(playerName);
        }
    }
}
