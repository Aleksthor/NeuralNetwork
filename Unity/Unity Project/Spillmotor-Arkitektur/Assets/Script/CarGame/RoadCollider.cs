using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCollider : MonoBehaviour
{
    public int index;
    int lap = 0;

    List<PhysicsCar> cars_one = new List<PhysicsCar>();
    List<PhysicsCar> cars_two = new List<PhysicsCar>();
    List<PhysicsCar> cars_three = new List<PhysicsCar>();
    List<float> times_one = new List<float>();
    List<float> times_two = new List<float>();
    List<float> times_three = new List<float>();
    public void AddTime(PhysicsCar car, float t, int _lap)
    {
        
        if (_lap > lap) { lap = _lap; }

        switch(lap)
        {
            case 0:
                cars_one.Add(car);
                times_one.Add(t);
                break;
            case 1:
                cars_two.Add(car);
                times_two.Add(t);
                break;
            case 2:
                cars_three.Add(car);
                times_three.Add(t);
                break;

        }
        
    }
    public void ResetCollider()
    {
        cars_one = new List<PhysicsCar>();
        cars_two = new List<PhysicsCar>();
        cars_three = new List<PhysicsCar>();
        times_one = new List<float>();
        times_two = new List<float>();
        times_three = new List<float>();
        lap = 0;
    }
    public bool HasTimes(int lap)
    {
        switch (lap)
        {
            case 0:
                return times_one.Count > 0;
            case 1:
                return times_two.Count > 0;
            case 2:
                return times_three.Count > 0;

        }
        return false;
    }
    public List<PhysicsCar> GetCars(int lap)
    {
        switch (lap)
        {
            case 0:
                return cars_one;
            case 1:
                return cars_two;
            case 2:
                return cars_three;

        }

        return null;
    }
    public List<float> GetTimes(int lap)
    {
        switch (lap)
        {
            case 0:
                return times_one;
            case 1:
                return times_two;
            case 2:
                return times_three;

        }

        return null;
    }
    public int GetLap()
    {
        return lap;
    }


}
