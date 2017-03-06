using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightBuzz.LeapMotion
{
    /// <summary>
    /// Exposes a Leap Motion event argument.
    /// </summary>
    public class LeapMotionEventArgs : EventArgs
    {
        /// <summary>
        /// The Leap Motion Controller that fired the event.
        /// </summary>
        public Controller Controller { get; set; }

        /// <summary>
        /// The available Frame, or null.
        /// </summary>
        public Frame Frame { get; set; }

        /// <summary>
        /// The recognized gesture, or null.
        /// </summary>
        public Gesture Gesture { get; set; }

        public Finger Finger { get; set; }

        /// <summary>
        /// Any exception raised during the event.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
