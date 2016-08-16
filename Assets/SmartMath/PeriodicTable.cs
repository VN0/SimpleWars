using UnityEngine;
using System.Collections;
using SmartMath;

public class PeriodicTable : MonoBehaviour {

    public int atomicnumber; //the number of an element
    public string elementname; //the name of the given element
    public string mass; //the mass of the given element
    public string symbol; //the symbol of the givem element

	void Update () 
    {
        elementname = Chemie.PeriodicTable.Name(atomicnumber);
        mass = Chemie.PeriodicTable.Atomicmass(atomicnumber);
        symbol = Chemie.PeriodicTable.Symbol(atomicnumber);

	}
}
