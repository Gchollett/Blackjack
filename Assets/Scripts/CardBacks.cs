using UnityEngine;

public class CardBacks : MonoBehaviour
{

    public Sprite[] backs;
    private MainManager mm = MainManager.Instance;
    public MainManager mainManagerBackup;
    private int backIndex = 0;

    void Start()
    {
        if(mm == null){
            mm = mainManagerBackup;
        }
    }
    void FixedUpdate() {
        gameObject.GetComponent<SpriteRenderer>().sprite = backs[backIndex];
    }

    public void NextBack(){
        backIndex = (backIndex+1)%backs.Length;
    }

    public void PrevBack(){
        backIndex = (backIndex <= 0)?backs.Length-1:backIndex-1;
    }

    public void SaveBack(){
        mm.cardBack = backs[backIndex];
    }
}
