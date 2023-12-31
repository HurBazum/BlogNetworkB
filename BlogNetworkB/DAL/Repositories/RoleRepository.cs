﻿using BlogNetworkB.DAL.Enteties;
using BlogNetworkB.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogNetworkB.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        readonly BlogContext _blogContext;
        public RoleRepository(BlogContext blogContext) => _blogContext = blogContext;

        public async Task<Role[]> GetAll() => await _blogContext.Roles.ToArrayAsync();
        public async Task<Role> GetRoleById(int id) => await _blogContext.Roles.FirstOrDefaultAsync(x => x.Id == id);
    }
}