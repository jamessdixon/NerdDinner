﻿using System;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using NerdDinner.UI.Models;
using NerdDinner.UI.Services;
using DataServicesJSONP;
using NerdDinner.Models;

namespace NerdDinner.UI
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = System.ServiceModel.ConcurrencyMode.Single)]
    [JSONPSupportBehavior]
    public class ODataServices : DataService<NerdDinners>
    {
        IDinnerRepository dinnerRepository;
        //
        // Dependency Injection enabled constructors

        public ODataServices()
            : this(new DinnerRepository())
        {
        }

        public ODataServices(IDinnerRepository repository)
        {
            dinnerRepository = repository;
        }

        protected override NerdDinners CreateDataSource()
        {
            var nd = base.CreateDataSource();
            nd.Configuration.ProxyCreationEnabled = false;
            return nd;
        }

        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetPageSize("*", 100);
            config.SetEntitySetAccessRule("Dinners", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("RSVPs", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("DinnersNearMe", ServiceOperationRights.AllRead);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);

            HttpContext context = HttpContext.Current;
            if (null != context)
            {
                HttpCachePolicy c = context.Response.Cache;
                c.SetCacheability(HttpCacheability.ServerAndPrivate);
                c.SetExpires(context.Timestamp.AddSeconds(30));
                c.VaryByHeaders["Accept"] = true;
                c.VaryByHeaders["Accept-Charset"] = true;
                c.VaryByHeaders["Accept-Encoding"] = true;
                c.VaryByParams["*"] = true;
            }
        }

        [WebGet]
        public IQueryable<Dinner> FindUpcomingDinners()
        {
            return dinnerRepository.FindUpcomingDinners();
        }

        // http://localhost:60848/Services.svc/DinnersNearMe?placeOrZip='12345'
        [WebGet]
        public IQueryable<Dinner> DinnersNearMe(string placeOrZip)
        {
            if (String.IsNullOrEmpty(placeOrZip)) return null; ;

            LatLong location = GeolocationService.PlaceOrZipToLatLong(placeOrZip);

            var dinners = dinnerRepository.
                            FindByLocation(location.Lat, location.Long).
                            OrderByDescending(p => p.EventDate);
            return dinners;
        }

    }
}
