using System.Linq;
using UnityEngine;

namespace SmartMath
{
	public class Math
	{
		public static float CubeArea(float a)
		{
			return 6 * Mathf.Pow(a, 2);
		}
		
		public static float CubeVolume(float a)
		{
			return Mathf.Pow(a, 3);
		}
		
		public static float CuboidArea(float a, float b, float c, float d)
		{
			return 2 * (a*b +b*c + c*a);
		}
		
		public static float CuboidVolume(float a, float b, float c)
		{
			return a * b * c;
		}
		
		public static float CylinderArea(float r, float h)
		{
			return 2*Mathf.Pow (r,2)* Mathf.PI + 2*r*Mathf.PI*h;
		}
		public static float CylinderVolume(float r, float h)
		{
			return Mathf.Pow(r,2)*Mathf.PI*h;
		}
		public static float ConeArea(float r, float a)
		{
			return Mathf.Pow(r,2)*Mathf.PI+r*Mathf.PI*a ;
		}
		public static float ConeVolume(float r, float h)
		{
			return (Mathf.Pow(r, 2) * Mathf.PI * h) / 3;
		}
		public static float SphereArea(float R)
		{
			return 4*Mathf.Pow(R,2)*Mathf.PI;
		}
		public static float SphereVolume(float R)
		{
			return (4 * Mathf.Pow(R, 2) * Mathf.PI) / 3;
		}
		public static float EllipsoidVolume(float a, float b, float c)
		{
			return 4 * a * b * c * Mathf.PI;
		}
		public static float TetrahedronArea(float a)
		{
			return  Mathf.Pow(a, 2)* Mathf.Sqrt(3);
		}
		public static float TetrahedronVolume (float a)
		{
			return  (Mathf.Pow(a, 3)*Mathf.Sqrt(2))/12;
		}
		public static float OctahedronArea(float a)
		{
			return 2*Mathf.Pow(a,2)*Mathf.Sqrt(3);
		}
		public static float OctahedronVolume(float a)
		{
			return (Mathf.Pow(a, 3) * Mathf.Sqrt(2)) / 3;
		}
		public static float DodecahedronArea(float a)
		{
			return 3*Mathf.Pow(a,2)*(Mathf.Sqrt(25+10)*(Mathf.Sqrt(5)));
		}
		public static float DodecahedronVolume(float a)
		{
			return (Mathf.Pow(a, 3) * (15 + 7 * Mathf.Sqrt(5))) / 4;
		}
		public static float IcosahedronArea(float a)
		{
			return 5*Mathf.Pow(a,2)*Mathf.Sqrt(3);
		}
		public static float IcosahedronVolume(float a)
		{
			return (Mathf.Pow(a, 3) *(15+5*Mathf.Sqrt(5)))/12;
		}
		/*
        public static float CardanoFormula(float q, float p)

        {
            return Mathf.Pow(-q/2 + Mathf.Sqrt(Mathf.Pow(q/2,2) + (Mathf.Pow(p/3,3))),1/3) + Mathf.Pow(-q/2 - Mathf.Sqrt(Mathf.Pow(q/2,2) + (Mathf.Pow(p/3,3))),1/3);
        } 
        */
		
