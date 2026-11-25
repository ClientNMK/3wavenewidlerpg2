using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DataId
{
    Gold = 1,
    Exp,
    Jewel,
    Cooking_Exp,
    Milege,
    Lp,
    Equip,
    Skill,
    DragonHeart,
    Ability,
    Core,
    Skill_Fragment,
    BlackMarket_Coin,
    Cosutme_Coin,
    Guild_Coin,
    Guild_Ability,
    Guild_Exp,
    Daily_1,
    Daily_2,
    Daily_3,
    Daily_4,
    Daily_5,
    Elemental,
    ElementalAbility,
    Pot,
    Guild_Pot,
    Guild_Pot_Coin,
    Lab,
    StatueExp,
    Bait,

    Accelerate = 51,

    Cooking_Box = 98,
    Vip_Point = 99,

    Cash = 100,

    Ticket_101 = 101,
    Ticket_102,
    Ticket_103,
    Ticket_104,
    Ticket_105,
    Ticket_106,
    Ticket_107,
    Ticket_108,
    Ticket_109,
    // 레이드 소탕권(레이드 도전은 티켓 소모 없음)
    Ticket_112 = 112,

    SummonTicket_Weapon = 201,
    SummonTicket_Armor,
    SummonTicket_Helmet,
    SummonTicket_Skill,
    SummonTicket_Relic,
    SummonTicket_AncientRelic,
    SummonTicket_Ring,
    SummonTicket_AncientRelic_1,

    
    Exchange_Ticket = 299,

    Event_Drop = 301,
    Event_Coin = 302,
    Event_MiniGame_1 = 303,
    Event_MiniGame_2 = 304,

    //Event_Summer_Bingo = 305,
    //Event_Summer_Coin = 306,
    //Event_Summer_Drop = 307,
    //Event_Summer_Dungeon = 308,
    //Event_Barbecue_Drop = 309,
    //Event_Barbecue_Coin = 310,
    //Event_Barbecue_Bingo = 311,
    //Event_chuseok = 312,
    //Event_chuseok_Coin = 313,
    //Event_chuseok_Bingo = 314,
    //Event_chuseok_Mini = 315,
    //Event_Chuseok_Food_1 = 316,
    //Event_Chuseok_Food_2 = 317,
    //Event_Chuseok_Food_3 = 318,

    Event_Halloween = 319,
    Event_Halloween_Coin = 320,
    Event_Halloween_Bingo = 321,
    Event_Halloween_Marble = 322,
    Event_Halloween_Food_1 = 323,
    Event_Halloween_Food_2 = 324,
    Event_Halloween_Food_3 = 325,

    Marble = 340,

    Event_Winter_Fish_1 = 341,
    Event_Winter_Fish_2 = 342,
    Event_Winter_Fish_3 = 343,
    Event_Winter_Fish_4 = 344,
    Event_Winter_Fish_5 = 345,
    Event_Winter_Fish_6 = 346,
    Event_Winter_Fish_7 = 347,
    Event_Winter_Fish_8 = 348,
    Event_Winter_Fish_9 = 349,
    Event_Winter_Fish_10 = 350,
    Event_Winter_Fish_Bread = 351,     // 붕어빵 티켓 (일일 충전, 먼저 소진)
    Event_WinterFish_Bread_p = 352,     // 붕어빵 티켓 (보상·패스 획득)

    Event_WinterIce_Coin = 353,         // 겨울 상점 코인
    Event_WinterIce_Broken = 354,     // 얼음 티켓 (일일 충전, 먼저 소진)
    Event_WinterIce_Broken_p = 355,     // 얼음 티켓 (보상·패스 획득)

    Event_Max = 400,

    RandomBox_Start = 401,
    RandomBox_Max = 500,

    Relic_Start = 501,
    //AncientRelic = 351,
    Relic_Max = 600,
    

    Item_Weapon = 1001,
    Item_DefaultFishing = 1901,
    Item_Armor = 2001,
    Item_Helmet = 3001,
    Item_Ring = 4001,
    Item_Fishing = 4901,
    Item_EquipMax = 5000,

    Item_Box = 5001,
    Item_Box_Max = 6000,

    Title_Start = 6001,
    Title_Max = 8000,
    Item_Max = 9999,

    Item_Cooking_1 = 10000,
    Item_Cooking_2 = 10100,
    Item_Cooking_3 = 10200,
    Item_Cooking_4 = 10300,
    Item_Cooking_5 = 10400,
    Item_Cooking_6 = 10500,
    Item_Cooking_7 = 10600,
    Item_Cooking_8 = 10700,
    Item_Cooking_9 = 10800,
    Item_Cooking_10 = 10050,
    Item_Cooking_11 = 10150,
    Item_Cooking_12 = 10250,
    Item_Cooking_13 = 10350,
    Item_Cooking_14 = 10450,
    Item_Cooking_15 = 10550,
    Item_Cooking_16 = 10650,
    Item_Cooking_17 = 10750,
    Item_Cooking_18 = 10850,
    Item_Cooking_19 = 10950,

    Item_Food_Start = 11001,

    Item_Cooking_20 = 15000,
    Item_Cooking_21 = 15050,
    Item_Cooking_22 = 15100,
    Item_Cooking_23 = 15150,
    Item_Cooking_24 = 15200,
    Item_Cooking_25 = 15300,
    

    Item_Cooking_Max = 20000,

    Skill_Start = 20001,

    Skill_Passive = 25001,
    Skill_Max = 30000,

    Shop_Start = 30001,
    Item_Shop_AdRemove = 30801,
    Shop_Event_Start = 30901,
    Shop_Event_End = 31000,
    Shop_End = 50000,

    Fishing = 50001,
    Fishing_End = 60000
}

