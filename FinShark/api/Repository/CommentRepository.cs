using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.data;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {  
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
           var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

           if(commentModel == null)
           {
            return null;
           }

           _context.Comments.Remove(commentModel);
           await _context.SaveChangesAsync();
           return commentModel;
        }

        public Task<List<Comment>> GetAllAsync()
        {
            return _context.Comments.Include(a => a.AppUser).ToListAsync();
        }

        public  async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var excistingcomment = await _context.Comments.FindAsync(id);

            if(excistingcomment == null){
                return null;
            }

            excistingcomment.Title = commentModel.Title;
            excistingcomment.Content = commentModel.Content;

            await _context.SaveChangesAsync();

            return excistingcomment;
        }
    }
}