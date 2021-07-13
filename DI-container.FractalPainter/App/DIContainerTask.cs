using System;
using System.Drawing;
using System.Linq;
using FractalPainting.App.Fractals;
using FractalPainting.Infrastructure.Common;
using FractalPainting.Infrastructure.UiActions;
using Ninject;
using Ninject.Extensions.Factory;

namespace FractalPainting.App
{
    public static class DIContainerTask
    {
        public static MainForm CreateMainForm()
        {
            var container = ConfigureContainer();
            var saveImageAction =
                new SaveImageAction(container.Get<PictureBoxImageHolder>(), container.Get<AppSettings>());
            var dragonFractalAction = container.Get<DragonFractalAction>();
            var kochFractalAction = container.Get<KochFractalAction>();
            var imageSettingsAction =
                new ImageSettingsAction(container.Get<PictureBoxImageHolder>(), container.Get<ImageSettings>());
            var paletteSettingsAction = new PaletteSettingsAction(container.Get<Palette>());
            return new MainForm(saveImageAction, dragonFractalAction, kochFractalAction, imageSettingsAction,
                paletteSettingsAction, container.Get<PictureBoxImageHolder>());
        }

        public static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind<IUiAction>().To<ImageSettingsAction>();
            container.Bind<IUiAction>().To<SaveImageAction>();
            container.Bind<IUiAction>().To<PaletteSettingsAction>();
            container.Bind<IUiAction>().To<DragonFractalAction>();
            container.Bind<IUiAction>().To<KochFractalAction>();
            container.Bind<ImageSettings>().ToMethod(context => context.Kernel.Get<AppSettings>().ImageSettings)
                .InSingletonScope();
            container.Bind<IImageHolder, PictureBoxImageHolder>().To<PictureBoxImageHolder>().InSingletonScope();
            container.Bind<IObjectSerializer>().To<XmlObjectSerializer>().WhenInjectedInto<SettingsManager>();
            container.Bind<IBlobStorage>().To<FileBlobStorage>().WhenInjectedInto<SettingsManager>();
            container.Bind<AppSettings>().ToMethod(context =>
                new SettingsManager(context.Kernel.Get<XmlObjectSerializer>(), context.Kernel.Get<FileBlobStorage>())
                    .Load()).InSingletonScope();
            container.Bind<Palette>().To<Palette>().InSingletonScope();
            container.Bind<IDragonPainterFactory>().ToFactory();
            return container;
        }
    }

    public class DragonFractalAction : IUiAction
    {
        private IDragonPainterFactory DragonPainterFactory { get; }
        public MenuCategory Category => MenuCategory.Fractals;
        public string Name => "Дракон";
        public string Description => "Дракон Хартера-Хейтуэя";

        public DragonFractalAction(IDragonPainterFactory dragonPainterFactory)
        {
            DragonPainterFactory = dragonPainterFactory;
        }

        public void Perform()
        {
            var dragonSettings = CreateRandomSettings();
            SettingsForm.For(dragonSettings).ShowDialog();
            DragonPainterFactory.CreateDragonPainter(dragonSettings).Paint();
        }

        private static DragonSettings CreateRandomSettings()
        {
            return new DragonSettingsGenerator(new Random()).Generate();
        }
    }

    public class KochFractalAction : IUiAction
    {
        private Lazy<KochPainter> KochPainter { get; }
        public MenuCategory Category => MenuCategory.Fractals;
        public string Name => "Кривая Коха";
        public string Description => "Кривая Коха";

        public KochFractalAction(Lazy<KochPainter> kochPainter)
        {
            KochPainter = kochPainter;
        }

        public void Perform()
        {
            KochPainter.Value.Paint();
        }
    }

    public class DragonPainter
    {
        private IImageHolder ImageHolder { get; }
        private Palette Palette { get; }
        private DragonSettings Settings { get; }

        public DragonPainter(IImageHolder imageHolder, Palette palette, DragonSettings settings)
        {
            this.ImageHolder = imageHolder;
            Palette = palette;
            this.Settings = settings;
        }

        public void Paint()
        {
            var imageSize = ImageHolder.GetImageSize();
            var size = Math.Min(imageSize.Width, imageSize.Height) / 2.1f;
            using (var graphics = ImageHolder.StartDrawing())
            using (var backgroundBrush = new SolidBrush(Palette.BackgroundColor))
            {
                graphics.FillRectangle(backgroundBrush, 0, 0, imageSize.Width, imageSize.Height);
                var r = new Random();
                var cosa = (float) Math.Cos(Settings.Angle1);
                var sina = (float) Math.Sin(Settings.Angle1);
                var cosb = (float) Math.Cos(Settings.Angle2);
                var sinb = (float) Math.Sin(Settings.Angle2);
                var shiftX = Settings.ShiftX * size * 0.8f;
                var shiftY = Settings.ShiftY * size * 0.8f;
                var scale = Settings.Scale;
                var p = new PointF(0, 0);
                foreach (var i in Enumerable.Range(0, Settings.IterationsCount))
                {
                    graphics.FillRectangle(Brushes.Yellow, imageSize.Width / 3f + p.X, imageSize.Height / 2f + p.Y, 1,
                        1);
                    p = r.Next(0, 2) == 0
                        ? new PointF(scale * (p.X * cosa - p.Y * sina), scale * (p.X * sina + p.Y * cosa))
                        : new PointF(scale * (p.X * cosb - p.Y * sinb) + shiftX,
                            scale * (p.X * sinb + p.Y * cosb) + shiftY);
                    if (i % 100 == 0)
                        ImageHolder.UpdateUi();
                }
            }
            ImageHolder.UpdateUi();
        }
    }

    public interface IDragonPainterFactory
    {
        public DragonPainter CreateDragonPainter(DragonSettings dragonSettings);
    }
}