public enum ItemType
{
    Goods = 1,
    Equipment,
    Box,
    Costume,
    SkillJewel,
    Shop,
    Relic,
    Food,
    Title,
}
public enum CookingType
{
    Cooking = 1,
    FishCooking = 2,
}
public enum EquipItemType
{
    Weapon = 1,
    Armor,
    Helmet,
    Ring,
}
public enum FoodType
{
    Material,
    Food,
    MaterialBox,
    FishMaterial,
    FishFood,
}
public enum WorldType
{
    Stage = 1,
    DailyDungeon = 2,
    WorldBoss = 3,
    Challenge = 4,
    Guild = 5,
    Summer = 6,
    Raid = 7,
}
public enum DungeonType
{
    Chapter_1 = 1,

    Dungeon_Gold = 1001,
    Dungeon_Equip,
    Dungeon_Skill,
    Dungeon_Heart,
    Dungeon_Material,
    Dungeon_Core,
    Dungeon_Elemental,
    Dungeon_Fishing,
    Dungeon_Soul,


    Dungeon_Weekly_1 = 2001,
    Dungeon_Weekly_2 = 2002,
    Dungeon_Weekly_3 = 2003,
    Dungeon_Weekly_4 = 2004,
    Dungeon_Weekly_5 = 2005,

    Challenge_1 = 3001,
    Challenge_2 = 3002,
    Challenge_3 = 3003,
    Challenge_4 = 3004,

    Dungeon_Guild = 5001,
    Dungeon_Guild2 = 5002,

    Summer_Dps = 6001,
    Raid = 7001,
}

public enum OpenType
{
    Close = 0,
    Open = 1,
}

public enum ShopType
{
    Summon = 1,
    Pakage,
    Goods,
    Mileage
}

public enum ShopItemType
{
    Jewel = 1,                                  // 보석 
    Ads,                                        // 광고상점 
    Costume,                                    // 코스튬 
    Mileage,                                    // 마일리지 
    Shop_Daily,                                 // 일간 상점 
    Shop_Weekly,                                // 주간 상점 
    Shop_Monthly,                               // 월간 상점 
    Package_Limit,                              // 한정 패키지 
    Package_Event,                              // 이벤트 패키지 
    Package_StepUp,                             // 스텝업 패키지 

    Pass_DailyAccess,
    Pass_DailyDefeat,
    Pass_Level,
    Pass_Stage,

    Package_Surprise = 15,
    Event_Coin = 16,
    