		public static float Percentage(float number, float percent)
		{
			return (number / 100) * percent;
		}
		public static float PerMille(float number, float permill)
		{
			return (number / 1000) * permill;
		}
		public static float Distance3D(Vector3 firstPoint, Vector3 secondPoint)
		{
			return Mathf.Sqrt(Mathf.Pow((secondPoint.x - firstPoint.x),2) + Mathf.Pow((secondPoint.y - firstPoint.y),2) + Mathf.Pow((secondPoint.z - firstPoint.z),2));
		}
		public static float Distance2D(Vector2 firstPoint, Vector2 secondPoint)
		{
			return Mathf.Sqrt(Mathf.Pow((secondPoint.x - firstPoint.x), 2) + Mathf.Pow((secondPoint.y - firstPoint.y), 2));
		}
		public static float Distance1D(float firstPoint, float secondPoint)
		{
			return Mathf.Sqrt(Mathf.Pow((secondPoint - firstPoint), 2));
		}
		public static float Average(params double[] i)
		{
			return float.Parse(i.Average().ToString());
		}
		public static float Max(params double[] i) 
		{
			return float.Parse(i.Max().ToString());
		}
		public static float Min(params double[] i)
		{
			return float.Parse(i.Min().ToString());
		}
		public static float Sum(params double[] i)
		{
			return float.Parse(i.Sum().ToString());
		}
		public static float GeometricSequence(float r, float a, float n)
		{
			return a * Mathf.Pow(r,(n-1));
		}
		public static float GeometricSequenceSum(float r, float a, float n)
		{
			return a + ((1-Mathf.Pow(r,n)) / (1-r));
		}
		public static float ArithmeticSequence(float d, float a, float n)
		{
			return a + (n - 1) * d;
		}
		public static float ArithmeticSequenceSum1(float a1, float an, float n)
		{
			return (n*(a1+an)) /2;
		}
		public static float ArithmeticSequenceSum2(float a1, float n, float d)
		{
			return ((2*a1+d*(n-1))*n) /2;
		}
		public static float CompoundInterest(float P, float i, float n, float t)
		{
			return Mathf.Round(P*Mathf.Pow((1 + i/n),n*t));
		}
		public static float NRoot(float root, float number)
		{
			return Mathf.Pow(number, 1F / root);
		}
		public static float HeronsFormula(float s, float a, float b, float c)
		{
			float resultsqrt;
			
			resultsqrt = s * (s - a) * (s - b) * (s - c);
			
			return Mathf.Sqrt(resultsqrt);
		}
		public static float TrianglePerimeter(float a, float b, float c)
		{
			return a + b + c;
		}
		public static float SquarePerimeter(float a)
		{
			return 4 * a;
		}
		public static float RhombusPerimeter(float a)
		{
			return 4 * a;
		}
		public static float CirclePerimeter(float r)
		{
			return 2 * r * Mathf.PI;
		}
		public static float RectanglePerimeter(float a, float b)
		{
			return (2 * a) + (2 * b);
		}
		public static float KitePerimeter(float a, float b)
		{
			return (2 * a) + (2 * b);
		}
		public static float SemicirclePerimeter(float r)
		{
			return r * Mathf.PI;
		}
		public static float TriangleArea(float a, float h)
		{
			return (a * h) / 2;
		}
		public static float SquareArea(float a)
		{
			return Mathf.Pow(a,2);
		}
		public static float RectangleArea(float a, float b)
		{
			return a * b;
		}
		public static float RhombusArea(float a, float h)
		{
			return a * h;
		}
		public static float KiteArea(float d1, float d2)
		{
			return (d1 * d2) / 2;
		}
		public static float CircleArea(float r)
		{
			return Mathf.Pow(r, 2) * Mathf.PI;
		}
		public static float SemicircleArea(float r)
		{
			return (Mathf.Pow(r, 2) * Mathf.PI) / 2;
		}
		public static float SinesLaw(float egyikoldal, float elsoszog, float masodikszog)
		{
			return ((egyikoldal * Mathf.Sin(elsoszog)) / Mathf.Sin(masodikszog));
		}
		public static float CosineLaw(float egyikoldal, float masikoldal, float szog)
		{
			float result1;
			float result;
			result1 = (Mathf.Pow(egyikoldal, 2) + Mathf.Pow(masikoldal, 2) - 2 * egyikoldal * masikoldal * Mathf.Cos(szog));
			result = Mathf.Sqrt(result1);
			return result;
		}
		public static float Pythagoras(float befogo1, float befogo2)
		{
			float result1;
			result1 = Mathf.Pow(befogo1, 2) + Mathf.Pow(befogo2, 2);
			return Mathf.Sqrt(result1);
		}
		public static float QuadraticEquationPlus(float a, float b, float c)
		{
			float result1;
			float result3;
			float result;
			
			result1 = Mathf.Pow((b), 2) - 4*a*c;
			result3 = Mathf.Sqrt(result1);
			result = (-(b) + result3) / (2*a);
			return result;
			
		}
		public static float QuadraticEquationMinus(float a, float b, float c)
		{
			float result1;
			float result3;
			float result;
			
			result1 = Mathf.Pow((b), 2) - 4 * a * c;
			result3 = Mathf.Sqrt(result1);
			result = (-(b) - result3) / (2 * a);
			return result;
		}
		public static float RemarkableIdentities1(float a, float b)
		{
			float az = Mathf.Pow(a + b, 2);
			return  az = Mathf.Pow(a, 2) + 2 * a * b + Mathf.Pow(b, 2);
		}
		public static float RemarkableIdentities2(float a, float b)
		{
			float az = Mathf.Pow(a - b, 2);
			return az = Mathf.Pow(a, 2) - 2 * a * b + Mathf.Pow(b, 2);
		}
		public static float RemarkableIdentities3(float a, float b)
		{
			float az = (a + b) * (a - b);
			return az = Mathf.Pow(a, 2) - Mathf.Pow(b, 2);
		}

