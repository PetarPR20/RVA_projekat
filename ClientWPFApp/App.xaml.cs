using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.ServiceModel;
using System.Windows;
using Common;
using Common.Interface;

namespace WPFModelClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CallService();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void CallService()
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress("http://localhost:8080/MyService");

            var factory = new ChannelFactory<IModelService>(binding, endpoint);
            IModelService proxy = factory.CreateChannel();

            List<Model> result = proxy.GetAllModels();
            MessageBox.Show(result[0].ModelName);

            ((IClientChannel)proxy).Close();
            factory.Close();
        }
    }

}
