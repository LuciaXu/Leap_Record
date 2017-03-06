using Leap;
using LightBuzz.LeapMotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LeapMotionTest
{
    class Program
    {
        static string path;
        static int num;
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            path = now.ToString("yyMMddHmmss");
            Directory.CreateDirectory(path);
            num = 0;
            //using (System.IO.BinaryReader br =
            //new System.IO.BinaryReader(System.IO.File.Open(path+"//leap.data", System.IO.FileMode.Open)))
            //{
            //    while (br.BaseStream.Position < br.BaseStream.Length)
            //    {
            //        Int32 nextBlock = br.ReadInt32();
            //        byte[] frameData = br.ReadBytes(nextBlock);
            //        Leap.Frame newFrame = new Leap.Frame();
            //        newFrame.Deserialize(frameData);
            //    }
            //}


            LeapMotion leap = new LeapMotion();

            leap.EnableGestures();

            leap.FrameReady += Leap_FrameReady;
           // leap.GestureRecognized += Leap_GestureRecognized;

            Console.ReadKey();
        }

        static void Leap_FrameReady(object sender, LeapMotionEventArgs e)
        {
            Console.WriteLine("Number of fingers detected: " + e.Frame.Fingers.Count);
            Leap.Frame newframe = e.Frame;
            LeapSave(newframe,num);
            num++;
        }

        static void Leap_GestureRecognized(object sender, LeapMotionEventArgs e)
        {
            Console.WriteLine("Gesture detected: " + e.Gesture.Type);
            Console.WriteLine("Finger position x: "+ e.Finger.StabilizedTipPosition.x);
            Console.WriteLine("Finger position y: " + e.Finger.StabilizedTipPosition.y);
            Console.WriteLine("Finger position z: " + e.Finger.StabilizedTipPosition.z);
        }

        static void LeapSave(Leap.Frame frame, int number)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "//leap_" + number + ".txt"))
            {
                DateTime current = DateTime.Now;
                string leap_time = current.ToString("yyMMddHmmssfff");
                file.WriteLine("Frame id: " + frame.Id
                    + ", timestamp: " + leap_time
                    + ", hands: " + frame.Hands.Count
                    + ", fingers: " + frame.Fingers.Count
                    + ", tools: " + frame.Tools.Count
                    + ", gestures: " + frame.Gestures().Count);
                foreach (Hand hand in frame.Hands)
                {
                    file.WriteLine("  Hand id: " + hand.Id
                                + ", palm position: " + hand.PalmPosition
                                + ", palm nomal:" + hand.PalmNormal);
                    // Get the hand's normal vector and direction
                    Leap.Vector normal = hand.PalmNormal;
                    Leap.Vector direction = hand.Direction;

                    // Calculate the hand's pitch, roll, and yaw angles
                    file.WriteLine("  Hand pitch: " + direction.Pitch * 180.0f / (float)Math.PI + " degrees, "
                                + "roll: " + normal.Roll * 180.0f / (float)Math.PI + " degrees, "
                                + "yaw: " + direction.Yaw * 180.0f / (float)Math.PI + " degrees");

                    // Get the Arm bone
                    Arm arm = hand.Arm;
                    file.WriteLine("  Arm direction: " + arm.Direction
                                + ", wrist position: " + arm.WristPosition
                                + ", elbow position: " + arm.ElbowPosition);

                    // Get fingers
                    foreach (Finger finger in hand.Fingers)
                    {
                        file.WriteLine("    Finger id: " + finger.Id
                                    + ", " + finger.Type.ToString()
                                    + ", length: " + finger.Length
                                    + "mm, width: " + finger.Width + "mm"
                                    + ", tip position: " + finger.TipPosition);

                        // Get finger bones
                        Bone bone;
                        foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
                        {
                            bone = finger.Bone(boneType);
                            file.WriteLine("      Bone: " + boneType
                                        + ", start: " + bone.PrevJoint
                                        + ", end: " + bone.NextJoint
                                        + ", direction: " + bone.Direction);
                        }
                    }

                }

                // Get tools
                foreach (Tool tool in frame.Tools)
                {
                    file.WriteLine("  Tool id: " + tool.Id
                                + ", position: " + tool.TipPosition
                                + ", direction " + tool.Direction);
                }

                // Get gestures
                GestureList gestures = frame.Gestures();
                for (int i = 0; i < gestures.Count; i++)
                {
                    Gesture gesture = gestures[i];

                    switch (gesture.Type)
                    {
                        case Gesture.GestureType.TYPE_CIRCLE:
                            CircleGesture circle = new CircleGesture(gesture);

                            // Calculate clock direction using the angle between circle normal and pointable
                            String clockwiseness;
                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 2)
                            {
                                //Clockwise if angle is less than 90 degrees
                                clockwiseness = "clockwise";
                            }
                            else
                            {
                                clockwiseness = "counterclockwise";
                            }

                            float sweptAngle = 0;

                            // Calculate angle swept since last frame
                            //if (circle.State != Gesture.GestureState.STATE_START)
                            //{
                            //    CircleGesture previousUpdate = new CircleGesture(controller.Frame(1).Gesture(circle.Id));
                            //    sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;
                            //}

                            //file.WriteLine("  Circle id: " + circle.Id
                            //               + ", " + circle.State
                            //               + ", progress: " + circle.Progress
                            //               + ", radius: " + circle.Radius
                            //               + ", angle: " + sweptAngle
                            //               + ", " + clockwiseness);
                            break;
                        case Gesture.GestureType.TYPE_SWIPE:
                            SwipeGesture swipe = new SwipeGesture(gesture);
                            file.WriteLine("  Swipe id: " + swipe.Id
                                           + ", " + swipe.State
                                           + ", position: " + swipe.Position
                                           + ", direction: " + swipe.Direction
                                           + ", speed: " + swipe.Speed);
                            break;
                        case Gesture.GestureType.TYPE_KEY_TAP:
                            KeyTapGesture keytap = new KeyTapGesture(gesture);
                            file.WriteLine("  Tap id: " + keytap.Id
                                           + ", " + keytap.State
                                           + ", position: " + keytap.Position
                                           + ", direction: " + keytap.Direction);
                            break;
                        case Gesture.GestureType.TYPE_SCREEN_TAP:
                            ScreenTapGesture screentap = new ScreenTapGesture(gesture);
                            file.WriteLine("  Tap id: " + screentap.Id
                                           + ", " + screentap.State
                                           + ", position: " + screentap.Position
                                           + ", direction: " + screentap.Direction);
                            break;
                        default:
                            file.WriteLine("  Unknown gesture type.");
                            break;
                    }
                }

                if (!frame.Hands.IsEmpty || !frame.Gestures().IsEmpty)
                {
                    file.WriteLine("");
                }
            }
        }
    }

}