		public static float Probability(float favorablenumber, float possiblenumber)
		{
			return favorablenumber / possiblenumber;
		}
		public static int Factorial(int number)
		{
			int i;
			int result;
			
			result = number;
			
			if (number == 0)
			{
				result = 1;
				
			}
			if (number < 0)
			{
				
				number *= -1;
				
			}
			for (i = number - 1; i >= 1; i--)
			{
				if (number > 0)
				{
					result = result * i;
				}
			}
			return result;
			
		}
		
	}
	public class Chemie
	{
		public static float Faraday(float M, float z , float F, float l, float t)
		{
			return (M / (z * F)) * l * t;
		}
		
		public class PeriodicTable
		{
			
			private static string[] names = new string[] { "-", "Hydrogen", "Helium", "Lithium", "Beryllium", "Boron", "Carbon", "Nitrogen", "Oxygen", "Fluorine", "Neon", "Sodium", "Magnesium", "Aluminium", "Silicon", "Phosphorus", "Sulfur", "Chlorine", "Argon", "Potassium", "Calcium", "Scandium", "Titanium", "Vanadium", "Chromium", "Manganese", "Iron", "Cobalt", "Nickel", "Copper", "Zinc", "Gallium", "Germanium", "Arsenic", "Selenium", "Bromine", "Krypton", "Rubidium", "Strontium", "Yttrium", "Zirconium", "Niobium", "Molybdenum", "Technetium", "Ruthenium", "Rhodium", "Palladium", "Silver", "Cadmium", "Indium", "Tin", "Antimony", "Tellurium", "Iodine", "Xenon", "Cesium", "Barium", "Lanthanium", "Cerium", "Praseodymium", "Neodymium", "Promethium", "Samarium", "Europium", "Gadolinium", "Terbium", "Dysprosium", "Holmium", "Erbium", "Thulium", "Ytterbium", "Lutetium", "Hafnium", "Tantalum", "Tungsten", "Rhenium", "Osmium", "Iridium", "Platinum", "Gold", "Mercury", "Thallium", "Lead", "Bismuth", "Polonium", "Astatine", "Radon", "Francium", "Radium", "Actinium", "Thorium", "Protactinium", "Uranium", "Neptunium", "Plutonium", "Americium", "Curium", "Berkelium", "Californium", "Einsteinium", "Fermium", "Mendelevium", "Nobelium", "Lawrenceium", "Rutherfordium", "Dubnium", "Seaborgium", "Bohrium", "Hassium", "Meitnerium", "Darmstatium", "Roentgenium", "Copernicium", "Ununtrium", "Flerovium", "Ununpentium", "Livermorium", "Ununseptium", "Ununoctium" };
			private static string[] symbols = new string[] { "-", "H", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne", "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar", "K", "Ca", "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr", "Rb", "Sr", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I", "Xe", "Cs", "Ba", "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu", "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg", "Ti", "Pb", "Bi", "Po", "At", "Rn", "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr", "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Uut", "Fl", "Uup", "Lv", "Uus", "Uuo" };
			private static string[] masses = new string[] { "-", "1.008", "4.003", "6.941", "9.012", "10.811", "12.011", "14.007", "15.999", "18.998", "20.180", "22.990", "24.305", "26.982", "28.086", "30.974", "32.066", "35.453", "39.948", "39.098", "40.078", "44.956", "47.867", "50.942", "51.996", "54.938", "55.845", "58.933", "58.693", "63.546", "65.38", "69.723", "72.631", "74.922", "78.971", "79.904", "84.798", "84.468", "87.62", "88.906", "91.224", "92.906", "95.95", "98.907", "101.07", "102.906", "106.42", "107.868", "112.411", "114.818", "118.711", "121.780", "127.6", "126.904", "131.294", "132.905", "137.328", "138.905", "140.116", "140.908", "144.243", "144.913", "150.36", "151.964", "157.25", "158.925", "162.500", "164.930", "167.259", ",168.934", "173.055", "174.967", "178.49", "180.948", "183.84", "186.207", "190.23", "192.217", "195.085", "196.967", "200.592", "204.383", "207.2", "208.980", "208.982", "209.987", "222.018", "223.020", "226.025", "227.028", "232.038", "231.036", "238.029", "237.048", "244.064", "243.061", "247.070", "247.070", "251.080", "254", "257.095", "258.1", "259.101", "262", "261", "262", "266", "264", "269", "268", "269", "272", "277", "-", "289", "-", "298", "-", "-" };
			
			
			public static string Name(int atomicnumber)
			{
				return names[atomicnumber];
			}
			public static string Symbol(int atomicnumber)
			{
				return symbols[atomicnumber];
			}
			public static string Atomicmass(int atomicnumber)
			{
				return masses[atomicnumber];
			}
		}
		
	}
	public class General
	{
		
