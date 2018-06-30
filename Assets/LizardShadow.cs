using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardShadow : MonoBehaviour {

	MeshRenderer renderer;
	Vector3 parentPos;
	Mesh mesh;
	MeshFilter filter;
	float offset;


	// Use this for initialization
	void Start () {

		renderer = GetComponent<MeshRenderer>();
		filter = GetComponent<MeshFilter>();

		renderer.enabled = true;

		offset = (float) 0.2;

		parentPos = GetComponentInParent<Lizard>().transform.position;

		transform.position = new Vector3(parentPos.x + offset, parentPos.y + offset, parentPos.z);

		

		
	}
	
	// Update is called once per frame
	void Update () {

		renderer.enabled = GetComponentInParent<Lizard>().GetState() == 2;

		parentPos = GetComponentInParent<Lizard>().transform.position;

		transform.position = new Vector3(parentPos.x + offset, parentPos.y + offset, parentPos.z);

		filter.mesh = GetComponentInParent<Lizard>().mesh;
	}
}
