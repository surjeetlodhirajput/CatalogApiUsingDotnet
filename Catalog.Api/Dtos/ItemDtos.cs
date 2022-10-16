using System;
namespace Catalog.Api.Dtos
{
    public record ItemDtos
    {
        public Guid Id{ get; init; }//cannot modifies after creation
        public string Name {get; init; }

        public decimal Price{get; init;}
        public DateTimeOffset CreatedDate {get; init;}

    }
}