using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);

    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
        .Where(x => x.SourceUserId == currentUserId)
        .Select(x => x.TargetUserId)
        .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PageList<MemberDto>> GetUserLikes(LikesParam likesParam)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;

        switch (likesParam.Predicate)
        {

            case "liked":
                query = likes
                .Where(x => x.SourceUserId == likesParam.UserId)
                .Select(x => x.TargetUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            case "likeBy":
                query = likes
                .Where(x => x.TargetUserId == likesParam.UserId)
                .Select(x => x.SourceUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            default:
                var likedIds = await GetCurrentUserLikeIds(likesParam.UserId);

                query = likes
                .Where(x => x.TargetUserId == likesParam.UserId && likedIds.Contains(x.SourceUserId))
                .Select(x => x.SourceUser)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }
        return await PageList<MemberDto>.CreateAsync(query, likesParam.PageNumber, likesParam.PageSize);
    }

    public async Task<bool> SaveChange()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
