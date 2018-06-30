using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lib : MonoBehaviour {


	public static double ToRad(double degrees) {
		return degrees * (Math.PI / 180);
	}

	public static double ToDeg(double radians) {
		return radians * (180 / Math.PI);
	}

	public static float CosX(double magnitude, double angle) {
		return (float) (magnitude * (Math.Cos(ToRad(angle))));
	}

	public static float SinY(double magnitude, double angle) {
		return (float) (magnitude * (Math.Sin(ToRad(angle))));
	}

	public static double GetDir(double x1, double y1, double x2, double y2) {
		return (((x2<x1) ? 180:0) + ToDeg(Math.Atan((y2 - y1) / (x2 - x1)))) % 360;
	}

	public static double GetDist(double x1, double y1, double x2, double y2) {
		return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
	}


}
