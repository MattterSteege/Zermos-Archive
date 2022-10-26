using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public BackgroundManager Instance;
    
    public Sprite[] backgrounds;
    public Image backgroundComponent;
    
    public void setBackGround(int index)
    {
        backgroundComponent.sprite = backgrounds[index - 1];
    }
    
    void start ()
    {
        Instance = this;
        PlayerPrefs.GetInt("minutesbeforeclass");
        setBackGround(PlayerPrefs.GetInt("minutesbeforeclass"));
    }
}
