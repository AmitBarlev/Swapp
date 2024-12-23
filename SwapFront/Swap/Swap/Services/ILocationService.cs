using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Swap.Services.ILocationService))]
namespace Swap.Services
{
    public class GpsNotAvailableException : Exception
    {

    }
    public interface ILocationService
    {
        void OpenSettings();
    }
}
