using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawCardWithCurrency : MonoBehaviour
{

    [SerializeField] private GameObject DrawUI;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private int drawCost;
    [SerializeField] private int costIncrement;

    private int cardsDrawnWithCurrency = 0;


    private void OnMouseEnter()
    {
        DrawUI.SetActive(true);
        //Tilt first FakeCard
    }

    private void OnMouseExit()
    {
        DrawUI.SetActive(false);
        //Untilt first FakeCard
    }

    private void OnMouseDown()
    {
        //Draw Card
        //Add Random Card to Hand
        //Substract Player's Currency 
        cardsDrawnWithCurrency++;
        drawCost += costIncrement;
        costText.text = drawCost.ToString();
    }


    // Start is called before the first frame update
    private void Awake()
    {
        DrawUI.SetActive(false);
        
    }

    void Start()
    {
        costText.text = drawCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
