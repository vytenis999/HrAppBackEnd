namespace MouseTagProject.DTOs.ClientDtos;
public record ClientGetDto (int Id, string Name,string Project, string Comment, string WillBeContacted, IEnumerable<ClientCandidateDto> Candidates);
