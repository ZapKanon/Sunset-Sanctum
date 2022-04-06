using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum NodeNames
{ 
    SunsetSanctum,
    PreserveSunlight,
    PreserveFlowers,
    EarthGift,
    OceanGift,
    BloomingFlowers,
    MatchingBottles,
    SellingSeashells,
    FillingPots,
    Embraced,
    Unbound,
    Enduring,
    Flowing,
    SunlightScion,
    DarknessDisciple,
    StarcladServant,
    MoonlightAcolyte
}

public class DevotionNodes : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI nodeTitleText;
    [SerializeField] private TextMeshProUGUI nodeDescriptionText;
    [SerializeField] private TextMeshProUGUI nodeCostText;
    [SerializeField] public List<Node> nodes;

    private string baseCostText = "Receive (";
    private string endText = ")";
    private int tier1Cost = 20;
    private int tier2Cost = 40;
    private int tier3Cost = 60;
    private int finalCost = 100;
    private int finalCostDarkness = 0;

    private Node activeNode;
    private int activeCost;

    // Start is called before the first frame update
    void Start()
    {
        //SetUpMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpMenu()
    {
        activeNode = nodes[(int)NodeNames.SunsetSanctum];
        DisplayNodeInfo(0);
        ShowUnlockedNodes();
    }

    public void HoveredNode(Node hoveredNode)
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            if (hoveredNode == nodes[i])
            {
                activeNode = hoveredNode;
                DisplayNodeInfo(i);             
            }
        }
    }

    /// <summary>
    /// Show node information only when an adjacent node has been purchased.
    /// </summary>
    public void ShowUnlockedNodes()
    {
        //Logic for node visiblity

        //All nodes start outlined except for final nodes
        nodes[(int)NodeNames.SunlightScion].outlined = false;
        nodes[(int)NodeNames.DarknessDisciple].outlined = false;
        nodes[(int)NodeNames.StarcladServant].outlined = false;
        nodes[(int)NodeNames.MoonlightAcolyte].outlined = false;

        //Root node is always visible
        nodes[(int)NodeNames.SunsetSanctum].available = true;

        //Tier 1 nodes are made visible after purchasing root node
        if (nodes[(int)NodeNames.SunsetSanctum].purchased)
        {
            nodes[(int)NodeNames.PreserveSunlight].available = true;
            nodes[(int)NodeNames.PreserveFlowers].available = true;
            nodes[(int)NodeNames.EarthGift].available = true;
            nodes[(int)NodeNames.OceanGift].available = true;
        }

        //Each Tier 1 node has its own visiblity unlocks
        if (nodes[(int)NodeNames.PreserveSunlight].purchased)
        {
            nodes[(int)NodeNames.FillingPots].available = true;
            nodes[(int)NodeNames.Embraced].available = true;
            nodes[(int)NodeNames.MatchingBottles].available = true;
        }

        if (nodes[(int)NodeNames.PreserveFlowers].purchased)
        {
            nodes[(int)NodeNames.BloomingFlowers].available = true;
            nodes[(int)NodeNames.Unbound].available = true;
            nodes[(int)NodeNames.SellingSeashells].available = true;
        }

        if (nodes[(int)NodeNames.EarthGift].purchased)
        {
            nodes[(int)NodeNames.BloomingFlowers].available = true;
            nodes[(int)NodeNames.Enduring].available = true;
            nodes[(int)NodeNames.FillingPots].available = true;
        }

        if (nodes[(int)NodeNames.OceanGift].purchased)
        {
            nodes[(int)NodeNames.MatchingBottles].available = true;
            nodes[(int)NodeNames.Flowing].available = true;
            nodes[(int)NodeNames.SellingSeashells].available = true;
        }

        //Tier 3 nodes reveal their respective final nodes
        if (nodes[(int)NodeNames.Embraced].purchased)
        {
            nodes[(int)NodeNames.SunlightScion].outlined = true;
            nodes[(int)NodeNames.SunlightScion].available = true;
        }

        if (nodes[(int)NodeNames.Unbound].purchased)
        {
            nodes[(int)NodeNames.DarknessDisciple].outlined = true;
            nodes[(int)NodeNames.DarknessDisciple].available = true;
        }

        if (nodes[(int)NodeNames.Enduring].purchased)
        {
            nodes[(int)NodeNames.StarcladServant].outlined = true;
            nodes[(int)NodeNames.StarcladServant].available = true;
        }

        if (nodes[(int)NodeNames.Flowing].purchased)
        {
            nodes[(int)NodeNames.MoonlightAcolyte].outlined = true;
            nodes[(int)NodeNames.MoonlightAcolyte].available = true;
        }

        //Visually update all nodes to match new states
        foreach (Node node in nodes)
        {
            node.UpdateAppearance();
        }
    }

    public void DisplayNodeInfo(int i)
    {
        NodeNames hoveredNode = (NodeNames)i;
        switch (hoveredNode)
        {
            case NodeNames.SunsetSanctum:
                nodeTitleText.text = "Sunset Sanctum";
                nodeDescriptionText.text = "Deepen your devotion within this Sunset Sanctum and receive blessings from above.";
                nodeCostText.text = baseCostText + finalCostDarkness + endText;
                activeCost = finalCostDarkness;
                break;

            case NodeNames.PreserveSunlight:
                nodeTitleText.text = "Preserve Sunlight";
                nodeDescriptionText.text = "25% chance to preserve a bottled sunlight overnight." +
                    "\n\nBottled sunlight protects one's sleeping mind from the debilitating influence of darkness.";
                nodeCostText.text = baseCostText + tier1Cost + endText;
                activeCost = tier1Cost;
                break;

            case NodeNames.PreserveFlowers:
                nodeTitleText.text = "Preserve Flowers";
                nodeDescriptionText.text = "25% chance to preserve a sea flower overnight." +
                    "\n\nSea flowers provide nourishment to maintain one's physical presence.";
                nodeCostText.text = baseCostText + tier1Cost + endText;
                activeCost = tier1Cost;
                break;

            case NodeNames.EarthGift:
                nodeTitleText.text = "Earth Gift";
                nodeDescriptionText.text = "50% chance to obtain one plant pot each morning." +
                    "\n\nPlant pots are found by clearing debris. They can be used to grow seashells into sea flowers.";
                nodeCostText.text = baseCostText + tier1Cost + endText;
                activeCost = tier1Cost;
                break;

            case NodeNames.OceanGift:
                nodeTitleText.text = "Ocean Gift";
                nodeDescriptionText.text = "50% chance to obtain one seashell each morning." +
                    "\n\nSeashells are found by combing the beach. They can be planted in pots to grow sea flowers.";
                nodeCostText.text = baseCostText + tier1Cost + endText;
                activeCost = tier1Cost;
                break;

            case NodeNames.BloomingFlowers:
                nodeTitleText.text = "Blooming Flowers";
                nodeDescriptionText.text = "Obtain twice as many sea flowers from planting.";
                nodeCostText.text = baseCostText + tier2Cost + endText;
                activeCost = tier2Cost;
                break;

            case NodeNames.MatchingBottles:
                nodeTitleText.text = "Matching Bottles";
                nodeDescriptionText.text = "Obtain twice as many bottles from beachcombing and clearing debris.";
                nodeCostText.text = baseCostText + tier2Cost + endText;
                activeCost = tier2Cost;
                break;

            case NodeNames.SellingSeashells:
                nodeTitleText.text = "Selling Seashells";
                nodeDescriptionText.text = "Obtain twice as many seashells from beachcombing.";
                nodeCostText.text = baseCostText + tier2Cost + endText;
                activeCost = tier2Cost;
                break;

            case NodeNames.FillingPots:
                nodeTitleText.text = "Filling Pots";
                nodeDescriptionText.text = "Obtain twice as many plant pots from clearing debris.";
                nodeCostText.text = baseCostText + tier2Cost + endText;
                activeCost = tier2Cost;
                break;

            case NodeNames.Embraced:
                nodeTitleText.text = "Embraced";
                nodeDescriptionText.text = "With 80+ Mental Light, gain double devotion when praying." +
                    "\n\nThe Sun rewards one who maintains a bright and clear mind.";
                nodeCostText.text = baseCostText + tier3Cost + endText;
                activeCost = tier3Cost;
                break;

            case NodeNames.Unbound:
                nodeTitleText.text = "Unbound";
                nodeDescriptionText.text = "With 80+ Health, gain no devotion each morning." +
                    "\n\nWithin the clarity of Darkness, one prioritizes oneself over unneccessary piety.";
                nodeCostText.text = baseCostText + tier3Cost + endText;
                activeCost = tier3Cost;
                break;

            case NodeNames.Enduring:
                nodeTitleText.text = "Enduring";
                nodeDescriptionText.text = "With 80+ Devotion, Health cannot be reduced." +
                    "\n\nThe infinite of the Stars dwarfs the troubles of the self.";
                nodeCostText.text = baseCostText + tier3Cost + endText;
                activeCost = tier3Cost;
                break;

            case NodeNames.Flowing:
                nodeTitleText.text = "Flowing";
                nodeDescriptionText.text = "With 20 or less Mental Light, gain 10 extra Devotion each morning." +
                    "\n\nThe subdued light of the Moon grants one new conviction.";
                nodeCostText.text = baseCostText + tier3Cost + endText;
                activeCost = tier3Cost;
                break;

            case NodeNames.SunlightScion:
                nodeTitleText.text = "Sunlight Scion";
                nodeDescriptionText.text = "Pledge yourself to the Sun." +
                    "\n\nRequirements:" +
                    "\n100 Devotion" +
                    "\n100 Mental Light" +
                    "\n20 Bottled Sunlight";
                nodeCostText.text = baseCostText + finalCost + endText;
                activeCost = finalCost;
                break;

            case NodeNames.DarknessDisciple:
                nodeTitleText.text = "Darkness Disciple";
                nodeDescriptionText.text = "Pledge yourself to Darkness." +
                    "\n\nRequirements:" +
                    "\n100 Health" +
                    "\n0 Devotion" +
                    "\n20 Sea Flowers";
                nodeCostText.text = baseCostText + finalCostDarkness + endText;
                activeCost = finalCostDarkness;
                break;

            case NodeNames.StarcladServant:
                nodeTitleText.text = "Starclad Servant";
                nodeDescriptionText.text = "Pledge yourself to the Stars." +
                    "\n\nRequirements:" +
                    "\n100 Health" +
                    "\n100 Devotion" +
                    "\n20 Plant Pots";
                nodeCostText.text = baseCostText + finalCost + endText;
                activeCost = finalCost;
                break;

            case NodeNames.MoonlightAcolyte:
                nodeTitleText.text = "Moonlight Acolyte";
                nodeDescriptionText.text = "Pledge yourself to the Moon." +
                    "\n\nRequirements:" +
                    "\n100 Devotion" +
                    "\n20 or less Mental Light" +
                    "\n20 Seashells";
                nodeCostText.text = baseCostText + finalCost + endText;
                activeCost = finalCost;
                break;
        }

        //Set purchase button text if node has already been purchased
        if (activeNode.purchased)
        {
            nodeCostText.text = "Received";
        }


        foreach (Node node in nodes)
        {
            node.active = false;
        }
        activeNode.active = true;
        //Visually update all nodes to match new states
        foreach (Node node in nodes)
        {
            node.UpdateAppearance();
        }
    }

    /// <summary>
    /// Attempt to purchase the currently selected node. Costs resources. Nodes can only be purchased once.
    /// </summary>
    public void PurchaseNode()
    {
        if(!activeNode.purchased && gameManager.devotion >= activeCost)
        {
            bool success = true;

            //Specific cases for final nodes (these have unique costs)

            //Sunlight Scion
            if (activeNode == nodes[(int)NodeNames.SunlightScion])
            {
                if (success = gameManager.mentalLight >= 100 && gameManager.bottledSunlight >= 20)
                {
                    gameManager.bottledSunlight -= 20;
                }
            }
            //Darkness Disciple
            else if (activeNode == nodes[(int)NodeNames.DarknessDisciple])
            {
                if (success = gameManager.devotion == 0 && gameManager.seaFlowers >= 20)
                {
                    gameManager.seaFlowers -= 20;
                }
            }
            //Starclad Servant
            else if (activeNode == nodes[(int)NodeNames.StarcladServant])
            {
                if (success = gameManager.health >= 100 && gameManager.plantPots >= 20)
                {
                    gameManager.plantPots -= 20;
                }
            }
            //Moonlight Acolyte
            else if (activeNode == nodes[(int)NodeNames.MoonlightAcolyte])
            {
                if (success = gameManager.mentalLight <= 20 && gameManager.seashells >= 20)
                {
                    gameManager.seashells -= 20;
                }
            }

            //If requirements are met
            if (success)
            {
                gameManager.devotion -= activeCost;
                activeNode.purchased = true;
                ShowUnlockedNodes();
                nodeCostText.text = "Received";
            }
            else
            {
                Debug.Log("Unmet Requirements.");
            }
            
        }
        else
        {
            Debug.Log("Could not purchase.");
        }
    }
}
