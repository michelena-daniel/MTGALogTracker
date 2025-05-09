﻿using Domain.Models;

namespace Domain.Interfaces
{
    public interface IMatchService
    {
        string FetchMatches(string line, string delimeter, EventState eventState);
        Task WriteMatches(List<Domain.Entities.Match> matches);
        List<Domain.Entities.Match> MapMatches(List<MatchDto> matches);
    }
}
