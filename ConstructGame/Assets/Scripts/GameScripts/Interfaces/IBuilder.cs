using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilder
{
    void setMode(BuildMode mode);
    void setTurret(TurretTypes type);
}

public enum BuildMode
{
    Wall,
    Turrets,
    last
}