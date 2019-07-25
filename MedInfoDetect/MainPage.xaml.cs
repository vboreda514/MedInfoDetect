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

            CameraButton.Clicked += async (sender, args) =>
            {
                
                await CrossMedia.Current.Initialize();

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
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
                bool initialised = await api.Init("eng");
                bool success = await api.SetImage(photoStream);
                if (success)
                {
                    //List<Result> words = api.Results(PageIteratorLevel.Word);
                    //List<Result> symbols = api.Results(PageIteratorLevel.Symbol);
                    //List<Result> blocks = api.Results(PageIteratorLevel.Block);
                    List<Result> results = (List<Result>)api.Results(PageIteratorLevel.Paragraph);
                    //List<Result> lines = api.Results(PageIteratorLevel.Textline);
                }
            };
            
        }



        private void CameraButton_Clicked(object sender, EventArgs e)
        {
         
        } 
    }
}
