using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo
{
    class ParametrizedConverterHandler
    {
        private MainWindow mainWindow;
        private ParametrizedConverter converter = new ParametrizedConverter();

        public ParametrizedConverterHandler(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            ResetStrategyOptions(mainWindow);
        }

        private void ResetStrategyOptions(MainWindow mainWindow)
        {
            converter.ParametrizedConvertStrategy?.Parameters.ForEach(element => element.Clear());
        }

        public void SetConvertStrategy(string strategyName)
        {
            Type strategyType = Type.GetType("KinectDemo." + strategyName);
            converter.ParametrizedConvertStrategy = (IParametrizedConvertStrategy)Activator.CreateInstance(strategyType);

            mainWindow.ParametrizedConvertParameters.Children.Clear();
            ShowStrategyOptions();
        }

        private void ShowStrategyOptions()
        {
            foreach (var option in converter.ParametrizedConvertStrategy.Parameters)
                option.CreateElement(mainWindow.ParametrizedConvertParameters.Children);
        }

        public void Convert()
        {
            if ((converter.ConvertId = mainWindow.GetParametrizedConvertId()).Length == 0)
                throw new Exception("set id");

            converter.Convert();
        }
    }
}
