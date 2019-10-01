using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Utils
{
    public static GameObject FindGameObject(string path, string callingClass)
    {
        GameObject go = GameObject.Find(path);
        if (!go)
            throw new CustomExceptons.InvalidGameObjectNameException(callingClass + " could not get a reference to " + path);
        return go;
    }
}
