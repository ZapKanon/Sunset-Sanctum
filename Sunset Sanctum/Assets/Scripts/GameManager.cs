using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fungus;

public enum TimeOfDay
{
    sunrise,
    sunset,
    night
}

public class GameManager : MonoBehaviour
{
    //Time tracking
    public int currentDay;
    public const int finalDay = 12;
    public TimeOfDay timeOfDay;

    //Resources the player has gathered
    public int emptyBottles;
    public int bottledSunlight;

    public int plantPots;
    public int seashells;
    public int seaFlowers;

    //Player Stats
    public int mentalLight;
    public int devotion;
    public int health;

    //Player item find min / max values
    //Based on stats being above certain thresholds / prayer effects
    public int plantPotsMin = 1;
    public int plantPotsMax = 3;
    public int emptyBottlesMin = 2;
    public int emptyBottlesMax = 2;
    public int bottledSunlightMin = 1;
    public int bottledSunlightMax = 3;
    public int seashellsMin = 1;
    public int seashellsMax = 3;
    public int seaFlowersMin = 1;
    public int seaFlowersMax = 3;

    private const string SaveDataFile = "SanctumSave.txt";

    //Ending variables
    public bool sunEnding = false;
    public bool darkEnding = false;
    public bool starsEnding = false;
    public bool moonEnding = false;

    [SerializeField] private GameObject returnToMenuButton;
    [SerializeField] private GameObject titleMenuContainer;
    [SerializeField] private Image titleSunSetting;
    [SerializeField] private Image titleSun;
    [SerializeField] private Image titleMoon;
    [SerializeField] private Image titleStars;
    [SerializeField] private Image titleOcean;
    [SerializeField] private Image titleOceanDark;
    [SerializeField] private Image titleSunbeam;
    [SerializeField] private Image titleBackground;
    [SerializeField] private Image titleBackgroundDark;

    [SerializeField] private Image actionBackground;
    [SerializeField] private DevotionNodes devotionManager;
    [SerializeField] private TextMeshProUGUI currentDateDisplay;
    [SerializeField] private TextMeshProUGUI sunriseDisplay;
    [SerializeField] private TextMeshProUGUI sunsetDisplay;
    [SerializeField] private TextMeshProUGUI nightDisplay;
    [SerializeField] private GameObject actionSunriseContainer;
    [SerializeField] private GameObject actionSunsetContainer;
    [SerializeField] private GameObject actionNightContainer;
    [SerializeField] private List<TextMeshProUGUI> itemValues;
    [SerializeField] private List<TextMeshProUGUI> statValues;
    [SerializeField] private List<Image> statBars;
    [SerializeField] private GameObject devotionMenu;
    [SerializeField] private TextMeshProUGUI devotionToggleText;
    [SerializeField] private TextMeshProUGUI lastActionName;
    [SerializeField] private TextMeshProUGUI lastActionText1;
    [SerializeField] private TextMeshProUGUI lastActionText2;
    [SerializeField] private GameObject RandomDevotionContainer;
    [SerializeField] private List<GameObject> randomReportTexts;
    
    [SerializeField] private List<GameObject> speakButtons;
    [SerializeField] private Flowchart flowchart;

    // Start is called before the first frame update
    void Start()
    {
        UpdateItemFind();
        LoadGame();
        DisplayTitleMenu();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplayedValues();
    }

    /// <summary>
    /// Show the action buttons appropriate at sunrise.
    /// </summary>
    public void DisplaySunriseActions()
    {
        actionSunriseContainer.SetActive(true);
        actionSunsetContainer.SetActive(false);
        actionNightContainer.SetActive(false);
    }

    /// <summary>
    /// Show the action buttons appropriate at sunset.
    /// </summary>
    public void DisplaySunsetActions()
    {
        actionSunriseContainer.SetActive(false);
        actionSunsetContainer.SetActive(true);
        actionNightContainer.SetActive(false);

        RandomDevotionContainer.SetActive(false);
        randomReportTexts[0].SetActive(false);
        randomReportTexts[1].SetActive(false);
        randomReportTexts[2].SetActive(false);
        randomReportTexts[3].SetActive(false);
    }

    /// <summary>
    /// Show night action buttons once the player has chosen an option.
    /// </summary>
    public void DisplayNightActions()
    {
        actionSunriseContainer.SetActive(false);
        actionSunsetContainer.SetActive(false);
        actionNightContainer.SetActive(true);
    }

