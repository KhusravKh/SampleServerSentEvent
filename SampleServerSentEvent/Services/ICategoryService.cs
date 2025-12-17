using SampleServerSentEvent.Models;

namespace SampleServerSentEvent.Services;

public interface ICategoryService
{
    Task<List<Category>> GetCategories(List<long> ids);
}