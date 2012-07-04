using System.Globalization;
using UnityEngine;
using System.Collections;

public class DoorInvoker : MonoBehaviour
{
    public int DoorId = 0;

    public void Toggle()
    {
        Debug.Log("Showing... "+DoorId.ToString(CultureInfo.InvariantCulture));
    }


}
