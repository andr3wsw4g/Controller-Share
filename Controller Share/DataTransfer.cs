using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using vJoyInterfaceWrap;
using System.CodeDom.Compiler;
using SharpDX.XInput;
namespace Controller_Share
{
    class DataTransfer
    {
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ByteArrayToObject<T>(byte [] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        public static INFO_STRUCT GetControllerInfo(vJoy controller, uint id)
        {
            INFO_STRUCT temp = new INFO_STRUCT();
            if (valid_controller_info(controller, id, ref temp))
                return temp;
            return null;
        }

        //checks 
        private static bool valid_controller_info(vJoy controller, uint id, ref INFO_STRUCT info)
        {
            if (id <= 0 || id > 16)
            {
                Console.WriteLine("Bad device ID {0}\nExit!", id);
                return false;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!controller.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }
            else
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", controller.GetvJoyManufacturerString(), controller.GetvJoyProductString(), controller.GetvJoySerialNumberString());

            // Get the state of the requested device
            VjdStat status = controller.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is being used\n", id);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", id);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is being used \nCannot continue\n", id);
                    return false;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                    return false;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                    return false;
            };
            info.axis_X = controller.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            info.axis_Y = controller.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            info.axis_Z = controller.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            info.axis_RX = controller.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            info.axis_RY = controller.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RY);
            info.axis_RZ = controller.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            info.n_buttons = controller.GetVJDButtonNumber(id);
            info.n_POV = (byte)controller.GetVJDContPovNumber(id);
            info.n_DPOV = (byte)controller.GetVJDDiscPovNumber(id);
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!controller.AcquireVJD(id))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                return false;
            }
            return true;
        }

        public static INPUT_STRUCT GetControllerInput(INFO_STRUCT info, Gamepad controller, uint id)
        {
            INPUT_STRUCT temp = new INPUT_STRUCT();

            //axis logic
            if (info.axis_X)
            {
                temp.sAxisX = controller.LeftThumbX;
            }
            if (info.axis_Y)
            {
                temp.sAxisY = controller.LeftThumbY;
            }
            if (info.axis_Z)
            {
                temp.sAxisZ = controller.LeftTrigger;
            }
            if (info.axis_RX)
            {
                temp.sAxisRX = controller.RightThumbX;
            }
            if (info.axis_RY)
            {
                temp.sAxisRY = controller.RightThumbY;
            }
            if (info.axis_RZ)
            {
                temp.sAxisRZ = controller.RightTrigger;
            }

            temp.Buttons = (long)controller.Buttons;

            return null;
        }



        public long XinputButtonToVJoy(GamepadButtonFlags buttons)
        {

        }
    }
}
