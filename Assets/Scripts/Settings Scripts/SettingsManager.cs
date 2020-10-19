using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    GameObject NicknameDialog;

    public string Nickname { get; private set; }

    public static SettingsManager Instance;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        Nickname = PlayerPrefs.GetString("Nickname");

        if (string.IsNullOrEmpty(Nickname))
            NicknameDialog.SetActive(true);
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(Nickname))
            Nickname = "xX2Kool4Nickname" + Random.Range(0, 1000) + "Xx";
        
        PlayerPrefs.SetString("Nickname", Nickname);

        PlayerPrefs.Save();

        if (NicknameDialog.activeInHierarchy)
            NicknameDialog.SetActive(false);
    }

    public void SetNickname(string nickname)
    {
        Nickname = nickname;
    }
}
