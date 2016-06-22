using UnityEngine;

public class Explosion : MonoBehaviour {

	public GameObject explosion;
	public float rate = 1f;
	public float size = 10f;
	public float force = 500f;
	public int delay = 10;
	float cRadius = 0f;

	bool exploded = false;
	CircleCollider2D radius;
	PointEffector2D exploder;
    GameObject ex;


	void explode ()
	{
		Destroy (GetComponent<SpriteRenderer>());
		Destroy (GetComponent<BoxCollider2D>());
		Destroy (GetComponent<AnchoredJoint2D>());
		Destroy (GetComponent<Rigidbody2D>());
        Transform tr = gameObject.transform;
        //tr.DetachChildren ();
        for(int i=0;i<gameObject.transform.childCount;i++) {
            tr.GetChild(i).transform.SetParent(tr.parent);
        }
		exploded = true;

		radius = gameObject.AddComponent<CircleCollider2D> ();
		radius.isTrigger = true;
		radius.usedByEffector = true;
		exploder = gameObject.AddComponent<PointEffector2D> ();
		exploder.forceMagnitude = force;
		exploder.forceMode = EffectorForceMode2D.InverseSquared;
		exploder.forceTarget = EffectorSelection2D.Collider;
		ex = Instantiate (explosion, transform.position, Quaternion.identity) as GameObject;
        ex.transform.SetParent(transform);
	}

	void Start ()
	{
		if (rate == 0f)
			rate = size;
	}


	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.E /*|| GetComponent<FixedJoint2D>().reactionForce */)) {
			explode ();
		}
		//print(GetComponent<AnchoredJoint2D> ().reactionForce);
	}


	void FixedUpdate ()
	{
		if (exploded == true) {
			if (cRadius < size) {
				cRadius += rate;
			    radius.radius = cRadius;
			} else if (delay > 0) {
				delay -= 1;
			}
            if (!ex.GetComponent<ParticleSystem>().IsAlive()){
				Destroy(gameObject);
			}
		}
	}
}