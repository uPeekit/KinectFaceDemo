using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace KinectDemo
{
    public interface IParametrizedConvertStrategy
    {
        List<Parameter> Parameters { get; set; }

        string ResultFileName { get; }

        List<List<string>> GetResultDataWithHeaders();
    }
}