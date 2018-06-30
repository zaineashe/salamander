using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body {
	private Vector3 position;
	private Lizard master;
	private Body parent;

	private double baseSize;
	private double size;
	private ArrayList legs = null;
	private double direction;

	private double breathing;
	private bool inOut;
	private double speedFactor;


	public Body(double size, Vector3 position, Lizard master, Body parent) {
		legs = null;
		this.baseSize = size;
		this.size = baseSize;
		this.position = position;
		this.parent = parent;
		direction = 0;
		this.master = master;
		breathing = 0;
		speedFactor = 0;
	}

	public void SetLegs(ArrayList legs) {
		this.legs = legs;
	}

	public void PointTo(float x, float y) {
		direction = Lib.GetDir(position.x,position.y,x,y);
	}

	public float GetX() {
		return position.x;
	}

	public float GetY() {
		return position.y;
	}

	public float GetZ() {
		return position.z;
	}

	public double GetDirection() {
		return direction;
	}

	public Leg GetLeg(bool orientation) {

		orientation = !orientation;

		if (legs == null) {
			return null;
		} else {
			for (int i = 0; i < legs.Count; i++) {
				if (((Leg)legs[i]).GetOrientation() == orientation) {
					return ((Leg)legs[i]);
				}
			}
			return null;
		}
	}

	public void Push(double angle) {
		double dir = Lib.GetDir(parent.GetX(), parent.GetY(), position.x, position.y);
		double dist = Lib.GetDist(parent.GetX(), parent.GetY(), position.x, position.y);
		position.x = parent.GetX() + Lib.CosX(dist, dir + angle);
		position.y = parent.GetY() + Lib.SinY(dist, dir + angle);
	}

	private Vector3 UpperBodyVect(bool side) {
		return new Vector3(	position.x + Lib.CosX(size, direction + 45.0 * (side ? 1 : -1)),
							position.y + Lib.SinY(size, direction + 45.0 * (side ? 1 : -1)), position.z);
	}

	private Vector3 LowerBodyVect(bool side) {
		return new Vector3(	position.x + Lib.CosX(size, direction + 135.0 * (side ? 1 : -1)),
							position.y + Lib.SinY(size, direction + 135.0 * (side ? 1 : -1)), position.z);
	}

	public Vector3[] RawSideVectors(bool side) {
		return new Vector3[] {	UpperBodyVect(side),LowerBodyVect(side) };
	}

	public ArrayList GetLegCoords(ArrayList coords, bool orientation) {
		Leg leg = GetLeg(orientation);
		if (leg != null) {
			coords = leg.GetLegByShoulder(coords, new Vector3[] {UpperBodyVect(!orientation), LowerBodyVect(!orientation)}, !orientation);
		}
		return coords;
	}

	public ArrayList GetVectors(ArrayList coords) {
		coords.Add(UpperBodyVect(true));
		coords.Add(UpperBodyVect(false));
		coords.Add(LowerBodyVect(true));
		coords.Add(LowerBodyVect(false));
		return coords;
	}

	public double GetSpeed() {
		return speedFactor;
	}

	public int GetState() {
		return master.GetState();
	}

	public void UpdateBody() {

		size = baseSize + breathing;

		if ((inOut == true && breathing > 0.2) || (inOut == false && breathing <= 0.1)) {
			inOut = !inOut;
		}

		if (master.GetSpeed() < 0.1) {
			breathing += ((inOut ? 0.3 : 0) - breathing) / 20;
		} else {
			inOut = false;
		}
		

		float targetX = position.x;
		float targetY = position.y;

		if (parent == null) {

			if (Lib.GetDist(position.x, position.y, master.transform.position.x, master.transform.position.y) > size * 2) {
				targetX = master.transform.position.x + (float)Lib.CosX(0.1, master.GetDirection() + 180);
				targetY = master.transform.position.y + (float)Lib.SinY(0.1, master.GetDirection() + 180);
			}

			direction = master.GetDirection();

		} else {
			PointTo(parent.GetX(), parent.GetY());

			if (Lib.GetDist(position.x, position.y, parent.GetX(), parent.GetY()) > size * 2) {
				targetX = parent.GetX() + (float)Lib.CosX(size * 2, parent.GetDirection() + 180);
				targetY = parent.GetY() + (float)Lib.SinY(size * 2, parent.GetDirection() + 180);
			}
		}

		
		position.x += (float)((targetX - position.x) / 3.0);
		position.y += (float)((targetY - position.y) / 3.0);

		speedFactor = Math.Abs((((targetX - position.x) / 3.0) + (((targetY - position.y) / 3.0)))/2) * 10;
		if (speedFactor < 0.8) {
			speedFactor = 0.8;
		}

		if (legs != null)
		{
			for (int i = 0; i < legs.Count; i++)
			{
				Leg leg = (Leg)legs[i];
				leg.UpdateLeg();
				
			}
		}

		if (master.GetState() == 0) {
			VectorFactory.DrawCircle(position, size, Color.white);
		}
	}
}
