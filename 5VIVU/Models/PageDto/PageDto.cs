namespace SignUp.Models.PageDto;

public class PageDto<T>
{
    public List<T> Data { get; set; }
    public PageMetaDto Meta { get; set; }
    public PageDto(List<T> data, PageMetaDto meta)
    {
        Data = data;
        Meta = meta;
    }
}
