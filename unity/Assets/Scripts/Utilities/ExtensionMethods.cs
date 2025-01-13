using UnityEngine;

/// <summary>
/// will probably be used in the future
/// </summary>
public static class ExtensionMethods
{
    public static Vector3 GetWithNewX(this Vector3 v, float x)
    {
        //works - but return value has to be assigned to this... Vector since it doesnt operate on the reference!
        //v.x = x; //somehow doesnt work, even though it has no error
        //return v;
        Vector3 temp = new Vector3(x, v.y, v.z);
        return temp;
    }
}