using  Catalog.Api.Dtos;
using  Catalog.Api.Entities;
namespace Catalog.Api
{
    public static class Extensions
    {
        public static ItemDtos AsDto(this Item it){
          
            return new ItemDtos{Id=it.Id, Name = it.Name , Price = it.Price, CreatedDate = it.CreatedDate};
        }
    }
}