    Package_BlackMarket = 17,

    Shop_Cash = 19,
    Shop_Jewel = 20,
    Shop_Skill = 21,
    Shop_Guild = 22,
    //Shop_EventSummer = 23,
    //Shop_EventSummer_Package = 24,

    //Shop_Event_BBQ_Coin = 25,
    //Shop_Event_BBQ_Jewel = 26,
    //Shop_Event_BBQ_Package = 27,
    Shop_Guild2 = 28,
    //Shop_Event_Chuseok_Coin = 29,
    //Shop_Event_Chuseok_Jewel = 30,
    //Shop_Event_Chuseok_Package = 31,
    //Shop_Event_Chuseok_WeeklyPackage = 32,

    // Shop_Event_Halloween_Coin = 33,
    // Shop_Event_Halloween_Jewel = 34,
    // Shop_Event_Halloween_Package = 35,
    // Shop_Event_Halloween_WeeklyPackage = 36,

    Shop_Event_Winter_Coin = 37,
    Shop_Event_Winter_Jewel = 38,
    Shop_Event_Winter_WeeklyPackage = 39,
    Shop_Event_Winter_Package = 40,
}

public enum PriceType
{
    Cash = 1,           // : Cash : 현금으로 구매할 수 있다
    Goods = 2,          // : Goods : 보유하고 있는 재화로 구매한다(마일리지, 보석 등)
    AD = 3,             // : AD - 광고 시청 시 구매된다
    Free = 4,           // : Free - 누구든지 구매할 수 있다 조건X
}
public enum PurchaseLimitType
{
    None = 0,               // : 구매제한 없음(무한 구매 가능)
    Account = 1,            // : 계정 구매제한 : 계정 구매의 경우 초기화되지 않는다
    Daily,                  // : 일일초기화
    Weekly,                 // : 주간초기화
    Monthly,                // : 월간초기화
}

public enum SummonType
{
    Weapon = 1, 
    Armor,
    Helmet,
    Skill,
    Ring,
    Relic = 11,
    AncientRelic = 12,
    AncientRelic_1 = 13,
}

public enum GradeType
{
    None = 0,
    D = 1,     
    C,       
    B,           
    A,         
    S,         
    SS,      
    SSS,
    US,
    USS,
    UR,
    URR,
    LR,
    LRR,
    MR,
    MRR,
    XR,
    XRR,
    ZR,
    ZRR,
    TR,
    TRR,
    GR,
    GRR,
    AR,
    ARR,
    SR,
    SRR,

    Max
}

public enum MissionType
{
    Daily = 1,
    Weekly,
    Repeat,
    
    Daily_ClearMission = 4,
    Weekly_ClearMission = 5,

    Event = 6, //BlackMarket

    Event_Christmas = 8,
    Event_Christmas_ClearMission = 9,

    //Summer_1 = 10,
    //Summer_2 = 11,
    //Summer_3 = 12,
    //Summer_4 = 13,
    //Summer_1 = 14,
    //Summer_2 = 15,
    //Summer_3 = 16,
    //Summer_4 = 17,
    //Summer_1 = 18,
    //Summer_2 = 19,
    Summer_1 = 20,
    Summer_2 = 21,

    Newbie = 22,

    Guild = 100,
    Guild_ClearMission = 101,
}

public enum CollectionType
{
    Weapon = 1,
    Armor,
    Helmet,
    Skill,
    Ring,
}

public enum ProgressType
{
    // ETC
    Etc = 0,

    // 스테이지 N도달
    Attendance = 1,

    // 골드 강화
    // 0 : 아무거나(카운팅), 1 : 공격력 레벨(누적), 2 : 체력 레벨(누적)
    PlayTime,

    // 몬스터 N마리 처치
    // 1 : 모든 몬스터, 2 : 일반 몬스터, 3 : 보스 몬스터
    KillMonster,

    // 소환 N번
    // 0 : 아무거나(머리, 검, 상의, 스킬), 1 : 머리, 2 : 검, 3 : 상의, 4 : 스킬, 5 : 유물
    Summon,

