using System;

namespace SmartHomeExample
{
    public class SmartHomeController
    {

        public DateTime LastMotionTime { get; private set; }

        public SmartHomeController()  { }
              
    
        #region bad API
        /**
         * What's wrong with this method?
         * 1. Tightly coupled to the concrete data source which is the primary root of most testability problems
         * 
         * 2. Violates the Single Responsibility Principle (SRP). 
         *  The method has both consumes the information and also processes it. 
         *  that method has 2 reason to change. 
         *  
         * 3. Hidden Dependencies. Method's signature doesn't explicit express dependencies.
         *  Developer has to read the method's source code to understand all dependencies
         *  
         * 4. unpredicatable and hard to maintain.
         *   mutable global state makes behaviour dependant on system time's this is an example but 
         *   real world example makes a real headache to decipher what's going on
         *  
         * 
         * It's ok to have static on calculation.
         * 
         */
        public TimeOfDay GetTimeOfDay()
        {
            var dateTime = DateTime.Now;

            if (dateTime.Hour >= 0 && dateTime.Hour < 6)
            {
                return TimeOfDay.Night;
            }
            if (dateTime.Hour >= 6 && dateTime.Hour < 12)
            {
                return TimeOfDay.Morning;
            }
            if (dateTime.Hour >= 12 && dateTime.Hour < 18)
            {
                return TimeOfDay.Afternoon;
            }
            return TimeOfDay.Evening;
        }

        #endregion

        #region improved API

        public TimeOfDay GetTimeOfDay(DateTime dateTime)
        {
            if (dateTime.Hour >= 0 && dateTime.Hour < 6)
            {
                return TimeOfDay.Night;
            }
            if (dateTime.Hour >= 6 && dateTime.Hour < 12)
            {
                return TimeOfDay.Morning;
            }
            if (dateTime.Hour >= 12 && dateTime.Hour < 18)
            {
                return TimeOfDay.Afternoon;
            }
            return TimeOfDay.Evening;
        }

        #endregion

        public void ActuateLights(bool motionDetected)
        {
            /**
             * Hidden DateTime Dependencies was pushed to the GetDateTime's client.
             * So what to do? we could push it up but that won't ultimately solve the problem
             */
            DateTime time = DateTime.Now; // Ouch!

            // Update the time of last motion.
            if (motionDetected)
            {
                LastMotionTime = time;
            }

            // If motion was detected in the evening or at night, turn the light on.
            var timeOfDay = GetTimeOfDay(time);
            if (motionDetected && (timeOfDay == TimeOfDay.Evening || timeOfDay == TimeOfDay.Night))
            {
                BackyardLightSwitcher.Instance.TurnOn();
            }
            // If no motion is detected for one minute, or if it is morning or day, turn the light off.
            else if (time.Subtract(LastMotionTime) > TimeSpan.FromMinutes(1) ||
                    (timeOfDay == TimeOfDay.Morning || timeOfDay == TimeOfDay.Afternoon))
            {
                BackyardLightSwitcher.Instance.TurnOff();
            }

            /**
             * What's wrong here?
             * Responsibility to turn on or off light is delegated to BackyardLightSwitcher which is 
             * a Singleton:
             * 
             * Unit testing the scenario requires a interaction based unit test which would become 
             * more like an integration test due to the hard static dependencies on BackyardLightSwitcher.Instance
             *  
             * 1. Tightly coupled to the concrete hard coded implementation of BackyardLightSwitcher,
             * Cannot switch any other lights than the backyard.
             * 
             * 2. Violates the Single Responsibility Principle (SRP). 
             *  The API has two reasons to change: 
             *   a) changes to the internal logic or feature 
             *      (such as choosing to make the light turn on only at night, but not in the evening) 
             *   b) replacing the light-switching mechanism with another one.
             *   
             * 3. Hidden Dependencies. Method's signature doesn't explicit express dependencies.
             *  Developer has to read the method's source code to understand all dependencies
             *  
             * 4. unpredicatable and hard to maintain.
             *   Bugs could come from BackyardLightSwitcher which can take a while to be identified
             */
        }
               
        #region Fully testable higher function 

        public void ActuateLights(bool motionDetected, Action turnOn, Action turnOff)
        {
            /**
             * Delegate responsability to obtaining date/time to abstraction.
             */
            DateTime time = _dateTimeProvider.Now;

            // Update the time of last motion.
            if (motionDetected)
            {
                LastMotionTime = time;
            }

            // If motion was detected in the evening or at night, turn the light on.
            var timeOfDay = GetTimeOfDay(time);
            if (motionDetected && (timeOfDay == TimeOfDay.Evening || timeOfDay == TimeOfDay.Night))
            {
                turnOn();
            }
            // If no motion is detected for one minute, or if it is morning or day, turn the light off.
            else if (time.Subtract(LastMotionTime) > TimeSpan.FromMinutes(1) ||
                    (timeOfDay == TimeOfDay.Morning || timeOfDay == TimeOfDay.Afternoon))
            {
                turnOff();
            }
        }

        #endregion

        private readonly IDateTimeProvider _dateTimeProvider;



    }




}
