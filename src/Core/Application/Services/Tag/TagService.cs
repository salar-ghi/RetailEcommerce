using Domain.Entities;

namespace Application.Services;

public class TagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TagService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto> GetTagByIdAsync(int id)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        if (tag == null) throw new KeyNotFoundException($"Tag with ID {id} not found.");
        return _mapper.Map<TagDto>(tag);
    }

    public async Task AddTagAsync(TagDto tagDto)
    {
        var tag = _mapper.Map<Tag>(tagDto);
        tag.CreatedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        tag.ModifiedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        await _unitOfWork.Tags.AddAsync(tag);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateTagAsync(TagDto tagDto)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(tagDto.Id);
        if (tag == null) throw new KeyNotFoundException($"Tag with ID {tagDto.Id} not found.");
        _mapper.Map(tagDto, tag);
        await _unitOfWork.Tags.UpdateAsync(tag);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id);
        if (tag != null)
        {
            await _unitOfWork.Tags.DeleteAsync(tag);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TagDto>> SearchTagsByNameAsync(string name)
    {
        var tags = await _unitOfWork.Tags.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }
}