    // 레벨업 or 강화 N번
    // 0 : 아무거나(머리, 검, 상의, 스킬), 1 : 머리, 2 : 검, 3 : 상의, 4 : 스킬, 5 : 캐릭터, 6 : 스탯포인트, 7 : 유물, 8 : 문양, 9 : 룬
    Combine,

    // 던전 N회 플레이
    // 0 : 아무거나 / WorldId (1001, 1002...)
    Contents_Play,

    // 광고 시청
    WatchAd,
    //// 던전 N회 클리어
    //// 0 : 아무거나 / WorldId (1001, 1002...)
    //Contents_Clear,
    // 장착
    // 1 : 머리, 2 : 검, 3 : 상의, 4 : 스킬
    Enhance,

    Cooking,

    Eat,

    // 미션 클리어
    Daily_Mission_Clear = 11,
    Weekly_Mission_Clear = 12,
    Event_Mission_Clear = 13,
    EventGame_Mission_Clear = 14,

    UseJewel = 15,
    GoldEnhance = 16,
    EquipBox = 17,
    GuildAttendance = 18,
    GuildContentsPlay = 19,
    GuildPot = 20,
    GuildShop = 21,
    GuildAbility = 22,
    Guild_Mission_Clear = 23,

    Kill_EliteMonster = 24,
    Cooking_Chuseok = 25,
    Halloween_Mission_Clear = 26,
    Alphabet = 27,
    Delivery = 28,
    BlackMarket = 29,
    Fish = 31,
}
public enum GuideQuest_ProgressType
{
    // ETC
    Etc = 0,

    // 스테이지 N도달
    MaxStage = 1,

    // 골드 강화
    // 0 : 아무거나(카운팅), 1 : 공격력 레벨(누적), 2 : 체력 레벨(누적)
    GoldUpgrade = 2,

    // 소환 N번
    // 0 : 아무거나(머리, 검, 상의, 스킬), 1 : 머리, 2 : 검, 3 : 상의, 4 : 스킬, 5 : 유물
    Summon = 3,

    // 장착
    // 1 : 머리, 2 : 검, 3 : 상의, 4 : 스킬
    Equip = 4,

    // 레벨업 or 강화 N번
    
    LevelUp = 5,

    // 각성 N번
    
    Awakening = 6,

    
    // 몬스터 N마리 처치
    // 1 : 모든 몬스터, 2 : 일반 몬스터, 3 : 보스 몬스터
    KillMonster = 7,


    // 던전 N회 클리어
    // 0 : 아무거나 / WorldId (1001, 1002...)

    // 1 : clearcount / 2 : clearlevel
    Contents_Clear = 8,

    CheckMenu = 9,
    // 아이템 사용
    UseItem = 10,

    Cooking = 11,

    Purchase = 12,

    AdBuff = 13,

    CookingPromo = 14,


    // 1 : weapon / 2 : armor / 3 : helmet
    Merge = 15,



    //// 광고 시청
    //WatchAd,

    //// 던전 N회 플레이
    //// 0 : 아무거나 / WorldId (1001, 1002...)
    //Contents_Play,



    // 메뉴 확인
    // MenuId
    

    //// 게임플레이
    //// 1 : 접속 여부,	2 : 플레이시간
    //PlayTime,

    //// 합성
    //// 0 : 아무거나, 1 : 머리, 2 : 검, 3 : 상의, 4 : 스킬
    //Combine,

    //// 미션 클리어
    //// 1 : 일일 미션 2 : 주간 미션 3 : 반복 미션 (MissionType)
    //Mission_Clear,

    ////GuildAttendance
    //GuildAttendance,

}

public enum PassType
{
    Level_1 = 1,
    Level_2,
    Level_3,
    Level_4,
    Level_5,
    Level_6,
    Level_7,
    Level_8,
    Level_9,
    Level_10,

    Stage_1 = 101,
    Stage_2,
    Stage_3,
    Stage_4,
    Stage_5,
    Stage_6,
    Stage_7,
    Stage_8,
    Stage_9,
    Stage_10,

    StageMax = 1000,

    DailyAccess = 1001,
    DailyDefeat = 1002,

