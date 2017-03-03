using UnityEngine;

namespace SimpleWars.Parts
{
    interface BuilderExtension
    {
        VehicleControl vehicle { get; set; }
        bool detached { get; set; }

        void Initialize (Vehicle.Part partType);

        void Render ();

        void Removed ();
    }
}
