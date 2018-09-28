using AppName.Web.Models;
using AppName.Web.Repositories;
using GraphQL;

namespace AppName.Web.GraphQL
{
    public class Mutation
    {
        private readonly IData _data;

        public Mutation(IData data)
        {
            _data = data;
        }

        [GraphQLMetadata("addAirport")]
        public Airport AddAirport(AirportInput airport)
        {
            Airport a = _data.GetAirport(airport.Code);
            if (a == null)
            {
                _data.AddAirport(airport);

                return _data.GetAirport(airport.Code);
            }

            return a;
        }
    }
}