    EventAttendance = 2001,
    EventDefeat = 2002,
    Event = 2003,
    SummerEvent_End = 2004,
    SummerEvent_End2 = 2005,
    SummerEvent_End3 = 2006,
    SummerEvent = 2007,
    Newbie = 2008,
    WinterEvent = 2009,

    EventMax = 3000,

    Cooking_1 = 3001,
    Cooking_2,
    Cooking_3,
    Cooking_4,
    Cooking_5,
    Cooking_6,
    Cooking_7,
    Cooking_8,
    Cooking_9,
    Cooking_10,
    
    EventReturnPass = 4001,
    EventReturnPass2 = 4002,
}

public enum AttendanceType
{
    Repeat = 1,
    Once = 2,
}

public enum RankType
{
    Stage = 1,
    CP,
    Guild,
    Summer,
    GuildPot,
}

public enum RelicType
{
    Relic = 1,
    Ancient = 2,
    Ancient_1 = 3,
}

public enum QuestType
{
    GuideQuest = 1,
    Mission = 2,
}

public enum MenuId
{
    Character = 1,
    Equipment,
    Skill,
    Mission,
    Shop,
    Contents,
    Cooking,

    Character_GoldUpgrade = 11,
    Character_StatUpgrade = 12,
    Character_Relic = 13,
    Character_Ability = 14,
    Character_DragonHeart = 15,
    Character_DragonCore = 16,
    Character_DragonTurtle = 17,
    Character_Elemental = 18,
    Character_DragonSoul = 19,

    Equipment_Weapon = 21,
    Equipment_Armor = 22,
    Equipment_Helmet = 23,
    Equipment_Ring = 24,

    Skill_Active = 31,

    Mission_Mission = 41,
    Mission_Pass = 42,

    Shop_Summon = 51,
    Shop_Summon_Weapon = 52,
    Shop_Summon_Armor = 53,
    Shop_Summon_Helmet = 54,
    Shop_Summon_Skill = 55,
    Shop_Package = 56,
    Shop_Goods = 57,
    Shop_Shop = 58,
    Shop_Weekly = 59,
    Shop_Monthly = 60,
    Shop_Limit = 61,
    Shop_Costume = 62,
    Shop_Shop_Normal = 63,
    Shop_Goods_Jewel = 64,

    Content_DailyDungeon = 65,
    Content_Challenge = 66,
    Content_Weekly = 67,
    Content_Raid = 68,

    Cooking_Cooking = 71,
    Cooking_Inventory = 72,
    Cooking_Promo = 73,

    Stage = 101,
    Ranking,
    Post,
    Collection,
    SleepMode,
    Attendance,
    Event,
    Setting,
    Menu,
    Guild,
    Inventory,
    Costume,
    SummerEvent,
    Fief,

    Surprise = 201,
    BlackMarket,
    StepUp,
    ReturnPass,
    ReturnPass2,
    NewbiePass,

    Chat = 301,

    Info = 401,
    Title = 402,
    Nickname = 403,

    Delivery = 411,

    DungeonGold = 1001,
    DungeonEquip = 1002,
    DungeonSkill = 1003,
    DungeonHeart = 1004,
    DungeonMaterial = 1005,
    DungeonCore = 1006,
    DungeonElemental = 1007,
    DungeonFishing = 1008,
    DungeonSoul = 1009,

    DungeonChallenge1 = 3001,
    DungeonChallenge2 = 3002,
    DungeonChallenge3 = 3003,

    DungeonRaid = 7001,

    Shop_Summon_Ring = 9001,

    CookingSlot = 10001,

    Parm = 10101,
    StatResearch = 10102,
    Statue = 10103,
    Fishing = 10104,

    RaidInfo = 11001,
    RaidStatResearch = 11002,
}

public enum RuneType
{
    Rune_1 = 1,
    Rune_2,
    Rune_3,
    Rune_4,
}

public enum SurpriseType
{
    GuideQuest = 1,         // 가이드 퀘스트
    StageLevel,             // 스테이지
    SummonCount_Weapon,      // 해골 소환 레벨
    SummonCount_Armor,      // 검 소환 레벨
    SummonCount_Helmet,      // 갑옷 소환 레벨
    SummonCount_Skill,      // 스킬 소환 레벨
    SummonCount_Relic,      // 유물 소환 횟수
    PurchaseCount,          // 무조건 캐시로 사는것만 포함

