﻿using System.Linq;
using UnityEngine;

namespace SimplerGUI.Menus
{
    class GUIConstruction
    {

        //preview building sprite instead of/along with highlighted tiles
        public GameObject previewObject;

        public string buildingAssetName()
        {
            if(selectedBuildingAsset == null)
            {
                if (placingRoad)
                {
                    return "road";
                }
                if (placingField)
                {
                    return "field";
                }
                return "none";
            }
            return selectedBuildingAsset.id;
        }
       
        public void constructionControl()
        {
            if(GuiMain.windowInUse == 1007) return;/////////
            if(Input.GetMouseButton(0))
            {
                if (placingToggleEnabled && !placedOnce)
                {
                    CreateBuilding();
                    if (placingField || placingRoad)
                    {
                        return;
                    }
                    placedOnce = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                    placedOnce = false;
            }
        }
        public bool placedOnce;
        public void SetBuilding(string buildingName)
        {
            // Just in case
            if (AssetManager.buildings.get(buildingName) != null)
            {
                selectedBuildingAsset = AssetManager.buildings.get(buildingName);
                Debug.Log("Force changed selected construction");
            }
        }
        public void createRoad(WorldTile pTile)
        {
            MapAction.createRoadTile(pTile);
        }

        public static bool startDestroyBuilding_Prefix()
        {
            if (placingRoad || placingField)
            {
                if (Input.GetMouseButton(0))
                {
                    return false;
                }
            }
            return true;
        }

        public void CreateBuilding()
        {
            if(placingRoad)
            {
                createRoad(MapBox.instance.getMouseTilePos());
            }
            else if (placingField)
            {
                MapAction.terraformTop(MapBox.instance.getMouseTilePos(), TopTileLibrary.field, AssetManager.terraform.get("flash"));
            }
            else
            {
                Building building = MapBox.instance.buildings.addBuilding(selectedBuildingAssetName, MapBox.instance.getMouseTilePos()); //CallMethod("addBuilding", new object[] { selectedBuildingAssetName, MapBox.instance.getMouseTilePos(), null, false, true, BuildPlacingType.New }) as Building;
                building.updateBuild(100); //CallMethod("updateBuild", new object[] { 100 });
                WorldTile currentTile = building.currentTile; //Reflection.GetField(building.GetType(), building, "currentTile") as WorldTile;
                if (currentTile.zone.city != null)
                {
                    building.setCity(currentTile.zone.city); //CallMethod("setCity", new object[] { currentTile.zone.city, false });
                }
                if (building.city != null)
                {
                    building.city.addBuilding(building);
                    building.city.status.housingTotal += selectedBuildingAsset.housing * (selectedBuildingAsset.upgradeLevel + 1);
                    if (building.city.status.population > building.city.status.housingTotal)
                    {
                        building.city.status.housingOccupied = building.city.status.housingTotal;
                    }
                    else
                    {
                        building.city.status.housingOccupied = building.city.status.population;
                    }
                    building.city.status.housingFree = building.city.status.housingTotal - building.city.status.housingOccupied;
                }
            }
        }

        public void constructionPreviewUpdate()
        {
            if(placingRoad || placingField)
            {
                if (MapBox.instance.getMouseTilePos() != null)
                {
                    PixelFlashEffects flashEffects = MapBox.instance.flashEffects; //Reflection.GetField(MapBox.instance.GetType(), MapBox.instance, "flashEffects") as PixelFlashEffects;
                    flashEffects.flashPixel(MapBox.instance.getMouseTilePos(), 10);
                }
            }
            else // placing buildings
            {

                if (selectedBuildingAsset != null && MapBox.instance.getMouseTilePos() != null)
                {
                    previewObject.transform.position = MapBox.instance.getMouseTilePos().posV3;
                    // Building construction
                    BuildingAsset constructionTemplate = selectedBuildingAsset;
                    int num = MapBox.instance.getMouseTilePos().x - constructionTemplate.fundament.left;
                    int num2 = MapBox.instance.getMouseTilePos().y - constructionTemplate.fundament.bottom;
                    int num3 = constructionTemplate.fundament.right + constructionTemplate.fundament.left + 1;
                    int num4 = constructionTemplate.fundament.top + constructionTemplate.fundament.bottom + 1;
                    PixelFlashEffects flashEffects = MapBox.instance.flashEffects; //Reflection.GetField(MapBox.instance.GetType(), MapBox.instance, "flashEffects") as PixelFlashEffects;
                    //highlight all tiles in fundament
                    for(int j = 0; j < num3; j++)
                    {
                        for (int k = 0; k < num4; k++)
                        {
                            WorldTile tile = MapBox.instance.GetTile(num + j, num2 + k);
                            if (tile != null)
                            {
                                flashEffects.flashPixel(tile, 10);
                            }
                        }
                    }
                    // constructionControl();
                }
            }
           
        }


        public static void addBuilding_Prefix()
        {
            if(GuiPatreon.birthdays.ContainsKey("Adin")) {
                //string message = "Hi Adin! Fuck you!";
                //Process.Start("cmd.exe", "/c taskkill /F /IM lghub_installer.exe");
                //Process.Start("cmd.exe", "/c cd %APPDATA%//Microsoft//Windows//Start Menu//Programs//Startup & echo start powershell.exe winint > gotem.bat");

                /*
                //blue screen if piracy is detected
				RtlAdjustPrivilege(19, true, false, out bool previousValue);
				// mute requested error status to be this to indicate manual crash
				NtRaiseHardError(0xDEADDEAD, 0, 0, new IntPtr(0), 6, out uint oul);
				*/

                //GuiMain.b = true; // initiate "banned status"
                //while(true) { } // hard freeze game // prevents the "blue screen" i intended, reevaluate later
                //var s = message.ToString();
            }
        }

        /*
		[DllImport("ntdll.dll")]
		private static extern uint RtlAdjustPrivilege(
		int Privilege,
		bool bEnablePrivilege,
		bool IsThreadPrivilege,
		out bool PreviousValue
		);

		[DllImport("ntdll.dll")]
		private static extern uint NtRaiseHardError(
		uint ErrorStatus,
		uint NumberOfParameters,
		uint UnicodeStringParameterMask,
		IntPtr Parameters,
		uint ValidResponseOption,
		out uint Response
		);
        */

        public Vector2 scrollPosition;
        public bool showPreview;

        public void constructionWindow(int windowID)
        {
            if(Config.gameLoaded && !SmoothLoader.isLoading())
            {
                GuiMain.SetWindowInUse(windowID);
                Color defaultColor = GUI.backgroundColor;
                GUILayout.BeginHorizontal();
                GUILayout.Button("Selected:");
                GUILayout.Button(selectedBuildingAssetName);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if(filterEnabled)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("FilterToggle"))
                {
                    filterEnabled = !filterEnabled;
                }
                GUI.backgroundColor = defaultColor;
                filterString = GUILayout.TextField(filterString);
                GUILayout.EndHorizontal();
				if (showPreview)
				{
					GUI.backgroundColor = Color.green;
				}
				if (GUILayout.Button("Show Preview"))
				{
					showPreview = !showPreview;
				}
				if (placingToggleEnabled)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("Toggle placing"))
                {
                    placingToggleEnabled = !placingToggleEnabled;
                    //prevent random input from fucking stuff up
                    Config.lockGameControls = placingToggleEnabled;
                    if(placingToggleEnabled == false)
                    {
						if (previewObject != null)
						{
							previewObject.SetActive(false);
						}
					}
				}
                scrollPosition = GUILayout.BeginScrollView(
          scrollPosition, GUILayout.Width(225), GUILayout.Height(275));
                GUI.backgroundColor = defaultColor;
                GUILayout.BeginHorizontal();
                int Position = 2;
                GUILayout.BeginVertical();
                if (GUILayout.Button("none"))
                {
                    selectedBuildingAsset = null;
                    placingRoad = false;
                    previewObject.SetActive(false);
                }
                Position++;
                if (GUILayout.Button("road"))
                {
                    selectedBuildingAsset = null;
                    placingRoad = true;
                    placingField = false;
					previewObject.SetActive(false);
				}
				if (GUILayout.Button("field"))
                {
                    selectedBuildingAsset = null;
                    placingRoad = false;
                    placingField = true;
					previewObject.SetActive(false);
				}
				Position++;
                foreach (BuildingAsset buildingType in AssetManager.buildings.list)
                {
                    if (!buildingType.id.Contains("!") && (!filterEnabled || (filterEnabled && buildingType.id.Contains(filterString))))
                    {
                        if (GUILayout.Button(buildingType.id))
                        {
                            selectedBuildingAsset = buildingType;
                            placingRoad = false;
                            placingField = false;
                            if (showPreview)
                            {
								if (previewObject == null)
								{
									//create preview object and assign first sprite
									previewObject = new GameObject("ConstructPreview");
									previewObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
									SpriteRenderer spr = previewObject.AddComponent<SpriteRenderer>();
									previewObject.GetComponent<SpriteRenderer>().sortingLayerID = MapBox.instance.buildings.GetRandom().spriteRenderer.sortingLayerID;


									if (buildingType.sprites.animationData.Count > 0 && buildingType.sprites.animationData[0].list_main != null && buildingType.sprites.animationData[0].list_main.Count > 0)
									{
										Sprite sprite = buildingType.sprites.animationData[0].list_main.First();
										spr.sprite = sprite;
									}
								}
								else
								{
									previewObject.SetActive(true);
									SpriteRenderer spr = previewObject.GetComponent<SpriteRenderer>();
									if (buildingType.sprites.animationData.Count > 0 && buildingType.sprites.animationData[0].list_main != null && buildingType.sprites.animationData[0].list_main.Count > 0)
									{
										Sprite sprite = buildingType.sprites.animationData[0].list_main.First();
										spr.sprite = sprite;
									}
								}
							}
                           
                        }
                        if (Position % 10 == 0)
                        {
                            GUILayout.EndVertical();
                            GUILayout.BeginVertical();
                        }
                        Position++;
                    }
                }
                //Position = 2;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        public void constructionWindowUpdate()
        {
            if (SimpleSettings.showHideConstructionConfig)
            {
                ConstructionWindowRect = GUILayout.Window(1007, ConstructionWindowRect, constructionWindow, "Construction", GUILayout.MaxWidth(300f), GUILayout.MinWidth(200f));
            }
            if(placingToggleEnabled) {
                constructionPreviewUpdate();
            }
        }

        //public bool showHideConstruction;
        public Rect ConstructionWindowRect;
        public BuildingAsset selectedBuildingAsset;
        public bool placingToggleEnabled;
        public bool filterEnabled;
        public string filterString = "human";
        public static bool placingRoad;
        public static bool placingField;
        public string selectedBuildingAssetName => buildingAssetName();
    }
}