		public static float Calorie(float age, string gender, float weightinkg)
		{
			float result = 0;
			
			if (gender == "female" && age >= 11 && age <= 18)
			{
				result = ((12.2f*weightinkg) + 746)*1.6f;
			}
			
			else if (gender == "female" && age >= 19 && age <= 30)
			{
				result = ((14.7f * weightinkg) + 496) * 1.6f;
			}
			
			else if (gender == "female" && age >= 31 && age <= 60)
			{
				result = ((08.7f * weightinkg) + 829) * 1.6f;
			}
			
			else if (gender == "male" && age >= 11 && age <= 18)
			{
				result = ((17.5f * weightinkg) + 651) * 2.2f;
			}
			
			else if (gender == "male" && age >= 19 && age <= 30)
			{
				result = ((15.3f * weightinkg) + 679) * 2.2f;
			}
			
			else if (gender == "male" && age >= 31 && age <= 60)
			{
				result = ((11.6f * weightinkg) + 879) * 2.2f;
			}
			return result;    
		}
		public static float BMI(float weightinkg, float heightinm)
		{
			return weightinkg / (Mathf.Pow(heightinm, 2));
		}
		public static float BFP(float weightinkg,float heightinm,string gender, float age) 
		{
			float result = 0;
			if (gender == "female")
			{
				result = (1.2f * (weightinkg / Mathf.Pow(heightinm, 2)) + (0.23f * age) - 5.4f - (10.8f * 0));
			}
			if (gender == "male")
			{
				result = (1.2f * (weightinkg / Mathf.Pow(heightinm, 2)) + (0.23f * age) - 5.4f - (10.8f * 1));
			}
			return Mathf.Round(result);
		}
	}
	public class Physic
	{
		public static float PendulumGravityAcceleration(float l, float T)
		{
			float result1;
			float result2;
			
			result1 = 4 * (Mathf.Pow(Mathf.PI, 2));
			result2 = l/(Mathf.Pow(T,2));
			
			return result1 * result2;
		}
		public static float Frequency(float T)
		{
			return 1f / T;
		}
		public static float OhmLaw(float V, float R)
		{
			return V / R;
		}
		public static float PeriodicWave(float lambda, float f)
		{
			return lambda * f;
		}
		public static float Coulomb(float k, float q1, float q2, float r)
		{
			
			return k * ((q1 * q2) / float.Parse(System.Math.Pow(r, 2).ToString()));
			
		}
		public static float Capacitance(float Q, float V)
		{
			
			return Q / V;
			
		}
		public static float SensibleHeat(float m, float c, float T)
		{
			
			return m * c * T;
			
		}
		public static float LatentHeat(float m, float L)
		{
			
			return m * L;
			
		}
		public static float ThermoDinFirstLaw(float Q, float W)
		{
			
			return Q + W;
			
		}
		public static float InternalEnergy(float n, float R, float T)
		{
			
			return 1.5f * n * R * T;
			
		}
		public static float MachNumber(float v, float c)
		{
			
			return v / c;
			
		}
		public static float IndexOfRefraction(float c, float v)
		{
			
			return c / v;
			
		}
		public static float MassEnergy(float m, float c)
		{
			
			return m * float.Parse(System.Math.Pow(c, 2).ToString());
			
		}
		public static float PhotonEnergy(float h, float f)
		{
			
			return h * f;
			
		}
		public static float PhotonMomentum(float h, float lambda)
		{
			
			return h / lambda;
			
		}
		public static float PhotoelectricEffect(float h, float f, float f0)
		{
			
			return h * (f - f0);
			
		}
	}
	public class TechnicalDrawing
	{
		public static float ToleranceQ(float s)
		{
			float res1;
			
			res1 = Mathf.Pow(10, 1F / 5);
			return Mathf.Pow(res1, s - 1);
		}
		public static float ToleranceU(float bs)
		{
			float res1;
			res1 = 0.45f*Mathf.Pow(bs, 1F/3);
			return res1 + 0.001f * bs;
		}
		public static float Tolerance1(float bs, float q) 
		{
			return Mathf.Round(bs*q);
		}
		public static float Tolerance2(float d1, float d2)
		{
			return Mathf.Round(d1 - d2);
		}
		
		
		
		
		
	}
	public class Mechanic 
	{
		public static float CenterOfGravityX(float x1, float a1, float x2, float a2)
		{
			return ((x1 * a1) + (x2 * a2)) / (a1 + a2);
		}
		public static float CenterOfGravityY(float y1, float a1, float y2, float a2)
		{
			return ((y1 * a1) + (y2 * a2)) / (a1 + a2);
		}
		public static float ResultantForce(float F1, float F2)
		{
			float sqrt;
			sqrt = Mathf.Pow(F1, 2) + Mathf.Pow(F2, 2);
			return Mathf.Sqrt(sqrt);
		}
		public static float CentripetalAcceleration(float v, float r)
		{
			
			return float.Parse((System.Math.Pow(v, 2) / r).ToString());
			
		}
		public static float Weight(float m, float g)
		{
			
			return m * g;
			
		}
		public static float AverageSpeed(float s, float t)
		{
			
			return s / t;
			
		}
		public static float Distance(float v, float t)
		{
			
			return v * t;
			
		}
		public static float AverageAcceleration(float v, float t)
		{
			
			return v / t;
			
		}
		public static float KineticEnergy(float m, float v)
		{
			
			return float.Parse((0.5f * m * System.Math.Pow(v, 2)).ToString());
			
		}
		public static float MechanicalEfficiency(float W, float E)
		{
			
			return W / E;
			
		}
		public static float AveragePower(float W, float t)
		{
			
			return W / t;
			
		}
		public static float Torque(float r, float F, float Theta)
		{
			
			return float.Parse((r * F * System.Math.Sin(Theta)).ToString());
			
		}
		public static float Newton2(float m, float a)
		{
			
			return m * a;
			
		}
		public static float ImpulseMomentum(float m, float v)
		{
			
			return m * v;
			
		}
		public static float HookeLaw(float k, float x)
		{
			
			return -k * x;
			
		}
		public static float Pressure(float F, float A)
		{
			
			return F / A;
			
		}
		public static float Buoyancy(float rho, float g, float V)
		{
			
			return rho * g * V;
			
		}
		public static float KinematicViscosity(float rho, float eta)
		{
			
			return eta / rho;
			
		}
		public static float AerodynamicDrag(float rho, float C, float A, float v)
		{
			
			return float.Parse((0.5f * rho * C * A * System.Math.Pow(v, 2)).ToString());
			
		}
	}
	public class MaterialsScience
	{
		public static float CuttingSplit(float m, float s)
		{
			return m * s;
		}
		public static float KFactor(float alpha1, float alpha2)
		{
			return alpha2 / alpha1;
		}
	}
	public class Converting
	{
		public class Length
		{
			