    DungeonClearCount_Gold,     // 던전 클리어 횟수
    DungeonClearCount_Equip,     // 던전 클리어 횟수
    DungeonClearCount_Skill,     // 던전 클리어 횟수
    DungeonClearCount_Heart,     // 던전 클리어 횟수
    DungeonClearCount_Core,     // 던전 클리어 횟수
    DungeonClearCount_Material,     // 던전 클리어 횟수

    HeartLevel = 17,
    CoreLevel = 18,
    CookingLevel = 19,
    Relic = 20,
    GuildAbility = 21,
    AncientRelic = 22,
    Trans_Weapon = 23,
    Trans_Armor = 24,
    Trans_Helmet = 25,

    Inject_Level_1 = 26,
    Inject_Level_2 = 27,
    Inject_Level_3 = 28,
    Inject_Level_4 = 29,
    Inject_Level_5 = 30,
    DungeonClearCount_Weekly = 31,
    Inject_TotalLevel = 32,
    Trans_Skill = 33,
    Elemental = 34,
    SummonCount_Ring = 35,
    Pot = 36,
    AncientRelic_1 = 37,
    DungeonClearCount_Fishing = 38,
    FishingLevel = 39,
    DragonSoul_Level = 40,
}

public enum ServerType
{
    Kr = 1,
    Global = 2,

    Kr2 = 11,
    Kr3 = 12,
    
    Dev = 100,
}
public enum EventType
{
    None = 0,
    Attendance = 1,
}
public enum CookingGrade_ProgressType
{
    CharGrade = 1,          // 승급 0
    Cooking = 2,            // 요리하기 Num.1 11001~11005..  
    Dungeon_Clear = 3,      // 던전클리어
    Level = 4,              // 플레이어 레벨
    Relic = 5,              // 유물 레벨 달성
    FishingGrade = 6,              // 낚시 승급 0
}
public enum RandomBox_Type
{
    Random = 1,
    Choice = 2,
}
public enum StatInfoType
{
    Attack,
    Hp,
    Defence,
    Penetrate,
    Crit_1,
    Crit_2,
    Crit_1_Dmg,
    Crit_2_Dmg,
    NormalAttack,
    BossAttack,
    AttackSpeed,
    MoveSpeed,
    Gold,
    Exp,
    FoodDrop,
    ItemDrop,

    LastCooking_1,
    LastCooking_2,
    LastCooking_3,
    LastCooking_4,
    LastCooking_5,
    LastCooking_6,
    LastCooking_7,
    LastCooking_8,
    LastCooking_9,
    LastCooking_10,
    LastCooking_11,
    LastCooking_12,
    LastCooking_13,
    LastCooking_14,
    LastCooking_15,
    LastCooking_16,
    LastCooking_17,
    LastCooking_18,
    LastCooking_19,
    LastCooking_20,
    LastCooking_21,
    LastCooking_22,
    LastCooking_23,
    LastCooking_24,
    LastCooking_25,
    LastCooking_26,
    LastCooking_27,

    LastCooking,

    CooltimeReduce,
    SkillDmg,
    NormalAttackDmg,
    CookingCooltimeReduce,
    DoubleAttackPer,
    DoubleAttackDmgPer,
}
public enum TitleProgressType
{
    Cooking = 1,
    Level = 2,
    Stage = 3,
    Content = 4,  // 1.Clear 2.Play
    Attendance = 5,
    Equip = 6,    // 1.Get   2.Awaken
    Skill = 7,     // 1.Get   2.Awaken
    Rank = 8,      // 1.Stage  2.CP
    KillMonster = 9, // 1.Boss  2.Normal
    CookingPromo = 10,
}
public enum UtilStatType
{
    Gold,
    Exp,
    Item,
    Food,
}

public enum PresetType
{
    CharAbility,
    GuildAbility,
    Skill,
    //Elemental,

    Max
}