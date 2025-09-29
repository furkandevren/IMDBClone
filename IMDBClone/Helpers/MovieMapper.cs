using System;
using System.Collections.Generic;
using System.Linq;
using IMDBClone.API.Dtos.MovieDtos;
using IMDBClone.API.Models;

namespace IMDBClone.API.Helpers
{
    public static class MovieMapper
    {
        public static MovieReadDto ToReadDto(this Movie m) =>
            new MovieReadDto
            {
                Id = m.Id!,
                Title = m.Title,
                Year = m.Year,
                Overview = m.Overview,
                Genres = m.Genres,
                ReleaseDate = m.ReleaseDate,
                PosterUrl = m.PosterUrl,
                Rating = m.Rating,
                RatingCount = m.RatingCount,
                CreatedAt = m.CreatedAt,
                Cast = m.Cast?.Select(c => new CastMemberReadDto
                {
                    ActorId = c.ActorId,
                    Character = c.Character,
                    Order = c.Order
                }).ToList() ?? new List<CastMemberReadDto>()
            };

        public static Movie FromCreateDto(this MovieCreateDto dto) =>
            new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Overview = dto.Overview,
                Genres = dto.Genres ?? new(),
                ReleaseDate = dto.ReleaseDate,
                PosterUrl = dto.PosterUrl,
                Rating = 0,
                RatingCount = 0,
                CreatedAt = DateTime.UtcNow,
                ReviewIds = new(),
                Cast = dto.Cast?.Select(c => new CastMember
                {
                    ActorId = c.ActorId,      // <<--- actorId ataması aktif
                    Character = c.Character,
                    Order = c.Order
                }).ToList() ?? new List<CastMember>()
            };

        public static Movie FromUpdateDto(this MovieUpdateDto dto, Movie existing)
        {
            // Sadece güncellenmesini istediğimiz alanları değiştirelim
            existing.Title = dto.Title;
            existing.Year = dto.Year;
            existing.Overview = dto.Overview;
            existing.Genres = dto.Genres ?? new();
            existing.ReleaseDate = dto.ReleaseDate;
            existing.PosterUrl = dto.PosterUrl;
            // Not: rating/ratingCount, createdAt korunur
            existing.Cast = dto.Cast?.Select(c => new CastMember
            {
                ActorId = c.ActorId,      // <<--- actorId ataması aktif
                Character = c.Character,
                Order = c.Order
            }).ToList() ?? new List<CastMember>();

            return existing;
        }
    }
}
