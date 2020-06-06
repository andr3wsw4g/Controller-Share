using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SharpDX.XInput;
using System.Windows.Controls;

namespace Controller_Share
{
    [Serializable]
    //once controller_settings have been sent, this is the inputs that need to be sent over TCP
    class INPUT_STRUCT
    {
        public long Buttons; //8 Byte
        public byte bLeftTrigger; //1 byte
        public byte bRightTrigger; //1 byte
        public short sAxisX; //2 byte
        public short sAxisY; //2 byte
        public short sAxisZ; //2 byte
        public short sAxisRX; //2 byte
        public short sAxisRY; //2 byte
        public short sAxisRZ; //2 byte
        public short POV1; //2 byte
        public short POV2; //2 byte
        public short POV3; //2 byte
        public short POV4; //2 byte
    }
    class INFO_STRUCT
    {
        public int n_buttons;
        public bool axis_X;
        public bool axis_Y;
        public bool axis_Z;
        public bool axis_RX;
        public bool axis_RY;
        public bool axis_RZ;
        public int controller_type;
        public byte n_POV;
        public byte n_DPOV;
    }
}
