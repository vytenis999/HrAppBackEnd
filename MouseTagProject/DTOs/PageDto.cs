namespace MouseTagProject.DTOs;
public record PageDto<TEntity>(int pages, int currentPage, int total, List<TEntity> data);
