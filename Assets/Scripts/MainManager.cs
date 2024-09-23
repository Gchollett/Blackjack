using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance
    {
        get;
        set;
    }
    public Sprite cardBack {
        get;
        set;
    }
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
