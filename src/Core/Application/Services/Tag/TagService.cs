using Domain.Entities;

namespace Application.Services;

public class TagService
{
    private const string DefaultColor = "blue";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TagService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _unitOfWork.Tags
            .GetAll(tag => !tag.IsDeleted)
            .Include(tag => tag.Products)
            .OrderByDescending(tag => tag.CreatedTime)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto> GetTagByIdAsync(int id)
    {
        var tag = await GetActiveTagEntityByIdAsync(id);
        return _mapper.Map<TagDto>(tag);
    }

    public async Task<TagDto> AddTagAsync(CreateTagDto tagDto)
    {
        tagDto.Name = NormalizeName(tagDto.Name, nameof(tagDto.Name));
        tagDto.Color = NormalizeColor(tagDto.Color);
        tagDto.Description = NormalizeOptionalText(tagDto.Description);

        await EnsureNameIsUniqueAsync(tagDto.Name);

        var tag = _mapper.Map<Tag>(tagDto);
        await _unitOfWork.Tags.AddAsync(tag);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TagDto>(tag);
    }

    public async Task<TagDto> UpdateTagAsync(int id, UpdateTagDto tagDto)
    {
        var tag = await GetActiveTagEntityByIdAsync(id);

        var normalizedName = tagDto.Name == null
            ? tag.Name
            : NormalizeName(tagDto.Name, nameof(tagDto.Name));
        var normalizedColor = tagDto.Color == null
            ? tag.Color
            : NormalizeColor(tagDto.Color);
        var normalizedDescription = tagDto.Description == null
            ? tag.Description
            : NormalizeOptionalText(tagDto.Description);

        await EnsureNameIsUniqueAsync(normalizedName, id);

        tag.Name = normalizedName;
        tag.Color = normalizedColor;
        tag.Description = normalizedDescription;
        await _unitOfWork.Tags.UpdateAsync(tag);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TagDto>(tag);
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await GetActiveTagEntityByIdAsync(id);
        await _unitOfWork.Tags.DeleteAsync(tag);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<TagDto>> SearchTagsByNameAsync(string name)
    {
        var normalizedName = NormalizeName(name, nameof(name));
        var tags = await _unitOfWork.Tags
            .GetAll(tag => !tag.IsDeleted && tag.Name.Contains(normalizedName))
            .Include(tag => tag.Products)
            .OrderBy(tag => tag.Name)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    private async Task<Tag> GetActiveTagEntityByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Tag id must be greater than zero.", nameof(id));
        }

        var tag = await _unitOfWork.Tags
            .GetAll(tag => !tag.IsDeleted && tag.Id == id)
            .Include(tag => tag.Products)
            .FirstOrDefaultAsync();

        if (tag == null)
        {
            throw new KeyNotFoundException($"Tag with ID {id} not found.");
        }

        return tag;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludedId = null)
    {
        var normalizedName = name.ToLowerInvariant();
        var exists = await _unitOfWork.Tags
            .GetAll(tag => !tag.IsDeleted && tag.Name.ToLower() == normalizedName && (!excludedId.HasValue || tag.Id != excludedId.Value))
            .AnyAsync();

        if (exists)
        {
            throw new ArgumentException($"Tag with name '{name}' already exists.");
        }
    }

    private static string NormalizeName(string? name, string parameterName)
    {
        var normalizedName = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Tag name is required.", parameterName);
        }

        if (normalizedName.Length > 50)
        {
            throw new ArgumentException("Tag name cannot exceed 50 characters.", parameterName);
        }

        return normalizedName;
    }

    private static string NormalizeColor(string? color)
    {
        var normalizedColor = color?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedColor))
        {
            return DefaultColor;
        }

        if (normalizedColor.Length > 50)
        {
            throw new ArgumentException("Tag color cannot exceed 50 characters.", nameof(color));
        }

        return normalizedColor.ToLowerInvariant();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        var normalizedValue = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            return null;
        }

        if (normalizedValue.Length > 500)
        {
            throw new ArgumentException("Tag description cannot exceed 500 characters.", nameof(value));
        }

        return normalizedValue;
    }
}
