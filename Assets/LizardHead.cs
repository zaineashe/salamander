using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardHead : MonoBehaviour {

	Lizard parent;
	SpriteRenderer renderer;

	private void Start() {
		renderer = GetComponent<SpriteRenderer>();
		renderer.enabled = true;
	}
	// Update is called once per frame
	void Update () {



		parent = GetComponentInParent<Lizard>();
		transform.position = parent.transform.position;

		double angle = parent.GetDirection();

		Quaternion quat = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,(float)angle),1);

		transform.rotation = quat;

		renderer.enabled = (GetComponentInParent<Lizard>().GetState() == 2);
	}
}
