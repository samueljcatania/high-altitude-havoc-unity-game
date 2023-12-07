namespace StealthBomber
{
    /// <summary>
    /// This interface is used to define the PerformAction method that all interactable objects must implement.
    /// </summary>
    public interface ICockpitInteractable
    {
        /// <summary>
        /// This method is called when the player interacts with an object in the cockpit.
        /// </summary>
        void PerformAction();
    }
}