			//mm
			public static float MmToCm(float mm)
			{
				return mm / 10;
			}
			public static float CmToMm(float cm)
			{
				return cm * 10;
			}
			public static float MmToMeter(float mm)
			{
				return mm / 1000;
			}
			public static float MeterToMm(float meter)
			{
				return meter * 1000;
			}
			public static float MmToDm(float mm)
			{
				return mm / 100;
			}
			public static float DmToMm(float dm)
			{
				return dm * 100;
			}
			public static float MmToKm(float mm)
			{
				return mm / 1000000;
			}
			public static float KmToMm(float km)
			{
				return km * 1000000;
			}
			public static float MmToMile(float mm)
			{
				return mm / 1609000;
			}
			public static float MileToMm(float mile)
			{
				return mile * 1609344;
			}
			public static double MmToYard(float mm)
			{
				return mm / 914.4;
			}
			public static double YardToMm(float yard)
			{
				return yard * 914.4;
			}
			public static double MmToFeet(float mm)
			{
				return mm / 304.8;
			}
			public static double FeetToMm(float feet)
			{
				return feet * 304.8;
			}
			public static double MmToInch(float mm)
			{
				return mm / 25.4;
			}
			public static double InchToMm(float inch)
			{
				return inch * 25.4;
			}
			
			
			
