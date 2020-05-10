using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleCollege
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker() { SuggestedStartLocation = PickerLocationId.ComputerFolder };
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");            
            StorageFolder folder = await picker.PickSingleFolderAsync();
            Windows.Storage.StorageFolder storageFolder = folder;
            var files = await folder.GetFilesAsync();
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("ImagesFolder", CreationCollisionOption.ReplaceExisting);
            
            StorageFolder testFolder =null;
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


            int i = 0;
            var files2 = await testFolder.GetFilesAsync();
            foreach (var file in files2)
            {
                i += 1;
                Debug.WriteLine(file.Path);
                Debug.WriteLine(file.DisplayName);

                BitmapImage bitmapImage = new BitmapImage();
                Uri uri = new Uri(file.Path,UriKind.RelativeOrAbsolute);
                bitmapImage.UriSource = uri;                
                var spV = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10) };
                spV.Children.Add(new Image() { Source = bitmapImage, Stretch = Stretch.UniformToFill, Width = Convert.ToInt32(widthImage.Text), Height = Convert.ToInt32(heightImage.Text) });
                spV.Children.Add(new TextBlock() { Text = file.DisplayName, HorizontalAlignment = HorizontalAlignment.Center });
                if(i<=7)
                {
                    sp1.Children.Add(spV);
                }
                else if( i>7 && i<=14)
                {
                    sp2.Children.Add(spV);
                }
                else if (i > 14 && i <= 21)
                {
                    sp3.Children.Add(spV);
                }
                else
                {
                    sp4.Children.Add(spV);
                }

            }
            
            
        }

        
        private void AppBarButton_Click1(object sender, RoutedEventArgs e)
        {
           
            
           // sp.Children.Add( new Image() { Source = bitmapImage , Stretch = Stretch.None , Width = 170, Height=170 });
        }
    }
}
