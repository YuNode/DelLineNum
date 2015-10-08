using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DelLineNum
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();
            ReadSettings();
        }

        private async void btndelNum_Click(object sender, RoutedEventArgs e)
        {
            string arr = this.textBox.Text;
            //char[] cha = new char[str.Length];
            //str.CopyTo(0, cha, 0, str.Length);
            //string arr = new string(cha);

#if DEBUG
            MessageDialog md = new MessageDialog("" + arr, "结果");
            await md.ShowAsync();
#endif

            //arr = arr.Remove(1, 15);

            //获得第一个行号并删除
            int num = 0;
            for (int i = 0; i < arr.Length; i++)
            {

                if ((arr[i] >= '0') && (arr[i] <= '9'))
                {
                    num = Convert.ToUInt16(arr[i]) - 48;
                    //arr = arr.Replace(arr[i], ' ');
                    arr = arr.Remove(i, 1);
                    arr = arr.Insert(i, " ");
                    break;
                }
            }


            //删除行号

            for (int i = 0; i < arr.Length; i++)
            {
                if ((arr[i] >= '0') && (arr[i] <= '9'))
                {


                    int curnum = Convert.ToUInt16(arr[i]) - 48;

                    if (curnum == (num + 1))
                    {
                        //MessageDialog md3 = new MessageDialog("curnum =" + curnum+"\n"+ "i=" + i + "\n" + "arr[i]=" + arr[i] + "\n"  + arr, "num="+num);
                        //await md3.ShowAsync();

                        num = curnum;
                        arr = arr.Remove(i, 1);
                        arr = arr.Insert(i, " ");
                    }



                }

            }



#if DEBUG
            MessageDialog md1 = new MessageDialog("" + arr, "结果");
            await md1.ShowAsync();
#endif

            this.textBox.Text = arr;
        }

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        string mestxt;
        int index;
        private async void ReadSettings()
        {
            //读取配置信息
            string saveday = (string)localSettings.Values["saveday"];
            if (saveday == null)
            {
                // No data
#if DEBUG
                MessageDialog md = new MessageDialog("没有数据", "提示");
                await md.ShowAsync();
#endif
                SetBackground("old");
            }
            else
            {
                // Access data in value
                if (saveday == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    SetBackground("old");
                }
                else
                {
                    SetBackground("new");
                }
            }
        }


        private async void SetBackground(string newORrold)
        {


            WriteableBitmap writeAbleBitmap = new WriteableBitmap(340, 460);

            StorageFolder pictureFolder = await KnownFolders.PicturesLibrary.GetFolderAsync("Backgrounds");
            IReadOnlyList<StorageFile> picfiles = await pictureFolder.GetFilesAsync();

            //读取背景文件夹信息
            StorageFolder mes = await pictureFolder.CreateFolderAsync("mes", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFolder> folders = await mes.GetFoldersAsync();
            if (folders.Count == 0)
            {
                StorageFolder ind = await mes.CreateFolderAsync("0", CreationCollisionOption.OpenIfExists);
            }
            mestxt = folders[0].Name;
            //var bgmes = await pictureFolder.CreateFileAsync("bgmes.jpg", CreationCollisionOption.OpenIfExists);
            //mestxt = await FileIO.ReadTextAsync(bgmes);
            index = Convert.ToInt16(mestxt);
            if (newORrold == "new")
            {
                index = index + 1;
            }

            if (index == picfiles.Count)
                index = 0;

            IRandomAccessStream stream = await picfiles[index].OpenAsync(FileAccessMode.Read);
            await writeAbleBitmap.SetSourceAsync(stream);
            imgBackground.ImageSource = writeAbleBitmap;
            try
            {
                await folders[0].RenameAsync(Convert.ToString(index), NameCollisionOption.ReplaceExisting);
            }
            catch
            {

            }

            localSettings.Values["saveday"] = DateTime.Now.ToString("yyyy-MM-dd");
        }


    }
}
