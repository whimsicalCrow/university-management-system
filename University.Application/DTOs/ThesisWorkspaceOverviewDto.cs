using System.Collections.Generic;

namespace University.Application.DTOs;

public record ThesisWorkspaceOverviewDto(
    ThesisProjectDto Project,
    StudentDto Student,
    ProfessorDto Professor,
    IReadOnlyList<ThesisUpdateDto> Updates,
    IReadOnlyList<MeetingDto> Meetings
);