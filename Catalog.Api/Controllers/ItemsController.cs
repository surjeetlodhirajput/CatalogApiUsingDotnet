using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using  Catalog.Api.Repositories;
using System;
using  Catalog.Api.Entities;
using  Catalog.Api.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IInMemItemRepository repository;
        private readonly ILogger<ItemsController> logger;
        public ItemsController(IInMemItemRepository repository, ILogger<ItemsController> logger){
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        //get items
        public async Task<IEnumerable<ItemDtos>> GetItems(){
            var items = (await repository.GetItemsAsync()).Select(it => it.AsDto());
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm::ss")}: Retrived {items.Count()}");
            return items; 
        }
        //get item by guid
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDtos>> GetItemAsync(Guid id){
        Item it =await repository.GetItemAsync(id);
        if(it is null){
            return NotFound();
        }
        var item = it.AsDto();
        return Ok(item);
        }
        //post request
        [HttpPost]
        public async Task<ActionResult<ItemDtos>> CreateItem([FromBody]CreateItemDto createItemDto)
        {
            Item item = new(){
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
          await  repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync),new {id = item.Id},item.AsDto());
        }
        //put/item
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var ex =await repository.GetItemAsync(id);
            if(ex is null){
                return NotFound();
            }
            Item updateItem  = ex with {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
          await  repository.UpdateItemAsync(updateItem);
            return NoContent();
        } 
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id){
            var rep =await repository.GetItemAsync(id);
            if(rep is null)return NotFound();
           await repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}