			//cm
			
			public static float CmToMeter(float cm)
			{
				return cm / 100;
			}
			public static float MeterToCm(float meter)
			{
				return meter * 100;
			}
			public static float CmToDm(float cm)
			{
				return cm / 10;
			}
			public static float DmToCm(float dm)
			{
				return dm * 10;
			}
			public static float CmToKm(float cm)
			{
				return cm / 100000;
			}
			public static float KmToCm(float km)
			{
				return km * 100000;
			}
			public static float CmToMile(float cm)
			{
				return cm / 160900;
			}
			public static double MileToCm(float mile)
			{
				return mile * 160934.4;
			}
			public static double CmToYard(float cm)
			{
				return cm / 91.44;
			}
			public static double YardToCm(float yard)
			{
				return yard * 91.44;
			}
			public static double CmToFeet(float cm)
			{
				return cm / 30.48;
			}
			public static double FeetToCm(float feet)
			{
				return feet * 30.48;
			}
			public static double CmToInch(float cm)
			{
				return cm / 2.54;
			}
			public static double InchToCm(float inch)
			{
				return inch * 2.54;
			}
			
			
			//dm
			
			public static float DmToMeter(float dm)
			{
				return dm / 10;
			}
			public static float MeterToDm(float meter)
			{
				return meter * 10;
			}
			public static float DmToKm(float dm)
			{
				return dm / 10000;
			}
			public static float KmToDm(float km)
			{
				return km * 10000;
			}
			public static float DmToMile(float dm)
			{
				return dm / 16090;
			}
			public static double MileToDm(float mile)
			{
				return mile * 16093.44;
			}
			public static double DmToYard(float dm)
			{
				return dm / 9.144;
			}
			public static double YardToDm(float yard)
			{
				return yard * 9.144;
			}
			public static double DmToFeet(float dm)
			{
				return dm / 3.048;
			}
			public static double FeetToDm(float feet)
			{
				return feet * 3.048;
			}
			public static double DmToInch(float dm)
			{
				return dm * 3.93700787;
			}
			public static double InchToDm(float inch)
			{
				return inch * 0.254;
			}
			
			
			
			
			//meter
			
			public static float MeterToKm(float meter)
			{
				return meter / 1000;
			}
			public static float KmToMeter(float km)
			{
				return km * 1000;
			}
			public static float MeterToMile(float meter)
			{
				return meter / 1609;
			}
			public static double MileToMeter(float mile)
			{
				return mile * 1609.344;
			}
			public static double MeterToYard(float meter)
			{
				return meter * 1.0936133;
			}
			public static double YardToMeter(float yard)
			{
				return yard * 0.9144;
			}
			public static double MeterToFeet(float meter)
			{
				return meter * 3.2808399;
			}
			public static double FeetToMeter(float feet)
			{
				return feet * 0.3048;
			}
			public static double MeterToInch(float meter)
			{
				return meter * 39.3700787;
			}
			public static double InchToMeter(float inch)
			{
				return inch * 0.0254;
			}
			
			
			
