using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSidemenuViewManager : ViewManager
{
    public Sidemenu TopMenu;
    public Sidemenu CenterMenu;
    public Sidemenu LeftMenu;
    public Sidemenu RightMenu;

    private bool isTopMenuVisible = false;
    private bool isBottomMenuVisible = false;
    private bool isLeftMenuVisible = false;
    private bool isRightMenuVisible = false;

    public void ToggleTopMenu()
    {
        isTopMenuVisible = !TopMenu.IsOpen;
        ToggleSidemenu(TopMenu, isTopMenuVisible);
    }

    public void ToggleCenterMenu()
    {
        isBottomMenuVisible = !CenterMenu.IsOpen;
        ToggleSidemenu(CenterMenu, isBottomMenuVisible);
    }

    public void ToggleLeftMenu()
    {
        isLeftMenuVisible = !LeftMenu.IsOpen;
        ToggleSidemenu(LeftMenu, isLeftMenuVisible);
    }

    public void ToggleRightMenu()
    {
        isRightMenuVisible = !RightMenu.IsOpen;
        ToggleSidemenu(RightMenu, isRightMenuVisible);
    }

    private void ToggleSidemenu(Sidemenu sidemenu, bool isShow)
    {
        sidemenu.ToggleSideMenu(isShow);
    }
}
