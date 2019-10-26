using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	const int laengeMax = 12;
	const int breiteMax = 10;
	const int quellenMax = 3;

	const int riverFaktor = 120; // je hoeher, desto wahrscheinlicher das der Fluss langer wird.
	const int turnFaktor = 20; // je hoeher, desto wahrscheinlicher das der Fluss breiter wird.
	const int returnFaktor = 20; // je hoeher, desto wahrscheinlicher das der Fluss schmaler wird.
	const int lakeFaktor = 30; // je hoeher, desto wahrscheinlicher das ein See entsteht.
	const int hillFaktor = 50; // je hoeher, desto wahrscheinlicher das ein Huegel entsteht.
	const int villageFaktor = 94; // je hoeher, desto mehr doerfer.

	public int[,] field;

	void Awake()
	{
		System.Random rnd = new System.Random();
		field = new int[laengeMax, breiteMax];//-1,0,1, 9: Haus
		SetzeQuellen(field, rnd);
		GenLevel1(field, rnd);
		GenLevel2(field, rnd);
		for (int n = 0; n < 3; n++)
		{
			GenHaus(field, rnd);
		}
		/*
		for (int l = 0; l < laengeMax; l++)
		{
			for (int b = 0; b < breiteMax; b++)
			{
				Console.Write(field[l, b]);
			}
			Console.WriteLine();
		}
		*/
	}

	static private int ProbabilityLimit(int entfernung)
	{

		return ((100 - (100 * entfernung / laengeMax)));
	}

	// Setzt Quellen 
	static private void SetzeQuellen(int[,] field, System.Random rnd)
	{
		int posQuelleMoeglich = 0;
		int quellenGesetzt = 0;
		float limit;
		for (int b = 0; b < breiteMax; b++)
		{
			if (quellenGesetzt < quellenMax)
			{
				limit = (200 * (b - posQuelleMoeglich)) / breiteMax;
				if (rnd.Next(99) < limit)
				{
					field[0, b] = -1;
					quellenGesetzt++;
					posQuelleMoeglich = b + 1;
				}
				else
				{
					field[0, b] = 0;
				}
			}
		}
	}

	static private void GenLevel1(int[,] field, System.Random rnd)
	{
		for (int l = 1; l < laengeMax; l++)
		{
			for (int b = 0; b < breiteMax; b++)
			{
				if (field[l - 1, b] == -1)
				{
					if ((b > 0 && field[l - 1, b - 1] == -1) || (b < breiteMax - 1 && field[l - 1, b + 1] == -1))
					{
						if (rnd.Next(99) < returnFaktor)
						{
							field[l, b] = 0;
						}
						else
						{
							if (rnd.Next(99) + 100 - riverFaktor < ProbabilityLimit(l))
							{
								field[l, b] = -1;
							}
							else
							{
								field[l, b] = 0;
							}
						}
					}
					else
					{
						if (rnd.Next(99) + 100 - riverFaktor < ProbabilityLimit(l))
						{
							field[l, b] = -1;
						}
						else
						{
							field[l, b] = 0;
						}
					}
				}
				else if ((b > 0 && field[l - 1, b - 1] == -1) || (b < breiteMax - 1 && field[l - 1, b + 1] == -1))
				{
					if (rnd.Next(99) + 100 - turnFaktor < ProbabilityLimit(l))
					{
						field[l, b] = -1;
					}
					else
					{
						field[l, b] = 0;
					}
				}
				else
				{
					if (rnd.Next(99) + 100 - lakeFaktor < 100 - ProbabilityLimit(l))
					{
						field[l, b] = -1;
					}
					else
					{
						field[l, b] = 0;
					}
				}
			}
		}
	}

	static private int Umgebung(int[,] field, int centerLaenge, int centerBreite, int art)
	{
		int ret = 0;
		for (int l = centerLaenge - 1; l <= centerLaenge + 1; l++)
		{
			for (int b = centerBreite - 1; b <= centerBreite + 1; b++)
			{
				if (field[l, b] == art)
				{
					ret++;
				}
			}
		}
		return ret;
	}

	static private void GenLevel2(int[,] field, System.Random rnd)
	{
		for (int l = 1; l < laengeMax - 1; l++)
		{
			for (int b = 1; b < breiteMax - 1; b++)
			{
				if (field[l, b] == 0)
				{
					int umgebung = Umgebung(field, l, b, 0);
					if (rnd.Next(99) + 100 - hillFaktor < umgebung * 10)
					{
						field[l, b] = 1;
					}
				}
			}
		}
	}

	static private void GenHaus(int[,] field, System.Random rnd)
	{
		for (int l = 1; l < laengeMax - 1; l++)
		{
			for (int b = 1; b < breiteMax - 1; b++)
			{
				if (field[l, b] == 0)
				{
					int umgebung = Umgebung(field, l, b, 9);
					if (rnd.Next(99) + 100 - villageFaktor < (umgebung + 1) * 10)
					{
						field[l, b] = 9;
					}
				}
			}
		}
	}
}