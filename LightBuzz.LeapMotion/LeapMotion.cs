using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightBuzz.LeapMotion
{
    /// <summary>
    /// Leap Motion wrapper class, providing a well-exposed managed interface.
    /// </summary>
    public class LeapMotion : Listener, IDisposable
    {
        #region Members

        Controller _controller = new Controller();

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the sensor is initialized.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> Initialized;

        /// <summary>
        /// Occurs when the sensor is un-initialized.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> Exited;

        /// <summary>
        /// Occurs when the sensor is connected.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> Connected;

        /// <summary>
        /// Occurs when the sensor is disconnected.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> Disconnected;

        /// <summary>
        /// Occurs when the sensor gets focus.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> GotFocus;

        /// <summary>
        /// Occurs when the sensor loses focus.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> LostFocus;

        /// <summary>
        /// Occurs when a new frame is available.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> FrameReady;

        /// <summary>
        /// Occurs when a gesture is recognized.
        /// </summary>
        public event EventHandler<LeapMotionEventArgs> GestureRecognized;

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the current Leap Motion sensor is properly connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _controller.IsConnected; }
        }

        /// <summary>
        /// Determines whether the current Leap Motion sensor has focus.
        /// </summary>
        public bool HasFocus
        {
            get { return _controller.HasFocus; }
        }

        #endregion

        #region Constructor

        public LeapMotion()
        {
            _controller.AddListener(this);
        }

        #endregion

        #region Event handlers
        
        public override void OnInit(Controller controller)
        {
            if (Initialized != null)
            {
                Initialized(this, new LeapMotionEventArgs { Controller = controller });
            }
        }

        public override void OnExit(Controller controller)
        {
            if (Exited != null)
            {
                Exited(this, new LeapMotionEventArgs { Controller = controller });
            }
        }

        public override void OnConnect(Controller controller)
        {
            if (Connected != null)
            {
                Connected(this, new LeapMotionEventArgs { Controller = controller });
            }
        }

        public override void OnDisconnect(Controller controller)
        {
            if (Disconnected != null)
            {
                Disconnected(this, new LeapMotionEventArgs { Controller = controller });
            }
        }

        public override void OnFocusGained(Controller controller)
        {
            if (GotFocus != null)
            {
                GotFocus(this, new LeapMotionEventArgs { Controller = controller });
            }
        }

        public override void OnFocusLost(Controller controller)
        {
            if (LostFocus != null)
            {
                LostFocus(this, new LeapMotionEventArgs { Controller = controller });
            }
        }

        public override void OnFrame(Controller controller)
        {
            using (Frame frame = controller.Frame())
            {
                if (frame != null)
                {
                    GestureList gestures = frame.Gestures();

                    if (gestures != null && gestures.Count > 0)
                    {
                        foreach (var gesture in gestures)
                        {
                            if (GestureRecognized != null)
                            {
                                FingerList fingers = frame.Fingers;
                                if(fingers != null && fingers.Count >0)
                                {
                                    foreach(var finger in fingers)
                                    {
                                        GestureRecognized(this, new LeapMotionEventArgs { Controller = controller, Frame = frame, Gesture = gesture , Finger = finger});
                                    }
                                }
                            }
                        }
                    }

                    if (FrameReady != null)
                    {
                        FrameReady(this, new LeapMotionEventArgs { Controller = controller, Frame = frame });
                    }
                }
            }
        }

        public override void Dispose()
        {
            _controller.RemoveListener(this);
            _controller.Dispose();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Enables the specified gesture, so the sensor will recognize it.
        /// </summary>
        /// <param name="type">The gesture type.</param>
        public void EnableGesture(Leap.Gesture.GestureType type)
        {
            _controller.EnableGesture(type);
        }

        /// <summary>
        /// Enables all of the available gestures in the GestureType enum.
        /// </summary>
        public void EnableGestures()
        {
            foreach (Leap.Gesture.GestureType type in Enum.GetValues(typeof(Leap.Gesture.GestureType)))
            {
                _controller.EnableGesture(type);
            }
        }

        #endregion
    }
}
