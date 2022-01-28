using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGJ2022.Grid
{
    public enum GridTraversability
    {
        debug,
        easy,
        medium,
        hard
    }
    public enum GridType
    {
        difficult,
        traversable,
        easy
    }
    public enum GridHostility
    {
        hostile,
        dangerous,
        safe
    }
    public enum GridOwner
    {
        player,
        enemy,
        neutral,
        unknown
    }
}