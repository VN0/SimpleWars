using UnityEngine;

public class OverridableMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake ()
    {
        UpdateManager.AddItem(this);
    }

    /// <summary>
    /// If your class uses the Awake function, please use  protected override void Awake() instead.
    /// Also don't forget to call OverridableMonoBehaviour.Awake(); first.
    /// If your class does not use the Awake function, this object will be added to the UpdateManager automatically.
    /// Do not forget to replace your Update function with public override void UpdateMe()
    /// </summary>
    public virtual void UpdateMe () { }

    public virtual void FixedUpdateMe () { }
}
