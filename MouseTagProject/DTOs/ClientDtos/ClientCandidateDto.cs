namespace MouseTagProject.DTOs.ClientDtos;
public record ClientCandidateDto(int Id, string Name, string SurName, string Email, IEnumerable<TechnologyDto> Technologies);

