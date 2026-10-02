using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Engine.OpenGL;

// 2D-Textur auf der GPU. Die Pixelwerte werden unverändert gespeichert (RGBA, 8 Bit pro Kanal);
// eine Farbraumumrechnung (sRGB/Gamma) findet vorerst nicht statt.
public sealed class Texture : IDisposable
{
    private int _handle;

    public int Width { get; }
    public int Height { get; }

    internal int Handle
    {
        get
        {
            if (_handle == 0) throw new ObjectDisposedException(nameof(Texture));
            return _handle;
        }
    }

    private Texture(int width, int height, byte[] rgbaPixels)
    {
        Width = width;
        Height = height;
        _handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _handle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, rgbaPixels);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    // Lädt PNG, JPG, BMP, TGA u. a. Bilddateien liegen oben links beginnend vor, OpenGL erwartet
    // die erste Zeile unten – deshalb wird beim Laden vertikal gespiegelt.
    public static Texture FromFile(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        using FileStream stream = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        return new Texture(image.Width, image.Height, image.Data);
    }

    public void Dispose()
    {
        if (_handle == 0) return;
        GL.DeleteTexture(_handle);
        _handle = 0;
    }
}
