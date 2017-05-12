namespace ThereMustBeAnotherWay.Misc.Interfaces
{
    /// <summary>
    /// Laws of gravity apply.
    /// </summary>
    interface INewtonian
    {
        /// <summary>
        /// The value of gravity. Set it to 0 for default.
        /// </summary>
        float GravityStrength { get; set; }

        /// <summary>
        /// Returns true if entity is flying.
        /// </summary>
        bool IsFlying { get; set; }

        /// <summary>
        /// Returns true if entity is jumping.
        /// </summary>
        bool IsJumping { get; set; }
    }
}
