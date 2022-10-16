using System;
namespace Catalog.UnitTests;
using Moq;
using  Catalog.Api.Repositories;
using Catalog.Api.Controllers;
using Catalog.Api.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Catalog.Api.Dtos;

public class UnitTest1
{
    private readonly  Mock<IInMemItemRepository> repositoryStud = new();
    private readonly  Mock<ILogger<ItemsController>> loggerStud = new();
    private readonly Random rand = new();
    [Fact]
    public async Task GetItemAsync_WithUnexistingItem_ReturnNotFound()
    {
        //Arrange 
        repositoryStud.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
        .ReturnsAsync((Item)null);

        var controller = new ItemsController(repositoryStud.Object,loggerStud.Object);
        //ACT
        var result = await controller.GetItemAsync(Guid.NewGuid());
        //assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
    [Fact]
    public async Task GetItemAsync_WithUnexistingItem_ReturnExpectedItem()
    {
        //Arrange
       var expectedItem = CreateRandomItem();
        repositoryStud.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
        .ReturnsAsync(expectedItem);
        var controller = new ItemsController(repositoryStud.Object, loggerStud.Object);
        //ACT
        var result = await controller.GetItemAsync(Guid.NewGuid());
        //assert
        result.Value.Should().BeEquivalentTo(expectedItem,
        options => options.ComparingByMembers<Item>()    
        );       
    }
     [Fact]
    public async Task GetItemAsync_WithexistingItem_ReturnAllItems(){

        //Arrange 
        var expectedItems = new[]{CreateRandomItem(),CreateRandomItem(),CreateRandomItem()};
        repositoryStud.Setup(repo => repo.GetItemsAsync()).ReturnsAsync(expectedItems);
        var controller = new ItemsController(repositoryStud.Object, loggerStud.Object);
        //act
        var actualItems = await controller.GetItems();
        //assert
        actualItems.Should().BeEquivalentTo(expectedItems,
        options => options.ComparingByMembers<Item>());
    }
     [Fact]
    public async Task CreateItemAsync_WithItemToCreate_ReturnCreatedItem(){

        //Arrange 
        var itemToCreate = new CreateItemDto(){
            Name = Guid.NewGuid().ToString(),
            Price = rand.Next(1000)
        };

        var controller = new ItemsController(repositoryStud.Object, loggerStud.Object);
        //act
        var result = await controller.CreateItem(itemToCreate);
        //assert
        var createItem = (result.Result as CreatedAtActionResult).Value as ItemDtos;
        itemToCreate.Should().BeEquivalentTo(createItem,
        options => options.ComparingByMembers<ItemDtos>().ExcludingMissingMembers()
        );
        createItem.Id.Should().NotBeEmpty();
       // createItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow,  1000);
         }
    [Fact]
    public async Task UpdateItemAsync_WithItemUpdateCreate_ReturnNoContent(){
        Item exepectedItem =CreateRandomItem();
        repositoryStud.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
        .ReturnsAsync(exepectedItem);
        
        var itemId = exepectedItem.Id;
        var itemToUpdate = new UpdateItemDto(){Name = Guid.NewGuid().ToString()
        ,Price = exepectedItem.Price + 3};
        var controller = new ItemsController(repositoryStud.Object, loggerStud.Object);
        var result = await controller.UpdateItem(itemId, itemToUpdate);
        //assert
        result.Should().BeOfType<NoContentResult>();
    }
    private Item CreateRandomItem(){
        return new(){
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Price = rand.Next(200),
            CreatedDate = DateTime.UtcNow
        };
    }
}