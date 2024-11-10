using System;
using Tao.Sdl;

public class Image
{
    public IntPtr Pointer { get; private set; }

    public Image(string imagePath)
    {
        LoadImage(imagePath);
    }

    private void LoadImage(string imagePath)
    {
        Pointer = SdlImage.IMG_Load(imagePath);
        if (Pointer == IntPtr.Zero)
        {
            Console.WriteLine("Imagen inexistente: {0}", imagePath);
            Environment.Exit(4);
        }
    }
}