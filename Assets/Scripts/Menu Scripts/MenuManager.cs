using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] TextMeshProUGUI menuTitle;
    [SerializeField] List<Menu> menus;

    public Menu GetCurrentMenu => menus.First(x => x.gameObject.activeInHierarchy);

    private void Awake()
    {
        Instance = this;
        menuTitle.text = GetCurrentMenu.menuName;
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Count; i++)
        {
            if (menus[i].menuName == menuName)
                menus[i].Open();
            else if (menus[i].open)
                CloseMenu(menus[i]);
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Count; i++)
        {
            if (menus[i].open)
                CloseMenu(menus[i]);
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void SetMenuTitle(string title)
    {
        menuTitle.text = title;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
