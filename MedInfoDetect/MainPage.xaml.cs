using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Tesseract;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace MedInfoDetect
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]

    public partial class MainPage : ContentPage
    {
        private readonly ITesseractApi api;
        
        public MainPage()
        {
            
            InitializeComponent();
            api = DependencyService.Get<ITesseractApi>();

            /*CameraButton.Clicked += async (sender, args) =>
            {
                
                await CrossMedia.Current.Initialize();

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "MedSticker",
                    Name = "sticker.jpg"
                });

                if (file == null)
                    return;
                System.IO.Stream photoStream = file.GetStream();

                
                //photo.Source = ImageSource.FromStream(() =>
                // {
                //     var stream = file.GetStream();
                //file.Dispose();
                //     return stream;
                // });

                NameEntry.Text  = "1";
                //Photo.Source = ImageSource.FromStream(file.GetStream); //debug code
                Bitmap bitmap = new Bitmap(photoStream);
                Stream stream2 = RaiseContrast(bitmap);
                //Photo.Source = ImageSource.FromStream(() => stream2);
                bool initialised = await api.Init("eng");
                NameEntry.Text += "2";
                api.SetWhitelist("0123456789:-,#'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                bool success = await api.SetImage(stream2);
                NameEntry.Text += "3";                                                    
                if (success)
                {
                    //List<Result> lines = api.Results(PageIteratorLevel.Textline);
                    //List<Result> words = api.Results(PageIteratorLevel.Word);
                    List<Result> results = api.Results(PageIteratorLevel.Symbol).ToList();
                    //List<Result> blocks = api.Results(PageIteratorLevel.Block);

                    //List<Result> results = api.Res    ults(PageIteratorLevel.Paragraph).ToList();
                    var res = " ";
                    var conf = " ";
                    foreach(Result r in results)
                    {
                        if (r.Confidence > 85f)
                        {
                            res += r.Text.ToUpper();
                            conf += r.Confidence.ToString() + " ";
                        }
                        
                    }
                    SexEntry.Text = api.Text;
                    MREntry.Text = conf;
                    //SexLabel.Text = res;

                    
                }
                else
                {
                    
                    NameEntry.Text = "Image Recognition Failed";
                }
                
            }; */
            
        }

        public Stream RaiseContrast(Bitmap bitmap1)
        {
            LockBitmap bitmap = new LockBitmap(bitmap1);

            bitmap.LockBits();
            var raisedby = 30.0;      //change this
            var contrast = Math.Pow((100.0 + raisedby) / 100.0, 2);

            for(int x = 0; x < bitmap.Width; x++)
            {
                for(int y = 0; y < bitmap.Height; y++)
                {
                    System.Drawing.Color prev = bitmap.GetPixel(x, y);
                    var red = ((((prev.R / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var green = ((((prev.G / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    var blue = ((((prev.B / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                    if (red > 255)
                        red = 255.0;
                    else if (red < 0)
                        red = 0.0;
                    if (green > 255)
                        green = 255.0;
                    else if (green < 0)
                        green = 0.0;
                    if (blue > 255)
                        blue = 255.0;
                    else if (blue < 0)
                        blue = 0.0;

                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(prev.A, (int)red, (int)green, (int)blue));  
                }
            }
            bitmap.UnlockBits();
            Stream m = new MemoryStream();
            m.Write(bitmap.Pixels, 0, bitmap.Pixels.Length);
            return m;
        }


        private async void CameraButton_Clicked(object sender, EventArgs e)
        {

            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "MedSticker",
                Name = "sticker.jpg"
            });

            if (file == null)
                return;
            System.IO.Stream photoStream = file.GetStream();


            //photo.Source = ImageSource.FromStream(() =>
            // {
            //     var stream = file.GetStream();
            //file.Dispose();
            //     return stream;
            // });

            //NameEntry.Text = "1";
            //Photo.Source = ImageSource.FromStream(file.GetStream); //debug code
            //Bitmap bitmap = new Bitmap(photoStream);
            //Stream photoStream2 = RaiseContrast(bitmap);
            //Photo.Source = ImageSource.FromStream(() => stream2);
            bool initialised = await api.Init("eng");
            //NameEntry.Text += "2";
            api.SetWhitelist("0123456789:-,#'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            bool success = await api.SetImage(photoStream);
            //NameEntry.Text += "3";
            if (success)
            {
                //List<Result> lines = api.Results(PageIteratorLevel.Textline);
                //List<Result> words = api.Results(PageIteratorLevel.Word);
                List<Result> results = api.Results(PageIteratorLevel.Symbol).ToList();
                //List<Result> blocks = api.Results(PageIteratorLevel.Block);

                //List<Result> results = api.Res    ults(PageIteratorLevel.Paragraph).ToList();
                var res = " ";
                var conf = " ";
                foreach (Result r in results)
                {
                    if (r.Confidence > 85f)
                    {
                        res += r.Text.ToUpper();
                        conf += r.Confidence.ToString() + " ";
                    }

                }
                res = res.Replace(":", "");
                //SexEntry.Text = res;
                //MREntry.Text = conf;
                if (res.Contains("PR"))
                {
                    var sindex = res.LastIndexOf("PR") - 1;
                    SexEntry.Text = res.Substring(sindex, 1);
                }
                if (res.Contains("PCP"))
                {
                    var nindex = res.LastIndexOf("PCP");
                    NameEntry.Text = res.Substring(0, nindex - 1);
                }
                if (res.Contains("ACCT") & res.Contains("#")){
                    var mindex2 = res.LastIndexOf("ACCT");
                    var mindex = res.IndexOf("#");
                    MREntry.Text = res.Substring(mindex + 1, mindex2 - mindex-1);
                }
                if(!(res.Contains("PR") | res.Contains("PCP") | res.Contains("ACCT") | res.Contains("#")))
                {
                    await DisplayAlert("Could not detect text.", "Try again or enter info manually.", "OK");
                }
            }
            else
            {

                NameEntry.Text = "Image Recognition Failed";
            }



            
        }

    

        private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            var name = NameEntry.Text;
            var sex = SexEntry.Text;
            var mr = MREntry.Text;
            TestLabel.Text = name + " " + sex + " " + mr;
        }

        
    }
}
