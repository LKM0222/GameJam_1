using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;


/*
    내가 본 게 맞다면
    일단 호출하는 함수 따로, 이벤트 따로일것.

    이벤트는 Pool 함수에 이벤트가 호출된다면 자동으로 호출되는것..!

    그러면 어쨌든 다른 스크립트에 있어도 이벤트가 호출되지 않을까?

    이벤트가 호출되는 스크립트는 EventManager에서 호출됨.
*/

public class MatchTestManager : MonoBehaviour
{
    
    private static MatchTestManager _instance = null;

    public List<MatchCard> matchCards = new List<MatchCard>();

    public MatchCard matchCard;

    public static MatchTestManager Instance { 
        get {
            if(_instance == null){
                // _instance = new MatchTestManager();
                _instance = FindObjectOfType(typeof(MatchTestManager)) as MatchTestManager;
            }
            return _instance;
        }
    }

    //여기까지가 방을 만들고, 방에 유저를 모으는 작업.
    //대기방 참여 코드
    public void CreateMatchingRoom(){ //EventHandler는 EventManager에 위치함.
        CreateMatchRoom();//대기방 생성
        GetMatchList(); //추후 매칭신청을 위해 카드 리스트 가져와야함.
        print("matchcards count is " + matchCards.Count);
        matchCard = matchCards[0];
        if(matchCard != null){
            print("MatchTestManager matchcard is : " + matchCard);
        }
        // SceneManager.LoadScene("MatchingRoom");
    }

    public void Join(){
        ErrorInfo errorInfo;
        if(Backend.Match.JoinMatchMakingServer(out errorInfo)){
            Debug.Log("Success " + errorInfo);
        }
    }
    
    void CreateMatchRoom(){ //envent handler는 eventManager에 있음.
        Backend.Match.CreateMatchRoom();
    }

    //인게임 리퀘스트 함수
    public void RequestMatchMaking(){
        GetMatchList();
        matchCard = matchCards[0]; //왜그런지 모르겠지만, 매치카드를 조회하려면 GetMatchList()를 먼저 호출해야됨...
        Backend.Match.RequestMatchMaking(matchCard.matchType, matchCard.matchModeType, matchCard.inDate);
    }

    



