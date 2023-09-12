

using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using UnityEngine;


namespace StickIttotheStickman
{

    class Main : MonoBehaviour
    {
    

      
        bool esp = true;
        bool Enemyesp = true;
     
      
        public static List<Player> EnemyBase = new List<Player>();
        public static List<EnemyAI> EnemyAIBase = new List<EnemyAI>();
        public static List<LevelManager> LevelManager = new List<LevelManager>();
        public static List<UnlockManager> UnlockManager = new List<UnlockManager>();



        float natNextUpdateTime;
        private static Material chamsMaterial;

        private Color blackCol;
        private Color entityBoxCol;
        public static Camera cam;
   
    
        private bool showMenu = true; // Whether to show the menu or not

     
    

        private Rect menuRect = new Rect(10, 10, 460, 300); // Initial position and size of the menu
        private int selectedTab = 0;
        private string[] tabNames = { "Main", "Esp"};


        //menu part
        private void DrawMenu(int windowID)
        {
            // Create toggle buttons for each tab
            GUILayout.BeginHorizontal();
            for (int i = 0; i < tabNames.Length; i++)
            {
                if (GUILayout.Toggle(selectedTab == i, tabNames[i], "Button", GUILayout.ExpandWidth(true)))
                {
                    selectedTab = i; // Set the selected tab index
                }
            }
            GUILayout.EndHorizontal();

            // Display content for the selected tab
            switch (selectedTab)
            {
                case 0:



                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    if (GUILayout.Button("Victory"))
                    {
                        foreach (LevelManager player in LevelManager)
                        {
                            player.EndLevelInVictory();
                        }
                    }
                    if (GUILayout.Button("Kill all"))
                    {
                        foreach (EnemyAI player in EnemyAIBase)
                        {
                            if (player.Unit.Faction == FactionType.Enemy)
                            {
                                player.Unit.Health.TakeDamage(new Damage(player.Unit, 9999, DamageType.Electric, null));
                            }
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    GUILayout.BeginVertical();
                    if (GUILayout.Button("Defeat"))
                    {
                        foreach (LevelManager player in LevelManager)
                        {
                            player.EndLevelInDefeat();
                        }
                      }
                    if (GUILayout.Button("Unlockall"))
                    {
                        foreach (UnlockManager player in UnlockManager)
                        {
                            player.UnlockTrophy();
                            UnlockAllStarterArchetypes();
                            UnlockEverything();


                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                  

                    break;

                case 1:
                    // Content for tab 2

                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    esp = GUILayout.Toggle(esp, "Player");
                  
                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    GUILayout.BeginVertical();
                    Enemyesp = GUILayout.Toggle(Enemyesp, "Enemy");
                    //  Weapontest = GUILayout.Toggle(Weapontest, "text");
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    break;
            }

         

            GUI.DragWindow(); // Allow the user to drag the window around
        }


        public static void UnlockAllStarterArchetypes()
        {
            foreach (LevelInfo levelInfo in LevelInfo.AllLevelInfos)
            {
                foreach (OfficeArchitype archetypeUnlocked in from a in levelInfo.archetypes
                                                              where a.unlockedAtStart
                                                              select a)
                {
                    GameSaveDataController.GetLevelData(levelInfo).SetArchetypeUnlocked(archetypeUnlocked);
                }
            }
            SingletonBehaviour<GameSaveDataController>.Instance.PerformSave();
        }

        // Token: 0x060017DE RID: 6110 RVA: 0x00094224 File Offset: 0x00092424
        public static void UnlockEverything()
        {
            foreach (LevelInfo levelInfo in from l in LevelInfo.AllLevelInfos
                                            where l.hasSaveData
                                            select l)
            {
                LevelSaveData levelData = GameSaveDataController.GetLevelData(levelInfo);
                if (levelData != null)
                {
                    levelData.SetLevelHasBeenVisited();
                }
                LevelSaveData levelData2 = GameSaveDataController.GetLevelData(levelInfo);
                if (levelData2 != null)
                {
                    levelData2.SetHaveHadLevelNightmare();
                }
                foreach (OfficeArchitype archetypeUnlocked in levelInfo.archetypes)
                {
                    LevelSaveData levelData3 = GameSaveDataController.GetLevelData(levelInfo);
                    if (levelData3 != null)
                    {
                        levelData3.SetArchetypeUnlocked(archetypeUnlocked);
                    }
                }
                foreach (ExpansionData expansionPurchased in levelInfo.expansions)
                {
                    LevelSaveData levelData4 = GameSaveDataController.GetLevelData(levelInfo);
                    if (levelData4 != null)
                    {
                        levelData4.SetExpansionPurchased(expansionPurchased);
                    }
                }
            }
            SingletonBehaviour<GameSaveDataController>.Instance.PerformSave();
            Debug.Log("[UnlockManager] - UnlockEverything");
        }




        private float DistanceFromCamera(Vector3 worldPos)
        {
            return Vector3.Distance(cam.transform.position, worldPos);
        }

       

        public void OnGUI()
        {

            if (showMenu) // Only draw the menu when showMenu is true
            {
                // Set the background color
                // Set the background color
                GUI.backgroundColor = Color.black;

                menuRect = GUI.Window(0, menuRect, DrawMenu, "WoodgamerHD Menu"); // name of the menu
            }

            // RuntimeConsole.SetActive(!showconsole);
            if (esp)
            {


                foreach (Player player in EnemyBase)
                {






                    Vector3 w2s = cam.WorldToScreenPoint(player.transform.position);


                        if (ESPUtils.IsOnScreen(w2s) && Vector3.Distance(Camera.main.transform.position, player.transform.position) < 400f)
                        {
                            DrawEnemyInfo(player);


                        }
                    
                }
            }
            if (Enemyesp)
            {


                foreach (EnemyAI player in EnemyAIBase)
                {






                    Vector3 w2s = cam.WorldToScreenPoint(player.transform.position);


                    if (ESPUtils.IsOnScreen(w2s) && Vector3.Distance(Camera.main.transform.position, player.transform.position) < 400f)
                    {
                        DrawEnemyAIInfo(player);


                    }

                }
            }


            //GameManager.GetTeleportString();








        }

        public void DrawEnemyAIInfo(EnemyAI player)
        {

            Vector3 enemyBottom = player.Unit.Movement.Torso.transform.position;
            Vector3 enemyTop;
            enemyTop.x = enemyBottom.x;
            enemyTop.z = enemyBottom.z;
            enemyTop.y = enemyBottom.y + 2f;
            Vector3 worldToScreenTop = Camera.main.WorldToScreenPoint(enemyTop);
            Vector3 w2s = Camera.main.WorldToScreenPoint(enemyBottom);


            float height = Mathf.Abs(worldToScreenTop.y - w2s.y);
            float x = w2s.x - height * 0.3f;
            float y = UnityEngine.Screen.height - worldToScreenTop.y;



            // Calculate the initial positions of the text
       
            Vector2 bottomTextPosition = new Vector2(x + (height / 4f) + 3f, y + height + 3f);

            // Offset the text positions based on the player's movement
     
            bottomTextPosition -= new Vector2(player.Unit.Movement.Torso.transform.position.x - enemyBottom.x, 0f);

            float distance = Vector3.Distance(Camera.main.transform.position, player.Unit.Movement.Torso.transform.position);
            int fontSize = Mathf.Clamp(Mathf.RoundToInt(12f / distance), 10, 20);

            // Draw the text at the adjusted positions with the adjusted font size
            ESPUtils.DrawString(bottomTextPosition, player.name +"\n"+ "HP: " + player.Unit.Health.CurrentHealth, Color.red, true, fontSize, FontStyle.Bold);
    
        }

        public void DrawEnemyInfo(Player player)
        {

            Vector3 enemyBottom = player.stickManHero.Unit.Movement.Head.transform.position;
            Vector3 enemyTop;
            enemyTop.x = enemyBottom.x;
            enemyTop.z = enemyBottom.z;
            enemyTop.y = enemyBottom.y + 2f;
            Vector3 worldToScreenTop = Camera.main.WorldToScreenPoint(enemyTop);
            Vector3 w2s = Camera.main.WorldToScreenPoint(enemyBottom);


            float height = Mathf.Abs(worldToScreenTop.y - w2s.y);
            float x = w2s.x - height * 0.3f;
            float y = UnityEngine.Screen.height - worldToScreenTop.y;

         
            Vector2 bottomTextPosition = new Vector2(x + (height / 4f) + 3f, y + height + 3f);

            // Offset the text positions based on the player's movement
         
            bottomTextPosition -= new Vector2(player.stickManHero.Unit.Movement.Head.transform.position.x - enemyBottom.x, 0f);

            float distance = Vector3.Distance(Camera.main.transform.position, player.stickManHero.Unit.Movement.Head.transform.position);
            int fontSize = Mathf.Clamp(Mathf.RoundToInt(12f / distance), 10, 20);

            // Draw the text at the adjusted positions with the adjusted font size
            ESPUtils.DrawString(bottomTextPosition, player.details.playerName + "\n" + "HP: " + player.stickManHero.Unit.Health.CurrentHealth + "\n" + "ID: " + player.details.playerIndex, Color.green, true, fontSize, FontStyle.Bold);
      
        }


        public void Start()
        {
          
            blackCol = new Color(0f, 0f, 0f, 120f);
            entityBoxCol = new Color(0.42f, 0.36f, 0.90f, 1f);
        }

        public void Update()
        {

           


            if (Input.GetKeyDown(KeyCode.Insert))
            {
                showMenu = !showMenu;
            }


         

          

            natNextUpdateTime += Time.deltaTime;

            if (natNextUpdateTime >= 0.4f)
            {


                EnemyBase = FindObjectsOfType<Player>().ToList();
                LevelManager = FindObjectsOfType<LevelManager>().ToList();
                EnemyAIBase = FindObjectsOfType<EnemyAI>().ToList();
                UnlockManager = FindObjectsOfType<UnlockManager>().ToList();
               


               
                natNextUpdateTime = 0f;
            }
          
              

                   
                
            



            cam = Camera.main;

        }
    }
}