    /// <summary>
    /// Update displayed stats and item quantities.
    /// </summary>
    public void UpdateDisplayedValues()
    {
        //Today's Date
        currentDateDisplay.text = string.Format("Day {0}/{1}", currentDay, finalDay);

        //Item quanitites
        itemValues[0].text = seashells.ToString();
        itemValues[1].text = plantPots.ToString();
        itemValues[2].text = seaFlowers.ToString();
        itemValues[3].text = emptyBottles.ToString();
        itemValues[4].text = bottledSunlight.ToString();

        //Stats
        statValues[0].text = health.ToString();
        statValues[1].text = devotion.ToString();
        statValues[2].text = mentalLight.ToString();

        statBars[0].fillAmount = health / 100f;
        statBars[1].fillAmount = devotion / 100f;
        statBars[2].fillAmount = mentalLight / 100f;
    }

    /// <summary>
    /// After any action has been completed, advance the time of day accordingly.
    /// </summary>
    public void CompleteAction()
    {
        CheckStats();
        switch (timeOfDay)
        {
            case TimeOfDay.sunrise:
                timeOfDay = TimeOfDay.sunset;
                sunriseDisplay.enabled = false;
                sunsetDisplay.enabled = true;
                DisplaySunsetActions();
                
                actionBackground.color = new Color32(231, 120, 0, 200);
                break;
            
            case TimeOfDay.sunset:
                timeOfDay = TimeOfDay.night;
                sunsetDisplay.enabled = false;
                nightDisplay.enabled = true;
                //Show the current day's speak button
                for (int i = 0; i < speakButtons.Count; i++)
                {
                    speakButtons[i].SetActive(false);
                }
                speakButtons[currentDay - 1].SetActive(true);
                DisplayNightActions();

                actionBackground.color = new Color32(110, 83, 241, 200);
                break;

            case TimeOfDay.night:
                actionBackground.color = new Color32(255, 210, 85, 200);
                //Gain 3 free devotion each night
                //Darkness disciple prevents all devotion gain
                if ((!devotionManager.nodes[(int)NodeNames.DarknessDisciple].purchased || health <= 80))
                {
                    devotion += 5;

                    //Moonlight acolyte grants 10 extra devotion
                    if (devotionManager.nodes[(int)NodeNames.MoonlightAcolyte].purchased && mentalLight <= 20)
                    {
                        devotion += 10;
                    }
                }
                   
                //Proceed to the next day if this is not the final day.
                if (currentDay < finalDay)
                {
                    EndDay();
                }
                else
                {
                    EndFinalDay();
                    //TEMP: Hide night actions
                    actionNightContainer.SetActive(false);
                }
                break;
        }
    }

