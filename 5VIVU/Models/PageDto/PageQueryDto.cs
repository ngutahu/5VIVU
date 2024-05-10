namespace SignUp.Models.PageDto;

public class PageQueryDto
{
    public int Page { get; set; } = 1;
    public int Take { get; set; } = 10;
    public SearchType SearchType { get; set; }
    public String SearchValue { get; set; } = String.Empty;
    public int Skip() => (Page - 1) * Take;
}
