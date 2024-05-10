namespace SignUp.Models.PageDto;

public class PageMetaDto
{
    public int Page { get; set; }
    public int Take { get; set; }
    public int PageCount => (ItemCount + Take - 1) / Take;
    public int ItemCount { get; set; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < PageCount;
    public SearchType SearchType { get; set; }
    public String SearchValue { get; set; }
    public PageMetaDto(PageQueryDto pageQueryDto, int itemCount)
    {
        Page = pageQueryDto.Page;
        Take = pageQueryDto.Take;
        ItemCount = itemCount;
        SearchType = pageQueryDto.SearchType;
        SearchValue = pageQueryDto.SearchValue;
    }
}
