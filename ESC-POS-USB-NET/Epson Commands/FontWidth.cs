using ESC_POS_USB_NET.Extensions;
using ESC_POS_USB_NET.Interfaces.Command;

namespace ESC_POS_USB_NET.EpsonCommands
{
    public class FontWidth : IFontWidth
    {
        public byte[] Normal()
        {
            return new byte[] { 27, '!'.ToByte(), 0 };
        }

        public byte[] DoubleWidth2()
        {
            return new byte[] { 29, '!'.ToByte(), 16 };
        }

        public byte[] DoubleWidth3()
        {
            return new byte[] { 29, '!'.ToByte(), 32 };
        }


        // additional methods
        

        public byte[] DoubleHeight()
        {
            return new byte[] { 27, '!'.ToByte(), 16 }; // Double height
        }

        public byte[] DoubleWidth()
        {
            return new byte[] { 27, '!'.ToByte(), 32 }; // Double width
        }

        public byte[] DoubleSize()
        {
            return new byte[] { 27, '!'.ToByte(), 48 }; // Double height + width
        }

        public byte[] Scale2xHeight()
        {
            return new byte[] { 29, '!'.ToByte(), 1 };  // Corrected (00000001 = 2x height)
        }

        public byte[] Scale2x()
        {
            return new byte[] { 29, '!'.ToByte(), 17 }; // Corrected (00010001 = 2x both)
        }

        public byte[] Scale(int widthMultiplier, int heightMultiplier)
        {
            // Input validation (1-8x)
            byte width = (byte)((widthMultiplier - 1) << 4);  // High nibble
            byte height = (byte)(heightMultiplier - 1);        // Low nibble
            return new byte[] { 29, '!'.ToByte(), (byte)(width | height) };
        }


    }
}

