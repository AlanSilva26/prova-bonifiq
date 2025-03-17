using AutoFixture;
using ProvaPub.Models;

namespace ProvaPub.Tests.UnitTests.Models;

public class PaginatedListTests
{
    private readonly Fixture _fixture;

    public PaginatedListTests()
    {
        _fixture = new Fixture();
    }

    [Fact(DisplayName = "Deve criar uma lista paginada corretamente")]
    public void PaginatedList_ShouldInitializeCorrectly()
    {
        var items = _fixture.CreateMany<object>(10).ToList();
        int totalCount = 100;
        int page = 1;
        int pageSize = 10;

        var paginatedList = new PaginatedList<object>(items, totalCount, page, pageSize);

        Assert.NotNull(paginatedList);
        Assert.Equal(items.Count, paginatedList.Items.Count);
        Assert.Equal(totalCount, paginatedList.TotalCount);
        Assert.Equal(page, paginatedList.Page);
        Assert.Equal(pageSize, paginatedList.PageSize);
    }

    [Fact(DisplayName = "HasNext deve ser verdadeiro quando houver mais páginas")]
    public void HasNext_ShouldReturnTrue_WhenMorePagesExist()
    {
        int page = 2;
        int pageSize = 10;
        int totalCount = 25;
        var items = _fixture.CreateMany<object>(pageSize).ToList();

        var paginatedList = new PaginatedList<object>(items, totalCount, page, pageSize);

        Assert.True(paginatedList.HasNext);
        Assert.True((paginatedList.Page * paginatedList.PageSize) <= paginatedList.TotalCount);
        Assert.False((paginatedList.Page * paginatedList.PageSize) == paginatedList.TotalCount);
    }

    [Fact(DisplayName = "HasNext deve ser falso quando for a última página")]
    public void HasNext_ShouldReturnFalse_WhenOnLastPage()
    {
        int page = 3;
        int pageSize = 10;
        int totalCount = 25;
        var items = _fixture.CreateMany<object>(pageSize).ToList();

        var paginatedList = new PaginatedList<object>(items, totalCount, page, pageSize);

        Assert.False(paginatedList.HasNext);
    }

    [Fact(DisplayName = "HasNext deve ser falso quando não há itens suficientes para uma nova página")]
    public void HasNext_ShouldReturnFalse_WhenTotalCountLessThanPageSize()
    {
        int page = 1;
        int pageSize = 10;
        int totalCount = 9;
        var items = _fixture.CreateMany<object>(totalCount).ToList();

        var paginatedList = new PaginatedList<object>(items, totalCount, page, pageSize);

        Assert.False(paginatedList.HasNext);
    }

    [Fact(DisplayName = "Deve permitir uma lista vazia sem erros")]
    public void PaginatedList_ShouldAllowEmptyList()
    {
        var items = new List<object>();
        int totalCount = 0;
        int page = 1;
        int pageSize = 10;

        var paginatedList = new PaginatedList<object>(items, totalCount, page, pageSize);

        Assert.NotNull(paginatedList);
        Assert.Empty(paginatedList.Items);
        Assert.Equal(0, paginatedList.TotalCount);
        Assert.False(paginatedList.HasNext);
    }

    [Fact(DisplayName = "Deve retornar false quando Page * PageSize for igual a TotalCount")]
    public void HasNext_ShouldReturnFalse_WhenPageTimesPageSizeEqualsTotalCount()
    {
        var items = new List<object>();
        int totalCount = 10;
        int pageSize = 5;
        int page = 2;

        var paginatedList = new PaginatedList<object>(items, totalCount, page, pageSize);

        bool result = paginatedList.HasNext;

        Assert.False(result);
    }
}