    void GetMatchList()
    {
        Debug.Log("start GetmatchList");
        var callback = Backend.Match.GetMatchList();

        if(!callback.IsSuccess())
        {
            Debug.LogError("Backend.Match.GetMatchList Error : " + callback);
            return;
        }

        List<MatchCard> matchCardList = new List<MatchCard>();

        LitJson.JsonData matchCardListJson = callback.FlattenRows();

        Debug.Log("Backend.Match.GetMatchList : " + callback);

        MatchCard matchCard = new MatchCard();

        for(int i = 0; i < matchCardListJson.Count; i++)
        {
            matchCard.inDate = matchCardListJson[i]["inDate"].ToString();
            matchCard.result_processing_type = matchCardListJson[i]["result_processing_type"].ToString();
            matchCard.version = int.Parse(matchCardListJson[i]["version"].ToString());
            matchCard.matchTitle = matchCardListJson[i]["matchTitle"].ToString();
            matchCard.enable_sandbox = matchCardListJson[i]["enable_sandbox"].ToString() == "true" ? true : false;
            string matchType = matchCardListJson[i]["matchType"].ToString();
            string matchModeType = matchCardListJson[i]["matchModeType"].ToString();

            switch(matchType){
                case "random":
                    matchCard.matchType = BackEnd.Tcp.MatchType.Random;
                    break;

                case "point":
                    matchCard.matchType = BackEnd.Tcp.MatchType.Point;
                    break;

                case "mmr":
                    matchCard.matchType = BackEnd.Tcp.MatchType.MMR;
                    break;
            }

            switch(matchModeType){
                case "Melee":
                    matchCard.matchModeType = BackEnd.Tcp.MatchModeType.Melee;
                    break;
                
                case "TeamOnTeam":
                    matchCard.matchModeType = BackEnd.Tcp.MatchModeType.TeamOnTeam;
                    break;
                
                case "OneOnOne":
                    matchCard.matchModeType = BackEnd.Tcp.MatchModeType.OneOnOne;
                    break;
            }


            matchCard.matchHeadCount = int.Parse(matchCardListJson[i]["matchHeadCount"].ToString());
            matchCard.enable_battle_royale = matchCardListJson[i]["enable_battle_royale"].ToString() == "true" ? true : false;
            matchCard.match_timeout_m = int.Parse(matchCardListJson[i]["match_timeout_m"].ToString());
            matchCard.transit_to_sandbox_timeout_ms = int.Parse(matchCardListJson[i]["transit_to_sandbox_timeout_ms"].ToString());
            matchCard.match_start_waiting_time_s = int.Parse(matchCardListJson[i]["match_start_waiting_time_s"].ToString());

            if(matchCardListJson[i].ContainsKey("match_increment_time_s"))
            {
                matchCard.match_increment_time_s = int.Parse(matchCardListJson[i]["match_increment_time_s"].ToString());
            }
            if(matchCardListJson[i].ContainsKey("maxMatchRange"))
            {
                matchCard.maxMatchRange = int.Parse(matchCardListJson[i]["maxMatchRange"].ToString());
            }
            if(matchCardListJson[i].ContainsKey("increaseAndDecrease"))
            {
                matchCard.increaseAndDecrease = int.Parse(matchCardListJson[i]["increaseAndDecrease"].ToString());
            }
            if(matchCardListJson[i].ContainsKey("initializeCycle"))
            {
                matchCard.initializeCycle = matchCardListJson[i]["initializeCycle"].ToString();
            }
            if(matchCardListJson[i].ContainsKey("defaultPoint"))
            {
                matchCard.defaultPoint = int.Parse(matchCardListJson[i]["defaultPoint"].ToString());
            }

            if(matchCardListJson[i].ContainsKey("savingPoint"))
            {
                if(matchCardListJson[i]["savingPoint"].IsArray)
                {
                    for(int listNum = 0; listNum < matchCardListJson[i]["savingPoint"].Count; listNum++)
                    {
                        var keyList = matchCardListJson[i]["savingPoint"][listNum].Keys;
                        foreach(var key in keyList)
                        {
                            matchCard.savingPoint.Add(key, int.Parse(matchCardListJson[i]["savingPoint"][listNum][key].ToString()));
                        }
                    }
                }
                else
                {
                    foreach(var key in matchCardListJson[i]["savingPoint"].Keys)
                    {
                        matchCard.savingPoint.Add(key, int.Parse(matchCardListJson[i]["savingPoint"][key].ToString()));
                    }
                }
            }
            matchCardList.Add(matchCard);
            // this.matchCards = matchCardList;
        }

        foreach(var matchcard in matchCardList)
        {
            Debug.Log(matchcard.ToString());
        }
        if(matchCardList.Count > 0){
            matchCards = matchCardList;
            Debug.Log("list idx 0 is " + matchCardList[0]);
        }
        
    }

}

public class MatchCard
{
    public string inDate;
    public string matchTitle;
    public bool enable_sandbox;
    public BackEnd.Tcp.MatchType matchType;
    public MatchModeType matchModeType;
    public int matchHeadCount;
    public bool enable_battle_royale;
    public int match_timeout_m;
    public int transit_to_sandbox_timeout_ms;
    public int match_start_waiting_time_s;
    public int match_increment_time_s;
    public int maxMatchRange;
    public int increaseAndDecrease;
    public string initializeCycle;
    public int defaultPoint;
    public int version;
    public string result_processing_type;
    public Dictionary<string, int> savingPoint = new Dictionary<string, int>(); // 팀전/개인전에 따라 키값이 달라질 수 있음.  
    public override string ToString()
    {
        string savingPointString = "savingPont : \n";
        foreach(var dic in savingPoint)
        {
            savingPointString += $"{dic.Key} : {dic.Value}\n";
        }
        savingPointString += "\n";
        return $"inDate : {inDate}\n" +
        $"matchTitle : {matchTitle}\n" +
        $"enable_sandbox : {enable_sandbox}\n" +
        $"matchType : {matchType}\n" +
        $"matchModeType : {matchModeType}\n" +
        $"matchHeadCount : {matchHeadCount}\n" +
        $"enable_battle_royale : {enable_battle_royale}\n" +
        $"match_timeout_m : {match_timeout_m}\n" +
        $"transit_to_sandbox_timeout_ms : {transit_to_sandbox_timeout_ms}\n" +
        $"match_start_waiting_time_s : {match_start_waiting_time_s}\n" +
        $"match_increment_time_s : {match_increment_time_s}\n" +
        $"maxMatchRange : {maxMatchRange}\n" +
        $"increaseAndDecrease : {increaseAndDecrease}\n" +
        $"initializeCycle : {initializeCycle}\n" +
        $"defaultPoint : {defaultPoint}\n" +
        $"version : {version}\n" +
        $"result_processing_type : {result_processing_type}\n" +
        savingPointString;
    }
}




