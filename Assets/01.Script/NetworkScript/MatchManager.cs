using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public class MatchManager
{
    public List<MatchCard> matchCards = new List<MatchCard>();
    int index = 0;
    public bool listIsEmpty = false;
    
    private static MatchManager _instance = null;

    public static MatchManager Instance {
        get {
            if(_instance == null){
                _instance = new MatchManager();
                return _instance;
            }
            else{
                return _instance;
            }
            
        }
    }
    private void Update() {
        
    }

    public void JoinMatchMakingServer(){
        Join();
        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) =>  {
            //createroom
            if(args.ErrInfo == ErrorInfo.Success){
                CreateMatchRoom();
                GetMatchList();
            }
        };
        
    }

    void Join(){
        ErrorInfo errorInfo;
        if(Backend.Match.JoinMatchMakingServer(out errorInfo)){
            Debug.Log("Join Success : " + errorInfo.ToString());
        }else{
            Debug.Log("Join error : " + errorInfo.ToString());
        }
    }

    void CreateMatchRoom(){
        Backend.Match.CreateMatchRoom();
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) => {
            if(args.ErrInfo == ErrorCode.Success){
                Backend.Match.RequestMatchMaking(matchCards[index].matchType, matchCards[index].matchModeType, matchCards[index].inDate);
                Debug.Log("CreateRoom Success : " + args.ToString());
            } else {
                Debug.Log("CreateRoom error : " + args.ToString());
            }
        };
        Debug.Log("CreateMatchingRoom");
        Backend.Match.CreateMatchRoom();
    }

    void MatchMakingRoomUserList(){
        Backend.Match.OnMatchMakingRoomUserList = (MatchMakingGamerInfoListInRoomEventArgs args) => {
            if(args.ErrInfo == ErrorCode.Success){
                Debug.Log("UserList " + args.UserInfos);
            } else{
                Debug.Log("userlist error");
            }
        };
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
            matchCards = matchCardList;
        }

        foreach(var matchcard in matchCardList)
        {
            Debug.Log(matchcard.ToString());
        }
        if(matchCardList.Count > 0){
            Debug.Log("list idx 0 is " + matchCardList[0]);
            // MatchingRoomScript.Instance.roomMatchCard = matchCardList[0];
            listIsEmpty = true;
        }
        
    }


}
// public class MatchCard
// {
//     public string inDate;
//     public string matchTitle;
//     public bool enable_sandbox;
//     public BackEnd.Tcp.MatchType matchType;
//     public MatchModeType matchModeType;
//     public int matchHeadCount;
//     public bool enable_battle_royale;
//     public int match_timeout_m;
//     public int transit_to_sandbox_timeout_ms;
//     public int match_start_waiting_time_s;
//     public int match_increment_time_s;
//     public int maxMatchRange;
//     public int increaseAndDecrease;
//     public string initializeCycle;
//     public int defaultPoint;
//     public int version;
//     public string result_processing_type;
//     public Dictionary<string, int> savingPoint = new Dictionary<string, int>(); // 팀전/개인전에 따라 키값이 달라질 수 있음.  
//     public override string ToString()
//     {
//         string savingPointString = "savingPont : \n";
//         foreach(var dic in savingPoint)
//         {
//             savingPointString += $"{dic.Key} : {dic.Value}\n";
//         }
//         savingPointString += "\n";
//         return $"inDate : {inDate}\n" +
//         $"matchTitle : {matchTitle}\n" +
//         $"enable_sandbox : {enable_sandbox}\n" +
//         $"matchType : {matchType}\n" +
//         $"matchModeType : {matchModeType}\n" +
//         $"matchHeadCount : {matchHeadCount}\n" +
//         $"enable_battle_royale : {enable_battle_royale}\n" +
//         $"match_timeout_m : {match_timeout_m}\n" +
//         $"transit_to_sandbox_timeout_ms : {transit_to_sandbox_timeout_ms}\n" +
//         $"match_start_waiting_time_s : {match_start_waiting_time_s}\n" +
//         $"match_increment_time_s : {match_increment_time_s}\n" +
//         $"maxMatchRange : {maxMatchRange}\n" +
//         $"increaseAndDecrease : {increaseAndDecrease}\n" +
//         $"initializeCycle : {initializeCycle}\n" +
//         $"defaultPoint : {defaultPoint}\n" +
//         $"version : {version}\n" +
//         $"result_processing_type : {result_processing_type}\n" +
//         savingPointString;
//     }
// }



