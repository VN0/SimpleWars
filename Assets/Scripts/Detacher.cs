using UnityEngine;

public class Detacher : PartFunction
{
	void Update () {
        try {
			Destroy (GetComponent<AnchoredJoint2D> ());
		} catch {}
        enabled = false;
	}
}
