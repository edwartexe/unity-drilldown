using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class globals {
    //units
    public const int action_cost_base_drill = 3;//5
    public const int action_cost_base_tank = 2;//3
    public const int action_cost_base_scout = 1;//1
    public const int action_cost_base_bomb = 2;//5
    public const int action_cost_base_armoredscout = 2;

    public const string drillname = "Drill";
    public const string tankname = "Tank";
    public const string scoutname = "Scout";
    public const string bombname = "Demolition";
    public const string armoredScoutname = "Amored Scout";

    public const int drillhealth = 30;
    public const int tankhealth  = 50;
    public const int scouthealth = 25;
    public const int bombhealth = 25;
    public const int armoredScouthealth= 50;

    public const int drill_create_cost = 15;
    public const int tank_create_cost = 25;
    public const int scout_create_cost = 20;
    public const int bomb_create_cost = 25;
    public const int armoredScout_create_cost = 30;

    public const  int action_cost_tile_move = 0;//1
    public const  int action_cost_drill = 0;//1
    //public const  int action_cost_drillattack = 1;
    public const  int action_cost_attack = 0;//1
    public const  int action_cost_hold = 0;
    public const  int action_cost_scan = 0;//5


    public const int drillattackstat = 15;
    public const int tankattackstat = 20;
    public const int bombExplodeStat = 50;
    public const int armoredScout_attackstat = 15;
    public const int scout_scan_range = 2;

    public const int gasCoinCost = 2;
    public const int coinRedAlert = 30;

    //tiles
    public const  int node_reveal_gas = 3;
    public const  int node_reveal_coin = 1;
    public const  int node_blocked_coin = 6;
    public const  int relicGasloot = 10;
    public const  int relicCoinloot = 15;
    public const  int relicSaveGasloot = 25;
    public const  int relicSaveCoinloot = 40;

    //-----------------enemies
    public const string wormname = "Elder Thing";
    public const string sharkname = "Sand Shark";
    public const string thiefname = "Gremlin";
    public const string thief2name = "Super Gremlin";
    public const string bossname = "Sphinx";
    public const string nestName = "Gremlin Nest";
    public const string unlockerName = "Lock";

    public const int wormhealth = 20;
    public const int sharkhealth = 50;
    public const int thiefhealth = 10;
    public const int thief2health = 30;
    public const int bosshealth = 100;
    public const int nesthealth = 40;
    public const int unlockerhealth = 10;

    public const int wormPower = 10;
    public const int sharkPower = 20;
    public const int thiefPower = 10;
    public const int thief2Power = 20;
    public const int bossPower = 30;
    public const int nestPower = 0;
    public const int unlockerpower = 0;

    public const int wormGasloot = 20;
    public const int sharkGasloot = 30;
    public const int thiefGasloot = 15;
    public const int thief2Gasloot = 25;
    public const int bossGasloot = 35;
    public const int nestGasLoot = 10;

    public const int wormCoinloot = 15;//5
    public const int sharkCoinloot = 25;//10
    public const int thiefCoinloot = 8;//1
    public const int thief2Coinloot = 15;//2
    public const int bossCoinloot = 60;//20
    public const int nestCoinLoot = 5;//1
    public const int unlockerCoinLoot = 5;

    //functions
    public static int Mod(int x, int m) {
        int r = x % m;
        return r < 0 ? r + m : r;
    }


    //Saved tutorials
    static tutorial_item tutorial_basic1 = new tutorial_item(Resources.Load<Sprite>("keys"), "Select a unit/option with Z, cancel with X and move with the arrow keys");
    static tutorial_item tutorial_basic2 = new tutorial_item(Resources.Load<Sprite>("ss_move"), "Select a unit so see its movement range, then select a square to move it there");
    static tutorial_item tutorial_basic3 = new tutorial_item(Resources.Load<Sprite>("ss_drill"), "The Drill can dig holes in the labyrinth, but its also expensive. It will be your most important unit.");
    static tutorial_item tutorial_basic4 = new tutorial_item(Resources.Load<Sprite>("ss_relic"), "To win you must find and take all the relics in the labyrinth back to the base");
    static tutorial_item tutorial_scout3 = new tutorial_item(Resources.Load<Sprite>("ss_scouting3"), "The scout can find and move the relics");
    static tutorial_item tutorial_basic5 = new tutorial_item(Resources.Load<Sprite>("ss_base"), "You can always buy more units at the HQ");
    static tutorial_item tutorial_basic6 = new tutorial_item(Resources.Load<Sprite>("unit_colored"), "The expedition will continue as long as you have funds. Unit upkeep will be expensive, every turn counts.");
    
    static tutorial_item tutorial_battle1 = new tutorial_item(Resources.Load<Sprite>("ss_tank"), "The labyrinth is full of monsters, destroy them to protect your units and gain money.");
    static tutorial_item tutorial_battle2 = new tutorial_item(Resources.Load<Sprite>("ss_enemy"), "Selecting an enemy will show you it's movement and attack range");
    
    static tutorial_item tutorial_scout1 = new tutorial_item(Resources.Load<Sprite>("ss_scouting"), "The scout can scan a wide area around itself");
    static tutorial_item tutorial_scout2 = new tutorial_item(Resources.Load<Sprite>("ss_scouting2"), "It will show a total of all important things in the highlihted zone");
    
    static tutorial_item tutorial_bomb = new tutorial_item(Resources.Load<Sprite>("ss_bomb"), "Bombs can clear away a lot of walls in one go. This will also take out most enemies");
    
    static tutorial_item tutorial_battle3 = new tutorial_item(Resources.Load<Sprite>("ss_thief"), "The gremlins will try to steal your money and run back to their base.");
    static tutorial_item tutorial_battle4 = new tutorial_item(Resources.Load<Sprite>("ss_cactus"), "Some zones are locked out by a certain enemy. It's always close to the zones it locks");
    static tutorial_item tutorial_battle5 = new tutorial_item(Resources.Load<Sprite>("ss_sphinx"), "Expeditions where this statue has been found have all vanished. Tread Carefully");
    
    public static List<tutorial_item> allTutorials = new List<tutorial_item> {
        tutorial_basic1,  tutorial_basic2, tutorial_basic3, tutorial_basic4, tutorial_basic5, tutorial_basic6,
        tutorial_battle1, tutorial_battle2,
        tutorial_scout1, tutorial_scout2, tutorial_scout3, tutorial_bomb,
        tutorial_battle3, tutorial_battle4, tutorial_battle5
    };
    public static List<tutorial_item> basicTutorials = new List<tutorial_item> { tutorial_basic1,  tutorial_basic2, tutorial_basic3, tutorial_basic4, tutorial_scout3, tutorial_basic5, tutorial_basic6}; 
    public static List<tutorial_item> tankTutorials = new List<tutorial_item> { tutorial_battle1, tutorial_battle2}; 
    public static List<tutorial_item> scoutTutorials = new List<tutorial_item> {tutorial_scout1, tutorial_scout2 };
    public static List<tutorial_item> bombTutorials = new List<tutorial_item> { tutorial_bomb };
    public static List<tutorial_item> moreEnemiesTutorial = new List<tutorial_item> { tutorial_battle3, tutorial_battle4, tutorial_battle5 };


    //stages
    static TextAsset stage_basic = Resources.Load<TextAsset>("mapfiles/basic");
    static TextAsset stage_bombLevel = Resources.Load<TextAsset>("mapfiles/bombLevel");
    static TextAsset stage_circular = Resources.Load<TextAsset>("mapfiles/circular");
    static TextAsset stage_centralmap = Resources.Load<TextAsset>("mapfiles/centralMap");
    static TextAsset stage_lockAndKey = Resources.Load<TextAsset>("mapfiles/lockAndKey");
    static TextAsset stage_topStart = Resources.Load<TextAsset>("mapfiles/topStart");
    static TextAsset stage_topStartHoles = Resources.Load<TextAsset>("mapfiles/topStartHoles");
    static TextAsset stage_Tutorial = Resources.Load<TextAsset>("mapfiles/Tutorial");
    public static List<TextAsset> aviableMaps = new List<TextAsset> {
        stage_basic,
        stage_bombLevel,
        stage_circular,
        stage_centralmap,
        stage_lockAndKey,
        stage_topStart,
        stage_topStartHoles,
        stage_Tutorial
    };
    public static List<TextAsset> mapFiles = new List<TextAsset> {
        stage_basic,
        stage_bombLevel,
        stage_circular,
        stage_centralmap,
        stage_lockAndKey,
        stage_topStartHoles
    };
}
