using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;

namespace LoadLevel
{
    public class LoadLevelModule : ETGModule
    {
        public override void Init()
        {
        }

        public override void Start()
        {
			LoadLevelAutocompletion = new AutocompletionSettings(delegate (string input)
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, string> pair in DungeonDictionary)
				{
					if (pair.Key.AutocompletionMatch(input.ToLower()))
					{
						Console.WriteLine(string.Format("INPUT {0} KEY {1} MATCH!", input, pair.Key));
						list.Add(pair.Key);
					}
					else
					{
						Console.WriteLine(string.Format("INPUT {0} KEY {1} NO MATCH!", input, pair.Key));
					}
				}
				return list.ToArray();
			});
			ETGModConsole.Commands.AddGroup("loadlevel", LoadLevel, LoadLevelAutocompletion);
			ETGModConsole.Commands.GetGroup("loadlevel").AddUnit("ignoredictionary", LoadLevelIgnoreDictionary, LoadLevelAutocompletion);
			ETGModConsole.Commands.GetGroup("loadlevel").AddUnit("help", LoadLevelHelp);
			ETGModConsole.Log("Load Level started successfully.");
			ETGModConsole.Log("[Load Level] Use <color=#ff0000>loadlevel help</color> to get a list of all available commands.");
		}

		public void LoadLevelIgnoreDictionary(string[] args)
		{
			if (args.Length <= 0)
			{
				ETGModConsole.Log("Failed to load level: <color=#ff0000>[level]</color> argument is missing!");
			}
			else
			{
				LoadLevel(args[0], true, args.Length > 1 ? bool.Parse(args[1]) : false);
			}
		}

		public void LoadLevel(string[] args)
		{
			if (args.Length <= 0)
			{
				LoadLevelHelp(args);
			}
			else
			{
				LoadLevel(args[0], false, args.Length > 1 ? bool.Parse(args[1]) : false);
			} 
		}

		public void LoadLevel(string level, bool ignoreDictionary, bool glitched)
		{
			ETGModConsole.Log("[Load Level] Attempting to load level \"" + level + "\"...");
			string sceneName = level;
			if (DungeonDictionary.ContainsKey(sceneName.ToLower()) && !ignoreDictionary)
			{
				sceneName = DungeonDictionary[sceneName.ToLower()];
			}
			if (GameManager.HasInstance)
			{
				GameLevelDefinition gameLevelDefinition = null;
				for (int i = 0; i < GameManager.Instance.dungeonFloors.Count; i++)
				{
					if (GameManager.Instance.dungeonFloors[i].dungeonSceneName.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
					{
						gameLevelDefinition = GameManager.Instance.dungeonFloors[i];
						break;
					}
				}
				if (gameLevelDefinition == null)
				{
					for (int j = 0; j < GameManager.Instance.customFloors.Count; j++)
					{
						if (GameManager.Instance.customFloors[j].dungeonSceneName.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
						{
							gameLevelDefinition = GameManager.Instance.customFloors[j];
							break;
						}
					}
				}
				if (gameLevelDefinition == null)
				{
					ETGModConsole.Log("[Load Level] Failed to load level: Level \"" + sceneName + "\" doesn't exist!");
				}
				else
				{
					if(GameManager.Instance.IsFoyer && Foyer.Instance != null)
                    {
						Foyer.Instance.OnDepartedFoyer();
					}
                    if (glitched)
                    {
						GameManager.Instance.InjectedFlowPath = "Core Game Flows/Secret_DoubleBeholster_Flow";
					}
					if((level == "gunslingerpast" || level == "gunslinger_past") && !ignoreDictionary)
                    {
						GameManager.IsGunslingerPast = true;
                    }
					if(sceneName == "fs_coop")
                    {
						GameManager.IsCoopPast = true;
                    }
					GameManager.Instance.LoadCustomLevel(sceneName);
					ETGModConsole.Log("[Load Level] Successfully loaded level \"" + level + "\" with scene name \"" + sceneName + "\".");
				}
			}
			else
			{
				ETGModConsole.Log("[Load Level] Failed to load level: Game Manager doesn't exist!");
			}
		}

		public void LoadLevelHelp(string[] args)
        {
			string str = "<color=#00ff00>AVAILABLE COMMANDS:</color>\n" +
				"     <color=#ff0000>* loadlevel [level] [glitched]</color> - loads a level, if <color=#ff0000>[level]</color> argument is missing shows help.If the level dictionary contains <color=#ff0000>[level]</color>, loads a level from level dictionary " +
				"instead. If <color=#ff0000>[glitched]</color> argument is \"true\" or \"True\", loads a glitched version of the level instead\n" +
				"     <color=#ff0000>* loadlevel ignoredictionary [level] [glitched]</color> - loads a level. Does <color=#ff0000>NOT</color> load a level from the level dictionary even if it contains <color=#ff0000>[level]</color>. If " +
				"<color=#ff0000>[glitched]</color> argument is \"true\" or \"True\", loads a glitched version of the level instead\n" +
				"     <color=#ff0000>* loadlevel help</color> - shows help.\n" +
				"<color=#00ff00>LEVEL DICTIONARY:</color>\n" +
				"     <color=#00ffff>* keep</color>, <color=#00ffff>leadlordkeep</color> or <color=#00ffff>lead_lord_keep</color> - Keep of the Lead Lord.\n" +
				"     <color=#00ffff>* proper</color>, <color=#00ffff>gungeon</color>, <color=#00ffff>gungeonproper</color> or <color=#00ffff>gungeon_proper</color> - Gungeon Proper\n" +
				"     <color=#00ffff>* mines</color>, <color=#00ffff>mine</color>, <color=#00ffff>powdermines</color>, <color=#00ffff>powdermine</color>, <color=#00ffff>powder_mines</color> or <color=#00ffff>powder_mine</color> - Black Powder Mines\n" +
				"     <color=#00ffff>* hollow</color> - Hollow\n" +
				"     <color=#00ffff>* forge</color> - Forge\n" +
				"     <color=#00ffff>* hell</color>, <color=#00ffff>bullethell</color> or <color=#00ffff>bullet_hell</color> - Bullet Hell\n" +
				"     <color=#00ffff>* oubliette</color>, <color=#00ffff>sewer</color> or <color=#00ffff>sewers</color> - Oubliette\n" +
				"     <color=#00ffff>* abbey</color>, <color=#00ffff>truegunabbbey</color> or <color=#00ffff>true_gun_abbey</color> - Abbey of the True Gun\n" +
				"     <color=#00ffff>* ratlair</color>, <color=#00ffff>ratden</color>, <color=#00ffff>rat_lair</color> or <color=#00ffff>rat_den</color> - Resourceful Rat's Lair\n" +
				"     <color=#00ffff>* rng</color>, <color=#00ffff>r&g</color>, <color=#00ffff>rngdept</color> or <color=#00ffff>r&gdept</color>, <color=#00ffff>rng_dept</color> or <color=#00ffff>r&g_dept</color> - R&G Dept.\n" +
				"     <color=#00ffff>* marinepast</color> or <color=#00ffff>marine_past</color> - Marine's Past\n" +
				"     <color=#00ffff>* convictpast</color> or <color=#00ffff>convict_past</color> - Convict's Past\n" +
				"     <color=#00ffff>* hunterpast</color> or <color=#00ffff>hunter_past</color> - Hunter's Past\n" +
				"     <color=#00ffff>* pilotpast</color> or <color=#00ffff>pilot_past</color> - Pilot's Past\n" +
				"     <color=#00ffff>* robotpast</color> or <color=#00ffff>robot_past</color> - Robot's Past\n" +
				"     <color=#00ffff>* bulletpast</color> or <color=#00ffff>bullet_past</color> - Bullet's Past\n" +
				"     <color=#00ffff>* gunslingerpast</color> or <color=#00ffff>gunslinger_past</color> - Gunslinger's Past\n" +
				"     <color=#00ffff>* cultistpast</color>, <color=#00ffff>cultist_past</color>, <color=#00ffff>cooppast</color> or <color=#00ffff>coop_past</color> - Cultist's Past\n" +
				"     <color=#00ffff>* tutorial</color>, <color=#00ffff>halls</color>, <color=#00ffff>knowledgehalls</color> or <color=#00ffff>knowledge_halls</color> - Halls of Knowledge\n" +
				"     <color=#00ffff>* jungle</color> - Jungle <color=#ff0000>(only works with Expand the Gungeon)</color>\n" +
				"     <color=#00ffff>* belly</color> - Belly <color=#ff0000>(only works with Expand the Gungeon)</color>\n" +
				"     <color=#00ffff>* west</color>, <color=#00ffff>oldwest</color> or <color=#00ffff>old_west</color> - Old West <color=#ff0000>(only works with Expand the Gungeon)</color>\n" +
				"<color=#00ff00>AVAILABLE LEVELS:</color>";
			List<string> availableLevels = new List<string>();
            if (GameManager.HasInstance)
            {
				if(GameManager.Instance.dungeonFloors != null)
				{
					foreach (GameLevelDefinition def in GameManager.Instance.dungeonFloors)
					{
						availableLevels.Add("\n     <color=#00ffff>* " + def.dungeonSceneName + " (prefab path: " + def.dungeonPrefabPath + ")</color>");
					}
				}
				if (GameManager.Instance.customFloors != null)
				{
					foreach (GameLevelDefinition def in GameManager.Instance.customFloors)
					{
						availableLevels.Add("\n     <color=#00ffff>* " + def.dungeonSceneName + " (prefab path: " + def.dungeonPrefabPath + ")</color>");
					}
				}
			}
			ETGModConsole.Log(str + string.Join("", availableLevels.ToArray()));
        }

        public override void Exit()
        {
        }

        public static AutocompletionSettings LoadLevelAutocompletion;
		public static Dictionary<string, string> DungeonDictionary = new Dictionary<string, string>
		{
			{ "keep", "tt_castle" },
			{ "leadlordkeep", "tt_castle" },
			{ "lead_lord_keep", "tt_castle" },
			{ "proper", "tt5" },
			{ "gungeon", "tt5" },
			{ "gungeonproper", "tt5" },
			{ "gungeon_proper", "tt5" },
			{ "mines", "tt_mines" },
			{ "mine", "tt_mines" },
			{ "powdermines", "tt_mines" },
			{ "powdermine", "tt_mines" },
			{ "powder_mines", "tt_mines" },
			{ "powder_mine", "tt_mines" },
			{ "hollow", "tt_catacombs" },
			{ "forge", "tt_forge" },
			{ "hell", "tt_bullethell" },
			{ "bullethell", "tt_bullethell" },
			{ "bullet_hell", "tt_bullethell" },
			{ "oubliette", "tt_sewer" },
			{ "sewer", "tt_sewer" },
			{ "sewers", "tt_sewer" },
			{ "abbey", "tt_cathedral" },
			{ "truegunabbbey", "tt_cathedral" },
			{ "true_gun_abbey", "tt_cathedral" },
			{ "ratlair", "ss_resourcefulrat" },
			{ "ratden", "ss_resourcefulrat" },
			{ "rat_lair", "ss_resourcefulrat" },
			{ "rat_den", "ss_resourcefulrat" },
			{ "rng", "tt_nakatomi" },
			{ "r&g", "tt_nakatomi" },
			{ "rngdept", "tt_nakatomi" },
			{ "r&gdept", "tt_nakatomi" },
			{ "rng_dept", "tt_nakatomi" },
			{ "r&g_dept", "tt_nakatomi" },
			{ "marinepast", "fs_soldier" },
			{ "marine_past", "fs_soldier" },
			{ "convictpast", "fs_convict" },
			{ "convict_past", "fs_convict" },
			{ "hunterpast", "fs_guide" },
			{ "hunter_past", "fs_guide" },
			{ "pilotpast", "fs_pilot" },
			{ "pilot_past", "fs_pilot" },
			{ "robotpast", "fs_robot" },
			{ "robot_past", "fs_robot" },
			{ "bulletpast", "fs_bullet" },
			{ "bullet_past", "fs_bullet" },
			{ "gunslingerpast", "tt_bullethell" },
			{ "gunslinger_past", "tt_bullethell" },
			{ "jungle", "tt_jungle" },
			{ "belly", "tt_belly" },
			{ "west", "tt_west" },
			{ "oldwest", "tt_west" },
			{ "old_west", "tt_west" },
			{ "tutorial", "tt_tutorial" },
			{ "halls", "tt_tutorial" },
			{ "knowledgehalls", "tt_tutorial" },
			{ "knowledge_halls", "tt_tutorial" },
			{ "cultistpast", "fs_coop" },
			{ "cultist_past", "fs_coop" },
			{ "cooppast", "fs_coop" },
			{ "coop_past", "fs_coop" },
		};
    }
}
