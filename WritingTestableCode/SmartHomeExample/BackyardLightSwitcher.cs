namespace SmartHomeExample
{
    public sealed class BackyardLightSwitcher 
    {
        private BackyardLightSwitcher() { }

        public static BackyardLightSwitcher Instance { get; } = new BackyardLightSwitcher();

        public void TurnOn() { }
        public void TurnOff() { }
    }




}
