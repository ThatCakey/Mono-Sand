using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Falling_sand;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    //scaling factor for rendering of application, some values big borked but... just don't use those
    static float scalefactor = 1.3f;

    //amount of colors in the gradient, more values means the particles change color slower, do not make larget than 63999
    static int colors = 500;

    public GridObject MainGrid;

    //size of the grid in rows and colums
    int width = 100;
    int height = 50;

    //the size in pixels of each element of the grid
    int pixelwidth = 10;
    int pixelheight = 10;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        //sets the window size according to the amount of grid elements and pixel size of each element including scaling

        _graphics.PreferredBackBufferHeight = (int)Math.Round(height * pixelheight * scalefactor);
        _graphics.PreferredBackBufferWidth = (int)Math.Round(width * pixelwidth * scalefactor);

    }


    protected override void Initialize()
    {
        MainGrid = GridObject.Initialize(width, height);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    int particlecolor = 1;
    Vector2 offset = new Vector2();

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        //run the gravity funciton in the grid, and some code to spawn and destroy particles

        MainGrid.gravitycalc();
        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
        {

            //spawn particles with colors based on the range defined by colors < 64000
            if (MainGrid.saferead((int)offset.X, (int)offset.Y) == 0)
            {
                MainGrid.safewrite((int)offset.X, (int)offset.Y, particlecolor);
                particlecolor++;
                if (particlecolor > colors) particlecolor = 1;
            }
        }
        if (Mouse.GetState().RightButton == ButtonState.Pressed)
        {
            if (MainGrid.saferead((int)offset.X, (int)offset.Y) < 64000)
                MainGrid.safewrite((int)offset.X, (int)offset.Y, 0);
        }

        int mousepx = Mouse.GetState().X;
        mousepx = Math.Clamp(mousepx, 0, _graphics.PreferredBackBufferWidth);

        int mousepy = Mouse.GetState().Y;
        mousepy = Math.Clamp(mousepy, 0, _graphics.PreferredBackBufferHeight);

        int mousegrid = mousepx / (int)Math.Round(pixelwidth * scalefactor);
        int mousegridy = mousepy / (int)Math.Round(pixelheight * scalefactor);
        Debug.WriteLine(mousegrid);
        offset.X = mousegrid;
        offset.Y = mousegridy;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightGray);

        _spriteBatch.Begin();

        //draws the grid and scales the texture to screen size

        Texture2D filled = new Texture2D(_graphics.GraphicsDevice, (int)Math.Round(pixelwidth * scalefactor), (int)Math.Round(pixelheight * scalefactor));
        int Texwidth = filled.Width;
        int Texheight = filled.Height;

        Color[] whitePixels = new Color[Texwidth * Texheight];
        for (int i = 0; i < whitePixels.Length; i++)
        {
            whitePixels[i] = Color.White;
        }

        filled.SetData(whitePixels);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (MainGrid.grid[j, i] != 0)
                {
                    _spriteBatch.Draw(filled, new Vector2(j, i) * Texheight, ParticleColor(MainGrid.saferead(j, i)));
                }
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    //handle the color of a particle based on its integer state, 0 is empty, 1-63999 are reserved for colors, and anything above that is reserved for special cases
    Color ParticleColor(int p)
    {
        switch (p)
        {
            case 64000: return Color.DarkGray;

            default: return GetHslColor(p);
        }

    }


    //following is some deepseek ass color functions because i don't f##k with hsl, but they work so we ball

    public static Color GetHslColor(int value)
    {
        // Ensure value is within range (1-63000)
        value = MathHelper.Clamp(value, 1, colors);

        // Normalize to 0-1 range (0 and 1 both being red)
        float hue = (value - 1) / (float)colors;

        // Convert HSL to RGB (with full saturation and lightness)
        return HslToRgb(hue, 1f, 0.5f);
    }

    private static Color HslToRgb(float h, float s, float l)
    {
        float r, g, b;

        if (s == 0f)
        {
            r = g = b = l; // Grayscale
        }
        else
        {
            float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
            float p = 2 * l - q;
            r = HueToRgb(p, q, h + 1f / 3f);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1f / 3f);
        }

        return new Color(r, g, b);
    }

    private static float HueToRgb(float p, float q, float t)
    {
        if (t < 0f) t += 1f;
        if (t > 1f) t -= 1f;
        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }

}
