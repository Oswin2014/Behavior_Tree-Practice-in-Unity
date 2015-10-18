using UnityEngine;
using System.Collections;

public class GenericHelper
{

    /// <summary>
    /// 查找子节点
    /// </summary>
    public static GameObject FindChildByName(GameObject go, string childname, bool includeInactive = false)
    {
        if (go == null)
            return null;
        Transform[] trans = go.transform.GetComponentsInChildren<Transform>(includeInactive);
        foreach (Transform t in trans)
        {
            if (t.name.CompareTo(childname) == 0)
            {
                return t.gameObject;
            }
        }

        return null;
    }

    public static LayerMask GetLayerMask(string ln)
    {
        return 1 << LayerMask.NameToLayer(ln);
    }

}
