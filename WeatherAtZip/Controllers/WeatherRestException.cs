using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeatherAtZip.Model;


namespace WeatherAtZip.Controllers
{
    public class WeatherRestException : Exception
    {
        /// <summary>
        /// If an exception needs to be thrown, then include the status
        /// and any data or message. 
        /// </summary>
        public ActionResult InternalEntity { get; private set; }

        public WeatherRestException(ActionResult internalEntity)
        {
            this.InternalEntity = internalEntity;
        }

    }
}