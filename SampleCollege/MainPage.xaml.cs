using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleCollege
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFolder testFolder = null;
        bool stopFlag = true;
       
        public MainPage()
        {
            this.InitializeComponent();
          
        }

     
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                FolderPicker picker = new FolderPicker() { SuggestedStartLocation = PickerLocationId.ComputerFolder };
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".jpg");
                StorageFolder folder = await picker.PickSingleFolderAsync();
                Windows.Storage.StorageFolder storageFolder = folder;
                var files = await folder.GetFilesAsync();
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("ImagesFolder", CreationCollisionOption.ReplaceExisting);


                foreach (var file in files)
                {
                    StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                    string folderName = System.IO.Path.GetFileNameWithoutExtension(file.Name);  //folder name with filename

                    ////await ApplicationData.Current.LocalFolder.CreateFolderAsync("Data");//need to change the folder name with filename

                    testFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("ImagesFolder", CreationCollisionOption.OpenIfExists);

                    ////StorageFolder newFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

                    //StorageFolder newFolder = await testFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

                    //string desiredName = file.Name;
                    ////should copy it to subfolder and raise alert if already exist

                    ////StorageFile newFile = await localFolder.CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);

                    try
                    {
                        await file.CopyAsync(testFolder, file.Name, NameCollisionOption.FailIfExists);
                    }
                    catch (Exception exp)
                    {
                        //show here messagebox that is exists
                        Windows.UI.Xaml.Controls.ContentDialog replacePromptDialog = new Windows.UI.Xaml.Controls.ContentDialog()
                        {
                            Title = "File exists in the new location",
                            Content = "Do you want to replace the old file with the new file?",
                            CloseButtonText = "Keep the old one",
                            PrimaryButtonText = "Replace with new one"
                        };
                        Windows.UI.Xaml.Controls.ContentDialogResult result = await replacePromptDialog.ShowAsync();
                        if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                        {
                            // await file.CopyAsync(newFolder, desiredName, NameCollisionOption.ReplaceExisting);
                        }
                    }
                }
                Process(testFolder);
                refreshButton.IsEnabled = true;
                stopFlag = false;
            }
            catch(Exception exx)
            {
                Debug.WriteLine("Exception");
            }
        }

        private async void Process(StorageFolder storageFolder)
        {
            var files2 = await storageFolder.GetFilesAsync();
            int fileIndex = 0;
            for (int rows = 0; rows < Convert.ToInt32(rowNum.Text); rows++)
            {
                //create a stackpanel with horizontal orientation
                var horiSp = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10), HorizontalAlignment = HorizontalAlignment.Center };
                for (int image_num = 0; image_num < Convert.ToInt32(noOfImages.Text); image_num++)
                {

                    //add the image to the row sp
                    StorageFile currentImagefile = files2[fileIndex];
                    var spV = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness= new Thickness(imageBorderThickness.Value) };
                    BitmapImage bitmapImage = new BitmapImage();
                    Uri uri = new Uri(currentImagefile.Path, UriKind.RelativeOrAbsolute);
                    bitmapImage.UriSource = uri;
                    spV.Children.Add(new Image() { Source = bitmapImage, Stretch = Stretch.UniformToFill, Width = Convert.ToInt32(widthImage.Text), Height = Convert.ToInt32(heightImage.Text) });
                    spV.Children.Add(new TextBlock() { Text = currentImagefile.DisplayName, HorizontalAlignment = HorizontalAlignment.Center });

                    if (fileIndex < files2.Count)
                    {
                        fileIndex += 1;
                    }
                    horiSp.Children.Add(spV);
                    if (fileIndex >= files2.Count)
                        break;

                }
                sp.Children.Add(horiSp);
                if (fileIndex > files2.Count - 1)
                    break;
            }
        }
        private async void AppBarButton_Click1(object sender, RoutedEventArgs e)
        {
            // ScreenshotButton_ClickAsync(sender, e);
            // Render to an image at the current system scale and retrieve pixel contents
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(grid);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            var savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".png";
            savePicker.FileTypeChoices.Add(".png", new List<string> { ".png" });
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.SuggestedFileName = "snapshot.png";

            // Prompt the user to select a file
            var saveFile = await savePicker.PickSaveFileAsync();

            // Verify the user selected a file
            if (saveFile == null)
                return;

            // Encode the image to the selected file on disk
            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }
        }

        private void headerSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                headerTB.FontSize = Convert.ToInt32(headerSize.Text);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Empty Header Size");
            }
        }

        private void subHeaderSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                subheaderTB.FontSize = Convert.ToInt32(subHeaderSize.Text);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Empty Sub Header Size");
            }
            
        }

        private void AppBarButton_Click2(object sender, RoutedEventArgs e)
        {
            sp.Children.Clear();
            Process(testFolder);

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!stopFlag)
            {
                ToggleSwitch swi = sender as ToggleSwitch;
                if (swi.IsOn)
                {
                    subheaderTB.Visibility = Visibility.Visible;
                }
                else
                {
                    subheaderTB.Visibility = Visibility.Collapsed;
                }
            }
            
        }

        private void ToggleSwitch_Toggled1(object sender, RoutedEventArgs e)
        {
            if (!stopFlag)
            {
                ToggleSwitch swi = sender as ToggleSwitch;
                if (swi.IsOn)
                {
                    
                }
                else
                {
                    subheaderTB.Visibility = Visibility.Collapsed;
                }
            }

        }
       
    }
}
