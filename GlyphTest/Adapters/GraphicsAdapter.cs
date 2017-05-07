// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using NBidi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.WPF.Utilities;

namespace TheArtOfDev.HtmlRenderer.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF Graphics.
    /// </summary>
    public sealed class GraphicsAdapter : RGraphics
    {
        #region Fields and Consts

        /// <summary>
        /// The wrapped WPF graphics object
        /// </summary>
        private readonly DrawingContext _g;

        /// <summary>
        /// if to release the graphics object on dispose
        /// </summary>
        private readonly bool _releaseGraphics;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="g">the WPF graphics object to use</param>
        /// <param name="initialClip">the initial clip of the graphics</param>
        /// <param name="releaseGraphics">optional: if to release the graphics object on dispose (default - false)</param>
        public GraphicsAdapter(DrawingContext g, RRect initialClip, bool releaseGraphics = false)
            : base(WpfAdapter.Instance, initialClip)
        {
            ArgChecker.AssertArgNotNull(g, "g");

            _g = g;
            _releaseGraphics = releaseGraphics;
        }
        string Direction;
        public void setDirection(string direction)
        {
            Direction = direction;
        }
        /// <summary>
        /// Init.
        /// </summary>
        public GraphicsAdapter()
            : base(WpfAdapter.Instance, RRect.Empty)
        {
            _g = null;
            _releaseGraphics = false;
        }

        public override void PopClip()
        {
            _g.Pop();
            _clipStack.Pop();
        }

        public override void PushClip(RRect rect)
        {
            _clipStack.Push(rect);
            _g.PushClip(new RectangleGeometry(Utils.Convert(rect)));
        }

        public override void PushClipExclude(RRect rect)
        {
            var geometry = new CombinedGeometry();
            geometry.Geometry1 = new RectangleGeometry(Utils.Convert(_clipStack.Peek()));
            geometry.Geometry2 = new RectangleGeometry(Utils.Convert(rect));
            geometry.GeometryCombineMode = GeometryCombineMode.Exclude;

            _clipStack.Push(_clipStack.Peek());
            _g.PushClip(geometry);
        }

        public override Object SetAntiAliasSmoothingMode()
        {
            return null;
        }

        public override void ReturnPreviousSmoothingMode(Object prevMode)
        { }

        public override RSize MeasureString(string str, RFont font)
        {
            //var str = Reform(str);
            double width = 0;
            GlyphTypeface glyphTypeface = ((FontAdapter)font).GlyphTypeface;
            if (glyphTypeface != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (glyphTypeface.CharacterToGlyphMap.ContainsKey(str[i]))
                    {
                        ushort glyph = glyphTypeface.CharacterToGlyphMap[str[i]];
                        double advanceWidth = glyphTypeface.AdvanceWidths[glyph];
                        width += advanceWidth;
                    }
                    else
                    {
                        width = 0;
                        break;
                    }
                }
            }
            ////////////////////////
            
            if (width <= 0 && Direction=="rtl")
            {
                var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.RightToLeft, ((FontAdapter)font).Font, 96d / 72d * font.Size, Brushes.Red);
                return new RSize(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
            }
            else if(width <= 0 && Direction == "ltr")
            {
                var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, ((FontAdapter)font).Font, 96d / 72d * font.Size, Brushes.Red);
                return new RSize(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
            }

            return new RSize(width * font.Size * 96d / 72d, font.Height);
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            //var str = Reform(str);
            charFit = 0;
            charFitWidth = 0;
            bool handled = false;
            GlyphTypeface glyphTypeface = ((FontAdapter)font).GlyphTypeface;
            if (glyphTypeface != null)
            {
                handled = true;
                double width = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (glyphTypeface.CharacterToGlyphMap.ContainsKey(str[i]))
                    {
                        ushort glyph = glyphTypeface.CharacterToGlyphMap[str[i]];
                        double advanceWidth = glyphTypeface.AdvanceWidths[glyph] * font.Size * 96d / 72d;

                        if (!(width + advanceWidth < maxWidth))
                        {
                            charFit = i;
                            charFitWidth = width;
                            break;
                        }
                        width += advanceWidth;
                    }
                    else
                    {
                        handled = false;
                        break;
                    }
                }
            }

            if (!handled)
            {
                var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, ((FontAdapter)font).Font, 96d / 72d * font.Size, Brushes.Red);
                charFit = str.Length;
                charFitWidth = formattedText.WidthIncludingTrailingWhitespace;
            }
        }

        public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
        {
            //var strs = Reform(str, rtl);
            var colorConv = ((BrushAdapter)_adapter.GetSolidBrush(color)).Brush;

            bool glyphRendered = false;
            GlyphTypeface glyphTypeface = ((FontAdapter)font).GlyphTypeface;
            if (glyphTypeface != null)
            {
                double extraWidth = 0;
                double width = 0;
                ushort[] glyphs = new ushort[str.Length];
                double[] widths = new double[str.Length];

                int i = 0;
                for (; i < str.Length; i++)
                {
                    ushort glyph;
                    if (!glyphTypeface.CharacterToGlyphMap.TryGetValue(str[i], out glyph))
                        break;

                    glyphs[i] = glyph;
                    width += glyphTypeface.AdvanceWidths[glyph];
                    extraWidth += glyphTypeface.LeftSideBearings[glyph] + glyphTypeface.RightSideBearings[glyph];
                    widths[i] = 96d / 72d * font.Size * glyphTypeface.AdvanceWidths[glyph];
                }

                if (i >= str.Length)
                {
                    point.Y += glyphTypeface.Baseline * font.Size * 96d / 72d;
                    point.X += rtl ? 96d / 72d * font.Size * width /*(width - extraWidth)*/: 0;

                    glyphRendered = true;
                    var glyphRun = new GlyphRun(glyphTypeface, rtl ? 1 : 0, false, 96d / 72d * font.Size, glyphs, Utils.ConvertRound(point), widths, null, null, null, null, null, null);
                    _g.DrawGlyphRun(colorConv, glyphRun);
                }
            }

            if (!glyphRendered)
            {
                var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, rtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight, ((FontAdapter)font).Font, 96d / 72d * font.Size, colorConv);
                point.X += rtl ? formattedText.Width : 0;
                _g.DrawText(formattedText, Utils.ConvertRound(point));
            }
        }

        // این متد کار همان متدی را می کند که در 
        // mainWindow.cs
        // وجود دارد، تنها تفاوت در ورودی متد است که در اینجا کلمه به کلمه تبدیل می کند و در فرم اصلی کل جمله را تبدیل می کند
        // مشکل در اینجاست که ترتیب کلمات انگلیسی به هم می خورد
        private char[] Reform(string input, bool rtl = true)
        {
            if (input.Length == 0)
                return null;

            string sentence = input;
            var glyphChars = new List<char>();

            Stack<char> asciiCharacterStack = new Stack<char>();

            //NBidi.UnicodeGeneralCategory.

            foreach (var word in sentence.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
            {
                //NBidi.BidiCharacterType.

                var preCharacterJoiningType = ArabicShapeJoiningType.U;

                for (int i = 0; i < word.Length; i++)
                {
                    var currentCharacter = word[i];
                    if (currentCharacter < 128)
                    {
                        asciiCharacterStack.Push(currentCharacter);
                        preCharacterJoiningType = ArabicShapeJoiningType.U;
                    }
                    else
                    {
                        while (asciiCharacterStack.Count > 0)
                        {
                            glyphChars.Add(asciiCharacterStack.Pop());
                        }
                        var currentCharacterJoiningType = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(currentCharacter);

                        if (preCharacterJoiningType == ArabicShapeJoiningType.D || preCharacterJoiningType == ArabicShapeJoiningType.L)
                        {
                            if (i == word.Length - 1)
                            {
                                glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));
                            }
                            else
                            {
                                var nextCharacter = word[i + 1];
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Medial));
                                else
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));
                            }
                        }
                        else
                        {
                            if (i == word.Length - 1)
                            {
                                glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));
                            }
                            else
                            {
                                var nextCharacter = word[i + 1];
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Initial));
                                else
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));
                            }
                        }

                        preCharacterJoiningType = currentCharacterJoiningType;
                    }
                }

                //if (asciiCharacterStack.Count > 0)
                //    asciiCharacterStack.Push(' ');

                //glyphChars.Add(' ');
            }

            while (asciiCharacterStack.Count > 0)
            {
                glyphChars.Add(asciiCharacterStack.Pop());
            }

            //var finalOutput = "";

            //finalOutput = new string(glyphChars.ToArray());
            if (!rtl)
                glyphChars.Reverse();

            return glyphChars.ToArray();
        }


        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            var brush = new ImageBrush(((ImageAdapter)image).Image);
            brush.Stretch = Stretch.None;
            brush.TileMode = TileMode.Tile;
            brush.Viewport = Utils.Convert(dstRect);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Transform = new TranslateTransform(translateTransformLocation.X, translateTransformLocation.Y);
            brush.Freeze();
            return new BrushAdapter(brush);
        }

        public override RGraphicsPath GetGraphicsPath()
        {
            return new GraphicsPathAdapter();
        }

        public override void Dispose()
        {
            if (_releaseGraphics)
                _g.Close();
        }


        #region Delegate graphics methods

        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
        {
            x1 = (int)x1;
            x2 = (int)x2;
            y1 = (int)y1;
            y2 = (int)y2;

            var adj = pen.Width;
            if (Math.Abs(x1 - x2) < .1 && Math.Abs(adj % 2 - 1) < .1)
            {
                x1 += .5;
                x2 += .5;
            }
            if (Math.Abs(y1 - y2) < .1 && Math.Abs(adj % 2 - 1) < .1)
            {
                y1 += .5;
                y2 += .5;
            }

            _g.DrawLine(((PenAdapter)pen).CreatePen(), new Point(x1, y1), new Point(x2, y2));
        }

        public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
        {
            var adj = pen.Width;
            if (Math.Abs(adj % 2 - 1) < .1)
            {
                x += .5;
                y += .5;
            }
            
            _g.DrawRectangle(null, ((PenAdapter)pen).CreatePen(), new Rect(x, y, width, height));
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
        {
            _g.DrawRectangle(((BrushAdapter)brush).Brush, null, new Rect(x, y, width, height));
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
        {
            CroppedBitmap croppedImage = new CroppedBitmap(((ImageAdapter)image).Image, new Int32Rect((int)srcRect.X, (int)srcRect.Y, (int)srcRect.Width, (int)srcRect.Height));
            _g.DrawImage(croppedImage, Utils.ConvertRound(destRect));
        }

        public override void DrawImage(RImage image, RRect destRect)
        {
            _g.DrawImage(((ImageAdapter)image).Image, Utils.ConvertRound(destRect));
        }

        public override void DrawPath(RPen pen, RGraphicsPath path)
        {
            _g.DrawGeometry(null, ((PenAdapter)pen).CreatePen(), ((GraphicsPathAdapter)path).GetClosedGeometry());
        }

        public override void DrawPath(RBrush brush, RGraphicsPath path)
        {
            _g.DrawGeometry(((BrushAdapter)brush).Brush, null, ((GraphicsPathAdapter)path).GetClosedGeometry());
        }

        public override void DrawPolygon(RBrush brush, RPoint[] points)
        {
            if (points != null && points.Length > 0)
            {
                var g = new StreamGeometry();
                using (var context = g.Open())
                {
                    context.BeginFigure(Utils.Convert(points[0]), true, true);
                    for (int i = 1; i < points.Length; i++)
                        context.LineTo(Utils.Convert(points[i]), true, true);
                }
                g.Freeze();

                _g.DrawGeometry(((BrushAdapter)brush).Brush, null, g);
            }
        }

        #endregion
    }
}