    /// <summary>
    /// Once the player selects an action at sunset, hide the action buttons and go to the next day.
    /// Sleeping consumes one bottled sunlight or reduces mental light. Health is lost as well.
    /// </summary>
    public void EndDay()
    {

        //UpdateDisplayedValues();
        lastActionName.text = "Retired for the Night";

        //Consume a sea flower to maintain health
        if (seaFlowers > 0)
        {
            //25% chance to keep a sea flower
            if (devotionManager.nodes[(int)NodeNames.PreserveFlowers].purchased)
            {
                if (Random.Range(1, 5) == 1)
                {
                    seaFlowers += 1;
                    randomReportTexts[3].SetActive(true);
                }
                else
                {
                    randomReportTexts[3].SetActive(false);
                }
            }
            seaFlowers -= 1;
            

            int restoredHealth = Random.Range(5, 11);
            health += restoredHealth;
            lastActionText1.text = "Ate a sea flower, restoring " + restoredHealth + " health.";
        }
        else
        {
            int lostHealth = Random.Range(15, 21);

            //Starclad Servant prevents health drop
            if (devotionManager.nodes[(int)NodeNames.Enduring].purchased && devotion >= 80)
            {
                lostHealth = 0;
            }

            health -= lostHealth;
            lastActionText1.text = "With no sea flower to eat, lost " + lostHealth + " health.";
        }

        //Consume bottled sunlight to maintain mental light
        if (bottledSunlight > 0)
        {
            //25% chance to keep a sea flower
            if (devotionManager.nodes[(int)NodeNames.PreserveSunlight].purchased)
            {
                if (Random.Range(1, 5) == 1)
                {
                    bottledSunlight += 1;
                    randomReportTexts[2].SetActive(true);
                }
                else
                {
                    randomReportTexts[2].SetActive(false);
                }
            }
            bottledSunlight -= 1;
            
            int restoredMentalLight = Random.Range(5, 11);
            mentalLight += restoredMentalLight;
            lastActionText2.text = "Protected by bottled sunlight, restoring " + restoredMentalLight + " mental light.";
        }
        else
        {
            int lostMentalLight = Random.Range(15, 21);
            mentalLight -= lostMentalLight;
            lastActionText2.text = "With no protection from bottled sunlight, lost " + lostMentalLight + " mental light.";
        }

        //Next Morning
        timeOfDay = TimeOfDay.sunrise;
        sunriseDisplay.enabled = true;
        nightDisplay.enabled = false;
        DisplaySunriseActions();
        currentDay += 1;

        //50% chance to gain a seashell
        if (devotionManager.nodes[(int)NodeNames.OceanGift].purchased)
        {
            if (Random.Range(1, 3) == 1)
            {
                seashells += 1;
                randomReportTexts[0].SetActive(true);
            }
            else
            {
                randomReportTexts[0].SetActive(false);
            }
        }

        //50% chance to gain a plant pot
        if (devotionManager.nodes[(int)NodeNames.EarthGift].purchased)
        {
            if (Random.Range(1, 3) == 1)
            {
                plantPots += 1;
                randomReportTexts[1].SetActive(true);
            }
            else
            {
                randomReportTexts[1].SetActive(false);
            }
        }

        UpdateItemFind();
        CheckStats();
        RandomDevotionContainer.SetActive(true);
    }

    /// <summary>
    /// Check if the player has run out of health or mental light.
    /// Hitting 0 in either of these stats causes a game over.
    /// </summary>
    public void CheckStats()
    {
        if (mentalLight > 100)
        {
            mentalLight = 100;
        }
        else if (mentalLight <= 0)
        {
            mentalLight = 0;
            //Game Over
            Debug.Log("Game Over. No mental light.");
            GameOver();
        }

        if (health > 100)
        {
            health = 100;
        }
        else if (health <= 0)
        {
            health = 0;
            //Game Over
            Debug.Log("Game Over. No health.");
            GameOver();
        }

        if (devotion >= 100)
        {
            devotion = 100;
        }
    }

    /// <summary>
    /// Present an ending to the player based on their allegiance to the four factions.
    /// </summary>
    public void EndFinalDay()
    {
        if (devotionManager.nodes[(int)NodeNames.SunlightScion].purchased)
        {
            //Sun Ending
            Debug.Log("Sun Ending");
            sunEnding = true;
            flowchart.ExecuteBlock("Sun Ending");
        }
        else if (devotionManager.nodes[(int)NodeNames.MoonlightAcolyte].purchased)
        {
            //Moon Ending
            Debug.Log("Moon Ending");
            moonEnding = true;
            flowchart.ExecuteBlock("Moon Ending");
        }
        else if (devotionManager.nodes[(int)NodeNames.StarcladServant].purchased)
        {
            //Stars Ending
            Debug.Log("Stars Ending");
            starsEnding = true;
            flowchart.ExecuteBlock("Stars Ending");
        }
        else if (devotionManager.nodes[(int)NodeNames.DarknessDisciple].purchased)
        {
            //Darkness Ending
            Debug.Log("Darkness Ending");
            darkEnding = true;
            flowchart.ExecuteBlock("Darkness Ending");
        }
        else
        {
            //Normal Ending
            Debug.Log("Normal Ending");
            flowchart.ExecuteBlock("Normal Ending");
        }

        //Button to return to menu
        returnToMenuButton.SetActive(true);

        //Save obtained endings
        SaveGame();
    }

    /// <summary>
    /// Update min / max values for finding and crafting items based on stats / prayer effects
    /// </summary>
    public void UpdateItemFind()
    {

    }

    //---------------------ACTIONS-----------------------

