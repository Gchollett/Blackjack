using UnityEngine;

public class CardBackScript : MonoBehaviour
{
    public Sprite defaultBack;
    private static MainManager mm = MainManager.Instance;

    public Sprite back{get;set;}
    // Start is called before the first frame update
    void Start()
    {
        if(mm != null && mm.cardBack != null){
            back = mm.cardBack;
        }else{
            back = defaultBack;
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = back;
    }
    public Sprite GetCardBack(){return back;}
}
