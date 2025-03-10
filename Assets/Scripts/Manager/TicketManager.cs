using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TicketManager : MonoBehaviour
{
    public static TicketManager Instance { get; private set;}
    public List<string> tickets = new List<string>(); //list of active tickets on ordersPanel
    public GameObject ordersPanel; //full UI of the pop up
    public GameObject orderTicket; //individual ticket prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetUpOrdersPanel();
    }

    void Update()
    {
        if (ordersPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    HideOrders();
                }
            }
        }
    } 

    public void AddOrderToPanel(Order order)
    {
        // Create ticket for order panel
        GameObject ticket = Instantiate(orderTicket);

        // Set ticket's name and add it to tickets list
        ticket.name = order.ghostName + tickets.Count;
        tickets.Add(ticket.name);

        // Set ticket's parent
        ticket.transform.SetParent(ordersPanel.transform.GetChild(0).GetChild(0));

        // Set width and height of ticket
        ticket.transform.localScale = new Vector3(0.5f, 2.0f, 1.0f);

        // Set ticket to active
        ticket.SetActive(true);

        // Put order stuff on ticket
        TextMeshProUGUI[] texts = ticket.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = order.ghostName;
        texts[1].text = "Order: " + order.recipeName;
        texts[2].text = "Reward: " + order.price.ToString();

        // Set button functionality
        ticket.GetComponentInChildren<Button>().onClick.AddListener(() => GameManager.Instance.orderManager.MakeOrder(tickets.Count - 1));
    }

    public void ShowOrders()
    {
        if (GameManager.Instance.orderManager.activeOrders.Count > 0 && !ordersPanel.activeSelf)
        {
            // Reset visible tickets
            SetUpOrdersPanel();

            // Reset tickets
            tickets.Clear();

            // Add each order to panel
            foreach (Order order in GameManager.Instance.orderManager.activeOrders)
            {
                AddOrderToPanel(order);
            }

            // Show the orders panel
            ordersPanel.SetActive(true);
        }
    }

    public void HideOrders()
    {
        ordersPanel.SetActive(false);
    }

    //loads in assets for ordersPanel
    public void SetUpOrdersPanel()
    {
        // Get rid of old tickets
        while (ordersPanel.transform.GetChild(0).GetChild(0).childCount > 1)
        {
            DestroyImmediate(ordersPanel.transform.GetChild(0).GetChild(0).GetChild(1).gameObject);
        }

        // Set the order panel to inactive
        ordersPanel.SetActive(false);
        orderTicket.SetActive(false);
    }
}
