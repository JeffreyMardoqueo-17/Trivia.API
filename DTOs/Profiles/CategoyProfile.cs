using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TriviaGame.Api.DTOs.Category;
using TriviaGame.Api.Models;

namespace TriviaGame.Api.DTOs.Profiles
{
    public class CategoyProfile : Profile
    {
        public CategoyProfile()
        {
            CreateMap<Categories, CategoryResponseDTO>();
        }
    }
}