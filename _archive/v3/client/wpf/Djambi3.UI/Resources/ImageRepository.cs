using System;
using System.IO;
using System.Windows.Media.Imaging;
using Djambi.Model;

namespace Djambi.UI.Resources
{
    public interface IImageRepository
    {
        BitmapImage AppIcon { get; }
        BitmapImage GetPieceImage(Piece piece);
    }

    class ImageRepository : IImageRepository
    {
        private readonly IThemeLoader _themeLoader;

        private BitmapImage _assassin;
        private BitmapImage _chief;
        private BitmapImage _corpse;
        private BitmapImage _diplomat;
        private BitmapImage _journalist;
        private BitmapImage _thug;
        private BitmapImage _undertaker;

        public ImageRepository(IThemeLoader themeLoader)
        {
            _themeLoader = themeLoader;
            _themeLoader.ThemeLoaded += Clear;
        }

        private void Clear()
        {
            _assassin = null;
            _chief = null;
            _corpse = null;
            _diplomat = null;
            _journalist = null;
            _thug = null;
            _undertaker = null;
        }

        private string ImagesDirectory =>
            $"{Directory.GetCurrentDirectory()}/Resources/{_themeLoader.Theme.ThemeName}";

        private BitmapImage GetImage(string imageName) =>
            new BitmapImage(new Uri($"{ImagesDirectory}/{imageName}.png"));

        private BitmapImage Assassin => _assassin is null ? _assassin = GetImage("assassin") : _assassin;
        private BitmapImage Chief => _chief is null ? _chief = GetImage("chief") : _chief;
        private BitmapImage Diplomat => _diplomat is null ? _diplomat = GetImage("diplomat") : _diplomat;
        private BitmapImage Journalist => _journalist is null ? _journalist = GetImage("journalist") : _journalist;
        private BitmapImage Thug => _thug is null ? _thug = GetImage("thug") : _thug;
        private BitmapImage Undertaker => _undertaker is null ? _undertaker = GetImage("undertaker") : _undertaker;
        private BitmapImage Corpse => _corpse is null ? _corpse = GetImage("corpse") : _corpse;

        public BitmapImage GetPieceImage(Piece piece) =>
            piece.IsAlive
                ? piece.Type switch
                    {
                        PieceType.Assassin => Assassin,
                        PieceType.Chief => Chief,
                        PieceType.Diplomat => Diplomat,
                        PieceType.Journalist => Journalist,
                        PieceType.Thug => Thug,
                        PieceType.Undertaker => Undertaker,
                        _ => throw new Exception($"Invalid {nameof(PieceType)} value ({piece.Type})."),
                    }
                : Corpse;

        public BitmapImage AppIcon => Chief;
    }
}
