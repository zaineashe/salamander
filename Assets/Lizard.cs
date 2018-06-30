using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Lizard : MonoBehaviour {

	public Mesh mesh;
	MeshFilter filter;
	MeshRenderer renderer;
	
	private double headSize;
	private double direction;
	private double speed;
	private double maxSpeed;
	private double targetSpeed;
	private double dirSpeed;
	private int state;

	ArrayList body;
	int bodyLength;


	// Use this for initialization
	void Start () {

		state = 0;
		mesh = new Mesh();

		filter = GetComponent<MeshFilter>();
		renderer = GetComponent<MeshRenderer>();
		renderer.enabled = true;

		body = new ArrayList();

		double[] sizes = new double[] { 0.7, 1.3, 1.4, 1.6, 1.3, 1.3, 0.9, 0.7, 0.4, 0.4 };

		Body previousBody = null;

		for (int i =0; i<sizes.Length; i++) {

			previousBody = new Body(sizes[i], transform.position, this, previousBody);

			if (i == 1 || i == 5) { 
				ArrayList legs = new ArrayList();
				legs.Add(new Leg(previousBody, 0.6, 7 + (i == 5 ? 3 : 0), false, true));
				legs.Add(new Leg(previousBody, 0.6, 7 + (i == 5 ? 3 : 0), true, false));
				previousBody.SetLegs(legs);
			}
			
			body.Add(previousBody);
		}

		transform.position = new Vector3(30,0,0);
		headSize = 1;
		speed = 0;
		maxSpeed = 0.3;
		direction = 0;
		targetSpeed = 0;
		dirSpeed = 10;
	}

	private Vector3 NewPos(double magnitude, double dir) {
		return new Vector3(transform.position.x + (float)Lib.CosX(magnitude, dir),
					transform.position.y + (float)Lib.SinY(magnitude, dir), 0);
	}

	private void MoveHead(bool up, bool left, bool right) {
		if (up) {
			targetSpeed = maxSpeed;
			direction += dirSpeed * ((left ? speed : 0)
									- (right ? speed : 0));
			direction = direction % 360;
		}
		else {
			targetSpeed = 0;
		}

		speed += (targetSpeed - speed) / 10;

		transform.position = NewPos(speed, direction);
	}

	public double GetDirection() {
		return direction;
	}

	public double GetSpeed() {
		return speed;
	}

	public ArrayList CreateQuad(ArrayList triangles, ArrayList vertices, int i, int boundUp) {

		if ((i + 2) < boundUp) {
			triangles.Add(i + 2);
			triangles.Add(i);
			triangles.Add(i + 1);
		}
		if ((i + 3 < boundUp)) {
			triangles.Add(i + 2);
			triangles.Add(i + 1);
			triangles.Add(i + 3);
		}

		return triangles;
	}

	public ArrayList FinalizeCoords() {
		ArrayList coords = new ArrayList();

		for (int i = 0; i < body.Count; i++) {
			((Body)body[i]).UpdateBody();
			coords = ((Body)body[i]).GetVectors(coords);
		}

		bodyLength = coords.Count;

		for (int i = 0; i < body.Count; i++) {
			coords = ((Body)body[i]).GetLegCoords(coords, true);
			coords = ((Body)body[i]).GetLegCoords(coords, false);
		}
		return coords;
	}

	public int GetState() {
		return state;
	}

	private void ChangeState(bool keyPress) {
		if (keyPress) {
			state += 1;
			state = state % 3;
		}
	}
	
	// Update is called once per frame
	void Update () {

		MoveHead(Input.GetKey(KeyCode.UpArrow),
			Input.GetKey(KeyCode.LeftArrow),
			Input.GetKey(KeyCode.RightArrow));

		ChangeState(Input.GetKeyDown(KeyCode.Space));

		ArrayList coords = FinalizeCoords();

		Vector3[] vertices = new Vector3[coords.Count];
		Vector3[] normals = new Vector3[coords.Count];
		Vector2[] uvs = new Vector2[coords.Count];

		//triangulation
		ArrayList tris = new ArrayList();

		int boundUp = bodyLength;

		for (int i = 0; i<coords.Count; i++) {
			Vector3 vect = (Vector3)coords[i];
			vertices[i] = new Vector3(vect.x - transform.position.x, vect.y - transform.position.y, 0);
			normals[i] = Vector3.down;

			uvs[i] = new Vector2(vertices[i].x,vertices[i].y);

			if (i >= bodyLength) {
				if ((i - bodyLength) % 8 == 0) {
					if (boundUp + 8 > coords.Count) {
						boundUp = coords.Count;
					} else {
						boundUp += 8;
					}
				}
				tris = CreateQuad(tris, coords, i, boundUp);
			} else if (i < bodyLength && i % 2 == 0) {
				tris = CreateQuad(tris, coords, i, boundUp);
			}
		}

		int[] triangles = new int[tris.Count];

		for (int i = 0; i < tris.Count; i++) {
			triangles[i] = (int)tris[i];
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.RecalculateBounds();

		filter.mesh = mesh;

		renderer.enabled = state == 2;

		if (state == 1) {
			VectorFactory.DrawTriangles(transform.position,triangles, vertices);
		}
	}
}
