using AppName.Web.Models;
using AppName.Web.Repositories;
using GraphQL;
using System;
using System.Collections.Generic;

namespace AppName.Web.GraphQL
{
    public class Query
    {
        private readonly IData _data;

        public Query(IData data)
        {
            _data = data;
        }

        [GraphQLMetadata("airport")]
        public Airport GetAirport(string code)
        {
            return _data.GetAirport(code);
        }

        [GraphQLMetadata("airports")]
        public List<Airport> GetAirports()
        {
            return _data.GetAirports();
        }

        [GraphQLMetadata("healthcheck")]
        public Healthcheck GetHealthcheck()
        {
            return new Healthcheck("1.0.0", DateTime.Now);
        }
    }
}