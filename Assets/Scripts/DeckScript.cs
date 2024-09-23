using UnityEngine;

public class DeckScript : MonoBehaviour
{

    public static DeckScript Instance;
    public Sprite[] cards;
    public Sprite hitFist;
    public Sprite pointerHand;
    private bool hitting = false;
    private int curr = 0;
    void Awake()
    {
        Instance = this;
    }
    public Sprite Deal()
    {
        if(curr >= cards.Length) return null;
        Sprite ret = cards[curr];
        curr++;
        return ret;
    }
    public void Shuffle()
    {
        System.Random random = new System.Random();
        for(int i = 0; i < cards.Length; i++){
            int j = random.Next(i,cards.Length);
            Sprite temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
        curr = 0;
    }

    public void SwitchHand()
    {
        hitting = !hitting;
        SpriteRenderer handRenderer = transform.GetChild(3).GetComponent<SpriteRenderer>();
        if(hitting) handRenderer.sprite = hitFist;
        else handRenderer.sprite = pointerHand;
    }
}
