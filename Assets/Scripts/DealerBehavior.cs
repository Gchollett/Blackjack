using System.Collections;
using UnityEngine;

public class DealerBehavior : MonoBehaviour
{
    public GameObject PlayerHand;
    public GameManager gm;
    public DeckScript Deck;
    public DialogueManager dm;
    public bool playEnded{get;set;}
    
    private string[] hittingWhileCloseSentences = {"I guess I can hit again","Might as well take another","One more can't hurt"};
    private string[] hittingWhileFarSentences = {"It would be stupid to not hit","Of course I'm gonna hit","Hitting!!!"};
    private string[] stoppingLosingSentences = {"I think I'm going to stop here","WHY COULDN'T I JUST...","Guess that's my turn"};
    private string[] stoppingWinningSentences = {"I'm feeling good about this one!","Looking too good to hit again","Looks like the deck was stacked in my favor"};

    void Awake()
    {
        playEnded = false;
    }
    public void MakePlay(){
        if(!playEnded && dm.ended){
            if(gm.CheckHand(gameObject,true) < 17 && gm.CheckHand(gameObject,true) < gm.CheckHand(PlayerHand)){
                StartCoroutine(GoGo());
            }else if(gm.CheckHand(gameObject,true) > 21){
                dm.StartDialogue(new string[] {"Looks like...","I...","BuStEd"});
                playEnded = true;
            } 
            else if(gm.CheckHand(gameObject,true) < gm.CheckHand(PlayerHand)) {
                dm.StartDialogue(new string[] {stoppingLosingSentences[Random.Range(0,stoppingLosingSentences.Length)]});
                playEnded = true;
            }
            else {
                dm.StartDialogue(new string[] {stoppingWinningSentences[Random.Range(0,stoppingWinningSentences.Length)]});
                playEnded = true;
            }
        }
    }

    IEnumerator GoGo()
    {
        int value = gm.CheckHand(gameObject,true);
        if(value < 8) dm.StartDialogue(new string[] {hittingWhileFarSentences[Random.Range(0,hittingWhileFarSentences.Length)]},false);
        else if(value < 17) dm.StartDialogue(new string[] {hittingWhileCloseSentences[Random.Range(0,hittingWhileCloseSentences.Length)]},false);
        yield return new WaitForSeconds(2);
        dm.DisplayNextSentence();
        gm.DealCardTo(gameObject,(gameObject.transform.childCount-2)*40+75,gameObject.transform.childCount+1);
        yield return new WaitForSeconds(2);
    }
}
