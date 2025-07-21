namespace ESC_POS_USB_NET.Interfaces.Command
{
    internal interface IFontWidth
    {
        byte[] Normal();
        byte[] DoubleWidth2();
        byte[] DoubleWidth3();

        // additional methods
        byte[] Small();
        byte[] DoubleHeight();
        byte[] DoubleWidth();
        byte[] DoubleSize();
        byte[] Scale2xWidth();
        byte[] Scale2xHeight();
        byte[] Scale2x();
        byte[] Scale(int widthMultiplier, int heightMultiplier);
    }
}

