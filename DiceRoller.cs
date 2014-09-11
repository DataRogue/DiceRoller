using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *This is a dice rolling system created to power 
 * internal checks in Vigilante: Breaking Point. 
 * The system is primarily inspired by PnP RPGs
 * such as Shadorun and World Of Darkness. It
 * uses bell curve probability alogorithms,
 * which is useful for more realistic simulation
 * of ability vs difficulty challanges than linear
 * progression.
 * 
 * The abstract of math is that one "rolls" a certain amount
 * of dice. For each die that meets or exceeds the "target"
 * they gain one "hit". If the total amount of "hit"s exceeds
 * the "success" value, then RollTest returns true to indicate
 * success, otherwise returns false for failure.
 * 
 * The two main methods are GetChanceOfSuccess, which
 * predicts the the chances of success out of %100 rounded
 * down using a logarthmic function. The second main function
 * is RollTest which simulates the dice rolling and also has
 * an output option to determine how many hits were acquired.
 * 
 * Designed for Unity. For generic C# conversion simply change the
 * referenced Math class.
 */

public static class DiceRoller {
	static public int StandardTarget = 4; //Inclusive, target value on die to hit.
	static public int SidesOnDice = 6;	// How many sides will the simulated die contain
	static public List<int> Seeds = new List<int>(); // A list of saved seeds.

	//Return a % chance rounded down of how likely RollTest will return true
	//A bit resource instensive, careful with frequent calls on mobile devices.
	static public int GetChanceOfSuccess(float rolls, float target, int success)
	{
		if (rolls<=0) return 0;
		float chanceOfTarget = (SidesOnDice-target+1)/SidesOnDice;
		float val = 0;
		for(int i=success; i<=rolls;i++)
		{
			float successChance = Mathf.Pow(chanceOfTarget, i);
			float failureChance = Mathf.Pow(1-chanceOfTarget, rolls-i);
			val += successChance*failureChance*Combination(rolls, (float)i);
		}

		return (int) (val*100);
	}

	//A simple function that calculates how many ways it's possible for n to pick k.
	static public float Combination(float n, float k)
	{
		float sum=0;
		for(float i=0;i<n;i++)
		{
			sum+=Mathf.Log10(i+1);
		}
		for(float i=1;i<=n-k;i++)
		{
			sum-=Mathf.Log10(i);
		}
		for(float i=1;i<=k;i++)
		{
			sum-=Mathf.Log10(i);
		}
		return Mathf.Pow(10, sum);
	}

	//Simulates the dice rolls and returns true or false
	static public bool RollTest(int rolls, int target, int success)
	{
		if(rolls<=0) return false;
		if(RollHits(rolls,target)>=success) return true;
		return false;
	}

	//Overload option in-case the amount of hits acquired is also needed
	static public bool RollTest(int rolls, int target, int success, ref int output)
	{
		int hits = RollHits(rolls,target);
		output = hits;
		if(hits>=success) return true;
		return false;
	}
	
	//Unity ONLY. Use this to get/set seed for more controlled results.
	//Returns the index value of the seed.
	static public int SaveSeed()
	{
		Seeds.Add(Random.seed);
		return Seeds.Count-1;
	}
	//Unity ONLY. Use this to get/set seed for more controlled results.
	static public void SetSeed(int seed)
	{
		Random.seed = seed;
	}
	//Rolls a single die and returns its value
	static public int RollDie()
	{
		return Random.Range(1, SidesOnDice);
	}

	//Recurisve function warning, default tries is 100. 
	//Returns how many rolls it took to get a hit. If fails to get hit before
	//limit is reached, returns -1.
	static public int RollUntilHit(int target, int maxTries = 100, int start = 0)
	{
		int val = start + 1;
		if(RollTarget(target))return val;
		if(val >= maxTries) return -1;
		return RollUntilHit(target, maxTries, val);
	}

	static private int RollHits(int rolls, int target)
	{
		int hits = 0;
		for(int i=0; i<rolls;i++)
		{
			if(RollTarget(target))hits++;
		}
		return hits;
	}

	static private bool RollTarget(int target)
	{
		return (RollDie() >= target);
	}
}
