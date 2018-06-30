using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg {

	private double thickness;
	private double length;
	private bool outwardJoint;
	private bool orientation;
	private bool stepping;

	private double legOffset;
	private double stepSpeed;
	private double lastDist;

	private Vector3 position;
	private Body parent = null;

	public Leg(Body parent, double thickness, double length, bool orientation, bool outwardJoint) {
		this.legOffset = 20.0;
		this.parent = parent;
		this.thickness = thickness;
		this.length = length;
		this.orientation = orientation;
		this.outwardJoint = outwardJoint;
		this.stepping = false;
		this.position = new Vector3(parent.GetX() + Lib.CosX(length * (orientation ? -1 : 1), legOffset),
									parent.GetY() + Lib.SinY(length * (orientation ? -1 : 1), legOffset),
									parent.GetZ());
		this.stepSpeed = 0.7;
		this.lastDist = Lib.GetDist(parent.GetX(), parent.GetY(), position.x, position.y);
	}

	private void ToggleStepping() {
		stepping = !stepping;
	}

	public ArrayList GetLegByShoulder(ArrayList coords, Vector3[] shoulder, bool orientation) {
		Vector3 point1 = new Vector3(parent.GetX(), parent.GetY(), 0);
		Vector3 point2 = new Vector3(position.x, position.y, 0);

		double offset = Lib.ToDeg(Math.Acos((Lib.GetDist(point1.x, point1.y, point2.x, point2.y) / 2) / (length / 2)));
		offset *= outwardJoint ? -1 : 1;

		double rawAngle = Lib.GetDir(point1.x, point1.y, point2.x, point2.y);

		double angle;

		if (Double.IsNaN(offset)) {
			angle = rawAngle;
		} else {
			angle = rawAngle + offset;
		}

		Vector3 elbow = new Vector3(point1.x + Lib.CosX(length / 2, angle),
										point1.y + Lib.SinY(length / 2, angle),
										point1.z);

		double footAngle = Lib.GetDir(elbow.x, elbow.y, point2.x, point2.y);
		double elbowAngle = (angle + footAngle) / 2;

		coords.Add(shoulder[orientation ? 0:1]);
		coords.Add(shoulder[orientation ? 1:0]);
		coords.Add(VectorFactory.IKPoint(elbow, thickness / 2, angle + 90));
		coords.Add(VectorFactory.IKPoint(elbow, thickness / 2, angle - 90));
		coords.Add(VectorFactory.IKPoint(elbow, thickness / 2, footAngle + 90));
		coords.Add(VectorFactory.IKPoint(elbow, thickness / 2, footAngle - 90));
		coords.Add(VectorFactory.IKPoint(point2, thickness / 2, footAngle + 90));
		coords.Add(VectorFactory.IKPoint(point2, thickness / 2, footAngle - 90));
		
		return coords;

	}

	public bool GetOrientation() {
		return orientation;
	}

	public void UpdateLeg() {

		stepSpeed = parent.GetSpeed();

		double angle =	parent.GetDirection()
						+ legOffset * (orientation ? 1 : -1);

		Vector3 targetPos = new Vector3(parent.GetX() + (float) Lib.CosX(length, angle),
										parent.GetY() + (float) Lib.SinY(length, angle),
										parent.GetZ());

		if (stepping) {
			position.x += (float)((position.x > targetPos.x) ? -stepSpeed : stepSpeed);
			position.y += (float)((position.y > targetPos.y) ? -stepSpeed : stepSpeed);

			if (Lib.GetDist(position.x, position.y, targetPos.x, targetPos.y) < stepSpeed) {
				ToggleStepping();
			}
		} else {
			if (Lib.GetDist(position.x, position.y, targetPos.x, targetPos.y) > length) {
				ToggleStepping();
			} else if (Lib.GetDist(parent.GetX(), parent.GetY(), position.x, position.y) < lastDist) {
				parent.Push(length/5 * (orientation ? 1:-1));
			}
		}

		lastDist = Lib.GetDist(parent.GetX(), parent.GetY(), position.x, position.y);

		if (parent.GetState() == 1) {
			VectorFactory.DrawCircle(position, thickness, Color.yellow);
		}
	}



}
