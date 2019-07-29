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

                NameLabel.Text = "1";
                //photo.Source = ImageSource.FromStream(file.GetStream);
                bool initialised = await api.Init("eng");
                NameLabel.Text = NameLabel.Text + "2";
                bool success = await api.SetImage(photoStream);
                NameLabel.Text = NameLabel.Text + "3";
                if (success)
                {
                    //List<Result> lines = api.Results(PageIteratorLevel.Textline);
                    //List<Result> words = api.Results(PageIteratorLevel.Word);
                    //List<Result> symbols = api.Results(PageIteratorLevel.Symbol);
                    //List<Result> blocks = api.Results(PageIteratorLevel.Block);

                    List<Result> results = api.Results(PageIteratorLevel.Paragraph).ToList();
                    var res = " ";
                    foreach(Result r in results)
                    {
                        res += r.Text;
                       
                    }
                    SexLabel.Text = api.Text;
                    //SexLabel.Text = res;

                    
                }
                else
                {
                    
                    NameLabel.Text = "Image Recognition Failed";
                }
                
            };
            
        }



        private void CameraButton_Clicked(object sender, EventArgs e)
        {
         
        } 
    }
}
