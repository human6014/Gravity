using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Setting
{
    public abstract void LoadDefault();
    public abstract void LoadData();
    public abstract void SaveData();
}
