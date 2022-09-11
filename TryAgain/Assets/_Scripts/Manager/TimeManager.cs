using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace _Scripts.Manager
{
    public static class TimeManager
    {
        private static readonly List<Timer> Timers = new List<Timer>();

        public static float TimePassedFor(object o)
        {
            var timer = Timers.Find(t => t.Agent == o);
            var timePassed = 0f;
            if (timer is null)
            {
                Timers.Add(new Timer(o));
                return 0;
            }

            timePassed = timer.TimePassed;
            timer.TimePassed = 0;

            return timePassed;
        }

        public static void PassTime(float timePassed)
        {
            foreach (var timer in Timers)
            {
                timer.TimePassed += timePassed;
            }
        }

        private class Timer
        {
            public readonly object Agent;
            public float TimePassed;

            public Timer(object agent)
            {
                Agent = agent;
                TimePassed = 0;
            }
        }
    }
}