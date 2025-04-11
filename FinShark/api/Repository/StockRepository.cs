using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteSync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if(stockModel == null)
            {
                return  null;
            }
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public  async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
           var stock =  _context.Stocks.Include(c => c.Comments).AsQueryable();
        
           if(!string.IsNullOrWhiteSpace(query.CompanyName))
           {
            stock = stock.Where(s=> s.CompanyName.Contains(query.CompanyName));
           }

           if(!string.IsNullOrWhiteSpace(query.Symbol))
           {
            stock = stock.Where(s=> s.Symbol.Contains(query.Symbol));
           }

           if(!string.IsNullOrWhiteSpace(query.SortBy))
           {
            if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
            {
                stock = query.IsDescsending ? stock.OrderByDescending(s => s.Symbol) : stock.OrderBy(s => s.Symbol);
            }
           }

           var skipNumber = (query.PageNumber - 1)* query.PageSize;

           return await stock.Skip(skipNumber).Take(query.PageSize).ToListAsync();
         }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var excitingstock =  await _context.Stocks.FirstOrDefaultAsync(x => x.Id ==id);

            if(excitingstock == null){
                return null;
            }

            excitingstock.Symbol = stockDto.Symbol;
            excitingstock.CompanyName = stockDto.CompanyName;
            excitingstock.Purchase = stockDto.Purchase;
            excitingstock.LastDiv = stockDto.LastDiv;
            excitingstock.Industry = stockDto.Industry;
            excitingstock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();

            return excitingstock;
        }
    }
}