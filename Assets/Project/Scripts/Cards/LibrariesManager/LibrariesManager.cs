using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibrariesManager : MonoBehaviour
{
    static private LibrariesManager instance;

    [SerializeField] private CardsLibrary cardsLibrary;
    [SerializeField] private PartsLibrary partsLibrary;
    
    public CardsLibrary CardsLibrary => cardsLibrary;
    public PartsLibrary PartsLibrary => partsLibrary;




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(this);
        }
    }

    static public LibrariesManager GetInstance()
    {
        return instance;
    }

    private void Init()
    {
        
    }


}
