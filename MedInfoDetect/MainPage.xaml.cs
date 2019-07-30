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
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                     api = DependencyService.Get<ITesseractApi>();
                    break;
                case Device.Android:
                     api = DependencyService.Get<ITesseractApi>();
                    break;
            }

            CameraButton.Clicked += async (sender, args) =>
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
                //photo.Source = ImageSource.FromStream(file.GetStream);
                bool initialised = await api.Init("eng");
                NameEntry.Text = NameEntry.Text + "2";
                api.SetWhitelist("0123456789:-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                bool success = await api.SetImage(photoStream);
                NameEntry.Text = NameEntry.Text + "3";
                if (success)
                {
                    //List<Result> lines = api.Results(PageIteratorLevel.Textline);
                    //List<Result> words = api.Results(PageIteratorLevel.Word);
                    List<Result> results = api.Results(PageIteratorLevel.Symbol).ToList();
                    //List<Result> blocks = api.Results(PageIteratorLevel.Block);

                    //List<Result> results = api.Results(PageIteratorLevel.Paragraph).ToList();
                    var res = " ";
                    var conf = " ";
                    foreach(Result r in results)
                    {
                        if (r.Confidence > 75f)
                        {
                            res += r.Text;
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
                
            };
            
        }



        private void CameraButton_Clicked(object sender, EventArgs e)
        {
         
        } 
    }
}