			//Km
			
			public static double KmToMile(float km)
			{
				return km / 1.609;
			}
			public static double MileToKm(float mile)
			{
				return mile * 1.609;
			}
			public static double KmToYard(float km)
			{
				return km / 0.0009144;
			}
			public static double YardToKm(float yard)
			{
				return yard * 0.0009144;
			}
			public static double KmToFeet(float km)
			{
				return km / 0.0003048;
			}
			public static double FeetToKm(float feet)
			{
				return feet * 0.0003048;
			}
			public static double KmToInch(float km)
			{
				return km / 0.0000254;
			}
			public static double InchToKm(float inch)
			{
				return inch * 0.0000254;
			}
			
			
			
			//mile
			
			public static double MileToYard(float mile)
			{
				return 1760 * mile;
			}
			public static double YardToMile(float yard)
			{
				return 0.000568181818 * yard;
			}
			public static double MileToFeet(float mile)
			{
				return mile * 5280;
			}
			public static double FeetToMile(float feet)
			{
				return 0.000189393939 * feet;
			}
			public static double MileToInch(float mile)
			{
				return 63360 * mile;
			}
			public static double InchToMile(float inch)
			{
				return inch * 0.0000157862;
			}
			
			//yard
			
			public static double YardToFeet(float yard)
			{
				return yard * 3;
			}
			public static double FeetToYard(float feet)
			{
				return 0.333333333 * feet;
			}
			public static double YardToInch(float yard)
			{
				return 36 * yard;
			}
			public static double InchToYard(float inch)
			{
				return inch * 0.0277777778;
			}
			
			//feet
			
			public static double FeetToInch(float feet)
			{
				return 12 * feet;
			}
			public static double InchToFeet(float inch)
			{
				return inch * 0.0833333333;
			}
			
		}
		public class Temperature
		{
			public static float CelsiusToFahrenheit(float celsius)
			{
				return celsius * 9 / 5 + 32;
			}
			public static float FahrenheitToCelsius(float fahrenheit)
			{
				return (fahrenheit - 32) * 5f / 9;
			}
			public static double CelsiusToKelvin(float celsius)
			{
				return celsius + 273.15;
			}
			public static double KelvinToCelsius(float kelvin)
			{
				if (kelvin == 0)
				{
					return 0;
				}
				else
				{
					return kelvin - 273.15;
				}
			}
		}
		public class Times
		{
			public static float SecToMin(float sec)
			{
				return sec / 60;
			}
			public static float MinToSec(float min)
			{
				return min * 60;
			}
			public static float SecToHour(float sec)
			{
				return sec / 3600;
			}
			public static float HourToSec(float hour)
			{
				return hour * 3600;
			}
			public static float SecToWeek(float sec)
			{
				return sec / 604800;
			}
			public static float WeekToSec(float week)
			{
				return week * 604800;
			}
			public static float SecToMonth(float sec)
			{
				return sec / 2629744;
			}
			public static float MonthToSec(float month)
			{
				return month * 2629744;
			}
			public static float SecToYear(float sec)
			{
				return sec / 31556926;
			}
			public static float YearToSec(float year)
			{
				return year * 31556926;
			}
			
			
			
			public static float MinToHour(float min)
			{
				return min / 60;
			}
			public static float HourToMin(float hour)
			{
				return hour * 60;
			}
			public static float MinToWeek(float min)
			{
				return min / 10080;
			}
			public static float WeekToMin(float week)
			{
				return week * 10080;
			}
			public static double MinToMonth(float min)
			{
				return min / 43829.0639;
			}
			public static double MonthToMin(float month)
			{
				return month * 43829.0639;
			}
			public static float MinToYear(float min)
			{
				return min / 525949;
			}
			public static float YearToMin(float year)
			{
				return year * 525949;
			}
			
			
			
