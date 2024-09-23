using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Hand;
    public GameObject DealerHand;
    public GameObject MoneyPot;
    public DeckScript Deck;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI differenceText;
    public GameObject deck;
    public GameObject chipBoard;
    public DialogueManager dm;
    public MoneyManager mm;
    public MoneyManager cmm;
    public GameObject moneyPad;
    public GameObject faceDownCard;
    public DealerBehavior dealer;
    private int moneyPool = 0;
    public bool playerTurn{get;set;}
    public bool betting{get;set;}
    private bool lost = false;
    private bool roundOver = false;
    private bool stop = false;
    private bool busted = false;
    private Animator chipAnimator;
    private Animator moneyAnimator;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        dm.StartDialogue(new string[] {"Welcome to Dony's Casino!","I hope you lose... I mean win a lot today.","Start by placing a bet..."});
        betting = true;
        playerTurn = true;
        chipAnimator = chipBoard.GetComponent<Animator>();
        moneyAnimator = moneyPad.GetComponent<Animator>();
        
    }
    void FixedUpdate()
    {
        HandleText();
        HandlePlayerActive();
        HandleBetting();
        HandleDealerTurn();
        HandleLosing();
        HandleWinning();
    }

    void HandleDealerTurn()
    {
        StartCoroutine(something());
    }

    IEnumerator something(){
        if(!playerTurn && dm.ended && !roundOver){
            yield return new WaitForSeconds(1);
            dealer.MakePlay();
            roundOver = dealer.playEnded;
        }
    }

    public void Stand()
    {
        DealerHand.transform.GetChild(1).gameObject.SetActive(true);
        playerTurn = false;
    }

    void HandleText()
    {
        differenceText.color = (mm.startingAmount-mm.amount<0)?Color.green:Color.red;
        if(mm.startingAmount-mm.amount==0) differenceText.color = Color.black;
        differenceText.text = ((mm.startingAmount-mm.amount<0)?"+$":"-$")+math.abs(mm.startingAmount-mm.amount);
        balanceText.text = "$"+mm.amount;
    }

    void HandlePlayerActive(){
        if(!playerTurn||betting||!dm.ended){
            deck.GetComponent<Button>().enabled = false;
            if(!deck.transform.GetChild(2).GetComponent<Animator>().GetBool("Hovering")){
                deck.GetComponent<EventTrigger>().enabled = false;
            }else{
                deck.transform.GetChild(2).GetComponent<Animator>().SetBool("Hovering",false);
            }
            deck.transform.GetChild(3).gameObject.SetActive(false);
        }else{
            deck.GetComponent<Button>().enabled = true;
            deck.GetComponent<EventTrigger>().enabled = true;

        }
    }

    void HandleBetting()
    {
        if(betting&&dm.ended){
            for(int i = 0; i < 7;i++){
                chipBoard.transform.GetChild(i).GetComponent<Button>().enabled = true;
            }
            chipBoard.transform.GetChild(8).gameObject.SetActive(true);
            chipBoard.transform.GetChild(9).gameObject.SetActive(true);
            chipBoard.transform.GetChild(10).gameObject.SetActive(true);
        }else{
            for(int i = 0; i < 7;i++){
                chipBoard.transform.GetChild(i).GetComponent<Button>().enabled = false;
            }
            chipBoard.transform.GetChild(8).gameObject.SetActive(false);
            chipBoard.transform.GetChild(9).gameObject.SetActive(false);
            chipBoard.transform.GetChild(10).gameObject.SetActive(false);
        }
        chipAnimator.SetBool("IsBetting",betting&&dm.ended);
        moneyAnimator.SetBool("IsBetting",betting&&dm.ended);
    }



    public void PlaceBet(){
        int currentBet = mm.PlaceBet();
        if(currentBet == 0){dm.StartDialogue(new string[] {"Hey, we're not playing for fun.","Put a little cash on the line."}); return;}
        if(currentBet > cmm.amount){
            cmm.AllIn();
        }else{
            cmm.Bet(currentBet);
        }
        moneyPool = currentBet + cmm.PlaceBet();
        betting = false;
    }

    void HandleLosing(){
        if(stop || !dm.ended) return;
        if(!lost && CheckHand(Hand) > 21 && !busted){
            StartCoroutine(DialougeThen(new string[] {"Looks like you busted","Ha Ha Ha..."},() => {lost = true;}));
            busted = true;
        }
        if(lost && mm.amount == 0){
            cmm.Gain(moneyPool);
            StartCoroutine(EndGame(new string[] {"Looks like you're all out of cash", "Dony might give me a raise for saving his casino","Anyways, goodbye mister money bags...","Whoops, forgot I took it all"}));
            stop = true;
            
        }else if(lost && mm.amount > 0){
            cmm.Gain(moneyPool);
            StartCoroutine(DialougeThen(new string[] {"Welp, Looks like you lost","Your Chips are now mine","Another Game?","I knew it was going to be a yes!","Place your bet and let's go"},ResetGame));
            stop = true;
        }
        lost = roundOver && CheckHand(Hand) <= CheckHand(DealerHand,true) && CheckHand(DealerHand,true) <= 21;
    }

    IEnumerator DialougeThen(string[] sentences, Action action,int seconds = 4){
        dm.StartDialogue(sentences);
        yield return new WaitUntil(() => dm.ended);
        action();
    }
    IEnumerator EndGame(string[] sentences){
        dm.StartDialogue(sentences);
        yield return new WaitUntil(() => dm.ended);
        SceneManager.LoadScene(0);
    }

    void HandleWinning(){
        if(stop) return;
        lost = roundOver && CheckHand(Hand) <= CheckHand(DealerHand,true) && CheckHand(DealerHand,true) <= 21;
        if(!lost && roundOver && cmm.amount == 0){
            mm.Gain(moneyPool);
            StartCoroutine(EndGame(new string[] {"No...","This can't be","You...","You took all of the money","Dony's Casino is for sure going bankrupt now","And...","I'm going to get fired","Can you please give a poor man some money..."}));
            stop = true;
        }
        if(!lost && roundOver && cmm.amount > 0){
            mm.Gain(moneyPool);
            StartCoroutine(DialougeThen(new string[] {"Welp, you can't win them all I guess","Good thing we're gonna keep going","More money to lose... I mean win"},ResetGame));
            stop = true;
        }
    }

    void ResetGame(){
        Deck.SwitchHand();
        for(int i=0; i < Hand.transform.childCount; i++){
            Destroy(Hand.transform.GetChild(i).gameObject);
        }
        for(int i=1; i < DealerHand.transform.childCount; i++){
            Destroy(DealerHand.transform.GetChild(i).gameObject);
        }
        playerTurn = true; 
        roundOver = false; 
        betting = true;
        lost = false;
        stop = false;
        moneyPool = 0;
        busted = false;
        dealer.playEnded = false;
        DealerHand.transform.GetChild(0).gameObject.SetActive(false);
        
    }

    public void DealCardTo(GameObject hand,int offset,int sortingOrder=1,bool facedown=false){
        Sprite card_val = Deck.Deal();
        GameObject card = new GameObject(card_val.name);
        card.transform.parent = hand.transform;
        card.AddComponent<SpriteRenderer>().sprite = card_val;
        card.transform.localScale = new Vector3(200,200,1);
        card.transform.localPosition = new Vector3(offset,0,1);
        card.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        card.SetActive(!facedown);
    }

    public void DealCards_Hit()
    {   
        if(betting || !playerTurn) return;
        if(Hand.transform.childCount > 0){
            DealCardTo(Hand,(Hand.transform.childCount-1)*40,Hand.transform.childCount+1);
        }else{
            Deck.SwitchHand();
            Deck.Shuffle();
            faceDownCard.GetComponent<Image>().sprite = deck.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            faceDownCard.SetActive(true);
            for(int i = 1; i <= 2; i++){
                DealCardTo(Hand,(i==1)?-40:0,i);
                DealCardTo(DealerHand,(i==1)?-75:75,i,i==1);
            }
        }
    }

    public int CheckHand(GameObject hand,bool ignoreFirst=false)
    {
        int ret = 0;
        int num;
        int aceCount = 0;
        for(int i = ignoreFirst?1:0; i < hand.transform.childCount; i++)
        {
            num = int.Parse(Regex.Match(hand.transform.GetChild(i).name,@"\d+").Value);
            if(num > 10) num = 10;
            if(num == 1) aceCount++;
            else ret += num;
        }
        if(aceCount > 0 && ret + 11 + aceCount-1 <= 21){
            ret += 11 + aceCount-1;
        }else{
            ret += aceCount;
        }
        return ret;
    }

    
}
