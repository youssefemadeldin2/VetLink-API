namespace VetLink.Services.Helper
{
    public class PaginatedResultDto<T>
    {
		public PaginatedResultDto(IReadOnlyList<T> data, int pageNumber, int pageSize)
		{
			Data = data;
			PageNumber = pageNumber;
			PageSize = pageSize;
			TotalItems = 0; // You'll need to set this properly
			TotalPages = 0; // You'll need to set this properly
		}

		public PaginatedResultDto(IReadOnlyList<T> data, int pageNumber, int pageSize, int totalItems, int totalPages)
		{
			Data = data;
			PageNumber = pageNumber;
			PageSize = pageSize;
			TotalItems = totalItems;
			TotalPages = totalPages;
		}

		public IReadOnlyList<T> Data { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalItems { get; set; }
		public int TotalPages { get; set; }
	}
}
