namespace LostFoundPetReporter.API.DTO.Interfaces
{
    public interface IResponseDto<TEntity, TDto>
        where TDto : IResponseDto<TEntity, TDto>
    {
        static abstract TDto FromEntity(TEntity entity);
    }

    public interface IEntityDto<TEntity>
    {
        TEntity ToEntity();
    }

    public interface IHasId
    {
        int? Id { get; set; }
    }



}