    /// <summary>
    /// Search the beach for bottles and seashells that have washed ashore.
    /// </summary>
    public void CombBeach()
    {
        int foundEmptyBottles = Random.Range(emptyBottlesMin, emptyBottlesMax + 1);
        int foundSeashells = Random.Range(seashellsMin, seashellsMax + 1);

        //Devotion doubling
        if (devotionManager.nodes[(int)NodeNames.MatchingBottles].purchased)
        {
            foundEmptyBottles *= 2;
        }
        if (devotionManager.nodes[(int)NodeNames.SellingSeashells].purchased)
        {
            foundSeashells *= 2;
        }

        emptyBottles += foundEmptyBottles;
        seashells += foundSeashells;

        //Display action results
        lastActionName.text = "Combed the Beach";
        lastActionText1.text = "Found " + foundSeashells + " seashell(s).";
        lastActionText2.text = "Found " + foundEmptyBottles + " empty bottle(s).";

        CompleteAction();
    }

    /// <summary>
    /// Remove debris from the sanctum, unearthing hidden pots and bottles.
    /// </summary>
    public void ClearDebris()
    {
        int foundEmptyBottles = Random.Range(emptyBottlesMin, emptyBottlesMax + 1);
        int foundPlantPots = Random.Range(plantPotsMin, plantPotsMax + 1);

        //Devotion doubling
        if (devotionManager.nodes[(int)NodeNames.MatchingBottles].purchased)
        {
            foundEmptyBottles *= 2;
        }
        if (devotionManager.nodes[(int)NodeNames.FillingPots].purchased)
        {
            foundPlantPots *= 2;
        }

        emptyBottles += foundEmptyBottles;
        plantPots += foundPlantPots;

        //Display action results
        lastActionName.text = "Cleared Debris";
        lastActionText1.text = "Found " + foundPlantPots + " plant pot(s).";
        lastActionText2.text = "Found " + foundEmptyBottles + " empty bottle(s).";

        CompleteAction();
    }

    /// <summary>
    /// Pray within the sanctum, deepening your devotion.
    /// </summary>
    public void Pray()
    {
        int gainedDevotion = Random.Range(15, 21);

        //Devotion doubling
        if (devotionManager.nodes[(int)NodeNames.Embraced].purchased)
        {
            gainedDevotion *= 2;
        }

        devotion += gainedDevotion;

        //Display action results
        lastActionName.text = "Prayed";
        lastActionText1.text = "Gained " + gainedDevotion + " devotion.";
        lastActionText2.text = "";

        CompleteAction();
    }

    /// <summary>
    /// Place empty bottles to collect sunlight.
    /// </summary>
    public void SetOutBottles()
    {
        int sucessfulBottles = emptyBottles;

        bottledSunlight += sucessfulBottles;
        emptyBottles = 0;

        //Display action results
        lastActionName.text = "Set out Bottles";
        lastActionText1.text = "Captured " + sucessfulBottles + " bottled sunlight.";
        lastActionText2.text = "";

        CompleteAction();
    }

    /// <summary>
    /// Plant seashells within plant pots to grow edible sea flowers.
    /// </summary>
    public void PlantSeashells()
    {
        //Can only create a sea flower with both a plant pot and a seashell
        //Make as many as possible until either resource is depleted
        int successfulPlants = Mathf.Min(seashells, plantPots);

        seashells -= successfulPlants;
        plantPots -= successfulPlants;

        //Devotion doubling
        if (devotionManager.nodes[(int)NodeNames.BloomingFlowers].purchased)
        {
            successfulPlants *= 2;
        }

        seaFlowers += successfulPlants;

        //Display action results
        lastActionName.text = "Planted Seashells";
        lastActionText1.text = "Harvested " + successfulPlants + " sea flowers.";
        lastActionText2.text = "";

        CompleteAction();
    }

    /// <summary>
    /// Speak to someone who has visited the sanctum.
    /// </summary>
    public void Speak()
    {
        //TODO: Add FUNGUS functionality
    }

    /// <summary>
    /// Choose to end the day.
    /// </summary>
    public void EatSleep()
    {
        //Yes EatSleep() is currently redundant, but it might have additional functionality at some point.
        CompleteAction();
    }

    //---------------------DEVOTION-----------------------

    //Open the devotion menu to choose devotion upgrades
    public void ToggleDevotionMenu()
    {
        //Toggle the devotion menu
        devotionMenu.SetActive(!devotionMenu.activeSelf);
        devotionManager.SetUpMenu();

        //Update button text to match available function
        if (devotionMenu.activeSelf)
        {
            devotionToggleText.text = "Hide Devotion";
        }
        else
        {
            devotionToggleText.text = "Show Devotion";
        }   
    }

