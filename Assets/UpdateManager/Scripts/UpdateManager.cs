using UnityEngine;

/// <summary>
/// Made by Feiko Joosten
/// 
/// I have based this code on this blogpost. Decided to give it more functionality. http://blogs.unity3d.com/2015/12/23/1k-update-calls/
/// Use this to speed up your performance when you have a lot of update calls in your scene
/// Let the object you want to give increased performance inherit from OverridableMonoBehaviour
/// Replace your void Update() for public abstract void UpdateMe()
/// OverridableMonoBehaviour will add the object to the update manager
/// UpdateManager will handle all of the update calls
/// </summary>

public class UpdateManager : MonoBehaviour
{
    private static UpdateManager instance;

    private int count = 0;
    private OverridableMonoBehaviour[] array;

    public UpdateManager ()
    {
        instance = this;
    }

    public static void AddItem (OverridableMonoBehaviour behaviour)
    {
        instance.AddItemToArray(behaviour);
    }

    public static void RemoveSpecificItem (OverridableMonoBehaviour behaviour)
    {
        instance.RemoveSpecificItemFromArray(behaviour);
    }

    public static void RemoveSpecificItemAndDestroyIt (OverridableMonoBehaviour behaviour)
    {
        instance.RemoveSpecificItemFromArray(behaviour);

        Destroy(behaviour.gameObject);
    }

    private void AddItemToArray (OverridableMonoBehaviour behaviour)
    {
        if (array == null)
        {
            array = new OverridableMonoBehaviour[1];
        }
        else
        {
            System.Array.Resize(ref array, array.Length + 1);
        }
        array[array.Length - 1] = behaviour;
        count = array.Length;
    }

    private void RemoveSpecificItemFromArray (OverridableMonoBehaviour behaviour)
    {
        int addAt = 0;
        OverridableMonoBehaviour[] tempArray = new OverridableMonoBehaviour[array.Length - 1];

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                continue;
            }
            else if (array[i] == behaviour)
            {
                continue;
            }
            tempArray[addAt] = array[i];
            addAt++;
        }

        array = new OverridableMonoBehaviour[tempArray.Length];

        for (int i = 0; i < tempArray.Length; i++)
        {
            array[i] = tempArray[i];
        }

        count = array.Length;
    }

    private void Update ()
    {
        if (count > 0)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    continue;
                }
                array[i].UpdateMe();
            }
        }
    }

    private void FixedUpdate ()
    {
        if (count > 0)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    continue;
                }
                array[i].FixedUpdateMe();
            }
        }
    }
}











