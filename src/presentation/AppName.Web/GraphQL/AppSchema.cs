using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;

namespace AppName.Web.GraphQL
{
    public class AppSchema : Schema
    {
        public AppSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<AppSchemaInternal>().Query;
            Mutation = resolver.Resolve<AppSchemaInternal>().Mutation;
        }
    }

    internal class AppSchemaInternal
    {
        private const string typedefinitions = @"
type Airport {
    code: ID
    name: String
}

type Healthcheck {
    version: String
    serverTimestamp: DateTime
}

type Query {
    airports: [Airport]
    airport(code: ID!): Airport
    healthcheck: Healthcheck
}

input AirportInput {
    code: ID
    name: String
}

type Mutation {
    addAirport(airport: AirportInput!): Airport
}
            ";

        private readonly ISchema _schema;
        
        public AppSchemaInternal(IDependencyResolver resolver)
        {
            var builder = new SchemaBuilder();
            builder.DependencyResolver = resolver;
            builder.Types.Include<Query>();
            builder.Types.Include<Mutation>();
            _schema = builder.Build(typedefinitions);
        }

        internal IObjectGraphType Query
        {
            get { return _schema.Query; }
        }

        internal IObjectGraphType Mutation
        {
            get { return _schema.Mutation; }
        }
    }
}