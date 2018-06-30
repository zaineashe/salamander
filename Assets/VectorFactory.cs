using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFactory : MonoBehaviour {

    public Material material;
    private static ArrayList queue;

    void Start () {
        queue = new ArrayList();
        material = GetComponent<VectorFactory>().material;

    }

	private bool ValidateQueue() {
		return (queue != null);
	}

    private bool ValidateVector(Vector3[] coords) {
        return (coords != null);
    }

    private void OnPostRender()
    {
        DrawQueue();
        queue.Clear();
    }

    public static void DrawLine(Vector3 point1, Vector3 point2, Color color) {
		DrawPoly(new Vector3[] {point1, point2}, color);
    }

	public static void DrawIKLine(Vector3 point1, Vector3 point2, double length, bool outwardJoint, Color color) {

		double offset = Lib.ToDeg(Math.Acos((Lib.GetDist(point1.x, point1.y, point2.x, point2.y) / 2) / (length / 2)));
		offset *= outwardJoint ? -1 : 1;

		double angle =	Lib.GetDir(point1.x, point1.y, point2.x, point2.y) + offset;

		if (Double.IsNaN(angle)) {
			DrawLine(point1, point2, color);
		}
		else {
			Vector3 elbow = new Vector3(point1.x + Lib.CosX(length / 2, angle),
										point1.y + Lib.SinY(length / 2, angle),
										point1.z);

			Vector3[] coords = new Vector3[] { point1, elbow, point2 };

			DrawPoly(coords, color);
		}
	}

	public static Vector3 IKPoint(Vector3 point, double magnitude, double angle) {
		return new Vector3(point.x + Lib.CosX(magnitude, angle), point.y + Lib.SinY(magnitude,angle),point.z);
	}

	public static void DrawTriangles(int[] triangles, Vector3[] vectors) {

		int i = 0;

		while (i < triangles.Length) {
			Vector3[] tri = new Vector3[] {vectors[triangles[i]],vectors[triangles[i+1]],vectors[triangles[i+2]],vectors[triangles[i]]};
			DrawPoly(tri,Color.black);
			i += 3;
		}

	}

	private static Vector3 OffsetVect(Vector3 vector, float xOffset, float yOffset) {

		return new Vector3(vector.x + xOffset,vector.y + yOffset,vector.z);
	}

	public static void DrawTriangles(Vector3 pos, int[] triangles, Vector3[] vectors) {

		int i = 0;


		while (i < triangles.Length) {

			Vector3 origin = OffsetVect(vectors[triangles[i]], pos.x, pos.y);

			Vector3[] tri = new Vector3[] { origin,
											OffsetVect(vectors[triangles[i + 1]], pos.x, pos.y),
											OffsetVect(vectors[triangles[i + 2]], pos.x, pos.y),
											origin};
			DrawPoly(tri, Color.black);
			i += 3;
		}

	}

	public static void DrawCircle(Vector3 origin, double radius, Color color) {

		int polySize = 9;

		Vector3[] coords = new Vector3[polySize + 1];

		for (int i = 0; i < polySize + 1; i++) {

			double angle = (360 / polySize) * i;

			coords[i] = new Vector3(origin.x + (float) Lib.CosX(radius,angle),
									origin.y + (float) Lib.SinY(radius, angle));
		}

		DrawPoly(coords, color);

	}

	public static void DrawPoints(Vector3[] coords, Color color) {
		for (int i = 0; i < coords.Length; i++) {
			DrawCircle(coords[i],0.1,color);
		}
	}

    public static void DrawPoly(Vector3[] coords, Color color) {
		ArrayList poly = new ArrayList();
		poly.Add(coords);
		poly.Add(color);
        queue.Add(poly);
    }

	public static void DrawPoly(ArrayList coords, Color color) {
		ArrayList poly = new ArrayList();
		Vector3[] finalCoords = new Vector3[coords.Count];
		for (int i = 0; i<coords.Count; i++) {
			finalCoords[i] = (Vector3) coords[i];
		}
		poly.Add(finalCoords);
		poly.Add(color);
		queue.Add(poly);
	}

    private void DrawQueue() {

        GL.Begin(GL.LINES);
        material.SetPass(0);

		if (ValidateQueue()) {

			for (int n = 0; n < queue.Count; n++) {

				ArrayList poly = (ArrayList) queue[n];

				Vector3[] coords = (Vector3[]) poly[0];

				GL.Color((Color) poly[1]);

				if (ValidateVector(coords)) {
					for (int i = 0; i < coords.Length; i++) {

						if (i != coords.Length - 1) {
							//draw the line from the current point to the next point
							GL.Vertex(coords[i]);
							GL.Vertex(coords[i + 1]);
						}
					}
				}
			}
		}
		GL.End();
    }
}
