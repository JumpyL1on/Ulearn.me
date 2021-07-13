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
        private IImageHolder ImageHolder { get; }
        private ImageSettings ImageSettings { get; }
        public MenuCategory Category => MenuCategory.Settings;
        public string Name => "Изображение...";
        public string Description => "Размеры изображения";

        public ImageSettingsAction(IImageHolder imageHolder, ImageSettings imageSettings)
        {
            ImageHolder = imageHolder;
            ImageSettings = imageSettings;
        }

        public void Perform()
        {
            SettingsForm.For(ImageSettings).ShowDialog();
            ImageHolder.RecreateImage(ImageSettings);
        }
    }

    public class SaveImageAction : IUiAction
    {
        private IImageHolder ImageHolder { get; }
        private AppSettings AppSettings { get; }
        public MenuCategory Category => MenuCategory.File;
        public string Name => "Сохранить...";
        public string Description => "Сохранить изображение в файл";

        public SaveImageAction(IImageHolder imageHolder, AppSettings appSettings)
        {
            ImageHolder = imageHolder;
            AppSettings = appSettings;
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
        public MainForm(IUiAction saveImageAction, IUiAction dragonFractalAction, IUiAction kochFractalAction,
            IUiAction imageSettingsAction, IUiAction paletteSettingsAction, PictureBoxImageHolder pictureBox)
        {
            var imageSettings = CreateSettingsManager().Load().ImageSettings;
            ClientSize = new Size(imageSettings.Width, imageSettings.Height);
            pictureBox.RecreateImage(imageSettings);
            pictureBox.Dock = DockStyle.Fill;
            Controls.Add(pictureBox);
            var mainMenu = new MenuStrip();
            var actions = new[]
                {saveImageAction, dragonFractalAction, kochFractalAction, imageSettingsAction, paletteSettingsAction};
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