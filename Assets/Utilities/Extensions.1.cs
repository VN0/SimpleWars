using UnityEngine;


static public class ExtensionMethods
{
    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    static public T GetOrAddComponent<T> (this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    static public Vector2 Snap2Grid (this Vector2 vector, float distance)
    {
        return new Vector2(
            Mathf.Round(vector.x / distance) * distance,
            Mathf.Round(vector.y / distance) * distance
        );
    }
}