    /// <summary>
    /// Return the game to its initial state and return to the title screen.
    /// </summary>
    public void ResetGame()
    {
        lastActionName.text = "";
        lastActionText1.text = "";
        lastActionText2.text = "";
        health = 50;
        devotion = 0;
        mentalLight = 65;

        EndDay();

        lastActionName.text = "";
        lastActionText1.text = "";
        lastActionText2.text = "";
        health = 50;
        devotion = 0;
        mentalLight = 65;

        emptyBottles = 2;
        seashells = 2;
        plantPots = 2;
        bottledSunlight = 2;
        seaFlowers = 2;

        currentDay = 1;

        if(devotionMenu.activeSelf)
        {
            ToggleDevotionMenu();            
        }

        foreach (Node node in devotionManager.nodes)
        {
            node.available = false;
            node.purchased = false;
            node.active = false;
        }

        RandomDevotionContainer.SetActive(false);
        randomReportTexts[0].SetActive(false);
        randomReportTexts[1].SetActive(false);
        randomReportTexts[2].SetActive(false);
        randomReportTexts[3].SetActive(false);

        DisplayTitleMenu();
    }

    /// <summary>
    /// Show the title menu and configure its appearance based on obtained endings.
    /// </summary>
    public void DisplayTitleMenu()
    {
        returnToMenuButton.SetActive(false);
        titleMenuContainer.SetActive(true);

        //End any active dialogue
        flowchart.StopAllBlocks();

                //Save obtained endings
        SaveGame();

        //Sun
        titleSunSetting.enabled = !sunEnding;
        titleSun.enabled = sunEnding;
        titleSunbeam.enabled = !sunEnding;

        //Moon
        titleMoon.enabled = moonEnding;

        //Stars
        titleStars.enabled = starsEnding;

        //Dark
        titleBackground.enabled = !darkEnding;
        titleBackgroundDark.enabled = darkEnding;
        titleOcean.enabled = !darkEnding;
        titleOceanDark.enabled = darkEnding;

        //Play the final dialogue if all endings have been obtained
        if (sunEnding && moonEnding && starsEnding && darkEnding)
        {
            flowchart.ExecuteBlock("Finallusion");            
        }
    }

    /// <summary>
    /// Close the title menu to start the game.
    /// </summary>
    public void PlayGame()
    {
        titleMenuContainer.SetActive(false);
        flowchart.StopAllBlocks();
    }

    /// <summary>
    /// Forced game over when the player reaches 0 health or mental light
    /// </summary>
    public void GameOver()
    {
        actionSunriseContainer.SetActive(false);
        returnToMenuButton.SetActive(true);

        lastActionName.text = "Game Over";

        if (mentalLight <= 0 && health <= 0)
        {
            //No mental light AND no health message
            lastActionText1.text = "Body succumbed from lack of nourishment.";
            lastActionText2.text = "Mind succumbed from lack of light.";
        }
        else if (mentalLight <= 0)
        {
            //No mental light message
            lastActionText1.text = "Mind succumbed from lack of light.";
            lastActionText2.text = "";
        }
        else
        {
            //No health message
            lastActionText1.text = "Body succumbed from lack of nourishment.";
            lastActionText2.text = "";
        }
    }

    /// <summary>
    /// Save the endings the player has obtained.
    /// </summary>
    public void SaveGame()
    {
        //Build the save data string
        string saveData = string.Format("{0},{1},{2},{3}", sunEnding.ToString(), moonEnding.ToString(), starsEnding.ToString(), darkEnding.ToString());

        //Save data to file
        File.WriteAllText(SaveDataFile, saveData);
    }

    /// <summary>
    /// Load the endings the player has obtained.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(SaveDataFile))
        {
            //File format:
            //Bool value (true, false) for each ending
            //Sun, Moon, Stars, Darkness
            string[] fileContent = File.ReadAllText(SaveDataFile).Split(',');

            //Set ending status
            bool.TryParse(fileContent[0], out bool ending);
            sunEnding = ending;

            bool.TryParse(fileContent[1], out ending);
            moonEnding = ending;

            bool.TryParse(fileContent[2], out ending);
            starsEnding = ending;

            bool.TryParse(fileContent[3], out ending);
            darkEnding = ending;
        }
    }

    /// <summary>
    /// Exit the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