			public static float HourToWeek(float hour)
			{
				return hour / 168;
			}
			public static float WeekToHour(float week)
			{
				return week * 168;
			}
			public static double HourToMonth(float hour)
			{
				return hour / 730;
			}
			public static double MonthToHour(float month)
			{
				return month * 730;
			}
			public static float HourToYear(float hour)
			{
				return hour / 8766;
			}
			public static float YearToHour(float year)
			{
				return year * 8766;
			}
			
			
			
			
			public static double WeekToMonth(float week)
			{
				return week / 4.34812141;
			}
			public static double MonthToWeek(float month)
			{
				return month * 4.34812141;
			}
			public static double WeekToYear(float week)
			{
				return week / 52.177457;
			}
			public static double YearToWeek(float year)
			{
				return year * 52.177457;
			}
			
			
			
			public static float MonthToYear(float month)
			{
				return month / 12;
			}
			public static double YearToMonth(float year)
			{
				return year * 12;
				
			}
		}
		public class Weight
		{
			
			public static float GramToDekagram(float gram)
			{
				
				return gram / 10;
				
			}
			
			public static float DekagramToGram(float dekagram)
			{
				
				return dekagram * 10;
			}
			
			public static float GramToKilogram(float gram)
			{
				
				return gram / 1000;
			}
			
			public static float KilogramToGram(float kilogram)
			{
				
				return kilogram * 1000;
			}
			public static float GramToTonne(float gram)
			{
				
				return gram / 1000000;
			}
			public static float TonneToGram(float tonne)
			{
				
				return tonne * 1000000;
			}
			
			public static double PoundToGram(float pound)
			{
				
				return 453.59237 * pound;
			}
			public static double GramToPound(float gram)
			{
				
				return 0.0022046 * gram;
			}
			
			public static double GramToOunce(float gram)
			{
				
				return 0.0352739619 * gram;
				
			}
			
			public static double OunceToGram(float ounce)
			{
				
				return 28.35 * ounce;
			}
			
			
			public static float DekagramToKilogram(float dekagram)
			{
				
				return dekagram / 100;
				
			}
			
			public static float KilogramToDekagram(float kilogram)
			{
				
				return kilogram * 100;
			}
			
			public static float DekagramToTonne(float dekagram)
			{
				
				
				return dekagram / 10000;
				
			}
			
			public static float TonneToDekagram(float tonne)
			{
				
				return tonne * 100000;
				
			}
			
			public static double DekagramToPound(float dekagram)
			{
				
				
				return 0.0220462262 * dekagram;
				
			}
			
			public static double PoundToDekagram(float pound)
			{
				
				
				return 45.359237 * pound;
				
			}
			
			public static double DekagramToOunces(float dekagram)
			{
				
				
				return 0.352739619 * dekagram;
				
			}
			
			public static double OuncesToDekagram(float ounces)
			{
				return 2.83495231 * ounces;
			}
			public static float KilogramToTonne(float kilogram)
			{
				return kilogram / 1000;
			}
			public static float TonneToKilogram(float tonne)
			{
				return tonne * 1000;
			}
			public static double KilogramToPound(float kilogram)
			{
				return kilogram * 2.205;
			}
			public static double PoundToKilogram(float pound)
			{
				return pound / 2.205;
			}
			public static double KilogramToOunces(float kilogram)
			{
				return kilogram * 35.2739619;
			}
			public static double OuncesToKilogram(float ounces)
			{
				return ounces * 0.0283495231;
			}
			
			
			
			public static double TonneToPound(float tonne)
			{
				return tonne * 2205;
			}
			public static double PoundToTonne(float pound)
			{
				return pound / 2205;
			}
			public static double TonneToOunces(float tonne)
			{
				return tonne * 35274;
			}
			public static double OuncesToTonne(float ounces)
			{
				return ounces / 35274;
			}
			public static float PoundToOunces(float pound)
			{
				return 16 * pound;
			}
			public static double OuncesToPound(float ounces)
			{
				return 0.0625 * ounces;
			}
		}
	}
}