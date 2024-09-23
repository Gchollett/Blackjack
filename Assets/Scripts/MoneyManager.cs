using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int startingAmount;
    public int amount{get;set;}
    private int currentBet = 0;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        amount = startingAmount;
    }

    public void Bet(int bet){
        if(bet > amount) return;
        amount -= bet;
        currentBet += bet;
    }

    public void Gain(int winnings){
        amount += winnings;
    }

    public void ResetBet(){
        Gain(currentBet);
        currentBet = 0;
    }

    public int PlaceBet(){
        int ret = currentBet;
        currentBet = 0;
        return ret;
    }

    public void AllIn(){
        Bet(amount);
    }
}
