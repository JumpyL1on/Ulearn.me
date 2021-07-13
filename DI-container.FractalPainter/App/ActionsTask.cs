using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using FractalPainting.Infrastructure.Common;
using FractalPainting.Infrastructure.UiActions;

namespace FractalPainting.App
{
    public class ImageSettingsAction : IUiAction
    {
        private ImageSettings ImageSettings { get; }
        private IImageHolder ImageHolder { get; }
        public MenuCategory Category => MenuCategory.Settings;
        public string Name => "Изображение...";
        public string Description => "Размеры изображения";

        public ImageSettingsAction(ImageSettings imageSettings, IImageHolder imageHolder)
        {
            ImageSettings = imageSettings;
            ImageHolder = imageHolder;
        }

        public void Perform()
        {
            SettingsForm.For(ImageSettings).ShowDialog();
            ImageHolder.RecreateImage(ImageSettings);
        }
    }

    public class SaveImageAction : IUiAction
    {
        private AppSettings AppSettings { get; }
        private IImageHolder ImageHolder { get; }
        public MenuCategory Category => MenuCategory.File;
        public string Name => "Сохранить...";
        public string Description => "Сохранить изображение в файл";

        public SaveImageAction(AppSettings appSettings, IImageHolder imageHolder)
        {
            AppSettings = appSettings;
            ImageHolder = imageHolder;
        }

        public void Perform()
        {
            var dialog = new SaveFileDialog
            {
                CheckFileExists = false,
                InitialDirectory = Path.GetFullPath(AppSettings.ImagesDirectory),
                DefaultExt = "bmp",
                FileName = "image.bmp",
                Filter = "Изображения (*.bmp)|*.bmp"
            };
            var res = dialog.ShowDialog();
            if (res == DialogResult.OK)
                ImageHolder.SaveImage(dialog.FileName);
        }
    }

    public class PaletteSettingsAction : IUiAction
    {
        private Palette Palette { get; }
        public MenuCategory Category => MenuCategory.Settings;
        public string Name => "Палитра...";
        public string Description => "Цвета для рисования фракталов";

        public PaletteSettingsAction(Palette palette)
        {
            Palette = palette;
        }

        public void Perform()
        {
            SettingsForm.For(Palette).ShowDialog();
        }
    }

    public class MainForm : Form
    {
        public MainForm() : this(
            new IUiAction[]
            {
                new SaveImageAction(Services.GetAppSettings(), Services.GetImageHolder()), new DragonFractalAction(),
                new KochFractalAction(),
                new ImageSettingsAction(Services.GetImageSettings(), Services.GetImageHolder()),
                new PaletteSettingsAction(Services.GetPalette())
            }, Services.GetPictureBoxImageHolder())
        {
        }

        private MainForm(IUiAction[] actions, PictureBoxImageHolder pictureBox)
        {
            var imageSettings = CreateSettingsManager().Load().ImageSettings;
            ClientSize = new Size(imageSettings.Width, imageSettings.Height);
            pictureBox.RecreateImage(imageSettings);
            pictureBox.Dock = DockStyle.Fill;
            Controls.Add(pictureBox);
            var mainMenu = new MenuStrip();
            mainMenu.Items.AddRange(actions.ToMenuItems());
            mainMenu.Dock = DockStyle.Top;
            Controls.Add(mainMenu);
        }

        private static SettingsManager CreateSettingsManager()
        {
            return new SettingsManager(new XmlObjectSerializer(), new FileBlobStorage());
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Text = "Fractal Painter";
        }
    }
}